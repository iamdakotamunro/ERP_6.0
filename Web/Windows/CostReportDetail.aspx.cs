using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ERP.BLL.Implement.Organization;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Enum.Attribute;
using ERP.Environment;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Keede.Ecsoft.Model;
using Telerik.Web.UI;

//================================================
// 功能：费用申报
// 作者：刘彩军
// 时间：2011-February-22th
//================================================
namespace ERP.UI.Web.Windows
{
    public partial class CostReportDetail : WindowsPage
    {
        private readonly PersonnelManager _personnelManager=new PersonnelManager();
        protected decimal WarningNumber = 0;
        private readonly ICostReport _costReport = new DAL.Implement.Inventory.CostReport(GlobalConfig.DB.FromType.Read);

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!String.IsNullOrEmpty(Request.QueryString["CompanyId"]))
                {
                    CompanyId = new Guid(Request.QueryString["CompanyId"]);
                }
                if (!String.IsNullOrEmpty(Request.QueryString["FilialeId"]))
                {
                    FilialeId = new Guid(Request.QueryString["FilialeId"]);
                }
                if (!String.IsNullOrEmpty(Request.QueryString["BranchId"]))
                {
                    BranchId = new Guid(Request.QueryString["BranchId"]);
                }
                if (!String.IsNullOrEmpty(Request.QueryString["StartDate"]))
                {
                    StartTime = DateTime.Parse(Server.UrlDecode(Request.QueryString["StartDate"]));
                }
                if (!String.IsNullOrEmpty(Request.QueryString["EndDate"]))
                {
                    EndTime = DateTime.Parse(Server.UrlDecode(Request.QueryString["EndDate"]));
                }
            }
        }
        //报销类型
        protected string GetReportKindName(int reportKind)
        {
            try
            {
                return EnumAttribute.GetKeyName((CostReportKind)reportKind);
            }
            catch
            {
                return "-";
            }
        }

        #region[分类]
        public Guid CompanyId
        {
            get
            {
                if (ViewState["CompanyId"] == null)
                    return Guid.Empty;
                return new Guid(ViewState["CompanyId"].ToString());
            }
            set
            {
                ViewState["CompanyId"] = value;
            }
        }
        #endregion

        #region[部门]
        public Guid BranchId
        {
            get
            {
                if (ViewState["BranchId"] == null)
                    return Guid.Empty;
                return new Guid(ViewState["BranchId"].ToString());
            }
            set
            {
                ViewState["BranchId"] = value;
            }
        }
        #endregion

        #region[公司]
        public Guid FilialeId
        {
            get
            {
                if (ViewState["FilialeId"] == null)
                    return Guid.Empty;
                return new Guid(ViewState["FilialeId"].ToString());
            }
            set
            {
                ViewState["FilialeId"] = value;
            }
        }
        #endregion

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
                ViewState["StartTime"] = value.ToString(CultureInfo.InvariantCulture);
            }
        }

        protected DateTime EndTime
        {
            get
            {
                if (ViewState["EndTime"] == null) return DateTime.MinValue;
                return Convert.ToDateTime(ViewState["EndTime"]).AddDays(1).AddSeconds(-1);
            }
            set
            {
                ViewState["EndTime"] = value.ToString(CultureInfo.InvariantCulture);
            }
        }
        #endregion

        #region[设置列表数据源]
        protected void RG_Report_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            IList<CostReportInfo> costReportList = _costReport.GetReportList().Where(r => r.State == (int)CostReportState.Complete || r.State == (int)CostReportState.Execute || r.State == (int)CostReportState.WaitCheck).ToList();
            if (StartTime != DateTime.MinValue)
            {
                costReportList = costReportList.Where(r => r.ReportDate >= StartTime).ToList();
            }
            if (EndTime != DateTime.MaxValue)
            {
                costReportList = costReportList.Where(r => r.ReportDate <= EndTime).ToList();
            }
            if (FilialeId != Guid.Empty)
            {
                costReportList = costReportList.Where(ent => ent.AssumeFilialeId == FilialeId).ToList();
            }
            if (BranchId != Guid.Empty)
            {
                costReportList = costReportList.Where(ent => ent.AssumeBranchId == BranchId).ToList();
            }
            if (CompanyId != Guid.Empty)
            {
                ICost cost = new Cost(GlobalConfig.DB.FromType.Read);
                var info = cost.GetCompanyClass(CompanyId);
                if (info != null && info.CompanyClassId != Guid.Empty)
                {
                    costReportList = costReportList.Where(ent => ent.CompanyClassId == CompanyId).ToList();
                }
                else
                {
                    costReportList = costReportList.Where(ent => ent.CompanyId == CompanyId).ToList();
                }
            }
            var rlist = costReportList.OrderByDescending(r => r.ReportDate).ToList();
            RG_Report.DataSource = rlist;
            var totalAmount = RG_Report.MasterTableView.Columns.FindByUniqueName("PayCost");//合计
            if (rlist.Count > 0)
            {
                totalAmount.FooterText = string.Format("合计:{0}", WebControl.NumberSeparator(rlist.Sum(ent => ent.PayCost)));
            }
            else
            {
                totalAmount.FooterText = "合计:0.00";
            }
        }
        #endregion

        #region[获取状态]

        /// <summary>
        /// 获取状态
        /// </summary>
        /// <param name="reportState">申报状态</param>
        /// <param name="reportKind"> </param>
        /// <param name="invoiceId"> </param>
        /// <returns></returns>
        protected string GetReportState(Object reportState, Object reportKind, Object invoiceId)
        {
            if ((CostReportState)reportState == CostReportState.AlreadyAuditing)
            {
                return "待收款";
            }
            if ((CostReportState)reportState == CostReportState.WaitReturn)
            {
                return "待付款";
            }
            if (reportKind.ToString() == "1" && new Guid(invoiceId.ToString()) == Guid.Empty && (CostReportState)reportState == CostReportState.NoAuditing)
            {
                return "待审核";
            }
            return EnumAttribute.GetKeyName((CostReportState)reportState);
        }
        #endregion

        #region[获取票据类型]
        protected string GetInvoiceType(Object invoiceType)
        {
            return EnumAttribute.GetKeyName((CostReportInvoiceType)invoiceType);
        }
        #endregion

        #region[获取用户名]
        public string GetUserName(Object personnelId)
        {
            return _personnelManager.GetName(new Guid(personnelId.ToString()));
        }
        #endregion
    }
}
