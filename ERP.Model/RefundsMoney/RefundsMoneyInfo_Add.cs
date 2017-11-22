using System;
using System.Runtime.Serialization;

namespace ERP.Model.RefundsMoney
{
    /// <summary>
    /// 退款打款——添加
    /// </summary>
    [Serializable]
    [DataContract]
    public class RefundsMoneyInfo_Add
    {
        #region Model

        /// <summary>
        /// 主键：退款打款ID
        /// </summary>
        [DataMember]
        public Guid ID { get; set; }
        
        /// <summary>
        /// 售后单号（退换货号）
        /// </summary>
        [DataMember]
        public string AfterSalesNumber { get; set; }

        /// <summary>
        /// 订单号
        /// </summary>
        [DataMember]
        public string OrderNumber { get; set; }

        /// <summary>
        /// 第三方订单号
        /// </summary>
        [DataMember]
        public string ThirdPartyOrderNumber { get; set; }

        /// <summary>
        /// 第三方账户名
        /// </summary>
        [DataMember]
        public string ThirdPartyAccountName { get; set; }

        /// <summary>
        /// 销售平台
        /// </summary>
        [DataMember]
        public Guid SalePlatformId { get; set; }

        /// <summary>
        /// 销售公司
        /// </summary>
        [DataMember]
        public Guid SaleFilialeId { get; set; }

        /// <summary>
        /// 退款金额
        /// </summary>
        [DataMember]
        public decimal RefundsAmount { get; set; }
        
        /// <summary>
        /// 创建人
        /// </summary>
        [DataMember]
        public string CreateUser { get; set; }
                
        #endregion Model
    }
}
