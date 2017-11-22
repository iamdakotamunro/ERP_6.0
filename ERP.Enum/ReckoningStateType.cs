using System;
using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>
    /// 财务记帐单状态枚举
    /// </summary>
    [Serializable]
    public enum ReckoningStateType
    {
        /// <summary>
        /// 正常
        /// </summary>
        [Enum("正常")]
        Currently = 1,
        /// <summary>
        /// 红冲
        /// </summary>
        [Enum("红冲")]
        Cancellation = 2
    }
}
