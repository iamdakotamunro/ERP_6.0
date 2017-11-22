using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Transactions;
using System.Web.UI.WebControls;
using ERP.BLL.Implement;
using ERP.BLL.Implement.Inventory;
using ERP.BLL.Implement.Organization;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Environment;
using ERP.Model;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Keede.Ecsoft.Model;
using MIS.Enum;
using OperationLog.Core;
using Cost = ERP.DAL.Implement.Inventory.Cost;
using WebControl = ERP.UI.Web.Common.WebControl;
using Framework.Data;

namespace ERP.UI.Web.CostReport
{
    public partial class CostReport_Later : WindowsPage
    {
        private static readonly OperationLogManager _operationLogManager = new OperationLogManager();
        private static readonly ICost _costDao = new Cost(GlobalConfig.DB.FromType.Read);
        private static readonly ICostCussent _costCussentDao = new CostCussent(GlobalConfig.DB.FromType.Read);
        private static readonly ICostReport _costReport = new DAL.Implement.Inventory.CostReport(GlobalConfig.DB.FromType.Write);
        private static readonly ICostReportInvoice _costReportInvoice = new CostReportInvoice(GlobalConfig.DB.FromType.Write);
        private static readonly ICostReportTravel _costReportTravel = new CostReportTravelDal(GlobalConfig.DB.FromType.Write);
        private static readonly ICostReportTermini _costReportTermini = new CostReportTerminiDal(GlobalConfig.DB.FromType.Write);
        private static readonly ICostReportBill _costReportBill = new CostReportBillDal(GlobalConfig.DB.FromType.Write);
        readonly ICostReportAuditingPower _costReportAuditingPower = new DAL.Implement.Inventory.CostReportAuditingPower(GlobalConfig.DB.FromType.Write);

        readonly List<CostReportTravelInfo> _costReportTravelInfoList = new List<CostReportTravelInfo>();

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
        /// ERP公司
        /// </summary>
        private string ErpFiliale
        {
            get
            {
                if (ViewState["ERPFiliale"] == null)
                {
                    ViewState["ERPFiliale"] = ConfigManage.GetConfigValue("RECKONING_ELSE_FILIALEID").ToLower();
                }
                return ViewState["ERPFiliale"].ToString();
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
        /// 费用申报=>费用分类(广告费=>推广费用)
        /// </summary>
        private string CostReport_PromotionExpenses
        {
            get
            {
                if (ViewState["CostReport_PromotionExpenses"] == null)
                {
                    ViewState["CostReport_PromotionExpenses"] = ConfigManage.GetConfigValue("CostReport_PromotionExpenses");
                }
                return ViewState["CostReport_PromotionExpenses"].ToString();
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
        #endregion

        #region 60s内禁止重复提交
        private SubmitController _submitController;
        /// <summary>
        /// 60s内禁止重复提交
        /// </summary>
        protected void CreateSubmitController()
        {
            if (ViewState["SubmitControllerSave"] == null)
            {
                _submitController = new SubmitController();
                ViewState["SubmitControllerSave"] = _submitController;
            }
            else
            {
                _submitController = (SubmitController)ViewState["SubmitControllerSave"];
            }
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            CreateSubmitController();
            if (!IsPostBack)
            {
                var costReportAuditingInfo = _costReportAuditingPower.GetPowerList().FirstOrDefault(p => p.Kind == (int)CostReportAuditingType.Invoice);
                if (costReportAuditingInfo != null)
                {
                    lit_InvoiceAcceptPersonInfo.Text = string.IsNullOrEmpty(costReportAuditingInfo.Description) ? "财务部" : costReportAuditingInfo.Description;
                }
                var reportId = Request.QueryString["ReportId"];
                if (string.IsNullOrEmpty(reportId))
                {
                    LoadAddData();//加载添加数据
                }
                else
                {
                    LoadEditData(reportId);//加载编辑数据
                }
            }
        }

        #region 数据准备
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

        //费用分类
        protected void LoadCostSortData()
        {
            var branchInfo = CacheCollection.Branch.Get(Personnel.FilialeId, Personnel.BranchId);
            IList<CostCompanyClassInfo> list = _costDao.GetPermissionCompanyClassList(Personnel.FilialeId, branchInfo.FilialeBranchInfo.ParentBranchId == Guid.Empty ? Personnel.BranchId : branchInfo.FilialeBranchInfo.ParentBranchId);
            ddl_CompanyClass.DataSource = list;
            ddl_CompanyClass.DataTextField = "CompanyClassName";
            ddl_CompanyClass.DataValueField = "CompanyClassId";
            ddl_CompanyClass.DataBind();
            ddl_CompanyClass.Items.Insert(0, new ListItem("请选择", ""));
        }

        /// <summary>
        /// 根据费用分类id获取“费用类型”数据
        /// </summary>
        /// <param name="companyClassId">费用分类id</param>
        protected void LoadFeeTypeData(Guid companyClassId)
        {
            var branchInfo = CacheCollection.Branch.Get(Personnel.FilialeId, Personnel.BranchId);
            var list = _costCussentDao.GetPermissionCompanyCussentList(Personnel.FilialeId, branchInfo.FilialeBranchInfo.ParentBranchId == Guid.Empty ? Personnel.BranchId : branchInfo.FilialeBranchInfo.ParentBranchId, companyClassId);
            ddl_FeeType.DataSource = list;
            ddl_FeeType.DataTextField = "CompanyName";
            ddl_FeeType.DataValueField = "CompanyId";
            ddl_FeeType.DataBind();
            ddl_FeeType.Items.Insert(0, new ListItem("请选择", ""));
        }

        //发票抬头
        protected void LoadInvoiceTitleData()
        {
            var list = CacheCollection.Filiale.GetHeadList();
            ddl_InvoiceTitle.DataSource = list;
            ddl_InvoiceTitle.DataTextField = "RealName";
            ddl_InvoiceTitle.DataValueField = "ID";
            ddl_InvoiceTitle.DataBind();
            ddl_InvoiceTitle.Items.Insert(0, new ListItem("请选择", ""));
        }

        //设置个人所属部门
        protected void SetAssumeBranchData()
        {
            var systemPositionInfo = CacheCollection.Position.GetPostionBySystemBrachPositionId(Personnel.SystemBrandPositionId);
            if (systemPositionInfo.ParentSystemBranchID.Equals(Guid.Empty))
            {
                ddl_AssumeBranch.SelectedValue = systemPositionInfo.SystemBranchID.ToString();
            }
            else
            {
                ddl_AssumeBranch.SelectedValue = systemPositionInfo.ParentSystemBranchID.ToString();
                if (!string.IsNullOrEmpty(systemPositionInfo.ParentSystemBranchID.ToString()) && systemPositionInfo.ParentSystemBranchID != Guid.Empty)
                {
                    LoadGroupData(systemPositionInfo.ParentSystemBranchID);
                    ddl_AssumeGroup.SelectedValue = systemPositionInfo.SystemBranchID.ToString();
                    if (!systemPositionInfo.SystemBranchID.Equals(Guid.Empty))
                    {
                        if (systemPositionInfo.ParentSystemBranchID.Equals(new Guid(ShopBranchId)))
                        {
                            LoadShopData();//加载门店数据
                            AssumeShop.Visible = true;
                        }
                    }
                }
            }
        }
        #endregion

        #region 添加准备
        //加载添加数据
        protected void LoadAddData()
        {
            LoadBranchData();//加载部门数据
            LoadCostSortData();//加载费用分类
            SetAssumeBranchData();//设置个人所属部门
            LoadInvoiceTitleData();//发票抬头
            //初始化日期
            txt_EndTime.Text = txt_StartTime.Text = DateTime.Now.ToString("yyyy年MM月");

            AddNewBill();//增加票据
        }
        #endregion

        #region 修改准备
        //加载编辑数据
        protected void LoadEditData(string reportId)
        {
            CostReportInfo costReportInfo = _costReport.GetReportByReportId(new Guid(reportId));

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
            LoadCostSortData();//加载费用分类
            if (!costReportInfo.CompanyClassId.Equals(Guid.Empty))
            {
                LoadFeeTypeData(costReportInfo.CompanyClassId); //根据费用分类id获取“费用类型”数据
            }
            LoadInvoiceTitleData();//加载发票抬头
            LoadReportData(costReportInfo);//初始化页面数据
        }

        //初始化页面数据
        protected void LoadReportData(CostReportInfo model)
        {
            ddl_AssumeBranch.SelectedValue = model.AssumeBranchId.Equals(Guid.Empty) ? string.Empty : model.AssumeBranchId.ToString();
            ddl_AssumeGroup.SelectedValue = model.AssumeGroupId.Equals(Guid.Empty) ? string.Empty : model.AssumeGroupId.ToString();
            ddl_AssumeShop.SelectedValue = model.AssumeShopId.Equals(Guid.Empty) ? string.Empty : model.AssumeShopId.ToString();
            rbl_UrgentOrDefer.SelectedValue = string.Format("{0}", model.UrgentOrDefer);
            txt_UrgentReason.Text = model.UrgentReason;
            Hid_CostsClass.Value = string.Format("{0}", model.CostsVarieties);
            txt_GoodsCode.Text = model.GoodsCode;
            ddl_CompanyClass.SelectedValue = model.CompanyClassId.Equals(Guid.Empty) ? string.Empty : model.CompanyClassId.ToString();
            ddl_FeeType.SelectedValue = model.CompanyId.Equals(Guid.Empty) ? string.Empty : model.CompanyId.ToString();
            txt_ReportName.Text = model.ReportName;
            txt_StartTime.Text = model.StartTime.ToString("yyyy年MM月");
            txt_EndTime.Text = model.EndTime.ToString("yyyy年MM月");
            txt_PayCompany.Text = model.PayCompany;
            txt_ReportCost.Text = model.ReportCost.ToString(CultureInfo.InvariantCulture);
            ReportCostChange();
            rbl_CostType.SelectedValue = string.Format("{0}", model.CostType);
            if (model.CostType.Equals(2))
            {
                if (!string.IsNullOrEmpty(model.BankAccountName))
                {
                    txt_BankName.Text = model.BankAccountName.Split(',')[0];
                    txt_SubBankName.Text = model.BankAccountName.Split(',')[1];
                }
                txt_BankAccount.Text = model.BankAccount;
            }
            //押金回收生成的“凭证报销”和“费用收入”有押金编号，此处要显示
            if (!string.IsNullOrEmpty(model.DepositNo))
            {
                DepositNo.Visible = txtDepositNo.Visible = true;
                txt_DepositNo.Text = model.DepositNo;
                txt_ReportCost.ReadOnly = true;
            }
            rbl_InvoiceType.SelectedValue = string.Format("{0}", model.InvoiceType);
            
            txt_ReportMemo.Text = model.ReportMemo;
            if (!string.IsNullOrEmpty(model.AuditingMemo))
            {
                AuditingMemo.Visible = true;
                lit_AuditingMemo.Text = model.AuditingMemo;
            }
            txt_Memo.Text = model.Memo;

            ReportProcess.Visible = true;//修改时显示“申报流程”

            #region 广告使用图片(广告费/购买物品)
            if (!model.InvoiceId.Equals(Guid.Empty))
            {
                var costReportInvoice = _costReportInvoice.GetInvoice(model.InvoiceId);
                if (costReportInvoice != null && !costReportInvoice.InvoiceId.Equals(Guid.Empty))
                {
                    PreA.HRef = costReportInvoice.ImagePath;
                    UploadImgName.Text = costReportInvoice.ImagePath.Substring(costReportInvoice.ImagePath.LastIndexOf('/') + 1);
                }
            }
            #endregion

            #region 差旅费
            if (!model.CompanyId.Equals(Guid.Empty) && model.CompanyId.Equals(new Guid(CostReport_TravelExpenses)))
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
            }
            #endregion

            #region 票据类型相关
            //获取票据
            CostReportBillInfoList = _costReportBill.Getlmshop_CostReportBillByReportId(model.ReportId);
            if (CostReportBillInfoList.Any())
            {
                RepeaterBillDataBind();
            }
            else
            {
                AddNewBill();//增加票据
            }
            #endregion

            #region Change方法处理
            FeeTypeChange();
            InvoiceTypeChange();
            #endregion

            ddl_InvoiceTitle.SelectedValue = model.InvoiceTitleFilialeId.Equals(Guid.Empty) ? string.Empty : model.InvoiceTitleFilialeId.ToString();
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

        //费用分类选择
        protected void ddl_CompanyClass_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedValue = ddl_CompanyClass.SelectedValue;
            if (!string.IsNullOrEmpty(selectedValue) && new Guid(selectedValue) != Guid.Empty)
            {
                LoadFeeTypeData(new Guid(selectedValue));
                FeeTypeChange();
            }
            else
            {
                ddl_FeeType.SelectedValue = string.Empty;
            }
            Hid_CostsClass.Value = string.Empty;
            ReportCostChange();
        }

        //费用分类子类选择
        protected void ddl_FeeType_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedFeeType = ddl_FeeType.SelectedValue;
            if (!string.IsNullOrEmpty(selectedFeeType))
            {
                CostsClass(selectedFeeType);
                FeeTypeChange();
                RepeaterTerminiDataBind();
            }
            else
            {
                Hid_CostsClass.Value = string.Empty;
            }
            ReportCostChange();
        }

        //票据类型
        protected void rbl_InvoiceType_SelectedIndexChanged(object sender, EventArgs e)
        {
            InvoiceTypeChange();
            CostReportBillInfoList = new List<CostReportBillInfo>();
            RepeaterBillDataBind();
            AddNewBill();
        }
        //申报金额转换成大写形式
        protected void txt_ReportCost_TextChanged(object sender, EventArgs e)
        {
            ReportCostChange();
        }
        #endregion

        #region 数据列表相关
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
        //增加票据
        protected void btn_AddBill_Click(object sender, EventArgs e)
        {
            AddNewBill();
        }
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

        //上传票据信息
        protected void btn_Upload_Click(object sender, EventArgs e)
        {
            var excelName = UploadExcelName.Text;
            UploadExcelName.Text = string.Empty;

            #region 验证文件
            if (!UploadExcel.HasFile || string.IsNullOrEmpty(excelName))
            {
                MessageBox.Show(this, "请选择格式为“.xls”文件！");
                return;
            }

            var ext = Path.GetExtension(UploadExcel.FileName);
            if (ext != null && !ext.Equals(".xls"))
            {
                MessageBox.Show(this, "文件格式错误(.xls)！");
                return;
            }
            #endregion

            try
            {
                #region 将上传文件保存至临时文件夹
                string fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + ext;
                string folderPath = "~/UserDir/CostReport/Bill/";
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

                List<CostReportBillInfo> list = new List<CostReportBillInfo>();
                StringBuilder errorMsg = new StringBuilder();
                int index = 2;

                if (int.Parse(rbl_InvoiceType.SelectedValue).Equals((int)CostReportInvoiceType.Invoice))
                {
                    #region 普通发票
                    for (int i = 0; i < excelDataTable.Rows.Count; i++)
                    {
                        StringBuilder rowMsg = new StringBuilder();
                        var billUnit = excelDataTable.Rows[i]["开票单位"].ToString();
                        var billDate = excelDataTable.Rows[i]["开票日期"].ToString();
                        var billNo = excelDataTable.Rows[i]["发票号码"].ToString();
                        var billCode = excelDataTable.Rows[i]["发票代码"].ToString();
                        var taxAmount = excelDataTable.Rows[i]["含税额"].ToString();

                        #region 验证数据空值
                        if (string.IsNullOrEmpty(billUnit))
                        {
                            rowMsg.Append("第").Append(index).Append("行“开票单位”为空！").Append("\\n");
                        }
                        if (string.IsNullOrEmpty(billDate))
                        {
                            rowMsg.Append("第").Append(index).Append("行“开票日期”为空！").Append("\\n");
                        }
                        if (string.IsNullOrEmpty(billNo))
                        {
                            rowMsg.Append("第").Append(index).Append("行“发票号码”为空！").Append("\\n");
                        }
                        if (string.IsNullOrEmpty(billCode))
                        {
                            rowMsg.Append("第").Append(index).Append("行“发票代码”为空！").Append("\\n");
                        }
                        if (string.IsNullOrEmpty(taxAmount))
                        {
                            rowMsg.Append("第").Append(index).Append("行“含税额”为空！").Append("\\n");
                        }
                        #endregion

                        #region 验证数据格式
                        DateTime tryBillingDate;
                        if (!DateTime.TryParse(billDate, out tryBillingDate))
                        {
                            rowMsg.Append("第").Append(index).Append("行“开票日期”格式错误！").Append("\\n");
                        }
                        decimal tryTaxAmount;
                        if (!decimal.TryParse(taxAmount, out tryTaxAmount))
                        {
                            rowMsg.Append("第").Append(index).Append("行“含税额”格式错误！").Append("\\n");
                        }
                        #endregion

                        if (string.IsNullOrEmpty(rowMsg.ToString()))
                        {
                            #region 保存数据
                            var costReportBillInfo = new CostReportBillInfo
                            {
                                BillId = Guid.NewGuid(),
                                ReportId = Guid.Empty,
                                BillUnit = billUnit,
                                BillDate = DateTime.Parse(billDate),
                                BillNo = billNo,
                                BillCode = billCode,
                                NoTaxAmount = 0,
                                Tax = 0,
                                TaxAmount = decimal.Parse(taxAmount),
                                BillState = (int)CostReportBillState.UnSubmit,
                                OperatingTime = DateTime.Now,
                                Memo = string.Empty,
                                Remark = WebControl.RetrunUserAndTime("【添加发票】")
                            };
                            list.Add(costReportBillInfo);
                            #endregion
                        }
                        else
                        {
                            errorMsg.Append(rowMsg);
                        }

                        index++;
                    }
                    #endregion
                }
                else if (int.Parse(rbl_InvoiceType.SelectedValue).Equals((int)CostReportInvoiceType.VatInvoice))
                {
                    #region 增值税专用发票
                    for (int i = 0; i < excelDataTable.Rows.Count; i++)
                    {
                        StringBuilder rowMsg = new StringBuilder();
                        var billUnit = excelDataTable.Rows[i]["开票单位"].ToString();
                        var billDate = excelDataTable.Rows[i]["开票日期"].ToString();
                        var billNo = excelDataTable.Rows[i]["发票号码"].ToString();
                        var billCode = excelDataTable.Rows[i]["发票代码"].ToString();
                        var noTaxAmount = excelDataTable.Rows[i]["金额(不含税)"].ToString();
                        var tax = excelDataTable.Rows[i]["税额"].ToString();
                        var taxAmount = excelDataTable.Rows[i]["含税额"].ToString();

                        #region 验证数据空值
                        if (string.IsNullOrEmpty(billUnit))
                        {
                            rowMsg.Append("第").Append(index).Append("行“开票单位”为空！").Append("\\n");
                        }
                        if (string.IsNullOrEmpty(billDate))
                        {
                            rowMsg.Append("第").Append(index).Append("行“开票日期”为空！").Append("\\n");
                        }
                        if (string.IsNullOrEmpty(billNo))
                        {
                            rowMsg.Append("第").Append(index).Append("行“发票号码”为空！").Append("\\n");
                        }
                        if (string.IsNullOrEmpty(billCode))
                        {
                            rowMsg.Append("第").Append(index).Append("行“发票代码”为空！").Append("\\n");
                        }
                        if (string.IsNullOrEmpty(noTaxAmount))
                        {
                            rowMsg.Append("第").Append(index).Append("行“金额(不含税)”为空！").Append("\\n");
                        }
                        if (string.IsNullOrEmpty(tax))
                        {
                            rowMsg.Append("第").Append(index).Append("行“税额”为空！").Append("\\n");
                        }
                        if (string.IsNullOrEmpty(taxAmount))
                        {
                            rowMsg.Append("第").Append(index).Append("行“含税额”为空！").Append("\\n");
                        }
                        #endregion

                        #region 验证数据格式
                        DateTime tryBillingDate;
                        if (!DateTime.TryParse(billDate, out tryBillingDate))
                        {
                            rowMsg.Append("第").Append(index).Append("行“开票日期”格式错误！").Append("\\n");
                        }
                        decimal tryNoTaxAmount;
                        if (!decimal.TryParse(noTaxAmount, out tryNoTaxAmount))
                        {
                            rowMsg.Append("第").Append(index).Append("行“金额(不含税)”格式错误！").Append("\\n");
                        }
                        decimal tryTax;
                        if (!decimal.TryParse(tax, out tryTax))
                        {
                            rowMsg.Append("第").Append(index).Append("行“税额”格式错误！").Append("\\n");
                        }
                        decimal tryTaxAmount;
                        if (!decimal.TryParse(taxAmount, out tryTaxAmount))
                        {
                            rowMsg.Append("第").Append(index).Append("行“含税额”格式错误！").Append("\\n");
                        }
                        #endregion

                        #region “金额(不含税)”+“税额”=“含税额”
                        if (!string.IsNullOrEmpty(noTaxAmount) && !string.IsNullOrEmpty(tax) && !string.IsNullOrEmpty(taxAmount))
                        {
                            if (decimal.Parse(noTaxAmount) + decimal.Parse(tax) != decimal.Parse(taxAmount))
                            {
                                rowMsg.Append("第").Append(index).Append("行“金额(不含税)”+“税额”≠“含税额”！").Append("\\n");
                            }
                        }
                        #endregion

                        if (string.IsNullOrEmpty(rowMsg.ToString()))
                        {
                            #region 保存数据
                            var costReportBillInfo = new CostReportBillInfo
                            {
                                BillId = Guid.NewGuid(),
                                ReportId = Guid.Empty,
                                BillUnit = billUnit,
                                BillDate = DateTime.Parse(billDate),
                                BillNo = billNo,
                                BillCode = billCode,
                                NoTaxAmount = decimal.Parse(noTaxAmount),
                                Tax = decimal.Parse(tax),
                                TaxAmount = decimal.Parse(taxAmount),
                                BillState = (int)CostReportBillState.UnSubmit,
                                OperatingTime = DateTime.Now,
                                Memo = string.Empty,
                                Remark = WebControl.RetrunUserAndTime("【添加发票】")
                            };
                            list.Add(costReportBillInfo);
                            #endregion
                        }
                        else
                        {
                            errorMsg.Append(rowMsg);
                        }

                        index++;
                    }
                    #endregion
                }
                else if (int.Parse(rbl_InvoiceType.SelectedValue).Equals((int)CostReportInvoiceType.Voucher))
                {
                    #region 收据
                    for (int i = 0; i < excelDataTable.Rows.Count; i++)
                    {
                        StringBuilder rowMsg = new StringBuilder();
                        var billUnit = excelDataTable.Rows[i]["开票单位"].ToString();
                        var billDate = excelDataTable.Rows[i]["开票日期"].ToString();
                        var billNo = excelDataTable.Rows[i]["收据号码"].ToString();
                        var taxAmount = excelDataTable.Rows[i]["金额"].ToString();

                        #region 验证数据空值
                        if (string.IsNullOrEmpty(billUnit))
                        {
                            rowMsg.Append("第").Append(index).Append("行“开票单位”为空！").Append("\\n");
                        }
                        if (string.IsNullOrEmpty(billDate))
                        {
                            rowMsg.Append("第").Append(index).Append("行“开票日期”为空！").Append("\\n");
                        }
                        if (string.IsNullOrEmpty(billNo))
                        {
                            rowMsg.Append("第").Append(index).Append("行“收据号码”为空！").Append("\\n");
                        }
                        if (string.IsNullOrEmpty(taxAmount))
                        {
                            rowMsg.Append("第").Append(index).Append("行“金额”为空！").Append("\\n");
                        }
                        #endregion

                        #region 验证数据格式
                        DateTime tryBillingDate;
                        if (!DateTime.TryParse(billDate, out tryBillingDate))
                        {
                            rowMsg.Append("第").Append(index).Append("行“开票日期”格式错误！").Append("\\n");
                        }
                        decimal tryTaxAmount;
                        if (!decimal.TryParse(taxAmount, out tryTaxAmount))
                        {
                            rowMsg.Append("第").Append(index).Append("行“金额”格式错误！").Append("\\n");
                        }
                        #endregion

                        if (string.IsNullOrEmpty(rowMsg.ToString()))
                        {
                            #region 保存数据
                            var costReportBillInfo = new CostReportBillInfo
                            {
                                BillId = Guid.NewGuid(),
                                ReportId = Guid.Empty,
                                BillUnit = billUnit,
                                BillDate = DateTime.Parse(billDate),
                                BillNo = billNo,
                                BillCode = string.Empty,
                                NoTaxAmount = 0,
                                Tax = 0,
                                TaxAmount = decimal.Parse(taxAmount),
                                BillState = (int)CostReportBillState.UnSubmit,
                                OperatingTime = DateTime.Now,
                                Memo = string.Empty,
                                Remark = WebControl.RetrunUserAndTime("【添加收据】")
                            };
                            list.Add(costReportBillInfo);
                            #endregion
                        }
                        else
                        {
                            errorMsg.Append(rowMsg);
                        }

                        index++;
                    }
                    #endregion
                }

                if (!string.IsNullOrEmpty(errorMsg.ToString()))
                {
                    MessageBox.Show(this, errorMsg.ToString());
                    return;
                }

                if (list.Any())
                {
                    var tempList = CostReportBillInfoList.Where(p => !p.BillId.Equals(Guid.Empty)).ToList();
                    tempList.AddRange(list);
                    CostReportBillInfoList = tempList;
                    RepeaterBillDataBind();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message);
            }
        }

        //保存票据
        protected void btn_BillAdd_Click(object sender, EventArgs e)
        {
            var dataItem = (RepeaterItem)((Button)sender).Parent;
            if (dataItem != null)
            {
                var billId = new Guid(((Button)sender).CommandName);
                var txtBillUnit = (TextBox)dataItem.FindControl("txt_BillUnit");
                var txtBillDate = (TextBox)dataItem.FindControl("txt_BillDate");
                var txtBillNo = (TextBox)dataItem.FindControl("txt_BillNo");
                var txtBillCode = (TextBox)dataItem.FindControl("txt_BillCode");
                var txtNoTaxAmount = (TextBox)dataItem.FindControl("txt_NoTaxAmount");
                var txtTax = (TextBox)dataItem.FindControl("txt_Tax");
                var txtTaxAmount = (TextBox)dataItem.FindControl("txt_TaxAmount");
                var txtMemo = (TextBox)dataItem.FindControl("txt_Memo");
                var remark = (Literal)dataItem.FindControl("lit_Remark");
                var isPay = (Literal)dataItem.FindControl("lit_IsPay");
                var isPass = (Literal)dataItem.FindControl("lit_IsPass");

                var costReportBillInfo = CostReportBillInfoList.FirstOrDefault(p => p.BillId.Equals(billId));
                if (costReportBillInfo != null)
                {
                    costReportBillInfo.BillId = billId.Equals(Guid.Empty) ? Guid.NewGuid() : billId;
                    costReportBillInfo.ReportId = Guid.Empty;
                    costReportBillInfo.BillUnit = txtBillUnit.Text;
                    costReportBillInfo.BillDate = DateTime.Parse(txtBillDate.Text);
                    costReportBillInfo.BillNo = txtBillNo.Text;
                    costReportBillInfo.BillCode = txtBillCode.Text;
                    costReportBillInfo.NoTaxAmount = decimal.Parse(string.IsNullOrEmpty(txtNoTaxAmount.Text) ? "0" : txtNoTaxAmount.Text);
                    costReportBillInfo.Tax = decimal.Parse(string.IsNullOrEmpty(txtTax.Text) ? "0" : txtTax.Text);
                    costReportBillInfo.TaxAmount = decimal.Parse(txtTaxAmount.Text);
                    costReportBillInfo.BillState = (int)CostReportBillState.UnSubmit;
                    costReportBillInfo.OperatingTime = costReportBillInfo.OperatingTime;
                    costReportBillInfo.Memo = txtMemo.Text;
                    costReportBillInfo.Remark = string.IsNullOrEmpty(Request.QueryString["ReportId"])
                        ? WebControl.RetrunUserAndTime("【添加" + rbl_InvoiceType.SelectedItem.Text + "】")
                        : remark.Text + WebControl.RetrunUserAndTime("【修改" + rbl_InvoiceType.SelectedItem.Text + "】");
                    costReportBillInfo.IsPay = bool.Parse(isPay.Text);
                    costReportBillInfo.IsPass = bool.Parse(isPass.Text);

                    CostReportBillInfoList.Remove(costReportBillInfo);
                    CostReportBillInfoList.Add(costReportBillInfo);
                }
            }
            RepeaterBillDataBind();
        }

        //删除票据
        protected void btn_BillDel_Click(object sender, EventArgs e)
        {
            var billId = new Guid(((Button)sender).CommandName);
            var removeBillItem = CostReportBillInfoList.FirstOrDefault(p => p.BillId.Equals(billId));
            CostReportBillInfoList.Remove(removeBillItem);
            RepeaterBillDataBind();
        }

        //增加票据
        protected void AddNewBill()
        {
            var billItem = CostReportBillInfoList.FirstOrDefault(p => p.BillId.Equals(Guid.Empty));
            if (billItem == null)
            {
                var list = CostReportBillInfoList;
                list.Add(new CostReportBillInfo { OperatingTime = DateTime.Now });
                CostReportBillInfoList = list;
                RepeaterBillDataBind();
            }
        }

        //保存数据时调用此方法(原因：防止当用户光标未离开表格，而直接点击保存按钮，导致数据没有保存上)
        protected string SaveBill()
        {
            var itemCount = Repeater_Bill.Items.Count;
            if (itemCount > 0)
            {
                List<CostReportBillInfo> list = new List<CostReportBillInfo>();
                StringBuilder errorMsg = new StringBuilder();
                foreach (RepeaterItem item in Repeater_Bill.Items)
                {
                    if (item != null)
                    {
                        var index = item.ItemIndex + 1;
                        var txtBillUnit = (TextBox)item.FindControl("txt_BillUnit");
                        var txtBillDate = (TextBox)item.FindControl("txt_BillDate");
                        var txtBillNo = (TextBox)item.FindControl("txt_BillNo");
                        var txtBillCode = (TextBox)item.FindControl("txt_BillCode");
                        var txtNoTaxAmount = (TextBox)item.FindControl("txt_NoTaxAmount");
                        var txtTax = (TextBox)item.FindControl("txt_Tax");
                        var txtTaxAmount = (TextBox)item.FindControl("txt_TaxAmount");
                        var txtMemo = (TextBox)item.FindControl("txt_Memo");
                        var remark = (Literal)item.FindControl("lit_Remark");
                        var isPay = (Literal)item.FindControl("lit_IsPay");
                        var isPass = (Literal)item.FindControl("lit_IsPass");
                        var billState = (Literal)item.FindControl("lit_BillState");

                        if (int.Parse(rbl_InvoiceType.SelectedValue).Equals((int)CostReportInvoiceType.Invoice))
                        {
                            #region 普通发票
                            errorMsg.Clear();
                            StringBuilder rowMsg = new StringBuilder();

                            #region 验证数据空值
                            if (string.IsNullOrEmpty(txtBillUnit.Text))
                            {
                                rowMsg.Append("第").Append(index).Append("行“开票单位”为空！").Append("\\n");
                            }
                            if (string.IsNullOrEmpty(txtBillDate.Text))
                            {
                                rowMsg.Append("第").Append(index).Append("行“开票日期”为空！").Append("\\n");
                            }
                            if (string.IsNullOrEmpty(txtBillNo.Text))
                            {
                                rowMsg.Append("第").Append(index).Append("行“发票号码”为空！").Append("\\n");
                            }
                            if (string.IsNullOrEmpty(txtBillCode.Text))
                            {
                                rowMsg.Append("第").Append(index).Append("行“发票代码”为空！").Append("\\n");
                            }
                            if (string.IsNullOrEmpty(txtTaxAmount.Text))
                            {
                                rowMsg.Append("第").Append(index).Append("行“含税金额”为空！").Append("\\n");
                            }
                            #endregion

                            if (string.IsNullOrEmpty(rowMsg.ToString()))
                            {
                                #region 验证数据格式
                                DateTime tryBillingDate;
                                if (!DateTime.TryParse(txtBillDate.Text, out tryBillingDate))
                                {
                                    errorMsg.Append("第").Append(index).Append("行“开票日期”格式错误！").Append("\\n");
                                }
                                decimal tryTaxAmount;
                                if (!decimal.TryParse(txtTaxAmount.Text, out tryTaxAmount))
                                {
                                    errorMsg.Append("第").Append(index).Append("行“含税金额”格式错误！").Append("\\n");
                                }
                                #endregion

                                if (string.IsNullOrEmpty(errorMsg.ToString()))
                                {
                                    #region 保存数据
                                    var costReportBillInfo = new CostReportBillInfo
                                    {
                                        BillId = Guid.NewGuid(),
                                        ReportId = Guid.Empty,
                                        BillUnit = txtBillUnit.Text,
                                        BillDate = DateTime.Parse(txtBillDate.Text),
                                        BillNo = txtBillNo.Text,
                                        BillCode = txtBillCode.Text,
                                        NoTaxAmount = 0,
                                        Tax = 0,
                                        TaxAmount = decimal.Parse(decimal.Parse(txtTaxAmount.Text).ToString("F2")),
                                        BillState = int.Parse(billState.Text),
                                        OperatingTime = DateTime.Now,
                                        Memo = txtMemo.Text,
                                        Remark = string.IsNullOrEmpty(Request.QueryString["ReportId"]) ? WebControl.RetrunUserAndTime("【添加" + rbl_InvoiceType.SelectedItem.Text + "】") : remark.Text + WebControl.RetrunUserAndTime("【修改" + rbl_InvoiceType.SelectedItem.Text + "】"),
                                        IsPay = bool.Parse(isPay.Text),
                                        IsPass = bool.Parse(isPass.Text)
                                    };
                                    list.Add(costReportBillInfo);
                                    #endregion
                                }
                            }
                            //else
                            //{
                            //    errorMsg.Append(rowMsg);
                            //}
                            #endregion
                        }
                        else if (int.Parse(rbl_InvoiceType.SelectedValue).Equals((int)CostReportInvoiceType.VatInvoice))
                        {
                            #region 增值税专用发票
                            errorMsg.Clear();
                            StringBuilder rowMsg = new StringBuilder();

                            #region 验证数据空值
                            if (string.IsNullOrEmpty(txtBillUnit.Text))
                            {
                                rowMsg.Append("第").Append(index).Append("行“开票单位”为空！").Append("\\n");
                            }
                            if (string.IsNullOrEmpty(txtBillDate.Text))
                            {
                                rowMsg.Append("第").Append(index).Append("行“开票日期”为空！").Append("\\n");
                            }
                            if (string.IsNullOrEmpty(txtBillNo.Text))
                            {
                                rowMsg.Append("第").Append(index).Append("行“发票号码”为空！").Append("\\n");
                            }
                            if (string.IsNullOrEmpty(txtBillCode.Text))
                            {
                                rowMsg.Append("第").Append(index).Append("行“发票代码”为空！").Append("\\n");
                            }
                            if (string.IsNullOrEmpty(txtNoTaxAmount.Text))
                            {
                                rowMsg.Append("第").Append(index).Append("行“未税金额”为空！").Append("\\n");
                            }
                            if (string.IsNullOrEmpty(txtTax.Text))
                            {
                                rowMsg.Append("第").Append(index).Append("行“税额”为空！").Append("\\n");
                            }
                            if (string.IsNullOrEmpty(txtTaxAmount.Text))
                            {
                                rowMsg.Append("第").Append(index).Append("行“含税金额”为空！").Append("\\n");
                            }
                            #endregion

                            if (string.IsNullOrEmpty(rowMsg.ToString()))
                            {
                                #region 验证数据格式
                                DateTime tryBillingDate;
                                if (!DateTime.TryParse(txtBillDate.Text, out tryBillingDate))
                                {
                                    errorMsg.Append("第").Append(index).Append("行“开票日期”格式错误！").Append("\\n");
                                }
                                decimal tryNoTaxAmount;
                                if (!decimal.TryParse(txtNoTaxAmount.Text, out tryNoTaxAmount))
                                {
                                    errorMsg.Append("第").Append(index).Append("行“未税金额”格式错误！").Append("\\n");
                                }
                                decimal tryTax;
                                if (!decimal.TryParse(txtTax.Text, out tryTax))
                                {
                                    errorMsg.Append("第").Append(index).Append("行“税额”格式错误！").Append("\\n");
                                }
                                decimal tryTaxAmount;
                                if (!decimal.TryParse(txtTaxAmount.Text, out tryTaxAmount))
                                {
                                    errorMsg.Append("第").Append(index).Append("行“含税金额”格式错误！").Append("\\n");
                                }
                                #endregion

                                #region “金额(不含税)”+“税额”=“含税额”
                                if (!string.IsNullOrEmpty(txtNoTaxAmount.Text) && !string.IsNullOrEmpty(txtTax.Text) && !string.IsNullOrEmpty(txtTaxAmount.Text))
                                {
                                    if (decimal.Parse(decimal.Parse(txtNoTaxAmount.Text).ToString("F2")) + decimal.Parse(decimal.Parse(txtTax.Text).ToString("F2")) != decimal.Parse(decimal.Parse(txtTaxAmount.Text).ToString("F2")))
                                    {
                                        errorMsg.Append("第").Append(index).Append("行“金额(不含税)”+“税额”≠“含税额”！").Append("\\n");
                                    }
                                }
                                #endregion

                                if (string.IsNullOrEmpty(errorMsg.ToString()))
                                {
                                    #region 保存数据
                                    var costReportBillInfo = new CostReportBillInfo
                                    {
                                        BillId = Guid.NewGuid(),
                                        ReportId = Guid.Empty,
                                        BillUnit = txtBillUnit.Text,
                                        BillDate = DateTime.Parse(txtBillDate.Text),
                                        BillNo = txtBillNo.Text,
                                        BillCode = txtBillCode.Text,
                                        NoTaxAmount = decimal.Parse(decimal.Parse(txtNoTaxAmount.Text).ToString("F2")),
                                        Tax = decimal.Parse(decimal.Parse(txtTax.Text).ToString("F2")),
                                        TaxAmount = decimal.Parse(decimal.Parse(txtTaxAmount.Text).ToString("F2")),
                                        BillState = int.Parse(billState.Text),
                                        OperatingTime = DateTime.Now,
                                        Memo = txtMemo.Text,
                                        Remark = string.IsNullOrEmpty(Request.QueryString["ReportId"]) ? WebControl.RetrunUserAndTime("【添加" + rbl_InvoiceType.SelectedItem.Text + "】") : remark.Text + WebControl.RetrunUserAndTime("【修改" + rbl_InvoiceType.SelectedItem.Text + "】"),
                                        IsPay = bool.Parse(isPay.Text),
                                        IsPass = bool.Parse(isPass.Text)
                                    };
                                    list.Add(costReportBillInfo);
                                    #endregion
                                }
                            }
                            //else
                            //{
                            //    errorMsg.Append(rowMsg);
                            //}
                            #endregion
                        }
                        else if (int.Parse(rbl_InvoiceType.SelectedValue).Equals((int)CostReportInvoiceType.Voucher))
                        {
                            #region 收据
                            errorMsg.Clear();
                            StringBuilder rowMsg = new StringBuilder();

                            #region 验证数据空值
                            if (string.IsNullOrEmpty(txtBillUnit.Text))
                            {
                                rowMsg.Append("第").Append(index).Append("行“开票单位”为空！").Append("\\n");
                            }
                            if (string.IsNullOrEmpty(txtBillDate.Text))
                            {
                                rowMsg.Append("第").Append(index).Append("行“开票日期”为空！").Append("\\n");
                            }
                            if (string.IsNullOrEmpty(txtBillNo.Text))
                            {
                                rowMsg.Append("第").Append(index).Append("行“收据号码”为空！").Append("\\n");
                            }
                            if (string.IsNullOrEmpty(txtTaxAmount.Text))
                            {
                                rowMsg.Append("第").Append(index).Append("行“含税金额”为空！").Append("\\n");
                            }
                            #endregion

                            if (string.IsNullOrEmpty(rowMsg.ToString()))
                            {
                                #region 验证数据格式
                                DateTime tryBillingDate;
                                if (!DateTime.TryParse(txtBillDate.Text, out tryBillingDate))
                                {
                                    errorMsg.Append("第").Append(index).Append("行“开票日期”格式错误！").Append("\\n");
                                }
                                decimal tryTaxAmount;
                                if (!decimal.TryParse(txtTaxAmount.Text, out tryTaxAmount))
                                {
                                    errorMsg.Append("第").Append(index).Append("行“金额”格式错误！").Append("\\n");
                                }
                                #endregion

                                if (string.IsNullOrEmpty(errorMsg.ToString()))
                                {
                                    #region 保存数据
                                    var costReportBillInfo = new CostReportBillInfo
                                    {
                                        BillId = Guid.NewGuid(),
                                        ReportId = Guid.Empty,
                                        BillUnit = txtBillUnit.Text,
                                        BillDate = DateTime.Parse(txtBillDate.Text),
                                        BillNo = txtBillNo.Text,
                                        BillCode = txtBillCode.Text,
                                        NoTaxAmount = 0,
                                        Tax = 0,
                                        TaxAmount = decimal.Parse(decimal.Parse(txtTaxAmount.Text).ToString("F2")),
                                        BillState = int.Parse(billState.Text),
                                        OperatingTime = DateTime.Now,
                                        Memo = txtMemo.Text,
                                        Remark = string.IsNullOrEmpty(Request.QueryString["ReportId"]) ? WebControl.RetrunUserAndTime("【添加" + rbl_InvoiceType.SelectedItem.Text + "】") : remark.Text + WebControl.RetrunUserAndTime("【修改" + rbl_InvoiceType.SelectedItem.Text + "】"),
                                        IsPay = bool.Parse(isPay.Text),
                                        IsPass = bool.Parse(isPass.Text)
                                    };
                                    list.Add(costReportBillInfo);
                                    #endregion
                                }
                            }
                            //else
                            //{
                            //    errorMsg.Append(rowMsg);
                            //}
                            #endregion
                        }
                    }
                }

                if (string.IsNullOrEmpty(errorMsg.ToString()) && list.Any())
                {
                    CostReportBillInfoList = list;
                    RepeaterBillDataBind();
                }
                else
                {
                    return errorMsg.ToString();
                }
            }
            return string.Empty;
        }
        #endregion
        #endregion

        #region 差旅费起讫
        //起讫数据源
        protected void RepeaterTerminiDataBind()
        {
            Repeater_Termini.DataSource = CostReportTerminiInfoList.OrderByDescending(p => p.OperatingTime);
            Repeater_Termini.DataBind();
        }

        #region 列表操作
        //增加起讫
        protected void btn_AddTermini_Click(object sender, EventArgs e)
        {
            AddTermini();
        }

        //保存起讫
        protected void btn_TerminiAdd_Click(object sender, EventArgs e)
        {
            var dataItem = (RepeaterItem)((Button)sender).Parent;
            if (dataItem != null)
            {
                var terminiId = new Guid(((Button)sender).CommandName);
                var txtStartDay = (TextBox)dataItem.FindControl("txt_StartDay");
                var txtEndDay = (TextBox)dataItem.FindControl("txt_EndDay");
                var txtTerminiLocation = (TextBox)dataItem.FindControl("txt_TerminiLocation");

                var costReportTerminiInfo = CostReportTerminiInfoList.FirstOrDefault(p => p.TerminiId.Equals(terminiId));
                if (costReportTerminiInfo != null)
                {
                    costReportTerminiInfo.TerminiId = terminiId.Equals(Guid.Empty) ? Guid.NewGuid() : terminiId;
                    costReportTerminiInfo.ReportId = Guid.Empty;
                    costReportTerminiInfo.StartDay = DateTime.Parse(txtStartDay.Text);
                    costReportTerminiInfo.EndDay = DateTime.Parse(txtEndDay.Text);
                    costReportTerminiInfo.TerminiLocation = txtTerminiLocation.Text;
                    costReportTerminiInfo.OperatingTime = DateTime.Now;

                    CostReportTerminiInfoList.Remove(costReportTerminiInfo);
                    CostReportTerminiInfoList.Add(costReportTerminiInfo);
                }
            }
            RepeaterTerminiDataBind();
        }

        //删除起讫
        protected void btn_TerminiDel_Click(object sender, EventArgs e)
        {
            var terminiId = new Guid(((Button)sender).CommandName);
            var removeItem = CostReportTerminiInfoList.FirstOrDefault(p => p.TerminiId.Equals(terminiId));
            CostReportTerminiInfoList.Remove(removeItem);
            RepeaterTerminiDataBind();
        }

        //增加起讫
        protected void AddTermini()
        {
            var item = CostReportTerminiInfoList.FirstOrDefault(p => p.TerminiId.Equals(Guid.Empty));
            if (item == null)
            {
                var list = CostReportTerminiInfoList;
                list.Add(new CostReportTerminiInfo { OperatingTime = DateTime.Now });
                CostReportTerminiInfoList = list;
                RepeaterTerminiDataBind();
            }
        }

        //保存数据时调用此方法(原因：防止当用户光标未离开表格，而直接点击保存按钮，导致数据没有保存上)
        protected string SaveTermini()
        {
            var itemCount = Repeater_Termini.Items.Count;
            if (itemCount > 0)
            {
                List<CostReportTerminiInfo> list = new List<CostReportTerminiInfo>();
                StringBuilder errorMsg = new StringBuilder();
                foreach (RepeaterItem item in Repeater_Termini.Items)
                {
                    if (item != null)
                    {
                        var index = item.ItemIndex + 1;
                        var txtStartDay = (TextBox)item.FindControl("txt_StartDay");
                        var txtEndDay = (TextBox)item.FindControl("txt_EndDay");
                        var txtTerminiLocation = (TextBox)item.FindControl("txt_TerminiLocation");

                        #region 普通发票
                        errorMsg.Clear();

                        #region 验证数据空值
                        if (string.IsNullOrEmpty(txtStartDay.Text))
                        {
                            errorMsg.Append("第").Append(index).Append("行“起日”为空！").Append("\\n");
                        }
                        if (string.IsNullOrEmpty(txtEndDay.Text))
                        {
                            errorMsg.Append("第").Append(index).Append("行“止日”为空！").Append("\\n");
                        }
                        if (string.IsNullOrEmpty(txtTerminiLocation.Text))
                        {
                            errorMsg.Append("第").Append(index).Append("行“起讫地点”为空！").Append("\\n");
                        }
                        #endregion

                        #region 验证数据格式
                        DateTime tryStartDay;
                        if (!DateTime.TryParse(txtStartDay.Text, out tryStartDay))
                        {
                            errorMsg.Append("第").Append(index).Append("行“起日”格式错误！").Append("\\n");
                        }
                        DateTime tryEndDay;
                        if (!DateTime.TryParse(txtEndDay.Text, out tryEndDay))
                        {
                            errorMsg.Append("第").Append(index).Append("行“止日”格式错误！").Append("\\n");
                        }
                        #endregion

                        if (string.IsNullOrEmpty(errorMsg.ToString()))
                        {
                            #region 保存数据
                            var costReportTerminiInfo = new CostReportTerminiInfo
                            {
                                TerminiId = Guid.NewGuid(),
                                ReportId = Guid.Empty,
                                StartDay = DateTime.Parse(txtStartDay.Text),
                                EndDay = DateTime.Parse(txtEndDay.Text),
                                TerminiLocation = txtTerminiLocation.Text,
                                OperatingTime = DateTime.Now
                            };
                            list.Add(costReportTerminiInfo);
                            #endregion
                        }
                        #endregion
                    }
                }
                if (string.IsNullOrEmpty(errorMsg.ToString()) && list.Any())
                {
                    CostReportTerminiInfoList = list;
                    RepeaterTerminiDataBind();
                }
                else
                {
                    return errorMsg.ToString();
                }
            }
            return string.Empty;
        }
        #endregion
        #endregion
        #endregion

        //保存数据
        protected void btn_Save_Click(object sender, EventArgs e)
        {
            #region 60s内禁止重复提交
            if (!_submitController.Enabled)
            {
                MessageBox.AppendScript(this, "alert('程序正在处理中，请稍候...');ParentCloseAndRebind();");
                return;
            }
            #endregion

            #region 差旅费(提前判断差旅费，因为验证数据时需要用到此结果)
            var selectedValue = ddl_FeeType.SelectedValue;
            if (!string.IsNullOrEmpty(selectedValue) && new Guid(selectedValue).Equals(new Guid(CostReport_TravelExpenses)))
            {
                EditTravelList(_costReportTravelInfoList);
            }
            #endregion

            #region 验证数据
            var errorMsg = CheckData();
            if (!string.IsNullOrEmpty(errorMsg))
            {
                MessageBox.Show(this, errorMsg);
                return;
            }
            #endregion

            #region 赋值
            CostReportInfo model;
            var reportId = Request.QueryString["ReportId"];
            if (string.IsNullOrEmpty(reportId))//添加
            {
                model = new CostReportInfo { ReportId = Guid.NewGuid() };
                EditCostReportModel(model, Personnel);
            }
            else//修改
            {
                model = _costReport.GetReportByReportId(new Guid(reportId));
                if (model.State != (int)CostReportState.Auditing && 
                    model.State != (int)CostReportState.AuditingNoPass &&
                    model.State != (int)CostReportState.InvoiceNoPass)
                {
                    MessageBox.Show(this, "该单据状态已更新，不允许此操作！");
                    return;
                }
                EditCostReportModel(model, Personnel);
            }
            #endregion

            #region 保存数据
            using (var ts = new TransactionScope(TransactionScopeOption.Required))
            {
                try
                {
                    #region 广告使用图片(广告费/购买物品)
                    var costsClass = Hid_CostsClass.Value;
                    if (!string.IsNullOrEmpty(costsClass))
                    {
                        if (costsClass.Equals(((int)CostsVarieties.AdvertisingFee).ToString()))
                        {
                            var costReportInvoice = _costReportInvoice.GetInvoice(model.InvoiceId);
                            if (costReportInvoice != null && !costReportInvoice.InvoiceId.Equals(Guid.Empty))
                            {
                                if (!string.IsNullOrEmpty(UploadImgName.Text))
                                {
                                    string filePath = UploadVoucher(costReportInvoice.ImagePath);
                                    if (!string.IsNullOrEmpty(filePath))
                                    {
                                        var invoiceInfo = new CostReportInvoiceInfo(model.InvoiceId, filePath, 0, 0, string.Empty);
                                        _costReportInvoice.UpdateInvoice(invoiceInfo);
                                    }
                                }
                                else
                                {
                                    #region 如果上传文件已经存在，则删除该文件
                                    if (!string.IsNullOrEmpty(costReportInvoice.ImagePath) && File.Exists(Server.MapPath(costReportInvoice.ImagePath)))
                                    {
                                        File.Delete(Server.MapPath(costReportInvoice.ImagePath));
                                    }
                                    #endregion

                                    _costReportInvoice.DelInvoice(model.InvoiceId);
                                    model.InvoiceId = Guid.Empty;
                                }
                            }
                            else
                            {
                                string filePath = UploadVoucher(string.Empty);
                                if (!string.IsNullOrEmpty(filePath))
                                {
                                    model.InvoiceId = Guid.NewGuid();
                                    var costReportInvoiceInfo = new CostReportInvoiceInfo(model.InvoiceId, filePath, 0, 0, string.Empty);
                                    _costReportInvoice.InsertInvoice(costReportInvoiceInfo);
                                }
                            }
                        }
                    }
                    #endregion

                    #region 差旅费
                    if (!string.IsNullOrEmpty(selectedValue) && new Guid(selectedValue).Equals(new Guid(CostReport_TravelExpenses)))
                    {
                        if (!string.IsNullOrEmpty(reportId))//如果是修改，则先删除再插入数据
                        {
                            _costReportTravel.DeletelmShop_CostReportTravelByReportId(model.ReportId);//删除差旅费
                            _costReportTermini.DeletelmShop_CostReportTerminiByReportId(model.ReportId);//删除起讫
                        }
                        if (_costReportTravelInfoList.Any())//添加差旅费
                        {
                            foreach (var item in _costReportTravelInfoList)
                            {
                                item.ReportId = model.ReportId;
                            }
                            _costReportTravel.AddBatchlmShop_CostReportTravel(_costReportTravelInfoList);
                        }
                        var removeTerminiItem = CostReportTerminiInfoList.FirstOrDefault(p => p.TerminiId.Equals(Guid.Empty));
                        CostReportTerminiInfoList.Remove(removeTerminiItem);
                        if (CostReportTerminiInfoList.Any())//添加起讫
                        {
                            foreach (var item in CostReportTerminiInfoList)
                            {
                                item.ReportId = model.ReportId;
                            }
                            _costReportTermini.AddBatchlmShop_CostReportTermini(CostReportTerminiInfoList);
                        }
                    }
                    #endregion

                    #region 票据类型相关
                    var removeBillItem = CostReportBillInfoList.FirstOrDefault(p => p.BillId.Equals(Guid.Empty));
                    CostReportBillInfoList.Remove(removeBillItem);
                    if (!string.IsNullOrEmpty(reportId))//如果是修改，则先删除再插入数据
                    {
                        _costReportBill.Deletelmshop_CostReportBillByReportId(model.ReportId);//删除票据
                    }
                    if (CostReportBillInfoList.Any())//添加票据
                    {
                        foreach (var item in CostReportBillInfoList)
                        {
                            item.ReportId = model.ReportId;
                        }
                        _costReportBill.AddBatchlmshop_CostReportBill(CostReportBillInfoList);
                    }
                    model.ApplyForCost = CostReportBillInfoList.Sum(p => p.TaxAmount);
                    #endregion

                    if (string.IsNullOrEmpty(reportId))//添加
                    {
                        string errorMessage;
                        var result = _costReport.InsertReport(model, out errorMessage);
                        if (!result)
                        {
                            MessageBox.Show(this, "申报失败！");
                        }
                    }
                    else//修改
                    {
                        _costReport.UpdateReport(model);
                    }

                    //添加日志
                    _operationLogManager.Add(Personnel.PersonnelId, Personnel.RealName, model.ReportId, model.ReportNo, OperationPoint.CostDeclare.Add.GetBusinessInfo(), 1, "");

                    ts.Complete();
                    MessageBox.AppendScript(this, "setTimeout(function(){ ParentCloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");

                    #region 60s内禁止重复提交
                    _submitController.Submit();
                    #endregion
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

        #region 编辑Model
        //费用申报(lmshop_CostReport)
        protected void EditCostReportModel(CostReportInfo model, PersonnelInfo personnelInfo)
        {
            model.ReportNo = new CodeManager().GetCode(CodeType.RE);
            model.ReportKind = (int)CostReportKind.Later;
            model.AssumeBranchId = string.IsNullOrEmpty(ddl_AssumeBranch.SelectedValue) ? Guid.Empty : new Guid(ddl_AssumeBranch.SelectedValue);
            model.AssumeGroupId = string.IsNullOrEmpty(ddl_AssumeGroup.SelectedValue) ? Guid.Empty : new Guid(ddl_AssumeGroup.SelectedValue);
            model.AssumeShopId = string.IsNullOrEmpty(ddl_AssumeShop.SelectedValue) ? Guid.Empty : new Guid(ddl_AssumeShop.SelectedValue);
            model.CostsVarieties = string.IsNullOrEmpty(Hid_CostsClass.Value) ? -1 : int.Parse(Hid_CostsClass.Value);
            model.GoodsCode = txt_GoodsCode.Text;
            model.CompanyClassId = string.IsNullOrEmpty(ddl_CompanyClass.SelectedValue) ? Guid.Empty : new Guid(ddl_CompanyClass.SelectedValue);
            model.CompanyId = string.IsNullOrEmpty(ddl_FeeType.SelectedValue) ? Guid.Empty : new Guid(ddl_FeeType.SelectedValue);
            model.UrgentOrDefer = int.Parse(rbl_UrgentOrDefer.SelectedValue);
            model.UrgentReason = model.UrgentOrDefer.Equals(1) ? txt_UrgentReason.Text : string.Empty;
            model.ReportName = txt_ReportName.Text;
            model.StartTime = DateTime.Parse(txt_StartTime.Text);
            model.EndTime = DateTime.Parse(txt_EndTime.Text);
            model.PayCompany = txt_PayCompany.Text;
            model.ReportCost = Math.Abs(Decimal.Parse(txt_ReportCost.Text));
            model.RealityCost = model.ReportCost;
            model.ActualAmount = model.ReportCost;
            model.CostType = int.Parse(rbl_CostType.SelectedValue);
            model.Deposit = 2;
            if (model.CostType.Equals(2))
            {
                model.BankAccountName = txt_BankName.Text.Trim() + "," + txt_SubBankName.Text.Trim();
                model.BankAccount = txt_BankAccount.Text;//收款账号
            }
            model.InvoiceType = int.Parse(rbl_InvoiceType.SelectedValue);
            model.InvoiceTitle = ddl_InvoiceTitle.SelectedItem.Text;
            model.InvoiceTitleFilialeId = new Guid(ddl_InvoiceTitle.SelectedValue);
            model.ReportMemo = txt_ReportMemo.Text;

            model.ReportFilialeId = personnelInfo.FilialeId;
            model.ReportBranchId = personnelInfo.BranchId;
            model.ReportPersonnelId = personnelInfo.PersonnelId;
            model.State = (int)CostReportState.Auditing;
            if (string.IsNullOrEmpty(Request.QueryString["ReportId"]))
            {
                model.ReportDate = DateTime.Now;
            }
            model.Memo = WebControl.RetrunUserAndTime("[【凭证报销】：" + txt_ReportMemo.Text + ";]");
            model.IsLastTime = true;
            model.IsSystem = false;
            model.ApplyNumber = 1;
            model.IsEnd = false;
        }

        //差旅费
        protected void EditTravelList(List<CostReportTravelInfo> costReportTravelInfoList)
        {
            if (!string.IsNullOrEmpty(txt_TrainChargeNum.Text) || !string.IsNullOrEmpty(txt_TrainChargeAmount.Text))
            {
                CostReportTravelInfo model = new CostReportTravelInfo();
                model.Entourage = txt_Entourage.Text;
                model.TravelAddressAndCourse = txt_TravelAddressAndCourse.Text;
                model.Matter = 0;
                model.TravelId = Guid.NewGuid();
                model.DayOrNum = string.IsNullOrEmpty(txt_TrainChargeNum.Text) ? 0 : decimal.Parse(txt_TrainChargeNum.Text);
                model.Amount = string.IsNullOrEmpty(txt_TrainChargeAmount.Text) ? 0 : decimal.Parse(txt_TrainChargeAmount.Text);
                model.OperatingTime = DateTime.Now;
                costReportTravelInfoList.Add(model);
            }

            if (!string.IsNullOrEmpty(txt_CarFeeNum.Text) || !string.IsNullOrEmpty(txt_CarFeeAmount.Text))
            {
                CostReportTravelInfo model = new CostReportTravelInfo();
                model.Entourage = txt_Entourage.Text;
                model.TravelAddressAndCourse = txt_TravelAddressAndCourse.Text;
                model.Matter = 1;
                model.TravelId = Guid.NewGuid();
                model.DayOrNum = string.IsNullOrEmpty(txt_CarFeeNum.Text) ? 0 : decimal.Parse(txt_CarFeeNum.Text);
                model.Amount = string.IsNullOrEmpty(txt_CarFeeAmount.Text) ? 0 : decimal.Parse(txt_CarFeeAmount.Text);
                model.OperatingTime = DateTime.Now;
                costReportTravelInfoList.Add(model);
            }

            if (!string.IsNullOrEmpty(txt_CityFeeNum.Text) || !string.IsNullOrEmpty(txt_CityFeeAmount.Text))
            {
                CostReportTravelInfo model = new CostReportTravelInfo();
                model.Entourage = txt_Entourage.Text;
                model.TravelAddressAndCourse = txt_TravelAddressAndCourse.Text;
                model.Matter = 2;
                model.TravelId = Guid.NewGuid();
                model.DayOrNum = string.IsNullOrEmpty(txt_CityFeeNum.Text) ? 0 : decimal.Parse(txt_CityFeeNum.Text);
                model.Amount = string.IsNullOrEmpty(txt_CityFeeAmount.Text) ? 0 : decimal.Parse(txt_CityFeeAmount.Text);
                model.OperatingTime = DateTime.Now;
                costReportTravelInfoList.Add(model);
            }

            if (!string.IsNullOrEmpty(txt_TollsNum.Text) || !string.IsNullOrEmpty(txt_TollsAmount.Text))
            {
                CostReportTravelInfo model = new CostReportTravelInfo();
                model.Entourage = txt_Entourage.Text;
                model.TravelAddressAndCourse = txt_TravelAddressAndCourse.Text;
                model.Matter = 3;
                model.TravelId = Guid.NewGuid();
                model.DayOrNum = string.IsNullOrEmpty(txt_TollsNum.Text) ? 0 : decimal.Parse(txt_TollsNum.Text);
                model.Amount = string.IsNullOrEmpty(txt_TollsAmount.Text) ? 0 : decimal.Parse(txt_TollsAmount.Text);
                model.OperatingTime = DateTime.Now;
                costReportTravelInfoList.Add(model);
            }

            if (!string.IsNullOrEmpty(txt_AircraftFeeNum.Text) || !string.IsNullOrEmpty(txt_AircraftFeeAmount.Text))
            {
                CostReportTravelInfo model = new CostReportTravelInfo();
                model.Entourage = txt_Entourage.Text;
                model.TravelAddressAndCourse = txt_TravelAddressAndCourse.Text;
                model.Matter = 4;
                model.TravelId = Guid.NewGuid();
                model.DayOrNum = string.IsNullOrEmpty(txt_AircraftFeeNum.Text) ? 0 : decimal.Parse(txt_AircraftFeeNum.Text);
                model.Amount = string.IsNullOrEmpty(txt_AircraftFeeAmount.Text) ? 0 : decimal.Parse(txt_AircraftFeeAmount.Text);
                model.OperatingTime = DateTime.Now;
                costReportTravelInfoList.Add(model);
            }

            if (!string.IsNullOrEmpty(txt_MealsDays.Text) || !string.IsNullOrEmpty(txt_MealsAmount.Text))
            {
                CostReportTravelInfo model = new CostReportTravelInfo();
                model.Entourage = txt_Entourage.Text;
                model.TravelAddressAndCourse = txt_TravelAddressAndCourse.Text;
                model.Matter = 5;
                model.TravelId = Guid.NewGuid();
                model.DayOrNum = string.IsNullOrEmpty(txt_MealsDays.Text) ? 0 : decimal.Parse(txt_MealsDays.Text);
                model.Amount = string.IsNullOrEmpty(txt_MealsAmount.Text) ? 0 : decimal.Parse(txt_MealsAmount.Text);
                model.OperatingTime = DateTime.Now;
                costReportTravelInfoList.Add(model);
            }

            if (!string.IsNullOrEmpty(txt_AccommodationDays.Text) || !string.IsNullOrEmpty(txt_AccommodationAmount.Text))
            {
                CostReportTravelInfo model = new CostReportTravelInfo();
                model.Entourage = txt_Entourage.Text;
                model.TravelAddressAndCourse = txt_TravelAddressAndCourse.Text;
                model.Matter = 6;
                model.TravelId = Guid.NewGuid();
                model.DayOrNum = string.IsNullOrEmpty(txt_AccommodationDays.Text) ? 0 : decimal.Parse(txt_AccommodationDays.Text);
                model.Amount = string.IsNullOrEmpty(txt_AccommodationAmount.Text) ? 0 : decimal.Parse(txt_AccommodationAmount.Text);
                model.OperatingTime = DateTime.Now;
                costReportTravelInfoList.Add(model);
            }

            if (!string.IsNullOrEmpty(txt_OtherDays.Text) || !string.IsNullOrEmpty(txt_OtherAmount.Text))
            {
                CostReportTravelInfo model = new CostReportTravelInfo();
                model.Entourage = txt_Entourage.Text;
                model.TravelAddressAndCourse = txt_TravelAddressAndCourse.Text;
                model.Matter = 7;
                model.TravelId = Guid.NewGuid();
                model.DayOrNum = string.IsNullOrEmpty(txt_OtherDays.Text) ? 0 : decimal.Parse(txt_OtherDays.Text);
                model.Amount = string.IsNullOrEmpty(txt_OtherAmount.Text) ? 0 : decimal.Parse(txt_OtherAmount.Text);
                model.OperatingTime = DateTime.Now;
                costReportTravelInfoList.Add(model);
            }
        }
        #endregion

        /// <summary>
        /// 上传图片
        /// </summary>
        /// <returns></returns>
        protected string UploadVoucher(string imagePath)
        {
            //判断是否已经上传文件
            if (UploadImg.HasFile && !string.IsNullOrEmpty(UploadImgName.Text))
            {
                if (UploadImg.PostedFile != null && UploadImg.PostedFile.ContentLength > 0)
                {
                    #region 如果上传文件已经存在，则删除该文件
                    if (!string.IsNullOrEmpty(imagePath) && File.Exists(Server.MapPath(imagePath)))
                    {
                        File.Delete(Server.MapPath(imagePath));
                    }
                    #endregion

                    string ext = Path.GetExtension(UploadImg.PostedFile.FileName).ToLower();
                    if (ext == ".jpg" || ext == ".jepg" || ext == ".bmp" || ext == ".gif" || ext == ".png")
                    {
                        string fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + ext;
                        string folderPath = string.Format("~/UserDir/CostReport/CostsClassImg/{0}", DateTime.Now.ToString("yyyyMM") + "/");
                        if (!Directory.Exists(Server.MapPath(folderPath)))
                        {
                            Directory.CreateDirectory(Server.MapPath(folderPath));
                        }
                        string filePath = folderPath + fileName;
                        UploadImg.PostedFile.SaveAs(Server.MapPath(filePath));
                        return filePath;
                    }
                    MessageBox.AppendScript(this, "图片格式错误（.jpg|.jepg|.bmp|.gif|.png）！");
                }
                else
                {
                    MessageBox.AppendScript(this, "请选择一张图片！");
                }
            }
            return string.Empty;
        }

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
            var companyClassId = ddl_CompanyClass.SelectedValue;
            var companyId = ddl_FeeType.SelectedValue;
            if (string.IsNullOrEmpty(companyClassId) || string.IsNullOrEmpty(companyId))
            {
                errorMsg.Append("请选择“费用分类”！").Append("\\n");
            }
            var reportName = txt_ReportName.Text;
            if (string.IsNullOrEmpty(reportName))
            {
                errorMsg.Append("请填写“费用名称”！").Append("\\n");
            }
            var startTime = txt_StartTime.Text;
            var endTime = txt_EndTime.Text;
            if (string.IsNullOrEmpty(startTime) || string.IsNullOrEmpty(endTime))
            {
                errorMsg.Append("请选择“费用实际发生时间”！").Append("\\n");
            }
            var payCompany = txt_PayCompany.Text;
            if (string.IsNullOrEmpty(payCompany))
            {
                errorMsg.Append("请填写“收款单位”！").Append("\\n");
            }
            var reportCost = txt_ReportCost.Text;
            if (string.IsNullOrEmpty(reportCost))
            {
                errorMsg.Append("请填写“申报金额”！").Append("\\n");
            }
            var reportMemo = txt_ReportMemo.Text;
            if (string.IsNullOrEmpty(reportMemo))
            {
                errorMsg.Append("请填写“申报说明”！");
            }
            #endregion

            #region 结算方式相关
            var rblCostType = rbl_CostType.SelectedValue;
            if (!string.IsNullOrEmpty(rblCostType) && rblCostType.Equals("2"))
            {
                var bankName = txt_BankName.Text;
                if (string.IsNullOrEmpty(bankName))
                {
                    errorMsg.Append("请填写“收款银行”！").Append("\\n");
                }
                else if (bankName.IndexOf("银行", StringComparison.Ordinal) > -1)
                {
                    var subBankName = txt_SubBankName.Text;
                    if (string.IsNullOrEmpty(subBankName))
                    {
                        errorMsg.Append("请填写“收款支行”！").Append("\\n");
                    }
                }
                var bankAccount = txt_BankAccount.Text;
                if (string.IsNullOrEmpty(bankAccount))
                {
                    errorMsg.Append("请填写“收款账号”！").Append("\\n");
                }
            }
            #endregion

            #region 票据类型相关
            string errMsg = string.Empty;
            var invoiceTitle = ddl_InvoiceTitle.SelectedValue;
            if (int.Parse(rbl_InvoiceType.SelectedValue).Equals((int)CostReportInvoiceType.Invoice) || int.Parse(rbl_InvoiceType.SelectedValue).Equals((int)CostReportInvoiceType.VatInvoice))
            {
                errMsg = "发票";
                if (string.IsNullOrEmpty(invoiceTitle))
                {
                    errorMsg.Append("请选择“发票抬头”！").Append("\\n");
                }
            }
            else if (int.Parse(rbl_InvoiceType.SelectedValue).Equals((int)CostReportInvoiceType.Voucher))
            {
                errMsg = "收据";
                if (string.IsNullOrEmpty(invoiceTitle))
                {
                    errorMsg.Append("请选择“收据抬头”！").Append("\\n");
                }
            }

            if (CostReportBillInfoList.Where(p => !p.BillId.Equals(Guid.Empty)).ToList().Count==0)
            {
                errorMsg.Append("请添加“" + errMsg + "”信息！").Append("\\n");
            }
            else
            {
                var sumTaxAmount = CostReportBillInfoList.Where(p => !p.BillId.Equals(Guid.Empty)).Sum(p => p.TaxAmount);
                if (decimal.Parse(reportCost) > sumTaxAmount)
                {
                    errorMsg.Append("票据总额应该大于等于“申报金额”！").Append("\\n");
                }
            }
            #endregion

            #region 费用种类相关
            var costsClass = Hid_CostsClass.Value;
            if (!string.IsNullOrEmpty(costsClass))
            {
                if (costsClass.Equals(((int)CostsVarieties.AdvertisingFee).ToString()))
                {
                    var uploadName = UploadImgName.Text;
                    if (string.IsNullOrEmpty(uploadName))
                    {
                        errorMsg.Append("请上传“广告使用图片”！").Append("\\n");
                    }
                }
                else if (costsClass.Equals(((int)CostsVarieties.BuyArticle).ToString()))
                {
                    var goodsCode = txt_GoodsCode.Text;
                    if (string.IsNullOrEmpty(goodsCode) && decimal.Parse(txt_ReportCost.Text) >= 500)
                    {
                        errorMsg.Append("请填写“人事部物品管理编码”！").Append("\\n");
                    }
                }
            }
            #endregion

            #region 差旅费
            var selectedValue = ddl_FeeType.SelectedValue;
            if (!string.IsNullOrEmpty(selectedValue) && new Guid(selectedValue).Equals(new Guid(CostReport_TravelExpenses)))
            {
                if (!_costReportTravelInfoList.Any())
                {
                    errorMsg.Append("请填写“差旅费项目”！").Append("\\n");
                }
                var removeTerminiItem = CostReportTerminiInfoList.FirstOrDefault(p => p.TerminiId.Equals(Guid.Empty));
                CostReportTerminiInfoList.Remove(removeTerminiItem);
                if (!CostReportTerminiInfoList.Any())
                {
                    errorMsg.Append("请添加“起讫”！").Append("\\n");
                }

                #region 验证起讫
                var resultTerminiMsg = SaveTermini();
                if (!string.IsNullOrEmpty(resultTerminiMsg))
                {
                    errorMsg.Append(resultTerminiMsg).Append("\\n");
                }
                #endregion
            }
            #endregion

            #region 验证票据信息
            var resultMsg = SaveBill();
            if (!string.IsNullOrEmpty(resultMsg))
            {
                errorMsg.Append(resultMsg).Append("\\n");
            }
            #endregion

            return errorMsg.ToString();
        }

        #region Change方法

        //票据类型
        protected void InvoiceTypeChange()
        {
            if (int.Parse(rbl_InvoiceType.SelectedValue).Equals((int)CostReportInvoiceType.Invoice) || int.Parse(rbl_InvoiceType.SelectedValue).Equals((int)CostReportInvoiceType.VatInvoice))
            {
                InvoiceType.InnerText = lit_Title.Text = "发票";
                InvoiceTitle.InnerHtml = "发票抬头：";

                BillNo.InnerText = "发票号码";
                BillCode.Visible = true;
                TaxAmount.InnerText = "含税金额";

                if (int.Parse(rbl_InvoiceType.SelectedValue).Equals((int)CostReportInvoiceType.Invoice))
                {
                    NoTaxAmount.Visible = Tax.Visible = false;
                    Template.HRef = "../App_Themes/费用申报票据模板(普通发票).xls";
                }
                else if (int.Parse(rbl_InvoiceType.SelectedValue).Equals((int)CostReportInvoiceType.VatInvoice))
                {
                    NoTaxAmount.Visible = Tax.Visible = true;
                    Template.HRef = "../App_Themes/费用申报票据模板(增值税专用发票).xls";
                }

                ddl_InvoiceTitle.Items.Remove(new ListItem("ERP公司", ErpFiliale));
            }
            else if (int.Parse(rbl_InvoiceType.SelectedValue).Equals((int)CostReportInvoiceType.Voucher))
            {
                InvoiceType.InnerText = lit_Title.Text = "收据";
                InvoiceTitle.InnerHtml = "收据抬头：";
                Template.HRef = "../App_Themes/费用申报票据模板(收据).xls";

                BillNo.InnerText = "收据号码";
                BillCode.Visible = false;
                TaxAmount.InnerText = "金额";
                NoTaxAmount.Visible = Tax.Visible = false;

                ddl_InvoiceTitle.Items.Add(new ListItem("ERP公司", ErpFiliale));
            }
        }

        //费用分类子类
        protected void FeeTypeChange()
        {
            var selectedValue = ddl_FeeType.SelectedValue;
            if (!string.IsNullOrEmpty(selectedValue) && new Guid(selectedValue).Equals(new Guid(CostReport_TravelExpenses)))
            {
                AddTermini();//增加起讫
                ddl_FeeType.Style["color"] = "red";
                ddl_FeeType.Style["font-weight"] = "bold";
                MessageBox.AppendScript(this, "$(\"#TraveA\").show();$(\"#TraveDetail\").show();$(\"input[id$='Hid_Travel']\").val(\"2\");");
            }
            else
            {
                ddl_FeeType.Style["color"] = "";
                ddl_FeeType.Style["font-weight"] = "";
                ClearTraveData();//清空差旅费相关数据
                CostReportTerminiInfoList = new List<CostReportTerminiInfo>();
                MessageBox.AppendScript(this, "$(\"#TraveA\").hide();$(\"#TraveDetail\").hide();$(\"input[id$='Hid_Travel']\").val(\"\");");
            }
        }

        //申报金额
        protected void ReportCostChange()
        {
            if (!string.IsNullOrEmpty(txt_ReportCost.Text))
            {
                Lit_CapitalAmount.Text = txt_ReportCost.Text;
                var selectedFeeType = ddl_FeeType.SelectedValue;
                if (!string.IsNullOrEmpty(selectedFeeType))
                {
                    if (selectedFeeType.Equals(CostReport_FacilityEquipmentSoftware))
                    {
                        if (decimal.Parse(txt_ReportCost.Text) >= 500)
                        {
                            Lit_CapitalAmount.Text += " (<b style='color:red;'>物品到达后，请到人事部登记。</b>)";
                        }
                    }
                }
            }
        }
        #endregion

        //费用种类
        protected void CostsClass(string selectedFeeType)
        {
            if (!string.IsNullOrEmpty(selectedFeeType))
            {
                if (selectedFeeType.Equals(CostReport_FacilityEquipmentSoftware))
                {
                    Hid_CostsClass.Value = ((int)CostsVarieties.BuyArticle).ToString();
                }
                else if (selectedFeeType.Equals(CostReport_PromotionExpenses))
                {
                    Hid_CostsClass.Value = ((int)CostsVarieties.AdvertisingFee).ToString();
                }
                else
                {
                    Hid_CostsClass.Value = ((int)CostsVarieties.PureCost).ToString();
                }
            }
        }

        //清空差旅费相关数据
        protected void ClearTraveData()
        {
            txt_Entourage.Text =
            txt_TravelAddressAndCourse.Text =
            txt_TrainChargeNum.Text =
            txt_TrainChargeAmount.Text =
            txt_MealsDays.Text =
            txt_MealsAmount.Text =
            txt_CarFeeNum.Text =
            txt_CarFeeAmount.Text =
            txt_AccommodationDays.Text =
            txt_AccommodationAmount.Text =
            txt_CityFeeNum.Text =
            txt_CityFeeAmount.Text =
            txt_TollsNum.Text =
            txt_TollsAmount.Text =
            txt_AircraftFeeNum.Text =
            txt_AircraftFeeAmount.Text =
            txt_OtherDays.Text =
            txt_OtherAmount.Text = string.Empty;
        }
    }
}