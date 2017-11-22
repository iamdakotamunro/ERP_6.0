using System;
using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>
    /// 销售订单商品结算价的关联单据类型
    /// </summary>
    [Serializable]
    public enum SaleOrderGoodsSettlementRelatedTradeType
    {
        /// <summary>
        /// B2C订单
        /// </summary>
        [Enum("B2C订单")]
        B2COrder = 1,

        /// <summary>
        /// 销售出库给门店
        /// </summary>
        [Enum("销售出库给门店")]
        SaleStockOutForShop = 2,

        /// <summary>
        /// 销售出库给销售公司
        /// </summary>
        [Enum("销售出库给销售公司")]
        SaleStockOutForSaleFiliale = 3,

        /// <summary>
        /// 销售出库给销售公司
        /// </summary>
        [Enum("销售出库给物流配送公司")]
        SaleStockOutForHostingFiliale = 4
    }
}
