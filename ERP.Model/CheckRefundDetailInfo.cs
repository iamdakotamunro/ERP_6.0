using System;
using System.Runtime.Serialization;

namespace Keede.Ecsoft.Model
{
    /// <summary>
    /// 退换货商品明细
    /// </summary>
    [Serializable]
    [DataContract]
    public class CheckRefundDetailInfo
    {
        /// <summary>
        /// 主键编号
        /// </summary>
        [DataMember]
        public Guid Id { get; set; }

        /// <summary>
        /// 退换货编号
        /// </summary>
        [DataMember]
        public Guid RefundId { get; set; }

        /// <summary>
        /// 商品编号
        /// </summary>
        [DataMember]
        public Guid GoodsId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public Guid RealGoodsId { get; set; }

        /// <summary>
        /// 商品编码
        /// </summary>
        [DataMember]
        public string GoodsCode { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        [DataMember]
        public string GoodsName { get; set; }

        /// <summary>
        /// 商品规格
        /// </summary>
        [DataMember]
        public string Specification { get; set; }

        /// <summary>
        ///商品数量
        /// </summary>
        [DataMember]
        public int Quantity { get; set; }

        /// <summary>
        /// 销售价格
        /// </summary>
        [DataMember]
        public decimal SellPrice { get; set; }

        /// <summary>
        /// 退回数量
        /// </summary>
        [DataMember]
        public int ReturnCount { get; set; }

        /// <summary>
        /// 退回原因
        /// </summary>
        [DataMember]
        public string ReturnReason { get; set; }

        /// <summary>
        /// 退回类型
        /// </summary>
        [DataMember]
        public int ReturnType { get; set; }

        /// <summary>
        /// 损坏数量
        /// </summary>
        [DataMember]
        public int DamageCount { get; set; }

        /// <summary>
        /// 货架类型
        /// </summary>
        [DataMember]
        public byte ShelfType { get; set; }
    }
}
