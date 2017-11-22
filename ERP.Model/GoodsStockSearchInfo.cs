using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ERP.Model
{
    /************************************************************************************ 
     * 创建人：  文雯
     * 创建时间：2016/8/26 13:05:18 
     * 描述    :
     * =====================================================================
     * 修改时间：2016/8/26 13:05:18 
     * 修改人  ：  
     * 描述    ：
     */
    public class GoodsStockSearchInfo
    {
        /// <summary>
        /// 商品ID
        /// </summary>
        public Guid GoodsId { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string GoodsName { get; set; }

        /// <summary>
        /// 商品编号
        /// </summary>
        public string GoodsCode { get; set; }

        /// <summary>
        /// 库存量
        /// </summary>
        public int CurrentQuantity { get; set; }

        /// <summary>
        /// 最近进货日期
        /// </summary>
        public DateTime RecentCDate { get; set; }

        /// <summary>
        /// 最近进货价
        /// </summary>
        public decimal RecentInPrice { get; set; }

        /// <summary>当前库存金额
        /// </summary>
        public Decimal CurrentSumPrice { get; set; }

        /// <summary>
        /// 是否缺货
        /// </summary>
        public bool IsScarcity { get; set; }

        /// <summary>
        /// 是否上架
        /// </summary>
        public bool IsOnShelf { get; set; }
    }
}
