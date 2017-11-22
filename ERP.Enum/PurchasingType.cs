using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>
    /// 采购类型枚举
    /// </summary>
    public enum PurchasingType
    {
        /// <summary>
        /// 所有
        /// </summary>
        [Enum("所有")]
        All = -1,

        /// <summary>
        /// 进货申报
        /// </summary>
        [Enum("进货申报")]
        StockDeclare = 0,

        /// <summary>
        /// 自动报备
        /// </summary>
        [Enum("自动报备")]
        AutoStock = 1,

        /// <summary>
        /// 自定义
        /// </summary>
        [Enum("自定义")]
        Custom = 2,

        [Enum("自动内部采购")]
        AutoInternal = 3
    }
}
