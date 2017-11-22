using System;
namespace Keede.Ecsoft.Model
{
    /// <summary>
    /// 产品销售排行
    /// </summary>
    [Serializable]
    public class SalesGoodsRankingInfo
    {
        /// <summary>
        /// 产品Id
        /// </summary>
        public Guid GoodsId { get; set; }

        /// <summary>
        /// 产品名称
        /// </summary>
        public string GoodsName { get; set; }

        /// <summary>
        /// 产品编号
        /// </summary>
        public string GoodsCode { get; set; }

        /// <summary>
        /// 产品单位
        /// </summary>
        public string Units { get; set; }

        /// <summary>
        /// 指定时间段内的销售量
        /// </summary>
        public double SalesNumber { get; set; }

        /// <summary>
        /// 指定时间段内的0元销售量
        /// </summary>
        public int ZeroNumber { get; set; }

        /// <summary>
        /// 产品价格
        /// </summary>
        public decimal GoodsPrice { get; set; }

        /// <summary>
        /// 销售价格
        /// </summary>
        public decimal SalePrice { get; set; }

        /// <summary>
        /// 日期
        /// </summary>
        public DateTime DayTime { get; set; }

        /// <summary>
        /// 销售平台
        /// </summary>
        public Guid SalePlatformId { get; set; }

        /// <summary>
        /// 商品系列
        /// </summary>
        public Guid SeriesId { get; set; }

        /// <summary>
        /// 商品品牌
        /// </summary>
        public Guid BrandId { get; set; }

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public SalesGoodsRankingInfo() { }

        /// <summary>
        /// 初始化构造函数
        /// </summary>
        /// <param name="goodsId">产品Id</param>
        /// <param name="goodsName">产品名称</param>
        /// <param name="goodsCode">产品编号</param>
        /// <param name="units">产品单位</param>
        /// <param name="salesNumber">指定时间段内的销售量</param>
        public SalesGoodsRankingInfo(Guid goodsId, string goodsName, string goodsCode, string units, double salesNumber)
        {
            GoodsId = goodsId;
            GoodsName = goodsName;
            GoodsCode = goodsCode;
            Units = units;
            SalesNumber = salesNumber;
        }
    }
}
