using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI;
using ERP.BLL.Implement.Order;
using ERP.Cache;
using ERP.Enum;
using ERP.Enum.Attribute;
using ERP.UI.Web.Common;
using Keede.Ecsoft.Model;
using NPOI.HSSF.UserModel;
using Telerik.Web.UI;

namespace ERP.UI.Web.Windows
{
    public partial class CompleteOrderTaskExpressForm : Page
    {
        public Guid TaskID
        {
            get
            {
                if (ViewState["TaskID"] == null)
                    ViewState["TaskID"] = Request.QueryString["ID"];
                return new Guid(ViewState["TaskID"].ToString());
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

            }
        }

        protected void RGTaskExpress_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            RGTaskExpress.DataSource = CompleteOrderTaskManager.GetCompleteOrderTaskExpressByTaskId(TaskID);
        }
        protected void RAM_AjaxRequest(object sender, AjaxRequestEventArgs e)
        {
            WebControl.RamAjajxRequest(RGTaskExpress, e);
        }

        protected void RGTaskExpress_ItemCommand(object sender, GridCommandEventArgs e)
        {
            if (e.CommandName == "ExportExcel")
            {
                var item = e.Item as GridDataItem;
                if (item != null)
                {
                    var expressId = new Guid(item.GetDataKeyValue("ExpressId").ToString());
                    ExportExcel(expressId);
                }
            }

        }

        private void ExportExcel(Guid expressId)
        {
            IList<GoodsOrderInfo> orderList = CompleteOrderTaskManager.GetExportGoodsOrderList(TaskID, expressId).ToList();
            var workbook = new HSSFWorkbook();
            HSSFSheet sheet = workbook.CreateSheet();
            HSSFRow row = sheet.CreateRow(0); //创建第一行
            row.CreateCell(0, HSSFCell.CELL_TYPE_STRING).SetCellValue("订单号");
            row.CreateCell(1, HSSFCell.CELL_TYPE_STRING).SetCellValue("快递号");
            //row.CreateCell(2, HSSFCell.CELL_TYPE_STRING).SetCellValue("收货人");
            //row.CreateCell(3, HSSFCell.CELL_TYPE_STRING).SetCellValue("手机");
            row.CreateCell(2, HSSFCell.CELL_TYPE_STRING).SetCellValue("省份");
            row.CreateCell(3, HSSFCell.CELL_TYPE_STRING).SetCellValue("城市");
            row.CreateCell(4, HSSFCell.CELL_TYPE_STRING).SetCellValue("收货地址");
            row.CreateCell(5, HSSFCell.CELL_TYPE_STRING).SetCellValue("支付类型");
            row.CreateCell(6, HSSFCell.CELL_TYPE_STRING).SetCellValue("订单总额");
            row.CreateCell(7, HSSFCell.CELL_TYPE_STRING).SetCellValue("实收金额");

            for (var i = 0; i < orderList.Count; i++)
            {
                var info = orderList[i];
                HSSFRow vrow = sheet.CreateRow(i + 1);
                vrow.CreateCell(0, HSSFCell.CELL_TYPE_STRING).SetCellValue(info.OrderNo);
                vrow.CreateCell(1, HSSFCell.CELL_TYPE_STRING).SetCellValue(info.ExpressNo ?? string.Empty);
                // vrow.CreateCell(2, HSSFCell.CELL_TYPE_STRING).SetCellValue(info.Consignee);
                //vrow.CreateCell(3, HSSFCell.CELL_TYPE_STRING).SetCellValue(info.Mobile);
                vrow.CreateCell(2, HSSFCell.CELL_TYPE_STRING).SetCellValue(Province.Instance.GetName(info.ProvinceId));
                vrow.CreateCell(3, HSSFCell.CELL_TYPE_STRING).SetCellValue(City.Instance.GetName(info.CityId));
                vrow.CreateCell(4, HSSFCell.CELL_TYPE_STRING).SetCellValue(ConvertDirection(info.Direction, info.ExpressId));
                vrow.CreateCell(5, HSSFCell.CELL_TYPE_STRING).SetCellValue(EnumAttribute.GetKeyName((PayMode)info.PayMode));
                vrow.CreateCell(6, HSSFCell.CELL_TYPE_STRING).SetCellValue(GetOrderTotalPrice(info.TotalPrice, info.Carriage));
                vrow.CreateCell(7, HSSFCell.CELL_TYPE_STRING).SetCellValue(info.PayMode == (int)PayMode.COG ? "0" : info.RealTotalPrice.ToString("0.#"));
            }
            //HttpResponse contextResponse = HttpContext.Current.Response;
            string fileName = "OrderList_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
            Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName); //解决中文文件名乱码    
            Response.ContentType = "application/vnd.ms-excel";
            //Response.BinaryWrite(workbook.GetBytes());
            var tempXlsPath = Server.MapPath("~/Temp/");
            if (!Directory.Exists(tempXlsPath))
            {
                Directory.CreateDirectory(tempXlsPath);
            }
            var tempXlsFile = tempXlsPath + fileName;
            using (var stream = new FileStream(tempXlsFile, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                workbook.Write(stream);
            }
            RAM.ResponseScripts.Add("window.open('/WebService/Downfile.ashx?exportOrder=1&filename=" + fileName + "')");
            //Response.TransmitFile(tempXlsFile);
        }

        /// <summary>地址信息包含数字或字母替换X
        /// </summary>
        /// <param name="direction">地址</param>
        /// <param name="expressId">快递公司ID </param>
        /// <returns></returns>
        private string ConvertDirection(string direction, Guid expressId)
        {
            //注：微特派快递，中国邮政不转换
            if (expressId != new Guid("7BB64B73-5FB9-478F-AA40-8B771C8CFEA9") && expressId != new Guid("4C6E9403-0E61-4796-9662-8196F842A1A6"))
            {
                var sb = new StringBuilder();
                foreach (var ch in direction)
                {
                    if (Regex.IsMatch(string.Format("{0}", ch), @"\b[a-zA-Z0-9]+\b"))//正则表达式
                    {
                        sb.Append("x");
                        continue;
                    }
                    sb.Append(ch);
                }
                return sb.ToString();
            }
            return direction;
        }

        private string GetOrderTotalPrice(object totalPrice, object carriage)
        {
            if (carriage == null)
                carriage = 0;
            if (totalPrice != null)
                return WebControl.NumberSeparator(Convert.ToDouble(totalPrice) + Convert.ToDouble(carriage));
            return (Convert.ToDouble(carriage).ToString("0.#"));
        }

        public string GetExpressName(Guid expressId)
        {
            var expressInfo = Express.Instance.Get(expressId);
            return expressInfo == null ? string.Empty : expressInfo.ExpressFullName;
        }
    }
}