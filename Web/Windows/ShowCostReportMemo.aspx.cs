using System;
using ERP.DAL.Interface.IInventory;
using ERP.Environment;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;

namespace ERP.UI.Web.Windows
{
    ///<summary>
    /// 费用申报备注信息
    /// Add by liucaijun at 2012-03-13
    ///</summary>
    public partial class ShowCostReportMemo : WindowsPage
    {
        private readonly ICostReport _costReport = new DAL.Implement.Inventory.CostReport(GlobalConfig.DB.FromType.Read);

        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                TB_Clew.Text = _costReport.GetReportByReportId(ReportId).Memo;
            }
        }

        protected Guid ReportId
        {
            get
            {
                return WebControl.GetGuidFromQueryString("ReportId");
            }
        }
    }
}
