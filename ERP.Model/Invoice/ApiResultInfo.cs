using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ERP.Model.Invoice
{
    [Serializable]
    public class ApiResultInfo
    {
        public B2CInvoiceStatistics Data { get; set; }

        public bool State { get; set; }

        public string Msg { get; set; }
    }
}
