using ERP.DAL.Interface.IInventory;
using ERP.Environment;
using ERP.UI.Web.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.UI.Web.CostReport
{
    /// <summary>
    /// ReportNoHandler 的摘要说明
    /// </summary>
    public class ReportNoHandler : IHttpHandler
    {
        private static readonly ICostReport _costReport = new DAL.Implement.Inventory.CostReport(GlobalConfig.DB.FromType.Write);
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            var reportId = context.Request.QueryString["reportId"];
            var reportPersonnelId = context.Request.QueryString["reportPersonnelId"];
            var payCompany = context.Request.QueryString["payCompany"];
            var reportCost = context.Request.QueryString["reportCost"];
            string payCompanyReportNoStr = string.Empty;
            string personnelReportNoStr = string.Empty;
            var reportNoArrayList = _costReport.ReportNoArrayList(payCompany, new Guid(reportPersonnelId), string.IsNullOrEmpty(reportCost) ? 0 : decimal.Parse(reportCost), new Guid(reportId));
            if (reportNoArrayList.Length > 0)
            {
                if (reportNoArrayList[0] != null && reportNoArrayList[0].Count > 0)
                {
                    var payCompanyStr = string.Join(",", reportNoArrayList[0].ToArray());
                    payCompanyReportNoStr = "与此单据的收款单位和金额相同的申报单据有" + payCompanyStr + "，是否需要继续提交？";
                }
                if (reportNoArrayList[1] != null && reportNoArrayList[1].Count > 0)
                {
                    var personnelStr = string.Join(",", reportNoArrayList[1].ToArray());
                    personnelReportNoStr = "与此单据的提交人和金额相同的申报单据有" + personnelStr + "，是否需要继续提交？";
                }
            }
            context.Response.Write(payCompanyReportNoStr + " <br/>" + personnelReportNoStr);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}