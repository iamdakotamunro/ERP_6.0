using System;
using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>
    /// 商品扩展描述展示位置枚举
    /// </summary>
    [Serializable]
    public enum GoodsServicePosition
    {
        /// <summary>
        /// 未设定
        /// </summary>
        [Enum("未设定")]
        NoSet = 0,
        /// <summary>
        /// 商品描述之前
        /// </summary>
        [Enum("商品描述之前")]
        BeforDescription = 1,
        /// <summary>
        /// 商品描述之后
        /// </summary>
        [Enum("商品描述之后")]
        AfterDescription = 2,
        /// <summary>
        /// 商品属性下方
        /// </summary>
        [Enum("商品属性下方")]
        AfterAttribute = 3
    }
}