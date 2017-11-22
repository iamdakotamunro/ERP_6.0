using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Transactions;
using System.Web.UI.WebControls;
using ERP.BLL.Implement;
using ERP.BLL.Implement.Organization;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Enum.Attribute;
using ERP.Environment;
using ERP.Model;
using ERP.SAL;
using ERP.SAL.Interface;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Keede.Ecsoft.Model;
using MIS.Enum;
using OperationLog.Core;
using Telerik.Web.UI;
using Cost = ERP.BLL.Implement.Inventory.Cost;
using WebControl = ERP.UI.Web.Common.WebControl;

namespace ERP.UI.Web.CostReport
{
    /************************************************************************************ 
     * 创建人：  张安龙
     * 创建时间：2015/11/11  
     * 描述    :费用申报审批执行
     * =====================================================================
     * 修改时间：2015/11/11  
     * 修改人  ：  
     * 描述    ：
     */
    public partial class CostReport_Audit : WindowsPage
    {
        private static readonly OperationLogManager _operationLogManager = new OperationLogManager();
        private static readonly IWasteBook _wasteBook = new WasteBook(GlobalConfig.DB.FromType.Write);
        private static readonly IBankAccounts _bankAccounts = new BankAccounts(GlobalConfig.DB.FromType.Read);
        private static readonly IPersonnelSao _personnelSao = new PersonnelSao();
        private static readonly ICostReckoning _costReckoning = new CostReckoning(GlobalConfig.DB.FromType.Write);
        private static readonly ICostReport _costReport = new DAL.Implement.Inventory.CostReport(GlobalConfig.DB.FromType.Write);
        private static readonly ICostReportInvoice _costReportInvoice = new CostReportInvoice(GlobalConfig.DB.FromType.Write);
        private static readonly ICostReportTravel _costReportTravel = new CostReportTravelDal(GlobalConfig.DB.FromType.Write);
        private static readonly ICostReportTermini _costReportTermini = new CostReportTerminiDal(GlobalConfig.DB.FromType.Write);
        private static readonly ICostReportBill _costReportBill = new CostReportBillDal(GlobalConfig.DB.FromType.Write);
        private static readonly ICostReportAmount _costReportAmount = new CostReportAmountDal(GlobalConfig.DB.FromType.Write);

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
        /// O2O事业部
        /// </summary>
        private string ShopBranchId
        {
            get
            {
                if (ViewState["ShopBranchId"] == null)
                {
                    ViewState["ShopBranchId"] = GlobalConfig.ShopBranchId;
                }
                return ViewState["ShopBranchId"].ToString();
            }
        }

        /// <summary>
        /// 费用申报=>费用分类(办公费=>差旅费)
        /// </summary>
        private string CostReport_TravelExpenses
        {
            get
            {
                if (ViewState["CostReport_TravelExpenses"] == null)
                {
                    ViewState["CostReport_TravelExpenses"] = ConfigManage.GetConfigValue("CostReport_TravelExpenses");
                }
                return ViewState["CostReport_TravelExpenses"].ToString();
            }
        }

        /// <summary>
        /// 费用申报=>费用分类(保证金押金)
        /// </summary>
        private string CostReport_DepositMargins
        {
            get
            {
                if (ViewState["CostReport_DepositMargins"] == null)
                {
                    ViewState["CostReport_DepositMargins"] = ConfigManage.GetConfigValue("CostReport_DepositMargins");
                }
                return ViewState["CostReport_DepositMargins"].ToString();
            }
        }

        /// <summary>
        /// 票据列表属性
        /// </summary>
        protected List<CostReportBillInfo> CostReportBillInfoList
        {
            get
            {
                if (ViewState["CostReportBillInfoList"] == null)
                    return new List<CostReportBillInfo>();
                return ViewState["CostReportBillInfoList"] as List<CostReportBillInfo>;
            }
            set
            {
                ViewState["CostReportBillInfoList"] = value;
            }
        }

        /// <summary>
        /// 差旅费列表属性
        /// </summary>
        protected List<CostReportTravelInfo> CostReportTravelInfoList
        {
            get
            {
                if (ViewState["CostReportTravelInfoList"] == null)
                    return new List<CostReportTravelInfo>();
                return ViewState["CostReportTravelInfoList"] as List<CostReportTravelInfo>;
            }
            set
            {
                ViewState["CostReportTravelInfoList"] = value;
            }
        }

        /// <summary>
        /// 起讫列表属性
        /// </summary>
        protected List<CostReportTerminiInfo> CostReportTerminiInfoList
        {
            get
            {
                if (ViewState["CostReportTerminiInfoList"] == null)
                    return new List<CostReportTerminiInfo>();
                return ViewState["CostReportTerminiInfoList"] as List<CostReportTerminiInfo>;
            }
            set
            {
                ViewState["CostReportTerminiInfoList"] = value;
            }
        }

        /// <summary>
        /// 申请金额列表属性
        /// </summary>
        protected List<CostReportAmountInfo> CostReportAmountInfoList
        {
            get
            {
                if (ViewState["CostReportAmountInfoList"] == null)
                    return new List<CostReportAmountInfo>();
                return ViewState["CostReportAmountInfoList"] as List<CostReportAmountInfo>;
            }
            set
            {
                ViewState["CostReportAmountInfoList"] = value;
            }
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var reportId = Request.QueryString["ReportId"];
                if (string.IsNullOrEmpty(reportId)) return;
                CostReportInfo costReportInfo = _costReport.GetReportByReportId(new Guid(reportId));

                LoadReportKindData();//加载申报类型
                LoadBranchData();//加载部门数据
                if (!costReportInfo.AssumeBranchId.Equals(Guid.Empty))
                {
                    LoadGroupData(costReportInfo.AssumeBranchId);//根据部门id获取“小组”数据
                }
                if (!costReportInfo.AssumeGroupId.Equals(Guid.Empty))
                {
                    LoadShopData();//加载门店数据
                    AssumeShop.Visible = costReportInfo.AssumeBranchId.Equals(new Guid(ShopBranchId));
                }
                LoadReportData(costReportInfo);//初始化页面数据
            }
        }

        #region 数据准备
        //申报类型
        protected void LoadReportKindData()
        {
            var list = EnumAttribute.GetDict<CostReportKind>().OrderBy(p => p.Key);
            rbl_ReportKind.DataSource = list;
            rbl_ReportKind.DataTextField = "Value";
            rbl_ReportKind.DataValueField = "Key";
            rbl_ReportKind.DataBind();
            rbl_ReportKind.SelectedValue = string.Format("{0}", (int)CostReportKind.Before);
        }

        //部门
        protected void LoadBranchData()
        {
            var branchList = CacheCollection.Branch.GetSystemBranchList();
            ddl_AssumeBranch.DataSource = branchList.Where(w => w.ParentID == Guid.Empty);
            ddl_AssumeBranch.DataTextField = "Name";
            ddl_AssumeBranch.DataValueField = "ID";
            ddl_AssumeBranch.DataBind();
            ddl_AssumeBranch.Items.Insert(0, new ListItem("请选择", ""));
        }

        /// <summary>
        /// 根据部门id获取“小组”数据
        /// </summary>
        /// <param name="branchId">部门id</param>
        protected void LoadGroupData(Guid branchId)
        {
            var groupList = CacheCollection.Branch.GetSystemBranchListByBranchId(branchId);
            ddl_AssumeGroup.DataSource = groupList;
            ddl_AssumeGroup.DataTextField = "Name";
            ddl_AssumeGroup.DataValueField = "ID";
            ddl_AssumeGroup.DataBind();
            ddl_AssumeGroup.Items.Insert(0, new ListItem("请选择", ""));
        }

        //门店
        protected void LoadShopData()
        {
            var shopList = FilialeManager.GetAllianceFilialeList().Where(act => act.Rank == (int)FilialeRank.Partial
                        && act.ShopJoinType == (int)ShopJoinType.DirectSales).OrderBy(act => act.Name);

            ddl_AssumeShop.DataSource = shopList;
            ddl_AssumeShop.DataTextField = "Name";
            ddl_AssumeShop.DataValueField = "ID";
            ddl_AssumeShop.DataBind();
            ddl_AssumeShop.Items.Insert(0, new ListItem("请选择", ""));
        }
        #endregion

        #region 初始化页面数据
        //初始化页面数据
        protected void LoadReportData(CostReportInfo model)
        {
            rbl_ReportKind.SelectedValue = model.ReportKind.ToString();
            ddl_AssumeBranch.SelectedValue = model.AssumeBranchId.Equals(Guid.Empty) ? string.Empty : model.AssumeBranchId.ToString();
            ddl_AssumeGroup.SelectedValue = model.AssumeGroupId.Equals(Guid.Empty) ? string.Empty : model.AssumeGroupId.ToString();
            ddl_AssumeShop.SelectedValue = model.AssumeShopId.Equals(Guid.Empty) ? string.Empty : model.AssumeShopId.ToString();
            txt_GoodsCode.Text = model.GoodsCode;
            if ((model.ReportKind.Equals((int)CostReportKind.Before) && model.IsLastTime) || model.ReportKind.Equals((int)CostReportKind.Later))
            {
                if (model.CostsVarieties.Equals(0))
                {
                    CompanyClass.Visible = ImgCompanyClass.Visible = true;
                }
                else if (model.CostsVarieties.Equals(1))
                {
                    GoodsCode.Visible = txtGoodsCode.Visible = true;
                }
            }
            txt_CompanyClass.Text = Cost.ReadInstance.GetCompanyName(model.CompanyClassId, model.CompanyId);
            rbl_UrgentOrDefer.SelectedValue = model.UrgentOrDefer.ToString();
            txt_UrgentReason.Text = model.UrgentReason;
            txt_ReportName.Text = model.ReportName + (model.ApplyNumber > 1 ? "  " + WebControl.ConvertToChnName(model.ApplyNumber) + "期" : string.Empty);
            if (model.ReportKind.Equals((int)CostReportKind.Before))
            {
                lit_ReportName.Text = "借款内容";
            }
            txt_StartTime.Text = model.StartTime.ToString("yyyy年MM月");
            txt_EndTime.Text = model.EndTime.ToString("yyyy年MM月");
            txt_PayCompany.Text = model.PayCompany;
            //“申报类型”是费用收入时，“收款人”变成“付款人”
            if (model.ReportKind.Equals((int)CostReportKind.FeeIncome))
            {
                lit_PayCompany.Text = "付款";
            }
            else
            {
                #region 预借款、凭证报销、已扣款核销 有“票据类型”
                BillType.Visible = true;
                txt_InvoiceTitle.ToolTip = txt_InvoiceTitle.Text = model.InvoiceTitle;
                rbl_InvoiceType.SelectedValue = string.Format("{0}", model.InvoiceType);
                if (model.InvoiceType.Equals((int)CostReportInvoiceType.Invoice) || model.InvoiceType.Equals((int)CostReportInvoiceType.VatInvoice))
                {
                    InvoiceType.InnerText = "发票";
                    InvoiceTitle.InnerHtml = "发票抬头：";

                    BillNo.InnerText = "发票号码";
                    BillCode.Visible = true;
                    TaxAmount.InnerText = "含税金额";

                    if (model.InvoiceType.Equals((int)CostReportInvoiceType.Invoice))
                    {
                        NoTaxAmount.Visible = Tax.Visible = false;
                    }
                    else if (model.InvoiceType.Equals((int)CostReportInvoiceType.VatInvoice))
                    {
                        NoTaxAmount.Visible = Tax.Visible = true;
                    }
                }
                else if (model.InvoiceType.Equals((int)CostReportInvoiceType.Voucher))
                {
                    InvoiceType.InnerText = "收据";
                    InvoiceTitle.InnerHtml = "收据抬头：";

                    BillNo.InnerText = "收据号码";
                    BillCode.Visible = false;
                    TaxAmount.InnerText = "金额";
                    NoTaxAmount.Visible = Tax.Visible = false;
                }
                #endregion
            }
            if (model.ReportKind.Equals((int)CostReportKind.Before))
            {
                lit_ReportCost.Text = "预估申报";
            }
            txt_ReportCost.Text = WebControl.RemoveDecimalEndZero(Math.Abs(model.ReportCost));
            Lit_CapitalAmount.Text = Math.Abs(model.ReportCost).ToString(CultureInfo.InvariantCulture);
            rbl_CostType.SelectedValue = model.CostType.ToString();
            if (model.CostType.Equals(2))//转账
            {
                if (!string.IsNullOrEmpty(model.BankAccountName))
                {
                    txt_BankName.Text = model.BankAccountName.Split(',')[0];
                    txt_SubBankName.Text = model.BankAccountName.Split(',')[1];
                }
                if (model.ReportKind.Equals((int)CostReportKind.Before) || model.ReportKind.Equals((int)CostReportKind.Later))
                {
                    txt_BankAccount.Text = model.BankAccount;
                    BankName.Visible = txtBankName.Visible = SubBankName.Visible = txtSubBankName.Visible = BankAccount.Visible = txtBankAccount.Visible = true;
                }
                else if (model.ReportKind.Equals((int)CostReportKind.Paying) || model.ReportKind.Equals((int)CostReportKind.FeeIncome))
                {
                    BankName.Visible = txtBankName.Visible = SubBankName.Visible = txtSubBankName.Visible = true;
                }
            }
            if (model.ReportKind.Equals((int)CostReportKind.Before) || model.ReportKind.Equals((int)CostReportKind.Later))
            {
                lit_BankName.Text = lit_SubBankName.Text = "收款";
            }
            else if (model.ReportKind.Equals((int)CostReportKind.Paying) || model.ReportKind.Equals((int)CostReportKind.FeeIncome))
            {
                lit_BankName.Text = lit_SubBankName.Text = "付款";

                PayBankAccountAndAssumeFiliale.Visible = true;
                if (!model.PayBankAccountId.Equals(Guid.Empty))
                {
                    var bankAccountInfo = _bankAccounts.GetBankAccounts(model.PayBankAccountId);
                    string payBankAccount = bankAccountInfo.BankName + "【" + bankAccountInfo.AccountsName + "】";
                    txt_PayBankAccount.ToolTip = txt_PayBankAccount.Text = payBankAccount;
                    txt_AssumeFiliale.Text = CacheCollection.Filiale.GetFilialeNameAndFilialeId(model.PayBankAccountId).Split(',')[0];
                }
            }

            //押金回收生成的“凭证报销”和“费用收入”有押金编号，此处要显示
            if (!string.IsNullOrEmpty(model.DepositNo))
            {
                DepositNo.Visible = txtDepositNo.Visible = true;
                txt_DepositNo.Text = model.DepositNo;
            }

            txt_ReportMemo.Text = model.ReportMemo;

            #region 广告使用图片(广告费=>推广费用)
            if (!model.InvoiceId.Equals(Guid.Empty))
            {
                var costReportInvoice = _costReportInvoice.GetInvoice(model.InvoiceId);
                if (costReportInvoice != null && !costReportInvoice.InvoiceId.Equals(Guid.Empty))
                {
                    PreA.HRef = costReportInvoice.ImagePath;
                    PreA.Visible = true;
                    UploadImgName.Text = costReportInvoice.ImagePath.Substring(costReportInvoice.ImagePath.LastIndexOf('/') + 1);
                }
            }
            #endregion

            #region 差旅费
            if (((model.ReportKind.Equals((int)CostReportKind.Before) && model.IsLastTime) || model.ReportKind == (int)CostReportKind.Later) && !model.CompanyId.Equals(Guid.Empty) && model.CompanyId.Equals(new Guid(CostReport_TravelExpenses)))
            {
                //获取差旅费
                CostReportTravelInfoList = _costReportTravel.GetlmShop_CostReportTravelByReportId(model.ReportId);
                if (CostReportTravelInfoList.Any())
                {
                    LoadTravelData(CostReportTravelInfoList);
                }
                //获取起讫
                CostReportTerminiInfoList = _costReportTermini.GetmShop_CostReportTerminiByReportId(model.ReportId);
                if (CostReportTerminiInfoList.Any())
                {
                    RepeaterTerminiDataBind();
                }
                txt_CompanyClass.Style["color"] = "red";
                txt_CompanyClass.Style["font-weight"] = "bold";
                MessageBox.AppendScript(this, "$(\"#TraveA\").show();$(\"#TraveDetail\").show();$(\"input[id$='Hid_Travel']\").val(\"0\");");
            }
            #endregion

            if (model.ReportKind.Equals((int)CostReportKind.Before))
            {
                if (!model.PayCost.Equals(0))
                {
                    IsLastTime.Visible = true;
                    rb_IsLastTime.SelectedValue = model.IsLastTime.ToString();
                    if (bool.Parse(rb_IsLastTime.SelectedValue))
                    {
                        #region 是否终结
                        rbIsEndTitle.Visible = rbIsEnd.Visible = true;
                        rb_IsEnd.SelectedValue = model.IsEnd.ToString();
                        #endregion
                    }
                }
                #region 申请金额
                //获取申请金额
                CostReportAmountInfoList = _costReportAmount.GetmShop_CostReportAmountByReportId(model.ReportId).Where(p => !p.IsSystem).ToList();
                if (CostReportAmountInfoList.Any())
                {
                    RepeaterAmountDataBind();
                    Amount.Visible = true;
                }
                #endregion
            }

            if (string.IsNullOrEmpty(model.DepositNo) && (model.ReportKind.Equals((int)CostReportKind.FeeIncome) || model.CompanyClassId.Equals(new Guid(CostReport_DepositMargins))))
            {
                Amount.Visible = false;
            }

            #region 票据类型相关
            //获取票据
            CostReportBillInfoList = _costReportBill.Getlmshop_CostReportBillByReportId(model.ReportId);
            if (CostReportBillInfoList.Any())
            {
                RepeaterBillDataBind();
                Bill.Visible = true;
            }
            #endregion

            #region 是否有权限更改“申报部门”
            var isEnabled = GetPowerOperationPoint("SetAssume");
            ddl_AssumeBranch.Enabled = isEnabled;
            ddl_AssumeGroup.Enabled = isEnabled;
            ddl_AssumeShop.Enabled = isEnabled;
            #endregion

            Hid_ReportPersonnelId.Value = model.ReportPersonnelId.ToString();
        }

        //加载差旅费
        protected void LoadTravelData(List<CostReportTravelInfo> costReportTravelInfoList)
        {
            txt_Entourage.Text = costReportTravelInfoList[0].Entourage;
            txt_TravelAddressAndCourse.Text = costReportTravelInfoList[0].TravelAddressAndCourse;

            foreach (var item in costReportTravelInfoList)
            {
                if (item.Matter == 0)
                {
                    txt_TrainChargeNum.Text = item.DayOrNum.ToString(CultureInfo.InvariantCulture);
                    txt_TrainChargeAmount.Text = item.Amount.ToString(CultureInfo.InvariantCulture);
                }
                else if (item.Matter == 1)
                {
                    txt_CarFeeNum.Text = item.DayOrNum.ToString(CultureInfo.InvariantCulture);
                    txt_CarFeeAmount.Text = item.Amount.ToString(CultureInfo.InvariantCulture);
                }
                else if (item.Matter == 2)
                {
                    txt_CityFeeNum.Text = item.DayOrNum.ToString(CultureInfo.InvariantCulture);
                    txt_CityFeeAmount.Text = item.Amount.ToString(CultureInfo.InvariantCulture);
                }
                else if (item.Matter == 3)
                {
                    txt_TollsNum.Text = item.DayOrNum.ToString(CultureInfo.InvariantCulture);
                    txt_TollsAmount.Text = item.Amount.ToString(CultureInfo.InvariantCulture);
                }
                else if (item.Matter == 4)
                {
                    txt_AircraftFeeNum.Text = item.DayOrNum.ToString(CultureInfo.InvariantCulture);
                    txt_AircraftFeeAmount.Text = item.Amount.ToString(CultureInfo.InvariantCulture);
                }
                else if (item.Matter == 5)
                {
                    txt_MealsDays.Text = item.DayOrNum.ToString(CultureInfo.InvariantCulture);
                    txt_MealsAmount.Text = item.Amount.ToString(CultureInfo.InvariantCulture);
                }
                else if (item.Matter == 6)
                {
                    txt_AccommodationDays.Text = item.DayOrNum.ToString(CultureInfo.InvariantCulture);
                    txt_AccommodationAmount.Text = item.Amount.ToString(CultureInfo.InvariantCulture);
                }
                else if (item.Matter == 7)
                {
                    txt_OtherDays.Text = item.DayOrNum.ToString(CultureInfo.InvariantCulture);
                    txt_OtherAmount.Text = item.Amount.ToString(CultureInfo.InvariantCulture);
                }
            }
        }
        #endregion

        #region SelectedIndexChanged事件
        //部门选择
        protected void ddl_AssumeBranch_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedValue = ddl_AssumeBranch.SelectedValue;
            if (!string.IsNullOrEmpty(selectedValue) && new Guid(selectedValue) != Guid.Empty)
            {
                LoadGroupData(new Guid(selectedValue));
                ddl_AssumeShop.SelectedValue = string.Empty;
                AssumeShop.Visible = false;
            }
        }

        //小组选择
        protected void ddl_AssumeGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            var assumeBranch = ddl_AssumeBranch.SelectedValue;
            if (assumeBranch.Equals(ShopBranchId))
            {
                var selectedValue = ddl_AssumeGroup.SelectedValue;
                if (!string.IsNullOrEmpty(selectedValue) && new Guid(selectedValue) != Guid.Empty)
                {
                    LoadShopData();
                    AssumeShop.Visible = true;
                }
                else
                {
                    ddl_AssumeShop.SelectedValue = string.Empty;
                    AssumeShop.Visible = false;
                }
            }
        }
        #endregion

        #region 数据列表相关
        #region 申请金额
        //申请金额数据源
        protected void RepeaterAmountDataBind()
        {
            Repeater_Amount.DataSource = CostReportAmountInfoList.OrderBy(p => p.Num);
            Repeater_Amount.DataBind();
            lit_SumAmount.Text = WebControl.RemoveDecimalEndZero(CostReportAmountInfoList.Sum(p => p.Amount)) + "元";
        }
        #endregion

        #region 票据(发票/收据)
        //票据数据源
        protected void RepeaterBillDataBind()
        {
            Repeater_Bill.DataSource = CostReportBillInfoList.OrderByDescending(p => p.OperatingTime).ThenByDescending(p => p.IsPay);
            Repeater_Bill.DataBind();
            lit_SumNoTaxAmount.Text = WebControl.RemoveDecimalEndZero(CostReportBillInfoList.Sum(p => p.NoTaxAmount)) + "元";
            lit_SumTax.Text = WebControl.RemoveDecimalEndZero(CostReportBillInfoList.Sum(p => p.Tax)) + "元";
            lit_SumTaxAmount.Text = WebControl.RemoveDecimalEndZero(CostReportBillInfoList.Sum(p => p.TaxAmount)) + "元";
        }

        #region 列表操作
        //行绑定事件
        protected void Repeater_Bill_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var txtBillCode = e.Item.FindControl("txtBillCode");
                var txtNoTaxAmount = e.Item.FindControl("txtNoTaxAmount");
                var txtTax = e.Item.FindControl("txtTax");

                if (int.Parse(rbl_InvoiceType.SelectedValue).Equals((int)CostReportInvoiceType.Invoice))
                {
                    txtBillCode.Visible = true;
                    txtNoTaxAmount.Visible = txtTax.Visible = litSumNoTaxAmount.Visible = litSumTax.Visible = false;
                    SumTitle.ColSpan = 4;
                }
                else if (int.Parse(rbl_InvoiceType.SelectedValue).Equals((int)CostReportInvoiceType.VatInvoice))
                {
                    txtBillCode.Visible = true;
                    txtNoTaxAmount.Visible = txtTax.Visible = litSumNoTaxAmount.Visible = litSumTax.Visible = true;
                    SumTitle.ColSpan = 4;
                }
                else if (int.Parse(rbl_InvoiceType.SelectedValue).Equals((int)CostReportInvoiceType.Voucher))
                {
                    txtBillCode.Visible = txtNoTaxAmount.Visible = txtTax.Visible = litSumNoTaxAmount.Visible = litSumTax.Visible = false;
                    SumTitle.ColSpan = 3;
                }
            }
        }
        #endregion
        #endregion

        #region 差旅费起讫
        //起讫数据源
        protected void RepeaterTerminiDataBind()
        {
            Repeater_Termini.DataSource = CostReportTerminiInfoList;
            Repeater_Termini.DataBind();
        }
        #endregion

        #region 当前审批人审批的 与当前单据申请人或者承担部门相同的三个月内的数据
        //列表条件：审批人=当前登录人；状态！=作废；申报时间三个月内；与当前单据申请人或者承担部门相同的
        protected void RG_Report_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            GridDataBind();
        }

        //Grid数据源
        protected void GridDataBind()
        {
            IList<CostReportInfo> list;
            var reportId = Request.QueryString["ReportId"];
            if (string.IsNullOrEmpty(reportId))
            {
                list = new List<CostReportInfo>();
            }
            else
            {
                DateTime startTime = DateTime.Now.AddDays(-30);
                DateTime endTime = DateTime.Now;
                CostReportInfo reportInfo = _costReport.GetReportByReportId(new Guid(reportId));
                list = _costReport.GetReportList();
                if (list.Count > 0)
                {
                    list = list.Where(p => (p.ReportPersonnelId == reportInfo.ReportPersonnelId || p.AssumeBranchId == reportInfo.AssumeBranchId) && p.ReportDate >= startTime && p.ReportDate <= endTime && p.State != (int)CostReportState.Auditing && p.ReportId != reportInfo.ReportId && p.AuditingMan == Personnel.PersonnelId).ToList();
                }
            }
            RG_Report.DataSource = list.OrderByDescending(c => c.ReportDate);
        }

        //行绑定事件
        protected void RG_Report_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item.ItemType == GridItemType.Item || e.Item.ItemType == GridItemType.AlternatingItem)
            {
                var reportInfo = (CostReportInfo)e.Item.DataItem;
                #region “申报金额”>=1万元(绿色);“申报金额”>=10万元(黄色);“申报金额”>=100万元(紫色)
                if (reportInfo.ReportCost >= 1000000)
                {
                    e.Item.Style["color"] = "#CC00FF";
                }
                else if (reportInfo.ReportCost >= 100000)
                {
                    e.Item.Style["color"] = "#FF9900";
                }
                else if (reportInfo.ReportCost >= 10000)
                {
                    e.Item.Style["color"] = "#009900";
                }
                #endregion
            }
        }
        #endregion
        #endregion

        //审核通过
        protected void btn_Pass_Click(object sender, EventArgs e)
        {
            #region 验证数据
            var errorMsg = CheckData();
            if (!string.IsNullOrEmpty(errorMsg))
            {
                MessageBox.Show(this, errorMsg);
                return;
            }
            #endregion

            CostReportInfo model = _costReport.GetReportByReportId(new Guid(Request.QueryString["ReportId"]));
            if (model.State != (int)CostReportState.Auditing)
            {
                MessageBox.Show(this, "该单据状态已更新，不允许此操作！");
                return;
            }

            bool execute = false;
            if (model.ReportKind == (int)CostReportKind.Before)
            {
                //预借款
                BeforeLoan(model);
            }
            else if (model.ReportKind == (int)CostReportKind.Paying)
            {
                execute = true;
                //已扣款核销
                PayVerification(model);
            }
            else if (model.ReportKind == (int)CostReportKind.Later)
            {
                //凭证报销
                VoucherPay(model);
            }
            else if (model.ReportKind == (int)CostReportKind.FeeIncome)
            {
                //费用收入
                FeeIncome(model);
            }

            model.AssumeBranchId = string.IsNullOrEmpty(ddl_AssumeBranch.SelectedValue) ? Guid.Empty : new Guid(ddl_AssumeBranch.SelectedValue);
            model.AssumeGroupId = string.IsNullOrEmpty(ddl_AssumeGroup.SelectedValue) ? Guid.Empty : new Guid(ddl_AssumeGroup.SelectedValue);
            model.AssumeShopId = string.IsNullOrEmpty(ddl_AssumeShop.SelectedValue) ? Guid.Empty : new Guid(ddl_AssumeShop.SelectedValue);
            model.AuditingMan = Personnel.PersonnelId;
            model.AuditingDate = DateTime.Now;
            model.AuditingMemo = txt_AuditingMemo.Text;

            #region 保存数据
            using (var ts = new TransactionScope(TransactionScopeOption.Required))
            {
                try
                {
                    _costReport.UpdateReport(model);

                    if (execute)
                    {
                        _costReport.UpdatePayCostAndExecuteDate(model.ReportId, model.RealityCost);
                        string errorMessage;
                        bool result = ExecuteFinishHandle(model, Personnel, out errorMessage);
                        if (!result)
                        {
                            throw new Exception(errorMessage);
                        }
                    }

                    //添加操作日志
                    _operationLogManager.Add(Personnel.PersonnelId, Personnel.RealName, model.ReportId, model.ReportNo, OperationPoint.CostDeclare.AuditDeclare.GetBusinessInfo(), 1, "");

                    ts.Complete();
                    MessageBox.AppendScript(this, "setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
                }
                catch
                {
                    MessageBox.Show(this, "保存失败！");
                }
                finally
                {
                    ts.Dispose();
                }
            }
            #endregion
        }

        //审核不通过
        protected void btn_NoPass_Click(object sender, EventArgs e)
        {
            #region 验证数据
            var errorMsg = CheckData();
            if (string.IsNullOrEmpty(txt_AuditingMemo.Text))
            {
                errorMsg += "请填写“审核说明”！";
            }
            if (!string.IsNullOrEmpty(errorMsg))
            {
                MessageBox.Show(this, errorMsg);
                return;
            }
            #endregion

            CostReportInfo model = _costReport.GetReportByReportId(new Guid(Request.QueryString["ReportId"]));
            if (model.State != (int)CostReportState.Auditing)
            {
                MessageBox.Show(this, "该单据状态已更新，不允许此操作！");
                return;
            }

            var state = (int)CostReportState.AuditingNoPass;
            var memo = WebControl.RetrunUserAndTime("[【审核】:审核不通过;审核说明:" + (string.IsNullOrEmpty(txt_AuditingMemo.Text) ? "暂无说明" : txt_AuditingMemo.Text) + ";]");

            #region 保存数据
            using (var ts = new TransactionScope(TransactionScopeOption.Required))
            {
                try
                {
                    _costReportBill.Updatelmshop_CostReportBillForPassByReportId(model.ReportId, false);
                    _costReport.UpdateReport(model.ReportId, state, txt_AuditingMemo.Text, memo, Personnel.PersonnelId);
                    //添加操作日志
                    _operationLogManager.Add(Personnel.PersonnelId, Personnel.RealName, model.ReportId, model.ReportNo, OperationPoint.CostDeclare.AuditDeclare.GetBusinessInfo(), 1, "");

                    ts.Complete();
                    MessageBox.AppendScript(this, "setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
                }
                catch
                {
                    MessageBox.Show(this, "保存失败！");
                }
            }
            #endregion
        }

        #region 预借款
        protected void BeforeLoan(CostReportInfo model)
        {
            if (model.IsSystem)
            {
                model.State = (int)CostReportState.AlreadyAuditing;//状态：待付款
            }
            else
            {
                //查询没有付款的票据
                var isPayList = CostReportBillInfoList.Where(p => !p.IsPay);
                if (isPayList.Any())//有没有付款的票据
                {
                    model.State = (int)CostReportState.NoAuditing;//状态：票据待受理
                }
                else
                {
                    if (model.IsLastTime)
                    {
                        if (CostReportTravelInfoList.Any() || CostReportTerminiInfoList.Any() || !string.IsNullOrEmpty(model.GoodsCode) || !model.InvoiceId.Equals(Guid.Empty))
                        {
                            model.State = (int)CostReportState.NoAuditing;//状态：票据待受理
                        }
                        else
                        {
                            model.State = (int)CostReportState.AlreadyAuditing;//状态：待付款
                        }
                    }
                    else
                    {
                        model.State = (int)CostReportState.AlreadyAuditing;//状态：待付款
                    }
                }
            }
            model.Memo = WebControl.RetrunUserAndTime("[【审核】:审核通过;审核说明:" + (string.IsNullOrEmpty(txt_AuditingMemo.Text) ? "暂无说明" : txt_AuditingMemo.Text) + ";]");
        }
        #endregion

        #region 已扣款核销
        protected void PayVerification(CostReportInfo model)
        {
            model.State = (int)CostReportState.NoAuditing;//状态：票据待受理

            #region 结算账号
            IBankAccounts bankAccounts = new BankAccounts(GlobalConfig.DB.FromType.Read);
            var bankAccountInfo = bankAccounts.GetBankAccounts(model.PayBankAccountId);
            string payBankAccount = bankAccountInfo == null ? "暂无结算" : (bankAccountInfo.BankName + "【" + bankAccountInfo.AccountsName + "】");
            #endregion

            model.Memo = WebControl.RetrunUserAndTime("[【审核】:审核通过;审核说明:" + (string.IsNullOrEmpty(txt_AuditingMemo.Text) ? "暂无说明" : txt_AuditingMemo.Text) + ";]") + WebControl.RetrunUserAndTime("[【已付款】:已支付" + WebControl.RemoveDecimalEndZero(model.RealityCost) + "元;结算账号:" + payBankAccount + ";]");
        }
        #endregion

        #region 凭证报销
        protected void VoucherPay(CostReportInfo model)
        {
            model.State = (int)CostReportState.NoAuditing;//状态：票据待受理
            model.Memo = WebControl.RetrunUserAndTime("[【审核】:审核通过;审核说明:" + (string.IsNullOrEmpty(txt_AuditingMemo.Text) ? "暂无说明" : txt_AuditingMemo.Text) + ";]");
        }
        #endregion

        #region 费用收入
        protected void FeeIncome(CostReportInfo model)
        {
            model.State = (int)CostReportState.WaitVerify;
            model.Memo = WebControl.RetrunUserAndTime("[【审核】:审核通过;审核说明:" + (string.IsNullOrEmpty(txt_AuditingMemo.Text) ? "暂无说明" : txt_AuditingMemo.Text) + ";]");
        }
        #endregion

        #region 执行完成时的操作
        protected bool ExecuteFinishHandle(CostReportInfo model, PersonnelInfo personnelInfo, out string errorMsg)
        {
            errorMsg = string.Empty;
            if (model.ReportKind.Equals((int)CostReportKind.FeeIncome))
            {
                model.RealityCost = -model.RealityCost;
            }

            var costReportBll = new BLL.Implement.Inventory.CostReport(_bankAccounts, _personnelSao, _costReckoning);
            #region 新增资金流
            var wasteBookInfo = costReportBll.AddWasteBookInfo(model, personnelInfo, false);
            if (_wasteBook.Insert(wasteBookInfo) <= 0)
            {
                errorMsg = "新增资金流失败！";
                return false;
            }
            #endregion

            #region 新增帐务记录
            var costReckoningInfo = costReportBll.AddCostReckoningInfo(model, personnelInfo, false);
            if (_costReckoning.Insert(costReckoningInfo) <= 0)
            {
                errorMsg = "新增帐务记录失败！";
                return false;
            }
            #endregion

            #region 与门店费用交互
            if (model.AssumeBranchId == new Guid(ShopBranchId) && !model.AssumeShopId.Equals(Guid.Empty))
            {
                string strErrorMsg;
                var description = string.Format("费用申报{0}{1}可用余额!", model.ReceiptNo, model.RealityCost < 0 ? "添加" : "扣除");
                var parentId = FilialeManager.GetShopHeadFilialeId(model.AssumeShopId);//获取门店所属公司
                if (!ShopSao.DeductBalance(parentId, model.AssumeShopId, model.RealityCost, description, out strErrorMsg))
                {
                    errorMsg = strErrorMsg;
                    return false;
                }

                var costRecordDto = costReportBll.AddCostRecordDto(model);
                if (!ShopSao.InsertCostRecord(parentId, costRecordDto))
                {
                    errorMsg = "扣除门店费用失败！";
                    return false;
                }
            }
            #endregion

            #region 新增资金流(手续费)
            if (model.RealityCost >= 0 && model.Poundage > 0 && !model.ReportKind.Equals((int)CostReportKind.FeeIncome))
            {
                var wasteBookInfoPoundage = costReportBll.AddWasteBookInfo(model, personnelInfo, true);
                if (_wasteBook.Insert(wasteBookInfoPoundage) <= 0)
                {
                    errorMsg = "新增手续费失败！";
                    return false;
                }
            }
            #endregion

            return true;
        }
        #endregion

        //验证数据
        protected string CheckData()
        {
            var errorMsg = new StringBuilder();

            #region 公共数据
            var assumeBranchId = ddl_AssumeBranch.SelectedValue;
            if (string.IsNullOrEmpty(assumeBranchId))
            {
                errorMsg.Append("请选择“申报部门”！").Append("\\n");
            }
            #endregion

            #region 验证是否存在票据信息(用于判断押金回收的“凭证报销”是否信息完整)
            var reportKind = rbl_ReportKind.SelectedValue;
            if (int.Parse(reportKind).Equals((int)CostReportKind.Later))
            {
                var costReportBillList = _costReportBill.Getlmshop_CostReportBillByReportId(new Guid(Request.QueryString["ReportId"]));
                if (!costReportBillList.Any())
                {
                    var reportPersonnelName = new PersonnelManager().GetName(new Guid(Hid_ReportPersonnelId.Value));
                    errorMsg.Append("请提醒申报人“" + reportPersonnelName + "”完善相关信息！").Append("\\n");
                }
            }
            #endregion

            return errorMsg.ToString();
        }

        /// <summary>
        /// 取得用户操作权限
        /// </summary>
        protected bool GetPowerOperationPoint(string powerName)
        {
            const string PAGE_NAME = "/CostReport/CostReport_AuditManage.aspx";
            return WebControl.GetPowerOperationPoint(PAGE_NAME, powerName);
        }
    }
}