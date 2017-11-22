using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Model
{
    [Serializable]
    public class SourceBindGoods
    {
        public string TradeCode { get; set; }

        public string LinkTradeCode { get; set; }

        public Guid GoodsId { get; set; }

        public string GoodsCode { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }
    }
}
