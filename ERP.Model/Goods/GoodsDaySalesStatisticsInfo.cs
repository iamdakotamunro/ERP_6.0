using System;

namespace ERP.Model.Goods
{
    ///<summary>
    /// 商品日销售统计 
    /// add by jiang 2012-02-09
    ///</summary>
    public class GoodsDaySalesStatisticsInfo
    {
        /// <summary>
        /// 发货仓库Id
        /// </summary>
        public Guid DeliverWarehouseId { get; set; }

        /// <summary>
        /// 销售公司
        /// </summary>
        public Guid SaleFilialeId { get; set; }

        /// <summary>
        /// 销售平台
        /// </summary>
        public Guid SalePlatformId { get; set; }

        /// <summary>
        /// 商品Id
        /// </summary>
        public Guid GoodsId { get; set; }

        /// <summary>
        /// 子商品Id
        /// </summary>
        public Guid RealGoodsId { get; set; }

        /// <summary>
        /// 销售时间
        /// </summary>
        public DateTime DayTime { get; set; }

        /// <summary>
        /// 日销售量
        /// </summary>
        public double GoodsSales { get; set; }

        /// <summary>
        /// 规格
        /// </summary>
        public string Specification { get; set; }

        /// <summary>
        /// 销售价格
        /// </summary>
        public Decimal SellPrice { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string GoodsName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string GoodsCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Guid ClassId { get; set; }

        public Guid SeriesId { get; set; }

        public Guid BrandId { get; set; }

        /// <summary>
        /// 子商品每天的总进货成本
        /// zal 2017-08-11
        /// 此字段原来对应结算价表中的结算价，后来采用了及时结算价，为计算进货成本此字段的含义更改为每个子商品每天的总进货成本
        /// </summary>
        public Decimal AvgSettlePrice { get; set; }

        public Guid HostingFilialeId { get; set; }
        
    }
}