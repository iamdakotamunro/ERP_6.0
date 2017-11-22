using System;
using System.Linq;
using System.Web.UI.WebControls;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Enum.Attribute;
using ERP.Environment;
using ERP.SAL;
using ERP.SAL.Interface;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Telerik.Web.UI;
using Keede.Ecsoft.Model;
using System.Collections.Generic;
using ERP.DAL.Implement.Inventory;
using ERP.Model;
using ERP.BLL.Implement;
using System.Web.UI;
using ERP.BLL.Implement.Inventory;
using ERP.BLL.Implement.Organization;
using MIS.Enum;
using MIS.Model.View;
using FilialeInfo = Keede.Ecsoft.Model.FilialeInfo;

namespace ERP.UI.Web.CostReport
{
    /************************************************************************************ 
     * 创建人：  张安龙
     * 创建时间：2015/11/11  
     * 描述    :费用申报查询
     * =====================================================================
     * 修改时间：2015/11/11  
     * 修改人  ：  
     * 描述    ：
     */
    public partial class CostReport_SearchManage : BasePage
    {
        Guid _reportPersonnelId = Guid.Empty;
        private readonly ICostReport _costReport = new DAL.Implement.Inventory.CostReport(GlobalConfig.DB.FromType.Read);
        private static readonly IBankAccounts _bankAccounts = new BankAccounts(GlobalConfig.DB.FromType.Read);
        private readonly IPersonnelSao _personnelSao = new PersonnelSao();
        private static readonly ICost _costDao = new ERP.DAL.Implement.Inventory.Cost(GlobalConfig.DB.FromType.Read);
        private static readonly ICostCussent _costCussentDao = new CostCussent(GlobalConfig.DB.FromType.Read);

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

        /// <summary>
        /// 费用归属部门=>部门
        /// </summary>
        protected IList<SystemBranchInfo> BranchList
        {
            get
            {
                if (ViewState["BranchList"] == null)
                {
                    return CacheCollection.Branch.GetSystemBranchList();
                }
                return ViewState["BranchList"] as IList<SystemBranchInfo>;
            }
        }

        /// <summary>
        /// 费用归属部门=>余额扣除门店
        /// </summary>
        protected IList<Keede.Ecsoft.Model.FilialeInfo> ShopList
        {
            get
            {
                if (ViewState["ShopList"] == null)
                {
                    return FilialeManager.GetAllianceFilialeList().Where(act => act.Rank == (int)FilialeRank.Partial && act.ShopJoinType == (int)ShopJoinType.DirectSales).OrderBy(act => act.Name).ToList();
                }
                return ViewState["ShopList"] as IList<Keede.Ecsoft.Model.FilialeInfo>;
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
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadStateData();//处理状态
                LoadReportKindData();//申报类型
                LoadAssumeFilialeData();//结算公司
                LoadBillStateData();//票据状态
                LoadCostSortData();//加载费用分类
            }
            //判断登录用户是否有此操作的权限
            if (GetPowerOperationPoint("Review"))
            {
                ReportCost.Visible = ddlReportCost.Visible = ReviewState.Visible = ddlReviewState.Visible = true;
            }
            MessageBox.AppendScript(this, "ShowValue('" + Hid_State.Value + "');");
        }

        //查询
        protected void btn_Search_Click(object sender, EventArgs e)
        {
            Hid_Search.Value = "2";
            GridDataBind();
            RG_Report.DataBind();
        }

        #region 数据准备
        //处理状态
        protected void LoadStateData()
        {
            var list = EnumAttribute.GetDict<CostReportState>();

            #region 这几个状态，目前没有用
            list.Remove((int)CostReportState.ExecuteNoPass);
            list.Remove((int)CostReportState.PayBack);
            list.Remove((int)CostReportState.AuditingBeforePay);
            list.Remove((int)CostReportState.AuditingBeforePayNoPass);
            list.Remove((int)CostReportState.WaitReturn);
            list.Remove((int)CostReportState.Execute);
            #endregion

            list.Add(-1, "全部");

            Rcb_State.DataSource = list.OrderBy(p => p.Key);
            Rcb_State.DataTextField = "Value";
            Rcb_State.DataValueField = "Key";
            Rcb_State.DataBind();
        }

        //申报类型
        protected void LoadReportKindData()
        {
            var list = EnumAttribute.GetDict<CostReportKind>().OrderBy(p => p.Key);
            ddl_ReportKind.DataSource = list;
            ddl_ReportKind.DataTextField = "Value";
            ddl_ReportKind.DataValueField = "Key";
            ddl_ReportKind.DataBind();
            ddl_ReportKind.Items.Insert(0, new ListItem("全部", ""));
        }

        //结算公司
        protected void LoadAssumeFilialeData()
        {
            var list = CacheCollection.Filiale.GetHeadList();
            list.Add(new FilialeInfo { ID = new Guid(ConfigManage.GetConfigValue("RECKONING_ELSE_FILIALEID")), Name = "ERP公司" });
            ddl_AssumeFiliale.DataSource = list;
            ddl_AssumeFiliale.DataTextField = "Name";
            ddl_AssumeFiliale.DataValueField = "ID";
            ddl_AssumeFiliale.DataBind();
            ddl_AssumeFiliale.Items.Insert(0, new ListItem("全部", ""));
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

        //票据状态
        protected void LoadBillStateData()
        {
            var list = EnumAttribute.GetDict<CostReportBillState>();
            ddl_BillState.DataSource = list;
            ddl_BillState.DataTextField = "Value";
            ddl_BillState.DataValueField = "Key";
            ddl_BillState.DataBind();
            ddl_BillState.Items.Insert(0, new ListItem("请选择", ""));
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
        #endregion

        #region SelectedIndexChanged事件
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

        //申请人数据绑定
        protected void rcb_ReportPersonnel_ItemsRequested(object sender, RadComboBoxItemsRequestedEventArgs e)
        {
            var rcb = (RadComboBox)sender;
            rcb.Items.Clear();
            if (!string.IsNullOrEmpty(e.Text))
            {
                var personnelList = _personnelSao.GetList().Where(p => p.RealName.Contains(e.Text)).ToList();
                Int32 totalCount = personnelList.Count;
                if (e.NumberOfItems >= totalCount)
                    e.EndOfItems = true;
                else
                {
                    foreach (var item in personnelList)
                    {
                        rcb.Items.Add(new RadComboBoxItem(item.RealName, item.PersonnelId.ToString()));
                    }
                }
            }
        }
        #endregion

        #region 数据列表相关
        protected void RG_Report_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            GridDataBind();
        }

        //Grid数据源
        protected void GridDataBind()
        {
            _reportPersonnelId = Personnel.PersonnelId;
            DateTime? startTime = null, endtime;
            int reportKind = -1;
            string reportState = string.Empty, reportNo = string.Empty, reportName = string.Empty;
            if (!string.IsNullOrEmpty(Request.QueryString["IsWarning"]))//预警
            {
                Hid_TimeType.Value = "1";
                endtime = DateTime.Now.AddDays(-60);
                reportKind = (int)CostReportKind.Before;
                reportState = (int)CostReportState.NoAuditing + "," +
                              (int)CostReportState.InvoiceNoPass + "," +
                              (int)CostReportState.Auditing + "," +
                              (int)CostReportState.AuditingNoPass + "," +
                              (int)CostReportState.AlreadyAuditing + "," +
                              (int)CostReportState.WaitVerify + "," +
                              (int)CostReportState.WaitCheck + "," +
                              (int)CostReportState.Pay;
            }
            else//不是预警，默认查最近一个月的数据
            {
                if (Hid_Search.Value.Equals("1"))//默认显示 申请时间从本月1号至今的数据
                {
                    startTime = endtime = null;
                }
                else
                {
                    startTime = string.IsNullOrEmpty(txt_DateTimeStart.Text) ? DateTime.MinValue : DateTime.Parse(txt_DateTimeStart.Text);
                    endtime = string.IsNullOrEmpty(txt_DateTimeEnd.Text) ? DateTime.MinValue : DateTime.Parse(txt_DateTimeEnd.Text).AddDays(1);
                }

                if (!string.IsNullOrEmpty(ddl_ReportKind.SelectedValue))
                {
                    reportKind = int.Parse(ddl_ReportKind.SelectedValue);
                }

                #region 设置状态
                if (!string.IsNullOrEmpty(Hid_State.Value))
                {
                    reportState = Hid_State.Value;
                }
                #endregion
            }

            //如果登录人是财务部或者总裁办，则可以看到全部数据
            if (Personnel.BranchId.Equals(new Guid("89706D31-C526-47B8-ABDD-A8BA587D39B1")) || Personnel.BranchId.Equals(new Guid("C365D6E2-22EA-4295-9333-B2476351648A")))
            {
                _reportPersonnelId = Guid.Empty;
            }

            if (!_reportPersonnelId.Equals(Guid.Empty))
            {
                rcb_ReportPersonnel.EmptyMessage = Personnel.RealName;
                rcb_ReportPersonnel.Enabled = false;
            }
            if (!string.IsNullOrEmpty(txt_ReportNo.Text))
            {
                reportNo = txt_ReportNo.Text.Trim();
            }
            if (!string.IsNullOrEmpty(txt_ReportName.Text))
            {
                reportName = txt_ReportName.Text.Trim();
            }

            var costReportList = _costReport.GetCostReportForWarningsInfos(reportKind, startTime, endtime, _reportPersonnelId, reportState, reportNo, reportName, Hid_TimeType.Value, ddl_BillState.SelectedValue);
            var query = costReportList.AsQueryable();
            if (!string.IsNullOrEmpty(ddl_AssumeFiliale.SelectedValue))
            {
                query = query.Where(p => p.AssumeFilialeId.Equals(new Guid(ddl_AssumeFiliale.SelectedValue)));
            }
            if (!string.IsNullOrEmpty(rcb_PayBankAccount.SelectedValue))
            {
                query = query.Where(p => p.PayBankAccountId.Equals(new Guid(rcb_PayBankAccount.SelectedValue)));
            }
            if (!string.IsNullOrEmpty(ddl_InvoiceType.SelectedValue))
            {
                query = query.Where(p => p.InvoiceType.Equals(int.Parse(ddl_InvoiceType.SelectedValue)));
            }
            if (!string.IsNullOrEmpty(rcb_ReportPersonnel.SelectedValue) && !rcb_ReportPersonnel.SelectedValue.Equals(Guid.Empty.ToString()))
            {
                query = query.Where(p => p.ReportPersonnelId.Equals(new Guid(rcb_ReportPersonnel.SelectedValue)));
            }
            if (!string.IsNullOrEmpty(ddl_ReportCost.SelectedValue))
            {
                query = query.Where(p => p.ReportCost >= decimal.Parse(ddl_ReportCost.SelectedValue));
            }
            if (!string.IsNullOrEmpty(ddl_ReviewState.SelectedValue))
            {
                query = query.Where(p => p.ReviewState.Equals(int.Parse(ddl_ReviewState.SelectedValue)));
            }
            if (!string.IsNullOrEmpty(ddl_CompanyClass.SelectedValue))
            {
                query = query.Where(p => p.CompanyClassId.Equals(new Guid(ddl_CompanyClass.SelectedValue)));
            }
            if (!string.IsNullOrEmpty(ddl_FeeType.SelectedValue))
            {
                query = query.Where(p => p.CompanyId.Equals(new Guid(ddl_FeeType.SelectedValue)));
            }
            if (!string.IsNullOrEmpty(ddl_PayState.SelectedValue))
            {
                if (ddl_PayState.SelectedValue.Equals("0"))
                {
                    query = query.Where(p => p.PayCost == 0);
                }
                else if (ddl_PayState.SelectedValue.Equals("1"))
                {
                    query = query.Where(p => p.PayCost > 0);
                }
            }

            #region 合计
            var totalName = RG_Report.MasterTableView.Columns.FindByUniqueName("TotalName");
            var reportCost = RG_Report.MasterTableView.Columns.FindByUniqueName("ReportCost");
            var applyForCost = RG_Report.MasterTableView.Columns.FindByUniqueName("ApplyForCost");
            var payCost = RG_Report.MasterTableView.Columns.FindByUniqueName("PayCost");
            if (query.Any())
            {
                var sumReportCost = query.Sum(p => Math.Abs(p.ReportCost));
                var sumApplyForCost = query.Sum(p => Math.Abs(p.ApplyForCost));
                var sumPayCost = query.Sum(p => Math.Abs(p.PayCost));
                totalName.FooterText = "合计：";
                reportCost.FooterText = string.Format("{0}", ERP.UI.Web.Common.WebControl.NumberSeparator(sumReportCost));
                applyForCost.FooterText = string.Format("{0}", ERP.UI.Web.Common.WebControl.NumberSeparator(sumApplyForCost));
                payCost.FooterText = string.Format("{0}", ERP.UI.Web.Common.WebControl.NumberSeparator(sumPayCost));
            }
            else
            {
                totalName.FooterText = string.Empty;
                reportCost.FooterText = string.Empty;
                applyForCost.FooterText = string.Empty;
                payCost.FooterText = string.Empty;
            }
            #endregion

            RG_Report.DataSource = query.ToList();
        }

        //行绑定事件
        protected void RG_Report_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item.ItemType == GridItemType.Item || e.Item.ItemType == GridItemType.AlternatingItem)
            {
                #region “申报金额”>=1万元(绿色);“申报金额”>=10万元(黄色);“申报金额”>=100万元(紫色)
                var reportCost = DataBinder.Eval(e.Item.DataItem, "ReportCost");
                if (decimal.Parse(reportCost.ToString()) >= 1000000)
                {
                    e.Item.Style["color"] = "#CC00FF";
                }
                else if (decimal.Parse(reportCost.ToString()) >= 100000)
                {
                    e.Item.Style["color"] = "#FF9900";
                }
                else if (decimal.Parse(reportCost.ToString()) >= 10000)
                {
                    e.Item.Style["color"] = "#009900";
                }
                #endregion
            }
        }

        #region 列表显示辅助方法
        /// <summary>
        /// 获取处理状态
        /// </summary>
        /// <param name="reportState">状态</param>
        /// <param name="reportCost">申报金额</param>
        /// <returns></returns>
        protected string GetReportState(string reportState, int reportCost)
        {
            if (string.IsNullOrEmpty(reportState))
            {
                return "-";
            }

            //true:表示是财务部(即代表公司);false:表示是个人
            bool flag = _reportPersonnelId.Equals(Guid.Empty);
            var state = int.Parse(reportState);
            if (((int)CostReportState.AlreadyAuditing).Equals(state) && reportCost < 0)
            {
                return flag ? "待收款" : "待付款";
            }
            if (((int)CostReportState.AlreadyAuditing).Equals(state) && reportCost > 0)
            {
                return flag ? "待付款" : "待收款";
            }
            return EnumAttribute.GetKeyName((CostReportState)state);
        }

        /// <summary>
        /// 费用归属部门
        /// </summary>
        /// <param name="assumeBranchId">部门</param>
        /// <param name="assumeGroupId">小组</param>
        /// <param name="assumeShopId">门店</param>
        /// <returns></returns>
        protected string GetCostAttributionDepartment(Guid assumeBranchId, Guid assumeGroupId, Guid assumeShopId)
        {
            var assumeBranchName = string.Empty;
            var assumeGroupName = string.Empty;
            var assumeShopName = string.Empty;
            var systemBranchInfo = BranchList.FirstOrDefault(p => p.ID.Equals(assumeBranchId));
            if (systemBranchInfo != null)
            {
                assumeBranchName = systemBranchInfo.Name;
                var groupList = CacheCollection.Branch.GetSystemBranchListByBranchId(assumeBranchId);
                var firstOrDefault = groupList.FirstOrDefault(p => p.ID.Equals(assumeGroupId));
                if (firstOrDefault != null)
                {
                    assumeGroupName = "－"+firstOrDefault.Name;
                    var filialeInfo = ShopList.FirstOrDefault(p => p.ID.Equals(assumeShopId));
                    if (filialeInfo != null)
                        assumeShopName = "－" + filialeInfo.Name;
                }
            }
            return assumeBranchName + assumeGroupName + assumeShopName;
        }

        #endregion
        #endregion


        //导出Excel
        protected void btn_ExportExcel_Click(object sender, EventArgs e)
        {
            RG_Report.Columns[15].Visible = false;
            RG_Report.Columns[16].Visible = false;
            RG_Report.ExportSettings.ExportOnlyData = true;
            RG_Report.ExportSettings.IgnorePaging = true;
            RG_Report.ExportSettings.FileName = Server.UrlEncode("费用申报信息");
            RG_Report.MasterTableView.ExportToExcel();
        }

        /// <summary>
        /// 取得用户操作权限
        /// </summary>
        protected bool GetPowerOperationPoint(string powerName)
        {
            const string PAGE_NAME = "/CostReport/CostReport_SearchManage.aspx";
            return ERP.UI.Web.Common.WebControl.GetPowerOperationPoint(PAGE_NAME, powerName);
        }
    }
}