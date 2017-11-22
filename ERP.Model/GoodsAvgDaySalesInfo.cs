using System;
using System.Runtime.Serialization;

namespace ERP.Model
{
    /// <summary>
    /// 商品日均销量数据
    /// </summary>
    [Serializable]
    [DataContract]
    public class GoodsAvgDaySalesInfo
    {
        /// <summary>
        /// 发货仓库
        /// </summary>
        [DataMember]
        public Guid DeliverWarehouseId { get; set; }

        /// <summary>
        /// 销售公司
        /// </summary>
        [DataMember]
        public Guid SaleFilialeId { get; set; }

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
        /// 日均销量
        /// </summary>
        [DataMember]
        public decimal AvgQuantity { get; set; }
    }
}
