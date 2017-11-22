using System;

namespace ERP.Model.Goods
{
    /// <summary>
    /// 商品出入库汇总统计信息
    /// </summary>
    [Serializable]
    public class GoodsOutInStorageStatisticsInfo
    {
        /// <summary>
        /// 商品ID
        /// </summary>
        public Guid GoodsId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string GoodsName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// 次数
        /// </summary>
        public int Time { get; set; }

        /// <summary>
        /// 近期低价
        /// </summary>
        public decimal LowerPrice { get; set; }

        /// <summary>
        /// 单价
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 总金额
        /// </summary>
        public decimal TotalPrice
        {
            get { return Price * Quantity; }
        }

        /// <summary>
        /// 出入库类型
        /// </summary>
        public int StorageType { get; set; }

        public Guid FilialeId { get; set; }

        public bool IsOut { get; set; }
    }
}
