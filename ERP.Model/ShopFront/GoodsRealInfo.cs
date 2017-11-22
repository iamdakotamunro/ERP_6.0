using System;
namespace Keede.Ecsoft.Model.ShopFront
{
    /// <summary>
    /// 子商品ID信息
    /// </summary>
    [Serializable]
    public class GoodsRealInfo
    {
        /// <summary>
        /// 主商品ID
        /// </summary>
        public Guid GoodsId { get; set; }
        /// <summary>
        /// 子商品ID
        /// </summary>
        public Guid RealGoodsId { get; set; }
        /// <summary>
        /// 是否缺货
        /// </summary>
        public bool IsScarcity { get; set; }
        /// <summary>
        /// 条形码
        /// </summary>
        public string Barcode { get; set; }
    }
}
