using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>
    /// 采购单商品类型枚举
    /// </summary>
    public enum PurchasingGoodsType
    {
        /// <summary>
        /// 未定价
        /// </summary>
        [Enum("未定价")]
        NoSet = -1,

        /// <summary>
        /// 非赠品
        /// </summary>
        [Enum("非赠品")]
        NoGift = 0,

        /// <summary>
        /// 赠品
        /// </summary>
        [Enum("赠品")]
        Gift = 1,
    }
}
