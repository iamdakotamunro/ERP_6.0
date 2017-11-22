using System;
using System.Runtime.Serialization;

namespace ERP.Model
{
    /// <summary>
    /// 商品组合拆分明细
    /// </summary>
    [Serializable]
    [DataContract]
    public class MergeSplitDetailInfo
    {
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public Guid MergeSplitId { get; set; }

        /// <summary>
        /// 商品ID
        /// </summary>
        [DataMember]
        public Guid GoodsId { get; set; }

        /// <summary>
        /// 子商品ID
        /// </summary>
        [DataMember]
        public Guid RealGoodsId { get; set; }

        /// <summary>
        /// 商品规格
        /// </summary>
        [DataMember]
        public string Specification { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        [DataMember]
        public string GoodsName { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        [DataMember]
        public int Quantity { get; set; }

        /// <summary>
        /// 单价
        /// </summary>
        [DataMember]
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// 0原商品，1新商品
        /// </summary>
        [DataMember]
        public int Type { get; set; }

        /// <summary>
        /// 商品库存
        /// </summary>
        [DataMember]
        public int GoodsStock { get; set; }
    }
}
