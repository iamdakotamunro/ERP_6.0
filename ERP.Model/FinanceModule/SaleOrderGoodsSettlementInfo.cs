using System;

namespace ERP.Model.FinanceModule
{
    /// <summary>
    /// 销售订单商品结算价信息
    /// </summary>
    [Serializable]
    public class SaleOrderGoodsSettlementInfo
    {
        /// <summary>
        /// 公司ID
        /// </summary>
        public Guid FilialeId { get; set; }

        /// <summary>
        /// 主商品ID
        /// </summary>
        public Guid GoodsId  { get; set; }

        /// <summary>
        /// 结算价
        /// </summary>
        public decimal SettlementPrice { get; set; }

        /// <summary>
        /// 关联单据号
        /// </summary>
        public string RelatedTradeNo { get; set; }

        /// <summary>
        /// 关联单据类型<see cref="ERP.Enum.SaleOrderGoodsSettlementRelatedTradeType"/>
        /// </summary>
        public int RelatedTradeType { get; set; }

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
