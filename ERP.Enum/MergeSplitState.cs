using System;
using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>
    /// 商品组合拆分状态
    /// </summary>
    [Serializable]
    public enum MergeSplitState
    {
        /// <summary>
        /// 未审核
        /// </summary>
        [Enum("未审核")]
        WaitAuditing = 0,

        /// <summary>
        /// 已通过
        /// </summary>
        [Enum("已通过")]
        Pass = 1,

        /// <summary>
        /// 未通过
        /// </summary>
        [Enum("未通过")]
        Refuse = 2
    }
}
