using System;
using System.Collections.Generic;

namespace ERP.Model
{
    /// <summary>商品销量汇总  2015-06-16  陈重文 
    /// </summary>
    public class SalesVolumeInfo
    {
        /// <summary>主商品ID
        /// </summary>
        public Guid GoodsId { get; set; }

        /// <summary>近30天内销量
        /// </summary>
        public Int32 ThirtyDaySales { get; set; }

        /// <summary>近60天内销量（不包含ThirtyDaySales）
        /// </summary>
        public Int32 SixtyDaySales { get; set; }

        /// <summary>近90天内销量（不包含ThirtyDaySales，SixtyDaySales）
        /// </summary>
        public Int32 NinetyDaySales { get; set; }

        /// <summary>加权月平均销量
        /// </summary>
        public Int32 WeightedAverageSaleQuantity
        {
            get
            {
                var sumSaleNumber = (ThirtyDaySales * 3 + SixtyDaySales * 2 + NinetyDaySales);
                var sumWeightedNumber = ((ThirtyDaySales == 0 ? 0 : 3) + (SixtyDaySales == 0 ? 0 : 2) + (NinetyDaySales == 0 ? 0 : 1));
                //return sumWeightedNumber == 0 ? 0 : (sumSaleNumber / sumWeightedNumber) / 3;
                return sumWeightedNumber == 0 ? 0 : (sumSaleNumber / sumWeightedNumber);
            }
        }
    }
}
