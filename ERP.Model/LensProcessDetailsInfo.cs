using System;

namespace Keede.Ecsoft.Model
{
    /// <summary>
    /// 框架加工单——详细
    /// </summary>
    [Serializable]
    public class LensProcessDetailsInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid ID { get; set; }

        /// <summary>
        /// 商品ID
        /// </summary>
        public Guid GoodsId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Guid RealGoodsId { get; set; }

        /// <summary>
        /// 商品规格
        /// </summary>
        public string Specification { get; set; }

        /// <summary>
        /// 商品采购规格
        /// </summary>
        public string PurchaseSpecification { get; set; }

        /// <summary>
        /// 是否损坏
        /// </summary>
        public bool IsBad { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string GoodsName { get; set; }
    }
}
