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
using ERP.Enum.Attribute;
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
using ERP.SAL.Interface;
using ERP.SAL;

namespace ERP.UI.Web.CostReport
{
    public partial class CostReport_ReceivablesPaymentFeeIncomeDepositRecovery : WindowsPage
    {
        private static readonly OperationLogManager _operationLogManager = new OperationLogManager();
        private static readonly ICost _costDao = new Cost(GlobalConfig.DB.FromType.Read);
        private static readonly ICostCussent _costCussentDao = new CostCussent(GlobalConfig.DB.FromType.Read);
        private static readonly ICostReport _costReport = new DAL.Implement.Inventory.CostReport(GlobalConfig.DB.FromType.Write);
        private static readonly IWasteBook _wasteBook = new WasteBook(GlobalConfig.DB.FromType.Write);
        private static readonly IBankAccounts _bankAccounts = new BankAccounts(GlobalConfig.DB.FromType.Read);
        private static readonly IPersonnelSao _personnelSao = new PersonnelSao();
        private static readonly ICostReckoning _costReckoning = new CostReckoning(GlobalConfig.DB.FromType.Write);

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
                    ViewState["ERPFiliale"] = ConfigManage.GetConfigValue("RECKONING_ELSE_FILIALEID");
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
                LoadCostSortData();//加载费用分类
                if (!costReportInfo.CompanyClassId.Equals(Guid.Empty))
                {
                    LoadFeeTypeData(costReportInfo.CompanyClassId); //根据费用分类id获取“费用类型”数据
                }
                LoadAssumeFilialeData(costReportInfo);//结算公司
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

        //结算公司
        protected void LoadAssumeFilialeData(CostReportInfo model)
        {
            var list = CacheCollection.Filiale.GetHeadList();
            if (!model.AssumeFilialeId.Equals(Guid.Empty) && model.InvoiceTitleFilialeId.Equals(Guid.Empty) && !model.AssumeFilialeId.Equals(new Guid(ErpFiliale)))
            {
                list = list.Where(p => p.ID.Equals(model.AssumeFilialeId)).ToList();
            }
            else if (!model.InvoiceTitleFilialeId.Equals(new Guid(ErpFiliale)))
            {
                list = list.Where(p => p.ID.Equals(model.InvoiceTitleFilialeId)).ToList();
            }
            list.Add(new FilialeInfo { ID = new Guid(ErpFiliale), RealName = "ERP公司" });

            ddl_AssumeFiliale.DataSource = list;
            ddl_AssumeFiliale.DataTextField = "RealName";
            ddl_AssumeFiliale.DataValueField = "ID";
            ddl_AssumeFiliale.DataBind();
            ddl_AssumeFiliale.Items.Insert(0, new ListItem("请选择", ""));

            LoadPayBankAccountData(model.AssumeFilialeId);
        }

        //结算账号
        protected void LoadPayBankAccountData(Guid filialeId)
        {
            if (!filialeId.Equals(Guid.Empty))
            {
                var list = _bankAccounts.GetBankAccountsList(Personnel.FilialeId, Personnel.BranchId, Personnel.PositionId).Where(ent => ent.IsUse);

                //根据公司id查找该公司绑定的所有银行账号
                var bankAccountInfoList = BankAccountManager.ReadInstance.GetListByTargetId(filialeId);
                if (bankAccountInfoList.Any())
                {
                    //在当前登录人有权限的银行列表中，筛选出所有与发票抬头公司id相等的“结算账号”
                    var joinQuery = (from bankAccountInfo in bankAccountInfoList
                                     join listInfo in list
                                     on bankAccountInfo.BankAccountsId equals listInfo.BankAccountsId
                                     select new BankAccountInfo
                                     {
                                         BankName = listInfo.BankName,
                                         AccountsName = listInfo.AccountsName,
                                         BankAccountsId = listInfo.BankAccountsId
                                     }).ToList();
                    list = joinQuery;
                }
                else
                {
                    list = list.Where(p => !p.IsMain);
                }

                foreach (var info in list)
                {
                    var name = info.BankName + "【" + info.AccountsName + "】";
                    rcb_PayBankAccount.Items.Add(new RadComboBoxItem(name, info.BankAccountsId.ToString()));
                }
            }
            rcb_PayBankAccount.Items.Insert(0, new RadComboBoxItem("请选择", ""));
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
            rbl_UrgentOrDefer.SelectedValue = string.Format("{0}", model.UrgentOrDefer);
            txt_UrgentReason.Text = model.UrgentReason;
            Hid_CostsClass.Value = string.Format("{0}", model.CostsVarieties);
            ddl_CompanyClass.SelectedValue = model.CompanyClassId.Equals(Guid.Empty) ? string.Empty : model.CompanyClassId.ToString();
            ddl_FeeType.SelectedValue = model.CompanyId.Equals(Guid.Empty) ? string.Empty : model.CompanyId.ToString();
            txt_ReportName.Text = model.ReportName + (model.ApplyNumber > 1 ? "  " + WebControl.ConvertToChnName(model.ApplyNumber) + "期" : string.Empty);
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
                ddl_AssumeFiliale.SelectedValue = model.AssumeFilialeId.ToString();
                rcb_PayBankAccount.SelectedValue = model.PayBankAccountId.ToString();
            }
            txt_DepositNo.Text = model.DepositNo;
            txt_ReportMemo.Text = model.ReportMemo;
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

        //结算公司选择
        protected void ddl_AssumeFiliale_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedValue = ddl_AssumeFiliale.SelectedValue;
            rcb_PayBankAccount.Text = string.Empty;
            rcb_PayBankAccount.Items.Clear();
            if (!string.IsNullOrEmpty(selectedValue) && new Guid(selectedValue) != Guid.Empty)
            {
                LoadPayBankAccountData(new Guid(selectedValue));
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
        #endregion

        //付款
        protected void btn_Pay_Click(object sender, EventArgs e)
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
            if (model.State != (int)CostReportState.WaitVerify)
            {
                MessageBox.Show(this, "该单据状态已更新，不允许此操作！");
                return;
            }

            var bankAccountInfo = _bankAccounts.GetBankAccounts(new Guid(rcb_PayBankAccount.SelectedValue));

            #region 结算账号
            string payBankAccount = bankAccountInfo == null ? "暂无结算" : (bankAccountInfo.BankName + "【" + bankAccountInfo.AccountsName + "】");
            #endregion

            //费用收入(押金)
            FeeIncome(model, payBankAccount);

            #region 额外可修改的项
            UpdateItem(model);
            #endregion

            model.AssumeBranchId = string.IsNullOrEmpty(ddl_AssumeBranch.SelectedValue) ? Guid.Empty : new Guid(ddl_AssumeBranch.SelectedValue);
            model.AssumeGroupId = string.IsNullOrEmpty(ddl_AssumeGroup.SelectedValue) ? Guid.Empty : new Guid(ddl_AssumeGroup.SelectedValue);
            model.AssumeShopId = string.IsNullOrEmpty(ddl_AssumeShop.SelectedValue) ? Guid.Empty : new Guid(ddl_AssumeShop.SelectedValue);
            model.PayBankAccountId = string.IsNullOrEmpty(rcb_PayBankAccount.SelectedValue) ? Guid.Empty : new Guid(rcb_PayBankAccount.SelectedValue);
            model.AssumeFilialeId = string.IsNullOrEmpty(ddl_AssumeFiliale.SelectedValue) ? Guid.Empty : new Guid(ddl_AssumeFiliale.SelectedValue);
            model.Poundage = string.IsNullOrEmpty(txt_Poundage.Text) ? 0 : decimal.Parse(txt_Poundage.Text);
            model.TradeNo = txt_TradeNo.Text;
            model.AuditingMemo = txt_AuditingMemo.Text;
            if (bankAccountInfo != null && bankAccountInfo.IsMain)
            {
                model.IsOut = true;
            }
            else
            {
                model.IsOut = false;
            }

            #region 保存数据
            using (var ts = new TransactionScope(TransactionScopeOption.Required))
            {
                try
                {
                    _costReport.UpdateReport(model);

                    _costReport.UpdatePayCostAndExecuteDate(model.ReportId, model.ReportCost);
                    string errorMessage;
                    bool result = ExecuteFinishHandle(model, Personnel, out errorMessage);
                    if (!result)
                    {
                        throw new Exception(errorMessage);
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

        //保存数据
        protected void btn_Save_Click(object sender, EventArgs e)
        {
            #region 验证数据
            var errorMsg = CheckData();
            if (!string.IsNullOrEmpty(errorMsg))
            {
                MessageBox.Show(this, errorMsg);
                return;
            }
            #endregion

            //CostReportState.WaitVerify 此状态对于“个人”是【待付款确认】，对于“公司”是【待收款确认】
            CostReportInfo model = _costReport.GetReportByReportId(new Guid(Request.QueryString["ReportId"]));
            if (model.State != (int)CostReportState.WaitVerify)
            {
                MessageBox.Show(this, "该单据状态已更新，不允许此操作！");
                return;
            }

            #region 额外可修改的项
            UpdateItem(model);
            #endregion

            model.AssumeBranchId = string.IsNullOrEmpty(ddl_AssumeBranch.SelectedValue) ? Guid.Empty : new Guid(ddl_AssumeBranch.SelectedValue);
            model.AssumeGroupId = string.IsNullOrEmpty(ddl_AssumeGroup.SelectedValue) ? Guid.Empty : new Guid(ddl_AssumeGroup.SelectedValue);
            model.AssumeShopId = string.IsNullOrEmpty(ddl_AssumeShop.SelectedValue) ? Guid.Empty : new Guid(ddl_AssumeShop.SelectedValue);
            model.PayBankAccountId = string.IsNullOrEmpty(rcb_PayBankAccount.SelectedValue) ? Guid.Empty : new Guid(rcb_PayBankAccount.SelectedValue);
            model.AssumeFilialeId = string.IsNullOrEmpty(ddl_AssumeFiliale.SelectedValue) ? Guid.Empty : new Guid(ddl_AssumeFiliale.SelectedValue);
            model.Poundage = string.IsNullOrEmpty(txt_Poundage.Text) ? 0 : decimal.Parse(txt_Poundage.Text);
            model.TradeNo = txt_TradeNo.Text;
            model.AuditingMemo = txt_AuditingMemo.Text;
            model.Memo = WebControl.RetrunUserAndTime("[【保存数据】:已保存;收款说明:" + (string.IsNullOrEmpty(txt_AuditingMemo.Text) ? "暂无说明" : txt_AuditingMemo.Text) + ";]");
            var bankAccountInfo = _bankAccounts.GetBankAccounts(new Guid(rcb_PayBankAccount.SelectedValue));
            if (bankAccountInfo != null && bankAccountInfo.IsMain)
            {
                model.IsOut = true;
            }
            else
            {
                model.IsOut = false;
            }

            #region 保存数据
            using (var ts = new TransactionScope(TransactionScopeOption.Required))
            {
                try
                {
                    _costReport.UpdateReport(model);

                    //添加操作日志
                    _operationLogManager.Add(Personnel.PersonnelId, Personnel.RealName, model.ReportId, model.ReportNo, OperationPoint.CostDeclare.AuditDeclare.GetBusinessInfo(), 1, "");

                    ts.Complete();
                    MessageBox.AppendScript(this, "parent.SetFlag();CloseAndRebind();");
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

        #region 额外可修改的项
        protected void UpdateItem(CostReportInfo model)
        {
            model.CostsVarieties = string.IsNullOrEmpty(Hid_CostsClass.Value) ? -1 : int.Parse(Hid_CostsClass.Value);
            model.CompanyClassId = string.IsNullOrEmpty(ddl_CompanyClass.SelectedValue) ? Guid.Empty : new Guid(ddl_CompanyClass.SelectedValue);
            model.CompanyId = string.IsNullOrEmpty(ddl_FeeType.SelectedValue) ? Guid.Empty : new Guid(ddl_FeeType.SelectedValue);
            model.UrgentOrDefer = int.Parse(rbl_UrgentOrDefer.SelectedValue);
            model.UrgentReason = model.UrgentOrDefer.Equals(1) ? txt_UrgentReason.Text : string.Empty;
            model.ReportName = txt_ReportName.Text;
            model.StartTime = DateTime.Parse(txt_StartTime.Text);
            model.EndTime = DateTime.Parse(txt_EndTime.Text);
            model.PayCompany = txt_PayCompany.Text;
            model.CostType = int.Parse(rbl_CostType.SelectedValue);
            if (model.CostType.Equals(2))
            {
                model.BankAccountName = txt_BankName.Text.Trim() + "," + txt_SubBankName.Text.Trim();
            }
            model.ReportMemo = txt_ReportMemo.Text;
        }
        #endregion

        #region 费用收入(押金)
        protected void FeeIncome(CostReportInfo model, string payBankAccount)
        {
            model.State = (int)CostReportState.Complete;
            model.FinishDate = DateTime.Now;
            model.Memo = WebControl.RetrunUserAndTime("[【已收款】:已收入" + WebControl.RemoveDecimalEndZero(model.RealityCost) + "元;结算账号:" + payBankAccount + ";收款说明:" + (string.IsNullOrEmpty(txt_AuditingMemo.Text) ? "暂无说明" : txt_AuditingMemo.Text) + ";]");
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
            var costReckoningInfo = costReportBll.AddCostReckoningInfo(model, personnelInfo, true);
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
            var bankAccount = rcb_PayBankAccount.SelectedValue;
            if (string.IsNullOrEmpty(bankAccount))
            {
                errorMsg.Append("请选择“结算账号”！").Append("\\n");
            }
            var assumeFilialeId = ddl_AssumeFiliale.SelectedValue;
            if (string.IsNullOrEmpty(assumeFilialeId) || new Guid(assumeFilialeId).Equals(Guid.Empty))
            {
                errorMsg.Append("请填写“结算公司”！").Append("\\n");
            }
            var tradeNo = txt_TradeNo.Text;
            if (string.IsNullOrEmpty(tradeNo))
            {
                errorMsg.Append("请填写“交易流水号”！").Append("\\n");
            }
            var poundage = txt_Poundage.Text;
            if (!string.IsNullOrEmpty(poundage))
            {
                var result = WebControl.CheckPoundage(Decimal.Parse(txt_ReportCost.Text), Decimal.Parse(poundage));
                if (!result)
                {
                    errorMsg.Append("“手续费”【应小于等于6】或者【不超过付款金额的2%】！").Append("\\n");
                }
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