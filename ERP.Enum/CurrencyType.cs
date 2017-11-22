using System;
using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>
    /// 币种类型枚举
    /// </summary>
    [Serializable]
    public enum CurrencyType
    {
        /// <summary>
        /// 货币
        /// </summary>
        [Enum("货币")]
        Money = 1,

        /// <summary>
        /// 积分
        /// </summary>
        [Enum("积分")]
        Score = 2,
    }
}
