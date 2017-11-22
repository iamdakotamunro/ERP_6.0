using System;
using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>
    /// 仓库类型枚举
    /// </summary>
    [Serializable]
    public enum WarehouseType
    {
        /// <summary>
        /// 发货仓库
        /// </summary>
        [Enum("发货仓库")]
        MainStock=0,
        /// <summary>
        /// 坏件仓
        /// </summary>
        [Enum("坏件仓")]
        BadGoodsStock=1, 
        ///// <summary>
        ///// 特价仓
        ///// </summary>
        //[EnumArrtibute("特价仓")]
        //SpecialOfferStock=2,
        /// <summary>
        /// 其他仓
        /// </summary>
        [Enum("其他仓")]
        OtherStock = 3,
        /// <summary>
        /// 售后仓库
        /// </summary>
        [Enum("售后仓库")]
        AfterSaleStock = 4,
        
    }
}
