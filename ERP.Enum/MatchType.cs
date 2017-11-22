using System;
using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public enum MatchType
    {
        /// <summary>
        /// 文字匹配
        /// </summary>
        [Enum("文字匹配")]
        TextMatch = 0,

        /// <summary>
        /// 数值匹配
        /// </summary>
        [Enum("数值匹配")]
        NumberMatch = 1,

        /// <summary>
        /// 不匹配
        /// </summary>
        [Enum("不匹配")]
        UnMatch = 2,

        /// <summary>
        /// 价格匹配
        /// </summary>
        [Enum("价格匹配")]
        PriceMatch = 3,

        /// <summary>
        /// 尺寸匹配
        /// </summary>
        [Enum("尺寸匹配")]
        SizeMatch = 4,

        /// <summary>
        /// 类型匹配
        /// </summary>
        [Enum("类型匹配")]
        TypeMatch = 5
    }

}
