using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ERP.BLL.Implement.Organization;
using ERP.DAL.Interface.IInventory;
using ERP.Model.Invoice;
using ERP.Environment;
using Keede.Ecsoft.Model;
using ERP.UI.Web.Common;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using System.IO;
using System.Linq;
using ERP.Enum.ApplyInvocie;
using Telerik.Web.UI;

namespace ERP.UI.Web.Windows
{
    public partial class DoReceiveInvoiceForm : Page
    {
        readonly ICompanyCussent _companyCussent = new DAL.Implement.Inventory.CompanyCussent(GlobalConfig.DB.FromType.Read);
        readonly BLL.Implement.Inventory.CompanyFundReceipt _companyFundReceiptBll=new BLL.Implement.Inventory.CompanyFundReceipt(GlobalConfig.DB.FromType.Write);

        public Guid ReceiptId { get { return new Guid(Request.QueryString["ReceiptId"]); } }

        /// <summary>
        /// 是否为劳务费
        /// </summary>
        public bool IsService { get { return Request.QueryString["IsService"] !=null && Convert.ToBoolean(Request.QueryString["IsService"]); } }

        public bool IsShow
        {
            get { return ViewState["IsShwo"] != null && Convert.ToBoolean(ViewState["IsShwo"]); }
            set { ViewState["IsShwo"] = value; }
        }

        public CompanyFundReceiptInfo CompanyFundReceipt
        {
            get {
                if (ViewState["CompanyFundReceipt"] == null)
                {
                    var companyFundReceipt= _companyFundReceiptBll.GetCompanyFundReceiptInfo(ReceiptId);
                    ViewState["CompanyFundReceipt"] = companyFundReceipt;
                    return companyFundReceipt;
                }
                return (CompanyFundReceiptInfo)ViewState["CompanyFundReceipt"];
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                RbNormal.Checked = !IsService;
                RbService.Checked = IsService;
                RgInvoice.DataSource = new List<InvoiceRelationInfo> {new InvoiceRelationInfo(Guid.NewGuid())};
                RgInvoice.DataBind();

                var details = _companyFundReceiptBll.GetInvoiceRelationInfos(ReceiptId);
                BindInvoice(details);

                var invoiceTypes = Enum.Attribute.EnumAttribute.GetDict<ApplyInvoiceType>();
                if (IsService)
                {
                    RcbInvoiceType.Items.Add(new RadComboBoxItem(Enum.Attribute.EnumAttribute.GetKeyName<ApplyInvoiceType>(ApplyInvoiceType.Receipt), string.Format("{0}", (int)ApplyInvoiceType.Receipt)));
                }
                else
                {
                    foreach (var item in invoiceTypes)
                    {
                        if (item.Key == (int)ApplyInvoiceType.All || item.Key == (int)ApplyInvoiceType.Receipt) continue;
                        RcbInvoiceType.Items.Add(new RadComboBoxItem(item.Value, string.Format("{0}", item.Key)));
                    }
                }
                BindValue();
            }
            BindControl();
        }

        private void BindValue()
        {
            var companyFundRecipt = CompanyFundReceipt;
            RtbTradeCode.Text = companyFundRecipt.ReceiptNo;
            var filialeName= FilialeManager.GetName(companyFundRecipt.CompanyID);
            RtbCompany.Text = string.IsNullOrEmpty(filialeName) ? _companyCussent.GetCompanyCussent(companyFundRecipt.CompanyID).CompanyName : filialeName;
            RtbReciverUnit.Text = FilialeManager.GetName(companyFundRecipt.FilialeId);
            RtbUnit.Text = CompanyFundReceipt.InvoiceUnit;
        }

        private void BindControl()
        {
            if (IsService)
            {
                LbAmountOrDate.Text = "应收金额：";
                RtbAmountOrDate.Text = CompanyFundReceipt.RealityBalance.ToString("#0.00");
                RcbInvoiceType.SelectedValue = string.Format("{0}", (int) ApplyInvoiceType.Receipt);
                RcbInvoiceType.Enabled = false;
            }
            else
            {
                LbAmountOrDate.Text = "结账日期：";
                RtbAmountOrDate.Text =string.Format("{0}-{1}", CompanyFundReceipt.SettleStartDate.ToShortDateString(), CompanyFundReceipt.SettleEndDate.ToShortDateString()) ;
                RtbReceiver.Text = CompanyFundReceipt.RealityBalance.ToString("#0.00");
                RtbDisCount.Text = CompanyFundReceipt.DiscountMoney.ToString("#0.00");
                RcbInvoiceType.Enabled = true;
            }
            LbReceiver.Visible = !IsService;
            RtbReceiver.Visible = !IsService;
            LbDiscount.Visible = !IsService;
            RtbDisCount.Visible = !IsService;
            ShowColumns(!string.IsNullOrEmpty(RcbInvoiceType.SelectedValue) && RcbInvoiceType.SelectedValue == string.Format("{0}", (int)ApplyInvoiceType.Special));
        }

        protected void LbBackOncLick(object sender, EventArgs e)
        {
            var personnelInfo = CurrentSession.Personnel.Get();
            if (string.IsNullOrEmpty(RtbRefuseReason.Text))
            {
                RAM.Alert("请输入拒绝理由！");
                return;
            }
            var result = _companyFundReceiptBll.InvoiceBack(ReceiptId,string.Format("[操作人：{0}，操作时间：{1}][拒绝理由：{2}]", personnelInfo.RealName, DateTime.Now, RtbRefuseReason.Text));
            if(result)
                RAM.ResponseScripts.Add("CloseAndRebind()");
            else
                RAM.Alert("退回操作失败！");
        }

        protected void LbSaveOncLick(object sender, EventArgs e)
        {
            List<InvoiceRelationInfo> dataSource = new List<InvoiceRelationInfo>();
            if (string.IsNullOrEmpty(RtbUnit.Text))
            {
                RAM.Alert("开票单位不能为空！");
                return;
            }
            var isSpecial = !string.IsNullOrEmpty(RcbInvoiceType.SelectedValue) && RcbInvoiceType.SelectedValue == string.Format("{0}", (int)ApplyInvoiceType.Special);
            foreach (var item in RgInvoice.Items)
            {
                var dataItem = item as GridDataItem;
                if (dataItem != null)
                {
                    Guid invoiceId = new Guid(dataItem.GetDataKeyValue("InvoiceId").ToString());
                    var isCanEdit = Convert.ToBoolean(dataItem.GetDataKeyValue("IsCanEdit"));
                    var tbInvoiceNo = dataItem.FindControl("TbInvoiceNo") as TextBox;
                    var invoiceNo = tbInvoiceNo != null ? tbInvoiceNo.Text : string.Empty;
                    var tbInvoiceCode = dataItem.FindControl("TbInvoiceCode") as TextBox;
                    var invoiceCode = isSpecial && tbInvoiceCode != null  ? tbInvoiceCode.Text : string.Empty;
                    var tbRequestTime = dataItem.FindControl("RdpRequestTime") as RadDatePicker;
                    var requestTime = tbRequestTime != null && tbRequestTime.SelectedDate != null ? (DateTime)tbRequestTime.SelectedDate : DateTime.MinValue;
                    var tbUnTaxFee = dataItem.FindControl("TbUnTaxFee") as TextBox;
                    var unTaxFee = isSpecial && tbUnTaxFee != null ? tbUnTaxFee.Text : string.Empty;
                    var tbTaxFee = dataItem.FindControl("TbTaxFee") as TextBox;
                    var taxFee = isSpecial && tbTaxFee != null ? tbTaxFee.Text : string.Empty;
                    var tbTotalFee = dataItem.FindControl("TbTotalFee") as TextBox;
                    var totalFee = tbTotalFee != null ? tbTotalFee.Text : string.Empty;
                    var tbRemark = dataItem.FindControl("TbRemark") as TextBox;
                    if (!isCanEdit)
                    {
                        if (string.IsNullOrEmpty(invoiceNo) && requestTime == DateTime.MinValue && string.IsNullOrEmpty(totalFee))
                            continue;
                        if (string.IsNullOrEmpty(invoiceNo) || requestTime == DateTime.MinValue || string.IsNullOrEmpty(totalFee))
                        {
                            RAM.Alert("发票必填项(票据号码、开票日期、含税金额)");
                            return;
                        }
                    }
                    dataSource.Add(new InvoiceRelationInfo(invoiceId, ReceiptId, invoiceNo, string.Empty, requestTime, Convert.ToDecimal(unTaxFee.Length > 0 ? unTaxFee : "0"),
                            Convert.ToDecimal(taxFee.Length > 0 ? taxFee : "0"), Convert.ToDecimal(totalFee.Length > 0 ? totalFee : "0"), tbRemark.Text, isCanEdit));
                }
            }
            var personnelInfo = CurrentSession.Personnel.Get();
            var result = _companyFundReceiptBll.InvoicePass(ReceiptId, RtbUnit.Text, dataSource, string.Format("[开票人：{0}，开票时间：{1}]", personnelInfo.RealName, DateTime.Now),personnelInfo.PersonnelId);
            if (!result.IsSuccess)
            {
                RAM.Alert(result.Message);
                return;
            }
            RAM.ResponseScripts.Add("CloseAndRebind()");
        }

        protected void BtnExportOnClick(object sender, EventArgs e)
        {
            var workbook = new HSSFWorkbook();
            var ms = new MemoryStream();
            var sheet = new HSSFSheet[1];// 增加sheet。

            #region 
            sheet[0] = workbook.CreateSheet("收款发票模板" + DateTime.Now.ToString("yyyy-MM-dd"));//添加sheet名
            sheet[0].DefaultColumnWidth = 30;
            sheet[0].DefaultRowHeight = 20;

            HSSFRow rowtitle = sheet[0].CreateRow(0);
            HSSFCellStyle style = workbook.CreateCellStyle();
            style.Alignment = HSSFCellStyle.ALIGN_CENTER;
            HSSFFont font = workbook.CreateFont();
            font.FontHeightInPoints = 12;
            font.Color = HSSFColor.BLACK.index;
            font.Boldweight = 1;
            style.SetFont(font);
            #endregion

            int index = 0;
            for (var i = 0; i < RgInvoice.Columns.Count - 1; i++)
            {
                if (RgInvoice.Columns[i].Display)
                {
                    HSSFCell cell = rowtitle.CreateCell(index);
                    cell.SetCellValue(RgInvoice.Columns[i].HeaderText);
                    cell.CellStyle = style;
                    index++;
                }
            }

            sheet[0].DisplayGridlines = true;

            #region 输出
            workbook.Write(ms);
            Response.ContentEncoding = Encoding.GetEncoding("utf-8");
            Response.AddHeader("Content-Disposition", "attachment; filename=" + HttpUtility.UrlEncode("收款发票模板导出" + DateTime.Now.ToString("yyyyMMdd") + ".xls", Encoding.UTF8));
            Response.BinaryWrite(ms.ToArray());
            ms.Close();
            ms.Dispose();

            GC.Collect();
            #endregion
        }

        protected void BtnImportOnClick(object sender, EventArgs e)
        {
            var upl = FupFile.PostedFile;
            if (upl != null && !string.IsNullOrEmpty(upl.FileName))
            {
                var file = Path.GetExtension(upl.FileName);
                if (file != null && !file.Equals(".xls"))
                {
                    MessageBox.Show(this, "文件格式错误(.xls)！");
                    return;
                }
                #region 将上传文件保存至临时文件夹
                string fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + file;
                const string FOLDER_PATH = "~/UserDir/Invoice/";
                if (!Directory.Exists(Server.MapPath(FOLDER_PATH)))
                {
                    Directory.CreateDirectory(Server.MapPath(FOLDER_PATH));
                }
                string filePath = Server.MapPath(FOLDER_PATH + fileName);
                FupFile.PostedFile.SaveAs(filePath);

                List<InvoiceRelationInfo> dataSource = new List<InvoiceRelationInfo>();
                var fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                List<string> columns = new List<string>();
                for (var i = 0; i < RgInvoice.Columns.Count - 1; i++)
                {
                    columns.Add(RgInvoice.Columns[i].HeaderText);
                }
                HSSFWorkbook book = null;
                try
                {
                    book = new HSSFWorkbook(fs);
                    HSSFSheet sheet = book.GetSheetAt(0);
                    var isSpecial = !string.IsNullOrEmpty(RcbInvoiceType.SelectedValue) && RcbInvoiceType.SelectedValue==string.Format("{0}",(int)ApplyInvoiceType.Special);
                    for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
                    {
                        HSSFRow row = sheet.GetRow(i);
                        var requestTime = row.GetCell(GetColumnName(columns, 0)).DateCellValue;
                        var invoiceNo = GetCellValue(row.GetCell(GetColumnName(columns, 1)));
                        string invoiceCode = string.Empty;
                        double unTaxFee = 0;
                        double taxFee = 0;
                        double totalFee = 0;
                        string remark = string.Empty;
                        if (isSpecial)
                        {
                            invoiceCode = GetCellValue(row.GetCell(GetColumnName(columns, 2)));
                            totalFee = row.GetCell(GetColumnName(columns, 3)).NumericCellValue;
                            totalFee = row.GetCell(GetColumnName(columns, 4)).NumericCellValue;
                            totalFee = row.GetCell(GetColumnName(columns, 5)).NumericCellValue;
                            remark = GetCellValue(row.GetCell(GetColumnName(columns, 6)));
                        }
                        else
                        {
                            totalFee = row.GetCell(GetColumnName(columns, 2)).NumericCellValue;
                            remark = GetCellValue(row.GetCell(GetColumnName(columns, 3)));
                        }
                        dataSource.Add(new InvoiceRelationInfo(Guid.NewGuid(), ReceiptId,
                            invoiceNo,
                            invoiceCode,
                            Convert.ToDateTime(requestTime),
                            Convert.ToDecimal(unTaxFee),
                            Convert.ToDecimal(taxFee),
                            Convert.ToDecimal(totalFee),
                            remark));
                    }
                    foreach (var item in RgInvoice.Items)
                    {
                        var dataItem = item as GridDataItem;
                        if (dataItem != null)
                        {
                            Guid invoiceId = new Guid(dataItem.GetDataKeyValue("InvoiceId").ToString());
                            var isCanEdit = Convert.ToBoolean(dataItem.GetDataKeyValue("IsCanEdit"));
                            var tbInvoiceNo = dataItem.FindControl("TbInvoiceNo") as TextBox;
                            var tbInvoiceCode = dataItem.FindControl("TbInvoiceCode") as TextBox;
                            var tbRequestTime = dataItem.FindControl("RdpRequestTime") as RadDatePicker;
                            var tbUnTaxFee = dataItem.FindControl("TbUnTaxFee") as TextBox;
                            var tbTaxFee = dataItem.FindControl("TbTaxFee") as TextBox;
                            var tbTotalFee = dataItem.FindControl("TbTotalFee") as TextBox;
                            var tbRemark = dataItem.FindControl("TbRemark") as TextBox;
                            if (dataSource.Any(ent => ent.InvoiceNo.Equals(tbInvoiceNo.Text))) continue;
                            dataSource.Add(new InvoiceRelationInfo(invoiceId, ReceiptId, tbInvoiceNo.Text, tbInvoiceCode.Text, tbRequestTime.SelectedDate ?? DateTime.MinValue,
                                string.IsNullOrEmpty(tbUnTaxFee.Text) ? 0 : Convert.ToDecimal(tbUnTaxFee.Text),
                                    string.IsNullOrEmpty(tbTaxFee.Text) ? 0 : Convert.ToDecimal(tbTaxFee.Text),
                                    string.IsNullOrEmpty(tbTotalFee.Text) ? 0 : Convert.ToDecimal(tbTotalFee.Text), tbRemark.Text, isCanEdit));
                        }
                    }
                }
                catch (Exception ex)
                {
                    RAM.Alert(ex.Message);
                    return;
                }
                finally
                {
                    if (File.Exists(filePath))
                        File.Delete(filePath);
                }
                BindInvoice(dataSource);
                #endregion
            }
            else
            {
                RAM.Alert("请先选择文件！");
            }
        }

        private void BindInvoice(List<InvoiceRelationInfo> dataSource)
        {
            RgInvoice.DataSource = dataSource.Count > 0 ? dataSource : new List<InvoiceRelationInfo> { new InvoiceRelationInfo(ReceiptId) };
            RgInvoice.DataBind();
        }

        protected void BtnAddOnClick(object sender, EventArgs e)
        {
            List<InvoiceRelationInfo> dataSource = GetData();
            dataSource.Add(new InvoiceRelationInfo(ReceiptId));
            BindInvoice(dataSource);
        }

        protected void BtnDeleteOnClick(object sender, EventArgs e)
        {
            List<InvoiceRelationInfo> dataSource = new List<InvoiceRelationInfo>();
            var selectItem = ((Button)sender).Parent.Parent as GridDataItem;
            if (selectItem == null) return;
            Guid deleteId = new Guid(selectItem.GetDataKeyValue("InvoiceId").ToString()); ;
            foreach (var item in RgInvoice.Items)
            {
                var dataItem = item as GridDataItem;
                if (dataItem != null)
                {
                    Guid invoiceId = new Guid(dataItem.GetDataKeyValue("InvoiceId").ToString());
                    var isCanEdit = Convert.ToBoolean(dataItem.GetDataKeyValue("IsCanEdit"));
                    var tbInvoiceNo = dataItem.FindControl("TbInvoiceNo") as TextBox;
                    var tbInvoiceCode = dataItem.FindControl("TbInvoiceCode") as TextBox;
                    var tbRequestTime = dataItem.FindControl("RdpRequestTime") as RadDatePicker;
                    var unTaxFee = dataItem.FindControl("TbUnTaxFee") as TextBox;
                    var taxFee = dataItem.FindControl("TbTaxFee") as TextBox;
                    var tbTotalFee = dataItem.FindControl("TbTotalFee") as TextBox;
                    var tbRemark = dataItem.FindControl("TbRemark") as TextBox;
                    if (deleteId != invoiceId)
                    {
                        dataSource.Add(new InvoiceRelationInfo(invoiceId, ReceiptId, tbInvoiceNo.Text, tbInvoiceCode.Text, tbRequestTime.SelectedDate ?? DateTime.MinValue, 
                            string.IsNullOrEmpty(unTaxFee.Text) ? 0 : Convert.ToDecimal(unTaxFee.Text),
                           string.IsNullOrEmpty(taxFee.Text) ? 0 : Convert.ToDecimal(taxFee.Text), string.IsNullOrEmpty(tbTotalFee.Text) ? 0 : Convert.ToDecimal(tbTotalFee.Text), tbRemark.Text, isCanEdit));
                    }
                }
            }
            BindInvoice(dataSource);
        }

        private List<InvoiceRelationInfo> GetData()
        {
            List<InvoiceRelationInfo> dataSource = new List<InvoiceRelationInfo>();
            foreach (var item in RgInvoice.Items)
            {
                var dataItem = item as GridDataItem;
                if (dataItem != null)
                {
                    Guid invoiceId = new Guid(dataItem.GetDataKeyValue("InvoiceId").ToString());
                    var isCanEdit = Convert.ToBoolean(dataItem.GetDataKeyValue("IsCanEdit"));
                    var tbInvoiceNo = dataItem.FindControl("TbInvoiceNo") as TextBox;
                    var tbInvoiceCode = dataItem.FindControl("TbInvoiceCode") as TextBox;
                    var tbRequestTime = dataItem.FindControl("RdpRequestTime") as RadDatePicker;
                    var unTaxFee = dataItem.FindControl("TbUnTaxFee") as TextBox;
                    var taxFee = dataItem.FindControl("TbTaxFee") as TextBox;
                    var tbTotalFee = dataItem.FindControl("TbTotalFee") as TextBox;
                    var tbRemark = dataItem.FindControl("TbRemark") as TextBox;
                    dataSource.Add(new InvoiceRelationInfo(invoiceId, ReceiptId, tbInvoiceNo.Text, tbInvoiceCode.Text, tbRequestTime.SelectedDate ?? DateTime.MinValue,
                        string.IsNullOrEmpty(unTaxFee.Text) ? 0 : Convert.ToDecimal(unTaxFee.Text),
                           string.IsNullOrEmpty(taxFee.Text) ? 0 : Convert.ToDecimal(taxFee.Text), string.IsNullOrEmpty(tbTotalFee.Text) ? 0 : Convert.ToDecimal(tbTotalFee.Text), tbRemark.Text, isCanEdit));
                }
            }
            return dataSource;
        }


        private int GetColumnName(List<string> columns, int index)
        {
            if (columns.Count > index)
                return index;
            return -1;
        }

        private string GetCellValue(HSSFCell cell)
        {
            switch (cell.CellType)
            {
                case HSSFCell.CELL_TYPE_STRING: // 字符串
                    return cell.StringCellValue;
                case HSSFCell.CELL_TYPE_FORMULA:
                    return string.Format("{0}", cell.DateCellValue);
                case HSSFCell.CELL_TYPE_NUMERIC:
                    return string.Format("{0}", cell.NumericCellValue);
                default:
                    return string.Empty;
            }
        }

        protected DateTime? GetDate(object eval)
        {
            DateTime? selectDate = null;
            var date = Convert.ToDateTime(eval);
            if (date != DateTime.MinValue)
                selectDate = date;
            return selectDate;
        }

        protected void RcbInvoiceTypeOnSelectedIndexChanged(object o, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            BindInvoice(new List<InvoiceRelationInfo>());
        }

        private void ShowColumns(bool show)
        {
            RgInvoice.MasterTableView.GetColumn("InvoiceCode").Display = show;
            RgInvoice.MasterTableView.GetColumn("UnTaxFee").Display = show;
            RgInvoice.MasterTableView.GetColumn("TaxFee").Display = show;
        }
    }
}