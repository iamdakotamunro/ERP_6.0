using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Transactions;
using ERP.BLL.Implement;
using ERP.BLL.Implement.Organization;
using ERP.Cache;
using ERP.DAL.Implement.Company;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Enum.Attribute;
using ERP.Environment;
using ERP.Model;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Keede.Ecsoft.Model;
using Lonme.WebControls;
using OperationLog.Core;
using Telerik.Web.UI;

namespace ERP.UI.Web.Windows
{
    ///<summary>
    /// 往来单位收付款审核页面
    /// modify by liangcanren at 2015-04-15  去除银行的选择
    ///</summary>
    public partial class CheckReceiptDetail : WindowsPage
    {
        protected CompanyFundReceiptInfo CompanyFundReceiptInfoModel;
        readonly PersonnelManager _personnelManager = new PersonnelManager();
        readonly CompanyBankAccountBindDao _companyBankAccountBindDao = new CompanyBankAccountBindDao(GlobalConfig.DB.FromType.Read);
        readonly IReckoning _reckoning = new Reckoning(GlobalConfig.DB.FromType.Read);
        readonly ICompanyCussent _companyCussent = new CompanyCussent(GlobalConfig.DB.FromType.Read);
        readonly ICompanyFundReceipt _companyFundReceipt = new DAL.Implement.Inventory.CompanyFundReceipt(GlobalConfig.DB.FromType.Write);
        private readonly IStorageRecordDao _storageRecordDao = new StorageRecordDao(GlobalConfig.DB.FromType.Write);
        //其他公司
        private readonly Guid _reckoningElseFilialeid = new Guid(ConfigManage.GetConfigValue("RECKONING_ELSE_FILIALEID"));
        readonly ICompanyFundReceiptInvoice _companyFundReceiptInvoice = new CompanyFundReceiptInvoice(GlobalConfig.DB.FromType.Write);

        List<CompanyFundReceiptInvoiceInfo> _companyFundReceiptInvoiceInfoList = new List<CompanyFundReceiptInvoiceInfo>();
        #region[页面加载]
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CompanyFundReceiptInfoModel = _companyFundReceipt.GetCompanyFundReceiptInfo(RceiptId);
                if (CompanyFundReceiptInfoModel != null)
                {
                    _companyFundReceiptInvoiceInfoList = _companyFundReceiptInvoice.Getlmshop_CompanyFundReceiptInvoiceByReceiptID(RceiptId);
                    GetCompanyList();
                    BindValue();
                    RCB_CompanyList.Enabled = false;
                }
            }
        }
        #endregion

        #region  门店公司信息列表

        protected IList<FilialeInfo> ShopFilialeList
        {
            get
            {
                return CacheCollection.Filiale.GetShopList();
            }
        }
        #endregion

        #region[获取公司数据信息，包含供应商和物流公司]
        /// <summary>
        /// 获取公司数据信息，包含供应商和物流公司
        /// </summary>
        protected void GetCompanyList()
        {
            var dataSource = _companyCussent.GetCompanyCussentList(State.Enable).ToList();
            var data = dataSource.Where(act => act.CompanyType == (int)CompanyType.Suppliers
                  || act.CompanyType == (int)CompanyType.Express || act.RelevanceFilialeId != Guid.Empty).ToDictionary(k => k.CompanyId, v => v.CompanyName);

            if (ShopFilialeList.Count > 0)
            {
                foreach (var item in ShopFilialeList)
                {
                    if (data.ContainsKey(item.ID)) continue;
                    data.Add(item.ID, item.Name);
                }
            }
            RCB_CompanyList.DataSource = data;
            RCB_CompanyList.DataTextField = "Value";
            RCB_CompanyList.DataValueField = "Key";
            RCB_CompanyList.DataBind();
            RCB_CompanyList.Items.Add(new RadComboBoxItem("", Guid.Empty.ToString()));
            RCB_CompanyList.SelectedValue = Guid.Empty.ToString();
            var companyId = CompanyFundReceiptInfoModel.CompanyID;
            var companyCussentInfo = _companyCussent.GetCompanyCussent(companyId);
            if (companyCussentInfo == null)
            {
                if (ShopFilialeList.All(act => act.ID != companyId))
                {
                    RAM.Alert("不是有效的往来单位信息");
                    return;
                }
            }
            var filialeIds = CacheCollection.Filiale.GetHostingAndSaleFilialeList();
            if (filialeIds.Any())
            {
                ShowFiliale(filialeIds);
            }
            else
            {
                RCB_FilialeList.Items.Clear();
                RCB_FilialeList.Items.Add(new RadComboBoxItem("ERP", _reckoningElseFilialeid.ToString()));
                RCB_FilialeList.Items.Add(new RadComboBoxItem(string.Empty, Guid.Empty.ToString()));
                RCB_FilialeList.SelectedValue = CompanyFundReceiptInfoModel != null ? CompanyFundReceiptInfoModel.FilialeId.ToString() : Guid.Empty.ToString();
            }
        }
        #endregion

        #region[显示对应付款公司]
        protected void ShowFiliale(IList<FilialeInfo> filialeIds)
        {
            foreach (var filialeInfo in filialeIds)
            {
                if (filialeInfo != null)
                {
                    RCB_FilialeList.Items.Add(new RadComboBoxItem(filialeInfo.Name, filialeInfo.ID.ToString()));
                }
            }
            RCB_FilialeList.Items.Add(new RadComboBoxItem("ERP", _reckoningElseFilialeid.ToString()));
            RCB_FilialeList.Items.Add(new RadComboBoxItem(string.Empty, Guid.Empty.ToString()));
            RCB_FilialeList.SelectedValue = CompanyFundReceiptInfoModel != null ? CompanyFundReceiptInfoModel.FilialeId.ToString() : Guid.Empty.ToString();
        }
        #endregion

        #region[收付款单据ID]
        protected Guid RceiptId
        {
            get
            {
                if (Request.QueryString["RceiptId"] != null)
                {
                    if (MethodHelp.CheckGuid(Request.QueryString["RceiptId"]))

                        return new Guid(Request.QueryString["RceiptId"]);
                }
                return Guid.Empty;
            }
        }
        #endregion

        #region[审核通过]
        protected void Through_Click(object sender, EventArgs e)
        {
            try
            {
                var filialeId = new Guid(RCB_FilialeList.SelectedValue);
                if (filialeId == Guid.Empty)
                {
                    RAM.Alert("系统提示：请选择公司！");
                    return;
                }
                CompanyFundReceiptInfoModel = _companyFundReceipt.GetCompanyFundReceiptInfo(RceiptId);
                if (CompanyFundReceiptInfoModel.ReceiptStatus != (int)CompanyFundReceiptState.WaitAuditing &&
                    CompanyFundReceiptInfoModel.ReceiptStatus != (int)CompanyFundReceiptState.NoAuditing &&
                    CompanyFundReceiptInfoModel.ReceiptStatus != (int)CompanyFundReceiptState.PayBack)
                {
                    RAM.Alert("状态已更新，不允许此操作！");
                    return;
                }
                string remark = WebControl.RetrunUserAndTime("审核通过：" + RTB_BackReason.Text.Trim());
                Boolean isOut = false;
                if (filialeId != _reckoningElseFilialeid)
                {
                    var filialeInfo = CacheCollection.Filiale.Get(filialeId);
                    if (filialeInfo != null && filialeInfo.ID != Guid.Empty)
                    {
                        isOut = true;
                    }
                }
                var companyFundReceiptBll = new BLL.Implement.Inventory.CompanyFundReceipt(_companyFundReceipt);
                using (var ts = new TransactionScope(TransactionScopeOption.Required))
                {
                    var result = companyFundReceiptBll.UpdateFundReceiptState(RceiptId, CompanyFundReceiptState.Audited, remark, CurrentSession.Personnel.Get().PersonnelId);
                    if (result == 0)
                    {
                        RAM.Alert("系统提示：审核失败，请尝试刷新稍后再试！");
                        return;
                    }
                    _companyFundReceipt.SetDateTime(RceiptId, 1);
                    if (ShopFilialeList.All(act => act.ID != CompanyFundReceiptInfoModel.CompanyID))
                    {
                        //更新付款公司ID
                        _companyFundReceipt.UpdateFilialeId(RceiptId, filialeId, isOut);
                    }
                    ts.Complete();
                }

                //往来收付款审核增加操作记录添加
                if (CompanyFundReceiptInfoModel != null)
                {
                    var personnelInfo = CurrentSession.Personnel.Get();
                    WebControl.AddOperationLog(personnelInfo.PersonnelId, personnelInfo.RealName, RceiptId, CompanyFundReceiptInfoModel.ReceiptNo,
                        OperationPoint.CurrentReceivedPayment.PaymentAudit.GetBusinessInfo(), string.Empty);
                }
            }
            catch (Exception ex)
            {
                RAM.Alert("审核失败!" + ex.Message);
                return;
            }
            RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
        }
        #endregion

        #region[审核不通过]
        protected void Back_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(RTB_BackReason.Text.Trim()))
            {
                RAM.Alert("请填写不通过理由!");
                return;
            }
            try
            {
                CompanyFundReceiptInfoModel = _companyFundReceipt.GetCompanyFundReceiptInfo(RceiptId);
                CompanyFundReceiptInfoModel = _companyFundReceipt.GetCompanyFundReceiptInfo(RceiptId);
                if (CompanyFundReceiptInfoModel.ReceiptStatus != (int)CompanyFundReceiptState.WaitAuditing &&
                    CompanyFundReceiptInfoModel.ReceiptStatus != (int)CompanyFundReceiptState.NoAuditing &&
                    CompanyFundReceiptInfoModel.ReceiptStatus != (int)CompanyFundReceiptState.PayBack)
                {
                    RAM.Alert("状态已更新，不允许此操作！");
                    return;
                }

                string remark = WebControl.RetrunUserAndTime("审核不通过：" + RTB_BackReason.Text.Trim());
                BLL.Implement.Inventory.CompanyFundReceipt.WriteInstance.UpdateFundReceiptState(RceiptId, CompanyFundReceiptState.NoAuditingPass, remark);
                //IsFromShop(CompanyFundReceiptState.NoAuditing, remark);
                if (CompanyFundReceiptInfoModel != null)
                {
                    //往来收付款审核操作记录添加
                    var personnelInfo = CurrentSession.Personnel.Get();
                    WebControl.AddOperationLog(personnelInfo.PersonnelId, personnelInfo.RealName, RceiptId, CompanyFundReceiptInfoModel.ReceiptNo,
                                                          OperationPoint.CurrentReceivedPayment.PaymentAudit.GetBusinessInfo(),
                                                          string.Empty);
                }
            }
            catch (Exception ex)
            {
                RAM.Alert("审核失败!" + ex.Message);
                return;
            }
            RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
        }
        #endregion

        #region[根据入库单计算金额]
        /// <summary>
        /// 索取发票时使用(按日期付款)
        /// </summary>
        /// zal 2016-06-02
        public void GetDateAmount()
        {
            decimal amountSum = Math.Abs(CompanyFundReceiptInfoModel.RealityBalance) + Math.Abs(CompanyFundReceiptInfoModel.LastRebate) +
            Math.Abs(CompanyFundReceiptInfoModel.DiscountMoney);
            RTB_SettleBalance.Text = amountSum != 0 ? WebControl.NumberSeparator((amountSum).ToString("#.00")) : "0.00";
        }
        /// <summary>
        /// 索取发票时使用(按入库单付款)
        /// </summary>
        /// zal 2016-06-02
        public void GetStockAmount()
        {
            decimal amountSum = Math.Abs(CompanyFundReceiptInfoModel.RealityBalance) + Math.Abs(CompanyFundReceiptInfoModel.LastRebate) + Math.Abs(CompanyFundReceiptInfoModel.DiscountMoney) + Math.Abs(decimal.Parse(Lit_ReturnOrderMoney.Text)) + Math.Abs(decimal.Parse(Lit_PayOrderMoney.Text));
            RTB_SettleBalance.Text = amountSum != 0 ? WebControl.NumberSeparator((amountSum).ToString("#.00")) : "0.00";
        }
        #endregion

        #region[绑定数据]
        protected void BindValue()
        {
            if (CompanyFundReceiptInfoModel != null)
            {
                RCB_CompanyList.SelectedValue = CompanyFundReceiptInfoModel.CompanyID.ToString();
                RTB_ExpectBalance.Text = WebControl.NumberSeparator(CompanyFundReceiptInfoModel.ExpectBalance);
                RTB_RealityBalance.Text = WebControl.NumberSeparator(CompanyFundReceiptInfoModel.RealityBalance);
                RTB_DiscountMoney.Text = WebControl.NumberSeparator(CompanyFundReceiptInfoModel.DiscountMoney);
                RTB_DiscountCaption.Text = CompanyFundReceiptInfoModel.DiscountCaption;
                TB.InnerText = CompanyFundReceiptInfoModel.OtherDiscountCaption;
                if (!string.IsNullOrEmpty(RTB_DiscountMoney.Text) && RTB_DiscountMoney.Text != "0")
                    DIV_DiscountCaption.Visible = !string.IsNullOrEmpty(RTB_DiscountCaption.Text);
                LB_UpperCaseMoney.Text = WebUtility.ConvertSum(CompanyFundReceiptInfoModel.RealityBalance.ToString("#0.00"));
                RtbRebate.Text = string.Format("{0}", CompanyFundReceiptInfoModel.LastRebate);
                txt_PaymentDate.Text = CompanyFundReceiptInfoModel.PaymentDate.Equals(DateTime.MinValue) ? "" : Convert.ToDateTime(CompanyFundReceiptInfoModel.PaymentDate).ToString("yyyy-MM");

                #region 如果“付款公司”是ERP，则隐藏增加发票信息模块 zal 2016-02-24
                if (RCB_FilialeList.SelectedValue.Equals(_reckoningElseFilialeid.ToString()))
                {
                    div_Invoice.Visible = false;
                }
                else
                {
                    div_Invoice.Visible = true;
                }
                #endregion

                if ((CompanyFundReceiptType)CompanyFundReceiptInfoModel.ReceiptType == CompanyFundReceiptType.Payment)
                {
                    #region[付款]

                    var flag = string.IsNullOrEmpty(CompanyFundReceiptInfoModel.PurchaseOrderNo.Trim()) &&
                               string.IsNullOrEmpty(CompanyFundReceiptInfoModel.StockOrderNos.Trim()) &&
                               CompanyFundReceiptInfoModel.SettleEndDate != DateTime.Parse("1999-09-09");
                    //DIV_Type.Visible = true;
                    //DivInclude.Visible = flag;
                    //DivRemove.Visible = flag;
                    RCB_CompanyList.Enabled = false;
                    RCB_FilialeList.Enabled = false;
                    LB_RealityBalance.Text = "应付金额：";
                    LB_DiscountMoney.Text = "今年折扣：";
                    RtbRebate.Visible = true;
                    if (!string.IsNullOrEmpty(CompanyFundReceiptInfoModel.PurchaseOrderNo.Trim()))
                    {
                        //按采购单付款
                        RB_PurchaseOrder.Checked = true;
                        RTB_PurchaseOrderNo.Text = CompanyFundReceiptInfoModel.PurchaseOrderNo;
                        DIV_Date.Visible = false;
                        DIV_Goods.Visible = true;
                        DIV_Orders.Visible = false;
                        DIV_BackBalance.Visible = false;
                        DIV_ExpectBalance.Visible = false;
                        DIV_Related.Visible = false;
                        DivStockNos.Visible = false;
                    }
                    if (!string.IsNullOrEmpty(CompanyFundReceiptInfoModel.StockOrderNos.Trim()))
                    {
                        //按入库单付款
                        RB_Invoice.Checked = true;
                        TB_Orders.Text = CompanyFundReceiptInfoModel.StockOrderNos;
                        DIV_Date.Visible = false;
                        DIV_Goods.Visible = false;
                        DIV_Orders.Visible = true;
                        DIV_ExpectBalance.Visible = false;
                        DIV_Related.Visible = true;
                        DivStockNos.Visible = false;
                        DIV_BackBalance.Visible = true;
                        RTB_ReturnOrder.Text = CompanyFundReceiptInfoModel.ReturnOrder;
                        RTB_PayOrder.Text = CompanyFundReceiptInfoModel.PayOrder;
                        CheckReturnOrder();
                        CheckPayOrder();
                        GetStockAmount();
                    }
                    if (flag)
                    {
                        //按日期付款
                        RB_Date.Checked = true;
                        RDP_StartDate.SelectedDate = CompanyFundReceiptInfoModel.SettleStartDate;
                        RDP_EndDate.SelectedDate = CompanyFundReceiptInfoModel.SettleEndDate;
                        GetDateAmount();
                        DIV_Date.Visible = true;
                        DIV_Goods.Visible = false;
                        DIV_Orders.Visible = false;
                        RCB_CompanyList.Enabled = false;
                        DIV_BackBalance.Visible = true;
                        DIV_ExpectBalance.Visible = false;
                        RtbIncludeNos.Text = CompanyFundReceiptInfoModel.IncludeStockNos;
                        RtbRemoveNos.Text = CompanyFundReceiptInfoModel.DebarStockNos;
                        DIV_Related.Visible = false;
                        DivStockNos.Visible = true;
                    }
                    if (string.IsNullOrEmpty(CompanyFundReceiptInfoModel.PurchaseOrderNo.Trim())
                        && string.IsNullOrEmpty(CompanyFundReceiptInfoModel.StockOrderNos.Trim())
                        && CompanyFundReceiptInfoModel.SettleStartDate == DateTime.Parse("1999-09-09")
                        && CompanyFundReceiptInfoModel.SettleEndDate == DateTime.Parse("1999-09-09"))
                    {
                        //预付款
                        RbAdvance.Checked = true;
                        DIV_Date.Visible = false;
                        DIV_Goods.Visible = false;
                        DIV_Orders.Visible = false;
                        RCB_CompanyList.SelectedValue = Guid.Empty.ToString();
                        RCB_CompanyList.Enabled = true;
                        DIV_BackBalance.Visible = true;
                        DIV_ExpectBalance.Visible = false;
                        RCB_CompanyList.Enabled = true;
                        //LB_BankAmount.Text = "总欠款额：";
                        DIV_DiscountCaption.Visible = false;
                        decimal totalNumber = _reckoning.GetReckoningNonceTotalled(CompanyFundReceiptInfoModel.FilialeId, CompanyFundReceiptInfoModel.CompanyID, DateTime.Parse("1999-09-09 00:00:00.000"), DateTime.Now, (int)ReckoningStateType.Currently);
                        RTB_SettleBalance.Text = totalNumber != 0 ? WebControl.NumberSeparator((-totalNumber).ToString("#.00")) : "0.00";
                        DIV_Related.Visible = false;
                        DivStockNos.Visible = false;
                        DIV_DiscountMoney.Visible = false;
                    }
                    #endregion
                }
                else
                {
                    //DivInclude.Visible = false;
                    //DivRemove.Visible = false;
                    RtbRebate.Visible = false;
                    #region[收款]

                    LB_RealityBalance.Text = "应收金额：";
                    LB_DiscountMoney.Text = "收款折扣：";

                    RDP_StartDate.SelectedDate = CompanyFundReceiptInfoModel.SettleStartDate;
                    RDP_EndDate.SelectedDate = CompanyFundReceiptInfoModel.SettleEndDate;
                    //DIV_Type.Visible = false;
                    DIV_Date.Visible = true;
                    DIV_Goods.Visible = false;
                    DIV_Orders.Visible = false;
                    //int reckoningType = (int)ReckoningType.AccountReceivable;
                    //decimal totalNumber = Reckoning.GetReckoningNonceTotalByReceiptType(CompanyFundReceiptInfoModel.CompanyID, CompanyFundReceiptInfoModel.SettleEndDate);
                    decimal totalNumber = _reckoning.GetReckoningNonceTotalledByFilialeId(CompanyFundReceiptInfoModel.CompanyID, CompanyFundReceiptInfoModel.FilialeId, (DateTime)RDP_EndDate.SelectedDate);
                    RTB_SettleBalance.Text = totalNumber != 0 ? WebControl.NumberSeparator((totalNumber).ToString("#.00")) : "0.00";

                    #endregion
                }
                var filialeId = CompanyFundReceiptInfoModel.FilialeId;
                var info = _companyBankAccountBindDao.GetCompanyBankAccountBindInfo(CompanyFundReceiptInfoModel.CompanyID, filialeId);
                if (info != null && info.CompanyId != Guid.Empty)
                {
                    TB_Payee.Text = info.WebSite;
                    RTB_CompBank.Text = info.BankAccounts;
                    RTB_CompBankAccount.Text = info.AccountsNumber;
                }
                else
                {
                    CompanyCussentInfo compInfo = _companyCussent.GetCompanyCussent(CompanyFundReceiptInfoModel.CompanyID);
                    if (compInfo != null && compInfo.CompanyId != Guid.Empty)
                    {
                        if (!string.IsNullOrEmpty(compInfo.BankAccounts))
                        {
                            TB_Payee.Text = compInfo.WebSite;
                            RTB_CompBank.Text = compInfo.BankAccounts;
                            RTB_CompBankAccount.Text = compInfo.AccountNumber;
                        }
                        else
                        {
                            TB_Payee.Text = "";
                            RTB_CompBank.Text = "";
                            RTB_CompBankAccount.Text = "";
                        }
                    }
                    else
                    {
                        TB_Payee.Text = "";
                        RTB_CompBank.Text = "";
                        RTB_CompBankAccount.Text = "";
                    }
                }


                var shopInfo = ShopFilialeList.FirstOrDefault(act => act.ID == CompanyFundReceiptInfoModel.CompanyID);
                if (filialeId != _reckoningElseFilialeid && shopInfo == null)
                {
                    RCB_FilialeList.SelectedValue = CompanyFundReceiptInfoModel.FilialeId.ToString();
                }
                else
                {
                    RCB_FilialeList.SelectedValue = _reckoningElseFilialeid.ToString();
                    RCB_FilialeList.Enabled = false;
                }
            }
        }
        #endregion

        //验证退货单 zal 2016-02-14
        protected void CheckReturnOrder()
        {
            var returnOrder = RTB_ReturnOrder.Text.Trim();
            if (!string.IsNullOrEmpty(returnOrder))
            {
                var storageRecordInfoList = _storageRecordDao.GetStorageRecordList(returnOrder);
                decimal accountReceivable = 0;
                foreach (var item in storageRecordInfoList)
                {
                    accountReceivable += item.AccountReceivable;
                }
                Lit_ReturnOrderMoney.Text = Math.Abs(accountReceivable).ToString(CultureInfo.InvariantCulture);
                Lit_ReturnOrder.Text = string.Empty;
            }
            else
            {
                Lit_ReturnOrderMoney.Text = "0";
                Lit_ReturnOrder.Text = string.Empty;
            }
        }

        //验证付款单 zal 2016-02-14
        protected void CheckPayOrder()
        {
            var payOrder = RTB_PayOrder.Text.Trim();
            if (!string.IsNullOrEmpty(payOrder))
            {
                var storageRecordInfoList = _companyFundReceipt.GetCompanyFundReceiptList(payOrder);
                decimal realityBalance = 0;
                foreach (var item in storageRecordInfoList)
                {
                    realityBalance += item.RealityBalance;
                }
                Lit_PayOrderMoney.Text = Math.Abs(realityBalance).ToString(CultureInfo.InvariantCulture);
                Lit_PayOrder.Text = string.Empty;
            }
            else
            {
                Lit_PayOrderMoney.Text = "0";
                Lit_PayOrder.Text = string.Empty;
            }
        }

        #region[选择往来单位]
        protected void RcbCompanyListSelectedIndexChanged(object o, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            var companyID = new Guid(e.Value.Trim());
            CompanyCussentInfo compInfo = _companyCussent.GetCompanyCussent(companyID);
            if (compInfo != null && compInfo.CompanyId != Guid.Empty)
            {
                TB_Payee.Text = compInfo.WebSite;
                RTB_CompBank.Text = compInfo.BankAccounts;
                RTB_CompBankAccount.Text = compInfo.AccountNumber;
            }
            else
            {
                TB_Payee.Text = "";
                RTB_CompBank.Text = "";
                RTB_CompBankAccount.Text = "";
            }
        }
        #endregion

        #region[获取历史付款单据数据]
        protected string GetCompName(string compId)
        {
            //note 增加门店信息获取  2015-03-17 陈重文
            var info = RelatedCompany.Instance.ToList().FirstOrDefault(o => o.CompanyId == new Guid(compId));
            if (info != null)
            {
                return info.CompanyName;
            }
            var filialeInfo = CacheCollection.Filiale.Get(new Guid(compId));
            return filialeInfo != null ? filialeInfo.Name : string.Empty;
        }

        protected string GetPersonName(string personId)
        {
            return _personnelManager.GetName(new Guid(personId));
        }

        protected string GetReceiptStatus(string receiptStatus)
        {
            var stateDic = (Dictionary<int, string>)EnumAttribute.GetDict<CompanyFundReceiptState>();

            foreach (KeyValuePair<int, string> kvp in stateDic)
            {
                if (receiptStatus == string.Format("{0}", kvp.Key))
                {
                    return kvp.Value;
                }
            }
            return "未知状态";
        }

        protected void RgCheckInfoNeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            DateTime startTime = DateTime.Now.AddDays(-30);
            DateTime endTime = DateTime.Now;
            IList<CompanyFundReceiptInfo> list = _companyFundReceipt.GetAllFundReceiptListByCompanyId(CompanyFundReceiptInfoModel.CompanyID);
            if (list.Count > 0)
            {
                list = list.Where(c => c.ApplyDateTime >= startTime && c.ApplyDateTime <= endTime).ToList();
            }
            RG_CheckInfo.DataSource = list.OrderByDescending(ent => ent.ApplyDateTime).ToList();
        }
        #endregion

        #region[选择公司后加载出对应的银行账号]
        protected void Rcb_FilialeListSelectedIndexChanged(object o, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            CompanyFundReceiptInfoModel = _companyFundReceipt.GetCompanyFundReceiptInfo(RceiptId);
            if (CompanyFundReceiptInfoModel == null) return;
            var companyId = CompanyFundReceiptInfoModel.CompanyID;
            if (companyId == Guid.Empty)
            {
                RAM.Alert("系统提示：请先选择往来单位！");
                return;
            }
            var filialeId = new Guid(RCB_FilialeList.SelectedValue);
            if (filialeId == Guid.Empty)
            {
                RAM.Alert("系统提示：请选择公司！");
            }
            var info = _companyBankAccountBindDao.GetCompanyBankAccountBindInfo(companyId, filialeId);
            if (info != null && info.CompanyId != Guid.Empty)
            {
                TB_Payee.Text = info.WebSite;
                RTB_CompBank.Text = info.BankAccounts;
                RTB_CompBankAccount.Text = info.AccountsNumber;
            }
            else
            {
                TB_Payee.Text = string.Empty;
                RTB_CompBank.Text = string.Empty;
                RTB_CompBankAccount.Text = string.Empty;
            }
        }
        #endregion

        protected void RG_InvoiceList_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            var dataList = _companyFundReceiptInvoiceInfoList;
            if (_companyFundReceiptInvoiceInfoList.Any())
            {
                dataList = _companyFundReceiptInvoiceInfoList.OrderByDescending(p => p.BillingDate).ToList();

                //合计金额
                var invoiceCode = RG_InvoiceList.MasterTableView.Columns.FindByUniqueName("InvoiceCode");
                var noTaxAmount = RG_InvoiceList.MasterTableView.Columns.FindByUniqueName("NoTaxAmount");
                var tax = RG_InvoiceList.MasterTableView.Columns.FindByUniqueName("Tax");
                var taxAmount = RG_InvoiceList.MasterTableView.Columns.FindByUniqueName("TaxAmount");

                invoiceCode.FooterText = "合计：";
                var sumNoTaxAmount = _companyFundReceiptInvoiceInfoList.Sum(p => p.NoTaxAmount);
                noTaxAmount.FooterText = string.Format("{0}", WebControl.NumberSeparator(sumNoTaxAmount));
                var sumTax = _companyFundReceiptInvoiceInfoList.Sum(p => p.Tax);
                tax.FooterText = string.Format("{0}", WebControl.NumberSeparator(sumTax));
                var sumTaxAmount = _companyFundReceiptInvoiceInfoList.Sum(p => p.TaxAmount);
                taxAmount.FooterText = string.Format("{0}", WebControl.NumberSeparator(sumTaxAmount));
               // InvoiceTypeName.Text = _companyFundReceiptInvoiceInfoList.First().InvoiceType == 0 ? string.Empty : EnumAttribute.GetKeyName((CostReportInvoiceType)_companyFundReceiptInvoiceInfoList.First().InvoiceType);
            }
            RG_InvoiceList.DataSource = dataList;
        }

        public decimal GetTotalloled(List<string> stockOrderNos, List<string> removeNos)
        {
            if (RDP_EndDate.SelectedDate != null && RDP_StartDate.SelectedDate != null && RDP_EndDate.SelectedDate > RDP_StartDate.SelectedDate)
            {
                DateTime start = Convert.ToDateTime(RDP_StartDate.SelectedDate);
                DateTime end = Convert.ToDateTime(RDP_EndDate.SelectedDate);
                var dic = _reckoning.GetTotalledByDate(new Guid(RCB_CompanyList.SelectedValue), new Guid(RCB_FilialeList.SelectedValue),
                            start, end.AddDays(1).AddSeconds(-1), (int)CheckType.NotCheck, stockOrderNos, removeNos);
                return dic.Sum(act => act.Value);
            }
            return 0;
        }
    }
}
