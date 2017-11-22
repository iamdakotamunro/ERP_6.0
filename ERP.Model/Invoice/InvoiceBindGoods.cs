using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Model.Invoice
{
    [Serializable]
    public class InvoiceBindGoods
    {
        public int Id { get; set; }

        public Guid InvoiceId { get; set; }

        public Guid GoodsId { get; set; }

        public string GoodsCode { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice  { get; set; }
    }
}
