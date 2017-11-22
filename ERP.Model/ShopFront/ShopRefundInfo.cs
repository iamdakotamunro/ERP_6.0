using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ERP.Model.ShopFront
{
    /// <summary>
    /// 联盟店是否可购买商品模型
    /// </summary>
    [Serializable]
    public class ShopRefundInfo
    {
        /// <summary>
        /// 主商品Id
        /// </summary>
        public Guid GoodsId { get; set; }

        /// <summary>
        /// 同品牌是否允许采购
        /// </summary>
        public bool AllowPurchase { get; set; }

        /// <summary>
        /// 是否可以采购
        /// </summary>
        public bool IsForever { get; set; }
    }
}
