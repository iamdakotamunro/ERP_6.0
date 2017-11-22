using System;

namespace ERP.Model.Goods
{
    /// <summary>
    /// 第三方商品销售价格
    /// </summary>
    [Serializable]
    public class ThirdGoodsSalePriceInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid GoodsId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Guid GroupId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Guid SalePlatformId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal Price { get; set; }

        public bool IsDefault { get; set; }
    }
}
