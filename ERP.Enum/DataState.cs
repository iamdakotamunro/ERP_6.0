using System;
using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>
    /// 对账明细记录状态
    /// </summary>
    [Serializable]
    public enum DataState
    {
        /// <summary>
        /// 全部
        /// </summary>
        [Enum("全部")]
        All = -1,

        /// <summary>
        /// 已录入
        /// </summary>
        [Enum("已录入")]
        Entered = 0,

        /// <summary>
        /// 已对比
        /// </summary>
        [Enum("已对比")]
        Contrasted = 1,

        /// <summary>
        /// 已确认
        /// </summary>
        [Enum("已确认")]
        Confirmed = 2,

        /// <summary>
        /// 已处理
        /// </summary>
        [Enum("已处理")]
        Handed = 3
    }
}
