using ERP.Enum;
using ERP.UI.Web.Base;
using System;

namespace ERP.UI.Web.CostReport
{
    public partial class CostReport_Control : WindowsPage
    {
        public string ReportId = string.Empty, ReportKind = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ReportId = Request.QueryString["ReportId"];
                if (!string.IsNullOrEmpty(ReportId))
                {
                    ReportKind = ((CostReportKind) int.Parse(Request.QueryString["ReportKind"])).ToString();
                    rbl_ReportKind.SelectedValue = ReportKind;
                    rbl_ReportKind.Enabled = false;
                }
            }
        }
    }
}