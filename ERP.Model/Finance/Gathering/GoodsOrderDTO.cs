using System;
using System.Runtime.Serialization;

namespace ERP.Model.Finance.Gathering
{
    /// <summary>
    /// 收款单：订单
    /// </summary>
    [Serializable]
    [DataContract]
    public class GoodsOrderDTO
    {
        /// <summary>
        /// 订单ID
        /// </summary>
        [DataMember]
        public Guid OrderId { get; set; }

        /// <summary>
        /// 订单号
        /// </summary>
        [DataMember]
        public string OrderNo { get; set; }

        /// <summary>
        /// 会员ID
        /// </summary>
        [DataMember]
        public Guid MemberId { get; set; }

        /// <summary>
        /// 下单时间
        /// </summary>
        [DataMember]
        public DateTime OrderTime { get; set; }

        /// <summary>
        /// 总价
        /// </summary>
        [DataMember]
        public decimal TotalPrice { get; set; }

        /// <summary>
        /// 销售公司ID
        /// </summary>
        [DataMember]
        public Guid SaleFilialeId { get; set; }

        /// <summary>
        /// 销售平台
        /// </summary>
        [DataMember]
        public Guid SalePlatformId { get; set; }

        /// <summary>
        /// 收款单类型（true：正的收款单、false：负的收款单）
        /// </summary>
        [DataMember]
        public bool Type { get; set; }

        public Guid BankAccountsId { get; set; }
    }
}