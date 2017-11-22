using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using ERP.BLL.Implement.Inventory;
using ERP.BLL.Implement.Organization;
using ERP.Enum.ApplyInvocie;
using ERP.Enum.Attribute;
using ERP.Model.Invoice;
using ERP.Model.Finance;
using ERP.UI.Web.Common;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using Telerik.Web.UI;

namespace ERP.UI.Web.Invoices
{
    public partial class ApprovalLeagueInvoiceForm : System.Web.UI.Page
    {
        private readonly InvoiceApplySerivce _invoiceApplySerivce = new InvoiceApplySerivce();

        public Guid ApplyId { get { return new Guid(Request.QueryString["ApplyId"]); } }

        /// <summary>
        /// 0 审核 1 开票
        /// </summary>
        public bool IsAudit { get { return Request.QueryString["Type"] == "0"; } }

        public bool IsReceipt
        {
            get { return ViewState["IsReceipt"] != null && Convert.ToBoolean(ViewState["IsReceipt"]); }
            set { ViewState["IsReceipt"] = value; }
        }

        public bool IsCompany
        {
            get { return ViewState["IsCompany"] != null && Convert.ToBoolean(ViewState["IsCompany"]); }
            set { ViewState["IsCompany"] = value; }
        }

        public bool IsSpecial
        {
            get { return ViewState["IsSpecial"] != null && Convert.ToBoolean(ViewState["IsSpecial"]); }
            set { ViewState["IsSpecial"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var applyInfo = _invoiceApplySerivce.GetInvoiceApplyInfo(ApplyId);
                if (applyInfo!=null)
                {
                    IsCompany= applyInfo.InvoiceTitleType == (Byte)ApplyInvoiceTitleType.Company;
                    IsSpecial = applyInfo.ApplyType == (int)ApplyInvoiceType.Special;
                    RbSpecial.Checked = applyInfo.ApplyKind==(int)ApplyInvoiceKindType.Bail;
                    RbNormal.Checked = applyInfo.ApplyKind == (int)ApplyInvoiceKindType.Credit;
                    if (applyInfo.ApplyType == (int) ApplyInvoiceType.Receipt)
                    {
                        IsReceipt = true;
                        RgSpecial.DataSource = applyInfo.Details;
                        RgSpecial.DataBind();
                    }
                    else
                    {
                        IsReceipt = false;
                        RgNormal.DataSource = applyInfo.Details;
                        RgNormal.DataBind();
                    }
                    if (!IsAudit)
                        BindInvoice(applyInfo.RelationInfos);
                    BindValue(applyInfo);
                    RAM.ResponseScripts.Add(string.Format("load({0},{1},{2});", IsCompany ? 0 : 1, IsAudit ? 0 : 1, IsReceipt ? 1 : 0));
                }
            }
            else
            {
                RAM.ResponseScripts.Add(string.Format("load({0},{1},{2});", IsCompany?0:1, IsAudit ? 0 : 1, IsReceipt ? 1 : 0));
            }
        }

        private void BindValue(InvoiceApplyInfo applyInfo)
        {
            TbAddress.Text = applyInfo.Address;
            TbTelephone.Text = applyInfo.Telephone;
            TbReceiver.Text = applyInfo.Receiver;
            TbApplyRemark.Text = applyInfo.ApplyRemark;
            TbRetreat.Text = applyInfo.RetreatRemark;
            TbLeagueName.Text = FilialeManager.GetName(applyInfo.TargetId);
            TbInvoiceType.Text = EnumAttribute.GetKeyName((ApplyInvoiceType) applyInfo.ApplyType);
            TbTitleType.Text = EnumAttribute.GetKeyName((ApplyInvoiceTitleType)applyInfo.InvoiceTitleType);
            TbTitle.Text = applyInfo.Title;
            TbPerTitle.Text = applyInfo.Title;
            BtSave.Text = IsAudit ? "核准" : "开票";
            BtBack.Text = IsAudit ? "核退" : "退回";
            if (applyInfo.InvoiceTitleType == (int) ApplyInvoiceTitleType.Company)
            {
                TbContactAddress.Text = applyInfo.ContactAddress;
                TbContactPhone.Text = applyInfo.ContactTelephone;
                TbTaxNumber.Text = applyInfo.TaxpayerNumber;
                TbBankName.Text = applyInfo.BankName;
                TbBankAccountNo.Text = applyInfo.BankAccountNo;
            }
        }

        private void BindInvoice(List<InvoiceRelationInfo> dataSource)
        {
            RgInvoice.MasterTableView.GetColumn("UnTaxFee").Display = IsSpecial;
            RgInvoice.MasterTableView.GetColumn("TaxFee").Display = IsSpecial;
            RgInvoice.MasterTableView.GetColumn("InvoiceCode").Display = IsSpecial;

            RgInvoice.DataSource = dataSource.Count > 0 ? dataSource : new List<InvoiceRelationInfo> { new InvoiceRelationInfo(ApplyId) };
            RgInvoice.DataBind();
        }

        protected void BtBackClick(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(TbRetreat.Text))
            {
                RAM.Alert(string.Format("请输入{0}原因！", IsAudit ? "核退" : "退回"));
                return;
            }
            var personnelInfo = CurrentSession.Personnel.Get();
            var result = IsAudit ? _invoiceApplySerivce.Retreat(personnelInfo.PersonnelId, personnelInfo.RealName, ApplyId,TbRetreat.Text) :
                _invoiceApplySerivce.InvoiceReturn(personnelInfo.PersonnelId, personnelInfo.RealName, ApplyId,  TbRetreat.Text);
            if (!result.IsSuccess)
            {
                RAM.Alert(result.Message);
                return;
            }
            RAM.ResponseScripts.Add("CloseAndRebind()");
        }

        protected void BtSaveClick(object sender, EventArgs e)
        {
            ResultInfo result;
            var personnelInfo = CurrentSession.Personnel.Get();
            if (IsAudit)
            {
                result = _invoiceApplySerivce.Approval(personnelInfo.PersonnelId, personnelInfo.RealName, ApplyId);
                if (!result.IsSuccess)
                {
                    RAM.Alert(result.Message);
                    return;
                }
            }
            else
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
                        var invoiceNo = tbInvoiceNo != null ? tbInvoiceNo.Text : string.Empty;
                        var tbInvoiceCode = dataItem.FindControl("TbInvoiceCode") as TextBox;
                        var invoiceCode = IsSpecial && tbInvoiceCode != null ? tbInvoiceCode.Text : string.Empty;
                        var tbRequestTime = dataItem.FindControl("RdpRequestTime") as RadDatePicker;
                        var requestTime = tbRequestTime != null && tbRequestTime.SelectedDate != null ? (DateTime)tbRequestTime.SelectedDate : DateTime.MinValue;
                        var tbUnTaxFee = dataItem.FindControl("TbUnTaxFee") as TextBox;
                        var unTaxFee = IsSpecial && tbUnTaxFee != null ? tbUnTaxFee.Text : string.Empty;
                        var tbTaxFee = dataItem.FindControl("TbTaxFee") as TextBox;
                        var taxFee = IsSpecial && tbTaxFee != null ? tbTaxFee.Text : string.Empty;
                        var tbTotalFee = dataItem.FindControl("TbTotalFee") as TextBox;
                        var totalFee = tbTotalFee != null ? tbTotalFee.Text : string.Empty;
                        var tbRemark = dataItem.FindControl("TbRemark") as TextBox;
                        if (!isCanEdit)
                        {
                            if (string.IsNullOrEmpty(invoiceNo) && string.IsNullOrEmpty(invoiceCode) && requestTime == DateTime.MinValue && string.IsNullOrEmpty(totalFee))
                                continue;
                            if (string.IsNullOrEmpty(invoiceNo) || string.IsNullOrEmpty(invoiceCode) ||
                                requestTime == DateTime.MinValue || string.IsNullOrEmpty(totalFee))
                            {
                                RAM.Alert("发票必填项(发票号码、发票代码、开票日期、含税金额)");
                                return;
                            }
                        }
                        dataSource.Add(new InvoiceRelationInfo(invoiceId, ApplyId, invoiceNo, invoiceCode, requestTime, Convert.ToDecimal(unTaxFee.Length > 0 ? unTaxFee : "0"),
                                Convert.ToDecimal(taxFee.Length > 0 ? taxFee : "0"), Convert.ToDecimal(totalFee.Length > 0 ? totalFee : "0"), tbRemark.Text, isCanEdit));
                    }
                }
                result = _invoiceApplySerivce.InvoicePass(ApplyId, dataSource,personnelInfo.PersonnelId, personnelInfo.RealName);
            }
            if (!result.IsSuccess)
            {
                RAM.Alert(result.Message);
                return;
            }
            RAM.ResponseScripts.Add("CloseAndRebind()");
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
                    var tbUnTaxFee = dataItem.FindControl("TbUnTaxFee") as TextBox;
                    var tbTaxFee = dataItem.FindControl("TbTaxFee") as TextBox;
                    var tbTotalFee = dataItem.FindControl("TbTotalFee") as TextBox;
                    var tbRemark = dataItem.FindControl("TbRemark") as TextBox;
                    if (deleteId != invoiceId)
                    {
                        dataSource.Add(new InvoiceRelationInfo(invoiceId, ApplyId, tbInvoiceNo.Text, tbInvoiceCode.Text, tbRequestTime.SelectedDate ?? DateTime.MinValue, string.IsNullOrEmpty(tbUnTaxFee.Text) ? 0 : Convert.ToDecimal(tbUnTaxFee.Text),
                            string.IsNullOrEmpty(tbTaxFee.Text) ? 0 : Convert.ToDecimal(tbTaxFee.Text), string.IsNullOrEmpty(tbTotalFee.Text) ? 0 : Convert.ToDecimal(tbTotalFee.Text), tbRemark.Text, isCanEdit));
                    }
                }
            }
            BindInvoice(dataSource);
        }

        protected void BtnAddOnClick(object sender, EventArgs e)
        {
            List<InvoiceRelationInfo> dataSource = GetData();
            dataSource.Add(new InvoiceRelationInfo(ApplyId));
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

                    var tbUnTaxFee = dataItem.FindControl("TbUnTaxFee") as TextBox;
                    var tbTaxFee = dataItem.FindControl("TbTaxFee") as TextBox;
                    var tbTotalFee = dataItem.FindControl("TbTotalFee") as TextBox;
                    var tbRemark = dataItem.FindControl("TbRemark") as TextBox;
                    dataSource.Add(new InvoiceRelationInfo(invoiceId, ApplyId, tbInvoiceNo.Text, tbInvoiceCode.Text, tbRequestTime.SelectedDate ?? DateTime.MinValue, string.IsNullOrEmpty(tbUnTaxFee.Text) ? 0 : Convert.ToDecimal(tbUnTaxFee.Text),
                            string.IsNullOrEmpty(tbTaxFee.Text) ? 0 : Convert.ToDecimal(tbTaxFee.Text), string.IsNullOrEmpty(tbTotalFee.Text) ? 0 : Convert.ToDecimal(tbTotalFee.Text), tbRemark.Text, isCanEdit));
                }
            }
            return dataSource;
        }

        protected void BtnImportOnClick(object sender, EventArgs e)
        {
            var upl = FupFile.PostedFile;
            if (upl!=null && !string.IsNullOrEmpty(upl.FileName))
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
                        if (IsSpecial)
                        {
                            invoiceCode = GetCellValue(row.GetCell(GetColumnName(columns, 2)));
                            unTaxFee = row.GetCell(GetColumnName(columns, 3)).NumericCellValue;
                            taxFee = row.GetCell(GetColumnName(columns, 4)).NumericCellValue;
                            totalFee = row.GetCell(GetColumnName(columns, 5)).NumericCellValue;
                            remark = GetCellValue(row.GetCell(GetColumnName(columns, 6)));
                        }
                        else
                        {
                            totalFee = row.GetCell(GetColumnName(columns, 2)).NumericCellValue;
                            remark = GetCellValue(row.GetCell(GetColumnName(columns, 3)));
                        }
                        dataSource.Add(new InvoiceRelationInfo(Guid.NewGuid(), ApplyId,
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
                            dataSource.Add(new InvoiceRelationInfo(invoiceId, ApplyId, tbInvoiceNo.Text, IsSpecial?tbInvoiceCode.Text:string.Empty, tbRequestTime.SelectedDate ?? DateTime.MinValue, string.IsNullOrEmpty(tbUnTaxFee.Text) ? 0 : Convert.ToDecimal(tbUnTaxFee.Text),
                                    string.IsNullOrEmpty(tbTaxFee.Text) ? 0 : Convert.ToDecimal(tbTaxFee.Text), string.IsNullOrEmpty(tbTotalFee.Text) ? 0 : Convert.ToDecimal(tbTotalFee.Text), tbRemark.Text, isCanEdit));
                        }
                    }
                }
                catch(Exception ex)
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
                    return  string.Format("{0}",cell.NumericCellValue);
                default:
                    return string.Empty;
            }
        }

        protected void BtnExportOnClick(object sender, EventArgs e)
        {
            var workbook = new HSSFWorkbook();
            var ms = new MemoryStream();
            var sheet = new HSSFSheet[1];// 增加sheet。

            #region 
            sheet[0] = workbook.CreateSheet("手工发票模板" + DateTime.Now.ToString("yyyy-MM-dd"));//添加sheet名
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
            for (var i=0;i< RgInvoice.Columns.Count-1;i++)
            {
                if (!RgInvoice.Columns[i].Display)
                    continue;
                HSSFCell cell = rowtitle.CreateCell(index);
                cell.SetCellValue(RgInvoice.Columns[i].HeaderText);
                cell.CellStyle = style;
                index++;
            }

            sheet[0].DisplayGridlines = true;

            #region 输出
            workbook.Write(ms);
            Response.ContentEncoding = Encoding.GetEncoding("utf-8");
            Response.AddHeader("Content-Disposition", "attachment; filename=" + HttpUtility.UrlEncode("手工发票模板导出" + DateTime.Now.ToString("yyyyMMdd") + ".xls", Encoding.UTF8));
            Response.BinaryWrite(ms.ToArray());
            ms.Close();
            ms.Dispose();

            GC.Collect();
            #endregion
        }

        protected DateTime? GetDate(object eval)
        {
            DateTime? selectDate=null;
            var date = Convert.ToDateTime(eval);
            if (date != DateTime.MinValue)
                selectDate = date;
            return selectDate;
        }
    }
}