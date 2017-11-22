using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ERP.Model
{
    public class TempOrderDetailInfo
    {
        public Guid OrderId { get; set; }

        public Guid GoodsId { get; set; }

        public Guid RealGoodsId { get; set; }

        public decimal SellPrice { get; set; }

        public Int32 Quantity { get; set; }

        public decimal OriginalSellPrice { get; set; }
    }
}
