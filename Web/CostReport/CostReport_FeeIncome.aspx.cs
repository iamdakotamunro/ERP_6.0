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
using Telerik.Web.UI;

namespace ERP.UI.Web.CostReport
{
    public partial class CostReport_FeeIncome : WindowsPage
    {
        private static readonly OperationLogManager _operationLogManager = new OperationLogManager();
        private static readonly ICost _costDao = new Cost(GlobalConfig.DB.FromType.Read);
        private static readonly ICostCussent _costCussentDao = new CostCussent(GlobalConfig.DB.FromType.Read);
        private static readonly IBankAccounts _bankAccounts = new BankAccounts(GlobalConfig.DB.FromType.Read);
        private static readonly ICostReport _costReport = new DAL.Implement.Inventory.CostReport(GlobalConfig.DB.FromType.Write);

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
        /// 结算账号列表属性
        /// </summary>
        protected Dictionary<Guid, string> BankAccountInfoDic
        {
            get
            {
                if (ViewState["BankAccountInfoDic"] == null)
                {
                    return _bankAccounts.GetBankAccountsList(Personnel.FilialeId, Personnel.BranchId, Personnel.PositionId).Where(ent => ent.IsUse).ToDictionary(p => p.BankAccountsId, p => p.BankName + "【" + p.AccountsName + "】");
                }
                return ViewState["BankAccountInfoDic"] as Dictionary<Guid, string>;
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

        //结算账号
        protected void LoadPayBankAccountData()
        {
            rcb_PayBankAccount.DataSource = BankAccountInfoDic;
            rcb_PayBankAccount.DataTextField = "Value";
            rcb_PayBankAccount.DataValueField = "Key";
            rcb_PayBankAccount.DataBind();
            rcb_PayBankAccount.Items.Insert(0, new RadComboBoxItem("请选择", ""));
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
            LoadPayBankAccountData();//发票抬头
            //初始化日期
            txt_EndTime.Text = txt_StartTime.Text = DateTime.Now.ToString("yyyy年MM月");
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
            LoadPayBankAccountData();//加载结算账号
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
            }

            if (!model.PayBankAccountId.Equals(Guid.Empty))
            {
                rcb_PayBankAccount.SelectedValue = model.PayBankAccountId.ToString();
                txt_AssumeFiliale.Text = CacheCollection.Filiale.GetFilialeNameAndFilialeId(model.PayBankAccountId).Split(',')[0];
                Hid_AssumeFiliale.Value = model.AssumeFilialeId.Equals(Guid.Empty) ? string.Empty : model.AssumeFilialeId.ToString();
            }
            txt_ReportMemo.Text = model.ReportMemo;
            if (!string.IsNullOrEmpty(model.AuditingMemo))
            {
                AuditingMemo.Visible = true;
                lit_AuditingMemo.Text = model.AuditingMemo;
            }
            txt_Memo.Text = model.Memo;

            ReportProcess.Visible = true;//修改时显示“申报流程”
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
            }
            else
            {
                ddl_FeeType.SelectedValue = string.Empty;
            }
            Hid_CostsClass.Value = string.Empty;
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
        }

        //结算账号搜索
        protected void rcb_PayBankAccount_ItemsRequested(object sender, RadComboBoxItemsRequestedEventArgs e)
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

        //结算账号选择
        protected void rcb_PayBankAccount_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedValue = rcb_PayBankAccount.SelectedValue;
            if (!string.IsNullOrEmpty(selectedValue) && new Guid(selectedValue) != Guid.Empty)
            {
                var filialeNameAndFilialeId = CacheCollection.Filiale.GetFilialeNameAndFilialeId(new Guid(selectedValue));
                txt_AssumeFiliale.Text = filialeNameAndFilialeId.Split(',')[0];
                Hid_AssumeFiliale.Value = filialeNameAndFilialeId.Split(',')[1];
            }
            else
            {
                txt_AssumeFiliale.Text = string.Empty;
                Hid_AssumeFiliale.Value = string.Empty;
            }
        }

        //申报金额转换成大写形式
        protected void txt_ReportCost_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txt_ReportCost.Text))
            {
                Lit_CapitalAmount.Text = txt_ReportCost.Text;
            }
        }
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
                    model.State != (int)CostReportState.AuditingNoPass)
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
                    if (string.IsNullOrEmpty(reportId))//添加
                    {
                        string errorMessage;
                        var result = _costReport.InsertReport(model, out errorMessage);
                        if (!result)
                        {
                            MessageBox.Show(this, "申报失败！" + errorMessage);
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
            }
            #endregion
        }

        #region 编辑Model
        //费用申报(lmshop_CostReport)
        protected void EditCostReportModel(CostReportInfo model, PersonnelInfo personnelInfo)
        {
            model.ReportNo = new CodeManager().GetCode(CodeType.RE);
            model.ReportKind = (int)CostReportKind.FeeIncome;
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
            model.RealityCost = model.ReportCost;
            model.ApplyForCost = model.ReportCost;
            model.ActualAmount = model.ReportCost;
            model.CostType = int.Parse(rbl_CostType.SelectedValue);
            model.Deposit = 2;
            if (model.CostType.Equals(2))
            {
                model.BankAccountName = txt_BankName.Text.Trim() + "," + txt_SubBankName.Text.Trim();
            }
            model.InvoiceType = (int)CostReportInvoiceType.WaitCheck;
            model.PayBankAccountId = string.IsNullOrEmpty(rcb_PayBankAccount.SelectedValue) ? Guid.Empty : new Guid(rcb_PayBankAccount.SelectedValue); //结算账号
            model.AssumeFilialeId = string.IsNullOrEmpty(Hid_AssumeFiliale.Value) ? Guid.Empty : new Guid(Hid_AssumeFiliale.Value); //结算公司
            model.ReportMemo = txt_ReportMemo.Text;

            model.ReportFilialeId = personnelInfo.FilialeId;
            model.ReportBranchId = personnelInfo.BranchId;
            model.ReportPersonnelId = personnelInfo.PersonnelId;
            model.State = (int)CostReportState.Auditing;
            if (string.IsNullOrEmpty(Request.QueryString["ReportId"]))
            {
                model.ReportDate = DateTime.Now;
            }
            model.Memo = WebControl.RetrunUserAndTime("[【费用收入】：" + txt_ReportMemo.Text + ";]");
            model.IsLastTime = true;
            model.IsSystem = false;
            model.ApplyNumber = 1;
            model.IsEnd = false;

            if (!string.IsNullOrEmpty(rcb_PayBankAccount.SelectedValue))
            {
                var bankAccountInfo = _bankAccounts.GetBankAccounts(new Guid(rcb_PayBankAccount.SelectedValue));
                if (bankAccountInfo != null && bankAccountInfo.IsMain)
                {
                    model.IsOut = true;
                }
                else
                {
                    model.IsOut = false;
                }
            }
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
                errorMsg.Append("请填写“申报金额”！").Append("\\n");
            }
            var reportMemo = txt_ReportMemo.Text;
            if (string.IsNullOrEmpty(reportMemo))
            {
                errorMsg.Append("请填写“申报说明”！");
            }
            #endregion

            #region 结算账号/公司
            var bankAccount = rcb_PayBankAccount.SelectedValue;
            if (string.IsNullOrEmpty(bankAccount))
            {
                errorMsg.Append("请选择“结算账号”！").Append("\\n");
            }
            var assumeFilialeId = Hid_AssumeFiliale.Value;
            if (string.IsNullOrEmpty(assumeFilialeId))
            {
                errorMsg.Append("请填写“结算公司”！").Append("\\n");
            }
            #endregion

            return errorMsg.ToString();
        }

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