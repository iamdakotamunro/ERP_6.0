using System;
using System.Runtime.Serialization;
namespace Keede.Ecsoft.Model
{
    /// <summary>
    /// 付款通知类
    /// </summary>
    [Serializable]
    [DataContract]
    public class PaymentNoticeInfo
    {
        /// <summary>
        ///支付通知ID
        /// </summary>
        [DataMember]
        public Guid PayId { get; set; }
        /// <summary>
        ///订单ID
        /// </summary>
        [DataMember]
        public Guid OrderId { get; set; }
        /// <summary>
        ///订单号
        /// </summary>
        [DataMember]
        public string OrderNo { get; set; }
        /// <summary>
        ///付款银行
        /// </summary>
        [DataMember]
        public string PayBank { get; set; }
        /// <summary>
        ///付款金额
        /// </summary>
        [DataMember]
        public decimal PayPrince { get; set; }
        /// <summary>
        ///付款时间
        /// </summary>
        [DataMember]
        public DateTime PayTime { get; set; }
        /// <summary>
        ///付款姓名
        /// </summary>
        [DataMember]
        public string PayName { get; set; }
        /// <summary>
        ///备注
        /// </summary>
        [DataMember]
        public string PayDes { get; set; }
        /// <summary>
        ///状态
        /// </summary>
        [DataMember]
        public int PayState { get; set; }

        /// <summary>
        /// 如果是门店，则FromSourceId字段，保存的是门店公司id如果是网站，则FromSourceId字段，保存的是 网站的标识id
        /// </summary>
        [DataMember]
        public Guid SaleFilialeId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public Guid SalePlatformId { get; set; } 

    }
}
