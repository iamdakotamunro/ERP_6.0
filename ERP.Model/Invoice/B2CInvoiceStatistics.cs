using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ERP.Model.Invoice
{
    [Serializable]
    public class B2CInvoiceStatistics
    {
        public int TotalCount { get; set; }

        public List<B2CInvoiceDTO> DataList { get; set; }

        public decimal RedSumAmount { get; set; }

        public decimal BlueSumAmount { get; set; }

        public int RedCount { get; set; }

        public int BlueCount { get; set; }

        public int TotalPage { get; set; }
    }
}
