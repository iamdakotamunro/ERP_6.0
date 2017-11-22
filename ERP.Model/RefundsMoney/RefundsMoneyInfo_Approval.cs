using System;
using System.Runtime.Serialization;

namespace ERP.Model.RefundsMoney
{
    /// <summary>
    /// 退款打款——审核
    /// </summary>
    [Serializable]
    [DataContract]
    public class RefundsMoneyInfo_Approval
    {
        #region Model

        /// <summary>
        /// 主键：退款打款ID
        /// </summary>
        [DataMember]
        public Guid ID { get; set; }

        /// <summary>
        /// 机构名称
        /// </summary>
        [DataMember]
        public string BankName { get; set; }

        /// <summary>
        /// 客户支付宝\银行账户
        /// </summary>
        [DataMember]
        public string BankAccountNo { get; set; }

        /// <summary>
        /// 客户姓名
        /// </summary>
        [DataMember]
        public string UserName { get; set; }

        /// <summary>
        /// 审核是否通过
        /// </summary>
        [DataMember]
        public bool IsApproved { get; set; }

        /// <summary>
        /// 拒绝理由
        /// </summary>
        [DataMember]
        public string RejectReason { get; set; }

        /// <summary>
        /// 最后修改人
        /// </summary>
        [DataMember]
        public string ModifyUser { get; set; }

        #endregion Model
    }
}
