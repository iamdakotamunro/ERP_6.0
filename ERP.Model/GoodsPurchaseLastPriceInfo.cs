using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ERP.Model
{
    /************************************************************************************ 
     * 创建人：  文雯
     * 创建时间：2016/8/9 14:50:26 
     * 描述    :记录最后一次进货价
     * =====================================================================
     * 修改时间：2016/8/9 14:50:26 
     * 修改人  ：  
     * 描述    ：
     */
    public class GoodsPurchaseLastPriceInfo
    {
        /// <summary>
        /// 记录ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 供应商
        /// </summary>
        public Guid ThirdCompanyId { get; set; }

        /// <summary>
        /// 仓库ID
        /// </summary>
        public Guid WarehouseId { get; set; }

        /// <summary>
        /// 商品ID
        /// </summary>
        public Guid GoodsId { get; set; }

        /// <summary>
        /// 子商品ID
        /// </summary>
        public Guid RealGoodsId { get; set; }

        /// <summary>
        /// 价格
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// 最后一次进货日期
        /// </summary>
        public DateTime LastPriceDate { get; set; }
    }
}
