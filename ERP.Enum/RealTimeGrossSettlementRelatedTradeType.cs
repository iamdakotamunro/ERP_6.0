using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>
    /// 即时结算价关联的单据类型
    /// </summary>
    public enum RealTimeGrossSettlementRelatedTradeType
    {
        /// <summary>
        /// 采购入库单
        /// </summary>
        [Enum("采购入库单")]
        PurchaseStockIn = 1,

        /// <summary>
        /// 采购退货出库单
        /// </summary>
        [Enum("采购退货出库单")]
        PurchaseReturnStockOut = 2,

        /// <summary>
        /// 入库单红冲
        /// </summary>
        [Enum("入库单红冲")]
        StockInFormDashAtRed = 3,

        /// <summary>
        /// 拆分组合（来自WMS系统）
        /// </summary>
        [Enum("拆分组合")]
        CombineSplit = 4
    }
}
