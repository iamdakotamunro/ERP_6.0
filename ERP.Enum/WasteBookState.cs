using System;
using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>▄︻┻┳═一 资金流单据状态  ADD  陈重文  2014-12-25
    /// </summary>
    [Serializable]
    public enum WasteBookState
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
