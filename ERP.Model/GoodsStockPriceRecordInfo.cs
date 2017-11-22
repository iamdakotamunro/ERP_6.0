using System;

namespace ERP.Model
{
    /// <summary>
    /// 库存结算价模型
    /// </summary>
    [Serializable]
    public class GoodsStockPriceRecordInfo
    {
        /// <summary>
        /// 商品名称
        /// </summary>
        public Guid GoodsId { get; set; }

        /// <summary>
        /// 记录时间/年-月
        /// </summary>
        public DateTime DayTime { get; set; }

        /// <summary>
        /// 结算价
        /// </summary>
        public decimal AvgSettlePrice { get; set; }

        /// <summary>
        /// 月均进货价
        /// </summary>
        public decimal MonthAvgPrice { get; set; }
    }
}
