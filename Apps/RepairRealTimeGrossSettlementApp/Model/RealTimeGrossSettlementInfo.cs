using System;

namespace RepairRealTimeGrossSettlementApp.Model
{
    /// <summary>
    /// 即时结算价（每个公司，针对每个主商品的即时结算价）
    /// </summary>
    [Serializable]
    public class RealTimeGrossSettlementInfo
    {
        /// <summary>
        /// 采购方公司ID
        /// </summary>
        public Guid FilialeId { get; set; }

        /// <summary>
        /// 主商品ID
        /// </summary>
        public Guid GoodsId { get; set; }

        /// <summary>
        /// 单价
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// 总库存数
        /// </summary>
        public int StockQuantity { get; set; }

        /// <summary>
        /// 单子上的商品数
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
        /// 关联单据类型
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
