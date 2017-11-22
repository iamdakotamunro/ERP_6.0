using System;
using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>
    /// 是否选择枚举
    /// </summary>
    [Serializable]
    public enum YesOrNo
    {
        /// <summary>
        /// 否
        /// </summary>
        [Enum("否")]
        No = 0,
        /// <summary>
        /// 是
        /// </summary>
        [Enum("是")]
        Yes = 1,
        /// <summary>
        /// 全部
        /// </summary>
        [Enum("全部")]
        All = -1
    }
}
