using System;
using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>
    /// 售后退回检查状态
    /// </summary>
    [Serializable]
    public enum CheckState
    {
        /// <summary>
        /// 所有
        /// </summary>
        [Enum("所有")]
        ALL = -1,

        /// <summary>
        /// 待检查
        /// </summary>
        [Enum("待检查")]
        Checking = 0,

        /// <summary>
        /// 检查通过
        /// </summary>
        [Enum("检查通过")]
        Pass = 1,

        /// <summary>
        /// 退回
        /// </summary>
        [Enum("退回")]
        Refuse = 2,

        /// <summary>
        /// 作废
        /// </summary>
        [Enum("作废")]
        Cancellation = 3
    }
}
