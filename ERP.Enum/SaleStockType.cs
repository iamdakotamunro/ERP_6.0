using System;
using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>
    /// 商品展示类型枚举
    /// </summary>
    [Serializable]
    public enum SaleStockType
    {
        /// <summary>
        /// 非卖库存
        /// </summary>
        [Enum("非卖库存")]
        NotSellStock  = 0,

        /// <summary>
        /// 卖完缺货
        /// </summary>
        [Enum("卖完缺货")]
        ShortStock = 1,

        /// <summary>
        /// 卖完断货
        /// </summary>
        [Enum("卖完断货")]
        OutOfStock = 2,

        ///// <summary>
        ///// 卖完缺货申请
        ///// </summary>
        //[EnumArrtibute("卖完缺货申请")]
        //ShortStockApply = 3,

        ///// <summary>
        ///// 卖完断货申请
        ///// </summary>
        //[EnumArrtibute("卖完断货申请")]
        //OutOfStockApply = 4,

        ///// <summary>
        ///// 申请不通过
        ///// </summary>
        //[EnumArrtibute("申请不通过")]
        //ApplyNotPass = 5

    }
}
