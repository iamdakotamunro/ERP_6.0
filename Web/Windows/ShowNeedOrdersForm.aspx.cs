using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using ERP.BLL.Implement.Inventory;
using ERP.Enum;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Keede.Ecsoft.Model;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using Telerik.Web.UI;

namespace ERP.UI.Web.Windows
{
    public partial class ShowNeedOrdersForm : WindowsPage
    {
        private readonly PurchasingManagement _purchasingManagement = PurchasingManagement.ReadInstance;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                FilialesList = CacheCollection.Filiale.GetList().ToDictionary(ent => ent.ID, ent => ent.Name);
                var shoplist = CacheCollection.Filiale.GetShopList();
                foreach (var shopFilialeInfo in shoplist)
                {
                    if (FilialesList.Count(w => w.Key == shopFilialeInfo.ID) == 0)
                    {
                        FilialesList.Add(shopFilialeInfo.ID, shopFilialeInfo.Name);
                    }
                }
            }
        }
        protected void Rgd_NeedOrders_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            var pmid = new Guid(Request["PersonResponsible"]);
            var warehouseId = new Guid(Request["WarehouseId"]);
            DateTime starttime = Convert.ToDateTime(Request["starttime"]);
            DateTime endtime = Convert.ToDateTime(Request["endtime"]);
            var list = _purchasingManagement.GetAllocationOrdersList(pmid, starttime, endtime, warehouseId).OrderByDescending(w => w.EffectiveTime).ToList();
            rgd_needGoods.DataSource = list;
        }

        protected void NeedGoods_ExportOnClick(object sender, ImageClickEventArgs e)
        {
            var pmid = new Guid(Request["PersonResponsible"]);
            var warehouseId = new Guid(Request["warehouseId"]);
            DateTime starttime = Convert.ToDateTime(Request["starttime"]);
            DateTime endtime = Convert.ToDateTime(Request["endtime"]);
            var list = _purchasingManagement.GetAllocationOrdersList(pmid, starttime, endtime, warehouseId).OrderByDescending(w => w.EffectiveTime).ToList();
            OutPutExcel(list, YesOrNo.Yes);
        }

        /// <summary>
        /// 功    能:导出需调拨订单明细
        /// 时    间:2011-11-17
        /// 作    者:蒋赛标
        /// </summary>
        /// <param name="ordList"></param>
        /// <param name="yesorno"> </param>
        public void OutPutExcel(IList<GoodsOrderInfo> ordList, YesOrNo yesorno)
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
            sheet[t] = workbook.CreateSheet("需采购订单明细");//添加sheet名
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
            cell1.SetCellValue("订单号");
            cell2.SetCellValue("收货人");
            cell3.SetCellValue("有效时间");
            cell4.SetCellValue("订单来源");

            cell1.CellStyle = styletitle;
            cell2.CellStyle = styletitle;
            cell3.CellStyle = styletitle;
            cell4.CellStyle = styletitle;

            int row = 2;
            foreach (GoodsOrderInfo dInfo in ordList)
            {

                HSSFRow rowt = sheet[t].CreateRow(row);
                HSSFCell c1 = rowt.CreateCell(0);
                HSSFCell c2 = rowt.CreateCell(1);
                HSSFCell c3 = rowt.CreateCell(2);
                HSSFCell c4 = rowt.CreateCell(3);

                c1.SetCellValue(dInfo.OrderNo);
                c2.SetCellValue(dInfo.Consignee);
                c3.SetCellValue(string.Format("{0}",dInfo.EffectiveTime));
                c4.SetCellValue(GetSaleFilialeName(dInfo.SaleFilialeId));

                c1.CellStyle = styleContent;
                c2.CellStyle = styleContent;
                c3.CellStyle = styleContent;
                c4.CellStyle = styleContent;
                row++;

            }
            #endregion
            workbook.Write(ms);
            if (YesOrNo.Yes == yesorno)
            {
                Response.ContentEncoding = Encoding.GetEncoding("utf-8");
                Response.AddHeader("Content-Disposition", "attachment; filename=" + HttpUtility.UrlEncode("需采购订单明细" + DateTime.Now.ToString("yyyyMMdd") + ".xls", Encoding.UTF8));
                Response.BinaryWrite(ms.ToArray());
            }
            ms.Close();
            ms.Dispose();
            GC.Collect();
        }

        /// <summary>
        /// 网站列表
        /// </summary>
        protected Dictionary<Guid, String> FilialesList
        {
            get
            {
                if (ViewState["FilialesList"] == null) return new Dictionary<Guid, string>();
                return (Dictionary<Guid, String>)ViewState["FilialesList"];
            }
            set
            {
                ViewState["FilialesList"] = value;
            }
        }

        /// <summary>
        /// 订单来源
        /// </summary>
        /// <param name="saleFilialeId"></param>
        /// <returns></returns>
        public string GetSaleFilialeName(Guid saleFilialeId)
        {
            if (saleFilialeId == Guid.Empty)
                return "";
            return FilialesList.ContainsKey(saleFilialeId)? FilialesList[saleFilialeId]:"-";
        }
    }
}
