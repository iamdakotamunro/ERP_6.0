using System;

namespace ERP.Model
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class CheckStorageStatisticsInfo
    {
        /// <summary>
        /// 商品ID
        /// </summary>
        public Guid RealGoodsId { get; set; }

        /// <summary>
        /// 商品
        /// </summary>
        public string GoodsName { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public Guid WarehouseId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string WarehouseName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int StockType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int TotalQuantity { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal RecentPurchasingPrice { get; set; }
    }
}
