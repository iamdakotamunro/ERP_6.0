using System;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Web.UI.WebControls;
using ERP.BLL.Implement.Organization;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Enum.Attribute;
using ERP.Environment;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Keede.Ecsoft.Model;
using MIS.Enum;
using Cost = ERP.BLL.Implement.Inventory.Cost;
using WebControl = ERP.UI.Web.Common.WebControl;
using ERP.Model;
using System.Collections.Generic;
using ERP.DAL.Interface.IUtilities;
using ERP.DAL.Factory;
using ERP.BLL.Implement;

namespace ERP.UI.Web.CostReport
{
    /************************************************************************************ 
     * 创建人：  张安龙
     * 创建时间：2015/11/11  
     * 描述    :费用申报查看
     * =====================================================================
     * 修改时间：2015/11/11  
     * 修改人  ：  
     * 描述    ：
     */
    public partial class CostReport_Look : WindowsPage
    {
        private static readonly ICostReport _costReport = new DAL.Implement.Inventory.CostReport(GlobalConfig.DB.FromType.Write);
        private static readonly ICostReportInvoice _costReportInvoice = new CostReportInvoice(GlobalConfig.DB.FromType.Read);
        private static readonly IBankAccounts _bankAccounts = new BankAccounts(GlobalConfig.DB.FromType.Read);
        private static readonly ICostReportTravel _costReportTravel = new CostReportTravelDal(GlobalConfig.DB.FromType.Write);
        private static readonly ICostReportTermini _costReportTermini = new CostReportTerminiDal(GlobalConfig.DB.FromType.Write);
        private static readonly ICostReportBill _costReportBill = new CostReportBillDal(GlobalConfig.DB.FromType.Write);
        private static readonly ICostReportAmount _costReportAmount = new CostReportAmountDal(GlobalConfig.DB.FromType.Write);
        private static readonly IUtility _utility = InventoryInstance.GetUtilityDalDao(GlobalConfig.DB.FromType.Write);

        #region 属性
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
        /// 费用申报=>费用分类(办公费=>设施、设备、软件)
        /// </summary>
        private string CostReport_FacilityEquipmentSoftware
        {
            get
            {
                if (ViewState["CostReport_FacilityEquipmentSoftware"] == null)
                {
                    ViewState["CostReport_FacilityEquipmentSoftware"] = ConfigManage.GetConfigValue("CostReport_FacilityEquipmentSoftware");
                }
                return ViewState["CostReport_FacilityEquipmentSoftware"].ToString();
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

                //只有CostReport_SearchManage.aspx页面状态是“完成”并且“付款金额”>=1000的数据才有“审阅”按钮
                if ((Request.QueryString["Review"] != null && Request.QueryString["Review"] == "1") && costReportInfo.State.Equals((int)CostReportState.Complete) && costReportInfo.PayCost >= 1000)
                {
                    //判断登录用户是否有此操作的权限
                    if (GetPowerOperationPoint("Review") && costReportInfo.ReviewState.Equals(0))
                    {
                        btn_Review.Visible = true;
                    }
                }
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

        //初始化页面数据
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
                if (model.Deposit.Equals(2))//有押金的预借款查看时不需要显示“人事部物品管理编码”和“广告使用图片”
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
            //已扣款核销不走收付款，故没有“手续费”和“交易流水号”
            if (model.ReportKind.Equals((int)CostReportKind.Before) || (string.IsNullOrEmpty(model.DepositNo) && model.ReportKind.Equals((int)CostReportKind.Later)) || model.ReportKind.Equals((int)CostReportKind.FeeIncome))
            {
                PoundageAndTradeNo.Visible = true;
                txt_Poundage.Text = model.Poundage.ToString(CultureInfo.InvariantCulture);
                txt_TradeNo.Text = model.TradeNo;
            }

            if (model.ReportKind.Equals((int)CostReportKind.Before) || model.ReportKind.Equals((int)CostReportKind.Later))
            {
                lit_BankName.Text = lit_SubBankName.Text = "收款";
            }
            else if (model.ReportKind.Equals((int)CostReportKind.Paying) || model.ReportKind.Equals((int)CostReportKind.FeeIncome))
            {
                lit_BankName.Text = lit_SubBankName.Text = "付款";
            }
            if (!model.PayBankAccountId.Equals(Guid.Empty))
            {
                var bankAccountInfo = _bankAccounts.GetBankAccounts(model.PayBankAccountId);
                string payBankAccount = bankAccountInfo.BankName + "【" + bankAccountInfo.AccountsName + "】";
                txt_PayBankAccount.ToolTip = txt_PayBankAccount.Text = payBankAccount;
                txt_AssumeFiliale.Text = CacheCollection.Filiale.GetFilialeNameAndFilialeId(model.PayBankAccountId).Split(',')[0];
                PayBankAccountAndAssumeFiliale.Visible = true;
            }

            //押金回收生成的“凭证报销”和“费用收入”有押金编号，此处要显示
            if (!string.IsNullOrEmpty(model.DepositNo))
            {
                DepositNo.Visible = txtDepositNo.Visible = true;
                txt_DepositNo.Text = model.DepositNo;
            }

            txt_ReportMemo.Text = model.ReportMemo;

            txt_InvoiceTitle.ToolTip = txt_InvoiceTitle.Text = model.InvoiceTitle;
            txt_ReportMemo.Text = model.ReportMemo;
            if (!string.IsNullOrEmpty(model.AuditingMemo))
            {
                AuditingMemo.Visible = true;
                lit_AuditingMemo.Text = model.AuditingMemo;
            }
            txt_Memo.Text = model.Memo;

            #region 广告使用图片(广告费/购买物品)
            if (!model.InvoiceId.Equals(Guid.Empty))
            {
                var costReportInvoice = _costReportInvoice.GetInvoice(model.InvoiceId);
                if (costReportInvoice != null && !costReportInvoice.InvoiceId.Equals(Guid.Empty))
                {
                    if (model.InvoiceType.Equals((int)CostReportInvoiceType.NoVoucher))
                    {
                        rbl_NoVoucher.Visible = PreANoVoucher.Visible = true;
                        rbl_NoVoucher.Checked = true;
                        PreANoVoucher.HRef = costReportInvoice.ImagePath;
                    }
                    else
                    {
                        PreA.HRef = costReportInvoice.ImagePath;
                        PreA.Visible = true;
                        UploadImgName.Text = costReportInvoice.ImagePath.Substring(costReportInvoice.ImagePath.LastIndexOf('/') + 1);
                    }
                }
            }
            #endregion

            #region 差旅费
            if (((model.ReportKind.Equals((int)CostReportKind.Before) && model.IsLastTime) || model.ReportKind == (int)CostReportKind.Later) && !model.CompanyId.Equals(Guid.Empty) && model.CompanyId.Equals(new Guid(CostReport_TravelExpenses)))
            {
                //获取差旅费
                var costReportTravelList = _costReportTravel.GetlmShop_CostReportTravelByReportId(model.ReportId);
                if (costReportTravelList.Any())
                {
                    LoadTravelData(costReportTravelList);
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
        #endregion

        //审阅
        protected void btn_Review_Click(object sender, EventArgs e)
        {
            var result = _utility.UpdateFieldByPk("lmShop_CostReport", "ReviewState", new object[] { 1 }, "ReportId", Request.QueryString["ReportId"]);
            if (result > 0)
            {
                MessageBox.AppendScript(this, "setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
            }
            else
            {
                MessageBox.Show(this, "审阅失败！");
            }
        }

        /// <summary>
        /// 取得用户操作权限
        /// </summary>
        protected bool GetPowerOperationPoint(string powerName)
        {
            const string PAGE_NAME = "/CostReport/CostReport_SearchManage.aspx";
            return WebControl.GetPowerOperationPoint(PAGE_NAME, powerName);
        }
    }
}