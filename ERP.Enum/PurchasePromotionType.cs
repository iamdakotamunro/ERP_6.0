using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>
    /// 采购促销类型
    /// </summary>
    public enum PurchasePromotionType
    {
        /// <summary>
        /// 无
        /// </summary>
        [Enum("无")]
        None = 0,

        /// <summary>
        /// 现返
        /// </summary>
        [Enum("现返")]
        Back = 1,

        /// <summary>
        /// 非现返
        /// </summary>
        [Enum("非现返")]
        NoBack = 2,
    }
}
