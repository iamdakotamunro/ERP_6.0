using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
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
using System.IO;
using Framework.Data;

namespace ERP.UI.Web.CostReport
{
    public partial class CostReport_Before : WindowsPage
    {
        private static readonly OperationLogManager _operationLogManager = new OperationLogManager();
        private static readonly ICost _costDao = new Cost(GlobalConfig.DB.FromType.Read);
        private static readonly ICostCussent _costCussentDao = new CostCussent(GlobalConfig.DB.FromType.Read);
        private static readonly ICostReport _costReport = new DAL.Implement.Inventory.CostReport(GlobalConfig.DB.FromType.Write);
        private static readonly ICostReportBill _costReportBill = new CostReportBillDal(GlobalConfig.DB.FromType.Write);
        private static readonly ICostReportAmount _costReportAmount = new CostReportAmountDal(GlobalConfig.DB.FromType.Write);
        readonly ICostReportAuditingPower _costReportAuditingPower = new DAL.Implement.Inventory.CostReportAuditingPower(GlobalConfig.DB.FromType.Write);

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
            var shopList = FilialeManager.GetAllianceFilialeList().Where(act => act.Rank == (int)FilialeRank.Partial && act.ShopJoinType == (int)ShopJoinType.DirectSales).OrderBy(act => act.Name);

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
            ddl_CompanyClass.SelectedValue = model.CompanyClassId.Equals(Guid.Empty) ? string.Empty : model.CompanyClassId.ToString();
            ddl_FeeType.SelectedValue = model.CompanyId.Equals(Guid.Empty) ? string.Empty : model.CompanyId.ToString();
            txt_ReportName.Text = model.ReportName;
            txt_StartTime.Text = model.StartTime.ToString("yyyy年MM月");
            txt_EndTime.Text = model.EndTime.ToString("yyyy年MM月");
            txt_PayCompany.Text = model.PayCompany;
            txt_ReportCost.Text = model.ReportCost.ToString(CultureInfo.InvariantCulture);
            Lit_CapitalAmount.Text = !string.IsNullOrEmpty(txt_ReportCost.Text) ? txt_ReportCost.Text : string.Empty;
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
            rbl_InvoiceType.SelectedValue = string.Format("{0}", model.InvoiceType);
            
            txt_ReportMemo.Text = model.ReportMemo;
            if (!string.IsNullOrEmpty(model.AuditingMemo))
            {
                AuditingMemo.Visible = true;
                lit_AuditingMemo.Text = model.AuditingMemo;
            }
            txt_Memo.Text = model.Memo;

            ReportProcess.Visible = true;//修改时显示“申报流程”

            #region 申请金额
            //获取申请金额
            CostReportAmountInfoList = _costReportAmount.GetmShop_CostReportAmountByReportId(model.ReportId).Where(p => !p.IsSystem).ToList();
            if (CostReportAmountInfoList.Any())
            {
                RepeaterAmountDataBind();
            }
            else
            {
                AddAmount();//增加申请金额
            }
            #endregion

            if (model.CompanyClassId.Equals(new Guid(CostReport_DepositMargins)))
            {
                Amount.Visible = false;
            }
            else
            {
                Amount.Visible = true;
            }

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
            InvoiceTypeChange();
            #endregion

            ddl_InvoiceTitle.SelectedValue = model.InvoiceTitleFilialeId.Equals(Guid.Empty) ? string.Empty : model.InvoiceTitleFilialeId.ToString();
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
                if (new Guid(selectedValue).Equals(new Guid(CostReport_DepositMargins)))
                {
                    Amount.Visible = false;
                    var removeAmountItem = CostReportAmountInfoList.FirstOrDefault(p => !p.IsPay);
                    CostReportAmountInfoList.Remove(removeAmountItem);
                }
                else
                {
                    Amount.Visible = true;
                    AddAmount();//增加申请金额
                }
                LoadFeeTypeData(new Guid(selectedValue));
            }
            else
            {
                Amount.Visible = false;
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
            AddNewBill();
        }

        //申报金额转换成大写形式
        protected void txt_ReportCost_TextChanged(object sender, EventArgs e)
        {
            Lit_CapitalAmount.Text = !string.IsNullOrEmpty(txt_ReportCost.Text) ? txt_ReportCost.Text : string.Empty;
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
            ReportCostChange();
        }

        #region 列表操作
        //保存申请金额
        protected void btn_AmountAdd_Click(object sender, EventArgs e)
        {
            var dataItem = (RepeaterItem)((Button)sender).Parent;
            if (dataItem != null)
            {
                var amountId = new Guid(((Button)sender).CommandName);
                var litNum = (Literal)dataItem.FindControl("lit_Num");
                var txtAmount = (TextBox)dataItem.FindControl("txt_Amount");
                var isPay = (Literal)dataItem.FindControl("lit_IsPay");
                var isSystem = (Literal)dataItem.FindControl("lit_IsSystem");

                var costReportAmountInfo = CostReportAmountInfoList.FirstOrDefault(p => p.AmountId.Equals(amountId));
                if (costReportAmountInfo != null)
                {
                    costReportAmountInfo.AmountId = amountId.Equals(Guid.Empty) ? Guid.NewGuid() : amountId;
                    costReportAmountInfo.ReportId = Guid.Empty;
                    costReportAmountInfo.Num = int.Parse(litNum.Text);
                    costReportAmountInfo.Amount = decimal.Parse(txtAmount.Text);
                    costReportAmountInfo.IsPay = bool.Parse(isPay.Text);
                    costReportAmountInfo.IsSystem = bool.Parse(isSystem.Text);

                    CostReportAmountInfoList.Remove(costReportAmountInfo);
                    CostReportAmountInfoList.Add(costReportAmountInfo);
                }
            }
            RepeaterAmountDataBind();
        }

        //增加申请金额
        protected void AddAmount()
        {
            var amountItem = CostReportAmountInfoList.FirstOrDefault(p => !p.IsPay);
            if (amountItem == null)
            {
                var maxNum = CostReportAmountInfoList.Any() ? CostReportAmountInfoList.Select(p => p.Num).Max() : 0;
                var list = CostReportAmountInfoList;
                list.Add(new CostReportAmountInfo { AmountId = Guid.Empty, Num = maxNum + 1 });
                CostReportAmountInfoList = list;
                RepeaterAmountDataBind();
            }
        }

        //保存数据时调用此方法(原因：防止当用户光标未离开表格，而直接点击保存按钮，导致数据没有保存上)
        protected string SaveAmount()
        {
            var itemCount = Repeater_Amount.Items.Count;
            if (itemCount > 0)
            {
                List<CostReportAmountInfo> list = new List<CostReportAmountInfo>();
                StringBuilder errorMsg = new StringBuilder();
                foreach (RepeaterItem item in Repeater_Amount.Items)
                {
                    if (item != null)
                    {
                        var index = item.ItemIndex + 1;
                        var litNum = (Literal)item.FindControl("lit_Num");
                        var txtAmount = ((TextBox)item.FindControl("txt_Amount"));
                        var isPay = ((Literal)item.FindControl("lit_IsPay"));
                        var isSystem = ((Literal)item.FindControl("lit_IsSystem"));

                        #region 普通发票
                        errorMsg.Clear();

                        #region 验证数据空值
                        if (string.IsNullOrEmpty(txtAmount.Text))
                        {
                            errorMsg.Append("第").Append(index).Append("行“申请金额”为空！").Append("\\n");
                        }
                        #endregion

                        #region 验证数据格式
                        decimal tryAmount;
                        if (!decimal.TryParse(txtAmount.Text, out tryAmount))
                        {
                            errorMsg.Append("第").Append(index).Append("行“申请金额”格式错误！").Append("\\n");
                        }
                        #endregion

                        if (string.IsNullOrEmpty(errorMsg.ToString()))
                        {
                            #region 保存数据
                            var costReportAmountInfo = new CostReportAmountInfo
                            {
                                AmountId = Guid.NewGuid(),
                                ReportId = Guid.Empty,
                                Num = int.Parse(litNum.Text),
                                Amount = decimal.Parse(txtAmount.Text),
                                IsPay = bool.Parse(isPay.Text),
                                IsSystem = bool.Parse(isSystem.Text)
                            };
                            list.Add(costReportAmountInfo);
                            #endregion
                        }
                        #endregion
                    }
                }
                if (string.IsNullOrEmpty(errorMsg.ToString()) && list.Any())
                {
                    CostReportAmountInfoList = list;
                    RepeaterAmountDataBind();
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
                    if (model.CompanyClassId.Equals(new Guid(CostReport_DepositMargins)))
                    {
                        #region 押金处理
                        //押金没有票据和申请金额，所以票据金额总和=申请金额总和=预估申报金额
                        model.ApplyForCost = model.ActualAmount = model.RealityCost = model.ReportCost;

                        var costReportAmountInfoModel = new CostReportAmountInfo
                        {
                            AmountId = Guid.NewGuid(),
                            ReportId = Guid.Empty,
                            Num = 1,
                            Amount = model.ReportCost,
                            IsPay = false,
                            IsSystem = false
                        };

                        CostReportAmountInfoList = new List<CostReportAmountInfo> { costReportAmountInfoModel };
                        #endregion
                    }

                    #region 申请金额
                    var removeAmountItem = CostReportAmountInfoList.FirstOrDefault(p => p.AmountId.Equals(Guid.Empty));
                    CostReportAmountInfoList.Remove(removeAmountItem);
                    if (!string.IsNullOrEmpty(reportId))//如果是修改，则先删除再插入数据
                    {
                        _costReportAmount.DeletelmShop_CostReportAmountByReportId(model.ReportId);//删除申请金额
                    }
                    if (CostReportAmountInfoList.Any())//添加申请金额
                    {
                        foreach (var item in CostReportAmountInfoList)
                        {
                            item.ReportId = model.ReportId;
                        }
                        _costReportAmount.AddBatchlmshop_CostReportAmount(CostReportAmountInfoList);
                    }
                    model.RealityCost = CostReportAmountInfoList.Where(p => !p.IsPay).Sum(p => p.Amount);
                    model.ActualAmount = CostReportAmountInfoList.Where(p => !p.IsSystem).Sum(p => p.Amount);
                    #endregion

                    #region 票据相关
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

                    if (string.IsNullOrEmpty(reportId)) //添加
                    {
                        string errorMessage;
                        var result = _costReport.InsertReport(model, out errorMessage);
                        if (!result)
                        {
                            MessageBox.Show(this, "申报失败！" + errorMessage);
                        }
                    }
                    else //修改
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
                catch (Exception ex)
                {
                    MessageBox.Show(this, "保存失败！" + ex.Message);
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
            model.ReportKind = (int)CostReportKind.Before;
            model.AssumeBranchId = string.IsNullOrEmpty(ddl_AssumeBranch.SelectedValue) ? Guid.Empty : new Guid(ddl_AssumeBranch.SelectedValue);
            model.AssumeGroupId = string.IsNullOrEmpty(ddl_AssumeGroup.SelectedValue) ? Guid.Empty : new Guid(ddl_AssumeGroup.SelectedValue);
            model.AssumeShopId = string.IsNullOrEmpty(ddl_AssumeShop.SelectedValue) ? Guid.Empty : new Guid(ddl_AssumeShop.SelectedValue);
            model.CostsVarieties = string.IsNullOrEmpty(Hid_CostsClass.Value) ? -1 : int.Parse(Hid_CostsClass.Value);
            model.GoodsCode = string.Empty;
            model.CompanyClassId = string.IsNullOrEmpty(ddl_CompanyClass.SelectedValue) ? Guid.Empty : new Guid(ddl_CompanyClass.SelectedValue);
            model.CompanyId = string.IsNullOrEmpty(ddl_FeeType.SelectedValue) ? Guid.Empty : new Guid(ddl_FeeType.SelectedValue);
            model.UrgentOrDefer = int.Parse(rbl_UrgentOrDefer.SelectedValue);
            model.UrgentReason = model.UrgentOrDefer.Equals(1) ? txt_UrgentReason.Text : string.Empty;
            model.ReportName = txt_ReportName.Text;
            model.StartTime = DateTime.Parse(txt_StartTime.Text);
            model.EndTime = DateTime.Parse(txt_EndTime.Text);
            model.PayCompany = txt_PayCompany.Text;
            model.ReportCost = Math.Abs(Decimal.Parse(txt_ReportCost.Text));
            model.CostType = int.Parse(rbl_CostType.SelectedValue);
            model.Deposit = model.CompanyClassId.Equals(new Guid(CostReport_DepositMargins)) ? 1 : 2;
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
            model.Memo = WebControl.RetrunUserAndTime("[【预借款】：" + txt_ReportMemo.Text + ";]");
            model.IsLastTime = false;
            model.IsSystem = false;
            model.ApplyNumber = 1;
            model.IsEnd = false;
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
                errorMsg.Append("请填写“预估申报金额”！").Append("\\n");
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
            var invoiceTitle = ddl_InvoiceTitle.SelectedValue;
            if (int.Parse(rbl_InvoiceType.SelectedValue).Equals((int)CostReportInvoiceType.Invoice) || int.Parse(rbl_InvoiceType.SelectedValue).Equals((int)CostReportInvoiceType.VatInvoice))
            {
                if (string.IsNullOrEmpty(invoiceTitle))
                {
                    errorMsg.Append("请选择“发票抬头”！").Append("\\n");
                }
            }
            else if (int.Parse(rbl_InvoiceType.SelectedValue).Equals((int)CostReportInvoiceType.Voucher))
            {
                if (string.IsNullOrEmpty(invoiceTitle))
                {
                    errorMsg.Append("请选择“收据抬头”！").Append("\\n");
                }
            }
            #endregion

            #region 验证票据信息
            var resultBillMsg = SaveBill();
            if (!string.IsNullOrEmpty(resultBillMsg))
            {
                errorMsg.Append(resultBillMsg).Append("\\n");
            }
            #endregion

            if (!new Guid(ddl_CompanyClass.SelectedValue).Equals(new Guid(CostReport_DepositMargins)))
            {
                #region 申请金额相关
                if (CostReportAmountInfoList.Where(p => !p.AmountId.Equals(Guid.Empty)).ToList().Count == 0)
                {
                    errorMsg.Append("请添加“申请金额”！").Append("\\n");
                }
                #endregion

                #region 验证申请金额
                var resultAmountMsg = SaveAmount();
                if (!string.IsNullOrEmpty(resultAmountMsg))
                {
                    errorMsg.Append(resultAmountMsg).Append("\\n");
                }
                #endregion
            }
            else
            {
                var removeBillItem = CostReportBillInfoList.FirstOrDefault(p => p.BillId.Equals(Guid.Empty));
                CostReportBillInfoList.Remove(removeBillItem);
                if (CostReportBillInfoList.Count != 0)
                {
                    var sumTaxAmount = CostReportBillInfoList.Where(p => !p.BillId.Equals(Guid.Empty)).Sum(p => p.TaxAmount);
                    if (decimal.Parse(reportCost) > sumTaxAmount)
                    {
                        errorMsg.Append("票据总额应该大于等于“预估申报金额”！").Append("\\n");
                    }
                }
            }

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

        //申报金额
        protected void ReportCostChange()
        {
            Lit_Msg.Text = string.Empty;
            var sumAmount = CostReportAmountInfoList.Where(p => !p.IsSystem).Sum(p => p.Amount);
            if (sumAmount >= 500)
            {
                var selectedFeeType = ddl_FeeType.SelectedValue;
                if (!string.IsNullOrEmpty(selectedFeeType))
                {
                    if (selectedFeeType.Equals(CostReport_FacilityEquipmentSoftware))
                    {
                        Lit_Msg.Text += "物品到达后，请到人事部登记。";
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
    }
}