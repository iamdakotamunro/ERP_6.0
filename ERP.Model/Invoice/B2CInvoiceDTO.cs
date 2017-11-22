using System;

namespace ERP.Model.Invoice
{
    [Serializable]
    public class B2CInvoiceDTO
    {
        public Guid OrderId { get; set; }

        public string OrderNo { get; set; }

        public DateTime BillingDate { get; set; }

        public string InvoiceTitle { get; set; }

        public string InvoiceCode { get; set; }

        public string InvoiceNumber { get; set; }

        public string OriginalInvoiceCode { get; set; }

        public string OriginalInvoiceNumber { get; set; }

        public int BillingType { get; set; }

        public decimal NoTaxAmount { get; set; }

        public decimal Taxes { get; set; }

        public decimal Amount { get; set; }

        public string Remark { get; set; }
    }
}
