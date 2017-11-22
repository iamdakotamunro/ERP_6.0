using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using ERP.BLL.Implement.Organization;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Implement.Order;
using ERP.DAL.Interface.IInventory;
using ERP.DAL.Interface.IOrder;
using ERP.Enum;
using ERP.Environment;
using ERP.Model;
using ERP.SAL;
using ERP.SAL.MemberCenterSAL;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Keede.Ecsoft.Model;
using OperationLog.Core;
using Telerik.Web.UI;

namespace ERP.UI.Web.Windows
{
    /// <summary>
    /// 提现受理页面
    /// Add by liucaijun at 2011-October-25th
    /// Modify by zhangfan at 2012-April-28th
    /// 修改内容：增加修改备注
    /// </summary>
    public partial class MemberMentionProcess : WindowsPage
    {
        private readonly IWasteBook _wasteBook = new WasteBook(GlobalConfig.DB.FromType.Write);
        private readonly IBankAccounts _bankAccounts = new BankAccounts(GlobalConfig.DB.FromType.Read);

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
                    var saleFilialeId = MemberCenterSao.GetMemberMentionApply(SalePlatformId, ApplyId).SaleFilialeId;
                    var bankAccountInfoDic = _bankAccounts.GetBankAccountsList(saleFilialeId, Personnel.FilialeId, Personnel.BranchId, Personnel.PositionId).Where(ent => ent.IsUse).ToDictionary(p => p.BankAccountsId, p => p.BankName + "【" + p.AccountsName + "】");
                    bankAccountInfoDic.Add(new Guid("7175FA90-1214-46E7-8D15-F9053E16928C"), "支付宝【zfbahkd@keede.cn】");
                    return bankAccountInfoDic;
                }
                return ViewState["BankAccountInfoDic"] as Dictionary<Guid, string>;
            }
        }
        #endregion

        protected SubmitController SubmitController;

        protected SubmitController CreateSubmitInstance()
        {
            if (ViewState["InsertGoodsOrder"] == null)
            {
                SubmitController = new SubmitController(Guid.NewGuid());
                ViewState["InsertGoodsOrder"] = SubmitController;
            }
            return (SubmitController)ViewState["InsertGoodsOrder"];
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            SubmitController = CreateSubmitInstance();
            if (!IsPostBack)
            {
                if (!string.IsNullOrEmpty(Request.QueryString["ApplyId"]) && !string.IsNullOrEmpty(Request.QueryString["SalePlatformId"]))
                {
                    ApplyId = new Guid(Request.QueryString["ApplyId"]);
                    SalePlatformId = new Guid(Request.QueryString["SalePlatformId"]);
                    BindMemberMentionApply();
                    BT_Save.Enabled = true;
                }
                else
                {
                    BT_Save.Enabled = false;
                }
            }
        }

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

        #region[打款完成]
        protected void BtSaveClick(object sender, EventArgs e)
        {
            if (!SubmitController.Enabled)
            {
                return;
            }
            //判断手续费
            if (String.IsNullOrEmpty(TB_Poundage.Text))
            {
                if (!WebControl.CheckPoundage(Convert.ToDecimal(LB_Amount.Text), Convert.ToDecimal(string.IsNullOrEmpty(TB_Poundage.Text) ? "0" : TB_Poundage.Text)))
                {
                    RAM.Alert("手续费必须小于等于6或者不超过付款金额的2%！");
                    return;
                }
            }
            var transCode = TB_TransCode.Text.Trim() == string.Empty ? "无" : TB_TransCode.Text.Trim();
            var bankName = LB_AccountName.Text.Trim();
            MemberMentionApplyInfo memberMentionApplyInfo = MemberCenterSao.GetMemberMentionApply(SalePlatformId, ApplyId);
            if (memberMentionApplyInfo == null)
            {
                RAM.Alert("该提现申请记录未找到！");
                return;
            }
            if (memberMentionApplyInfo.State == (int)MemberMentionState.Finish)
            {
                RAM.Alert("该提现申请已经完成！");
                return;
            }
            try
            {
                var personnelInfo = CurrentSession.Personnel.Get();
                using (var ts = new TransactionScope(TransactionScopeOption.Required))
                {
                    //修改提现申请状态、受理时间
                    var memo = "提现" + memberMentionApplyInfo.ApplyNo + "取走余额;交易号为:" + transCode + ";";
                    var description = "提现" + memberMentionApplyInfo.ApplyNo + ",提走余额;" + WebControl.RetrunUserAndTime("提现受理") + "";
                    var dateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    var orderNoStr = String.Empty;
                    if (!String.IsNullOrWhiteSpace(memberMentionApplyInfo.OrderNo))
                    {
                        orderNoStr = String.Format("[订单号：{0}]", memberMentionApplyInfo.OrderNo);
                    }
                    if (!String.IsNullOrWhiteSpace(memberMentionApplyInfo.ThirdPartyOrderNo) || memberMentionApplyInfo.ThirdPartyOrderNo == "-")
                    {
                        orderNoStr = String.Format("{0}[第三方平台订单号：{1}]", orderNoStr, memberMentionApplyInfo.ThirdPartyOrderNo);
                    }
                    var rdescription = string.Format("[余额提现(提现会员:{0}；提现申请单号:{1}；提现到:{2}；交易流水号:{3}；操作人:{4}),资金减少，{5})]", memberMentionApplyInfo.UserName, memberMentionApplyInfo.ApplyNo, bankName, transCode, personnelInfo.RealName, dateTime);
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
                    if (info.Income != 0)
                        _wasteBook.Insert(info);
                    if (!string.IsNullOrEmpty(TB_Poundage.Text)) //手续费
                    {
                        var newinfo = new WasteBookInfo(Guid.NewGuid(), new Guid(RCB_BankAccountsId.SelectedValue),
                                                        info.TradeCode,
                                                        "[手续费]" + rdescription,
                                                        -decimal.Parse(TB_Poundage.Text), info.AuditingState,
                                                        (Int32)WasteBookType.Decrease,
                                                        memberMentionApplyInfo.SaleFilialeId)
                        {
                            LinkTradeCode = memberMentionApplyInfo.ApplyNo,
                            LinkTradeType = (int)WasteBookLinkTradeType.WithdrawDeposit,
                            BankTradeCode = string.Empty,
                            State = (int)WasteBookState.Currently,
                            IsOut = info.IsOut
                        };
                        if (newinfo.Income != 0)
                            _wasteBook.Insert(newinfo);
                    }
                    string errorMsg;
                    var result = MemberCenterSao.CompleteWithdrawApply(SalePlatformId, ApplyId, memo, description, out errorMsg);
                    if (!result)
                    {
                        RAM.Alert("温馨提示：" + errorMsg);
                        return;
                    }
                    //提现打款完成增加操作记录添加
                    WebControl.AddOperationLog(personnelInfo.PersonnelId, personnelInfo.RealName, info.WasteBookId, info.TradeCode,
                        OperationPoint.MemberWithdrawCash.PaySuccess.GetBusinessInfo(), string.Empty);
                    ts.Complete();
                }
                try
                {
                    MemberBaseInfo memberBaseInfo = MemberCenterSao.GetUserBase(SalePlatformId, memberMentionApplyInfo.MemberId);
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
                    RAM.Alert("温馨提示：执行成功，但短信发送失败，系统错误信息提示：" + ex.Message);
                }

                foreach (GridDataItem item in RG_Order.SelectedItems)
                {
                    var orderId = new Guid(item.GetDataKeyValue("OrderId").ToString());
                    var orderNo = item.GetDataKeyValue("OrderNo").ToString();
                    string clew = WebControl.RetrunUserAndTime("[余额提现][提现申请单号:" +
                                                               memberMentionApplyInfo.ApplyNo + "从我方帐号：" +
                                                               RCB_BankAccountsId.Text + "提现到：" +
                                                               LB_BankName.Text + "账户名：" + LB_AccountName.Text +
                                                               "帐号：" + LB_BankNo.Text +
                                                               "提现金额：" + LB_Amount.Text +
                                                               "备注：" + TB_Memo.Text + ",交易号：" + transCode + "]");

                    WebControl.AddOperationLog(personnelInfo.PersonnelId, personnelInfo.RealName, orderId, orderNo, OperationPoint.MemberWithdrawCash.PaySuccess.GetBusinessInfo(), clew);
                }
                SubmitController.Submit();
                RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
            }
            catch (Exception ex)
            {
                RAM.Alert("受理失败，系统提示：" + ex.Message);
            }
        }
        #endregion

        #region[退回申请]
        protected void BtBackClick(object sender, EventArgs e)
        {
            try
            {
                MemberMentionApplyInfo memberMentionApplyInfo = MemberCenterSao.GetMemberMentionApply(SalePlatformId, ApplyId);
                string memberMentionApplyClew = WebControl.RetrunUserAndTime("提现退回");
                string memo = TB_Memo.Text.Trim();
                string errorMessage;
                var result = MemberCenterSao.ReturnBackWithdrawApply(SalePlatformId, ApplyId, memo,
                                                  memberMentionApplyClew, out errorMessage);
                if (!result)
                {
                    RAM.Alert("温馨提示：" + errorMessage);
                    return;
                }
                MemberBaseInfo memberBaseInfo = MemberCenterSao.GetUserBase(SalePlatformId, memberMentionApplyInfo.MemberId);
                if (memberBaseInfo != null)
                {
                    if (!string.IsNullOrEmpty(memberBaseInfo.Mobile))
                    {
                        //百秀退回申请不发送短信
                        var baishopFilialeId = new Guid("444E0C93-1146-4386-BAE2-CB352DA70499");
                        if (memberMentionApplyInfo.SaleFilialeId != baishopFilialeId)
                        {
                            string msg = "您好，您的提现申请已退回，原因：" + TB_Memo.Text + "。详情可致电4006202020咨询。感谢您对可得网支持！";
                            MailSMSSao.SendShortMessage(memberMentionApplyInfo.SaleFilialeId, memberMentionApplyInfo.SalePlatformId, memberBaseInfo.Mobile, msg);
                        }
                    }
                }
                var personnelInfo = CurrentSession.Personnel.Get();
                WebControl.AddOperationLog(personnelInfo.PersonnelId, personnelInfo.RealName, memberMentionApplyInfo.Id, memberMentionApplyInfo.ApplyNo,
                        OperationPoint.MemberWithdrawCash.PayRefuse.GetBusinessInfo(), "提现退回");
                foreach (GridDataItem item in RG_Order.SelectedItems)
                {
                    var orderId = new Guid(item.GetDataKeyValue("OrderId").ToString());
                    var orderNo = item.GetDataKeyValue("OrderNo").ToString();
                    string clew = WebControl.RetrunUserAndTime("[余额提现][退回申请][提现申请单号:" +
                                                               memberMentionApplyInfo.ApplyNo + "提现到：" +
                                                               LB_BankName.Text + "账户名：" + LB_AccountName.Text +
                                                               "帐号：" + LB_BankNo.Text +
                                                               "提现金额：" + LB_Amount.Text +
                                                               "备注：" + TB_Memo.Text + "]");

                    WebControl.AddOperationLog(personnelInfo.PersonnelId, personnelInfo.RealName, orderId,
                        orderNo, OperationPoint.MemberWithdrawCash.PayRefuse.GetBusinessInfo(), clew);
                }
                RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
            }
            catch (Exception ex)
            {
                RAM.Alert("同步失败，系统提示：" + ex.Message);
            }
        }
        #endregion

        #region[绑定提现信息]

        /// <summary>
        /// 绑定提现信息
        /// </summary>
        public void BindMemberMentionApply()
        {
            MemberMentionApplyInfo memberMentionApplyInfo = MemberCenterSao.GetMemberMentionApply(SalePlatformId, ApplyId);
            LB_Member.Text = memberMentionApplyInfo.UserName;
            LB_Balance.Text = WebControl.NumberSeparator(memberMentionApplyInfo.Balance.ToString("#.00"));
            LB_Amount.Text = memberMentionApplyInfo.Amount.ToString("#.00");
            TB_Memo.Text = string.Empty;
            LB_BankName.Text = memberMentionApplyInfo.BankName;
            LB_AccountName.Text = memberMentionApplyInfo.BankAccountName;
            LB_BankNo.Text = memberMentionApplyInfo.BankAccounts;
            var filialeInfo = CacheCollection.Filiale.GetList().ToList().FirstOrDefault(p => p.ID == memberMentionApplyInfo.SaleFilialeId) ?? new FilialeInfo();
            Lbl_Website.Text = filialeInfo.Name;

            LoadBankAccountData();
        }
        #endregion

        #region[绑定数据源]
        protected void RgOrderNeedDataSource(object sender, EventArgs e)
        {

        }
        #endregion

        #region [ViewState]

        protected Guid ApplyId
        {
            get
            {
                if (ViewState["ApplyId"] == null)
                {
                    return Guid.Empty;
                }
                return new Guid(ViewState["ApplyId"].ToString());
            }
            set
            {
                ViewState["ApplyId"] = value;
            }
        }

        protected Guid SalePlatformId
        {
            get
            {
                if (ViewState["SalePlatformId"] == null)
                {
                    return Guid.Empty;
                }
                return new Guid(ViewState["SalePlatformId"].ToString());
            }
            set
            {
                ViewState["SalePlatformId"] = value;
            }
        }

        #endregion
    }
}
