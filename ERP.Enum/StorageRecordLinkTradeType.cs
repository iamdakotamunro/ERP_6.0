using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>▄︻┻┳═一 产生出入库单据对应枚举  ADD  陈重文  2014-12-25
    /// </summary>
    public enum StorageRecordLinkTradeType
    {
        /// <summary>采购单
        /// </summary>
        [Enum("采购单")]
        Purchasing = 1,

        /// <summary>订单
        /// </summary>
        [Enum("订单")]
        GoodsOrder = 2,

        /// <summary> 售后单
        /// </summary>
        [Enum("售后单")]
        AfterSaleGoodsOrder = 3,

        /// <summary> 调拨单
        /// </summary>
        [Enum("调拨单")]
        Allot = 4,

        /// <summary> 红冲
        /// </summary>
        [Enum("红冲")]
        Cancellation = 5,

        /// <summary> 其他
        /// </summary>
        [Enum("其他")]
        Other = 6,
    }
}
