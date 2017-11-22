using System;
using System.Transactions;
using ERP.Cache;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Implement.Order;
using ERP.DAL.Interface.IInventory;
using ERP.DAL.Interface.IOrder;
using ERP.Enum;
using ERP.Enum.Attribute;
using ERP.Environment;
using ERP.Model;
using ERP.SAL;
using ERP.SAL.MemberCenterSAL;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Framework.ThirdParty.AliPay;
using Framework.ThirdParty.AliPay.Enum;
using Keede.Ecsoft.Model;
using OperationLog.Core;
using Telerik.Web.UI;

namespace ERP.UI.Web.Windows
{
    /// <summary>待付款确认  
    /// </summary>
    public partial class AffirmSum : WindowsPage
    {
        private readonly IGoodsOrder _order = new GoodsOrder(GlobalConfig.DB.FromType.Read);
        private readonly SynchronousData _sync = SynchronousData.Instance;
        private readonly IWasteBook _wasteBook = new WasteBook(GlobalConfig.DB.FromType.Write);
        SubmitController _submitController;
        private readonly IPaymentNotice _paymentNoticeDao = new PaymentNotice(GlobalConfig.DB.FromType.Write);

        protected SubmitController CreateSubmitInstance()
        {
            if (ViewState["SubmitController"] == null)
            {
                _submitController = new SubmitController(Guid.NewGuid());
                ViewState["SubmitController"] = _submitController;
            }
            return (SubmitController)ViewState["SubmitController"];
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            _submitController = CreateSubmitInstance();
            if (!Page.IsPostBack)
            {
                PayId = new Guid(Request.QueryString["PayId"]);
                PaymentNoticeInfo pninfo = _paymentNoticeDao.GetPayNoticInfoByPayid(PayId);
                if (pninfo == null)
                {
                    RAM.Alert("参数异常，数据没有！");
                    RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
                    return;
                }

                OrderId = pninfo.OrderId;
                GoodsOrderInfoFromB2C = GetGoodsOrderInfoFromB2C(pninfo);
                if (GoodsOrderInfoFromB2C == null || GoodsOrderInfoFromB2C.OrderId == Guid.Empty)
                {
                    RAM.Alert("从独立后台数据加载订单失败!");
                    RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
                    return;
                }

                GoodsOrderInfo goodsOrderInfo = GoodsOrderInfoFromB2C;

                Lit_TotalPrice.Text = WebControl.NumberSeparator(goodsOrderInfo.TotalPrice);
                Lit_Carriage.Text = goodsOrderInfo.Carriage.ToString("0.##");
                Lit_VoucherValue.Text = goodsOrderInfo.PromotionValue.ToString("0.##");
                Lit_PaymentByBalance.Text = WebControl.NumberSeparator(goodsOrderInfo.PaymentByBalance);
                TBox_RealTotalPrice.Text = goodsOrderInfo.RealTotalPrice.ToString("0.##");
                lab_bank.Text = "到款银行:" + pninfo.PayBank;
                lab_time.Text = "付款时间:" + pninfo.PayTime;
                lab_amount.Text = string.Format("付款金额:{0}", WebControl.NumberSeparator(pninfo.PayPrince));
                lab_name.Text = "付款人:" + pninfo.PayName;
                TBox_PaidUp.Text = pninfo.PayPrince.ToString("0.##");

                var bankAccounts = BankAccount.Instance.Get(goodsOrderInfo.BankAccountsId);
                if (bankAccounts != null)
                {
                    RCB_BankAccountsId.Items.Add(new RadComboBoxItem(bankAccounts.BankName + "-" + bankAccounts.AccountsName, bankAccounts.BankAccountsId.ToString()));
                }

                RCB_BankAccountsId.SelectedValue = string.Format("{0}", goodsOrderInfo.BankAccountsId);
                RCB_BankAccountsId.Enabled = false;
                GetGoodsOrder();
                ButtonEnabled();
            }
        }

        #region 取得用户操作权限
        /// <summary>
        /// 取得用户操作权限
        /// </summary>
        protected bool GetPowerOperationPoint(string powerName)
        {
            string pageName = WebControl.FileName;
            return WebControl.GetPowerOperationPoint(pageName, powerName);
        }
        #endregion

        protected Guid PayId
        {
            get { return new Guid(ViewState["PayId"].ToString()); }
            set { ViewState["PayId"] = value.ToString(); }
        }
        protected Guid OrderId
        {
            get { return new Guid(ViewState["OrderId"].ToString()); }
            set { ViewState["OrderId"] = value.ToString(); }
        }

        /// <summary>
        /// 从新KEEDE里获取新订单的信息
        /// </summary>
        protected GoodsOrderInfo GoodsOrderInfoFromB2C
        {
            get { return (GoodsOrderInfo)ViewState["GoodsOrderInfoFromB2C"]; }
            set { ViewState["GoodsOrderInfoFromB2C"] = value; }
        }

        #region 绑定一条记录(从新KEEDE里获取)
        /// <summary>
        /// 从新KEEDE里获取新订单的信息
        /// </summary>
        protected void GetGoodsOrder()
        {
            if (GoodsOrderInfoFromB2C != null && GoodsOrderInfoFromB2C.OrderId != Guid.Empty)//如果有记录取第一条记录
            {
                Lbl_OrderNo.Text = GoodsOrderInfoFromB2C.OrderNo;
                Lbl_Consignee.Text = GoodsOrderInfoFromB2C.Consignee;
                Lbl_OrderState.Text = EnumAttribute.GetKeyName((OrderState)GoodsOrderInfoFromB2C.OrderState);
                Lbl_PayMode.Text = EnumAttribute.GetKeyName((PayState)GoodsOrderInfoFromB2C.PayState);
                Lbl_RealTotalPrice.Text = WebControl.NumberSeparator(GoodsOrderInfoFromB2C.RealTotalPrice);
                Lbl_TotalPrice.Text = WebControl.NumberSeparator(GoodsOrderInfoFromB2C.TotalPrice);
            }
        }

        /// <summary>
        /// 如果是已支付的,显示删除按钮,如果未支付,隐藏删除按钮
        /// </summary>
        public void ButtonEnabled()
        {
            if (GoodsOrderInfoFromB2C != null && GoodsOrderInfoFromB2C.OrderId != Guid.Empty)
            {
                if (GoodsOrderInfoFromB2C.PayState == (int)PayState.Paid)//已支付
                {
                    IBAffirm.Visible = false;
                    IBCancel.Visible = false;
                    TBox_Description.Enabled = false;
                    TBox_PaidUp.Enabled = false;
                    TBox_RealTotalPrice.Enabled = false;
                    //RCB_BankAccountsId.Enabled = false;
                    IBDelete.Visible = true;
                    LblAlert.Visible = true;
                    LblAlert.Text = "注:此订单已支付,不需要确认,请删除这条到款通知!";
                }
                else
                {
                    IBAffirm.Visible = true;
                    IBCancel.Visible = true;
                    TBox_Description.Enabled = true;
                    TBox_PaidUp.Enabled = true;
                    TBox_RealTotalPrice.Enabled = true;
                    //RCB_BankAccountsId.Enabled = true;
                    IBDelete.Visible = false;
                    LblAlert.Visible = false;
                    LblAlert.Text = "";
                }
            }
        }
        #endregion

        private GoodsOrderInfo GetGoodsOrderInfoFromB2C(PaymentNoticeInfo pninfo)
        {
            return OrderSao.GetGoodsOrderInfo(pninfo.SaleFilialeId, OrderId);
        }

        /// <summary>
        /// 删除的步骤:
        /// 1.查这个订单详情是否同步到总后台
        /// 1.1.如果未同步,同步此订单信息,
        /// 1.2.如果同步了,不做同步
        /// 2.修改这到款通知的状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Button_Delete(object sender, EventArgs e)
        {
            string orderno = Lbl_OrderNo.Text.Trim();
            if (!string.IsNullOrEmpty(orderno) && _order.IsOrderNo(orderno))//订单已在总后台
            {
                _paymentNoticeDao.UpdatePayNoticState(PayId, PayState.Paid, string.Empty);//修改到款通知的状态
            }
            else//不在总后台
            {
                RAM.Alert("此订单还未同步到总后台,请先从独立后台同步到总后台再删除这条到款通知!");
                return;
            }
            RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
        }

        protected void Button_Cancel(object sender, EventArgs e)
        {
            PaymentNoticeInfo pninfo = _paymentNoticeDao.GetPayNoticInfoByPayid(PayId);
            if (pninfo == null)
            {
                RAM.Alert("参数异常，数据没有！");
                return;
            }
            GoodsOrderInfo goodsOrderInfo = GetGoodsOrderInfoFromB2C(pninfo);//从分后台取得数据
            if (goodsOrderInfo == null || goodsOrderInfo.OrderId == Guid.Empty)
            {
                RAM.Alert("获取不到独立后台数据!暂时无法操作!");
                return;
            }
            if (goodsOrderInfo.PayState == (int)PayState.Paid)
            {
                RAM.Alert("这个订单已支付!");
                return;
            }
            string clew = "[财务确认]付款未到 " + DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            _paymentNoticeDao.UpdatePayNoticState(PayId, PayState.NoPay, clew);
            goodsOrderInfo.PayState = (int)PayState.NoPay;//订单修改为已支付
            _sync.SyncGoodsOrderUpdateAffirm(goodsOrderInfo.SaleFilialeId, goodsOrderInfo);
            //代付款确认操作记录添加
            var personnelInfo = CurrentSession.Personnel.Get();
            WebControl.AddOperationLog(personnelInfo.PersonnelId, personnelInfo.RealName, pninfo.PayId, pninfo.OrderNo,
                OperationPoint.WaitPaymentConfirmation.PaymentNoCome.GetBusinessInfo(), string.Empty);
        }

        protected void Button_Affirm(object sender, EventArgs e)
        {
            var strBankAccountId = RCB_BankAccountsId.SelectedValue;
            if (!_submitController.Enabled)
            {
                RAM.Alert("程序正在处理中，请稍候...");
                return;
            }
            if (string.IsNullOrEmpty(TBox_PaidUp.Text))
            {
                RAM.Alert("请输入实际收入!");
                return;
            }
            if (string.IsNullOrEmpty(strBankAccountId) || strBankAccountId == Guid.Empty.ToString())
            {
                RAM.Alert("请选择资金账户！");
                return;
            }
            var realTotalPrice = Convert.ToDecimal(TBox_RealTotalPrice.Text);
            var paidUp = Convert.ToDecimal(TBox_PaidUp.Text);
            var selectedBankAccountsId = new Guid(strBankAccountId);

            #region

            var pninfo = _paymentNoticeDao.GetPayNoticInfoByPayid(PayId);
            if (pninfo == null)
            {
                RAM.Alert("未找到当前订单的到付款通知单据！");
                return;
            }
            var goodsOrderInfo = GetGoodsOrderInfoFromB2C(pninfo);
            if (goodsOrderInfo == null || goodsOrderInfo.OrderId == Guid.Empty)
            {
                RAM.Alert("从独立后台数据加载订单失败!");
                RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
                return;
            }
            var dateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            var personnelInfo = CurrentSession.Personnel.Get();
            var description = string.Format("[订单付款（订单号：{0}），资金增加，{1}][{2}]", goodsOrderInfo.OrderNo, dateTime, TBox_Description.Text);

            //如果订单是已支付 状态是未审核 不作任何操作
            if (((PayState)goodsOrderInfo.PayState == PayState.Wait || (PayState)goodsOrderInfo.PayState == PayState.NoPay) &&
            goodsOrderInfo.OrderState == (int)OrderState.UnVerify)
            {
                #region

                try
                {
                    goodsOrderInfo.BankAccountsId = selectedBankAccountsId;
                    //当前实际支付+实际已支付-订单实际应收金额
                    decimal residualPayment = paidUp + goodsOrderInfo.PaidUp - goodsOrderInfo.RealTotalPrice - goodsOrderInfo.Carriage;

                    if (residualPayment >= 0)
                    {
                        goodsOrderInfo.PaidUp = goodsOrderInfo.RealTotalPrice;
                        goodsOrderInfo.PayState = (int)PayState.Paid; //订单修改为已支付
                    }
                    else
                    {
                        goodsOrderInfo.PaidUp = paidUp + goodsOrderInfo.PaidUp;
                        goodsOrderInfo.PayState = (int)PayState.NoPay;
                    }
                    var bankAccountsInfo = BankAccount.Instance.Get(selectedBankAccountsId);
                    bool isTradeStatus;
                    var isAliPay = IsAliPay(goodsOrderInfo.SaleFilialeId, goodsOrderInfo.OrderId, bankAccountsInfo, out isTradeStatus);

                    if (isTradeStatus)
                    {
                        //银行入收入支出类,实收入账
                        var wasteBookInfo = new WasteBookInfo(Guid.NewGuid(),
                                                              goodsOrderInfo.BankAccountsId,
                                                              goodsOrderInfo.OrderNo,
                                                              string.Format("{0}[操作人：{1}]", description, personnelInfo.RealName),
                                                              paidUp,
                                                              (int)AuditingState.Yes,
                                                              (int)WasteBookType.Increase,
                                                              goodsOrderInfo.SaleFilialeId)
                                                {
                                                    LinkTradeCode = goodsOrderInfo.OrderNo,
                                                    LinkTradeType = (int)WasteBookLinkTradeType.GoodsOrder,
                                                    BankTradeCode = string.Empty,
                                                    State = (int)WasteBookState.Currently,
                                                    IsOut = goodsOrderInfo.IsOut
                                                };

                        using (var ts = new TransactionScope(TransactionScopeOption.Required))
                        {
                            if (isAliPay)//如果支付宝已支付的,删除待付款确认单子,把分后台的支付流水改成已支付
                            {
                                bool isPassToNewKeede = OrderSao.UpdatePayStateByOrderIdAndStateBool(goodsOrderInfo.SaleFilialeId, goodsOrderInfo.OrderId, PayState.Paid, GoodsOrderPayState.Enabled);
                                if (!isPassToNewKeede)
                                {
                                    RAM.Alert("待付款确认异常，请再试一次");
                                    return;
                                }
                                _paymentNoticeDao.Delete(PayId);
                            }
                            else
                            {
                                _paymentNoticeDao.UpdatePayNoticState(PayId, PayState.Paid, "[财务确认]收款:" + paidUp + " 应收:" + realTotalPrice + " 存入会员账户:" + residualPayment + " " + DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            }
                            if (wasteBookInfo.Income != 0)
                                _wasteBook.Insert(wasteBookInfo); //插入资金流
                            if (residualPayment > 0)
                            {
                                var remark = "订单" + goodsOrderInfo.OrderNo + "支付超过金额；操作人：[" + CurrentSession.Personnel.Get().RealName + "]";
                                string errorMsg;
                                var result = MemberCenterSao.ReturnUserBalanceByPayOrder(goodsOrderInfo.SaleFilialeId, goodsOrderInfo.SalePlatformId, goodsOrderInfo.MemberId, goodsOrderInfo.OrderId, goodsOrderInfo.OrderNo, residualPayment, remark, out errorMsg);
                                if (!result)
                                {
                                    RAM.Alert("温馨提示：" + errorMsg);
                                    return;
                                }
                            }
                            //代付款确认操作记录添加
                            WebControl.AddOperationLog(personnelInfo.PersonnelId, personnelInfo.RealName, pninfo.OrderId, pninfo.OrderNo, OperationPoint.WaitPaymentConfirmation.WaitPayConfirmation.GetBusinessInfo(), string.Empty);
                            ts.Complete();
                        }
                        _sync.SyncGoodsOrderUpdateAffirm(goodsOrderInfo.SaleFilialeId, goodsOrderInfo); //由于分后台客服未受理 订单未同步到总后台,修改分后台信息
                    }
                    else
                    {
                        RAM.Alert("本交易收款银行状态未到款！");
                        return;
                    }
                    RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
                }
                catch (Exception exp)
                {
                    RAM.Alert("1.订单受理失败，订单状态修改失败！\\n\\n系统提示：" + exp.Message);
                    return;
                }

                #endregion
            }
            else if (((PayState)goodsOrderInfo.PayState == PayState.Wait || (PayState)goodsOrderInfo.PayState == PayState.NoPay) &&
            (goodsOrderInfo.OrderState == (int)OrderState.Approved || goodsOrderInfo.OrderState == (int)OrderState.WaitForPay))
            {
                #region
                try
                {
                    goodsOrderInfo.BankAccountsId = selectedBankAccountsId;
                    //当前实际支付+实际已支付-订单实际应收金额
                    decimal residualPayment = paidUp + goodsOrderInfo.PaidUp - goodsOrderInfo.RealTotalPrice;
                    if (residualPayment >= 0)
                    {
                        goodsOrderInfo.PaidUp = goodsOrderInfo.RealTotalPrice;
                        goodsOrderInfo.OrderState = (int)OrderState.StockUp;
                        goodsOrderInfo.PayState = (int)PayState.Paid; //订单修改为已支付
                        goodsOrderInfo.EffectiveTime = DateTime.Now;
                    }
                    else
                    {
                        goodsOrderInfo.PaidUp = paidUp + goodsOrderInfo.PaidUp;
                        goodsOrderInfo.PayState = (int)PayState.NoPay;
                    }
                    BankAccountInfo bankAccountsInfo = BankAccount.Instance.Get(selectedBankAccountsId);
                    bool isTradeStatus;
                    var isAliPay = IsAliPay(goodsOrderInfo.SaleFilialeId, goodsOrderInfo.OrderId, bankAccountsInfo, out isTradeStatus);
                    if (isTradeStatus)
                    {
                        //银行入收入支出类,实收入账
                        var wasteBookInfo = new WasteBookInfo(Guid.NewGuid(), goodsOrderInfo.BankAccountsId, goodsOrderInfo.OrderNo, string.Format("{0}[操作人：{1}]", description, personnelInfo.RealName), paidUp, (int)AuditingState.Yes, (int)WasteBookType.Increase, goodsOrderInfo.SaleFilialeId)
                                                {
                                                    LinkTradeCode = goodsOrderInfo.OrderNo,
                                                    LinkTradeType = (int)WasteBookLinkTradeType.GoodsOrder,
                                                    BankTradeCode = string.Empty,
                                                    State = (int)WasteBookState.Currently,
                                                    IsOut = goodsOrderInfo.IsOut
                                                };


                        using (var ts = new TransactionScope(TransactionScopeOption.Required))
                        {
                            if (isAliPay)//如果支付宝已支付的,删除待付款确认单子
                            {
                                var isPassToNewKeede = OrderSao.UpdatePayStateByOrderIdAndStateBool(goodsOrderInfo.SaleFilialeId, goodsOrderInfo.OrderId, PayState.Paid, GoodsOrderPayState.Enabled);
                                if (!isPassToNewKeede)
                                {
                                    RAM.Alert("待付款确认异常，请再试一次");
                                    return;
                                }
                                _paymentNoticeDao.Delete(PayId);
                            }
                            else
                            {
                                _paymentNoticeDao.UpdatePayNoticState(PayId, PayState.Paid, "[财务确认]收款:" + paidUp + " 应收:" + realTotalPrice + " 存入会员账户:" + residualPayment + " " + DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            }

                            if (wasteBookInfo.Income != 0)
                            {
                                _wasteBook.Insert(wasteBookInfo); //插入资金流
                            }

                            if (residualPayment > 0)
                            {
                                var remark = "订单" + goodsOrderInfo.OrderNo + "支付超过金额；操作人：[" + personnelInfo.RealName + "]";
                                string errorMsg;
                                var result = MemberCenterSao.ReturnUserBalanceByPayOrder(goodsOrderInfo.SaleFilialeId, goodsOrderInfo.SalePlatformId, goodsOrderInfo.MemberId, goodsOrderInfo.OrderId, goodsOrderInfo.OrderNo, residualPayment, remark, out errorMsg);
                                if (!result)
                                {
                                    RAM.Alert("温馨提示：" + errorMsg);
                                    return;
                                }
                            }
                            //代付款确认操作记录添加
                            WebControl.AddOperationLog(personnelInfo.PersonnelId, personnelInfo.RealName, pninfo.OrderId, pninfo.OrderNo, OperationPoint.WaitPaymentConfirmation.WaitPayConfirmation.GetBusinessInfo(), string.Empty); ts.Complete();
                        }
                        //不把快递单号同步到其他平台
                        goodsOrderInfo.ExpressNo = null;
                        _sync.SyncGoodsOrderModify(goodsOrderInfo.SaleFilialeId, goodsOrderInfo);
                    }
                    else
                    {
                        RAM.Alert("本交易收款银行状态未到款！");
                        return;
                    }
                    RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
                }
                catch (Exception exp)
                {
                    RAM.Alert("2.订单受理失败，订单状态修改失败！\\n\\n系统提示：" + exp.Message);
                    return;
                }

                #endregion
            }
            else if ((goodsOrderInfo.OrderState == (int)OrderState.Cancellation) && goodsOrderInfo.PayState != (int)PayState.Paid)
            {

                #region

                try
                {
                    decimal residualPayment = paidUp;
                    //银行入收入支出类,实收入账
                    var wasteBookInfo = new WasteBookInfo(Guid.NewGuid(),
                                                          selectedBankAccountsId,
                                                          goodsOrderInfo.OrderNo,
                                                          string.Format("{0}[操作人：{1}]", description, personnelInfo.RealName), paidUp,
                                                          (int)AuditingState.Yes,
                                                          (int)WasteBookType.Increase, goodsOrderInfo.SaleFilialeId)
                                            {
                                                LinkTradeCode = goodsOrderInfo.OrderNo,
                                                LinkTradeType = (int)WasteBookLinkTradeType.GoodsOrder,
                                                BankTradeCode = string.Empty,
                                                State = (int)WasteBookState.Currently,
                                                IsOut = goodsOrderInfo.IsOut
                                            };

                    using (var ts = new TransactionScope(TransactionScopeOption.Required))
                    {
                        _paymentNoticeDao.UpdatePayNoticState(PayId, PayState.Paid,"[财务确认]收款:" + residualPayment + "订单已取消 应收:" + 0 +" 存入会员账户:" + residualPayment + " " +DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                        if (residualPayment > 0)
                        {
                            if (wasteBookInfo.Income != 0)
                                _wasteBook.Insert(wasteBookInfo); //插入资金流
                            var remark = description + goodsOrderInfo.OrderNo + "[订单取消,余额冲值]；操作人：[" + personnelInfo.RealName + "]";
                            string errorMsg;
                            var result = MemberCenterSao.ReturnUserBalanceByPayOrder(goodsOrderInfo.SaleFilialeId, goodsOrderInfo.SalePlatformId, goodsOrderInfo.MemberId, goodsOrderInfo.OrderId, goodsOrderInfo.OrderNo, residualPayment, remark, out errorMsg);
                            if (!result)
                            {
                                RAM.Alert("温馨提示：" + errorMsg);
                                return;
                            }
                        }
                        //代付款确认操作记录添加
                        WebControl.AddOperationLog(personnelInfo.PersonnelId, personnelInfo.RealName, pninfo.OrderId, pninfo.OrderNo,OperationPoint.WaitPaymentConfirmation.WaitPayConfirmation.GetBusinessInfo(), string.Empty);
                        ts.Complete();
                    }
                    RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
                }
                catch (Exception exp)
                {
                    RAM.Alert("3.订单受理失败，订单状态修改失败！\\n\\n系统提示：" + exp.Message);
                    return;
                }

                #endregion
            }
            else
            {
                RAM.Alert("该订单已支付,请核实!");
                return;
            }

            #endregion

            _submitController.Submit();
        }
        private Boolean IsAliPay(Guid saleFilialeId, Guid orderId, BankAccountInfo bankAccountsInfo, out Boolean tradeStatus)
        {
            tradeStatus = true;
            //当前银行账户支付接口为支付宝支付时
            if (bankAccountsInfo.PaymentInterfaceId == new Guid("d2d05b15-6ebd-4b06-a576-c2bcb6f5627f"))
            {
                var info = _sync.GetGoodsOrderPayInfoByOrderIdStatePayState(saleFilialeId, orderId, GoodsOrderPayState.Enabled);
                if (info == null)
                {
                    tradeStatus = false;
                }
                else
                {
                    var aliPay = new Payment();
                    aliPay.GetTradeNo(bankAccountsInfo.Accounts.Trim(), info.PaidNo, bankAccountsInfo.AccountsKey.Trim());
                    if (aliPay.BankTradeStatus != TradeState.TRADE_FINISHED.ToString()
                        && aliPay.BankTradeStatus != TradeState.TRADE_SUCCESS.ToString()
                        && aliPay.BankTradeStatus != TradeState.WAIT_SELLER_SEND_GOODS.ToString())
                        tradeStatus = false;
                }
                return true;
            }
            return false;
        }
    }
}