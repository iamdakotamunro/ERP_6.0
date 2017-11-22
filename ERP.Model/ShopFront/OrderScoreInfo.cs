using System;
using System.Runtime.Serialization;

namespace ERP.Model.ShopFront
{
    /// <summary>
    /// 订单金额、积分模型
    /// </summary>
    [Serializable]
    [DataContract]
    public class OrderScoreInfo
    {
        /// <summary>
        /// 订单编号
        /// </summary>
        [DataMember]
        public string OrderNo { get; set; }

        /// <summary>
        /// 订单金额
        /// </summary>
        [DataMember]
        public decimal TotalPrice { get; set; }

        /// <summary>
        /// 订单积分
        /// </summary>
        [DataMember]
        public decimal OrderScore { get; set; }

        /// <summary>
        /// 下单时间
        /// </summary>
        [DataMember]
        public DateTime OrderTime { get; set; }
    }
}
