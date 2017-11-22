using System;
using System.IO;
using System.Linq;
using System.Transactions;
using System.Web.UI.WebControls;
using ERP.BLL.Implement;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Enum.Attribute;
using ERP.Environment;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Keede.Ecsoft.Model;
using OperationLog.Core;
using Telerik.Web.UI;
using WebControl = ERP.UI.Web.Common.WebControl;
using ERP.Model;
using System.Web.UI;

namespace ERP.UI.Web.CostReport
{
    /************************************************************************************ 
    * 创建人：  张安龙
    * 创建时间：2015/11/11  
    * 描述    :我的费用申报
    * =====================================================================
    * 修改时间：2015/11/11  
    * 修改人  ：  
    * 描述    ：
    */
    public partial class CostReport_ReportManage : BasePage
    {
        readonly OperationLogManager _operationLogManager = new OperationLogManager();
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
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        //查询
        protected void btn_Search_Click(object sender, EventArgs e)
        {
            GridDataBind();
            RG_Report.DataBind();
        }

        #region 数据列表相关
        protected void RG_Report_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            GridDataBind();
        }

        //Grid数据源
        protected void GridDataBind()
        {
            var costReportList = _costReport.GetReportList(Personnel.PersonnelId).OrderByDescending(r => r.ReportDate);
            var query = costReportList.AsQueryable();

            #region 查询条件
            if (!string.IsNullOrEmpty(txt_ReportNo.Text))
            {
                query = query.Where(p => p.ReportNo.Equals(txt_ReportNo.Text.Trim()));
            }
            if (!string.IsNullOrEmpty(txt_ReportDateStart.Text))
            {
                var startTime = DateTime.Parse(txt_ReportDateStart.Text);
                query = query.Where(p => p.ReportDate >= startTime);
            }
            if (!string.IsNullOrEmpty(txt_ReportDateEnd.Text))
            {
                var endtime = DateTime.Parse(txt_ReportDateEnd.Text);
                query = query.Where(p => p.ReportDate < endtime.AddDays(1));
            }

            if (!string.IsNullOrEmpty(txt_ReportName.Text))
            {
                query = query.Where(p => p.ReportName.Contains(txt_ReportName.Text.Trim()));
            }

            switch (ddl_State.SelectedValue)
            {
                case "0": //已处理
                    query = query.Where(p =>
                        p.State == (int)CostReportState.AuditingNoPass ||
                        p.State == (int)CostReportState.InvoiceNoPass ||
                        p.State == (int)CostReportState.Complete);
                    break;
                case "1": //未处理
                    query = query.Where(p =>
                        p.State == (int)CostReportState.Auditing ||
                        p.State == (int)CostReportState.NoAuditing ||
                        p.State == (int)CostReportState.AlreadyAuditing ||
                        p.State == (int)CostReportState.WaitVerify ||
                        p.State == (int)CostReportState.Pay ||
                        p.State == (int)CostReportState.CompletedMayApply);
                    break;
                case "-1": //全部
                    break;
                default:
                    query = query.Where(p => p.State != (int)CostReportState.Complete);
                    break;
            }
            #endregion

            #region 合计
            var totalName = RG_Report.MasterTableView.Columns.FindByUniqueName("TotalName");
            var reportCost = RG_Report.MasterTableView.Columns.FindByUniqueName("ReportCost");
            var payCost = RG_Report.MasterTableView.Columns.FindByUniqueName("PayCost");
            if (query.Any())
            {
                var sumReportCost = query.Sum(p => Math.Abs(p.ReportCost));
                var sumPayCost = query.Sum(p => Math.Abs(p.PayCost));
                totalName.FooterText = "合计：";
                reportCost.FooterText = string.Format("{0}", WebControl.NumberSeparator(sumReportCost));
                payCost.FooterText = string.Format("{0}", WebControl.NumberSeparator(sumPayCost));
            }
            else
            {
                totalName.FooterText = string.Empty;
                reportCost.FooterText = string.Empty;
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
                var reportInfo = (CostReportInfo)e.Item.DataItem;
                #region 票据截止日期 超过15天未提交票据，标红该行
                DateTime rportDate = Convert.ToDateTime(reportInfo.ReportDate);
                DateTime finallySubmitTicketDate = rportDate.Date.AddDays(15);
                if (reportInfo.State != (int)CostReportState.Cancel &&
                    reportInfo.State != (int)CostReportState.Complete)
                {
                    if (finallySubmitTicketDate <= DateTime.Now)
                    {
                        e.Item.Style["color"] = "red";
                        e.Item.ToolTip = "票据超过15天未提交！";
                    }
                }
                #endregion

                #region 控制操作按钮显示的文本
                var btnControl = (Button)e.Item.FindControl("btn_Control");
                if (
                    (reportInfo.ReportKind.Equals((int)CostReportKind.Before) && reportInfo.PayCost.Equals(0) && (reportInfo.State.Equals((int)CostReportState.InvoiceNoPass) || reportInfo.State.Equals((int)CostReportState.Auditing) || reportInfo.State.Equals((int)CostReportState.AuditingNoPass))) ||
                    (reportInfo.ReportKind.Equals((int)CostReportKind.Later) && (reportInfo.State.Equals((int)CostReportState.InvoiceNoPass) || reportInfo.State.Equals((int)CostReportState.Auditing) || reportInfo.State.Equals((int)CostReportState.AuditingNoPass))) ||
                    (reportInfo.ReportKind.Equals((int)CostReportKind.Paying) && (reportInfo.State.Equals((int)CostReportState.Auditing) || reportInfo.State.Equals((int)CostReportState.AuditingNoPass))) ||
                    (reportInfo.ReportKind.Equals((int)CostReportKind.FeeIncome) && (reportInfo.State.Equals((int)CostReportState.Auditing) || reportInfo.State.Equals((int)CostReportState.AuditingNoPass))) ||
                    (reportInfo.ReportKind.Equals((int)CostReportKind.Before) && !reportInfo.PayCost.Equals(0) && (reportInfo.State.Equals((int)CostReportState.InvoiceNoPass) || (!reportInfo.IsSystem && reportInfo.State.Equals((int)CostReportState.Auditing)) || reportInfo.State.Equals((int)CostReportState.AuditingNoPass) || reportInfo.State.Equals((int)CostReportState.CompletedMayApply))) ||
                    (reportInfo.ReportKind.Equals((int)CostReportKind.Paying) && reportInfo.State.Equals((int)CostReportState.InvoiceNoPass))
                    )
                {
                    btnControl.Text = "编辑";
                }
                else
                {
                    btnControl.Text = "查看";
                }
                #endregion

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
        /// 票据截止日期=申报日期+15天
        /// </summary>
        /// <param name="reportDate">申报日期</param>
        /// <returns></returns>
        protected string GetFinallySubmitTicketDate(string reportDate)
        {
            if (!reportDate.Equals("1900-01-01"))
            {
                DateTime date = Convert.ToDateTime(reportDate);
                return date.Date.AddDays(15).ToString("yyyy-MM-dd");
            }
            return string.Empty;
        }
        /// <summary>
        /// 获取处理状态
        /// </summary>
        /// <param name="reportState">状态</param>
        /// <returns></returns>
        protected string GetReportState(string reportState)
        {
            if (string.IsNullOrEmpty(reportState))
            {
                return "-";
            }

            var state = int.Parse(reportState);
            if (((int)CostReportState.AlreadyAuditing).Equals(state))
            {
                return "待收款";
            }
            if (((int)CostReportState.WaitVerify).Equals(state))
            {
                return "待付款";
            }
            return EnumAttribute.GetKeyName((CostReportState)state);
        }
        #endregion
        #endregion

        //作废
        protected void btn_Del_Click(object sender, EventArgs e)
        {
            var reportId = ((Button)sender).CommandArgument;
            var costReportInfo = _costReport.GetReportByReportId(new Guid(reportId));
            if (!(costReportInfo.State.Equals((int)CostReportState.Auditing) || costReportInfo.State.Equals((int)CostReportState.AuditingNoPass) || (costReportInfo.ReportKind.Equals((int)CostReportKind.Later) && costReportInfo.State.Equals((int)CostReportState.InvoiceNoPass))))
            {
                MessageBox.Show(this, "该单据状态已更新，不允许此操作！");
                return;
            }

            using (var ts = new TransactionScope(TransactionScopeOption.Required))
            {
                try
                {
                    var memo = WebControl.RetrunUserAndTime("[【作废申报】:申报作废;]");
                    _costReport.UpdateReport(new Guid(reportId), (int)CostReportState.Cancel, string.Empty, memo, Guid.Empty);

                    //添加操作日志
                    _operationLogManager.Add(Personnel.PersonnelId, Personnel.RealName, new Guid(reportId), costReportInfo.ReportNo, OperationPoint.CostDeclare.AuditDeclare.GetBusinessInfo(), 1, "");

                    RG_Report.Rebind();
                    ts.Complete();
                }
                catch
                {
                    MessageBox.AppendScript(this, "作废申报失败！");
                }
            }
        }
    }
}