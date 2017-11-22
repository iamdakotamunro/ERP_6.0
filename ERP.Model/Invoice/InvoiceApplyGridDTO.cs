using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Model.Invoice
{
    [Serializable]
    public class InvoiceApplyGridDTO
    {
        public Guid ApplyId { get; set; }

        public string TradeCode { get; set; }

        public DateTime ApplyDateTime { get; set; }

        public int InvoiceType { get; set; }

        public string InvoiceTypeName { get; set; }

        public string Receiver { get; set; }

        public decimal Amount { get; set; }

        public string ApplyState { get; set; }

        public string TargetName { get; set; }    

        public bool IsCanEdit { get; set; }

        public bool IsCanCancel { get; set; }
    }
}
