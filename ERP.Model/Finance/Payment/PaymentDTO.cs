using System;
using System.Runtime.Serialization;

namespace ERP.Model.Finance.Payment
{
    /// <summary>
    /// 财务：付款单
    /// </summary>
    [Serializable]
    [DataContract]
    public class PaymentDTO
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
        public Guid CompanyName { get; set; }

        /// <summary>
        /// 单据日期（开票日期）
        /// </summary>
        [DataMember]
        public DateTime? BillingDate { get; set; }

        //结算方式（银行卡，现金、支付宝、微信、储值卡）

        //供应商编码

        //部门编码

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