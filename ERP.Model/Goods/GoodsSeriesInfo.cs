using System;
using System.Collections.Generic;

namespace ERP.Model.Goods
{
    /// <summary>
    /// 商品系列
    /// </summary>
    public class GoodsSeriesInfo
    {
        /// <summary>
        /// 商品系列编号
        /// </summary>
        public Guid SeriesID { get; set; }

        /// <summary>
        /// 商品系列名称
        /// </summary>
        public string SeriesName { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public List<GoodsInfo> GoodsList { get; set; }
    }
}
