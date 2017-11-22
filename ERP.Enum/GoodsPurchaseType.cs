using System;
using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>
    /// 商品采购类型枚举
    /// </summary>
    [Serializable]
    public enum GoodsPurchaseType
    {
        /// <summary>
        /// 正常
        /// </summary>
        [Enum("正常")]
        Normal = 0,
        /// <summary>
        /// 未定价
        /// </summary>
        [Enum("未定价")]
        NotPriced = 1,
        /// <summary>
        /// 赠品
        /// </summary>
        [Enum("赠品")]
        Gifts = 2
    }
}