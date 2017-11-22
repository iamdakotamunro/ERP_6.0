using System;
using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>
    /// 信息状态枚举
    /// </summary>
    [Serializable]
    public enum State
    {
        /// <summary>
        /// 搁置
        /// </summary>
        [Enum("搁置")]
        Disable = 0,
        /// <summary>
        /// 启用
        /// </summary>
        [Enum("启用")]
        Enable = 1
    }
}
