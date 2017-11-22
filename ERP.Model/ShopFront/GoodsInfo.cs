using System;
namespace Keede.Ecsoft.Model.ShopFront
{
    /// <summary>
    /// 商品类
    /// <para>
    /// Create BY rjf 2012.6.25
    /// </para>
    /// </summary>
    [Serializable]
    public class GoodsInfo
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
        /// 商品编码
        /// </summary>
        public string GoodsCode { get; set; }

        /// <summary>
        /// 对应品牌ID
        /// </summary>
        public Guid BrandId { get; set; }

        /// <summary>
        /// 计量单位
        /// </summary>
        public string Units { get; set; }

        /// <summary>
        /// 加盟价
        /// </summary>
        public decimal JoinPrice { get; set; }

        /// <summary>
        /// 销售价
        /// </summary>
        public decimal SellPrice { get; set; }

        /// <summary>
        /// 重量
        /// </summary>
        public float Weight { get; set; }

        /// <summary>
        /// 是否缺货
        /// </summary>
        public bool IsScarcity { get; set; }

        /// <summary>
        /// 商品的类型
        /// </summary>
        public int GoodsType { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public int State { get; set; }

        /// <summary>
        /// 条码号
        /// </summary>
        public string BarCode { get; set; }

        /// <summary>
        /// 长
        /// </summary>
        public float Length { get; set; }

        /// <summary>
        /// 宽
        /// </summary>
        public float Width { get; set; }

        /// <summary>
        /// 高
        /// </summary>
        public float Height { get; set; }

        /// <summary>
        /// 规格
        /// </summary>
        public string Standards { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Guid ScoreId { get; set; }
    }
}
