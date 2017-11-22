using System;
using System.Runtime.Serialization;
using ERP.Enum;


namespace Keede.Ecsoft.Model
{
    /// <summary>
    /// 往来单位收付款单据表
    /// </summary>
    [Serializable]
    [DataContract]
    public class CompanyFundReceiptInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public CompanyFundReceiptInfo()
        {
            DiscountMoney = 0;
            RealityBalance = 0;
            ExpectBalance = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="receiptid">单据ID</param>
        /// <param name="receiptno">单据编号</param>
        /// <param name="receipttype">单据类型</param>
        /// <param name="applicantid">单据申请者ID</param>
        /// <param name="applydatetime">单据申请时间</param>
        /// <param name="purchaseorderno">采购单编号</param>
        /// <param name="companyid">往来单位ID</param>
        /// <param name="hasinvoice">是否有发票</param>
        /// <param name="settlestartdate">结算起始日期</param>
        /// <param name="settleenddate">结算结束日期</param>
        /// <param name="expectbalance">往来单位预计结算余额</param>
        /// <param name="realitybalance">往来单位实际应付结算余额</param>
        /// <param name="discountmoney">往来单位结算的折扣金额</param>
        /// <param name="discountcaption">往来单位结算折扣说明</param>
        /// <param name="otherdiscountcaption">往来单位差额说明</param>
        /// <param name="receiptstatus">往来单位实际应付结算余额</param>
        /// <param name="remark">备注记录</param>
        /// <param name="auditorid">审核者ID</param>
        /// <param name="auditfailurereason">审核失败的原因说明</param>
        /// <param name="invoicesdemander">发票索取这</param>
        /// <param name="stockOrderNos">入库单</param>
        public CompanyFundReceiptInfo(
            Guid receiptid,
            string receiptno,
            int receipttype,
            Guid applicantid,
            DateTime applydatetime,
            string purchaseorderno,
            Guid companyid,
            bool hasinvoice,
            DateTime settlestartdate,
            DateTime settleenddate,
            decimal expectbalance,
            decimal realitybalance,
            decimal discountmoney,
            string discountcaption,
            string otherdiscountcaption,
            int receiptstatus,
            string remark,
            Guid auditorid,
            string auditfailurereason,
            Guid invoicesdemander,
            string stockOrderNos
            )
        {
            ReceiptID = receiptid;
            ReceiptNo = receiptno;
            ReceiptType = receipttype;
            ApplicantID = applicantid;
            ApplyDateTime = applydatetime;
            PurchaseOrderNo = purchaseorderno;
            CompanyID = companyid;
            HasInvoice = hasinvoice;
            SettleStartDate = settlestartdate;
            SettleEndDate = settleenddate;
            ExpectBalance = expectbalance;
            RealityBalance = realitybalance;
            DiscountMoney = discountmoney;
            DiscountCaption = discountcaption;
            OtherDiscountCaption = otherdiscountcaption;
            ReceiptStatus = receiptstatus;
            Remark = remark;
            AuditorID = auditorid;
            AuditFailureReason = auditfailurereason;
            InvoicesDemander = invoicesdemander;
            StockOrderNos = stockOrderNos;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="receiptid">单据ID</param>
        /// <param name="receiptno">单据编号</param>
        /// <param name="receipttype">单据类型</param>
        /// <param name="applicantid">单据申请者ID</param>
        /// <param name="applydatetime">单据申请时间</param>
        /// <param name="purchaseorderno">采购单编号</param>
        /// <param name="companyid">往来单位ID</param>
        /// <param name="hasinvoice">是否有发票</param>
        /// <param name="settlestartdate">结算起始日期</param>
        /// <param name="settleenddate">结算结束日期</param>
        /// <param name="expectbalance">往来单位预计结算余额</param>
        /// <param name="realitybalance">往来单位实际应付结算余额</param>
        /// <param name="discountmoney">往来单位结算的折扣金额</param>
        /// <param name="discountcaption">往来单位结算折扣说明</param>
        /// <param name="otherdiscountcaption">往来单位差额说明</param>
        /// <param name="receiptstatus">往来单位实际应付结算余额</param>
        /// <param name="remark">备注记录</param>
        /// <param name="auditorid">审核者ID</param>
        /// <param name="auditfailurereason">审核失败的原因说明</param>
        /// <param name="invoicesdemander">发票索取这</param>
        /// <param name="stockOrderNos">入库单</param>
        /// <param name="executeDateTime">预付款时间</param>
        public CompanyFundReceiptInfo(
            Guid receiptid,
            string receiptno,
            int receipttype,
            Guid applicantid,
            DateTime applydatetime,
            string purchaseorderno,
            Guid companyid,
            bool hasinvoice,
            DateTime settlestartdate,
            DateTime settleenddate,
            decimal expectbalance,
            decimal realitybalance,
            decimal discountmoney,
            string discountcaption,
            string otherdiscountcaption,
            int receiptstatus,
            string remark,
            Guid auditorid,
            string auditfailurereason,
            Guid invoicesdemander,
            string stockOrderNos,
            DateTime executeDateTime
            )
        {
            ReceiptID = receiptid;
            ReceiptNo = receiptno;
            ReceiptType = receipttype;
            ApplicantID = applicantid;
            ApplyDateTime = applydatetime;
            PurchaseOrderNo = purchaseorderno;
            CompanyID = companyid;
            HasInvoice = hasinvoice;
            SettleStartDate = settlestartdate;
            SettleEndDate = settleenddate;
            ExpectBalance = expectbalance;
            RealityBalance = realitybalance;
            DiscountMoney = discountmoney;
            DiscountCaption = discountcaption;
            OtherDiscountCaption = otherdiscountcaption;
            ReceiptStatus = receiptstatus;
            Remark = remark;
            AuditorID = auditorid;
            AuditFailureReason = auditfailurereason;
            InvoicesDemander = invoicesdemander;
            StockOrderNos = stockOrderNos;
            ExecuteDateTime = executeDateTime;
        }

        #region --属性

        /// <summary>
        /// 单据ID
        /// </summary>
        [DataMember]
        public Guid ReceiptID { get; set; }

        /// <summary>
        /// 单据编号
        /// </summary>
        [DataMember]
        public string ReceiptNo { get; set; }

        /// <summary>
        /// 单据类型
        /// </summary>
        [DataMember]
        public int ReceiptType { get; set; }

        /// <summary>
        /// 单据申请者ID
        /// </summary>
        [DataMember]
        public Guid ApplicantID { get; set; }

        /// <summary>
        /// 单据申请时间
        /// </summary>
        [DataMember]
        public DateTime ApplyDateTime { get; set; }

        /// <summary>
        /// 预付款时间
        /// </summary>
        [DataMember]
        public DateTime ExecuteDateTime { get; set; }

        /// <summary>
        /// 采购单编号
        /// </summary>
        [DataMember]
        public string PurchaseOrderNo { get; set; }

        /// <summary>
        /// 往来单位ID
        /// </summary>
        [DataMember]
        public Guid CompanyID { get; set; }

        /// <summary>往来单位名称
        /// </summary>
        [DataMember]
        public String CompanyName { get; set; }

        /// <summary>
        /// 是否有发票
        /// </summary>
        [DataMember]
        public bool HasInvoice { get; set; }

        /// <summary>
        /// 发票类型
        /// </summary>
        public int InvoiceType { get; set; }

        /// <summary>
        /// 结算起始日期
        /// </summary>
        [DataMember]
        public DateTime SettleStartDate { get; set; }

        /// <summary>
        /// 结算结束日期
        /// </summary>
        [DataMember]
        public DateTime SettleEndDate { get; set; }

        /// <summary>
        /// 往来单位预计结算余额
        /// </summary>
        [DataMember]
        public decimal ExpectBalance { get; set; }

        /// <summary>
        /// 往来单位实际应付结算余额
        /// </summary>
        [DataMember]
        public decimal RealityBalance { get; set; }

        /// <summary>
        /// 付款金额(新加展示用)
        /// </summary>
        [DataMember]
        public decimal PayRealityBalance { get; set; }

        /// <summary>
        /// 往来单位结算的折扣金额
        /// </summary>
        [DataMember]
        public decimal DiscountMoney { get; set; }

        /// <summary>
        /// 往来单位结算折扣说明
        /// </summary>
        [DataMember]
        public string DiscountCaption { get; set; }

        /// <summary>
        /// 往来单位差额说明
        /// </summary>
        [DataMember]
        public string OtherDiscountCaption { get; set; }

        /// <summary>
        /// 单据状态
        /// </summary>
        [DataMember]
        public int ReceiptStatus { get; set; }

        /// <summary>
        /// 备注记录
        /// </summary>
        [DataMember]
        public string Remark { get; set; }

        /// <summary>
        /// 审核者ID
        /// </summary>
        [DataMember]
        public Guid AuditorID { get; set; }

        /// <summary>
        /// 审核失败的原因说明
        /// </summary>
        [DataMember]
        public string AuditFailureReason { get; set; }

        /// <summary>
        /// 发票索取这
        /// </summary>
        [DataMember]
        public Guid InvoicesDemander { get; set; }

        /// <summary>
        /// 入库单
        /// </summary>
        [DataMember]
        public string StockOrderNos { get; set; }

        /// <summary>
        /// 手续费
        /// Add by liucaijun at 2012-02-8
        /// </summary>
        [DataMember]
        public decimal Poundage { get; set; }

        /// <summary>
        /// 审核时间
        /// Add by liucaijun at 2012-03-15
        /// </summary>
        [DataMember]
        public DateTime AuditingDate { get; set; }

        /// <summary>
        /// 完成时间
        /// Add by liucaijun at 2012-03-15
        /// </summary>
        [DataMember]
        public DateTime FinishDate { get; set; }

        /// <summary>
        /// 公司ID
        /// </summary>
        [DataMember]
        public Guid FilialeId { get; set; }

        /// <summary>
        /// 交易流水号
        /// </summary>
        [DataMember]
        public string DealFlowNo { get; set; }

        [DataMember]
        public bool IsOut { get; set; }

        /// <summary>往来单位收付款--付款银行ID
        /// </summary>
        [DataMember]
        public Guid PayBankAccountsId { get; set; }

        #region add by liangcanren at 2015-05-29  往来帐对账
        /// <summary>
        /// 包含的出入库单据号
        /// </summary>
        [DataMember]
        public string IncludeStockNos { get; set; }

        /// <summary>
        /// 排除的出入库单据号
        /// </summary>
        [DataMember]
        public string DebarStockNos { get; set; }
        #endregion

        #region  当年折扣和去年返利

        [DataMember]
        public decimal LastRebate { get; set; }

        #endregion

        /// <summary>
        /// 付款期
        /// zal 2015-01-12
        /// </summary>
        [DataMember]
        public DateTime? PaymentDate { get; set; }

        /// <summary>
        /// 退货单
        /// zal 2015-01-12
        /// </summary>
        [DataMember]
        public string ReturnOrder { get; set; }
        /// <summary>
        /// 付款单
        /// zal 2015-01-12
        /// </summary>
        [DataMember]
        public string PayOrder { get; set; }

        /// <summary>
        /// 收款银行账户
        /// </summary>
        [DataMember]
        public Guid ReceiveBankAccountId { get; set; }
        #endregion

        #region
        /// <summary>
        /// 是否紧急
        /// </summary>
        public bool IsUrgent { get; set; }

        /// <summary>
        /// 紧急描述
        /// </summary>
        public string UrgentRemark { get; set; }

        /// <summary>
        /// 是否为劳务费
        /// </summary>
        public bool IsServiceFee
        {
            get { return ReceiptType == (int) CompanyFundReceiptType.Receive && SettleStartDate == new DateTime(1900,1,1); } 
        }

        /// <summary>
        /// 收款开票状态
        /// </summary>
        public int InvoiceState { get; set; }

        /// <summary>
        /// 开票单位
        /// </summary>
        public string InvoiceUnit { get; set; }
        #endregion
    }
}
