using System;
using System.Transactions;
using ERP.BLL.Implement;
using ERP.BLL.Implement.Inventory;
using ERP.BLL.Interface;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Environment;
using ERP.SAL;
using ERP.SAL.MemberCenterSAL;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Keede.Ecsoft.Model;
using OperationLog.Core;

namespace ERP.UI.Web.Windows
{
    /// <summary>会员余额管理通过/不通过
    /// </summary>
    public partial class MemberBalanceManageForm : WindowsPage
    {
        private readonly IWasteBook _wasteBook = new WasteBook(GlobalConfig.DB.FromType.Write);
        private static readonly IOperationLogManager _operationLogManager = new OperationLogManager();
        private readonly CodeManager _codeManager = new CodeManager();

        #region [注册提交控制]
        protected SubmitController SubmitController;

        protected SubmitController CreateSubmitInstance()
        {
            if (ViewState["Save"] == null)
            {
                SubmitController = new SubmitController(Guid.NewGuid());
                ViewState["Save"] = SubmitController;
            }
            return (SubmitController)ViewState["Save"];
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            SubmitController = CreateSubmitInstance();
            if (!IsPostBack)
            {
                ApplyId = string.IsNullOrEmpty(Request.QueryString["ApplyId"]) ? Guid.Empty : new Guid(Request.QueryString["ApplyId"]);
                SalePlatformId = string.IsNullOrEmpty(Request.QueryString["SalePlatformId"]) ? Guid.Empty : new Guid(Request.QueryString["SalePlatformId"]);
            }
        }

        /// <summary>确认通过
        /// </summary>
        protected void BtSaveClick(object sender, EventArgs e)
        {
            if (!SubmitController.Enabled)
            {
                return;
            }
            if (ApplyId == Guid.Empty) return;
            var personnelInfo = CurrentSession.Personnel.Get();
            var info = MemberCenterSao.GetMemeberBalanceChangeInfo(ApplyId);
            if (info == null)
            {
                RAM.Alert("温馨提示：未获取到数据，请重试！");
                return;
            }
            if (info.State == (int)MemberBalanceChangeStateEnum.Complete)
            {
                RAM.Alert("温馨提示：数据已完成！");
                return;
            }
            using (var ts = new TransactionScope(TransactionScopeOption.Required))
            {
                //如果是余额充值则记录资金流
                if (info.UserBalanceChangeType == (int)MemberBalanceChangeTypeEnum.BalanceRecharge)
                {
                    if (info.BankAccountId == Guid.Empty)
                    {
                        RAM.Alert("温馨提示：数据异常，银行账号为空！");
                        return;
                    }
                    //修改提现申请状态、受理时间
                    var bankInfo = BankAccountManager.ReadInstance.GetBankAccounts(info.BankAccountId);
                    if (bankInfo == null)
                    {
                        RAM.Alert("温馨提示：数据异常，银行账号为空！");
                        return;
                    }
                    var bankName = bankInfo.BankName + "-" + bankInfo.AccountsName;
                    // var description = WebControl.RetrunUserAndTime("余额充值审核通过,交易号:" + info.TradeCode);
                    var dateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    var des = string.Format("[余额充值(来源银行：{0}；交易流水号：{1}；操作人：{2}),资金增加，{3}]", bankName, info.TradeCode, personnelInfo.RealName, dateTime);
                    //插入资金流
                    var tradeCode = _codeManager.GetCode(CodeType.RF);
                    var wasteBookInfo = new WasteBookInfo(Guid.NewGuid(), info.BankAccountId, tradeCode, des,
                                                          info.Increase, (Int32)AuditingState.Yes,
                                                          (Int32)WasteBookType.Increase, info.SaleFilialeId)
                                            {
                                                LinkTradeCode = tradeCode,
                                                LinkTradeType = (int)WasteBookLinkTradeType.Pay,
                                                BankTradeCode = string.Empty,
                                                State = (int)WasteBookState.Currently,
                                                IsOut = false
                                            };
                    if (wasteBookInfo.Income != 0)
                    {
                        var isSuccess = _wasteBook.Insert(wasteBookInfo) > 0;
                        if (!isSuccess)
                        {
                            RAM.Alert("温馨提示：审核失败，插入资金流异常！");
                            return;
                        }
                    }
                }
                if (info.UserBalanceChangeType == (int)MemberBalanceChangeTypeEnum.BalancePresent || info.UserBalanceChangeType == (int)MemberBalanceChangeTypeEnum.BalanceSubtract)
                {
                    var goodsOrderInfo = OrderSao.GetGoodsOrderInfoByOrderNo(info.SaleFilialeId, info.TradeCode);
                    if (goodsOrderInfo != null && goodsOrderInfo.OrderId != Guid.Empty)
                    {
                        var clew = info.UserBalanceChangeType == (int)MemberBalanceChangeTypeEnum.BalancePresent ? "余额赠送[" + info.Increase + "]" : "余额扣除[" + info.Increase + "]";
                        //添加这条记录到管理系统
                        _operationLogManager.Add(personnelInfo.PersonnelId, personnelInfo.RealName,
                                                 goodsOrderInfo.OrderId, info.TradeCode,
                                                 OperationPoint.OrderProcess.HandAddMemo.GetBusinessInfo(), 1, clew);
                    }
                    else
                    {
                        RAM.Alert("系统提示：未找到对应的订单,无法插入管理意见！");
                        return;
                    }
                }
                string errorMessage;
                var result = MemberCenterSao.CompleteUserBalanceChange(SalePlatformId, ApplyId,
                                                                           personnelInfo.PersonnelId,
                                                                           personnelInfo.RealName, out errorMessage);

                if (!result)
                {
                    RAM.Alert("温馨提示：" + errorMessage);
                    return;
                }
                ts.Complete();
            }
            SubmitController.Submit();
            RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
        }

        /// <summary>确认不通过
        /// </summary>
        protected void BtNoPassClick(object sender, EventArgs e)
        {
            if (!SubmitController.Enabled)
            {
                return;
            }
            if (ApplyId == Guid.Empty) return;
            string description = WebControl.RetrunUserAndTime("[确认不通过]" + TB_Memo.Text);
            var personnelInfo = CurrentSession.Personnel.Get();
            string errorMessage;
            var result = MemberCenterSao.AffirmNoPassUserBalanceChange(SalePlatformId, ApplyId, personnelInfo.PersonnelId, personnelInfo.RealName, description, out errorMessage);
            if (!result)
            {
                RAM.Alert("温馨提示：" + errorMessage);
                return;
            }
            SubmitController.Submit();
            RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
        }

        #region[ViewState]

        /// <summary>申请记录ID
        /// </summary>
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

        /// <summary>销售平台ID
        /// </summary>
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