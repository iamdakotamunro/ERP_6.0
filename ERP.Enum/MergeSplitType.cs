using System;
using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>
    /// 商品组合拆分类型
    /// </summary>
    [Serializable]
    public enum MergeSplitType
    {
        /// <summary>
        /// 异常
        /// </summary>
        [Enum("异常")]
        NO = -1,

        /// <summary>
        /// 商品组合
        /// </summary>
        [Enum("商品组合")]
        MergeGoods = 0,

        /// <summary>
        /// 商品拆分
        /// </summary>
        [Enum("商品拆分")]
        SplitGoods = 1
    }
}
