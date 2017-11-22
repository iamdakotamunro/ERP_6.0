using System;
using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>
    /// 完成订单任务执行状态
    /// </summary>
    [Serializable]
    public enum CompleteOrderTaskState
    {
        /// <summary>
        /// 待处理
        /// </summary>
        [Enum("待处理")]
        Wait = 0,

        /// <summary>
        /// 处理中
        /// </summary>
        [Enum("处理中")]
        Processing = 1,

        /// <summary>
        /// 已处理
        /// </summary>
        [Enum("已处理")]
        Processed = 2,
    }
}
