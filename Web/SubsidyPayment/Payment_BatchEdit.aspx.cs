using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ERP.BLL.Implement.Organization;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Environment;
using ERP.UI.Web.Base;
using ERP.Model;
using ERP.SAL;
using ERP.SAL.MemberCenterSAL;
using ERP.UI.Web.Common;
using Keede.Ecsoft.Model;
using OperationLog.Core;
using Telerik.Web.UI;

namespace ERP.UI.Web.SubsidyPayment
{
    /// <summary>
    /// 补贴打款——批量受理
    /// </summary>
    public partial class Payment_BatchEdit : WindowsPage
    {
        //TODO:待定
        private readonly IBankAccounts _bankAccounts = new BankAccounts(GlobalConfig.DB.FromType.Read);
        private readonly IWasteBook _wasteBook = new WasteBook(GlobalConfig.DB.FromType.Write);

        #region 属性
        /// <summary>
        /// 当前登录人信息模型
        /// </summary>
        private PersonnelInfo Personnel
        {
            get
            {
                if (ViewState["Personnel"] == null)
                {
                    ViewState["Personnel"] = CurrentSession.Personnel.Get();
                }
                return (PersonnelInfo)ViewState["Personnel"];
            }
        }

        /// <summary>
        /// 资金账户列表属性
        /// </summary>
        protected Dictionary<Guid, string> BankAccountInfoDic
        {
            get
            {
                if (ViewState["BankAccountInfoDic"] == null)
                {
                    var saleFilialeId = string.IsNullOrEmpty(Request.QueryString["SaleFilialeId"]) ? Guid.Empty.ToString() : Request.QueryString["SaleFilialeId"];
                    var bankAccountInfoDic = _bankAccounts.GetBankAccountsList(new Guid(saleFilialeId), Personnel.FilialeId, Personnel.BranchId, Personnel.PositionId).Where(ent => ent.IsUse).ToDictionary(p => p.BankAccountsId, p => p.BankName + "【" + p.AccountsName + "】");
                    bankAccountInfoDic.Add(new Guid("7175FA90-1214-46E7-8D15-F9053E16928C"), "支付宝【zfbahkd@keede.cn】");
                    return bankAccountInfoDic;
                }
                return ViewState["BankAccountInfoDic"] as Dictionary<Guid, string>;
            }
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadData();
            }
        }

        #region 初始化页面数据
        protected void LoadData()
        {
            var paras = Request.QueryString["Paras"];
            if (paras != null)
            {
                Lit_Count.Text = string.IsNullOrEmpty(Request.QueryString["Count"]) ? "0" : Request.QueryString["Count"];
                Lit_AmountTotal.Text = string.IsNullOrEmpty(Request.QueryString["AmountTotal"]) ? "0" : Request.QueryString["AmountTotal"];
                LoadBankAccountData();
            }
        }
        #endregion

        #region 数据准备
        /// <summary>
        /// 根据销售公司获取资金账号
        /// </summary>
        protected void LoadBankAccountData()
        {
            RCB_BankAccountsId.DataSource = BankAccountInfoDic;
            RCB_BankAccountsId.DataTextField = "Value";
            RCB_BankAccountsId.DataValueField = "Key";
            RCB_BankAccountsId.DataBind();
            RCB_BankAccountsId.Items.Insert(0, new RadComboBoxItem("请选择", Guid.Empty.ToString()));
        }
        #endregion

        #region SelectedIndexChanged事件
        //资金账户搜索
        protected void RCB_BankAccountsId_ItemsRequested(object sender, RadComboBoxItemsRequestedEventArgs e)
        {
            var rcb = (RadComboBox)sender;
            rcb.Items.Clear();
            if (!string.IsNullOrEmpty(e.Text))
            {
                var bankAccountDic = BankAccountInfoDic.Where(p => p.Value.Contains(e.Text));
                Int32 totalCount = BankAccountInfoDic.Count;
                if (e.NumberOfItems >= totalCount)
                    e.EndOfItems = true;
                else
                {
                    foreach (var item in bankAccountDic)
                    {
                        rcb.Items.Add(new RadComboBoxItem(item.Value, item.Key.ToString()));
                    }
                }
            }
        }
        #endregion

        //打款完成
        protected void btn_PayFinish_Click(object sender, EventArgs e)
        {
            #region 验证数据
            if (string.IsNullOrEmpty(RCB_BankAccountsId.SelectedValue))
            {
                MessageBox.Show(this, "请选择“资金账号”!");
                return;
            }
            #endregion

            var idAndSalePlatformIds = Request.QueryString["Paras"].Split(',');
            try
            {
                foreach (var item in idAndSalePlatformIds)
                {
                    var id = item.Split('&')[0];
                    var salePlatformId = item.Split('&')[1];

                    PayFinish(new Guid(salePlatformId), new Guid(id));
                }
                MessageBox.AppendScript(this, "CloseAndRebind();");
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "打款失败!" + ex.Message);
            }
        }

        //退回申请
        protected void btn_ReturnApply_Click(object sender, EventArgs e)
        {
            #region 验证数据
            if (string.IsNullOrEmpty(txt_RefuseReason.Text))
            {
                MessageBox.Show(this, "请填写“拒绝理由”!");
                return;
            }
            #endregion

            var idAndSalePlatformIds = Request.QueryString["Paras"].Split(',');
            try
            {
                foreach (var item in idAndSalePlatformIds)
                {
                    var id = item.Split('&')[0];
                    var salePlatformId = item.Split('&')[1];

                    ReturnApply(new Guid(salePlatformId), new Guid(id));
                }
                MessageBox.AppendScript(this, "CloseAndRebind();");
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "退回失败!" + ex.Message);
            }
        }

        /// <summary>
        /// 打款完成
        /// </summary>
        /// <param name="salePlatformId"></param>
        /// <param name="applyId"></param>
        protected void PayFinish(Guid salePlatformId, Guid applyId)
        {
            var errorMsg = new StringBuilder();
            MemberMentionApplyInfo memberMentionApplyInfo = MemberCenterSao.GetMemberMentionApply(salePlatformId, applyId);
            if (memberMentionApplyInfo.State == (int)MemberMentionState.Process)
            {
                var orderNoStr = string.Empty;
                if (!string.IsNullOrWhiteSpace(memberMentionApplyInfo.OrderNo))
                {
                    orderNoStr = string.Format("[订单号：{0}]", memberMentionApplyInfo.OrderNo);
                }
                if (!string.IsNullOrWhiteSpace(memberMentionApplyInfo.ThirdPartyOrderNo) || memberMentionApplyInfo.ThirdPartyOrderNo == "-")
                {
                    orderNoStr = string.Format("{0}[第三方平台订单号：{1}]", orderNoStr, memberMentionApplyInfo.ThirdPartyOrderNo);
                }
                var rdescription = string.Format("[余额提现(提现会员:{0}；提现申请单号:{1}；提现到:{2}；操作人:{3}),资金减少，{4})]", memberMentionApplyInfo.UserName, memberMentionApplyInfo.ApplyNo, memberMentionApplyInfo.BankName, Personnel.RealName, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                var info = new WasteBookInfo(Guid.NewGuid(), new Guid(RCB_BankAccountsId.SelectedValue),
                    memberMentionApplyInfo.ApplyNo, rdescription + orderNoStr,
                    -memberMentionApplyInfo.Amount, (Int32)AuditingState.Yes,
                    (Int32)WasteBookType.Decrease, memberMentionApplyInfo.SaleFilialeId)
                {
                    LinkTradeCode = memberMentionApplyInfo.ApplyNo,
                    LinkTradeType = (int)WasteBookLinkTradeType.WithdrawDeposit,
                    BankTradeCode = string.Empty,
                    State = (int)WasteBookState.Currently,
                    IsOut = false
                };

                var memo = "提现" + memberMentionApplyInfo.ApplyNo + "取走余额;";
                var description = "提现" + memberMentionApplyInfo.ApplyNo + ",提走余额;" + Common.WebControl.RetrunUserAndTime("提现受理");

                #region 提现
                using (var ts = new TransactionScope(TransactionScopeOption.Required))
                {
                    try
                    {
                        if (info.Income != 0)
                        {
                            _wasteBook.Insert(info);
                        }
                        string errorMessage;
                        var result = MemberCenterSao.CompleteWithdrawApply(salePlatformId, applyId, memo, description, out errorMessage);
                        if (!result)
                        {
                            throw new Exception("会员名：" + memberMentionApplyInfo.UserName + "提现单号：" + memberMentionApplyInfo.ApplyNo + "   " + errorMessage);
                        }
                        //提现打款完成增加操作记录添加
                        Common.WebControl.AddOperationLog(Personnel.PersonnelId, Personnel.RealName, info.WasteBookId, info.TradeCode, OperationPoint.MemberWithdrawCash.PaySuccess.GetBusinessInfo(), string.Empty);
                        ts.Complete();
                    }
                    catch (Exception ex)
                    {
                        errorMsg.Append("提现失败！" + ex.Message).Append("\\n");
                    }
                    finally
                    {
                        ts.Dispose();
                    }
                }
                #endregion

                #region 发送短信
                try
                {
                    MemberBaseInfo memberBaseInfo = MemberCenterSao.GetUserBase(salePlatformId, memberMentionApplyInfo.MemberId);
                    if (memberBaseInfo != null)
                    {
                        if (!string.IsNullOrEmpty(memberBaseInfo.Mobile))
                        {
                            //可得完成打款发送短信
                            var keedeFilialeId = FilialeManager.GetList().First(f => f.Code.ToLower() == "kede").ID;
                            if (memberMentionApplyInfo.SaleFilialeId == keedeFilialeId)
                            {
                                const string MSG = "您好，您的提现申请已收到，现已完成打款，一般2-5个工作日到账，请注意查收。详情可致电4006202020咨询。感谢您对可得网支持！";
                                MailSMSSao.SendShortMessage(memberMentionApplyInfo.SaleFilialeId, memberMentionApplyInfo.SalePlatformId, memberBaseInfo.Mobile, MSG);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    errorMsg.Append("提现成功，短信发送失败！ " + "会员名：" + memberMentionApplyInfo.UserName + "提现单号：" + memberMentionApplyInfo.ApplyNo + "   " + ex.Message).Append("\\n");
                }
                #endregion
            }

            if (!string.IsNullOrEmpty(errorMsg.ToString()))
            {
                throw new Exception(errorMsg.ToString());
            }
        }

        /// <summary>
        /// 退回申请
        /// </summary>
        /// <param name="salePlatformId"></param>
        /// <param name="applyId"></param>
        protected void ReturnApply(Guid salePlatformId, Guid applyId)
        {
            var errorMsg = new StringBuilder();
            MemberMentionApplyInfo memberMentionApplyInfo = MemberCenterSao.GetMemberMentionApply(salePlatformId, applyId);
            if (memberMentionApplyInfo.State == (int)MemberMentionState.Process)
            {
                #region 退回申请
                try
                {
                    string errorMessage;
                    var result = MemberCenterSao.ReturnBackWithdrawApply(salePlatformId, applyId, txt_RefuseReason.Text.Trim(), Common.WebControl.RetrunUserAndTime("提现退回"), out errorMessage);
                    if (!result)
                    {
                        throw new Exception("会员名：" + memberMentionApplyInfo.UserName + "提现单号：" + memberMentionApplyInfo.ApplyNo + "   " + errorMessage);
                    }
                    Common.WebControl.AddOperationLog(Personnel.PersonnelId, Personnel.RealName, memberMentionApplyInfo.Id, memberMentionApplyInfo.ApplyNo, OperationPoint.MemberWithdrawCash.PayRefuse.GetBusinessInfo(), "提现退回");
                }
                catch (Exception ex)
                {
                    errorMsg.Append("退回申请失败！" + ex.Message).Append("\\n");
                }
                #endregion

                #region 发送短信
                try
                {
                    MemberBaseInfo memberBaseInfo = MemberCenterSao.GetUserBase(salePlatformId, memberMentionApplyInfo.MemberId);
                    if (memberBaseInfo != null)
                    {
                        if (!string.IsNullOrEmpty(memberBaseInfo.Mobile))
                        {
                            //百秀退回申请不发送短信
                            var baishopFilialeId = new Guid("444E0C93-1146-4386-BAE2-CB352DA70499");
                            if (memberMentionApplyInfo.SaleFilialeId != baishopFilialeId)
                            {
                                string msg = "您好，您的提现申请已退回，原因：" + txt_RefuseReason.Text + "。详情可致电4006202020咨询。感谢您对可得网支持！";
                                MailSMSSao.SendShortMessage(memberMentionApplyInfo.SaleFilialeId, memberMentionApplyInfo.SalePlatformId, memberBaseInfo.Mobile, msg);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    errorMsg.Append("退回申请成功，短信发送失败！ " + "会员名：" + memberMentionApplyInfo.UserName + "提现单号：" + memberMentionApplyInfo.ApplyNo + "   " + ex.Message).Append("\\n");
                }
                #endregion
            }

            if (!string.IsNullOrEmpty(errorMsg.ToString()))
            {
                throw new Exception(errorMsg.ToString());
            }
        }
    }
}