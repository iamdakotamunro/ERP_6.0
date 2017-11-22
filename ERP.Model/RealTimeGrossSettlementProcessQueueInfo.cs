using System;

namespace ERP.Model
{
    /// <summary>
    /// 即时结算价处理队列（处理完后会删除），参见<see cref="RealTimeGrossSettlementInfo"/>
    /// </summary>
    [Serializable]
    public class RealTimeGrossSettlementProcessQueueInfo
    {
        /// <summary>
        /// 唯一的队列ID
        /// </summary>
        public Guid QueueId { get; set; }

        /// <summary>
        /// 采购方公司ID
        /// </summary>
        public Guid FilialeId { get; set; }

        /// <summary>
        /// 主商品ID
        /// </summary>
        public Guid GoodsId { get; set; }

        /// <summary>
        /// 库存数
        /// </summary>
        public int StockQuantity { get; set; }


        /// <summary>
        /// 上次结算价信息
        /// </summary>
        public RealTimeGrossSettlementInfo LastGrossSettlement { get; set; }

        /// <summary>
        /// 上次的即时结算价
        /// </summary>
        public decimal LastUnitPrice
        {
            get { return LastGrossSettlement == null ? 0 : LastGrossSettlement.UnitPrice; }
        }

        /// <summary>
        /// 单子上的商品数（此为非负整数）
        /// </summary>
        public int GoodsQuantityInBill { get; set; }

        /// <summary>
        /// 单子上的总金额（允许负数，如入库单红冲低于当初采购、采购退货出库，总金额为负数）
        /// </summary>
        public decimal GoodsAmountInBill { get; set; }

        /// <summary>
        /// 扩展字段1
        /// </summary>
        public decimal ExtField_1 { get; set; }

        /// <summary>
        /// 关联单据类型 参照<see cref="ERP.Enum.RealTimeGrossSettlementRelatedTradeType"/>
        /// </summary>
        public int RelatedTradeType { get; set; }

        /// <summary>
        /// 关联单据号
        /// </summary>
        public string RelatedTradeNo { get; set; }

        /// <summary>
        /// 发生时间
        /// </summary>
        public DateTime OccurTime { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}
