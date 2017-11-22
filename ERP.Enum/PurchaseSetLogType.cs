using System;
using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>
    /// 商品采购设置变更值类型
    /// </summary>
    [Serializable]
    public enum PurchaseSetLogType
    {
        /// <summary>
        /// 采购价
        /// </summary>
        [Enum("采购价")]
        PurchasePrice = 1,

        /// <summary>
        /// 报备天数
        /// </summary>
        [Enum("报备天数")]
        StockingDays = 2

    }
}
