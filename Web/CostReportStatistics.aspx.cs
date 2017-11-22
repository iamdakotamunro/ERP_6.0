using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using ERP.BLL.Implement;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Environment;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Keede.Ecsoft.Model;
using Telerik.Web.UI;

//================================================
// 功能：费用申报统计
// 作者：刘彩军
// 时间：2011-April-19th
//================================================
namespace ERP.UI.Web
{
    public partial class CostReportStatistics : BasePage
    {
        private readonly ICostReport _costReport=new DAL.Implement.Inventory.CostReport(GlobalConfig.DB.FromType.Read);
        private readonly ICostCussent _costCussentDao=new CostCussent(GlobalConfig.DB.FromType.Read);
        private readonly ICost _costDao=new Cost(GlobalConfig.DB.FromType.Read);
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                SearchType = "Default";
                LoadData();
            }
        }

        private void LoadData()
        {
            var list = CacheCollection.Filiale.GetHeadList();
            list.Add(new FilialeInfo { ID = new Guid(ConfigManage.GetConfigValue("RECKONING_ELSE_FILIALEID")), Name = "ERP公司" });
            RCB_Filliale.DataSource = list;
            RCB_Filliale.DataValueField = "ID";
            RCB_Filliale.DataTextField = "Name";
            RCB_Filliale.DataBind();
            RCB_Filliale.Items.Insert(0, new RadComboBoxItem(string.Empty, Guid.Empty.ToString()));


            RCB_Branch.DataSource = CacheCollection.Branch.GetSystemBranchList().Where(act => act.ParentID == Guid.Empty).ToList();
            RCB_Branch.DataValueField = "ID";
            RCB_Branch.DataTextField = "Name";
            RCB_Branch.DataBind();
            RCB_Branch.Items.Insert(0, new RadComboBoxItem(string.Empty, Guid.Empty.ToString()));

            RCB_CompanyClass.DataValueField = "CompanyClassId";
            RCB_CompanyClass.DataTextField = "CompanyClassName";
            RCB_CompanyClass.DataSource = _costDao.GetCompanyClassList().ToList();
            RCB_CompanyClass.DataBind();
            RCB_CompanyClass.Items.Insert(0, new RadComboBoxItem(string.Empty, Guid.Empty.ToString()));

            RDP_StartTime.SelectedDate = DateTime.Now.Date.AddDays(-30);
            RDP_EndTime.SelectedDate = DateTime.Now;
        }

        #region[设置列表数据源]
        protected void RG_Report_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            IList<CostReportInfo> costReportInfoList;
            if (SearchType == "Search")
            {
                costReportInfoList = _costReport.GetReportStatistics(StartTime, EndTime, new Guid(SearchClass), new Guid(SearchBranch), new Guid(SearchFiliale));
            }
            else
            {
                if (StartTime == DateTime.MinValue)
                {
                    StartTime = DateTime.Now.Date.AddDays(-30);
                }
                if (EndTime == DateTime.MinValue)
                {
                    EndTime = DateTime.Now;
                }
                costReportInfoList = _costReport.GetReportStatisticsByBranch(StartTime, EndTime, new Guid(SearchBranch), new Guid(SearchFiliale));
            }

            //合计金额
            var sumPayCost = RG_Report.MasterTableView.Columns.FindByUniqueName("PayCost");
            if (costReportInfoList.Count > 0)
            {
                var total = costReportInfoList.Sum(ent => Math.Abs(ent.RealityCost));
                sumPayCost.FooterText = string.Format("合计：{0}", WebControl.NumberSeparator(total));
            }
            else
            {
                sumPayCost.FooterText = string.Empty;
            }
            RG_Report.DataSource = costReportInfoList;
        }
        #endregion

        #region[Ajax页面返回]
        protected void RAM_AjaxRequest(object sender, AjaxRequestEventArgs e)
        {
            WebControl.RamAjajxRequest(RG_Report, e);
        }
        #endregion

        #region[搜索部门]
        public String SearchBranch
        {
            get
            {
                if (ViewState["SearchBranch"] == null)
                    return Guid.Empty.ToString();
                return ViewState["SearchBranch"].ToString();
            }
            set
            {
                ViewState["SearchBranch"] = value;
            }
        }
        #endregion

        #region[搜索分类]
        public String SearchClass
        {
            get
            {
                if (ViewState["SearchClass"] == null)
                    return Guid.Empty.ToString();
                return ViewState["SearchClass"].ToString();
            }
            set
            {
                ViewState["SearchClass"] = value;
            }
        }
        #endregion

        /// <summary>搜索公司
        /// </summary>
        public String SearchFiliale
        {
            get
            {
                if (ViewState["SearchFiliale"] == null)
                    return Guid.Empty.ToString();
                return ViewState["SearchFiliale"].ToString();
            }
            set
            {
                ViewState["SearchFiliale"] = value;
            }
        }

        #region[时间段]
        protected DateTime StartTime
        {
            get
            {
                if (ViewState["StartTime"] == null) return DateTime.MinValue;
                return Convert.ToDateTime(ViewState["StartTime"]);
            }
            set
            {
                ViewState["StartTime"] = value;
            }
        }

        protected DateTime EndTime
        {
            get
            {
                if (ViewState["EndTime"] == null) return DateTime.MinValue;
                return Convert.ToDateTime(Convert.ToDateTime(ViewState["EndTime"]).AddDays(1).ToString("yyyy-MM-dd 00:00:00"));
            }
            set
            {
                ViewState["EndTime"] = value;
            }
        }
        #endregion

        #region[搜索类型]
        public String SearchType
        {
            get
            {
                if (ViewState["SearchType"] == null)
                    return "Default";
                return ViewState["SearchType"].ToString();
            }
            set
            {
                ViewState["SearchType"] = value;
            }
        }
        #endregion

        #region[获取分类]
        public string GetClass(object companyId)
        {
            if(new Guid(companyId.ToString())==Guid.Empty)
            {
                return string.Empty;
            }
            CostCussentInfo info = _costCussentDao.GetCompanyCussent(new Guid(companyId.ToString()));
            string className = 
                info.CompanyId != Guid.Empty ?
                info.CompanyName : _costDao.GetCompanyClass(new Guid(companyId.ToString())).CompanyClassName;
            if (string.IsNullOrEmpty(className))
            {
                className = string.Empty;
            }
            return className;
        }
        #endregion

        protected void OnClick_Search(object sender, ImageClickEventArgs e)
        {
            StartTime = RDP_StartTime.SelectedDate != null ? RDP_StartTime.SelectedDate.Value : DateTime.MinValue;
            EndTime = RDP_EndTime.SelectedDate != null ? RDP_EndTime.SelectedDate.Value : DateTime.MinValue;
            SearchFiliale = RCB_Filliale.SelectedValue;
            SearchBranch = RCB_Branch.SelectedValue;
            SearchClass = RCB_CompanyClass.SelectedValue;
            if (new Guid(SearchClass) == Guid.Empty && new Guid(SearchBranch) == Guid.Empty)
            {
                SearchType = "Default";
            }
            else
            {
                SearchType = "Search";
            }
            RG_Report.Rebind();
        }

        protected void RCB_Branch_OnSelectedIndexChanged(object o, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            HF_Branch.Value = RCB_Branch.SelectedValue;
        }

        protected void RCB_Filliale_OnSelectedIndexChanged(object o, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            HF_Filiale.Value = RCB_Filliale.SelectedValue;

        }
    }
}
