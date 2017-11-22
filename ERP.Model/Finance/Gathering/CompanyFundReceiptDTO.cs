using System;
using System.Runtime.Serialization;

namespace ERP.Model.Finance.Gathering
{
    /// <summary>
    /// 收款单：往来单位收付款
    /// 收款单类型Type（true：正的收款单、false：负的收款单）
    /// </summary>
    [Serializable]
    [DataContract]
    public class CompanyFundReceiptDTO
    {
        [DataMember]
        public Guid ReceiptID { get; set; }

        /// <summary>
        /// 单号（收款单号）
        /// </summary>
        [DataMember]
        public string ReceiptNo { get; set; }

        /// <summary>
        /// 公司ID
        /// </summary>
        [DataMember]
        public Guid FilialeId { get; set; }

        /// <summary>
        /// 公司名称
        /// </summary>
        [DataMember]
        public string CompanyName { get; set; }

        /// <summary>
        /// 单据日期（开票日期）
        /// </summary>
        [DataMember]
        public DateTime? BillingDate { get; set; }

        /// <summary>
        /// 收款银行账户
        /// </summary>
        [DataMember]
        public Guid ReceiveBankAccountId { get; set; }

        /// <summary>
        /// 银行名称（收款银行账户）
        /// </summary>
        [DataMember]
        public string ReceiveBankAccountName { get; set; }

        /// <summary>
        /// 往来单位实际应付结算余额
        /// </summary>
        public decimal RealityBalance { get; set; }

        /// <summary>
        /// 单据申请者ID
        /// </summary>
        [DataMember]
        public Guid ApplicantID { get; set; }

        /// <summary>
        /// 制单人（单据申请者）
        /// </summary>
        [DataMember]
        public string ApplicantName { get; set; }

        /// <summary>
        /// 收款单类型（true：正的收款单、false：负的收款单）
        /// </summary>
        [DataMember]
        public bool Type { get; set; }
    }
}