using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ERP.Model
{
    /// <summary>发货率统计模型  2015-04-20  陈重文 
    /// </summary>
    public class DeliveryRatioInfo
    {
        /// <summary>发货日期
        /// </summary>
        public DateTime DeliveryDate { get; set; }

        /// <summary>总订单量
        /// </summary>
        public Int32 GoodsOrderTotals { get; set; }

        /// <summary>可发订单量
        /// </summary>
        public Int32 CanGoodsOrderTotals { get; set; }

        /// <summary>发货量
        /// </summary>
        public Int32 RealityGoodsOrderTotals { get; set; }

        /// <summary>发货率(发货量/可发订单量)
        /// </summary>
        public String DeliveryRatio { get; set; }
    }
}
