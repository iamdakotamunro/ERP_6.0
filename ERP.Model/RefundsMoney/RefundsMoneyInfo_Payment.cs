using System;
using System.Runtime.Serialization;

namespace ERP.Model.RefundsMoney
{
    /// <summary>
    /// 退款打款——打款
    /// </summary>
    [Serializable]
    [DataContract]
    public class RefundsMoneyInfo_Payment
    {
        #region Model

        /// <summary>
        /// 主键：退款打款ID
        /// </summary>
        [DataMember]
        public Guid ID { get; set; }

        /// <summary>
        /// 是否打款完成
        /// </summary>
        [DataMember]
        public bool IsPayment { get; set; }

        /// <summary>
        /// 拒绝理由
        /// </summary>
        [DataMember]
        public string RejectReason { get; set; }

        /// <summary>
        /// 手续费
        /// </summary>
        [DataMember]
        public decimal? Fees { get; set; }

        /// <summary>
        /// 交易号
        /// </summary>
        [DataMember]
        public string TransactionNumber { get; set; }

        /// <summary>
        /// 资金账户
        /// </summary>
        [DataMember]
        public Guid? AccountID { get; set; }

        /// <summary>
        /// 最后修改人
        /// </summary>
        [DataMember]
        public string ModifyUser { get; set; }

        #endregion Model
    }
}
