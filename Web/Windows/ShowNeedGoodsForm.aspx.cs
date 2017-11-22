using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using ERP.BLL.Implement.Inventory;
using ERP.Enum;
using ERP.Model;
using ERP.UI.Web.Base;
using Keede.Ecsoft.Model;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using Telerik.Web.UI;

namespace ERP.UI.Web.Windows
{
    public partial class ShowNeedGoodsForm : WindowsPage
    {
        private readonly PurchasingManagement _purchasingManagement = PurchasingManagement.ReadInstance;
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Rgd_NeedGoods_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            var pmid = new Guid(Request["PersonResponsible"]);
            var warehouseId = new Guid(Request["WarehouseId"]);
            DateTime starttime = Convert.ToDateTime(Request["starttime"]);
            DateTime endtime = Convert.ToDateTime(Request["endtime"]);
            rgd_needGoods.DataSource = _purchasingManagement.GetAllocationGoodsList(pmid, starttime, endtime, warehouseId);
        }

        protected void NeedGoods_ExportOnClick(object sender, ImageClickEventArgs e)
        {
            var pmid = new Guid(Request["PersonResponsible"]);
            var warehouseId = new Guid(Request["WarehouseId"]);
            DateTime starttime = Convert.ToDateTime(Request["starttime"]);
            DateTime endtime = Convert.ToDateTime(Request["endtime"]);
            OutPutExcel(_purchasingManagement.GetAllocationGoodsList(pmid, starttime, endtime, warehouseId), YesOrNo.Yes);
        }

        /// <summary>
        /// 功    能:导出需调拨订单明细
        /// 时    间:2011-11-17
        /// 作    者:蒋赛标
        /// </summary>
        public void OutPutExcel(IList<StorageRecordDetailInfo> ordList, YesOrNo yesorno)
        {
            if (ordList.Count == 0)
            {
                //  RAM.Alert("没有可导出的数据!");
                return;
            }

            var workbook = new HSSFWorkbook();
            var ms = new MemoryStream();
            var sheet = new HSSFSheet[1];// 增加sheet。

            #region Excel样式

            //标题样式styletitle

            HSSFFont fonttitle = workbook.CreateFont();
            fonttitle.FontHeightInPoints = 12;
            fonttitle.Color = HSSFColor.RED.index;
            fonttitle.Boldweight = 1;
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
            int t = -1;

            t++;
            sheet[t] = workbook.CreateSheet("需采购商品明细");//添加sheet名
            sheet[t].DefaultColumnWidth = 40;
            sheet[t].DefaultRowHeight = 20;
            sheet[t].DisplayGridlines = false;
            HSSFRow rowtitle = sheet[t].CreateRow(0);
            HSSFCell celltitie = rowtitle.CreateCell(0);

            HSSFCellStyle style = workbook.CreateCellStyle();
            style.Alignment = HSSFCellStyle.ALIGN_CENTER;
            HSSFFont font = workbook.CreateFont();
            font.FontHeightInPoints = 20;
            font.Color = HSSFColor.BLACK.index;
            font.Boldweight = 2;
            style.SetFont(font);
            celltitie.CellStyle = style;
            HSSFRow row8 = sheet[t].CreateRow(1);
            HSSFCell cell1 = row8.CreateCell(0);
            HSSFCell cell2 = row8.CreateCell(1);
            HSSFCell cell3 = row8.CreateCell(2);
            HSSFCell cell4 = row8.CreateCell(3);
            cell1.SetCellValue("商品编号");
            cell2.SetCellValue("商品名");
            cell3.SetCellValue("SKU");
            cell4.SetCellValue("缺货数量");
            cell1.CellStyle = styletitle;
            cell2.CellStyle = styletitle;
            cell3.CellStyle = styletitle;
            cell4.CellStyle = styletitle;
            int row = 2;
            foreach (StorageRecordDetailInfo dInfo in ordList)
            {

                HSSFRow rowt = sheet[t].CreateRow(row);
                HSSFCell c1 = rowt.CreateCell(0);
                HSSFCell c2 = rowt.CreateCell(1);
                HSSFCell c3 = rowt.CreateCell(2);
                HSSFCell c4 = rowt.CreateCell(3);
                c1.SetCellValue(dInfo.GoodsCode);
                c2.SetCellValue(dInfo.GoodsName);
                c3.SetCellValue(dInfo.Specification);
                c4.SetCellValue(Math.Abs(dInfo.Quantity));
                c1.CellStyle = styleContent;
                c2.CellStyle = styleContent;
                c3.CellStyle = styleContent;
                c4.CellStyle = styleContent;
                c4.CellStyle.Alignment = HSSFCellStyle.ALIGN_CENTER;
                row++;

            }
            #endregion
            workbook.Write(ms);
            if (YesOrNo.Yes == yesorno)
            {
                Response.ContentEncoding = Encoding.GetEncoding("utf-8");
                Response.AddHeader("Content-Disposition", "attachment; filename=" + HttpUtility.UrlEncode("需采购商品明细" + DateTime.Now.ToString("yyyyMMdd") + ".xls", Encoding.UTF8));
                Response.BinaryWrite(ms.ToArray());
            }
            ms.Close();
            ms.Dispose();
            GC.Collect();


        }
    }
}
