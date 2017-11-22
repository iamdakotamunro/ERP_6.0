using System;

namespace ERP.Model.Report
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class MonthGoodsStockInfo
    {
        public Guid GoodsId { get; set; }

        public Guid RealGoodsId { get; set; }

        public int Quantity { get; set; }

        public decimal TotalPrice { get; set; }

        public int StockType { get; set; }
    }
}
