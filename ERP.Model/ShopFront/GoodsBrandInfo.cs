using System;
namespace Keede.Ecsoft.Model.ShopFront
{
    /// <summary>
    /// 商品品牌信息
    /// </summary>
    [Serializable]
    public class GoodsBrandInfo
    {
        /// <summary>
        /// 品牌ID
        /// </summary>
        public Guid BrandId { get; set; }

        /// <summary>
        /// 排序序号
        /// </summary>
        public int OrderIndex { get; set; }

        /// <summary>
        /// 品牌名称
        /// </summary>
        public string BrandName { get; set; }

        /// <summary>
        /// 品牌LOGO
        /// </summary>
        public string BrandLogo { get; set; }

        /// <summary>
        /// 品牌描述
        /// </summary>
        public string Description { get; set; }
    }
}
