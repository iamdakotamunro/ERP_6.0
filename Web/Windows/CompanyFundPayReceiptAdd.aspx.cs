using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Transactions;
using System.Web.UI.WebControls;
using ERP.BLL.Implement;
using ERP.BLL.Implement.Inventory;
using ERP.DAL.Factory;
using ERP.DAL.Implement.Company;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.ICompany;
using ERP.DAL.Interface.IInventory;
using ERP.DAL.Interface.IUtilities;
using ERP.Enum;
using ERP.Environment;
using ERP.Model;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Framework.Data;
using Keede.Ecsoft.Model;
using OperationLog.Core;
using Telerik.Web.UI;
using Telerik.Web.UI.Calendar;
using WebControl = ERP.UI.Web.Common.WebControl;
using Framework.Common;
using Lonme.WebControls;
using ERP.SAL.LogCenter;

/*
 * 最后修改人：刘彩军
 * 修改时间：2012-February-6th
 * 修改内容：需求整改
 */
namespace ERP.UI.Web.Windows
{
    ///<summary> 添加付款单据
    ///</summary>
    public partial class CompanyFundPayReceiptAdd : WindowsPage
    {
        protected PersonnelInfo MyPersonnelInfo;
        protected CodeManager MyCode = new CodeManager();
        readonly ICompanyBankAccountBind _companyBankAccountBindBll = new CompanyBankAccountBindDao(GlobalConfig.DB.FromType.Read);
        private readonly IStorageRecordDao _storageRecordDao = new StorageRecordDao(GlobalConfig.DB.FromType.Write);
        private readonly IReckoning _reckoning = new Reckoning(GlobalConfig.DB.FromType.Read);
        private readonly IPurchasing _purchasing = new Purchasing(GlobalConfig.DB.FromType.Read);
        readonly ICompanyCussent _companyCussent = new CompanyCussent(GlobalConfig.DB.FromType.Read);
        readonly ICompanyFundReceipt _companyFundReceipt = new DAL.Implement.Inventory.CompanyFundReceipt(GlobalConfig.DB.FromType.Write);
        readonly ICompanyFundReceiptInvoice _companyFundReceiptInvoice = new ERP.DAL.Implement.Inventory.CompanyFundReceiptInvoice(GlobalConfig.DB.FromType.Write);
        private readonly IDocumentRedDao _documentRedDto = new DocumentRedDao(GlobalConfig.DB.FromType.Read);
        private static readonly IUtility _utility = InventoryInstance.GetUtilityDalDao(GlobalConfig.DB.FromType.Write);
        //其他公司
        private readonly Guid _reckoningElseFilialeid = new Guid(ConfigManage.GetConfigValue("RECKONING_ELSE_FILIALEID"));
        SubmitController _submitController;
        public bool IsVisible = true;

        protected SubmitController CreateSubmitInstance()
        {
            if (ViewState["SubmitController"] == null)
            {
                _submitController = new SubmitController(Guid.NewGuid());
                ViewState["SubmitController"] = _submitController;
            }
            return (SubmitController)ViewState["SubmitController"];
        }


        #region[Page_Load]
        protected void Page_Load(object sender, EventArgs e)
        {
            _submitController = CreateSubmitInstance();
            if (!IsPostBack)
            {
                if (Request.QueryString["ID"] != null)
                {
                    CompanyFundReceiptInfoModel = _companyFundReceipt.GetCompanyFundReceiptInfo(new Guid(Request.QueryString["ID"]));
                    CompanyFundReceiptInvoiceInfoList = _companyFundReceiptInvoice.Getlmshop_CompanyFundReceiptInvoiceByReceiptID(new Guid(Request.QueryString["ID"]));
                    LB_Save.Visible = true;
                    LB_Inster.Visible = false;
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
                        RbNormal.Checked = true ;
                        RbUrgent.Checked = false;
                        LbUrgent.Visible = false;
                        LbUrgentTitle.Visible = false;
                        RtbUrgent.Visible = false;
                    }
                    if (Request.QueryString["Type"] != null && Request.QueryString["Type"].Equals("1"))
                    {
                        #region 补充发票信息
                        LB_Save.Visible = false;
                        LB_Inster.Visible = false;
                        LB_Cancel.Visible = false;
                        RbUrgent.Enabled = false;
                        RtbUrgent.Enabled = false;
                        RbNormal.Enabled = false;
                        #endregion
                    }
                    else
                    {
                        #region 如果该单据对应的发票信息有任何一条不处于”未提交“状态，则该单据对应的发票信息在此页面不能增加、修改、删除，此操作可在”发票操作“页面进行
                        //查询该单据发票除”未提交“以外的所有状态的发票信息
                        var invoiceList = _companyFundReceiptInvoice.Getlmshop_CompanyFundReceiptInvoiceByReceiptID(new Guid(Request.QueryString["ID"])).Where(p => (new int?[] { (int)CompanyFundReceiptInvoiceState.Submit, (int)CompanyFundReceiptInvoiceState.Receive, (int)CompanyFundReceiptInvoiceState.Authenticate, (int)CompanyFundReceiptInvoiceState.Verification }).Contains(p.InvoiceState));
                        if (invoiceList.Any())
                        {
                            btn_Upload.Enabled = false;
                            txt_BillingUnit.ReadOnly = true;
                            RG_InvoiceList.Columns[7].Visible = false;
                            IsVisible = false;
                        }
                        #endregion
                    }
                }
                else
                {
                    LB_Save.Visible = false;
                    LB_Inster.Visible = true;
                    RbNormal.Checked = true;
                    LbUrgent.Visible = false;
                    LbUrgentTitle.Visible = false;
                    RtbUrgent.Visible = false;
                }
                MyCode = new CodeManager();

                GetCompanyList();
                DIV_Date.Visible = false;
                DIV_Goods.Visible = false;
                DIV_Orders.Visible = false;
                RCB_CompanyList.Enabled = CompanyFundReceiptInfoModel.CompanyID == Guid.Empty;
                RCB_FilialeList.Enabled = CompanyFundReceiptInfoModel.FilialeId == Guid.Empty;
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
                    RCB_FilialeList.SelectedValue = CompanyFundReceiptInfoModel.FilialeId.ToString();
                }
                txt_PaymentDate.Text = DateTime.Now.ToString("yyyy-MM");
                BindValue();

                var flag = Request.QueryString["Flag"];
                if (flag != null && flag.Equals("1"))//type=1表示查看
                {
                    LB_Inster.Visible = LB_Save.Visible = LB_Cancel.Visible = false;
                    RG_InvoiceList.Columns[7].Visible = false;
                }
                if (int.Parse(rbl_InvoiceType.SelectedValue).Equals((int)CostReportInvoiceType.Invoice))
                {
                    RG_InvoiceList.MasterTableView.Columns.FindByUniqueName("NoTaxAmount").Display = false;
                    RG_InvoiceList.MasterTableView.Columns.FindByUniqueName("Tax").Display = false;
                }
            }
        }
        #endregion

        #region[获取往来单位数据信息，包含供应商和物流公司]
        /// <summary>
        /// 获取往来单位数据信息，包含供应商和物流公司
        /// </summary>
        protected void GetCompanyList()
        {
            ICompanyCussent companyCussent = new CompanyCussent(GlobalConfig.DB.FromType.Read);
            int[] companyType = { (int)CompanyType.Suppliers, (int)CompanyType.Express, (int)CompanyType.Vendors };
            var data = companyCussent.GetCompanyCussentList().Where(ent => (companyType.Contains(ent.CompanyType) || ent.RelevanceFilialeId != Guid.Empty) && ent.State == (int)State.Enable);

            RCB_CompanyList.DataSource = data;
            RCB_CompanyList.DataTextField = "CompanyName";
            RCB_CompanyList.DataValueField = "CompanyId";
            RCB_CompanyList.DataBind();
            RCB_CompanyList.Items.Add(new RadComboBoxItem("", Guid.Empty.ToString()));
            RCB_CompanyList.SelectedValue = CompanyFundReceiptInfoModel.CompanyID.ToString();
        }
        #endregion

        #region[获取公司数据信息，包含供应商和物流公司]
        /// <summary>
        /// 获取公司数据信息，包含供应商和物流公司
        /// </summary>
        protected void GetFilialeList()
        {
            RCB_FilialeList.DataSource = CacheCollection.Filiale.GetList();
            RCB_FilialeList.DataTextField = "Name";
            RCB_FilialeList.DataValueField = "ID";
            RCB_FilialeList.DataBind();
            RCB_FilialeList.Items.Add(new RadComboBoxItem("", Guid.Empty.ToString()));
            RCB_FilialeList.SelectedValue = Guid.Empty.ToString();
        }
        #endregion

        #region[在应付金额文本改变后，计算货币大写]
        /// <summary>
        /// 在应付金额文本改变后，计算货币大写
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RtbRealityBalanceTextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(RTB_RealityBalance.Text))
            {
                var realityBalance = RTB_RealityBalance.Text;
                if (RTB_RealityBalance.Text.Contains(','))
                {
                    realityBalance = WebControl.NumberRecovery(realityBalance);
                    RTB_RealityBalance.Text = string.Format("{0}", realityBalance);
                }
                LB_UpperCaseMoney.Text = WebUtility.ConvertSum(realityBalance);
            }
        }
        #endregion

        #region[在选择往来单位变动后，初始化一些控件的值]
        /// <summary>
        /// 在选择往来单位变动后，初始化一些控件的值
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        protected void RcbCompanyListSelectedIndexChanged(object o, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            RTB_SettleBalance.Text = "0";
            if (e.Value.Trim() != String.Empty)
            {
                var companyId = new Guid(RCB_CompanyList.SelectedValue);
                RCB_FilialeList.Enabled = companyId != Guid.Empty;
                if (companyId == Guid.Empty)
                {
                    RCB_FilialeList.Items.Clear();
                    return;
                }
                var filialeId = _companyCussent.GetRelevanceFilialeIdByCompanyId(companyId);
                if (filialeId != Guid.Empty)
                {
                    RCB_FilialeList.Items.Clear();
                    var filialeIds = CacheCollection.Filiale.GetHostingAndSaleFilialeList();
                    ShowFiliale(filialeIds.Where(ent => ent.ID != filialeId).ToList());
                }
                CompanyFundReceiptInfoModel.CompanyID = companyId;

                if (RB_Date.Checked)
                {
                    RtbRemove.Text = "";
                    RtbInclude.Text = "";
                    RTB_SettleBalance.Text = "0.00";
                    RTB_RealityBalance.Text = "0.00";
                }
                if (RbAdvance.Checked)
                {
                    //decimal totalNumber = Reckoning.GetReckoningNonceTotalled(new Guid(e.Value), DateTime.Parse("1999-09-09 00:00:00.000"), DateTime.Now, (int)ReckoningStateType.Currently);
                    //RTB_SettleBalance.Text = totalNumber != 0 ? (-totalNumber).ToString("#.00") : "0";
                }
                else
                {
                    RDP_EndDate.Enabled = true;
                    if (RDP_EndDate.SelectedDate != null)
                    {
                        RDP_EndDate.Clear();
                    }
                    RTB_SettleBalance.Text = "0.00";
                    RTB_RealityBalance.Text = "0.00";
                    LB_UpperCaseMoney.Text = "";
                    if (filialeId == Guid.Empty && RCB_FilialeList.SelectedValue.Trim() != "" && RCB_FilialeList.SelectedValue != Guid.Empty.ToString())
                    {
                        List<CompanyFundReceiptInfo> companyFundReceiptInfos = _companyFundReceipt.GetFundReceiptListByCompanyID(new Guid(e.Value), true).OrderByDescending(c => c.SettleEndDate).ToList();
                        if (companyFundReceiptInfos.Count > 0)
                        {
                            var tempList = companyFundReceiptInfos.Where(act => act.FilialeId == new Guid(RCB_FilialeList.SelectedValue)).Take(1).ToList();
                            var info = tempList.Count > 0 ? tempList[0] : companyFundReceiptInfos[0];
                            if (info.SettleEndDate != DateTime.Parse("1999-09-09 00:00:00.000"))
                            {
                                RDP_StartDate.SelectedDate = info.SettleEndDate.AddDays(1);
                                RDP_StartDate.Enabled = false;
                            }
                            else
                            {
                                RDP_StartDate.SelectedDate = null;
                                RDP_StartDate.Enabled = true;
                            }
                        }
                        else
                        {
                            RDP_StartDate.SelectedDate = null;
                            RDP_StartDate.Enabled = true;
                        }
                    }
                }
                BindCompany(companyId);
            }
        }

        protected void BindCompany(Guid companyId)
        {
            if (!string.IsNullOrEmpty(RCB_FilialeList.SelectedValue) && RCB_FilialeList.SelectedValue != Guid.Empty.ToString())
            {
                var info = _companyBankAccountBindBll.GetCompanyBankAccountIdBind(companyId, new Guid(RCB_FilialeList.SelectedValue))
                    ?? _companyBankAccountBindBll.GetCompanyBankAccountBindInfo(companyId, new Guid(RCB_FilialeList.SelectedValue));
                if (info != null && info.CompanyId != Guid.Empty)
                {
                    TB_Payee.Text = info.WebSite;
                    RTB_CompBank.Text = info.BankAccounts;
                    RTB_CompBankAccount.Text = info.AccountsNumber;
                }
                else
                {
                    CompanyCussentInfo compInfo = _companyCussent.GetCompanyCussent(companyId);
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
            }
        }
        #endregion

        #region[判断折扣说明是否显示]
        protected void RtbDiscountMoneyTextChanged(object sender, EventArgs e)
        {
            if ((!string.IsNullOrEmpty(RTB_DiscountMoney.Text) && RTB_DiscountMoney.Text != "0")
                || !string.IsNullOrEmpty(RtbLastRebate.Text) && RtbLastRebate.Text != "0")
                DIV_DiscountCaption.Visible = true;
            else
                DIV_DiscountCaption.Visible = false;
        }
        #endregion

        #region[选择按日期付款]
        protected void RbDateCheckedChanged(object sender, EventArgs e)
        {
            if (RB_Date.Checked)
            {
                DivStockNos.Visible = true;
                DIV_Date.Visible = true;
                DIV_Goods.Visible = false;
                DIV_Orders.Visible = false;
                DIV_Related.Visible = false;
                RCB_CompanyList.SelectedValue = Guid.Empty.ToString();
                RCB_CompanyList.Text = "";
                RCB_CompanyList.Enabled = true;
                RTB_SettleBalance.Text = "";
                DIV_BackBalance.Visible = true;
                RCB_CompanyList.Enabled = true;
                RCB_FilialeList.Enabled = true;
                LB_BankAmount.Text = "当期余额：";
                DIV_DiscountMoney.Visible = true;
                LB_Date.Visible = true;
                RDP_StartDate.Visible = true;
                RDP_EndDate.Visible = true;
                RtbLastRebate.Text = "0";
                RTB_DiscountMoney.Text = "0";
                RTB_DiscountCaption.Text = string.Empty;
                RTB_PayOrder.Text =
                RTB_ReturnOrder.Text =
                Lit_ReturnOrder.Text =
                Lit_PayOrder.Text = string.Empty;
                Lit_PayOrderMoney.Text =
                Lit_ReturnOrderMoney.Text = "0";
            }
            else
            {
                DivStockNos.Visible = false;
                DIV_Date.Visible = false;
                DIV_Goods.Visible = false;
                DIV_Orders.Visible = false;
            }
            BindCompany(new Guid(RCB_CompanyList.SelectedValue));
        }
        #endregion

        #region[在选择结算结束日期后发生的事件：获取后台余款]
        /// <summary>
        /// 在选择结算结束日期后发生的事件：获取后台余款
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RdpEndDateOnSelectedDateChanged(object sender, SelectedDateChangedEventArgs e)
        {
            RtbRemove.Text = "";
            RtbInclude.Text = "";

            var start = RDP_StartDate.SelectedDate ?? DateTime.MinValue;
            var end = RDP_EndDate.SelectedDate ?? DateTime.MinValue;
            if (start != DateTime.MinValue && end != DateTime.MinValue)
            {
                if (!IsSerarchYear(start, end, Convert.ToInt32(GlobalConfig.KeepYear)))
                {
                    RDP_EndDate.SelectedDate = null;
                    RAM.Alert("温馨提示：不支持当前时间段搜索，请检查配置文件！");
                    return;
                }
            }

            if (RCB_CompanyList.SelectedValue.Trim() != "")
            {
                var companyId = new Guid(RCB_CompanyList.SelectedValue);
                if (RDP_StartDate.SelectedDate != null && RDP_EndDate.SelectedDate != null)
                {
                    if (RDP_EndDate.SelectedDate > DateTime.Now)
                    {
                        RDP_EndDate.SelectedDate = DateTime.Now;
                    }
                    if (RDP_EndDate.SelectedDate > RDP_StartDate.SelectedDate)
                    {
                        List<CompanyFundReceiptInfo> fundReceiptInfos = _companyFundReceipt.GetAllFundReceiptListByCompanyId(companyId).Where(c => c.ReceiptStatus != (int)CompanyFundReceiptState.Cancel && c.FilialeId == new Guid(RCB_FilialeList.SelectedValue)).OrderByDescending(c => c.SettleEndDate).ToList();
                        if (fundReceiptInfos.Count > 0)
                        {
                            if (RDP_EndDate.SelectedDate <= fundReceiptInfos[0].SettleEndDate)
                            {
                                RDP_EndDate.SelectedDate = null;
                                RAM.Alert("该时间段已经包含在其它付款单中或者已经付款");
                                return;
                            }
                        }
                        var filialeId = new Guid(RCB_FilialeList.SelectedValue);
                        if (filialeId != Guid.Empty)
                        {
                            //排除相关单据
                            var tradeCodeList = _companyFundReceipt.CheckExistForDate(new Guid(RCB_CompanyList.SelectedValue), new Guid(RCB_FilialeList.SelectedValue), DateTime.Parse(RDP_StartDate.SelectedDate.ToString()), DateTime.Parse(RDP_EndDate.SelectedDate.ToString()));
                            if (tradeCodeList.Any())
                            {
                                RtbRemove.Text += tradeCodeList.Aggregate(string.Empty, (current, item) => current + ("," + item)).Substring(1);
                            }

                            decimal totalNumber = GetTotalloled(new List<string>(), tradeCodeList);
                            //Reckoning.GetReckoningNonceTotalled(filialeId, companyId, (DateTime)RDP_StartDate.SelectedDate, RDP_EndDate.SelectedDate > DateTime.Now ? DateTime.Now : ((DateTime)RDP_EndDate.SelectedDate).AddDays(1).AddSeconds(-1), (int)ReckoningStateType.Currently);
                            RTB_SettleBalance.Text = totalNumber != 0 ? WebControl.RemoveDecimalEndZero(-totalNumber) : "0";
                        }
                    }
                    else
                    {
                        RTB_SettleBalance.Text = "0";
                        RDP_EndDate.SelectedDate = null;
                        RAM.Alert("系統提示：截止日期必须大于开始日期！");
                    }
                }
            }
            else
            {
                RDP_EndDate.SelectedDate = null;
                ClientScript.RegisterStartupScript(GetType(), "js", "<script>alert('请先选择公司单位！');</script>");
            }
        }
        #endregion

        #region[选择按入库单据付款]
        protected void RbInvoiceCheckedChanged(object sender, EventArgs e)
        {
            DivStockNos.Visible = false;
            if (RB_Invoice.Checked)
            {
                DIV_Date.Visible = false;
                DIV_Goods.Visible = false;
                DIV_Orders.Visible = true;
                DIV_Related.Visible = true;
                TB_Orders.Text = "";
                RCB_CompanyList.SelectedValue = Guid.Empty.ToString();
                RCB_CompanyList.Text = "";
                RTB_SettleBalance.Text = "";
                DIV_BackBalance.Visible = true;
                RCB_CompanyList.Enabled = false;
                RCB_FilialeList.Enabled = false;
                LB_BankAmount.Text = "单据总额：";
                DIV_DiscountMoney.Visible = true;

                RtbLastRebate.Text = "0";
                RTB_DiscountMoney.Text = "0";
                RTB_DiscountCaption.Text = string.Empty;
                RTB_PayOrder.Text =
                RTB_ReturnOrder.Text =
                Lit_ReturnOrder.Text =
                Lit_PayOrder.Text = string.Empty;
                Lit_PayOrderMoney.Text =
                Lit_ReturnOrderMoney.Text = "0";
            }
            else
            {
                DIV_Date.Visible = true;
                DIV_Goods.Visible = true;
                DIV_Orders.Visible = false;
            }
            BindCompany(new Guid(RCB_CompanyList.SelectedValue));
        }
        #endregion

        #region[根据入库单计算金额]

        /// <summary>
        /// 根据入库单计算金额
        /// </summary>
        /// <param name="dics"></param>
        public void GetAmountByStockOrders(Dictionary<string, ReckoningInfo> dics)
        {
            decimal amountSum = 0;
            if (dics != null)
            {
                amountSum = Math.Abs(dics.Values.Sum(act => act.AccountReceivable));
            }
            else
            {
                dics = new Dictionary<string, ReckoningInfo>();
                var orderNos = TB_Orders.Text.Split(',');
                foreach (var orderNo in orderNos)
                {
                    if (string.IsNullOrEmpty(orderNo)) continue;
                    var guid = _reckoning.GetReckoningInfoByTradeCode(orderNo);

                    if (guid == Guid.Empty)
                    {
                        if (!(Request.QueryString["Type"] != null && Request.QueryString["Type"].Equals("1")))
                        {
                            RAM.Alert(string.Format("入库单{0}不存在未对账的往来帐记录!", orderNo));
                            TB_Orders.Text = string.Empty;
                            return;
                        }
                    }
                    var reck = _reckoning.GetReckoning(guid);
                    if (reck == null)
                    {
                        RAM.Alert(string.Format("入库单{0}对应往来帐不存在", orderNo));
                        TB_Orders.Text = string.Empty;
                        return;
                    }
                    dics.Add(orderNo, reck);
                    var documentReds = _documentRedDto.GetDocumentRedInfoByLinkTradeCode(orderNo);
                    var accountReceivable = Math.Abs(reck.AccountReceivable);
                    if (documentReds != null && documentReds.Count > 0)
                    {
                        var documentRed = documentReds.FirstOrDefault(act => act.DocumentType != (Int32)DocumentType.RedDocument);
                        if (documentRed != null)
                        {
                            if (documentRed.State != (Int32)DocumentRedState.Finished && string.IsNullOrEmpty(Request.QueryString["ID"]))
                            {
                                RAM.Alert(string.Format("单据{0}正在红冲中，无法申请付款单", orderNo));
                                TB_Orders.Text = string.Empty;
                                return;
                            }
                            accountReceivable = Math.Abs(documentRed.AccountReceivable);
                        }
                    }
                    amountSum += accountReceivable;
                }
            }
            RTB_SettleBalance.Text = amountSum.ToString("#.00");
            RCB_CompanyList.SelectedValue = string.Format("{0}", dics.Values.First().ThirdCompanyID);
            var info = dics.Values.First();
            RCB_FilialeList.SelectedValue = string.Format("{0}", dics.Values.First().FilialeId);

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
        }

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

        #region[检查入库单是否存在并计算金额]
        protected void TbOrdersTextChanged(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(TB_Orders.Text))
            {
                var orderNos = TB_Orders.Text.Split(',');
                var dics = new Dictionary<string, ReckoningInfo>();
                foreach (var orderNo in orderNos)
                {
                    if (string.IsNullOrEmpty(orderNo)) continue;
                    if (!_reckoning.IsExists(orderNo))
                    {
                        RAM.Alert(string.Format("入库单{0}对应的往来帐不存在", orderNo));
                        TB_Orders.Text = string.Empty;
                        return;
                    }
                    var guid = _reckoning.GetReckoningInfoByTradeCode(orderNo);
                    if (guid == Guid.Empty)
                    {
                        RAM.Alert(string.Format("入库单{0}对应的往来帐已对账!", orderNo));
                        TB_Orders.Text = string.Empty;
                        return;
                    }
                    if (_companyFundReceipt.IsExistsByStockOrderNos(orderNo, string.Empty))
                    {
                        RAM.Alert(string.Format("入库单{0}已存在对应的收付款单据", orderNo));
                        return;
                    }
                    var reckoningInfo = _reckoning.GetReckoning(guid);
                    if (reckoningInfo == null)
                    {
                        RAM.Alert(string.Format("入库单{0}对应往来帐不存在", orderNo));
                        TB_Orders.Text = string.Empty;
                        return;
                    }
                    if (reckoningInfo.LinkTradeType != (int)ReckoningLinkTradeType.StockIn)
                    {
                        RAM.Alert(string.Format("入库单{0}对应往来帐不是入库单类型", orderNo));
                        TB_Orders.Text = string.Empty;
                        return;
                    }
                    var documentReds = _documentRedDto.GetDocumentRedInfoByLinkTradeCode(orderNo);
                    if (documentReds != null && documentReds.Count > 0)
                    {
                        var documentRed = documentReds.FirstOrDefault(act => act.DocumentType != (Int32)DocumentType.RedDocument);
                        if (documentRed != null)
                        {
                            if (documentRed.State != (Int32)DocumentRedState.Finished && string.IsNullOrEmpty(Request.QueryString["ID"]))
                            {
                                RAM.Alert(string.Format("单据{0}正在红冲中，无法申请付款单", orderNo));
                                TB_Orders.Text = string.Empty;
                                return;
                            }
                            reckoningInfo.AccountReceivable = Math.Abs(documentRed.AccountReceivable);
                        }
                    }
                    dics.Add(orderNo, reckoningInfo);
                }
                var companyIds = dics.Values.Select(act => act.ThirdCompanyID).Distinct().ToList();
                if (companyIds.Count > 1)
                {
                    RAM.Alert(string.Format("入库单中存在多个供应商的往来帐"));
                    return;
                }
                if (dics.Values.GroupBy(ent => ent.IsOut).Count() > 1)
                {
                    RAM.Alert(string.Format("入库单中存在多个付款公司的往来帐"));
                    return;
                }
                if (dics.Values.First().IsOut)
                {
                    var filialeids = dics.Values.Select(act => act.FilialeId).Distinct().ToList();
                    if (filialeids.Count > 1)
                    {
                        RAM.Alert(string.Format("入库单中存在多个付款公司的往来帐"));
                        return;
                    }
                }

                GetAmountByStockOrders(dics);

                bool isOut = !new Guid(RCB_FilialeList.SelectedValue).Equals(_reckoningElseFilialeid);
                //排除相关单据
                var tradeCodeList = _companyFundReceipt.CheckExistForStorageNo(new Guid(RCB_CompanyList.SelectedValue), new Guid(RCB_FilialeList.SelectedValue), TB_Orders.Text.Trim());
                if (tradeCodeList.Any())
                {
                    string tradeCode = string.Empty;
                    foreach (var item in tradeCodeList)
                    {
                        tradeCode += "," + item;
                        TB_Orders.Text = TB_Orders.Text.IndexOf("," + item, StringComparison.Ordinal) > -1 ? TB_Orders.Text.Replace("," + item, "") : (TB_Orders.Text.IndexOf(item + ",", StringComparison.Ordinal) > -1 ? TB_Orders.Text.Replace(item + ",", "") : TB_Orders.Text.Replace(item, ""));
                    }
                    if (!string.IsNullOrEmpty(tradeCode))
                    {
                        RAM.Alert("入库单号“" + tradeCode.Substring(1) + "”已添加过付款单!");
                        return;
                    }
                }
            }
            else
            {
                RCB_CompanyList.SelectedValue = Guid.Empty.ToString();
                RCB_CompanyList.Text = "";
                RTB_SettleBalance.Text = "";
                RCB_CompanyList.Enabled = false;
            }
            BindCompany(new Guid(RCB_CompanyList.SelectedValue));
        }
        #endregion

        #region[选择按采购单付款]
        protected void RbPurchaseOrderCheckedChanged(object sender, EventArgs e)
        {
            DivStockNos.Visible = false;
            if (RB_PurchaseOrder.Checked)
            {
                DIV_Date.Visible = false;
                DIV_Goods.Visible = true;
                DIV_Orders.Visible = false;
                DIV_Related.Visible = false;
                RTB_PurchaseOrderNo.Text = "";
                RCB_CompanyList.SelectedValue = Guid.Empty.ToString();
                RCB_CompanyList.Text = "";
                RTB_SettleBalance.Text = "";
                DIV_BackBalance.Visible = false;
                RCB_CompanyList.Enabled = false;
                RCB_FilialeList.Enabled = false;
                DIV_DiscountMoney.Visible = true;
                LB_BankAmount.Text = "单据总额：";

                RtbLastRebate.Text = "0";
                RTB_DiscountMoney.Text = "0";
                RTB_DiscountCaption.Text = string.Empty;
                RTB_PayOrder.Text =
                RTB_ReturnOrder.Text =
                Lit_ReturnOrder.Text =
                Lit_PayOrder.Text = string.Empty;
                Lit_PayOrderMoney.Text =
                Lit_ReturnOrderMoney.Text = "0";
            }
            else
            {
                DIV_Date.Visible = false;
                DIV_Goods.Visible = false;
                DIV_Orders.Visible = false;
            }
            BindCompany(new Guid(RCB_CompanyList.SelectedValue));
        }
        #endregion

        #region[判断采购单是否存在]
        protected void RtbPurchaseOrderNoTextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(RTB_PurchaseOrderNo.Text))
            {
                SelectPurchasing();
            }
            else
            {
                RCB_CompanyList.SelectedValue = Guid.Empty.ToString();
                RCB_CompanyList.Text = "";
            }
            BindCompany(new Guid(RCB_CompanyList.SelectedValue));

        }
        #endregion

        #region[判断采购单是否存在是否重复/是否属于同一往来单位/是否已经付款]
        ///<summary>
        /// 判断采购单是否存在是否重复/是否属于同一往来单位/是否已经付款
        ///</summary>
        public void SelectPurchasing()
        {
            string[] purchasings = RTB_PurchaseOrderNo.Text.Split(',');
            Guid companyId = Guid.Empty;
            string purchasing1 = string.Empty;
            Guid purchasingFilialeId = Guid.Empty;
            Boolean isOut = false;
            if (purchasings.Length > 1)
            {
                RAM.Alert("按采购单入款仅限单条数据付款!");
                return;
            }
            //decimal totalAmount = 0;
            foreach (var purchasingitem in purchasings)
            {
                var purchasing = purchasingitem.Trim();
                PurchasingInfo purchasingInfo = _purchasing.GetPurchasingList(purchasing.Trim());
                if (purchasingInfo.PurchasingID == Guid.Empty || purchasingInfo.PurchasingState == (int)PurchasingState.NoSubmit || purchasingInfo.PurchasingState == (int)PurchasingState.Deleted)
                {
                    RAM.Alert("采购单:" + purchasing.Trim() + "不存在或未提交此采购单，请检查后重新输入");
                    return;
                }
                if (purchasing1 != purchasing)
                {
                    purchasing1 = purchasing;
                }
                else
                {
                    RAM.Alert("请不要重复添加相同的采购单");
                    return;
                }
                if (companyId == Guid.Empty)
                {
                    companyId = purchasingInfo.CompanyID;
                    isOut = purchasingInfo.IsOut;
                }
                else
                {
                    if (companyId != purchasingInfo.CompanyID)
                    {
                        RAM.Alert("不能同时添加多个往来单位的采购单");
                        return;
                    }
                }
                if (purchasingFilialeId == Guid.Empty)
                {
                    purchasingFilialeId = purchasingInfo.PurchasingFilialeId;
                }
                else
                {
                    if (purchasingFilialeId != purchasingInfo.PurchasingFilialeId)
                    {
                        RAM.Alert("不能同时添加不同采购公司的采购单");
                        return;
                    }
                }
                if (isOut != purchasingInfo.IsOut)
                {
                    RAM.Alert("系统提示：采购单属性不一致，不允许共同支付！");
                    return;
                }
                string purchasing2 = purchasing;
                List<CompanyFundReceiptInfo> companyFundReceiptInfos = _companyFundReceipt.GetAllFundReceiptListByCompanyId(purchasingInfo.CompanyID).Where(
                        c => c.PurchaseOrderNo.Split(',').Contains(purchasing2.Trim())).ToList();

                List<CompanyFundReceiptInfo> fundReceiptInfos = _companyFundReceipt.GetAllFundReceiptListByCompanyId(purchasingInfo.CompanyID).
                        OrderByDescending(c => c.SettleEndDate).Take(1).ToList();

                if (companyFundReceiptInfos.Count > 0 || (fundReceiptInfos.Count > 0 ? purchasingInfo.StartTime <= fundReceiptInfos[0].SettleEndDate : 1 == 2))
                {
                    RAM.Alert("采购单:" + purchasing + ",已经付款或者采购单未完成，不可重复付款");
                    return;
                }
                if (_storageRecordDao.GetStorageRecordsByLinkTradeNo(purchasingitem, StorageRecordState.Finished).Count > 0) //采购单对应的入库单
                {
                    RAM.Alert("采购单已存在入库单，请按入库单付款");
                    return;
                }
                //totalAmount += Purchasing.GetPurchasingAmount(purchasing.Trim());
                Task(purchasingInfo, new StorageRecordInfo());
            }
        }
        #endregion

        #region[选择预付款]
        protected void RbAdvanceCheckedChanged(object sender, EventArgs e)
        {
            DivStockNos.Visible = false;
            if (RbAdvance.Checked)
            {
                DIV_Date.Visible = true;
                DIV_Goods.Visible = false;
                DIV_Orders.Visible = false;
                RCB_CompanyList.SelectedValue = Guid.Empty.ToString();
                RCB_CompanyList.Text = "";
                RCB_CompanyList.Enabled = true;
                RTB_SettleBalance.Text = "";
                DIV_BackBalance.Visible = true;
                RCB_CompanyList.Enabled = true;
                RCB_FilialeList.Enabled = true;
                LB_BankAmount.Text = "总欠款额：";
                DIV_DiscountMoney.Visible = false;
                LB_Date.Visible = false;
                RDP_StartDate.Visible = false;
                RDP_EndDate.Visible = false;

                RtbLastRebate.Text = "0";
                RTB_DiscountMoney.Text = "0";
                RTB_DiscountCaption.Text = string.Empty;
                RTB_PayOrder.Text =
                RTB_ReturnOrder.Text =
                Lit_ReturnOrder.Text =
                Lit_PayOrder.Text = string.Empty;
                Lit_PayOrderMoney.Text =
                Lit_ReturnOrderMoney.Text = "0";
                DIV_Related.Visible = false;
            }
            else
            {
                DIV_Date.Visible = false;
                DIV_Goods.Visible = false;
                DIV_Orders.Visible = false;
                LB_Date.Visible = true;
                RDP_StartDate.Visible = true;
                RDP_EndDate.Visible = true;
            }
            BindCompany(new Guid(RCB_CompanyList.SelectedValue));
        }
        #endregion

        #region[保存]

        /// <summary>保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void LbInsterOncLick(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                if (!_submitController.Enabled)
                {
                    RAM.Alert("程序正在处理中，请稍候...");
                    return;
                }
                if (string.IsNullOrEmpty(RTB_RealityBalance.Text.Trim()))
                {
                    RAM.Alert("应付金额不能为空");
                    return;
                }
                if (!string.IsNullOrEmpty(RTB_SettleBalance.Text.Trim()) && !string.IsNullOrEmpty(RTB_RealityBalance.Text.Trim()))
                {
                    if (decimal.Parse(RTB_SettleBalance.Text) != decimal.Parse(RTB_RealityBalance.Text) && string.IsNullOrEmpty(RTB_OtherDiscountCaption.Text))
                    {
                        RAM.Alert("备注说明不能为空");
                        return;
                    }
                }
                if (string.IsNullOrEmpty(TB_Payee.Text) || string.IsNullOrEmpty(RTB_CompBank.Text) || string.IsNullOrEmpty(RTB_CompBankAccount.Text))
                {
                    RAM.Alert("往来单位对应公司未绑定银行账户信息!");
                    return;
                }
                decimal discountMoney = RTB_DiscountMoney.Text.Trim() == string.Empty ? 0 : Convert.ToDecimal(RTB_DiscountMoney.Text.Trim());
                decimal lastRebate = RtbLastRebate.Text.Trim() == string.Empty ? 0 : Convert.ToDecimal(RtbLastRebate.Text.Trim());

                if ((discountMoney != 0 || lastRebate != 0) && string.IsNullOrEmpty(RTB_DiscountCaption.Text))
                {
                    RAM.Alert("折扣说明不能为空!");
                    return;
                }

                #region [按日期付款]
                decimal totalNumber = RB_Date.Checked ? GetTotalloled(RtbInclude.Text.Trim().Split(',').ToList(), RtbRemove.Text.Trim().Split(',').ToList())
                    : _reckoning.GetReckoningNonceTotalledByFilialeId(new Guid(RCB_CompanyList.SelectedValue), new Guid(RCB_FilialeList.SelectedValue), DateTime.Now);
                if (RB_Date.Checked)
                {
                    if (RDP_StartDate.SelectedDate > RDP_EndDate.SelectedDate)
                    {
                        RDP_EndDate.SelectedDate = null;
                        RAM.Alert("系统提示：截止日期必须大于开始日期！");
                        return;
                    }
                    if (Convert.ToDecimal(RTB_RealityBalance.Text.Trim()) + Math.Abs(discountMoney) + Math.Abs(lastRebate) != Math.Abs(totalNumber))
                    {
                        RAM.Alert("应付金额+当年折扣+去年返利必须等于当前应付总额,当前总额为：" + Math.Abs(totalNumber).ToString("#.00"));
                        return;
                    }
                    string msg;
                    var receiptInfo = GetModel(out msg);
                    if (receiptInfo == null)
                    {
                        RAM.Alert(string.Format("系统提示：{0}", msg));
                        return;
                    }
                    var verifyReceiptInfo = _companyFundReceipt.GetFundReceiptInfoByLately(receiptInfo.CompanyID, receiptInfo.FilialeId);
                    if (verifyReceiptInfo != null)
                    {
                        if (receiptInfo.SettleEndDate <= verifyReceiptInfo.SettleEndDate)
                        {
                            RAM.Alert("系统提示：该时间段内已添加过付款单据，请核对后再操作！");
                            return;
                        }
                    }
                }
                #endregion

                #region [按入库单付款]
                if (RB_Invoice.Checked)
                {
                    decimal amountSum = 0;
                    var orders = TB_Orders.Text.Trim();
                    if (string.IsNullOrEmpty(orders))
                    {
                        RAM.Alert("入库单号不能为空!");
                        return;
                    }

                    //排除相关单据
                    var tradeCodeList = _companyFundReceipt.CheckExistForStorageNo(new Guid(RCB_CompanyList.SelectedValue), new Guid(RCB_FilialeList.SelectedValue), TB_Orders.Text.Trim());
                    if (tradeCodeList.Any())
                    {
                        string tradeCode = string.Empty;
                        foreach (var item in tradeCodeList)
                        {
                            tradeCode += "," + item;
                            TB_Orders.Text = TB_Orders.Text.IndexOf("," + item, StringComparison.Ordinal) > -1 ? TB_Orders.Text.Replace("," + item, "") : (TB_Orders.Text.IndexOf(item + ",", StringComparison.Ordinal) > -1 ? TB_Orders.Text.Replace(item + ",", "") : TB_Orders.Text.Replace(item, ""));
                        }
                        if (!string.IsNullOrEmpty(tradeCode))
                        {
                            RAM.Alert("入库单号“" + tradeCode.Substring(1) + "”已添加过付款单!");
                            return;
                        }
                    }

                    var orderNos = TB_Orders.Text.Split(',');
                    var dics = new Dictionary<string, ReckoningInfo>();
                    foreach (var orderNo in orderNos)
                    {
                        if (string.IsNullOrEmpty(orderNo)) continue;
                        var guid = _reckoning.GetReckoningInfoByTradeCode(orderNo);
                        if (guid == Guid.Empty)
                        {
                            RAM.Alert(string.Format("入库单{0}对应的往来帐已对账!", orderNo));
                            return;
                        }
                        if (_companyFundReceipt.IsExistsByStockOrderNos(orderNo, string.Empty))
                        {
                            RAM.Alert(string.Format("入库单{0}已存在对应的收付款单据", orderNo));
                            return;
                        }
                        var reckoningInfo = _reckoning.GetReckoning(guid);
                        if (reckoningInfo == null)
                        {
                            RAM.Alert(string.Format("入库单{0}对应往来帐不存在", orderNo));
                            return;
                        }
                        if (reckoningInfo.LinkTradeType != (int)ReckoningLinkTradeType.StockIn)
                        {
                            RAM.Alert(string.Format("入库单{0}对应往来帐不是入库单类型", orderNo));
                            return;
                        }
                        var documentReds = _documentRedDto.GetDocumentRedInfoByLinkTradeCode(orderNo);
                        var accountReceivable = Math.Abs(reckoningInfo.AccountReceivable);
                        if (documentReds != null && documentReds.Count > 0)
                        {
                            var documentRed = documentReds.FirstOrDefault(act => act.DocumentType != (Int32)DocumentType.RedDocument);
                            if (documentRed != null)
                            {
                                if (documentRed.State != (Int32)DocumentRedState.Finished)
                                {
                                    RAM.Alert(string.Format("单据{0}正在红冲中，无法申请付款单", orderNo));
                                    TB_Orders.Text = string.Empty;
                                    return;
                                }
                                accountReceivable = Math.Abs(documentRed.AccountReceivable);
                            }
                        }
                        amountSum += accountReceivable;
                    }

                    var companyIds = dics.Values.Select(act => act.ThirdCompanyID).Distinct().ToList();
                    var filialeids = dics.Values.Select(act => act.FilialeId).Distinct().ToList();
                    if (companyIds.Count > 1)
                    {
                        RAM.Alert(string.Format("入库单中存在多个供应商的往来帐"));
                        TB_Orders.Text = string.Empty;
                        return;
                    }
                    if (filialeids.Count > 1)
                    {
                        RAM.Alert(string.Format("入库单中存在多个付款公司的往来帐"));
                        TB_Orders.Text = string.Empty;
                        return;
                    }
                    if (amountSum != Convert.ToDecimal(RTB_RealityBalance.Text.Trim()) + Math.Abs(discountMoney) + Math.Abs(lastRebate) + Math.Abs(decimal.Parse(Lit_ReturnOrderMoney.Text)) + Math.Abs(decimal.Parse(Lit_PayOrderMoney.Text)))
                    {
                        RAM.Alert("“应付金额+今年折扣+去年返利+退货单+付款单”不等于单据总额，当前单据总额为：" + amountSum.ToString("#.00") + "元");
                        return;
                    }
                }
                #endregion

                #region [按采购单付款]
                if (RB_PurchaseOrder.Checked)
                {
                    if (RTB_PurchaseOrderNo.Text.Trim() == string.Empty)
                    {
                        RAM.Alert("请填写采购单号！");
                        return;
                    }
                    string[] purchasings = RTB_PurchaseOrderNo.Text.Split(',');
                    Guid companyId = Guid.Empty;
                    Guid purchasingFilialeId = Guid.Empty;
                    Decimal amount = 0;
                    IPurchasing purchasingDal = new Purchasing(GlobalConfig.DB.FromType.Read);
                    foreach (var purchasingitem in purchasings)
                    {
                        var purchasing = purchasingitem.Trim();
                        PurchasingInfo purchasingInfo = purchasingDal.GetPurchasingList(purchasing);

                        if (purchasingInfo.PurchasingID == Guid.Empty)
                        {
                            RAM.Alert("采购单:" + purchasing + ",不存在，请检查后重新输入");
                            return;
                        }
                        string purchasing1 = purchasing;
                        List<CompanyFundReceiptInfo> companyFundReceiptInfos =
                            _companyFundReceipt.GetAllFundReceiptListByCompanyId(purchasingInfo.CompanyID).Where(
                                c => c.PurchaseOrderNo.Split(',').Contains(purchasing1.Trim())).ToList();
                        if (companyFundReceiptInfos.Count > 0)
                        {
                            RAM.Alert("采购单:" + purchasing + ",已经付款，不可重复付款");
                            return;
                        }
                        if (companyId == Guid.Empty)
                        {
                            companyId = purchasingInfo.CompanyID;
                        }
                        else
                        {
                            if (companyId != purchasingInfo.CompanyID)
                            {
                                RAM.Alert("不能同时添加多个往来单位的采购单");
                                return;
                            }
                        }
                        if (purchasingFilialeId == Guid.Empty)
                        {
                            purchasingFilialeId = purchasingInfo.PurchasingFilialeId;
                        }
                        else
                        {
                            if (purchasingFilialeId != purchasingInfo.PurchasingFilialeId)
                            {
                                RAM.Alert("不能同时添加不通采购公司的采购单");
                                return;
                            }
                            if (_storageRecordDao.GetStorageRecordsByLinkTradeNo(purchasingitem, StorageRecordState.Finished).Count > 0) //采购单对应的入库单
                            {
                                RAM.Alert("采购单已存在入库单，请按入库单付款");
                                return;
                            }
                        }
                        amount += purchasingDal.GetPurchasingAmount(purchasing);
                    }
                    if (purchasings.Length > 1)
                    {
                        if (amount != Convert.ToDecimal(RTB_RealityBalance.Text.Trim()) + Math.Abs(discountMoney) + Math.Abs(lastRebate))
                        {
                            RAM.Alert("应付金额+今年折扣+去年返利必须等于总采购单金额，当前采购单金额为：" + amount + "元");
                            return;
                        }
                    }
                    else
                    {
                        if (amount < Convert.ToDecimal(RTB_RealityBalance.Text.Trim()) + Math.Abs(discountMoney) + Math.Abs(lastRebate))
                        {
                            RAM.Alert("应付金额+今年折扣+去年返利不得大于采购单金额，当前采购单金额为：" + amount + "元");
                            return;
                        }
                    }
                }
                #endregion

                string errorMsg;
                var receipt = GetModel(out errorMsg);
                if (receipt == null)
                {
                    RAM.Alert(string.Format("系统提示：{0}", errorMsg));
                    return;
                }
                if (RB_Date.Checked)
                {
                    receipt.IncludeStockNos = RtbInclude.Text.Trim();
                    receipt.DebarStockNos = RtbRemove.Text.Trim();
                }
                //note 按公司付款则开具发票  2015-03-20  陈重文
                if (receipt.FilialeId == _reckoningElseFilialeid)
                {
                    receipt.IsOut = false;
                    receipt.HasInvoice = false;
                }
                else
                {
                    var filialeInfo = CacheCollection.Filiale.Get(receipt.FilialeId);
                    if (filialeInfo != null && filialeInfo.ID != Guid.Empty)
                    {
                        receipt.IsOut = true;
                        receipt.HasInvoice = true;
                    }
                    else
                    {
                        receipt.IsOut = false;
                        receipt.HasInvoice = false;
                    }
                }

                if (RbUrgent.Checked && string.IsNullOrEmpty(RtbUrgent.Text))
                {
                    RAM.Alert("请填写加急理由！");
                    return;
                }
                receipt.IsUrgent = RbUrgent.Checked;
                receipt.UrgentRemark = RbUrgent.Checked ? RtbUrgent.Text : string.Empty;

                using (var ts = new TransactionScope(TransactionScopeOption.Required))
                {
                    try
                    {
                        bool isInsert = _companyFundReceipt.Insert(receipt);
                        var info = _companyFundReceipt.GetFundReceiptInfoByReceiptNo(receipt.ReceiptNo);
                        if (info.ReceiptID != Guid.Empty)
                        {
                            #region 如果“付款公司”是ERP，则隐藏增加发票信息模块 zal 2016-02-24
                            if (!RCB_FilialeList.SelectedValue.Equals(_reckoningElseFilialeid.ToString()))
                            {
                                //添加发票信息
                                AddCompanyFundReceiptInvoiceInfo(info.ReceiptID);
                            }
                            #endregion

                            //往来收付款添加付款单增加操作记录添加
                            var personnelInfo = CurrentSession.Personnel.Get();
                            WebControl.AddOperationLog(personnelInfo.PersonnelId, personnelInfo.RealName, info.ReceiptID, info.ReceiptNo,
                                OperationPoint.CurrentReceivedPayment.FillPayment.GetBusinessInfo(), string.Empty);
                        }
                        ts.Complete();
                        RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
                    }
                    catch (Exception ex)
                    {
                        LogService.LogError(ex.Message, "添加付款单", ex);
                        RAM.Alert("添加付款单失败！");
                    }
                }
                _submitController.Submit();
            }
        }

        #endregion

        #region[修改]
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void LbSaveOncLick(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                if (!_submitController.Enabled)
                {
                    RAM.Alert("程序正在处理中，请稍候...");
                    return;
                }

                CompanyFundReceiptInfo model = _companyFundReceipt.GetCompanyFundReceiptInfo(new Guid(Request.QueryString["ID"]));
                if (model.ReceiptStatus != (int)CompanyFundReceiptState.WaitAuditing &&
                    model.ReceiptStatus != (int)CompanyFundReceiptState.NoAuditing &&
                    model.ReceiptStatus != (int)CompanyFundReceiptState.NoAuditingPass)
                {
                    RAM.Alert("状态已更新，不允许此操作！");
                    return;
                }

                if (string.IsNullOrEmpty(RTB_RealityBalance.Text.Trim()))
                {
                    RAM.Alert("应付金额不能为空");
                    return;
                }
                if (string.IsNullOrEmpty(RTB_OtherDiscountCaption.Text))
                {
                    RAM.Alert("备注说明不能为空");
                    return;
                }

                if (string.IsNullOrEmpty(TB_Payee.Text) || string.IsNullOrEmpty(RTB_CompBank.Text) || string.IsNullOrEmpty(RTB_CompBankAccount.Text))
                {
                    RAM.Alert("往来单位对应公司未绑定银行账户信息!");
                    return;
                }
                decimal discountMoney = RTB_DiscountMoney.Text.Trim() == string.Empty ? 0 : Convert.ToDecimal(RTB_DiscountMoney.Text.Trim());
                decimal lastRebate = RtbLastRebate.Text.Trim() == string.Empty ? 0 : Convert.ToDecimal(RtbLastRebate.Text.Trim());

                if ((discountMoney != 0 || lastRebate != 0) && string.IsNullOrEmpty(RTB_DiscountCaption.Text))
                {
                    RAM.Alert("折扣说明不能为空!");
                    return;
                }
                decimal totalNumber = !RB_Date.Checked ? _reckoning.GetReckoningNonceTotalledByFilialeId(new Guid(RCB_CompanyList.SelectedValue), new Guid(RCB_FilialeList.SelectedValue), DateTime.Now) : GetTotalloled(RtbInclude.Text.Trim().Split(',').ToList(), RtbRemove.Text.Trim().Split(',').ToList());
                if (RB_Date.Checked)
                {
                    if (Convert.ToDecimal(RTB_RealityBalance.Text.Trim()) + Math.Abs(discountMoney) + Math.Abs(lastRebate) != Math.Abs(totalNumber))
                    {
                        RAM.Alert("应付金额+今年折扣+去年返利必须等于当前应付总额,当前总额为：" + Math.Abs(totalNumber).ToString("#.00"));
                        return;
                    }
                }
                string errorMsg;
                CompanyFundReceiptInfo receipt = GetModel(out errorMsg);
                if (receipt == null)
                {
                    RAM.Alert(errorMsg);
                    return;
                }
                receipt.ReceiptID = new Guid(Request.QueryString["ID"]);
                receipt.ReceiptNo = model.ReceiptNo;
                if (RB_Invoice.Checked)
                {
                    decimal amountSum = 0;
                    var orders = TB_Orders.Text.Trim();
                    if (string.IsNullOrEmpty(orders))
                    {
                        RAM.Alert("入库单号不能为空!");
                        return;
                    }
                    var orderNos = TB_Orders.Text.Split(',');
                    var dics = new Dictionary<string, ReckoningInfo>();
                    foreach (var orderNo in orderNos)
                    {
                        if (string.IsNullOrEmpty(orderNo)) continue;
                        var guid = _reckoning.GetReckoningInfoByTradeCode(orderNo);
                        if (guid == Guid.Empty)
                        {
                            RAM.Alert(string.Format("入库单{0}对应的往来帐已对账!", orderNo));
                            return;
                        }
                        //if (_companyFundReceipt.IsExistsByStockOrderNos(orderNo, receipt.ReceiptNo))
                        //{
                        //    RAM.Alert(string.Format("入库单{0}已存在对应的收付款单据", orderNo));
                        //    return;
                        //}
                        var reckoningInfo = _reckoning.GetReckoning(guid);
                        if (reckoningInfo == null)
                        {
                            RAM.Alert(string.Format("入库单{0}对应往来帐不存在", orderNo));
                            return;
                        }
                        if (reckoningInfo.LinkTradeType != (int)ReckoningLinkTradeType.StockIn)
                        {
                            RAM.Alert(string.Format("入库单{0}对应往来帐不是入库单类型", orderNo));
                            return;
                        }
                        var documentReds = _documentRedDto.GetDocumentRedInfoByLinkTradeCode(orderNo);
                        var accountReceivable = Math.Abs(reckoningInfo.AccountReceivable);
                        if (documentReds != null && documentReds.Count > 0)
                        {
                            var documentRed = documentReds.FirstOrDefault(act => act.DocumentType != (Int32)DocumentType.RedDocument);
                            if (documentRed != null)
                            {
                                if (documentRed.State != (Int32)DocumentRedState.Finished)
                                {
                                    RAM.Alert(string.Format("单据{0}正在红冲中，无法申请付款单", orderNo));
                                    TB_Orders.Text = string.Empty;
                                    return;
                                }
                                accountReceivable = Math.Abs(documentRed.AccountReceivable);
                            }
                        }
                        amountSum += accountReceivable;
                    }

                    var companyIds = dics.Values.Select(act => act.ThirdCompanyID).Distinct().ToList();
                    var filialeids = dics.Values.Select(act => act.FilialeId).Distinct().ToList();
                    if (companyIds.Count > 1)
                    {
                        RAM.Alert(string.Format("入库单中存在多个供应商的往来帐"));
                        TB_Orders.Text = string.Empty;
                        return;
                    }
                    if (filialeids.Count > 1)
                    {
                        RAM.Alert(string.Format("入库单中存在多个付款公司的往来帐"));
                        TB_Orders.Text = string.Empty;
                        return;
                    }
                    if (amountSum != Convert.ToDecimal(RTB_RealityBalance.Text.Trim()) + Math.Abs(discountMoney) + Math.Abs(lastRebate) + Math.Abs(decimal.Parse(Lit_ReturnOrderMoney.Text)) + Math.Abs(decimal.Parse(Lit_PayOrderMoney.Text)))
                    {
                        RAM.Alert("“应付金额+今年折扣+去年返利+退货单+付款单”不等于单据总额，当前单据总额为：" + amountSum.ToString("#.00") + "元");
                        return;
                    }
                }
                if (RB_PurchaseOrder.Checked)
                {
                    if (RTB_PurchaseOrderNo.Text.Trim() == string.Empty)
                    {
                        RAM.Alert("请填写采购单号！");
                        return;
                    }
                    var realityBalance = Convert.ToDecimal(RTB_RealityBalance.Text);
                    string[] purchasings = RTB_PurchaseOrderNo.Text.Split(',');
                    IPurchasing purchasingDal = new Purchasing(GlobalConfig.DB.FromType.Read);
                    var amount = purchasings.Sum(purchasing => purchasingDal.GetPurchasingAmount(purchasing));
                    if (purchasings.Length > 1)
                    {
                        if (amount != realityBalance + Math.Abs(discountMoney) + Math.Abs(lastRebate))
                        {
                            RAM.Alert("应付金额+今年折扣+去年返利必须等于总采购单金额，当前采购单金额为：" + amount.ToString("#.00") + "元");
                            return;
                        }
                    }
                    else
                    {
                        if (amount < realityBalance + Math.Abs(discountMoney) + Math.Abs(lastRebate))
                        {
                            RAM.Alert("应付金额+今年折扣+去年返利不得大于采购单金额，当前采购单金额为：" + amount.ToString("#.00") + "元");
                            return;
                        }
                        if (_storageRecordDao.GetStorageRecordsByLinkTradeNo(RTB_PurchaseOrderNo.Text, StorageRecordState.Finished).Count > 0) //采购单对应的入库单)
                        {
                            RAM.Alert("采购单已存在入库单，请按入库单付款");
                            return;
                        }
                    }

                }

                RCB_CompanyList.SelectedValue = receipt.CompanyID.ToString();

                //note 按公司付款则开具发票  2015-03-20  陈重文
                if (receipt.FilialeId == _reckoningElseFilialeid)
                {
                    receipt.IsOut = false;
                    receipt.HasInvoice = false;
                }
                else
                {
                    var filialeInfo = CacheCollection.Filiale.Get(receipt.FilialeId);
                    if (filialeInfo != null && filialeInfo.ID != Guid.Empty)
                    {
                        receipt.IsOut = true;
                        receipt.HasInvoice = true;
                    }
                    else
                    {
                        receipt.IsOut = false;
                        receipt.HasInvoice = false;
                    }
                }

                receipt.IsUrgent = RbUrgent.Checked;
                receipt.UrgentRemark = RbUrgent.Checked ? RtbUrgent.Text : string.Empty;

                using (var ts = new TransactionScope(TransactionScopeOption.Required))
                {
                    try
                    {
                        bool isSave = _companyFundReceipt.Update(receipt);
                        if (isSave)
                        {

                            #region 如果“付款公司”是ERP，则隐藏增加发票信息模块 zal 2016-02-24
                            if (!RCB_FilialeList.SelectedValue.Equals(_reckoningElseFilialeid.ToString()))
                            {
                                #region 如果该单据对应的发票信息有任何一条不处于”未提交“状态，则该单据对应的发票信息在此页面不能增加、修改、删除，此操作可在”发票操作“页面进行
                                //查询该单据发票除”未提交“以外的所有状态的发票信息
                                var invoiceList = _companyFundReceiptInvoice.Getlmshop_CompanyFundReceiptInvoiceByReceiptID(receipt.ReceiptID).Where(p => (new int?[] { (int)CompanyFundReceiptInvoiceState.Submit, (int)CompanyFundReceiptInvoiceState.Receive, (int)CompanyFundReceiptInvoiceState.Authenticate, (int)CompanyFundReceiptInvoiceState.Verification }).Contains(p.InvoiceState));
                                if (!invoiceList.Any())
                                {
                                    #region 插入发票信息 zal 2016-01-25
                                    if (CompanyFundReceiptInvoiceInfoList.Any())
                                    {
                                        _companyFundReceiptInvoice.Deletelmshop_CompanyFundReceiptInvoiceByReceiptID(receipt.ReceiptID);
                                        //添加发票信息
                                        AddCompanyFundReceiptInvoiceInfo(receipt.ReceiptID);
                                    }
                                    #endregion
                                }
                                #endregion
                            }
                            #endregion

                            //往来收付款修改增加操作记录添加
                            var personnelInfo = CurrentSession.Personnel.Get();
                            WebControl.AddOperationLog(personnelInfo.PersonnelId, personnelInfo.RealName, receipt.ReceiptID, receipt.ReceiptNo,
                                OperationPoint.CurrentReceivedPayment.Edit.GetBusinessInfo(), string.Empty);
                            ts.Complete();

                            RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
                        }
                        else
                        {
                            RAM.Alert("保存收付款单失败！");
                        }
                    }
                    catch (Exception ex)
                    {
                        RAM.Alert("添加付款单失败！");
                    }
                }
                _submitController.Submit();
            }
        }
        #endregion

        #region[修改绑定数据]
        protected void BindValue()
        {
            if (CompanyFundReceiptInfoModel.ReceiptID.Equals(Guid.Empty)) return;
            txt_PaymentDate.Text = CompanyFundReceiptInfoModel.PaymentDate == DateTime.MinValue ? DateTime.Now.ToString("yyyy-MM") : Convert.ToDateTime(CompanyFundReceiptInfoModel.PaymentDate).ToString("yyyy-MM");
            RCB_CompanyList.SelectedValue = CompanyFundReceiptInfoModel.CompanyID.ToString();
            RTB_RealityBalance.Text = CompanyFundReceiptInfoModel.RealityBalance.ToString(CultureInfo.InvariantCulture);
            RTB_DiscountMoney.Text = CompanyFundReceiptInfoModel.DiscountMoney.ToString(CultureInfo.InvariantCulture);
            RTB_DiscountCaption.Text = CompanyFundReceiptInfoModel.DiscountCaption;
            rbl_InvoiceType.Text = CompanyFundReceiptInfoModel.InvoiceType.ToString(CultureInfo.InvariantCulture);
            RTB_OtherDiscountCaption.Text = CompanyFundReceiptInfoModel.OtherDiscountCaption;
            RtbLastRebate.Text = string.Format("{0}", CompanyFundReceiptInfoModel.LastRebate);
            bool isDate = (CompanyFundReceiptType)CompanyFundReceiptInfoModel.ReceiptType ==
                                  CompanyFundReceiptType.Payment && string.IsNullOrEmpty(CompanyFundReceiptInfoModel.PurchaseOrderNo.Trim()) &&
                          string.IsNullOrEmpty(CompanyFundReceiptInfoModel.StockOrderNos.Trim()) &&
                          CompanyFundReceiptInfoModel.SettleEndDate != DateTime.Parse("1999-09-09");
            DivStockNos.Visible = isDate;

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
                RCB_CompanyList.Enabled = false;
                RCB_FilialeList.Enabled = false;
                //采购单
                if (!string.IsNullOrEmpty(CompanyFundReceiptInfoModel.PurchaseOrderNo.Trim()))
                {
                    RB_PurchaseOrder.Checked = true;
                    RTB_PurchaseOrderNo.Text = CompanyFundReceiptInfoModel.PurchaseOrderNo;
                    DIV_Date.Visible = false;
                    DIV_Goods.Visible = true;
                    DIV_Orders.Visible = false;
                    DIV_BackBalance.Visible = false;
                    //RCB_FilialeList.Enabled = false;
                    RB_Date.Enabled = false;
                    RB_Invoice.Enabled = false;
                    RB_PurchaseOrder.Enabled = false;
                    RbAdvance.Enabled = false;
                    RTB_PurchaseOrderNo.Enabled = false;
                    DIV_Related.Visible = false;
                    LB_BankAmount.Text = "单据总额：";
                }
                //入库单
                if (!string.IsNullOrEmpty(CompanyFundReceiptInfoModel.StockOrderNos.Trim()))
                {
                    DIV_BackBalance.Visible = true;
                    RB_Invoice.Checked = true;
                    LB_BankAmount.Text = "单据总额：";
                    TB_Orders.Text = CompanyFundReceiptInfoModel.StockOrderNos;
                    DIV_Date.Visible = false;
                    DIV_Goods.Visible = false;
                    DIV_Orders.Visible = true;
                    //RCB_FilialeList.Enabled = false;
                    RB_Date.Enabled = false;
                    RB_Invoice.Enabled = false;
                    RB_PurchaseOrder.Enabled = false;
                    RbAdvance.Enabled = false;
                    TB_Orders.Enabled = false;
                    DIV_BackBalance.Visible = true;
                    DIV_Related.Visible = true;
                    RTB_ReturnOrder.Text = CompanyFundReceiptInfoModel.ReturnOrder;
                    RTB_PayOrder.Text = CompanyFundReceiptInfoModel.PayOrder;
                    CheckReturnOrder();
                    CheckPayOrder();
                    if (Request.QueryString["Type"] != null && Request.QueryString["Type"].Equals("1"))
                    {
                        GetStockAmount();
                    }
                    else
                    {
                        GetAmountByStockOrders(null);
                    }
                }
                //日期付款
                if (isDate)
                {
                    RB_Date.Checked = true;
                    LB_BankAmount.Text = "当期余额：";
                    RDP_StartDate.SelectedDate = CompanyFundReceiptInfoModel.SettleStartDate;
                    RDP_EndDate.SelectedDate = CompanyFundReceiptInfoModel.SettleEndDate;
                    if (Request.QueryString["Type"] != null && Request.QueryString["Type"].Equals("1"))
                    {
                        GetDateAmount();
                    }
                    else
                    {
                        decimal totalNumber = GetTotalloled(CompanyFundReceiptInfoModel.IncludeStockNos.Split(',').ToList(), CompanyFundReceiptInfoModel.DebarStockNos.Split(',').ToList());
                        RTB_SettleBalance.Text = (-totalNumber).ToString("N");
                    }
                    DIV_Date.Visible = true;
                    DIV_Goods.Visible = false;
                    DIV_Orders.Visible = false;
                    //RCB_CompanyList.Enabled = false;
                    DIV_BackBalance.Visible = true;
                    RB_Date.Enabled = false;
                    RB_Invoice.Enabled = false;
                    RB_PurchaseOrder.Enabled = false;
                    RbAdvance.Enabled = false;
                    RtbInclude.Text = CompanyFundReceiptInfoModel.IncludeStockNos;
                    RtbRemove.Text = CompanyFundReceiptInfoModel.DebarStockNos;
                    DIV_Related.Visible = false;
                }
                //预付款
                if (string.IsNullOrEmpty(CompanyFundReceiptInfoModel.PurchaseOrderNo.Trim()) && string.IsNullOrEmpty(CompanyFundReceiptInfoModel.StockOrderNos.Trim()) && CompanyFundReceiptInfoModel.SettleStartDate == DateTime.Parse("1999-09-09") && CompanyFundReceiptInfoModel.SettleEndDate == DateTime.Parse("1999-09-09"))
                {
                    RbAdvance.Checked = true;
                    DIV_Date.Visible = true;
                    DIV_Goods.Visible = false;
                    DIV_Orders.Visible = false;
                    //RCB_CompanyList.SelectedValue = Guid.Empty.ToString();
                    //RCB_CompanyList.Enabled = false;
                    DIV_BackBalance.Visible = true;
                    LB_Date.Visible = false;
                    RDP_StartDate.Visible = false;
                    RDP_EndDate.Visible = false;
                    LB_BankAmount.Text = "总欠款额：";
                    DIV_DiscountMoney.Visible = false;
                    RB_Date.Enabled = false;
                    RB_Invoice.Enabled = false;
                    RB_PurchaseOrder.Enabled = false;
                    RbAdvance.Enabled = false;
                    DIV_Related.Visible = false;

                    decimal totalNumber = _reckoning.GetReckoningNonceTotalled(CompanyFundReceiptInfoModel.FilialeId, CompanyFundReceiptInfoModel.CompanyID, DateTime.Parse("1999-09-09 00:00:00.000"), DateTime.Now, (int)ReckoningStateType.Currently);
                    RTB_SettleBalance.Text = totalNumber != 0 ? (-totalNumber).ToString("#.00") : "0";
                }
                if ((!string.IsNullOrEmpty(RTB_DiscountMoney.Text) && RTB_DiscountMoney.Text != "0.00")
                    || (!string.IsNullOrEmpty(RtbLastRebate.Text) && RtbLastRebate.Text != "0.00"))
                    DIV_DiscountCaption.Visible = true;
                else
                    DIV_DiscountCaption.Visible = false;
            }

            var filialeId = CompanyFundReceiptInfoModel.FilialeId;
            if (filialeId == _reckoningElseFilialeid)
            {
                RCB_FilialeList.Items.Add(new RadComboBoxItem("ERP", _reckoningElseFilialeid.ToString()));
                RCB_FilialeList.SelectedValue = _reckoningElseFilialeid.ToString();
            }
            else
            {
                RCB_FilialeList.SelectedValue = CompanyFundReceiptInfoModel.FilialeId.ToString();
            }
            //RCB_BankAccount.SelectedValue = CompanyFundReceiptInfoModel.PayBankAccountsId.ToString();
            BindCompany(CompanyFundReceiptInfoModel.CompanyID);
        }
        #endregion

        #region[组装添加和修改时的模型]

        ///<summary>组装添加和修改时的模型
        ///</summary>
        ///<returns></returns>
        public CompanyFundReceiptInfo GetModel(out string errorMsg)
        {
            var receipt = new CompanyFundReceiptInfo
            {
                ReceiptNo = MyCode.GetCode(CodeType.PY),
                ReceiptType = Convert.ToInt32(CompanyFundReceiptType.Payment),
                ApplyDateTime = DateTime.Now,
                ApplicantID = CurrentSession.Personnel.Get().PersonnelId,
                CompanyID = new Guid(RCB_CompanyList.SelectedValue),
                PurchaseOrderNo = "",
                StockOrderNos = "",
                IncludeStockNos = RtbInclude.Text,
                DebarStockNos = RtbRemove.Text,
                FilialeId = new Guid(RCB_FilialeList.SelectedValue),
                InvoiceType = int.Parse(rbl_InvoiceType.SelectedValue)
            };
            var filialeId = _companyCussent.GetRelevanceFilialeIdByCompanyId(receipt.CompanyID);
            if (filialeId != Guid.Empty)
            {
                var bankAccounts = _companyBankAccountBindBll.GetCompanyBankAccountIdBind(receipt.CompanyID, receipt.FilialeId);
                if (bankAccounts != null)
                    receipt.ReceiveBankAccountId = bankAccounts.BankAccountsId;
                else
                {
                    errorMsg = "公司与往来单位未绑定收款账号";
                    return null;
                }
            }
            if (RB_Date.Checked)
            {
                if (RDP_StartDate.SelectedDate != null)
                {
                    receipt.SettleStartDate = (DateTime)RDP_StartDate.SelectedDate;
                    if (RDP_EndDate.SelectedDate != null) receipt.SettleEndDate = RDP_EndDate.SelectedDate > DateTime.Now ? DateTime.Now : (DateTime)RDP_EndDate.SelectedDate;
                }
            }
            if (RB_Invoice.Checked)
            {
                receipt.StockOrderNos = TB_Orders.Text.Trim();
                receipt.SettleStartDate = DateTime.Parse("1999-09-09");
                receipt.SettleEndDate = DateTime.Parse("1999-09-09");
            }
            if (RB_PurchaseOrder.Checked)
            {
                receipt.PurchaseOrderNo = RTB_PurchaseOrderNo.Text.Trim();
                receipt.SettleStartDate = DateTime.Parse("1999-09-09");
                receipt.SettleEndDate = DateTime.Parse("1999-09-09");
            }
            if (RbAdvance.Checked)
            {
                receipt.SettleStartDate = DateTime.Parse("1999-09-09");
                receipt.SettleEndDate = DateTime.Parse("1999-09-09");
            }
            receipt.ExpectBalance = 0;
            receipt.RealityBalance = Convert.ToDecimal(RTB_RealityBalance.Text.Trim());
            if (string.IsNullOrEmpty(RTB_DiscountMoney.Text.Trim()))
            {
                receipt.DiscountMoney = 0;
                receipt.DiscountCaption = string.Empty;
            }
            else
            {
                receipt.DiscountMoney = Convert.ToDecimal(RTB_DiscountMoney.Text.Trim());
                receipt.DiscountCaption = RTB_DiscountCaption.Text.Trim();
            }
            receipt.OtherDiscountCaption = RTB_OtherDiscountCaption.Text.Trim();
            receipt.ReceiptStatus = Convert.ToInt32(CompanyFundReceiptState.WaitAuditing);
            receipt.PayBankAccountsId = Guid.Empty;
            receipt.LastRebate = string.IsNullOrEmpty(RtbLastRebate.Text) ? 0 : Convert.ToDecimal(RtbLastRebate.Text);
            receipt.PaymentDate = Convert.ToDateTime(txt_PaymentDate.Text);
            receipt.ReturnOrder = RTB_ReturnOrder.Text;
            receipt.PayOrder = RTB_PayOrder.Text;
            errorMsg = "";
            return receipt;
        }

        #endregion

        #region[公用属性]
        protected CompanyFundReceiptInfo CompanyFundReceiptInfoModel
        {
            get
            {
                if (ViewState["CompanyFundReceiptInfoModel"] == null)
                {
                    return new CompanyFundReceiptInfo();
                }
                return (CompanyFundReceiptInfo)ViewState["CompanyFundReceiptInfoModel"];
            }
            set
            {
                ViewState["CompanyFundReceiptInfoModel"] = value;
            }
        }

        protected Guid CompangyID
        {
            get
            {
                if (ViewState["CompangyID"] == null) return Guid.Empty;
                return new Guid(ViewState["CompangyID"].ToString());
            }
            set
            {
                ViewState["CompangyID"] = value.ToString();
            }
        }


        /// <summary>
        /// 发票列表属性
        /// </summary>
        protected List<CompanyFundReceiptInvoiceInfo> CompanyFundReceiptInvoiceInfoList
        {
            get
            {
                if (ViewState["CompanyFundReceiptInvoiceInfoList"] == null)
                    return new List<CompanyFundReceiptInvoiceInfo>();
                return ViewState["CompanyFundReceiptInvoiceInfoList"] as List<CompanyFundReceiptInvoiceInfo>;
            }
            set
            {
                ViewState["CompanyFundReceiptInvoiceInfoList"] = value;
            }
        }
        #endregion

        #region[选择公司后加载出对应的银行账号]

        protected void Rcb_FilialeListSelectedIndexChanged(object o, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            var value = e.Value.Trim();
            Guid filialeId;
            Guid.TryParse(value, out filialeId);
            FilialeList(filialeId);
            if (RB_Date.Checked)
            {
                RtbRemove.Text = "";
                RtbInclude.Text = "";
                RTB_SettleBalance.Text = "0";
            }
            if (e.Value != "" && RCB_CompanyList.SelectedValue.Trim() != "")
            {
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

                CompanyFundReceiptInfo info = _companyFundReceipt.GetFundReceiptInfoByLately(new Guid(RCB_CompanyList.SelectedValue), filialeId);
                if (info != null)
                {
                    if (info.SettleEndDate != DateTime.Parse("1999-09-09 00:00:00.000"))
                    {
                        RDP_StartDate.SelectedDate = info.SettleEndDate.AddDays(1);
                        RDP_StartDate.Enabled = false;
                    }
                    else
                    {
                        RDP_StartDate.SelectedDate = null;
                        RDP_StartDate.Enabled = true;
                    }
                }
                else
                {
                    RDP_StartDate.SelectedDate = null;
                    RDP_StartDate.Enabled = true;
                }
                BindCompany(new Guid(RCB_CompanyList.SelectedValue));
            }
        }

        /// <summary> 付款公司
        /// </summary>
        /// <param name="filialeId"></param>
        protected void FilialeList(Guid filialeId)
        {
            var companyId = new Guid(RCB_CompanyList.SelectedValue);
            if (companyId == Guid.Empty)
            {
                RAM.Alert("又瞎搞了吧，没有选择对往来单位啦！");
                return;
            }
            if (filialeId == Guid.Empty) return;
            if (RbAdvance.Checked)
            {
                decimal totalNumber = _reckoning.GetReckoningNonceTotalledByFilialeId(companyId, filialeId, DateTime.Now);
                RTB_SettleBalance.Text = totalNumber != 0 ? (-totalNumber).ToString("#.00") : "0";
            }
        }

        #endregion

        #region[显示对应付款公司]

        protected void ShowFiliale(IList<FilialeInfo> filiales)
        {
            foreach (var filialeInfo in filiales)
            {
                if (filialeInfo != null)
                {
                    RCB_FilialeList.Items.Add(new RadComboBoxItem(filialeInfo.Name, filialeInfo.ID.ToString()));
                }
            }
            RCB_FilialeList.Items.Add(new RadComboBoxItem(string.Empty, Guid.Empty.ToString()));
            RCB_FilialeList.SelectedValue = CompanyFundReceiptInfoModel != null ? CompanyFundReceiptInfoModel.FilialeId.ToString() : Guid.Empty.ToString();
        }

        #endregion

        protected void RG_InvoiceList_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            decimal sumNoTaxAmount = 0, sumTax = 0, sumTaxAmount = 0;
            var dataList = CompanyFundReceiptInvoiceInfoList;
            if (CompanyFundReceiptInvoiceInfoList.Any())
            {
                dataList = CompanyFundReceiptInvoiceInfoList.OrderByDescending(p => p.BillingDate).ToList();
                //合计金额
                var invoiceCode = RG_InvoiceList.MasterTableView.Columns.FindByUniqueName("InvoiceCode");
                var noTaxAmount = RG_InvoiceList.MasterTableView.Columns.FindByUniqueName("NoTaxAmount");
                var tax = RG_InvoiceList.MasterTableView.Columns.FindByUniqueName("Tax");
                var taxAmount = RG_InvoiceList.MasterTableView.Columns.FindByUniqueName("TaxAmount");

                txt_BillingUnit.Text = CompanyFundReceiptInvoiceInfoList[0].BillingUnit;
                //rbl_InvoiceType.SelectedValue = string.Format("{0}", CompanyFundReceiptInvoiceInfoList[0].InvoiceType);
                #region 补充发票信息
                if (Request.QueryString["Type"] != null && Request.QueryString["Type"].Equals("1"))
                {
                    txt_BillingUnit.ReadOnly = string.IsNullOrEmpty(txt_BillingUnit.Text) ? true : false;
                }
                #endregion

                invoiceCode.FooterText = "合计：";
                sumNoTaxAmount = CompanyFundReceiptInvoiceInfoList.Sum(p => p.NoTaxAmount);
                noTaxAmount.FooterText = string.Format("{0}", WebControl.NumberSeparator(sumNoTaxAmount));
                sumTax = CompanyFundReceiptInvoiceInfoList.Sum(p => p.Tax);
                tax.FooterText = string.Format("{0}", WebControl.NumberSeparator(sumTax));
                sumTaxAmount = CompanyFundReceiptInvoiceInfoList.Sum(p => p.TaxAmount);
                taxAmount.FooterText = string.Format("{0}", WebControl.NumberSeparator(sumTaxAmount));
            }
            RG_InvoiceList.DataSource = dataList;
        }


        //上传发票信息
        protected void btn_Upload_Click(object sender, EventArgs e)
        {
            var excelName = UploadExcelName.Text;
            UploadExcelName.Text = string.Empty;
            if (!UploadExcel.HasFile || string.IsNullOrEmpty(excelName))
            {
                RAM.Alert("请选择格式为“.xls”文件！");
                return;
            }

            var ext = Path.GetExtension(UploadExcel.FileName);
            if (!ext.Equals(".xls"))
            {
                RAM.Alert("文件格式错误(.xls)！");
                return;
            }

            var txtBillingUnit = txt_BillingUnit.Text;
            if (string.IsNullOrEmpty(txtBillingUnit))
            {
                RAM.Alert("开票单位不能为空！");
                return;
            }

            try
            {
                #region 将上传文件保存至临时文件夹
                string fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + ext;
                string folderPath = "~/UserDir/Temp/";
                if (!Directory.Exists(Server.MapPath(folderPath)))
                {
                    Directory.CreateDirectory(Server.MapPath(folderPath));
                }
                string filePath = Server.MapPath(folderPath + fileName);
                UploadExcel.PostedFile.SaveAs(filePath);
                #endregion

                var excelDataTable = ExcelHelper.GetDataSet(filePath).Tables[0];

                #region 获取数据之后删除临时文件
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                #endregion

                List<CompanyFundReceiptInvoiceInfo> list = new List<CompanyFundReceiptInvoiceInfo>();
                var receiptId = Request.QueryString["ID"] == null ? Guid.Empty : new Guid(Request.QueryString["ID"]);
                StringBuilder errorMsg = new StringBuilder();
                int index = 2;
                for (int i = 0; i < excelDataTable.Rows.Count; i++)
                {
                    var billingDate = excelDataTable.Rows[i]["开票日期"].ToString();
                    var invoiceNo = excelDataTable.Rows[i]["发票号码"].ToString();
                    var invoiceCode = excelDataTable.Rows[i]["发票代码"].ToString();
                    var noTaxAmount = string.Empty;
                    var tax = string.Empty;

                    if (!int.Parse(rbl_InvoiceType.SelectedValue).Equals((int)CostReportInvoiceType.Invoice))
                    {

                        noTaxAmount = excelDataTable.Rows[i]["金额(不含税)"].ToString();
                        tax = excelDataTable.Rows[i]["税额"].ToString();
                        if (string.IsNullOrEmpty(noTaxAmount))
                        {
                            errorMsg.Append("第").Append(index).Append("行“金额(不含税)”为空！").Append("\\n");
                        }
                        if (string.IsNullOrEmpty(tax))
                        {
                            errorMsg.Append("第").Append(index).Append("行“税额”为空！").Append("\\n");
                        }
                        decimal tryNoTaxAmount;
                        if (!decimal.TryParse(noTaxAmount, out tryNoTaxAmount))
                        {
                            errorMsg.Append("第").Append(index).Append("行“金额(不含税)”格式错误！").Append("\\n");
                        }
                        decimal tryTax;
                        if (!decimal.TryParse(tax, out tryTax))
                        {
                            errorMsg.Append("第").Append(index).Append("行“税额”格式错误！").Append("\\n");
                        }
                    }

                    var taxAmount = excelDataTable.Rows[i]["含税额"].ToString();

                    #region 验证数据空值
                    if (string.IsNullOrEmpty(billingDate))
                    {
                        errorMsg.Append("第").Append(index).Append("行“开票日期”为空！").Append("\\n");
                    }
                    if (string.IsNullOrEmpty(invoiceNo))
                    {
                        errorMsg.Append("第").Append(index).Append("行“发票号码”为空！").Append("\\n");
                    }
                    if (string.IsNullOrEmpty(invoiceCode))
                    {
                        errorMsg.Append("第").Append(index).Append("行“发票代码”为空！").Append("\\n");
                    }

                    if (string.IsNullOrEmpty(taxAmount))
                    {
                        errorMsg.Append("第").Append(index).Append("行“含税额”为空！").Append("\\n");
                    }
                    #endregion

                    #region 验证数据格式
                    DateTime tryBillingDate;
                    if (!DateTime.TryParse(billingDate, out tryBillingDate))
                    {
                        errorMsg.Append("第").Append(index).Append("行“开票日期”格式错误！").Append("\\n");
                    }


                    decimal tryTaxAmount;
                    if (!decimal.TryParse(taxAmount, out tryTaxAmount))
                    {
                        errorMsg.Append("第").Append(index).Append("行“含税额”格式错误！").Append("\\n");
                    }
                    #endregion

                    index++;
                }

                if (!string.IsNullOrEmpty(errorMsg.ToString()))
                {
                    RAM.Alert(errorMsg.ToString());
                    return;
                }

                for (int i = 0; i < excelDataTable.Rows.Count; i++)
                {
                    var billingDate = excelDataTable.Rows[i]["开票日期"].ToString();
                    var invoiceNo = excelDataTable.Rows[i]["发票号码"].ToString();
                    var invoiceCode = excelDataTable.Rows[i]["发票代码"].ToString();
                    var noTaxAmount = string.Empty;
                    var tax = string.Empty;
                    if (!int.Parse(rbl_InvoiceType.SelectedValue).Equals((int)CostReportInvoiceType.Invoice))
                    {
                        noTaxAmount = excelDataTable.Rows[i]["金额(不含税)"].ToString();
                        tax = excelDataTable.Rows[i]["税额"].ToString();
                    }

                    var taxAmount = excelDataTable.Rows[i]["含税额"].ToString();

                    list.Add(new CompanyFundReceiptInvoiceInfo
                    {
                        InvoiceId = Guid.NewGuid(),
                        ReceiptID = receiptId,
                        BillingUnit = txtBillingUnit,
                        BillingDate = Convert.ToDateTime(billingDate),
                        InvoiceNo = invoiceNo,
                        InvoiceCode = invoiceCode,
                        NoTaxAmount = string.IsNullOrWhiteSpace(noTaxAmount) ? 0 : decimal.Parse(noTaxAmount),
                        Tax = string.IsNullOrWhiteSpace(tax) ? 0 : decimal.Parse(tax),
                        TaxAmount = decimal.Parse(taxAmount),
                        OperatingTime = DateTime.Now,
                        Memo = string.Empty,
                        Remark = WebControl.RetrunUserAndTime("【添加发票】")
                    });
                }

                if (list.Any())
                {
                    var tempList = CompanyFundReceiptInvoiceInfoList;
                    tempList.AddRange(list);
                    CompanyFundReceiptInvoiceInfoList = tempList;

                    #region 补充发票信息
                    if (Request.QueryString["Type"] != null && Request.QueryString["Type"].Equals("1"))
                    {
                        _companyFundReceiptInvoice.AddBulklmshop_CompanyFundReceiptInvoice(list);
                    }
                    #endregion

                    RG_InvoiceList.Rebind();
                }
            }
            catch (Exception ex)
            {
                RAM.Alert(ex.Message);
            }
        }

        //添加发票信息
        protected void AddCompanyFundReceiptInvoiceInfo(Guid receiptId)
        {
            if (CompanyFundReceiptInvoiceInfoList.Any())
            {
                foreach (var item in CompanyFundReceiptInvoiceInfoList)
                {
                    item.ReceiptID = receiptId;
                    item.InvoiceState = (int)CompanyFundReceiptInvoiceState.UnSubmit;
                    item.OperatingTime = DateTime.Now;
                    //item.InvoiceType = int.Parse(rbl_InvoiceType.SelectedValue);
                }
                _companyFundReceiptInvoice.AddBulklmshop_CompanyFundReceiptInvoice(CompanyFundReceiptInvoiceInfoList);
            }
        }

        protected void Task(PurchasingInfo purchasingInfo, StorageRecordInfo stockInfo)
        {
            RCB_CompanyList.Items.Clear();
            RCB_CompanyList.Text = string.Empty;
            RCB_CompanyList.Items.Insert(0, new RadComboBoxItem(purchasingInfo.CompanyName, purchasingInfo.CompanyID.ToString()));
            RCB_CompanyList.SelectedIndex = 0;
            RCB_CompanyList.Enabled = false;

            RCB_FilialeList.Items.Clear();
            RCB_FilialeList.Text = string.Empty;
            if (purchasingInfo.IsOut)
            {
                var filialeInfo = CacheCollection.Filiale.Get(purchasingInfo.PurchasingFilialeId);
                RCB_FilialeList.Items.Add(new RadComboBoxItem(filialeInfo.Name, filialeInfo.ID.ToString()));
            }
            else
            {
                RCB_FilialeList.Items.Add(new RadComboBoxItem("ERP", _reckoningElseFilialeid.ToString()));
            }
            RCB_FilialeList.SelectedIndex = 0;
            RCB_FilialeList.Enabled = false;

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

            if (stockInfo != null)
            {
                RTB_SettleBalance.Text = stockInfo.AccountReceivable.ToString("#.00");
            }
        }

        /// <summary>
        /// 判断添加的入库单是否符合要求
        /// 同往来单位且未对账、在日期之外
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RtbIncludeTextChanged(object sender, EventArgs e)
        {
            Check(true, RtbInclude.Text);
        }

        /// <summary>
        /// 判断排除的入库单是否符合要求
        /// 同往来单位且未对账、在日期之内
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RtbRemoveTextChanged(object sender, EventArgs e)
        {
            Check(false, RtbRemove.Text);
        }

        //判断退货单是否存在 zal 2015-01-21
        protected void RTB_ReturnOrder_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(RCB_CompanyList.SelectedValue) || new Guid(RCB_CompanyList.SelectedValue).Equals(Guid.Empty))
            {
                RAM.Alert("请选择“往来单位”！");
                return;
            }
            if (string.IsNullOrEmpty(RCB_FilialeList.SelectedValue) || new Guid(RCB_FilialeList.SelectedValue).Equals(Guid.Empty))
            {
                RAM.Alert("请选择“付款公司”！");
                return;
            }
            CheckReturnOrder();
        }

        //验证退货单 zal 2015-01-21
        protected void CheckReturnOrder()
        {
            var returnOrder = RTB_ReturnOrder.Text.Trim();
            if (!string.IsNullOrEmpty(returnOrder))
            {
                StringBuilder errorMsg = new StringBuilder();
                var strReturnOrder = string.Empty;
                var returnOrderList = returnOrder.Split(',');
                foreach (var item in returnOrderList)
                {
                    if (string.IsNullOrEmpty(item)) continue;
                    if (strReturnOrder.Contains(item))
                    {
                        errorMsg.Append("“退货单号”重复！").Append("\\n");
                        break;
                    }
                    //判断“退货单号”是否是采购退货类型，并且状态是“通过”的
                    bool result = _utility.CheckExists("StorageRecord", "TradeCode,StockType,StockState", new object[] { item, (int)StorageRecordType.BuyStockOut, (int)StorageRecordState.Finished }) > 0;
                    if (!result)
                    {
                        errorMsg.Append("“退货单号(" + item + ")”不存在！").Append("\\n");
                    }
                    var receiptId = Request.QueryString["ID"] == null ? Guid.Empty : new Guid(Request.QueryString["ID"]);
                    bool exist = _companyFundReceipt.CheckReturnOrderOrPayOrder(receiptId, item, string.Empty);
                    if (exist)
                    {
                        errorMsg.Append("“退货单号(" + item + ")”已存在其他付款单中！").Append("\\n");
                    }
                }
                if (!string.IsNullOrEmpty(errorMsg.ToString()))
                {
                    Lit_ReturnOrder.Text = returnOrder;
                    RTB_ReturnOrder.Text = string.Empty;
                    RAM.Alert(errorMsg.ToString());
                }
                else
                {
                    if (IsPostBack)
                    {
                        //排除相关单据
                        var tradeCodeList = _companyFundReceipt.CheckExistForStorageNo(new Guid(RCB_CompanyList.SelectedValue), new Guid(RCB_FilialeList.SelectedValue), RTB_ReturnOrder.Text.Trim());
                        if (tradeCodeList.Any())
                        {
                            string tradeCode = string.Empty;
                            foreach (var item in tradeCodeList)
                            {
                                tradeCode += "," + item;
                                RTB_ReturnOrder.Text = RTB_ReturnOrder.Text.IndexOf("," + item, StringComparison.Ordinal) > -1 ? RTB_ReturnOrder.Text.Replace("," + item, "") : (RTB_ReturnOrder.Text.IndexOf(item + ",", StringComparison.Ordinal) > -1 ? RTB_ReturnOrder.Text.Replace(item + ",", "") : RTB_ReturnOrder.Text.Replace(item, ""));
                            }
                            if (!string.IsNullOrEmpty(tradeCode))
                            {
                                RAM.Alert("入库单号“" + tradeCode.Substring(1) + "”已添加过付款单!");
                                return;
                            }
                        }
                    }

                    var storageRecordInfoList = _storageRecordDao.GetStorageRecordList(returnOrder);
                    decimal accountReceivable = 0;
                    foreach (var item in storageRecordInfoList)
                    {
                        accountReceivable += item.AccountReceivable;
                        if (!item.ThirdCompanyID.Equals(new Guid(RCB_CompanyList.SelectedValue)))
                        {
                            errorMsg.Append("“退货单号(" + item.TradeCode + ")”的往来单位和所选“往来单位”不一致！").Append("\\n");
                        }
                        if (!item.FilialeId.Equals(new Guid(RCB_FilialeList.SelectedValue)))
                        {
                            errorMsg.Append("“退货单号(" + item.TradeCode + ")”的付款公司和所选“付款公司”不一致！").Append("\\n");
                        }
                    }

                    if (!string.IsNullOrEmpty(errorMsg.ToString()))
                    {
                        RAM.Alert(errorMsg.ToString());
                    }
                    else
                    {
                        Lit_ReturnOrderMoney.Text = Math.Abs(accountReceivable).ToString(CultureInfo.InvariantCulture);
                        Lit_ReturnOrder.Text = string.Empty;
                    }
                }
            }
            else
            {
                Lit_ReturnOrderMoney.Text = "0";
                Lit_ReturnOrder.Text = string.Empty;
            }
        }

        //判断付款单是否存在 zal 2015-01-21
        protected void RTB_PayOrder_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(RCB_CompanyList.SelectedValue) || new Guid(RCB_CompanyList.SelectedValue).Equals(Guid.Empty))
            {
                RAM.Alert("请选择“往来单位”！");
                return;
            }
            if (string.IsNullOrEmpty(RCB_FilialeList.SelectedValue) || new Guid(RCB_FilialeList.SelectedValue).Equals(Guid.Empty))
            {
                RAM.Alert("请选择“付款公司”！");
                return;
            }
            CheckPayOrder();
        }

        //验证付款单 zal 2015-01-21
        protected void CheckPayOrder()
        {
            var payOrder = RTB_PayOrder.Text.Trim();
            if (!string.IsNullOrEmpty(payOrder))
            {
                StringBuilder errorMsg = new StringBuilder();
                var strPayOrder = string.Empty;
                var returnOrderList = payOrder.Split(',');
                foreach (var item in returnOrderList)
                {
                    if (string.IsNullOrEmpty(item)) continue;
                    if (strPayOrder.Contains(item))
                    {
                        errorMsg.Append("“付款单号”重复！").Append("\\n");
                        break;
                    }
                    //判断“付款单号”的状态是否是“完成打款”的 ReceiptType(0:收款；1:付款；)
                    bool result = _utility.CheckExists("lmshop_CompanyFundReceipt", "ReceiptNo,ReceiptStatus,ReceiptType", new object[] { item, 9, 1 }) > 0;
                    if (!result)
                    {
                        errorMsg.Append("“付款单号(" + item + ")”不存在或未完成打款！").Append("\\n");
                    }

                    var receiptId = Request.QueryString["ID"] == null ? Guid.Empty : new Guid(Request.QueryString["ID"]);
                    bool exist = _companyFundReceipt.CheckReturnOrderOrPayOrder(receiptId, string.Empty, item);
                    if (exist)
                    {
                        errorMsg.Append("“付款单号(" + item + ")”已存在其他付款单中！").Append("\\n");
                    }
                }
                if (!string.IsNullOrEmpty(errorMsg.ToString()))
                {
                    Lit_PayOrder.Text = payOrder;
                    RTB_PayOrder.Text = string.Empty;
                    RAM.Alert(errorMsg.ToString());
                }
                else
                {
                    var storageRecordInfoList = _companyFundReceipt.GetCompanyFundReceiptList(payOrder);
                    decimal realityBalance = 0;
                    foreach (var item in storageRecordInfoList)
                    {
                        realityBalance += item.RealityBalance;
                        if (!item.CompanyID.Equals(new Guid(RCB_CompanyList.SelectedValue)))
                        {
                            errorMsg.Append("“付款单号(" + item.ReceiptNo + ")”的往来单位和所选“往来单位”不一致！").Append("\\n");
                        }
                        if (!item.FilialeId.Equals(new Guid(RCB_FilialeList.SelectedValue)))
                        {
                            errorMsg.Append("“付款单号(" + item.ReceiptNo + ")”的付款公司和所选“付款公司”不一致！").Append("\\n");
                        }
                    }
                    if (!string.IsNullOrEmpty(errorMsg.ToString()))
                    {
                        RAM.Alert(errorMsg.ToString());
                    }
                    else
                    {
                        Lit_PayOrderMoney.Text = Math.Abs(realityBalance).ToString(CultureInfo.InvariantCulture);
                        Lit_PayOrder.Text = string.Empty;
                    }
                }
            }
            else
            {
                Lit_PayOrderMoney.Text = "0";
                Lit_PayOrder.Text = string.Empty;
            }
        }

        protected void Check(bool isAdd, string text)
        {
            var includeNos = text.Trim();
            if (!string.IsNullOrEmpty(includeNos))
            {
                //排除相关单据
                var tradeCodeList = _companyFundReceipt.CheckExistForStorageNo(new Guid(RCB_CompanyList.SelectedValue), new Guid(RCB_FilialeList.SelectedValue), includeNos);
                if (tradeCodeList.Any())
                {
                    string tradeCode = string.Empty;
                    foreach (var item in tradeCodeList)
                    {
                        tradeCode += "," + item;
                        RtbInclude.Text = includeNos = includeNos.IndexOf("," + item, StringComparison.Ordinal) > -1 ? includeNos.Replace("," + item, "") : (includeNos.IndexOf(item + ",", StringComparison.Ordinal) > -1 ? includeNos.Replace(item + ",", "") : includeNos.Replace(item, ""));
                    }
                    if (!string.IsNullOrEmpty(tradeCode))
                    {
                        RAM.Alert("入库单号“" + tradeCode.Substring(1) + "”已添加过付款单!");
                        return;
                    }
                }
                var dics = new Dictionary<string, string>();
                var nos = includeNos.Split(',');
                var list = new List<string>();
                var acutallList = new List<string>();
                foreach (var no in nos)
                {
                    if (string.IsNullOrEmpty(no)) continue;
                    if (list.Contains(no))
                    {
                        dics.Add(no, string.Format("入库单{0}重复", no));
                        break;
                    }
                    list.Add(no);

                    if (!_reckoning.IsExists(no))
                    {
                        dics.Add(no, string.Format("入库单{0}对应的往来帐不存在", no));
                        continue;
                    }
                    if (_companyFundReceipt.IsExistsByStockOrderNos(no, CompanyFundReceiptInfoModel.ReceiptNo))
                    {
                        dics.Add(no, string.Format("入库单{0}已存在对应的收付款单据", no));
                        continue;
                    }
                    var guid = _reckoning.GetReckoningInfoByTradeCode(no);
                    if (guid == Guid.Empty)
                    {
                        dics.Add(no, string.Format("入库单对应的往来帐{0}已对账", no));
                        continue;
                    }
                    var reckoningInfo = _reckoning.GetReckoning(guid);
                    if (reckoningInfo == null)
                    {
                        dics.Add(no, string.Format("入库单{0}对应往来帐不存在", no));
                        continue;
                    }
                    if (reckoningInfo.ThirdCompanyID != new Guid(RCB_CompanyList.SelectedValue))
                    {
                        dics.Add(no, string.Format("入库单对应往来帐{0}往来单位不一致", reckoningInfo.TradeCode));
                        continue;
                    }
                    if (reckoningInfo.IsOut && reckoningInfo.FilialeId != new Guid(RCB_FilialeList.SelectedValue))
                    {
                        dics.Add(no, string.Format("入库单对应往来帐{0}付款公司不一致", reckoningInfo.TradeCode));
                        continue;
                    }
                    var strs = new List<string> { "LO", "LI", "BO", "BI" };
                    if (!string.IsNullOrWhiteSpace(reckoningInfo.LinkTradeCode) && strs.Contains(reckoningInfo.LinkTradeCode.Substring(0, 2)))
                    {
                        dics.Add(no, string.Format("借入借出单往来帐无需对账{0}", reckoningInfo.TradeCode));
                        continue;
                    }
                    if (new Guid(RCB_FilialeList.SelectedValue) == _reckoningElseFilialeid && reckoningInfo.IsOut)
                    {
                        dics.Add(no, string.Format("入库单(对应的往来账{0})往来公司为非其它公司且IsOut=1", reckoningInfo.TradeCode));
                        continue;
                    }
                    if (new Guid(RCB_FilialeList.SelectedValue) != _reckoningElseFilialeid && !reckoningInfo.IsOut)
                    {
                        dics.Add(no, string.Format("入库单(对应的往来账{0})往来公司为其它公司且IsOut=0", reckoningInfo.TradeCode));
                        continue;
                    }
                    var endDate = Convert.ToDateTime(RDP_EndDate.SelectedDate).AddDays(1).AddSeconds(-1);
                    if (isAdd)
                    {
                        if (reckoningInfo.DateCreated >= RDP_StartDate.SelectedDate
                        && reckoningInfo.DateCreated <= endDate)
                        {
                            dics.Add(no, string.Format("入库单对应往来帐{0}在日期之内", reckoningInfo.TradeCode));
                            continue;
                        }
                    }
                    else
                    {
                        if (reckoningInfo.DateCreated < RDP_StartDate.SelectedDate
                        || reckoningInfo.DateCreated > endDate)
                        {
                            dics.Add(no, string.Format("入库单对应往来帐{0}不在日期之内", reckoningInfo.TradeCode));
                            continue;
                        }
                    }
                    acutallList.Add(no);
                }
                string stockNos = "";
                if (acutallList.Count > 0)
                {
                    for (var i = 0; i < acutallList.Count; i++)
                    {
                        stockNos += string.Format("{0}{1}", acutallList[i], i == acutallList.Count - 1 ? "" : ",");
                    }
                }
                if (isAdd)
                    RtbInclude.Text = stockNos;
                else
                {
                    RtbRemove.Text = stockNos;
                }
                decimal totalNumber = GetTotalloled(RtbInclude.Text.Trim().Split(',').ToList(), RtbRemove.Text.Trim().Split(',').ToList());
                RTB_SettleBalance.Text = totalNumber != 0 ? Math.Abs(-totalNumber).ToString("#.00") : "0";
                if (dics.Count > 0)
                {
                    var error = dics.Values.Aggregate(string.Format("{0}：\n", isAdd ? "添加" : "排除："), (current, value) => current + (value + "\n"));
                    RAM.Alert(error);
                }
            }
            else
            {
                decimal totalNumber = GetTotalloled(RtbInclude.Text.Trim().Split(',').ToList(), RtbRemove.Text.Trim().Split(',').ToList());
                RTB_SettleBalance.Text = totalNumber != 0 ? Math.Abs(-totalNumber).ToString("#.00") : "0";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RdpStartDateOnSelectedDateChanged(object sender, SelectedDateChangedEventArgs e)
        {
            RtbInclude.Text = "";
            RtbRemove.Text = "";
            var start = RDP_StartDate.SelectedDate ?? DateTime.MinValue;
            var end = RDP_EndDate.SelectedDate ?? DateTime.MinValue;
            if (start != DateTime.MinValue && end != DateTime.MinValue)
            {
                if (!IsSerarchYear(start, end, Convert.ToInt32(GlobalConfig.KeepYear)))
                {
                    RDP_StartDate.SelectedDate = null;
                    RAM.Alert("温馨提示：不支持当前时间段搜索，请检查配置文件！");
                }
            }
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

        /// <summary>
        /// 入库验证
        /// </summary>
        /// <param name="dics"></param>
        /// <param name="amountSum"></param>
        /// <param name="tips"></param>
        /// <returns></returns>
        protected bool IsValidate(Dictionary<string, ReckoningInfo> dics, decimal amountSum, out string tips)
        {
            if (string.IsNullOrEmpty(TB_Orders.Text.Trim()))
            {
                tips = "入库单号不能为空!";
                return false;
            }
            var orderNos = TB_Orders.Text.Split(',');
            foreach (var orderNo in orderNos)
            {
                if (string.IsNullOrEmpty(orderNo)) continue;
                var guid = _reckoning.GetReckoningInfoByTradeCode(orderNo);
                if (guid == Guid.Empty)
                {
                    tips = string.Format("入库单{0}对应的往来帐已对账!", orderNo);
                    return false;
                }
                var reckoningInfo = _reckoning.GetReckoning(guid);
                if (reckoningInfo == null)
                {
                    tips = string.Format("入库单{0}对应往来帐不存在", orderNo);
                    return false;
                }
                if (reckoningInfo.LinkTradeType != (int)ReckoningLinkTradeType.StockIn)
                {
                    tips = string.Format("入库单{0}对应往来帐不是入库单类型", orderNo);
                    return false;
                }
                amountSum += Math.Abs(reckoningInfo.AccountReceivable);
                dics.Add(orderNo, reckoningInfo);
            }
            var companyIds = dics.Values.Select(act => act.ThirdCompanyID).Distinct().ToList();
            var filialeids = dics.Values.Select(act => act.FilialeId).Distinct().ToList();
            if (companyIds.Count > 1)
            {
                tips = string.Format("入库单中存在多个供应商的往来帐");
                return false;
            }
            if (filialeids.Count > 1)
            {
                tips = string.Format("入库单中存在多个付款公司的往来帐");
                return false;
            }
            if (amountSum != Convert.ToDecimal(RTB_RealityBalance.Text.Trim()) +
                Math.Abs(Convert.ToDecimal(string.IsNullOrEmpty(RTB_DiscountMoney.Text.Trim()) ? "0" : RTB_DiscountMoney.Text.Trim())) +
               Math.Abs(Convert.ToDecimal(string.IsNullOrEmpty(RtbLastRebate.Text.Trim()) ? "0" : RtbLastRebate.Text.Trim())))
            {
                tips = "应付金额+今年折扣+去年返利不等于单据总额额，当前单据总额为：" + amountSum.ToString("#.00") + "元";
                return false;
            }
            tips = string.Empty;
            return true;
        }


        protected void btn_Add_OnClick(object sender, EventArgs e)
        {
            var dataItem = (GridDataItem)((Button)sender).Parent.Parent;
            if (dataItem != null && dataItem.Edit)
            {
                var txtBillingUnit = txt_BillingUnit.Text;
                if (string.IsNullOrEmpty(txtBillingUnit))
                {
                    RAM.Alert("开票单位不能为空！");
                    return;
                }
                var receiptId = Request.QueryString["ID"] == null ? Guid.Empty : new Guid(Request.QueryString["ID"]);
                var rdpBillingDate = ((RadDatePicker)dataItem.FindControl("RDP_BillingDate"));
                var txtInvoiceNo = (TextBox)dataItem.FindControl("txt_InvoiceNo");
                var txtInvoiceCode = (TextBox)dataItem.FindControl("txt_InvoiceCode");
                var txtNoTaxAmount = (TextBox)dataItem.FindControl("txt_NoTaxAmount");
                var txtTax = (TextBox)dataItem.FindControl("txt_Tax");
                var txtTaxAmount = (TextBox)dataItem.FindControl("txt_TaxAmount");
                var txtMemo = (TextBox)dataItem.FindControl("txt_Memo");

                var companyFundReceiptInvoiceInfo = new CompanyFundReceiptInvoiceInfo
                {
                    InvoiceId = Guid.NewGuid(),
                    ReceiptID = receiptId,
                    BillingUnit = txtBillingUnit,
                    BillingDate = rdpBillingDate.SelectedDate,
                    InvoiceNo = txtInvoiceNo.Text,
                    InvoiceCode = txtInvoiceCode.Text,
                    NoTaxAmount = String.IsNullOrWhiteSpace(txtNoTaxAmount.Text) ? decimal.Parse("0") : decimal.Parse(txtNoTaxAmount.Text),
                    Tax = String.IsNullOrWhiteSpace(txtTax.Text) ? decimal.Parse("0") : decimal.Parse(txtTax.Text),
                    TaxAmount = decimal.Parse(txtTaxAmount.Text),
                    InvoiceState = (int)CompanyFundReceiptInvoiceState.UnSubmit,
                    OperatingTime = DateTime.Now,
                    Memo = txtMemo.Text,
                    Remark = WebControl.RetrunUserAndTime("【添加发票】")
                };
                var list = CompanyFundReceiptInvoiceInfoList;
                list.Add(companyFundReceiptInvoiceInfo);
                CompanyFundReceiptInvoiceInfoList = list;

                #region 补充发票信息
                if (Request.QueryString["Type"] != null && Request.QueryString["Type"].Equals("1"))
                {
                    _companyFundReceiptInvoice.Addlmshop_CompanyFundReceiptInvoice(companyFundReceiptInvoiceInfo);
                }
                #endregion

                dataItem.Edit = false;
            }
            RG_InvoiceList.Rebind();
        }

        protected void btn_Cancel_OnClick(object sender, EventArgs e)
        {
            var dataItem = (GridDataItem)((Button)sender).Parent.Parent;
            if (dataItem != null)
            {
                dataItem.Edit = false;
            }
            RG_InvoiceList.Rebind();
        }

        protected void btn_Edit_OnClick(object sender, EventArgs e)
        {
            #region 补充发票信息时禁止修改
            if (Request.QueryString["Type"] != null)
            {
                return;
            }
            #endregion

            var dataItem = (GridDataItem)((Button)sender).Parent.Parent;
            if (dataItem != null)
            {
                var invoiceId = new Guid(dataItem.GetDataKeyValue("InvoiceId").ToString());
                var receiptId = Request.QueryString["ID"] == null ? Guid.Empty : new Guid(Request.QueryString["ID"]);
                var rdpBillingDate = ((RadDatePicker)dataItem.FindControl("RDP_BillingDate"));
                var txtInvoiceNo = (TextBox)dataItem.FindControl("txt_InvoiceNo");
                var txtInvoiceCode = (TextBox)dataItem.FindControl("txt_InvoiceCode");
                var txtNoTaxAmount = (TextBox)dataItem.FindControl("txt_NoTaxAmount");
                var txtTax = (TextBox)dataItem.FindControl("txt_Tax");
                var txtTaxAmount = (TextBox)dataItem.FindControl("txt_TaxAmount");
                var txtMemo = (TextBox)dataItem.FindControl("txt_Memo");
                var companyFundReceiptInvoiceInfo = new CompanyFundReceiptInvoiceInfo
                {
                    InvoiceId = invoiceId,
                    ReceiptID = receiptId,
                    BillingUnit = txt_BillingUnit.Text,
                    BillingDate = rdpBillingDate.SelectedDate,
                    InvoiceNo = txtInvoiceNo.Text,
                    InvoiceCode = txtInvoiceCode.Text,
                    NoTaxAmount = decimal.Parse(txtNoTaxAmount.Text),
                    Tax = decimal.Parse(txtTax.Text),
                    TaxAmount = decimal.Parse(txtTaxAmount.Text),
                    Memo = txtMemo.Text,
                    //InvoiceType = int.Parse(rbl_InvoiceType.SelectedValue),
                    Remark = WebControl.RetrunUserAndTime("【添加发票】") + WebControl.RetrunUserAndTime("【修改发票】")
                };

                var removeItem = CompanyFundReceiptInvoiceInfoList.FirstOrDefault(p => p.InvoiceId.Equals(invoiceId));
                CompanyFundReceiptInvoiceInfoList.Remove(removeItem);

                var list = CompanyFundReceiptInvoiceInfoList;
                list.Add(companyFundReceiptInvoiceInfo);
                CompanyFundReceiptInvoiceInfoList = list;
            }
            RG_InvoiceList.Rebind();
        }

        protected void btn_Del_OnClick(object sender, EventArgs e)
        {
            var dataItem = (GridDataItem)((Button)sender).Parent.Parent;
            if (dataItem != null)
            {
                var invoiceId = new Guid(dataItem.GetDataKeyValue("InvoiceId").ToString());
                var removeItem = CompanyFundReceiptInvoiceInfoList.FirstOrDefault(p => p.InvoiceId.Equals(invoiceId));
                CompanyFundReceiptInvoiceInfoList.Remove(removeItem);
            }
            RG_InvoiceList.Rebind();
        }

        protected void RG_InvoiceList_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item.ItemType == GridItemType.Item || e.Item.ItemType == GridItemType.AlternatingItem)
            {
                var btnAdd = ((Button)e.Item.FindControl("btn_Add"));
                var btnCancel = (Button)e.Item.FindControl("btn_Cancel");
                var btnEdit = (Button)e.Item.FindControl("btn_Edit");
                var btnDel = (Button)e.Item.FindControl("btn_Del");
                btnAdd.Visible = false;
                btnCancel.Visible = false;
                if (Request.QueryString["Type"] != null && Request.QueryString["Type"].Equals("1"))
                {
                    btnEdit.Visible = false;
                    btnDel.Visible = false;
                }
                else
                {
                    btnEdit.Visible = true;
                    btnDel.Visible = true;
                }
            }
            else if (e.Item.ItemType == GridItemType.CommandItem)
            {
                InvoiceTypeChange();
                //if (RbAdvance.Checked)
                //{
                //    if (int.Parse(rbl_InvoiceType.SelectedValue).Equals((int)CostReportInvoiceType.Invoice))
                //    {
                //        ((TextBox)e.Item.FindControl("txt_TaxAmount")).Attributes.Remove("onfocus");
                //    }
                //    else
                //    {
                //        ((TextBox)e.Item.FindControl("txt_TaxAmount")).Attributes.Add("onfocus", "this.blur();");
                //    }
                //}
            }
        }

        /// <summary>根据配置文件判断当前搜索时间段是否搜索
        /// </summary>
        protected bool IsSerarchYear(DateTime startTime, DateTime endTime, int keepyear)
        {
            var nowYear = DateTime.Now.Year;
            var startYear = startTime.Year;
            var endYear = endTime.Year;
            if (keepyear == 1)
            {
                return startYear == endYear;
            }
            if (keepyear == 2)
            {
                if (startYear != endYear)
                {
                    return (startYear == nowYear || endYear == nowYear) && startYear == endYear - 1;
                }
                return startYear == endYear;
            }
            if (keepyear == 3)
            {
                if (startYear != endYear)
                {
                    var minYear = nowYear - 2;
                    if (startYear >= minYear && endYear <= nowYear)
                    {
                        return true;
                    }
                }
                return startYear == endYear;
            }
            return false;
        }

        protected void rbl_InvoiceType_SelectedIndexChanged(object sender, EventArgs e)
        {
            InvoiceTypeChange();
        }

        protected void InvoiceTypeChange()
        {
            if (int.Parse(rbl_InvoiceType.SelectedValue).Equals((int)CostReportInvoiceType.Invoice) || int.Parse(rbl_InvoiceType.SelectedValue).Equals((int)CostReportInvoiceType.VatInvoice))
            {
                if (int.Parse(rbl_InvoiceType.SelectedValue).Equals((int)CostReportInvoiceType.Invoice))
                {
                    Template.HRef = "../App_Themes/费用申报票据模板(普通发票).xls";
                    RG_InvoiceList.MasterTableView.Columns.FindByUniqueName("NoTaxAmount").Display = false;
                    RG_InvoiceList.MasterTableView.Columns.FindByUniqueName("Tax").Display = false;
                }
                else if (int.Parse(rbl_InvoiceType.SelectedValue).Equals((int)CostReportInvoiceType.VatInvoice))
                {
                    Template.HRef = "../App_Themes/费用申报票据模板(增值税专用发票).xls";
                    RG_InvoiceList.MasterTableView.Columns.FindByUniqueName("NoTaxAmount").Display = true;
                    RG_InvoiceList.MasterTableView.Columns.FindByUniqueName("Tax").Display = true;
                }
            }
        }

        protected void RbNormalCheckedChanged(object sender, EventArgs e)
        {
            RtbUrgent.Visible = RbUrgent.Checked;
            LbUrgentTitle.Visible = RbUrgent.Checked;
            LbUrgent.Visible= RbUrgent.Checked;
        }
    }
}
