using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RebuildPurchaseFormOfSaleFromHostingApp.Model
{
    [Serializable]
    public class SaleFilialeStockInDetail
    {
        public SaleFilialeStockInDetail() { }

        public Guid GoodsId { get; set; }

        public Guid RealGoodsId { get; set; }

        public string GoodsCode { get; set; }

        public string GoodsName { get; set; }

        public string Specification { get; set; }

        public int Quantity { get; set; }

        public decimal Amount { get; set; }
    }
}
