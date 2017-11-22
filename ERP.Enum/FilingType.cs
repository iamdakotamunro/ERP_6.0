using System;
using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>
    /// 归类类型枚举
    /// </summary>
    [Serializable]
    public enum FilingType
    {
        /// <summary>
        /// 所有采购分类 不包括入库申请的
        /// </summary>
        [Enum("所有")]
        All = -1,
        /// <summary>
        /// 分类进货申报
        /// </summary>
        [Enum("进货申报")]
        Stock = 0,
        /// <summary>
        /// 分类报备
        /// </summary>
        [Enum("分类报备")]
        Class = 1,
        /// <summary>
        /// 核对入库
        /// </summary>
        [Enum("核对入库")]
        Storage = 2,
    }
}
