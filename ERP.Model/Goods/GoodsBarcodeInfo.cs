using System;
using System.Runtime.Serialization;

namespace ERP.Model.Goods
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    [DataContract]
    public class GoodsBarcodeInfo
    {
        /// <summary>
        /// 商品ID
        /// </summary>
        [DataMember]
        public Guid GoodsId { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        [DataMember]
        public string GoodsName { get; set; }

        ///// <summary>
        ///// 品牌名称
        ///// </summary>
        //public string BrandName { get; set; }

        /// <summary>
        /// 商品编码
        /// </summary>
        [DataMember]
        public string GoodsCode { get; set; }

        ///// <summary>
        ///// 商品条码
        ///// </summary>
        //public string Barcode { get; set; }

        /// <summary>
        /// 销售价
        /// </summary>
        [DataMember]
        public string SellPrice { get; set; }

        ///// <summary>
        ///// 市场价
        ///// </summary>
        //public string MarketPrice { get; set; }

        ///// <summary>
        ///// 产地
        ///// </summary>
        //public string Origin { get; set; }
    }
}
