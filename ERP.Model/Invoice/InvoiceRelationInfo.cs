using System;

namespace ERP.Model.Invoice
{
    [Serializable]
    public class InvoiceRelationInfo
    {
        public Guid InvoiceId { get; set; }

        public Guid ApplyId { get; set; }

        public string InvoiceNo { get; set; }

        public string InvoiceCode { get; set; }

        public DateTime RequestTime { get; set; }

        public decimal UnTaxFee { get; set; }

        public decimal TaxFee { get; set; }

        public decimal TotalFee { get; set; }

        public string Remark { get; set; }

        public bool IsCanEdit { get; set; }

        public InvoiceRelationInfo() { }

        public InvoiceRelationInfo(Guid applyId)
        {
            InvoiceId = Guid.NewGuid();
            ApplyId = applyId;
            InvoiceNo = string.Empty;
            InvoiceCode = string.Empty;
            RequestTime = DateTime.MinValue;
            Remark = string.Empty;
            IsCanEdit = true;
        }

        public InvoiceRelationInfo(Guid invoiceId,Guid applyId)
        {
            InvoiceId = invoiceId;
            InvoiceNo = string.Empty;
            InvoiceCode = string.Empty;
            RequestTime = DateTime.MinValue;
            Remark = string.Empty;
            ApplyId = applyId;
            IsCanEdit = true;
        }

        public InvoiceRelationInfo(Guid invoiceId,Guid applyId, string invoiceNo, string invoiceCode, DateTime requestTime, decimal unTaxFee, decimal taxFee, decimal totalFee, string remark,bool isCanEdit=true)
        {
            InvoiceId = invoiceId;
            InvoiceNo = invoiceNo??string.Empty;
            InvoiceCode = invoiceCode??string.Empty;
            RequestTime = requestTime;
            UnTaxFee = unTaxFee;
            TaxFee = taxFee;
            TotalFee = totalFee;
            Remark = remark??string.Empty;
            ApplyId = applyId;
            IsCanEdit = isCanEdit;
        }
    }
}
