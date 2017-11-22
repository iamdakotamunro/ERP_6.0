using System;
using ERP.Enum.Attribute;

namespace ERP.Enum.ShopFront
{
    /// <summary>
    /// 采购类型
    /// </summary>
    [Serializable]
    public enum PurchaseType
    {
        /// <summary>
        /// 订单类型
        /// </summary>
        [Enum("订单类型")]
        FromOrder = 1,

        /// <summary>
        /// 要货类型
        /// </summary>
        [Enum("要货类型")]
        FromPurchase = 2
    }
}
