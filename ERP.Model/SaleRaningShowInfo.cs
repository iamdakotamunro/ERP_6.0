using System;

namespace ERP.Model
{
    /// <summary>
    /// 商品销量排行展示模型
    /// </summary>
    [Serializable]
    public class SaleRaningShowInfo
    {
        /// <summary>
        /// 按销量排行为商品Id
        /// 按平台分组为平台Id
        /// 按品牌分组为品牌Id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 按销量排行为商品名称
        /// 按平台分组为平台名称
        /// 按品牌分组为品牌名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 商品编号
        /// </summary>
        public string GoodsCode { get; set; }

        /// <summary>
        /// 上一期零元销量
        /// </summary>
        public int PreZeroNumber { get; set; }

        /// <summary>
        /// 上一期销售额
        /// </summary>
        public decimal PreGoodsPrice { get; set; }

        /// <summary>
        /// 上一期销量
        /// </summary>
        public int PreSalesNumber { get; set; }

        /// <summary>
        /// 当期零元销量
        /// </summary>
        public int ZeroNumber { get; set; }

        /// <summary>
        /// 当期销售额
        /// </summary>
        public decimal GoodsPrice { get; set; }

        /// <summary>
        /// 当期销量
        /// </summary>
        public int SalesNumber { get; set; }
        /// <summary>
        /// 进货成本总价
        /// </summary>
        public decimal PurchasePrice { get; set; }
        /// <summary>
        /// 毛利率
        /// </summary>
        public string GrossMargin
        {
            get
            {
                if (GoodsPrice == 0 || PurchasePrice == 0) return "-";
                return (((GoodsPrice - PurchasePrice) / GoodsPrice) * 100).ToString("f1") + "%";
            }
        }

        /// <summary>
        /// 平台ID
        /// </summary>
        public Guid SalePlatformId { get; set; }

        /// <summary>
        /// 系列ID
        /// </summary>
        public Guid SeriesId { get; set; }


        /// <summary>
        /// 品牌ID
        /// </summary>
        public Guid BrandId { get; set; }

        /// <summary>
        /// 销售增长率
        /// </summary>
        public string SalesPriceIncrease
        {
            get
            {
                if (PreSalesNumber == 0 || SalesNumber==0) return "-";
                if (GoodsPrice == 0 || PreGoodsPrice == 0) return "-";
                return (((GoodsPrice - PreGoodsPrice) / PreGoodsPrice) * 100).ToString("f1") + "%";
            }
        }
    }
}
