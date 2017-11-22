using System;
using System.Runtime.Serialization;

namespace ERP.Model.SubsidyPayment
{
    /// <summary>
    /// 补贴审核、补贴打款
    /// </summary>
    [Serializable]
    [DataContract]
    public class SubsidyPaymentInfo
    {
        #region Model

        /// <summary>
        /// 主键：补贴审核ID
        /// </summary>
        [DataMember]
        public Guid ID { get; set; }

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
        /// 订单金额
        /// </summary>
        [DataMember]
        public decimal OrderAmount { get; set; }

        /// <summary>
        /// 补贴金额
        /// </summary>
        [DataMember]
        public decimal SubsidyAmount { get; set; }

        /// <summary>
        /// 补贴类型（1：补偿、2：赠送）
        /// </summary>
        [DataMember]
        public int SubsidyType { get; set; }

        /// <summary>
        /// 补贴审核的问题类型
        /// </summary>
        [DataMember]
        public Guid QuestionType { get; set; }

        /// <summary>
        /// 补贴审核的问题名称
        /// </summary>
        [DataMember]
        public string QuestionName { get; set; }

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
        /// 拒绝理由
        /// </summary>
        [DataMember]
        public string RejectReason { get; set; }

        /// <summary>
        /// 补贴审核（1：待审核、2：待财务审核、3：待打款、4：已作废、5：已打款）
        /// </summary>
        [DataMember]
        public int Status { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [DataMember]
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        [DataMember]
        public string CreateUser { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        [DataMember]
        public bool IsDelete { get; set; }

        /// <summary>
        /// 最后修改时间
        /// </summary>
        [DataMember]
        public DateTime ModifyTime { get; set; }

        /// <summary>
        /// 最后修改人
        /// </summary>
        [DataMember]
        public string ModifyUser { get; set; }

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
        /// 备注信息
        /// </summary>
        [DataMember]
        public string Remark { get; set; }

        #endregion Model
    }
}
