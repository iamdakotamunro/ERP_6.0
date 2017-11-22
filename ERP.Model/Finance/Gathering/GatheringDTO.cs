using System;
using System.Runtime.Serialization;

namespace ERP.Model.Finance.Gathering
{
    /// <summary>
    /// 收款单：收款单
    /// </summary>
    [Serializable]
    [DataContract]
    public class GatheringDTO
    {
        //TODO:待定

        /// <summary>
        /// lmshop_CompanyFundReceipt表的主键。
        /// </summary>
        [DataMember]
        public Guid ReceiptID { get; set; }

        /// <summary>
        /// 单号（收款单号）
        /// </summary>
        [DataMember]
        public string ReceiptNo { get; set; }

        /// <summary>
        /// 公司ID（可不要）
        /// </summary>
        [DataMember]
        public Guid CompanyID { get; set; }

        /// <summary>
        /// 公司名称（需要关联）
        /// </summary>
        [DataMember]
        public string CompanyName { get; set; }

        /// <summary>
        /// 单据日期（开票日期）
        /// </summary>
        [DataMember]
        public DateTime? BillingDate { get; set; }

        //收款类型（正常、营业外收入）
        //收款单类型有：往来单位类型、劳务费类型

        //结算方式（银行卡，现金、支付宝、微信、储值卡）

        /// <summary>
        /// 收款银行账户（可不要）
        /// </summary>
        [DataMember]
        public Guid ReceiveBankAccountId { get; set; }

        /// <summary>
        /// 银行名称（收款银行账户）（需要关联）
        /// </summary>
        [DataMember]
        public string ReceiveBankAccountName { get; set; }

        //客户编码

        //部门编码（空）

        //币种名称（默认人民币）

        //汇率（默认1）

        //原币金额

        //本币金额

        /// <summary>
        /// 单据申请者ID（可不要）
        /// </summary>
        [DataMember]
        public Guid ApplicantID { get; set; }

        /// <summary>
        /// 制单人（单据申请者）（需要关联）
        /// </summary>
        [DataMember]
        public string ApplicantName { get; set; }

        //备注

        //明细list

        /// <summary>
        /// 收款单类型：true正的收款单、false负的收款单。
        /// </summary>
        [DataMember]
        public bool Type { get; set; }
    }
}