using System;
//================================================
// 功能：费用申报实体类
// 作者：刘彩军
// 时间：2011-February-22th
//================================================
namespace Keede.Ecsoft.Model
{
    ///<summary>
    /// 费用申报实体类
    ///</summary>
    [Serializable]
    public class CostReportInfo
    {
        ///<summary>
        /// 默认构造函数
        ///</summary>
        public CostReportInfo() { }

        /// <summary>
        /// 添加申报专用
        /// </summary>
        public CostReportInfo(Guid reportId, String reportNo, Guid companyClassId, String reportName, Decimal reportCost, Decimal payCost,
            String payCompany, String bankAccountName, String bankAccount, String reportMemo, Int32 costType, Int32 reportKind, Int32 invoiceType,
            Guid invoiceId, Int32 state, String auditingMemo, String memo, Guid reportPersonnelId, DateTime reportDate, Guid reportFilialeId,
            Guid reportBranchId, string mobile, int refundmentKind, Guid refundmentBankAccountId)
        {
            ReportId = reportId;
            ReportNo = reportNo;
            CompanyClassId = companyClassId;
            ReportName = reportName;
            ReportCost = reportCost;
            PayCost = payCost;
            PayCompany = payCompany;
            BankAccountName = bankAccountName;
            BankAccount = bankAccount;
            ReportMemo = reportMemo;
            CostType = costType;
            ReportKind = reportKind;
            InvoiceType = invoiceType;
            InvoiceId = invoiceId;
            State = state;
            AuditingMemo = auditingMemo;
            Memo = memo;
            ReportPersonnelId = reportPersonnelId;
            ReportDate = reportDate;
            ReportFilialeId = reportFilialeId;
            ReportBranchId = reportBranchId;
            Mobile = mobile;
            RefundmentKind = refundmentKind;
            RefundmentBankAccountId = refundmentBankAccountId;
        }

        /// <summary>
        /// 取数据时用
        /// </summary>
        public CostReportInfo(Guid reportId, String reportNo, Guid companyClassId, String reportName, Decimal reportCost, Decimal payCost,
            String payCompany, String bankAccountName, String bankAccount, String reportMemo, Int32 costType, Int32 reportKind, Int32 invoiceType,
            Guid invoiceId, Int32 state, String auditingMemo, String memo, Guid reportPersonnelId, DateTime reportDate, Guid companyId, Guid auditingMan,
            Decimal realityCost, Guid payBankAccountId, Guid reportFilialeId, Guid reportBranchId, string mobile, int refundmentKind,
            Guid refundmentBankAccountId, DateTime executeDate)
        {
            ReportId = reportId;
            ReportNo = reportNo;
            CompanyClassId = companyClassId;
            ReportName = reportName;
            ReportCost = reportCost;
            PayCost = payCost;
            PayCompany = payCompany;
            BankAccountName = bankAccountName;
            BankAccount = bankAccount;
            ReportMemo = reportMemo;
            CostType = costType;
            ReportKind = reportKind;
            InvoiceType = invoiceType;
            InvoiceId = invoiceId;
            State = state;
            AuditingMemo = auditingMemo;
            Memo = memo;
            ReportPersonnelId = reportPersonnelId;
            ReportDate = reportDate;
            CompanyId = companyId;
            AuditingMan = auditingMan;
            RealityCost = realityCost;
            PayBankAccountId = payBankAccountId;
            ReportFilialeId = reportFilialeId;
            ReportBranchId = reportBranchId;
            Mobile = mobile;
            RefundmentKind = refundmentKind;
            RefundmentBankAccountId = refundmentBankAccountId;
            ExecuteDate = executeDate;
        }

        /// <summary>
        /// 费用申报统计专用
        /// </summary>
        public CostReportInfo(Guid companyId, Decimal realityCost)
        {
            CompanyId = companyId;
            RealityCost = realityCost;
        }

        /// <summary>
        ///申报ID
        /// </summary>
        public Guid ReportId { get; set; }

        /// <summary>
        ///申报单号
        /// </summary>
        public String ReportNo { get; set; }

        /// <summary>
        ///费用分类ID
        /// </summary>
        public Guid CompanyClassId { get; set; }

        /// <summary>
        /// 费用申报类型名称
        /// </summary>
        public string ReportClassName { get; set; }

        /// <summary>
        ///费用名称
        /// </summary>
        public String ReportName { get; set; }

        /// <summary>
        ///申报金额
        /// </summary>
        public Decimal ReportCost { get; set; }

        /// <summary>
        ///已支付金额
        /// </summary>
        public Decimal PayCost { get; set; }

        /// <summary>
        ///收/付款单位
        /// </summary>
        public String PayCompany { get; set; }

        /// <summary>
        ///收/付款银行支行
        /// </summary>
        public String BankAccountName { get; set; }

        /// <summary>
        ///收款账号
        /// </summary>
        public String BankAccount { get; set; }

        /// <summary>
        ///申报说明
        /// </summary>
        public String ReportMemo { get; set; }

        /// <summary>
        ///结算方式:1现金 2转账
        /// </summary>
        public Int32 CostType { get; set; }

        /// <summary>
        ///申报类型:1预借款 2凭证报销 3已扣款核销 4费用收入
        /// </summary>
        public Int32 ReportKind { get; set; }

        /// <summary>
        ///票据类型:1普通发票 2收据 5增值税专用发票
        /// </summary>
        public Int32 InvoiceType { get; set; }

        /// <summary>
        ///单据ID
        /// </summary>
        public Guid InvoiceId { get; set; }

        /// <summary>
        ///状态
        /// </summary>
        public Int32 State { get; set; }

        /// <summary>
        ///审批说明
        /// </summary>
        public String AuditingMemo { get; set; }

        /// <summary>
        ///备注
        /// </summary>
        public String Memo { get; set; }

        /// <summary>
        ///费用申报人ID
        /// </summary>
        public Guid ReportPersonnelId { get; set; }

        /// <summary>
        ///申报时间
        /// </summary>
        public DateTime ReportDate { get; set; }

        /// <summary>
        /// 打款分类
        /// </summary>
        public Guid CompanyId { get; set; }

        /// <summary>
        /// 审批人
        /// </summary>
        public Guid AuditingMan { get; set; }

        /// <summary>
        /// 实际金额
        /// </summary>
        public Decimal RealityCost { get; set; }

        /// <summary>
        /// 结算账号
        /// </summary>
        public Guid PayBankAccountId { get; set; }

        /// <summary>
        /// 打款银行
        /// </summary>
        public string BankName { get; set; }

        /// <summary>
        /// 申报人公司ID
        /// </summary>
        public Guid ReportFilialeId { get; set; }

        /// <summary>
        /// 申报人部门ID
        /// </summary>
        public Guid ReportBranchId { get; set; }

        /// <summary>
        /// 结算公司ID
        /// </summary>
        public Guid AssumeFilialeId { get; set; }

        /// <summary>
        /// 费用承担部门ID
        /// </summary>
        public Guid AssumeBranchId { get; set; }

        /// <summary>
        /// 费用承担小组
        /// </summary>
        public Guid AssumeGroupId { get; set; }

        /// <summary>
        /// 费用承担店铺
        /// </summary>
        public Guid AssumeShopId { get; set; }

        /// <summary>
        /// 移动电话
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 退款方式(1：现金；2：转账)
        /// </summary>
        public int RefundmentKind { get; set; }

        /// <summary>
        /// 退款帐号ID
        /// </summary>
        public Guid RefundmentBankAccountId { get; set; }

        /// <summary>
        /// 执行收付款时间
        /// </summary>
        public DateTime ExecuteDate { get; set; }

        /// <summary>
        /// 手续费
        /// </summary>
        public Decimal Poundage { get; set; }

        /// <summary>
        /// 本公司打款银行
        /// </summary>
        public Guid ExecuteBankId { get; set; }

        /// <summary>
        /// 审批时间
        /// </summary>
        public DateTime AuditingDate { get; set; }

        /// <summary>
        /// 完成时间
        /// </summary>
        public DateTime FinishDate { get; set; }

        /// <summary>
        /// 发票号码
        /// </summary>
        public string InvoiceNo { get; set; }

        /// <summary>
        /// 发票/收据抬头
        /// </summary>
        public string InvoiceTitle { get; set; }

        /// <summary>
        /// 发票/收据抬头公司Id
        /// </summary>
        public Guid InvoiceTitleFilialeId { get; set; }

        /// <summary>
        /// 收据号码
        /// </summary>
        public string ReceiptNo { get; set; }

        /// <summary>
        /// 紧急程度(1:加急;0:下周处理)
        /// </summary>
        public int UrgentOrDefer { get; set; }

        /// <summary>
        /// 加急原因
        /// </summary>
        public string UrgentReason { get; set; }

        /// <summary>
        /// 打款前审批 1不审批 2须审批
        /// </summary>
        public int AuditingBeforePay { get; set; }

        /// <summary>
        /// 交易流水号
        /// </summary>
        public string TradeNo { get; set; }

        /// <summary>
        /// 如果有票据，此字段是票据金额的总和；如果没有票据，此字段的值等于申报金额的值，但没有其他用途；
        /// </summary>
        public Decimal ApplyForCost { get; set; }

        /// <summary>
        /// 如果是预借款类型，此字段是申请金额的总和(此金额不包含系统自动生成的金额)；如果是其他类型，此字段的值等于申报金额的值，但没有其他用途；
        /// </summary>
        public Decimal ActualAmount { get; set; }

        /// <summary>最晚提交票据日
        /// </summary>
        public DateTime FinallySubmitTicketDate { get; set; }

        /// <summary>
        /// 结算账号是否是公司账户(是：true；否：false；)
        /// </summary>
        public bool IsOut { get; set; }
        /// <summary>
        /// 费用实际发生开始时间
        /// zal 2015-11-04
        /// </summary>
        public DateTime StartTime { get; set; }
        /// <summary>
        /// 费用实际发生结束时间
        /// zal 2015-11-04
        /// </summary>
        public DateTime EndTime { get; set; }
        /// <summary>
        /// 费用种类(0:广告费;1:购买物品;2:纯费用;)
        /// zal 2015-11-04
        /// </summary>
        public int CostsVarieties { get; set; }

        /// <summary>
        /// 押金(1:是;2:否)
        /// zal 2016-08-03
        /// </summary>
        public int Deposit { get; set; }

        /// <summary>
        /// 押金编号
        /// </summary>
        /// zal 2016-08-17
        public string DepositNo { get; set; }

        /// <summary>
        /// 人事部物品管理编码
        /// </summary>
        /// zal 2016-08-05
        public string GoodsCode { get; set; }

        /// <summary>
        /// 受理时间
        /// </summary>
        /// zal 2016-08-18
        public DateTime? AcceptDate { get; set; }

        /// <summary>
        /// 审阅(0:未审阅;1:已审阅;)
        /// zal 2016-08-19
        /// </summary>
        public int ReviewState { get; set; }

        /// <summary>
        /// 是否最后一次(True:是;False:否;)
        /// </summary>
        /// zal 2016-11-21
        public bool IsLastTime { get; set; }

        /// <summary>
        /// 是否系统生成(True:是;False:否;)
        /// </summary>
        /// zal 2016-11-21
        public bool IsSystem { get; set; }

        /// <summary>
        /// 申请次数
        /// </summary>
        /// zal 2016-12-05
        public int ApplyNumber { get; set; }

        /// <summary>
        /// 是否终结(True:是;False:否;)
        /// </summary>
        /// zal 2016-11-21
        public bool IsEnd { get; set; }
    }
}
