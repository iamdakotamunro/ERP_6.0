using System;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Enum.Attribute;
using ERP.Environment;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;

namespace ERP.UI.Web.Windows
{
    public partial class InvoiceEdit : WindowsPage
    {
        private readonly ICompanyFundReceiptInvoice _companyFundReceiptInvoice = new CompanyFundReceiptInvoice(GlobalConfig.DB.FromType.Write);
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var invoiceId = Request.QueryString["InvoiceId"];
                if (!string.IsNullOrEmpty(invoiceId))
                {
                    var receiptInvoice = _companyFundReceiptInvoice.Getlmshop_CompanyFundReceiptInvoiceByInvoiceId(new Guid(invoiceId));
                    txt_BillingUnit.Text = receiptInvoice.BillingUnit;
                    txt_BillingDate.Text = Convert.ToDateTime(receiptInvoice.BillingDate).ToString("yyyy-MM-dd");
                    txt_InvoiceNo.Text = receiptInvoice.InvoiceNo;
                    txt_InvoiceCode.Text = receiptInvoice.InvoiceCode;
                    txt_NoTaxAmount.Text = WebControl.RemoveDecimalEndZero(receiptInvoice.NoTaxAmount);
                    txt_Tax.Text = WebControl.RemoveDecimalEndZero(receiptInvoice.Tax);
                    txt_TaxAmount.Text = WebControl.RemoveDecimalEndZero(receiptInvoice.TaxAmount);
                    txt_InvoiceState.Text = EnumAttribute.GetKeyName((CompanyFundReceiptInvoiceState)Convert.ToInt32(receiptInvoice.InvoiceState));
                    txt_Memo.Text = receiptInvoice.Memo;
                }
            }
        }

        //保存
        protected void btn_Save_Click(object sender, EventArgs e)
        {
            var invoiceId = Request.QueryString["InvoiceId"];
            if (!string.IsNullOrEmpty(invoiceId))
            {
                var receiptInvoice = _companyFundReceiptInvoice.Getlmshop_CompanyFundReceiptInvoiceByInvoiceId(new Guid(invoiceId));
                receiptInvoice.BillingUnit = txt_BillingUnit.Text;
                receiptInvoice.BillingDate = DateTime.Parse(txt_BillingDate.Text);
                receiptInvoice.InvoiceNo = txt_InvoiceNo.Text;
                receiptInvoice.InvoiceCode = txt_InvoiceCode.Text;
                receiptInvoice.Memo = txt_Memo.Text;
                var result = _companyFundReceiptInvoice.Updatelmshop_CompanyFundReceiptInvoiceByInvoiceId(receiptInvoice);
                if (result)
                {
                    string remark = WebControl.RetrunUserAndTime("【修改发票】");
                    _companyFundReceiptInvoice.Updatelmshop_CompanyFundReceiptInvoiceRemarkByInvoiceId(remark, new Guid(invoiceId));
                    MessageBox.AppendScript(this, "setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
                }
            }
        }
    }
}