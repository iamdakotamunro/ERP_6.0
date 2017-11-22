using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>▄︻┻┳═一 产生往来帐单据对应枚举  ADD  陈重文  2014-12-25
    /// </summary>
    public enum ReckoningLinkTradeType
    {
        /// <summary>入库单
        /// </summary>
        [Enum("入库单")]
        StockIn = 1,

        /// <summary>出库单
        /// </summary>
        [Enum("出库单")]
        StockOut = 2,

        /// <summary> 订单
        /// </summary>
        [Enum("订单")]
        GoodsOrder = 3,

        /// <summary> 快递
        /// </summary>
        [Enum("快递")]
        Express = 4,

        /// <summary> 往来单位收付款
        /// </summary>
        [Enum("往来单位收付款")]
        CompanyFundReceipt= 5,

        /// <summary> 其他
        /// </summary>
        [Enum("其他")]
        Other =6,

        /// <summary> 采购单
        /// </summary>
        [Enum("采购单")]
        PurchasingNo = 7,

        /// <summary> 门店充值
        /// </summary>
        [Enum("门店充值")]
        Recharge = 8
    }
}
