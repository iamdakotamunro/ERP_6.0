using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Transactions;
using AllianceShop.Contract.DataTransferObject;
using ERP.BLL.Implement;
using ERP.BLL.Implement.Inventory;
using ERP.BLL.Implement.Organization;
using ERP.Cache;
using ERP.DAL.Implement.Company;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.ICompany;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Enum.Attribute;
using ERP.Environment;
using ERP.Model;
using ERP.SAL;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Keede.Ecsoft.Model;
using Lonme.WebControls;
using Telerik.Web.UI;

namespace ERP.UI.Web.Windows
{
    /// <summary>申请打款   执行。
    /// </summary>
    public partial class CheckReceiptForm : WindowsPage
    {
        protected CompanyFundReceiptInfo CompanyFundReceiptInfoModel;
        readonly CodeManager _codeBll = new CodeManager();
        readonly ICompanyBankAccountBind _companyBankAccountBindBll = new CompanyBankAccountBindDao(GlobalConfig.DB.FromType.Read);
        private readonly IStorageRecordDao _storageRecordDao = new StorageRecordDao(GlobalConfig.DB.FromType.Write);
        private readonly IWasteBook _wasteBook = new WasteBook(GlobalConfig.DB.FromType.Write);
        private static readonly IReckoning _reckoning = new Reckoning(GlobalConfig.DB.FromType.Write);
        private readonly ICompanyFundReceipt _companyFundReceipt = new DAL.Implement.Inventory.CompanyFundReceipt(GlobalConfig.DB.FromType.Write);
        private readonly IBankAccountDao _bankAccountDao = new BankAccountDao(GlobalConfig.DB.FromType.Read);
        private readonly IBankAccounts _bankAccounts = new BankAccounts(GlobalConfig.DB.FromType.Write);
        readonly CompanySubjectDiscountDal _companySubjectDiscountDao = new CompanySubjectDiscountDal(GlobalConfig.DB.FromType.Write);
        private readonly ICompanyCussent _companyCussent = new CompanyCussent(GlobalConfig.DB.FromType.Read);
        //其他公司
        private readonly Guid _reckoningElseFilialeid = new Guid(ConfigManage.GetConfigValue("RECKONING_ELSE_FILIALEID"));

        readonly ICompanyFundReceiptInvoice _companyFundReceiptInvoice = new ERP.DAL.Implement.Inventory.CompanyFundReceiptInvoice(GlobalConfig.DB.FromType.Write);
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

                    HF_ReceiptNo.Value = CompanyFundReceiptInfoModel.ReceiptNo;
                    HF_TypeName.Value = EnumAttribute.GetKeyName((CompanyFundReceiptType)CompanyFundReceiptInfoModel.ReceiptType);
                    CompanyCussentInfo compInfo = _companyCussent.GetCompanyCussent(CompanyFundReceiptInfoModel.CompanyID);

                    IsShopReceipt = false;
                    bool isShow = false;
                    if (compInfo == null)
                    {
                        var shopInfo = ShopFilialeList.FirstOrDefault(act => act.ID == CompanyFundReceiptInfoModel.CompanyID);
                        if (shopInfo == null)
                        {
                            RAM.Alert("不是有效的往来单位信息");
                            RAM.ResponseScripts.Add("CancelWindow();");
                            return;
                        }
                        IsShopReceipt = true;
                        CompanyFundReceiptDto = ShopSao.GetCompanyFundReceiptEntityByOriginalTradeCode(shopInfo.ParentId, CompanyFundReceiptInfoModel.ReceiptNo);
                    }
                    if (!IsShopReceipt && compInfo != null && compInfo.RelevanceFilialeId != Guid.Empty)
                    {
                        isShow = true;
                    }
                    GetCompanyList();
                    BindValue();
                    if (Request.QueryString["type"] != null)
                    {
                        var bankAccounts = _companyBankAccountBindBll.GetCompanyBankAccountIdBind(CompanyFundReceiptInfoModel.CompanyID, CompanyFundReceiptInfoModel.FilialeId) ??
                            _companyBankAccountBindBll.GetCompanyBankAccountBindInfo(CompanyFundReceiptInfoModel.CompanyID, CompanyFundReceiptInfoModel.FilialeId);
                        if (bankAccounts != null)
                        {
                            TB_Payee.Text = bankAccounts.WebSite;
                            RTB_CompBank.Text = bankAccounts.BankAccounts;
                            RTB_CompBankAccount.Text = bankAccounts.AccountsNumber;
                        }
                        else
                        {
                            TB_Payee.Text = compInfo != null ? compInfo.WebSite : CacheCollection.Filiale.GetName(CompanyFundReceiptInfoModel.FilialeId);
                            RTB_CompBank.Text = compInfo != null ? compInfo.BankAccounts : string.Empty;
                            RTB_CompBankAccount.Text = compInfo != null ? compInfo.AccountNumber : string.Empty;
                        }

                        TB_Poundage.Text = string.Format("{0}", CompanyFundReceiptInfoModel.Poundage);
                        if (Request.QueryString["type"] == "1")
                        {
                            DivFlowNo.Visible = true;
                            Div_ShowDo.Visible = true;
                            Btn_Do.Visible = CompanyFundReceiptInfoModel.ReceiptStatus == (int)CompanyFundReceiptState.Audited
                                || CompanyFundReceiptInfoModel.ReceiptStatus == (int)CompanyFundReceiptState.PayBack;
                        }
                        else
                        {
                            DivFlowNo.Visible = isShow;
                            TbFlowNo.ReadOnly = true;
                            Div_ShowtAuditing.Visible = true;
                            TB_Poundage.ReadOnly = true;
                        }
                    }
                    if (CompanyFundReceiptInfoModel.ReceiptStatus == (int)CompanyFundReceiptState.Finish)
                    {
                        //完成打款
                        btnFinish.Enabled = false;
                        BtnSave.Enabled = false;
                        btnBack.Enabled = false;
                        Btn_GoBack.Enabled = false;
                        Btn_Do.Enabled = false;
                        if (CompanyFundReceiptInfoModel.ReceiveBankAccountId != Guid.Empty)
                        {
                            DivFlowNo.Visible = true;
                            TbFlowNo.ReadOnly = true;
                        }
                        else
                        {
                            DivFlowNo.Visible = true;
                        }
                    }
                    //GetBank(CompanyFundReceiptInfoModel.FilialeId);
                    RCB_FilialeList.Enabled = false;
                    RCB_CompanyList.Enabled = false;
                }
            }
        }
        #endregion

        #region[获取公司数据信息，包含供应商和物流公司]
        /// <summary>
        /// 获取公司数据信息，包含供应商和物流公司
        /// </summary>
        protected void GetCompanyList()
        {
            int[] companyTypes = { (int)CompanyType.Suppliers, (int)CompanyType.Express, (int)CompanyType.Vendors };
            var data = _companyCussent.GetCompanyCussentList(State.Enable).Where(ent => companyTypes.Contains(ent.CompanyType) || ent.RelevanceFilialeId != Guid.Empty).ToDictionary(k => k.CompanyId, v => v.CompanyName);
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
            ShowFiliale();
        }
        #endregion

        #region[显示对应付款公司]
        protected void ShowFiliale()
        {
            IList<FilialeInfo> filialeList = !IsShopReceipt
                ? CacheCollection.Filiale.GetHostingAndSaleFilialeList()
                : new List<FilialeInfo>();
            if (CompanyFundReceiptInfoModel != null)
            {
                filialeList.Add(new FilialeInfo
                {
                    ID = CompanyFundReceiptInfoModel.FilialeId,
                    Name = CacheCollection.Filiale.GetName(CompanyFundReceiptInfoModel.FilialeId)
                });
            }

            RCB_FilialeList.DataSource = filialeList;
            RCB_FilialeList.DataTextField = "Name";
            RCB_FilialeList.DataValueField = "ID";
            RCB_FilialeList.DataBind();
            RCB_FilialeList.Items.Add(new RadComboBoxItem("", Guid.Empty.ToString()));
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

        /// <summary>
        /// 1 为申请打款  2 为打款完成
        /// </summary>
        public int Type
        {
            get
            {
                if (Request.QueryString["type"] != null)
                {
                    return Convert.ToInt32(Request.QueryString["type"]);
                }
                return 1;
            }
        }

        #endregion

        #region  判断是否为门店往来单位收付款

        public bool IsShopReceipt
        {
            get
            {
                return ViewState["IsShopReceipt"] != null && Convert.ToBoolean(ViewState["IsShopReceipt"]);
            }
            set
            {
                ViewState["IsShopReceipt"] = value;
            }
        }
        #endregion

        #region[收付款执行]
        protected void Btn_Do_Click(object sender, EventArgs e)
        {
            try
            {
                var bankAccountsId = RcbBankAccount.SelectedValue;
                var elseBankAccountsId = RcbElseBankAccount.SelectedValue;
                if ((string.IsNullOrWhiteSpace(bankAccountsId) || bankAccountsId == Guid.Empty.ToString()) && (string.IsNullOrWhiteSpace(elseBankAccountsId) || elseBankAccountsId == Guid.Empty.ToString()))
                {
                    RAM.Alert(string.Format("系统提示：{0}", Type == 2 ? "本公司银行帐号未绑定！" : "请选择银行！"));
                    return;
                }
                if ((!string.IsNullOrWhiteSpace(bankAccountsId) && bankAccountsId != Guid.Empty.ToString()) && (!string.IsNullOrWhiteSpace(elseBankAccountsId) && elseBankAccountsId != Guid.Empty.ToString()))
                {
                    RAM.Alert("系统提示：公司帐号和其他公司帐号只能选择其一！");
                    return;
                }
                var realBankAccountsId = bankAccountsId == Guid.Empty.ToString()
                    ? new Guid(elseBankAccountsId)
                    : new Guid(bankAccountsId);
                CompanyFundReceiptInfo info = _companyFundReceipt.GetCompanyFundReceiptInfo(RceiptId);
                if (string.IsNullOrEmpty(info.DealFlowNo))
                {
                    if (string.IsNullOrEmpty(TbFlowNo.Text))
                    {
                        RAM.Alert("交易流水号不能为空!");
                        return;
                    }
                    _companyFundReceipt.UpdateDealFlowNo(info.ReceiptID, TbFlowNo.Text);
                }
                else
                {
                    if (info.DealFlowNo != TbFlowNo.Text)
                        _companyFundReceipt.UpdateDealFlowNo(info.ReceiptID, TbFlowNo.Text);
                }
                var companyInfo = _companyCussent.GetCompanyCussent(info.CompanyID);
                CompanyFundReceiptInfoModel = _companyFundReceipt.GetCompanyFundReceiptInfo(RceiptId);
                string remark = WebControl.RetrunUserAndTime("执行");
                using (var ts = new TransactionScope(TransactionScopeOption.Required))
                {
                    var isSuccess = _companyFundReceipt.SetPayBankAccountsId(RceiptId, realBankAccountsId);
                    if (!isSuccess)
                    {
                        RAM.Alert("系统提示：审核失败，请尝试刷新稍后再试！");
                        return;
                    }
                    //付款
                    if (info.ReceiptType == (int)CompanyFundReceiptType.Payment)
                    {
                        if (string.IsNullOrEmpty(RTB_CompBankAccount.Text) && !IsShopReceipt)
                        {
                            RAM.Alert("往来单位银行帐号未绑定");
                            return;
                        }
                        var accountCount = _bankAccounts.GetBankAccountsNonce(realBankAccountsId);
                        if (accountCount <= 0)
                        {
                            RAM.Alert("帐户余额为0，无法付款！");
                            return;
                        }
                        if (Convert.ToDecimal(accountCount) < CompanyFundReceiptInfoModel.RealityBalance + CompanyFundReceiptInfoModel.Poundage)
                        {
                            RAM.Alert("帐户余额为不足，无法付款！");
                            return;
                        }
                        _companyFundReceipt.UpdateFundReceiptRemark(RceiptId, remark);
                        _companyFundReceipt.UpdateFundReceiptState(RceiptId, CompanyFundReceiptState.Executed);
                        _companyFundReceipt.SetDateTime(RceiptId, 2);
                        if (!IsShopReceipt)
                        {
                            _companyFundReceipt.UpdatePoundage(RceiptId, decimal.Parse(string.IsNullOrEmpty(TB_Poundage.Text) ? "0" : TB_Poundage.Text));
                        }
                    }
                    //收款
                    if (info.ReceiptType == (int)CompanyFundReceiptType.Receive)
                    {
                        _companyFundReceipt.UpdateFundReceiptRemark(RceiptId, remark);
                        _companyFundReceipt.UpdateFundReceiptState(RceiptId, CompanyFundReceiptState.Executed);
                        _companyFundReceipt.SetDateTime(RceiptId, 2);
                        if (!IsShopReceipt)
                        {
                            _companyFundReceipt.UpdatePoundage(RceiptId, decimal.Parse(string.IsNullOrEmpty(TB_Poundage.Text) ? "0" : TB_Poundage.Text));
                        }
                    }
                    ts.Complete();
                }
            }
            catch (Exception ex)
            {
                RAM.Alert("执行失败!" + ex.Message);
                return;
            }
            RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
        }
        #endregion

        #region[驳回执行]
        protected void Btn_GoBack_Click(object sender, EventArgs e)
        {
            try
            {
                CompanyFundReceiptInfo info = _companyFundReceipt.GetCompanyFundReceiptInfo(RceiptId);
                if (info != null && info.ReceiptStatus == (int)CompanyFundReceiptState.NoAuditing)
                {
                    RAM.Alert("该收付款单据状态已改变!");
                    return;
                }
                string remark = WebControl.RetrunUserAndTime("申请打款执行退回");
                var bll = new BLL.Implement.Inventory.CompanyFundReceipt(_companyFundReceipt);
                bll.UpdateFundReceiptState(RceiptId, CompanyFundReceiptState.NoAuditing, remark);
                CompanyFundReceiptInfoModel = _companyFundReceipt.GetCompanyFundReceiptInfo(RceiptId);
                //IsFromShop(CompanyFundReceiptState.NoAuditing, remark);
            }
            catch (Exception ex)
            {
                RAM.Alert("申请打款执行退回失败!" + ex.Message);
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

        #region  门店公司信息列表

        protected IList<FilialeInfo> ShopFilialeList
        {
            get
            {
                return CacheCollection.Filiale.GetShopList();
            }
        }
        #endregion

        #region  门店往来单位收付款

        public CompanyFundReceiptDTO CompanyFundReceiptDto
        {
            get
            {
                if (ViewState["CompanyFundReceiptDto"] != null)
                {
                    return (CompanyFundReceiptDTO)ViewState["CompanyFundReceiptDto"];
                }
                return null;
            }
            set { ViewState["CompanyFundReceiptDto"] = value; }
        }
        #endregion

        #region[绑定数据]
        // ReSharper disable once FunctionComplexityOverflow
        protected void BindValue()
        {
            if (CompanyFundReceiptInfoModel != null)
            {
                RCB_FilialeList.Enabled = false;
                RCB_CompanyList.SelectedValue = string.Format("{0}", CompanyFundReceiptInfoModel.CompanyID);
                RCB_FilialeList.SelectedValue = string.Format("{0}", CompanyFundReceiptInfoModel.FilialeId);
                RTB_ExpectBalance.Text = WebControl.NumberSeparator(CompanyFundReceiptInfoModel.ExpectBalance);
                RTB_RealityBalance.Text = WebControl.NumberSeparator(CompanyFundReceiptInfoModel.RealityBalance);
                RTB_DiscountMoney.Text = string.Format("{0}", CompanyFundReceiptInfoModel.DiscountMoney);
                Lb_DiscountCaption.Text = CompanyFundReceiptInfoModel.DiscountCaption;
                LB_OtherDiscountCaption.Text = CompanyFundReceiptInfoModel.OtherDiscountCaption;
                TbFlowNo.Text = CompanyFundReceiptInfoModel.DealFlowNo;
                if (!string.IsNullOrEmpty(RTB_DiscountMoney.Text) && RTB_DiscountMoney.Text != "0")
                    DIV_DiscountCaption.Visible = true;
                else
                    DIV_DiscountCaption.Visible = false;
                LB_UpperCaseMoney.Text = WebUtility.ConvertSum(string.Format("{0}", CompanyFundReceiptInfoModel.RealityBalance));
                bool isDate = string.IsNullOrEmpty(CompanyFundReceiptInfoModel.PurchaseOrderNo.Trim()) &&
                              string.IsNullOrEmpty(CompanyFundReceiptInfoModel.StockOrderNos.Trim()) &&
                              CompanyFundReceiptInfoModel.SettleEndDate != DateTime.Parse("1999-09-09");
                DivStockNos.Visible = isDate && (CompanyFundReceiptType)CompanyFundReceiptInfoModel.ReceiptType == CompanyFundReceiptType.Payment;
                RtbRebate.Text = string.Format("{0}", CompanyFundReceiptInfoModel.LastRebate);
                LbRebate.Text = "去年返利：";
                txt_PaymentDate.Text = CompanyFundReceiptInfoModel.PaymentDate.Equals(DateTime.MinValue) ? "" : Convert.ToDateTime(CompanyFundReceiptInfoModel.PaymentDate).ToString("yyyy-MM");

                if (CompanyFundReceiptInfoModel.IsUrgent)
                {
                    RbNormal.Checked = false;
                    RbUrgent.Checked = true;
                    LbUrgent.Visible = true;
                    LbUrgentTitle.Visible = true;
                    RtbUrgent.Visible = true;
                    RtbUrgent.Text = CompanyFundReceiptInfoModel.UrgentRemark;
                }
                else
                {
                    RbNormal.Checked = true;
                    RbUrgent.Checked = false;
                    LbUrgent.Visible = false;
                    LbUrgentTitle.Visible = false;
                    RtbUrgent.Visible = false;
                }

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
                    div_PaymentDate.Visible = true;
                    DIV_Type.Visible = true;
                    Lit_Payee.Text = "往来单位收款人";
                    DIV_Poundage.Visible = true;
                    LbRebate.Visible = true;
                    RtbRebate.Visible = true;
                    RCB_CompanyList.Enabled = false;
                    LB_RealityBalance.Text = "应付金额：";
                    LB_DiscountMoney.Text = "今年折扣：";

                    if (!string.IsNullOrEmpty(CompanyFundReceiptInfoModel.PurchaseOrderNo.Trim()))
                    {
                        //按采购单付款
                        RB_PurchaseOrder.Checked = true;
                        RTB_PurchaseOrderNo.Text = CompanyFundReceiptInfoModel.PurchaseOrderNo;
                        DIV_Date.Visible = false;
                        DIV_Goods.Visible = true;
                        DIV_Orders.Visible = false;
                        DIV_ExpectBalance.Visible = false;
                        DIV_Related.Visible = false;
                        DIV_BackBalance.Visible = false;
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
                        RTB_ReturnOrder.Text = CompanyFundReceiptInfoModel.ReturnOrder;
                        RTB_PayOrder.Text = CompanyFundReceiptInfoModel.PayOrder;
                        CheckReturnOrder();
                        CheckPayOrder();
                        GetStockAmount();
                        DIV_BackBalance.Visible = true;
                    }
                    if (isDate)
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
                    }
                    if (string.IsNullOrEmpty(CompanyFundReceiptInfoModel.PurchaseOrderNo.Trim())
                        && string.IsNullOrEmpty(CompanyFundReceiptInfoModel.StockOrderNos.Trim())
                        && CompanyFundReceiptInfoModel.SettleStartDate == DateTime.Parse("1999-09-09")
                        && CompanyFundReceiptInfoModel.SettleEndDate == DateTime.Parse("1999-09-09"))
                    {
                        //预付款
                        LbRebate.Visible = false;
                        RtbRebate.Visible = false;
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
                        LB_DiscountMoney.Visible = false;
                        DIV_DiscountCaption.Visible = false;
                        RTB_DiscountMoney.Visible = false;
                        decimal totalNumber = _reckoning.GetReckoningNonceTotalled(CompanyFundReceiptInfoModel.FilialeId, CompanyFundReceiptInfoModel.CompanyID, DateTime.Parse("1999-09-09 00:00:00.000"), DateTime.Now, (int)ReckoningStateType.Currently);
                        RTB_SettleBalance.Text = totalNumber != 0 ? WebControl.NumberSeparator((-totalNumber).ToString("#.00")) : "0";
                        DIV_Related.Visible = false;
                    }
                    #endregion
                }
                else
                {
                    #region[收款]
                    div_PaymentDate.Visible = false;

                    DIV_Poundage.Visible = false;
                    LbRebate.Visible = false;
                    RtbRebate.Visible = false;

                    LB_RealityBalance.Text = "应收金额：";
                    LB_DiscountMoney.Text = "收款折扣：";
                    if (!CompanyFundReceiptInfoModel.IsServiceFee)
                    {
                        RDP_StartDate.SelectedDate = CompanyFundReceiptInfoModel.SettleStartDate;
                        RDP_EndDate.SelectedDate = CompanyFundReceiptInfoModel.SettleEndDate;
                        decimal totalNumber = _reckoning.GetReckoningNonceTotalledByFilialeId(CompanyFundReceiptInfoModel.CompanyID, CompanyFundReceiptInfoModel.FilialeId, (DateTime)RDP_EndDate.SelectedDate);
                        RTB_SettleBalance.Text = totalNumber == 0 ? "0" : WebControl.NumberSeparator(totalNumber.ToString("#.00"));
                        DIV_Date.Visible = true;
                        DIV_DiscountCaption.Visible = true;
                        DIV_DiscountMoney.Visible = true;
                        DIV_BackBalance.Visible = true;
                        DIV_ExpectBalance.Visible = true;
                    }
                    else
                    {
                        DIV_Date.Visible = false;
                        DIV_DiscountCaption.Visible = false;
                        DIV_ExpectBalance.Visible = false;
                        DIV_DiscountMoney.Visible = false;
                    }
                    DIV_BackBalance.Visible = false;
                    LbUrgentLevel.Visible = false;
                    RbNormal.Visible = false;
                    RbUrgent.Visible = false;
                    LbUrgent.Visible = false;
                    DIV_Type.Visible = false;
                    Lit_Payee.Text = "往来单位付款人";
                    DIV_Goods.Visible = false;
                    DIV_Orders.Visible = false;
                    DIV_Related.Visible = false;
                    InvoiceTypeName.Text = "收据";

                    #endregion

                    RG_InvoiceList.MasterTableView.Columns.FindByUniqueName("InvoiceCode").Display = false;
                    RG_InvoiceList.MasterTableView.Columns.FindByUniqueName("NoTaxAmount").Display = false;
                    RG_InvoiceList.MasterTableView.Columns.FindByUniqueName("Tax").Display = false;


                }
                var filialeId = CompanyFundReceiptInfoModel.FilialeId;
                if (filialeId == _reckoningElseFilialeid || IsShopReceipt)
                {
                    RCB_FilialeList.Items.Add(new RadComboBoxItem("ERP", _reckoningElseFilialeid.ToString()));
                    RCB_FilialeList.SelectedValue = _reckoningElseFilialeid.ToString();
                }
                else
                {
                    RCB_FilialeList.SelectedValue = CompanyFundReceiptInfoModel.FilialeId.ToString();
                }
                GetBank(filialeId);
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

        #region[完成]
        // ReSharper disable once FunctionComplexityOverflow
        protected void BtnFinishClick(object sender, EventArgs e)
        {
            Guid receiptId = RceiptId;
            CompanyFundReceiptInfo info = _companyFundReceipt.GetCompanyFundReceiptInfo(receiptId) ?? new CompanyFundReceiptInfo();
            if (info.FinishDate != DateTime.MinValue)
            {
                RAM.Alert("该收付款单据已完成打款!");
                return;
            }

            if (!Convert.ToInt32(CompanyFundReceiptState.Executed).Equals(info.ReceiptStatus))
            {
                RAM.Alert("请先申请打款!");
                return;
            }

            var flag = info.ReceiptType == (int)CompanyFundReceiptType.Receive; //收款还是付款
            string reckNo = _codeBll.GetCode(CodeType.GT);
            //如果是其他公司则获取任意总公司
            //Guid originalFilialeId = info.FilialeId;
            Guid filialeId = info.FilialeId;
            string tradecode = info.ReceiptNo;
            BankAccountInfo bankInfo = BankAccount.Instance.Get(info.PayBankAccountsId);
            var compInfo = _companyCussent.GetCompanyCussent(info.CompanyID);
            if (compInfo == null && !IsShopReceipt)
            {
                RAM.Alert("不是有效的往来单位信息");
                return;
            }
            Guid receiveptFilialeId = _companyCussent.GetRelevanceFilialeIdByCompanyId(info.CompanyID);
            if (info.ReceiptType == (int)CompanyFundReceiptType.Payment)
            {
                var accountCount = _bankAccounts.GetBankAccountsNonce(info.PayBankAccountsId);
                if (accountCount <= 0)
                {
                    RAM.Alert("帐户余额为0，无法付款！");
                    return;
                }
                var bankAccount = _companyBankAccountBindBll.GetCompanyBankAccountIdBind(info.CompanyID, filialeId) ?? _companyBankAccountBindBll.GetCompanyBankAccountBindInfo(info.CompanyID, filialeId);
                if(bankAccount == null)
                {
                    RAM.Alert("公司银行账户信息未找到！");
                    return;
                }
                compInfo.WebSite = bankAccount.WebSite;
            }
            var personnelInfo = CurrentSession.Personnel.Get();
            using (var ts = new TransactionScope(TransactionScopeOption.Required))
            {
                try
                {
                    string remark;
                    string errorMsg;
                    #region  联盟店往来单位收付款完成 add by liangcanren at 2015-03-11 16:07
                    if (IsShopReceipt)
                    {
                        if (CompanyFundReceiptDto == null)
                        {
                            RAM.Alert("对应门店往来收付款单据未找到!");
                            return;
                        }
                        var rRDesc = string.Format("[{0}款银行:{1}][{2}款人:{3}]" + "[交易流水号：{4}]",
                        flag ? "收" : "付", CompanyFundReceiptDto.CompanyBankName, flag ? "付" : "收", CompanyFundReceiptDto.ShopName, info.DealFlowNo);
                        var receiveReckoningDesc = WebControl.RetrunUserAndTime(string.Format("[联盟店总管理对{0}{1}款,详细见备注说明]{2}",
                            CompanyFundReceiptDto.ShopName, flag ? "收" : "付", rRDesc));
                        var dateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        var wdes = string.Format("[往来{0}款（{1}款单位：{2}；交易流水号：{3}），资金{4}，{5}）]", flag ? "收" : "付", flag ? "付" : "收",
                            CompanyFundReceiptDto.ShopName, info.DealFlowNo, flag ? "增加" : "减少", dateTime);
                        //往来帐
                        var reckoningInfo = new ReckoningInfo(Guid.NewGuid(), info.FilialeId, info.CompanyID, reckNo,
                                                              receiveReckoningDesc,
                                                              flag ? -info.RealityBalance : info.RealityBalance, flag ? (int)ReckoningType.Defray : (int)ReckoningType.Income,
                                                              (int)ReckoningStateType.Currently,
                                                              (int)CheckType.NotCheck, (int)AuditingState.Yes,
                                                              info.ReceiptNo, Guid.Empty)
                        {
                            LinkTradeType = (int)ReckoningLinkTradeType.CompanyFundReceipt,
                            IsOut = info.IsOut
                        };
                        //资金流
                        var wasteBookinfo = new WasteBookInfo(Guid.NewGuid(), info.PayBankAccountsId, info.ReceiptNo,
                                                              wdes + "[备注说明：" + info.OtherDiscountCaption + "]",
                                                              flag ? info.RealityBalance : -info.RealityBalance, (Int32)AuditingState.Yes,
                                                              flag ? (int)WasteBookType.Increase : (int)WasteBookType.Decrease, info.FilialeId)
                        {
                            LinkTradeCode = info.ReceiptNo,
                            LinkTradeType = (int)WasteBookLinkTradeType.CompanyFundReceipt,
                            BankTradeCode = string.Empty,
                            State = (int)WasteBookState.Currently,
                            IsOut = bankInfo.IsMain
                        };
                        _reckoning.Insert(reckoningInfo, out errorMsg);
                        if (wasteBookinfo.Income != 0)
                            _wasteBook.Insert(wasteBookinfo);
                        remark = WebControl.RetrunUserAndTime("完成");
                        _companyFundReceipt.UpdateFundReceiptRemark(RceiptId, remark);
                        _companyFundReceipt.UpdateFundReceiptState(RceiptId, CompanyFundReceiptState.Finish);
                    }
                    #endregion
                    else
                    {
                        #region[付款]

                        if (info.ReceiptType == (int)CompanyFundReceiptType.Payment)
                        {
                            string originalTradeCode = info.ReceiptNo;
                            int paymentType = 4;
                            //说明
                            string payType = string.Empty;
                            if (!string.IsNullOrEmpty(info.PurchaseOrderNo))
                            {
                                payType = "按采购单付款,采购单号：" + info.PurchaseOrderNo;
                                originalTradeCode = info.PurchaseOrderNo;
                                paymentType = 1;
                            }
                            if (!string.IsNullOrEmpty(info.StockOrderNos))
                            {
                                payType = "按入库单付款,入库单号：" + info.StockOrderNos;
                                paymentType = 2;
                            }
                            if (string.IsNullOrEmpty(info.PurchaseOrderNo) && string.IsNullOrEmpty(info.StockOrderNos) &&
                                info.SettleEndDate != DateTime.Parse("1999-09-09"))
                            {
                                paymentType = 3;
                                payType = "按日期付款,日期:" + info.SettleStartDate.ToString("yyyy/MM/dd") + "到" +
                                          info.SettleEndDate.ToString("yyyy/MM/dd");
                            }
                            if (string.IsNullOrEmpty(info.PurchaseOrderNo) && string.IsNullOrEmpty(info.StockOrderNos) &&
                                info.SettleStartDate == DateTime.Parse("1999-09-09"))
                            {
                                payType = "预付款";
                            }
                            //完成打款人
                            var finishPersonnel = CurrentSession.Personnel.Get().RealName;
                            //提交人
                            var applicantPersonnel = new PersonnelManager().GetName(info.ApplicantID);

                            var pRDesc = "[付款银行:" + bankInfo.BankName + "-" + bankInfo.AccountsName + "][收款人:" +
                                         compInfo.WebSite + "]";
                            if (!string.IsNullOrEmpty(info.DealFlowNo))
                            {
                                pRDesc = "[付款银行:" + bankInfo.BankName + "-" + bankInfo.AccountsName + "][收款人:" + compInfo.WebSite + "]" + "[交易流水号：" +
                                         info.DealFlowNo + "]";
                            }

                            var dateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            //资金流备注
                            var wdes = string.Format("[往来付款（收款人:{0}；收款单位：{1}；付款类型：{2}；提交人：{3}；交易流水号：{4}；完成打款人：{5}），资金减少，{6}）]", compInfo.WebSite, compInfo.CompanyName, payType, applicantPersonnel, info.DealFlowNo, finishPersonnel, dateTime);
                            //往来帐备注
                            var rdes = string.Format("[往来付款（付款类型：{0}；申请人:{1}；完成打款人:{2}；付款银行：{3}；收款人：{4}；收款单位：{5}；交易流水号：{6}）,{7}）]", payType, applicantPersonnel, finishPersonnel, bankInfo.BankName + "-" + bankInfo.AccountsName, compInfo.WebSite, compInfo.CompanyName, info.DealFlowNo, dateTime);
                            //往来帐
                            var reckList = new List<ReckoningInfo>();
                            CompanyFundReceiptInfo receiveReceipt;
                            var result = CheckingData.CheckCompany(paymentType, info, rdes, reckList, info.IncludeStockNos.Split(',').ToList(), info.DebarStockNos.Split(',').ToList(), out receiveReceipt, out errorMsg);
                            if (!result)
                            {
                                RAM.Alert(string.Format("往来帐对账异常![{0}]", errorMsg));
                                return;
                            }
                            if (receiveReceipt != null)
                            {
                                _companyFundReceipt.Insert(receiveReceipt);
                            }
                            foreach (var reckoningInfo in reckList)
                            {
                                _reckoning.Insert(reckoningInfo, out errorMsg);

                                //_companyCussent.UpDatetCussentExtendInfo(info.CompanyID,
                                //"[付款单号：" + reckoningInfo.TradeCode + payType + "][备注说明：" + info.OtherDiscountCaption +
                                //"]" + char.ConvertFromUtf32(10));
                                var addResult = _companySubjectDiscountDao.Insert(new CompanySubjectDiscountInfo
                                {
                                    CompanyId = reckoningInfo.ThirdCompanyID,
                                    FilialeId = reckoningInfo.FilialeId,
                                    ID = Guid.NewGuid(),
                                    Datecreated = DateTime.Now,
                                    Income = 0,
                                    Memo = string.Format("[付款单号：{0}{1}][备注说明：{2}]{3}", reckoningInfo.TradeCode, payType,
                                    info.OtherDiscountCaption, char.ConvertFromUtf32(10)),
                                    PersonnelName = personnelInfo.RealName,
                                    MemoType = (int)MemoType.Subject
                                });
                                if (!addResult)
                                {
                                    RAM.Alert(string.Format("付款单{0}添加备注说明失败!", reckoningInfo.TradeCode));
                                    return;
                                }
                            }
                            //资金流
                            var wasteBookinfo = new WasteBookInfo(Guid.NewGuid(), bankInfo.BankAccountsId, _codeBll.GetCode(CodeType.RD),
                                wdes + "[备注说明：" + info.OtherDiscountCaption + "]",
                                -info.RealityBalance, (Int32)AuditingState.Yes,
                                (Int32)WasteBookType.Decrease, filialeId)
                            {
                                LinkTradeCode = info.ReceiptNo,
                                LinkTradeType = (int)WasteBookLinkTradeType.CompanyFundReceipt,
                                BankTradeCode = string.Empty,
                                State = (int)WasteBookState.Currently,
                                IsOut = bankInfo.IsMain
                            };
                            if (wasteBookinfo.Income != 0)
                                _wasteBook.Insert(wasteBookinfo);

                            if (receiveptFilialeId != Guid.Empty)
                            {
                                var receiveWasteBookInfo = new WasteBookInfo(Guid.NewGuid(), info.ReceiveBankAccountId, _codeBll.GetCode(CodeType.GI),
                                wdes + "[备注说明：" + info.OtherDiscountCaption + "]",
                                info.RealityBalance, (Int32)AuditingState.Yes,
                                (Int32)WasteBookType.Increase, receiveptFilialeId)
                                {
                                    LinkTradeCode = info.ReceiptNo,
                                    LinkTradeType = (int)WasteBookLinkTradeType.CompanyFundReceipt,
                                    BankTradeCode = string.Empty,
                                    State = (int)WasteBookState.Currently,
                                    IsOut = bankInfo.IsMain
                                };
                                if (receiveWasteBookInfo.Income != 0)
                                    _wasteBook.Insert(receiveWasteBookInfo);
                            }

                            if (info.Poundage != 0) //手续费
                            {
                                var pdes = string.Format("[往来付款（付款单据编号:{0}；[手续费] {1}）]", info.ReceiptNo, dateTime);
                                var newinfo = new WasteBookInfo(Guid.NewGuid(), bankInfo.BankAccountsId,
                                    _codeBll.GetCode(CodeType.RD), pdes,
                                    -info.Poundage, (Int32)AuditingState.Yes,
                                    (Int32)WasteBookType.Decrease, filialeId)
                                {
                                    LinkTradeCode = info.ReceiptNo,
                                    LinkTradeType = (int)WasteBookLinkTradeType.CompanyFundReceipt,
                                    BankTradeCode = string.Empty,
                                    State = (int)WasteBookState.Currently,
                                    IsOut = bankInfo.IsMain
                                };
                                _wasteBook.Insert(newinfo);
                            }
                            if (info.DiscountMoney != 0 || info.LastRebate != 0)
                            {
                                if (info.DiscountMoney != 0)
                                {
                                    var discountDescription =
                                    WebControl.RetrunUserAndTime("【折扣】[" + payType + "][今年折扣,详细见折扣+返利说明]" + pRDesc);
                                    //折扣往来帐
                                    var discountReckoningInfo = new ReckoningInfo(Guid.NewGuid(), filialeId, info.CompanyID,
                                        _codeBll.GetCode(CodeType.PY),
                                        discountDescription,
                                        info.DiscountMoney, (int)ReckoningType.Income,
                                        (int)ReckoningStateType.Currently,
                                        (int)CheckType.IsChecked,
                                        (int)AuditingState.Yes, originalTradeCode,
                                        Guid.Empty)
                                    {
                                        LinkTradeType = (int)ReckoningLinkTradeType.CompanyFundReceipt,
                                        IsOut = info.IsOut
                                    };
                                    var insertResult = _reckoning.Insert(discountReckoningInfo, out errorMsg);
                                    if (!insertResult) return;
                                }
                                if (info.LastRebate != 0)
                                {
                                    var lastRebateDescription =
                                    WebControl.RetrunUserAndTime("【返利】[" + payType + "][去年返利,详细见折扣+返利说明]" + pRDesc);
                                    //折扣往来帐
                                    var lastRebateReckoningInfo = new ReckoningInfo(Guid.NewGuid(), filialeId, info.CompanyID,
                                        _codeBll.GetCode(CodeType.PY),
                                        lastRebateDescription,
                                        info.LastRebate, (int)ReckoningType.Income,
                                        (int)ReckoningStateType.Currently,
                                        (int)CheckType.IsChecked,
                                        (int)AuditingState.Yes, originalTradeCode,
                                        Guid.Empty)
                                    {
                                        LinkTradeType = (int)ReckoningLinkTradeType.CompanyFundReceipt,
                                        IsOut = info.IsOut
                                    };
                                    var insertResult = _reckoning.Insert(lastRebateReckoningInfo, out errorMsg);
                                    if (!insertResult) return;
                                }

                                var addResult = _companySubjectDiscountDao.Insert(new CompanySubjectDiscountInfo
                                {
                                    CompanyId = info.CompanyID,
                                    FilialeId = info.FilialeId,
                                    ID = Guid.NewGuid(),
                                    Datecreated = DateTime.Now,
                                    Income = info.DiscountMoney + info.LastRebate,
                                    Memo = string.Format("[付款单号：{0}{1}][折扣+返利说明：{2}]{3}", info.ReceiptNo, payType,
                                    info.DiscountCaption, char.ConvertFromUtf32(10)),
                                    PersonnelName = personnelInfo.RealName,
                                    MemoType = (int)MemoType.Discount
                                });
                                if (!addResult)
                                {
                                    RAM.Alert(string.Format("付款单{0}添加折扣+返利说明失败!", info.ReceiptNo));
                                    return;
                                }
                            }
                            //remark = info.IsOut ? WebControl.RetrunUserAndTime("完成转入发票待索取状态") : WebControl.RetrunUserAndTime("完成");
                            remark = WebControl.RetrunUserAndTime("完成");
                            _companyFundReceipt.UpdateFundReceiptRemark(receiptId, remark);
                            //CompanyFundReceiptState receiptState = info.IsOut ? CompanyFundReceiptState.GettingInvoice : CompanyFundReceiptState.Finish;
                            var receiptState = CompanyFundReceiptState.Finish;
                            _companyFundReceipt.UpdateFundReceiptState(receiptId, receiptState);
                        }

                        #endregion

                        #region[收款]

                        if (info.ReceiptType == (int)CompanyFundReceiptType.Receive)
                        {
                            var rRDesc = "[收款银行:" + bankInfo.AccountsName + "-" + bankInfo.BankName + "][付款人:" +
                                         compInfo.WebSite + "]";
                            if (!string.IsNullOrEmpty(info.DealFlowNo))
                            {
                                rRDesc = "[收款银行:" + bankInfo.AccountsName + "-" + bankInfo.BankName + "][付款人:" + compInfo.WebSite + "]" +
                                         "[交易流水号：" + info.DealFlowNo + "]";
                            }
                            var receiveReckoningDesc =
                                WebControl.RetrunUserAndTime("[收款,日期间隔是:" + info.SettleStartDate.ToString("yyyy/MM/dd") +
                                                             "到" + info.SettleEndDate.ToString("yyyy/MM/dd") +
                                                             ",详细见备注说明]" + rRDesc);
                            var dateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            var wdes = string.Format("[往来收款（付款人:{1}；付款单位：{0}；交易流水号：{2}），资金增加，{3}）]", compInfo.CompanyName,
                                compInfo.WebSite, info.DealFlowNo, dateTime);
                            //往来帐
                            var reckoningInfo = new ReckoningInfo(Guid.NewGuid(), filialeId, info.CompanyID, reckNo,
                                receiveReckoningDesc,
                                -info.RealityBalance, (int)ReckoningType.Defray,
                                (int)ReckoningStateType.Currently,
                                (int)CheckType.IsChecked, (int)AuditingState.Yes,
                                info.ReceiptNo, Guid.Empty)
                            {
                                LinkTradeType = (int)ReckoningLinkTradeType.CompanyFundReceipt,
                                IsOut = info.IsOut
                            };

                            //资金流
                            var wasteBookinfo = new WasteBookInfo(Guid.NewGuid(), bankInfo.BankAccountsId, tradecode,
                                wdes + "[备注说明：" + info.OtherDiscountCaption + "]",
                                info.RealityBalance, (Int32)AuditingState.Yes,
                                (Int32)WasteBookType.Increase, filialeId)
                            {
                                LinkTradeCode = info.ReceiptNo,
                                LinkTradeType = (int)WasteBookLinkTradeType.CompanyFundReceipt,
                                BankTradeCode = string.Empty,
                                State = (int)WasteBookState.Currently,
                                IsOut = bankInfo.IsMain
                            };
                            _reckoning.Insert(reckoningInfo, out errorMsg);
                            if (wasteBookinfo.Income != 0)
                                _wasteBook.Insert(wasteBookinfo);

                            if (receiveptFilialeId != Guid.Empty)
                            {
                                var receiveWasteBookInfo = new WasteBookInfo(Guid.NewGuid(), info.ReceiveBankAccountId, _codeBll.GetCode(CodeType.RD),
                                wdes + "[备注说明：" + info.OtherDiscountCaption + "]",
                                -info.RealityBalance, (Int32)AuditingState.Yes,
                                (Int32)WasteBookType.Decrease, receiveptFilialeId)
                                {
                                    LinkTradeCode = info.ReceiptNo,
                                    LinkTradeType = (int)WasteBookLinkTradeType.CompanyFundReceipt,
                                    BankTradeCode = string.Empty,
                                    State = (int)WasteBookState.Currently,
                                    IsOut = bankInfo.IsMain
                                };
                                if (receiveWasteBookInfo.Income != 0)
                                    _wasteBook.Insert(receiveWasteBookInfo);
                            }
                            var addSubjectResult = _companySubjectDiscountDao.Insert(new CompanySubjectDiscountInfo
                            {
                                CompanyId = reckoningInfo.ThirdCompanyID,
                                FilialeId = bankInfo.IsMain ? reckoningInfo.FilialeId : _reckoningElseFilialeid,
                                ID = Guid.NewGuid(),
                                Datecreated = DateTime.Now,
                                Income = 0,
                                Memo = string.Format("[收款单号：{0}时间间隔是：{1}到{2}][备注说明：{3}]{4}", reckoningInfo.TradeCode, info.SettleStartDate.ToString("yyyy/MM/dd"),
                                info.SettleEndDate.ToString("yyyy/MM/dd"),
                                info.OtherDiscountCaption, char.ConvertFromUtf32(10)),
                                PersonnelName = personnelInfo.RealName,
                                MemoType = (int)MemoType.Subject
                            });
                            if (!addSubjectResult)
                            {
                                RAM.Alert(string.Format("收款单{0}添加备注说明失败!", reckoningInfo.TradeCode));
                                return;
                            }
                            if (info.DiscountMoney != 0 || info.LastRebate != 0)
                            {
                                if (info.DiscountMoney != 0)
                                {
                                    var discountDesc =
                                    WebControl.RetrunUserAndTime("[收款,日期间隔是:" +
                                                                 info.SettleStartDate.ToString("yyyy/MM/dd") + "到" +
                                                                 info.SettleEndDate.ToString("yyyy/MM/dd") +
                                                                 "][收款折扣,详细见折扣说明]");
                                    //折扣往来帐
                                    var discountReckoningInfo = new ReckoningInfo(Guid.NewGuid(), filialeId, info.CompanyID,
                                        _codeBll.GetCode(CodeType.GT), discountDesc,
                                        -info.DiscountMoney,
                                        (int)ReckoningType.Defray,
                                        (int)ReckoningStateType.Currently,
                                        (int)CheckType.IsChecked,
                                        (int)AuditingState.Yes, info.ReceiptNo,
                                        Guid.Empty)
                                    {
                                        LinkTradeType = (int)ReckoningLinkTradeType.CompanyFundReceipt,
                                        IsOut = info.IsOut
                                    };
                                    _reckoning.Insert(discountReckoningInfo, out errorMsg);
                                }
                                if (info.LastRebate != 0)
                                {
                                    var lastRebateDesc =
                                    WebControl.RetrunUserAndTime("[收款,日期间隔是:" +
                                                                 info.SettleStartDate.ToString("yyyy/MM/dd") + "到" +
                                                                 info.SettleEndDate.ToString("yyyy/MM/dd") +
                                                                 "][返利,详细见折扣+返利说明]");
                                    //折扣往来帐
                                    var lastRebateReckoningInfo = new ReckoningInfo(Guid.NewGuid(), filialeId, info.CompanyID,
                                        _codeBll.GetCode(CodeType.GT), lastRebateDesc,
                                        -info.LastRebate,
                                        (int)ReckoningType.Defray,
                                        (int)ReckoningStateType.Currently,
                                        (int)CheckType.IsChecked,
                                        (int)AuditingState.Yes, info.ReceiptNo,
                                        Guid.Empty)
                                    {
                                        LinkTradeType = (int)ReckoningLinkTradeType.CompanyFundReceipt,
                                        IsOut = info.IsOut
                                    };
                                    _reckoning.Insert(lastRebateReckoningInfo, out errorMsg);
                                }
                                var addResult = _companySubjectDiscountDao.Insert(new CompanySubjectDiscountInfo
                                {
                                    CompanyId = info.CompanyID,
                                    FilialeId = info.IsOut ? filialeId : _reckoningElseFilialeid,
                                    ID = Guid.NewGuid(),
                                    Datecreated = DateTime.Now,
                                    Income = info.DiscountMoney + info.LastRebate,
                                    Memo = string.Format("[收款单号：{0}日期间隔是：{1}到{2}][折扣+返利说明：{3}]{4}", info.ReceiptNo, info.SettleStartDate.ToString("yyyy/MM/dd"),
                                    info.SettleEndDate.ToString("yyyy/MM/dd"),
                                    info.DiscountCaption, char.ConvertFromUtf32(10)),
                                    PersonnelName = personnelInfo.RealName,
                                    MemoType = (int)MemoType.Discount
                                });
                                if (!addResult)
                                {
                                    RAM.Alert(string.Format("收款单{0}添加折扣+返利说明失败!", info.ReceiptNo));
                                    return;
                                }
                            }
                            remark = WebControl.RetrunUserAndTime("完成");
                            _companyFundReceipt.UpdateFundReceiptRemark(receiptId, remark);
                            _companyFundReceipt.UpdateFundReceiptState(receiptId, CompanyFundReceiptState.Finish);
                        }

                        #endregion
                    }
                    _companyFundReceipt.SetDateTime(receiptId, 3);
                    RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
                    ts.Complete();
                }
                catch (Exception ex)
                {
                    RAM.Alert("完成打款失败!" + ex.Message);
                }
                finally
                {
                    ts.Dispose();
                }
            }
        }
        #endregion

        #region[完成退回]
        protected void BtnBackClick(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(RTB_BackReason.Text))
            {
                RAM.Alert("请填写退回理由");
                return;
            }
            try
            {
                CompanyFundReceiptInfo info = _companyFundReceipt.GetCompanyFundReceiptInfo(RceiptId);
                if (info != null && info.ReceiptStatus == (int)CompanyFundReceiptState.PayBack)
                {
                    RAM.Alert("系统提示：退回失败，单据状态已改变！");
                    return;
                }
                //Modify by liangcanren at 2015-05-07
                using (var ts = new TransactionScope(TransactionScopeOption.Required))
                {
                    string remark = WebControl.RetrunUserAndTime("完成打款退回");
                    //单据所绑定的“支付银行ID”应该被清空  门店不清空
                    if (!IsShopReceipt)
                    {
                        var isSuccess = _companyFundReceipt.SetPayBankAccountsId(RceiptId, Guid.Empty);
                        if (!isSuccess)
                        {
                            RAM.Alert("系统提示：退回失败，请尝试刷新稍后再试！");
                            return;
                        }
                    }
                    var bll = new BLL.Implement.Inventory.CompanyFundReceipt(_companyFundReceipt);
                    bll.UpdateFundReceiptState(RceiptId, CompanyFundReceiptState.PayBack, remark);
                    CompanyFundReceiptInfoModel = _companyFundReceipt.GetCompanyFundReceiptInfo(RceiptId);
                    ts.Complete();
                    //IsFromShop(CompanyFundReceiptState.PayBack, remark);
                }
            }
            catch (Exception ex)
            {
                RAM.Alert("完成打款退回失败!" + ex.Message);
                return;
            }
            RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
        }
        #endregion

        #region[选择公司后加载出对应的银行账号]
        protected void Rcb_FilialeListSelectedIndexChanged(object o, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            CompanyFundReceiptInfoModel = _companyFundReceipt.GetCompanyFundReceiptInfo(RceiptId);
            var companyId = CompanyFundReceiptInfoModel.CompanyID;
            if (companyId == Guid.Empty)
            {
                RAM.Alert("请先选择往来单位");
            }
        }
        #endregion

        #region  付款审核银行选择转至申请打款 add by liangcanren at 2015-04-15

        public void GetBank(Guid filialeId)
        {
            RcbBankAccount.Items.Clear();
            RcbElseBankAccount.Items.Clear();
            RcbBankAccount.Text = string.Empty;
            RcbElseBankAccount.Text = string.Empty;
            IList<BankAccountInfo> bankList;
            var shopInfo = ShopFilialeList.FirstOrDefault(act => act.ID == CompanyFundReceiptInfoModel.CompanyID);

            if (filialeId == _reckoningElseFilialeid || shopInfo != null)
            {
                bankList = _bankAccountDao.GetListByNotIsMain(false).OrderBy(act => act.OrderIndex).ToList();
                span_titile.Visible = false;
                RcbElseBankAccount.Visible = false;
            }
            else
            {
                bankList = _bankAccountDao.GetListByTargetId(filialeId).OrderBy(act => act.OrderIndex).ToList();
                span_titile.Visible = true;
                RcbElseBankAccount.Visible = true;
            }
            if (bankList.Count > 0)
            {
                bool hasMain = false;
                foreach (var info in bankList.Where(ent => ent.IsMain))
                {
                    hasMain = true;
                    RcbBankAccount.Items.Add(new RadComboBoxItem(string.Format("{0}-{1}[{2}]", info.BankName, info.AccountsName,
                        WebControl.NumberSeparator(info.NonceBalance)), info.BankAccountsId.ToString()));
                }
                if (!hasMain)
                {
                    foreach (var info in bankList.Where(ent => !ent.IsMain))
                    {
                        RcbBankAccount.Items.Add(new RadComboBoxItem(string.Format("{0}-{1}[{2}]", info.BankName, info.AccountsName,
                            WebControl.NumberSeparator(info.NonceBalance)), info.BankAccountsId.ToString()));
                    }
                }
                var elsebankList = _bankAccountDao.GetListByNotIsMain(false).OrderBy(act => act.OrderIndex).ToList();
                foreach (var info in elsebankList)
                {
                    RcbElseBankAccount.Items.Add(new RadComboBoxItem(string.Format("{0}-{1}[{2}]", info.BankName, info.AccountsName,
                        WebControl.NumberSeparator(info.NonceBalance)), info.BankAccountsId.ToString()));
                }
                RcbElseBankAccount.Items.Insert(0, new RadComboBoxItem("其他银行列表", Guid.Empty.ToString()));
                RcbBankAccount.Items.Insert(0, new RadComboBoxItem("公司银行列表", Guid.Empty.ToString()));
                RcbBankAccount.SelectedIndex = 0;
                RcbElseBankAccount.SelectedIndex = 0;
                if (shopInfo != null)
                {
                    RcbBankAccount.SelectedValue = CompanyFundReceiptInfoModel.PayBankAccountsId.ToString();
                }
                else
                {
                    var item = bankList.FirstOrDefault(ent => ent.BankAccountsId == CompanyFundReceiptInfoModel.PayBankAccountsId);
                    if (item != null)
                        RcbBankAccount.SelectedValue = CompanyFundReceiptInfoModel.PayBankAccountsId.ToString();
                    else
                        RcbElseBankAccount.SelectedValue = CompanyFundReceiptInfoModel.PayBankAccountsId.ToString();
                    if (CompanyFundReceiptInfoModel.ReceiptStatus >= (int)CompanyFundReceiptState.Executed)
                    {
                        RcbBankAccount.Enabled = CompanyFundReceiptInfoModel.PayBankAccountsId == Guid.Empty;
                        RcbElseBankAccount.Enabled = CompanyFundReceiptInfoModel.PayBankAccountsId == Guid.Empty;
                    }
                }
            }
            else
            {
                RcbBankAccount.Items.Add(new RadComboBoxItem("未找到相关银行帐号", Guid.Empty.ToString()));
            }
        }
        #endregion

        /// <summary>
        /// 添加保存，保存选择的银行 手续费
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void BtnSaveClick(object sender, EventArgs e)
        {
            var bankAccountsId = RcbBankAccount.SelectedValue;
            var elseBankAccountsId = RcbElseBankAccount.SelectedValue;
            if ((string.IsNullOrWhiteSpace(bankAccountsId) || bankAccountsId == Guid.Empty.ToString()) && (string.IsNullOrWhiteSpace(elseBankAccountsId) || elseBankAccountsId == Guid.Empty.ToString()))
            {
                RAM.Alert(string.Format("系统提示：{0}", Type == 2 ? "本公司银行帐号未绑定！" : "请选择银行！"));
                return;
            }
            if ((!string.IsNullOrWhiteSpace(bankAccountsId) && bankAccountsId != Guid.Empty.ToString()) && (!string.IsNullOrWhiteSpace(elseBankAccountsId) && elseBankAccountsId != Guid.Empty.ToString()))
            {
                RAM.Alert("系统提示：公司帐号和其他公司帐号只能选择其一！");
                return;
            }

            CompanyFundReceiptInfo info = _companyFundReceipt.GetCompanyFundReceiptInfo(RceiptId) ?? new CompanyFundReceiptInfo();
            if (info.ReceiptID == Guid.Empty)
            {
                RAM.Alert(string.Format("单据未找到"));
                return;
            }

            Guid bankAccountId = !string.IsNullOrWhiteSpace(bankAccountsId) && bankAccountsId != Guid.Empty.ToString() ? new Guid(bankAccountsId) :
                !string.IsNullOrWhiteSpace(elseBankAccountsId) && elseBankAccountsId != Guid.Empty.ToString() ? new Guid(elseBankAccountsId) : Guid.Empty;
            if ((!string.IsNullOrWhiteSpace(bankAccountsId) && bankAccountsId != Guid.Empty.ToString())
                && (!string.IsNullOrWhiteSpace(elseBankAccountsId) && elseBankAccountsId != Guid.Empty.ToString()))
            {
                bankAccountId = Guid.Empty;
            }

            try
            {
                using (var ts = new TransactionScope(TransactionScopeOption.Required))
                {
                    if (!string.IsNullOrEmpty(TbFlowNo.Text) && info.DealFlowNo != TbFlowNo.Text)
                    {
                        _companyFundReceipt.UpdateDealFlowNo(RceiptId, TbFlowNo.Text);
                    }
                    _companyFundReceipt.UpdatePoundage(RceiptId, decimal.Parse(string.IsNullOrEmpty(TB_Poundage.Text) ? "0" : TB_Poundage.Text));
                    var isSuccess = true;
                    if (bankAccountId != Guid.Empty)
                    {
                        isSuccess = _companyFundReceipt.SetPayBankAccountsId(RceiptId, bankAccountId);
                    }
                    if (!isSuccess)
                    {
                        RAM.Alert("系统提示：保存失败！");
                    }
                    else
                    {
                        RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
                        ts.Complete();
                    }
                }
            }
            catch (Exception ex)
            {
                RAM.Alert(ex.Message);
            }
        }

        public decimal GetTotalloled(List<string> stockOrderNos, List<string> removeNos)
        {
            if (RDP_EndDate.SelectedDate != null && RDP_StartDate.SelectedDate != null && RDP_EndDate.SelectedDate > RDP_StartDate.SelectedDate)
            {
                DateTime start = Convert.ToDateTime(RDP_StartDate.SelectedDate);
                DateTime end = Convert.ToDateTime(RDP_EndDate.SelectedDate);
                var dic = _reckoning.GetTotalledByDate(new Guid(RCB_CompanyList.SelectedValue), new Guid(RCB_FilialeList.SelectedValue), start, end.AddDays(1).AddSeconds(-1), (int)CheckType.NotCheck, stockOrderNos, removeNos);
                return dic.Sum(act => act.Value);
            }
            return 0;
        }

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
                //InvoiceTypeName.Text = _companyFundReceiptInvoiceInfoList.First().InvoiceType == 0 ? string.Empty : EnumAttribute.GetKeyName((CostReportInvoiceType)_companyFundReceiptInvoiceInfoList.First().InvoiceType);
            }
            RG_InvoiceList.DataSource = dataList;
        }
    }
}