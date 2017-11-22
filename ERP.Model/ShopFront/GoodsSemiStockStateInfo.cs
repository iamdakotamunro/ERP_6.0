using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Keede.Ecsoft.Model.ShopFront
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class GoodsSemiStockStateInfo
    {
        /// <summary>
        /// 商品ID
        /// </summary>
        public Guid GoodsId { get; set; }

        /// <summary>出入库单据状态
        /// </summary>
        public int State { get; set; }

        /// <summary>
        /// 调拨数量
        /// </summary>
        public int Quantity { get; set; }
    }
}
