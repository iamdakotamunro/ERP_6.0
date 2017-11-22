using System;
using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary> 财务记帐单类型枚举
    /// </summary>
    [Serializable]
    public enum ReckoningType
    {
        /// <summary>应收
        /// </summary>
        [Enum("应收、应付已付")]
        Income = 0,

        /// <summary>应付
        /// </summary>
        [Enum("应付、应收已收")]
        Defray = 1
    }
}
