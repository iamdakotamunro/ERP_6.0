using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using ERP.BLL.Implement;
using ERP.BLL.Implement.Organization;
using ERP.DAL.Interface.IInventory;
using ERP.Environment;
using ERP.UI.Web.Common;
using Keede.Ecsoft.Model;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using Telerik.Web.UI;
using ERP.UI.Web.Base;

namespace ERP.UI.Web.CostReport
{
    /************************************************************************************ 
    * 创建人：  文雯
    * 创建时间：2016/07/04  
    * 描述    :押金回收
    * =====================================================================
    * 修改时间：2016/07/04  
    * 修改人  ：  
    * 描述    ：
    */

    public partial class CostReport_DepositRecoveryManage : BasePage
    {
        private static readonly ICostReport _costReport = new DAL.Implement.Inventory.CostReport(GlobalConfig.DB.FromType.Write);
        private static readonly ICostReportDepositRecovery _costReportDepositRecovery = new DAL.Implement.Inventory.CostReportDepositRecovery(GlobalConfig.DB.FromType.Write);

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        //查询
        protected void btn_Search_Click(object sender, EventArgs e)
        {
            GridDataBind();
            RG_ReportDepositRecovery.DataBind();
        }

        #region 数据列表相关
        protected void RG_ReportDepositRecovery_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            GridDataBind();
        }

        //Grid数据源
        protected void GridDataBind()
        {
            var stime = DateTime.MinValue;
            var etime = DateTime.MinValue;

            if (!string.IsNullOrEmpty(txt_DateTimeStart.Text))
            {
                stime = DateTime.Parse(txt_DateTimeStart.Text);
            }
            if (!string.IsNullOrEmpty(txt_DateTimeEnd.Text))
            {
                etime = DateTime.Parse(txt_DateTimeEnd.Text).AddDays(1);
            }

            var costReportList = _costReport.GetReportListForDeposit(int.Parse(ddl_State.SelectedValue), stime, etime, string.IsNullOrEmpty(RCB_ReportPersonnel.SelectedValue) ? null : new List<Guid> { new Guid(RCB_ReportPersonnel.SelectedValue) }, txt_ReportNo.Text);
            var query = costReportList.AsQueryable();

            #region 合计
            var totalName = RG_ReportDepositRecovery.MasterTableView.Columns.FindByUniqueName("TotalName");
            var reportCost = RG_ReportDepositRecovery.MasterTableView.Columns.FindByUniqueName("ReportCost");
            var payCost = RG_ReportDepositRecovery.MasterTableView.Columns.FindByUniqueName("PayCost");
            if (query.Any())
            {
                var sumReportCost = query.Sum(p => Math.Abs(p.ReportCost));
                var sumPayCost = query.Sum(p => Math.Abs(p.PayCost));
                totalName.FooterText = "合计：";
                reportCost.FooterText = string.Format("{0}", ERP.UI.Web.Common.WebControl.NumberSeparator(sumReportCost));
                payCost.FooterText = string.Format("{0}", ERP.UI.Web.Common.WebControl.NumberSeparator(sumPayCost));
            }
            else
            {
                totalName.FooterText = string.Empty;
                reportCost.FooterText = string.Empty;
                payCost.FooterText = string.Empty;
            }
            #endregion

            RG_ReportDepositRecovery.DataSource = query.OrderByDescending(p => p.ReportDate).ToList();
        }

        //行绑定事件
        protected void RG_ReportDepositRecovery_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item.ItemType == GridItemType.Item || e.Item.ItemType == GridItemType.AlternatingItem)
            {
                var dataItem = (GridDataItem)e.Item;
                var btnRecovery = (Button)dataItem.FindControl("btn_Recovery");
                btnRecovery.Visible = ddl_State.SelectedValue != "1";

                var reportId = (Guid)((GridDataItem)e.Item).GetDataKeyValue("ReportId");
                var depositRecoveryList = _costReportDepositRecovery.GetDepositRecoveryList(reportId);
                if (depositRecoveryList.Count <= 0)
                {
                    e.Item.Cells[17].Text = string.Empty;
                    e.Item.Cells[18].Text = string.Empty;
                    e.Item.Cells[19].Text = string.Empty;
                }
                else
                {
                    var row1 = string.Empty;
                    var row2 = string.Empty;
                    var row3 = string.Empty;
                    var row4 = string.Empty;

                    for (int i = 0; i < depositRecoveryList.Count; i++)
                    {
                        e.Item.Cells[19].Text = depositRecoveryList[0].RecoveryDate.ToString("yyyy-MM-dd");
                        if (i == 0)
                        {
                            row1 +=
                                "<tr><td class='Group' style='padding-top:5px; padding-bottom:5px;border-left:0px;border-bottom:0px;'  colspan='2'>" +
                                depositRecoveryList[i].ReportNo + "</td></tr>";
                            row3 +=
                                "<tr><td class='Group' style='padding-top:5px; padding-bottom:5px;border-left:0px;border-bottom:0px;'  colspan='2'>" +
                                depositRecoveryList[i].RecoveryCost + "</td></tr>";
                        }
                        if (i == 1)
                        {
                            row2 +=
                                "<tr><td class='title' style='padding-top:5px; padding-bottom:5px;border-left:0px;border-bottom:0px;'>" +
                                depositRecoveryList[i].ReportNo + "</td></tr>";
                            row4 +=
                                "<tr><td class='title' style='padding-top:5px; padding-bottom:5px;border-left:0px;border-bottom:0px;'>" +
                                depositRecoveryList[i].RecoveryCost + "</td></tr>";
                        }
                    }
                    var reportNoText = "<table cellspacing='0' cellpadding='0' style=\"width: 100%;\">" + row1 + row2 +
                                       "</table>";
                    var recoveryCostText =
                        "<table cellspacing='0' cellpadding='0' style=\"width: 100%;text-align:center;\">" + row3 + row4 +
                        "</table>";
                    e.Item.Cells[17].Text = reportNoText;
                    e.Item.Cells[18].Text = recoveryCostText;
                }
            }
        }
        #endregion

        //搜索申请人
        protected void SearchItemsRequested(object sender, RadComboBoxItemsRequestedEventArgs e)
        {
            var combo = (RadComboBox)sender;
            //获取关键字
            var searchKey = e.Text.Trim();
            if (string.IsNullOrEmpty(searchKey))
            {
                return;
            }
            IDictionary<string, string> dataDict = new Dictionary<string, string>();

            var personnelList =
                new PersonnelManager().GetList()
                    .Where(p => p.RealName.Contains(searchKey) || p.EnterpriseNo.Contains(searchKey));

            foreach (var info in personnelList)
            {
                dataDict.Add(info.PersonnelId.ToString(), info.RealName);
            }

            combo.DataSource = dataDict;
            combo.DataBind();
        }

        /// <summary> 
        /// 导出Excel
        /// </summary>
        protected void Ib_ExportData_Click(object sender, EventArgs e)
        {
            var stime = DateTime.MinValue;
            var etime = DateTime.MinValue;

            if (!string.IsNullOrEmpty(txt_DateTimeStart.Text))
            {
                stime = DateTime.Parse(txt_DateTimeStart.Text);
            }
            if (!string.IsNullOrEmpty(txt_DateTimeEnd.Text))
            {
                etime = DateTime.Parse(txt_DateTimeEnd.Text).AddDays(1);
            }

            var costReportList = _costReport.GetReportListForDeposit(int.Parse(ddl_State.SelectedValue), stime, etime, string.IsNullOrEmpty(RCB_ReportPersonnel.SelectedValue) ? null : new List<Guid> { new Guid(RCB_ReportPersonnel.SelectedValue) }, txt_ReportNo.Text);
            var query = costReportList.AsQueryable();

            OutPutExcel(query.OrderByDescending(p => p.ReportDate).ToList());
        }

        /// <summary>
        /// 导出Excel
        /// </summary>
        public void OutPutExcel(List<CostReportInfo> list)
        {
            var workbook = new HSSFWorkbook();
            var ms = new MemoryStream();
            var sheet = new HSSFSheet[1];// 增加sheet。

            #region Excel样式

            //标题样式styletitle

            HSSFFont fonttitle = workbook.CreateFont();
            fonttitle.FontHeightInPoints = 10;
            fonttitle.Boldweight = HSSFFont.BOLDWEIGHT_BOLD;
            HSSFCellStyle styletitle = workbook.CreateCellStyle();
            styletitle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            styletitle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            styletitle.BorderRight = HSSFCellStyle.BORDER_THIN;
            styletitle.BorderTop = HSSFCellStyle.BORDER_THIN;
            styletitle.SetFont(fonttitle);

            //内容字体styleContent
            HSSFFont fontcontent = workbook.CreateFont();
            fontcontent.FontHeightInPoints = 9;
            fontcontent.Color = HSSFColor.BLACK.index;
            HSSFCellStyle styleContent = workbook.CreateCellStyle();
            styleContent.BorderBottom = HSSFCellStyle.BORDER_THIN;
            styleContent.BorderLeft = HSSFCellStyle.BORDER_THIN;
            styleContent.BorderRight = HSSFCellStyle.BORDER_THIN;
            styleContent.BorderTop = HSSFCellStyle.BORDER_THIN;
            styleContent.SetFont(fontcontent);
            #endregion

            #region 将值插入sheet

            sheet[0] = workbook.CreateSheet("押金回收列表");//添加sheet名
            sheet[0].DefaultColumnWidth = 40;
            sheet[0].DefaultRowHeight = 20;

            HSSFRow rowtitle = sheet[0].CreateRow(0);
            HSSFCell celltitie = rowtitle.CreateCell(0);
            const string SHEET_TITLE = "押金回收列表";
            celltitie.SetCellValue(SHEET_TITLE);

            HSSFCellStyle style = workbook.CreateCellStyle();
            style.Alignment = HSSFCellStyle.ALIGN_CENTER;
            HSSFFont font = workbook.CreateFont();
            font.FontHeightInPoints = 20;
            font.Color = HSSFColor.BLACK.index;
            font.Boldweight = 2;
            style.SetFont(font);
            celltitie.CellStyle = style;

            sheet[0].AddMergedRegion(new Region(0, 0, 0, 2));

            #region //列名
            HSSFRow rowTitle = sheet[0].CreateRow(3);
            HSSFCell cell1 = rowTitle.CreateCell(0);
            HSSFCell cell2 = rowTitle.CreateCell(1);
            HSSFCell cell3 = rowTitle.CreateCell(2);
            HSSFCell cell4 = rowTitle.CreateCell(3);
            HSSFCell cell5 = rowTitle.CreateCell(4);
            HSSFCell cell6 = rowTitle.CreateCell(5);
            HSSFCell cell7 = rowTitle.CreateCell(6);
            HSSFCell cell8 = rowTitle.CreateCell(7);
            HSSFCell cell9 = rowTitle.CreateCell(8);
            HSSFCell cell10 = rowTitle.CreateCell(9);
            HSSFCell cell11 = rowTitle.CreateCell(10);
            cell1.SetCellValue("公司");
            cell2.SetCellValue("账户");
            cell3.SetCellValue("申请人");
            cell4.SetCellValue("预借款单据号");
            cell5.SetCellValue("付款金额");
            cell6.SetCellValue("付款时间");
            cell7.SetCellValue("回收单据号");
            cell8.SetCellValue("回收金额");
            cell9.SetCellValue("回收时间");
            cell10.SetCellValue("备注说明");
            cell11.SetCellValue("状态");

            cell1.CellStyle = styletitle;
            cell2.CellStyle = styletitle;
            cell3.CellStyle = styletitle;
            cell4.CellStyle = styletitle;
            cell5.CellStyle = styletitle;
            cell6.CellStyle = styletitle;
            cell7.CellStyle = styletitle;
            cell8.CellStyle = styletitle;
            cell9.CellStyle = styletitle;
            cell10.CellStyle = styletitle;
            cell11.CellStyle = styletitle;
            cell1.CellStyle.Alignment = HSSFCellStyle.ALIGN_CENTER;
            #endregion

            #region //内容
            int row = 4;

            if (list.Count > 0)
            {
                foreach (CostReportInfo info in list)
                {
                    var depositRecoveryList = _costReportDepositRecovery.GetDepositRecoveryList(info.ReportId);
                    if (depositRecoveryList.Count > 0)
                    {
                        foreach (var depositRecovery in depositRecoveryList)
                        {
                            HSSFRow rowt = sheet[0].CreateRow(row);
                            HSSFCell c1 = rowt.CreateCell(0);
                            HSSFCell c2 = rowt.CreateCell(1);
                            HSSFCell c3 = rowt.CreateCell(2);
                            HSSFCell c4 = rowt.CreateCell(3);
                            HSSFCell c5 = rowt.CreateCell(4);
                            HSSFCell c6 = rowt.CreateCell(5);
                            HSSFCell c7 = rowt.CreateCell(6);
                            HSSFCell c8 = rowt.CreateCell(7);
                            HSSFCell c9 = rowt.CreateCell(8);
                            HSSFCell c10 = rowt.CreateCell(9);
                            HSSFCell c11 = rowt.CreateCell(10);
                            c1.SetCellValue(CacheCollection.Filiale.GetFilialeNameAndFilialeId(info.PayBankAccountId).Split(',')[0]);
                            c2.SetCellValue(info.BankAccount);
                            c3.SetCellValue(new PersonnelManager().GetName(info.ReportPersonnelId));
                            c4.SetCellValue(info.ReportNo);
                            c5.SetCellValue(info.RealityCost.ToString());
                            c6.SetCellValue(info.ExecuteDate.ToString("yyyy-MM-dd") == "1900-01-01" ? "" : info.ExecuteDate.ToString("yyyy-MM-dd"));
                            c7.SetCellValue(depositRecovery.ReportNo);
                            c8.SetCellValue(depositRecovery.RecoveryCost.ToString());
                            c9.SetCellValue(depositRecovery.RecoveryDate.ToString("yyyy-MM-dd"));
                            c10.SetCellValue(depositRecovery.RecoveryRemarks);
                            c11.SetCellValue(ddl_State.SelectedItem.ToString());
                            c1.CellStyle = styleContent;
                            c2.CellStyle = styleContent;
                            c3.CellStyle = styleContent;
                            c4.CellStyle = styleContent;
                            c5.CellStyle = styleContent;
                            c6.CellStyle = styleContent;
                            c7.CellStyle = styleContent;
                            c8.CellStyle = styleContent;
                            c9.CellStyle = styleContent;
                            c10.CellStyle = styleContent;
                            c11.CellStyle = styleContent;
                            c1.CellStyle.Alignment = HSSFCellStyle.ALIGN_CENTER;
                            row++;
                        }
                    }
                    else
                    {
                        HSSFRow rowt = sheet[0].CreateRow(row);
                        HSSFCell c1 = rowt.CreateCell(0);
                        HSSFCell c2 = rowt.CreateCell(1);
                        HSSFCell c3 = rowt.CreateCell(2);
                        HSSFCell c4 = rowt.CreateCell(3);
                        HSSFCell c5 = rowt.CreateCell(4);
                        HSSFCell c6 = rowt.CreateCell(5);
                        HSSFCell c7 = rowt.CreateCell(6);
                        HSSFCell c8 = rowt.CreateCell(7);
                        HSSFCell c9 = rowt.CreateCell(8);
                        HSSFCell c10 = rowt.CreateCell(9);
                        HSSFCell c11 = rowt.CreateCell(10);
                        c1.SetCellValue(CacheCollection.Filiale.GetFilialeNameAndFilialeId(info.PayBankAccountId).Split(',')[0]);
                        c2.SetCellValue(info.BankAccount);
                        c3.SetCellValue(new PersonnelManager().GetName(info.ReportPersonnelId));
                        c4.SetCellValue(info.ReportNo);
                        c5.SetCellValue(info.RealityCost.ToString());
                        c6.SetCellValue(info.ExecuteDate.ToString("yyyy-MM-dd") == "1900-01-01" ? "" : info.ExecuteDate.ToString("yyyy-MM-dd"));
                        c7.SetCellValue("");
                        c8.SetCellValue("");
                        c9.SetCellValue("");
                        c10.SetCellValue("");
                        c11.SetCellValue(ddl_State.SelectedItem.ToString());
                        c1.CellStyle = styleContent;
                        c2.CellStyle = styleContent;
                        c3.CellStyle = styleContent;
                        c4.CellStyle = styleContent;
                        c5.CellStyle = styleContent;
                        c6.CellStyle = styleContent;
                        c7.CellStyle = styleContent;
                        c8.CellStyle = styleContent;
                        c9.CellStyle = styleContent;
                        c10.CellStyle = styleContent;
                        c11.CellStyle = styleContent;
                        c1.CellStyle.Alignment = HSSFCellStyle.ALIGN_CENTER;
                        row++;
                    }


                }
            }
            else
            {
                HSSFRow rtotal = sheet[0].CreateRow(row);
                HSSFCell t0 = rtotal.CreateCell(0);
                t0.SetCellValue("无数据显示");
            }
            sheet[0].DisplayGridlines = false;
            #endregion

            #region 输出
            workbook.Write(ms);
            Response.ContentEncoding = Encoding.GetEncoding("utf-8");
            Response.AddHeader("Content-Disposition", "attachment; filename=" + HttpUtility.UrlEncode("押金回收列表" + DateTime.Now.ToString("yyyyMMdd") + ".xls", Encoding.UTF8));
            Response.BinaryWrite(ms.ToArray());

            ms.Close();
            ms.Dispose();
            GC.Collect();
            #endregion

            #endregion
        }

        #region 取得用户操作权限
        /// <summary>
        /// 取得用户操作权限
        /// </summary>
        protected bool GetPowerOperationPoint(string powerName)
        {
            const string PAGE_NAME = "/CostReport/CostReport_DepositRecoveryManage.aspx";
            return Common.WebControl.GetPowerOperationPoint(PAGE_NAME, powerName);
        }

        #endregion
    }
}