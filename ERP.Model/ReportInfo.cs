using System;
//================================================
// 功能：费用审批申报实体类
// 作者：刘彩军
// 时间：2010-11-1
//================================================
namespace Keede.Ecsoft.Model
{
    /// <summary>
    /// 费用审批申报实体类
    /// </summary>
    public class ReportInfo
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        public ReportInfo() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reportId">申报ID</param>
        /// <param name="reportNo">申报NO</param>
        /// <param name="reportClassId">申报审批类别ID</param>
        /// <param name="reportPersonnelId">申报人ID</param>
        /// <param name="reportCost">申报金额</param>
        /// <param name="bankName">银行名称</param>
        /// <param name="accountsName">账户名</param>
        /// <param name="accounts">账号</param>
        /// <param name="reportDate">申报日期</param>
        /// <param name="reportMinDate">申报打款最早日期</param>
        /// <param name="reportMaxDate">申报打款最迟日期</param>
        /// <param name="auditingState">申报状态</param>
        /// <param name="memo">申报备注</param>
        public ReportInfo(Guid reportId, Guid reportClassId, Guid reportPersonnelId, Decimal reportCost, string bankName, string accountsName, string accounts, DateTime reportDate, DateTime reportMinDate, DateTime reportMaxDate, Int32 auditingState, string memo, string reportNo)
        {
            ReportId = reportId;
            ReportNo = reportNo;
            ReportClassId = reportClassId;
            ReportPersonnelId = reportPersonnelId;
            ReportCost = reportCost;
            BankName = bankName;
            AccountsName = accountsName;
            Accounts = accounts;
            ReportDate = reportDate;
            ReportMinDate = reportMinDate;
            ReportMaxDate = reportMaxDate;
            AuditingState = auditingState;
            Memo = memo;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reportId">申报ID</param>
        /// <param name="reportNo">申报NO</param>
        /// <param name="reportClassId">申报审批类别ID</param>
        /// <param name="reportPersonnelId">申报人ID</param>
        /// <param name="reportCost">申报金额</param>
        /// <param name="bankName">银行名称</param>
        /// <param name="accountsName">账户名</param>
        /// <param name="accounts">账号</param>
        /// <param name="reportDate">申报日期</param>
        /// <param name="reportMinDate">申报打款最早日期</param>
        /// <param name="reportMaxDate">申报打款最迟日期</param>
        /// <param name="auditingState">申报状态</param>
        /// <param name="memo">申报备注</param>
        /// <param name="kind">账目类型</param>
        /// <param name="accountId">账目ID</param>
        /// <param name="bankAccountId">银行ID</param>
        public ReportInfo(Guid reportId, Guid reportClassId, Guid reportPersonnelId, Decimal reportCost, string bankName, string accountsName, string accounts, DateTime reportDate, DateTime reportMinDate, DateTime reportMaxDate, Int32 auditingState, string memo, string reportNo, Int32 kind, Guid accountId, Guid bankAccountId)
        {
            ReportId = reportId;
            ReportNo = reportNo;
            ReportClassId = reportClassId;
            ReportPersonnelId = reportPersonnelId;
            ReportCost = reportCost;
            BankName = bankName;
            AccountsName = accountsName;
            Accounts = accounts;
            ReportDate = reportDate;
            ReportMinDate = reportMinDate;
            ReportMaxDate = reportMaxDate;
            AuditingState = auditingState;
            Memo = memo;
            Kind = kind;
            AccountId = accountId;
            BankAccountId = bankAccountId;
        }

        /// <summary>
        /// 申报ID
        /// </summary>
        public Guid ReportId { get; set; }

        /// <summary>
        /// 申报NO
        /// </summary>
        public string ReportNo { get; set; }

        /// <summary>
        /// 申报审批类别ID
        /// </summary>
        public Guid ReportClassId { get; set; }

        /// <summary>
        /// 申报人ID
        /// </summary>
        public Guid ReportPersonnelId { get; set; }

        /// <summary>
        /// 申报金额
        /// </summary>
        public Decimal ReportCost { get; set; }

        /// <summary>
        /// 银行名称
        /// </summary>
        public String BankName { get; set; }

        /// <summary>
        /// 账户名
        /// </summary>
        public String AccountsName { get; set; }

        /// <summary>
        /// 账号
        /// </summary>
        public String Accounts { get; set; }

        /// <summary>
        /// 申报日期
        /// </summary>
        public DateTime ReportDate { get; set; }

        /// <summary>
        /// 申报打款最早日期
        /// </summary>
        public DateTime ReportMinDate { get; set; }

        /// <summary>
        /// 申报打款最迟日期
        /// </summary>
        public DateTime ReportMaxDate { get; set; }

        /// <summary>
        /// 申报状态
        /// </summary>
        public Int32 AuditingState { get; set; }

        /// <summary>
        /// 申报备注
        /// </summary>
        public string Memo { get; set; }

        /// <summary>
        /// 账目类型
        /// </summary>
        public Int32 Kind { get; set; }

        /// <summary>
        /// 账目ID
        /// </summary>
        public Guid AccountId { get; set; }

        /// <summary>
        /// 银行ID
        /// </summary>
        public Guid BankAccountId { get; set; }
    }
}
