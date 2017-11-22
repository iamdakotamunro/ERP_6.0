using System;

namespace AutoPurchasing.Core.Model
{
    public class ChildGoodsSale
    {
        /// <summary>
        /// 商品ID
        /// </summary>
        public Guid GoodsId { get; set; }

        public Guid HostingFilialeId { get; set; }

        /// <summary>
        /// 销售统计
        /// </summary>
        public double SaleQuantity { get; set; }

        /// <summary>
        /// 规格
        /// </summary>
        public string Specification { get; set; }

        public DateTime DayTime { get; set; }
    }
}
