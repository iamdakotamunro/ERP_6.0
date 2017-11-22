using System;
using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>
    /// Func : 比较类型枚举
    /// Code : dyy
    /// Date : 2009 Nov 26th
    /// </summary>
    [Serializable]
    public enum CompareType
    {
        /// <summary>
        /// 小于
        /// </summary>
        [Enum("<")]
        Less = 0,
        
        /// <summary>
        /// 小于等于
        /// </summary>
        [Enum("<=")]
        LessEqual = 1,

        /// <summary>
        /// 等于
        /// </summary>
        [Enum("=")]
        Equal = 2,

        /// <summary>
        /// 大于
        /// </summary>
        [Enum(">")]
        Greater = 3,

        /// <summary>
        /// 大于等于
        /// </summary>
        [Enum(">=")]
        GreaterEqual = 4,

        /// <summary>
        /// 不等于
        /// </summary>
        [Enum("!=")]
        NotEqual = 5
    }
}
