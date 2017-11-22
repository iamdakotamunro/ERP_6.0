using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using ERP.Enum;
using ERP.Environment;
using Keede.DAL.Helper;
using Keede.Ecsoft.Model;

namespace ERP.DAL.Implement.Inventory
{
    /// <summary>
    /// 费用申报数据查询
    /// </summary>
    public partial class CostReport
    {
        public bool IsRepeatSubmit(Guid reportId, Guid assumeFilialeId, Guid assumeBranchId, Guid assumeGroupId, Guid reportPersonnelId, Guid companyClassId, Guid companyId, int reportKind, decimal reportCost)
        {
            var strbSql = new StringBuilder();
            strbSql.Append("SELECT 1 FROM lmShop_CostReport WHERE State NOT IN (7,13)");
            strbSql.Append(" AND AssumeFilialeId='").Append(assumeFilialeId).Append("'");
            strbSql.Append(" AND AssumeBranchId='").Append(assumeBranchId).Append("'");
            if (assumeGroupId != Guid.Empty)
                strbSql.Append(" AND AssumeGroupId='").Append(assumeGroupId).Append("'");
            //strbSql.Append(" AND ReportPersonnelId='").Append(reportPersonnelId).Append("'");
            strbSql.Append(" AND CompanyClassId='").Append(companyClassId).Append("'");
            strbSql.Append(" AND CompanyId='").Append(companyId).Append("'");
            strbSql.Append(" AND ApplyForCost=").Append(reportCost);
            strbSql.Append(" AND ReportKind=").Append(reportKind);
            if (reportId != Guid.Empty)
            {
                strbSql.Append(" AND ReportId!='").Append(reportId).Append("'");
            }

            var obj = SqlHelper.ExecuteScalar(GlobalConfig.ERP_DB_NAME, true, strbSql.ToString());
            return obj != null && Convert.ToInt32(obj) > 0;
        }

        public string GetBankAccount(Guid reportPersonnelId, string bankAccountName)
        {
            var strbSql = new StringBuilder();
            strbSql.Append("SELECT BankAccount FROM lmShop_CostReport WHERE CostType=2");
            strbSql.Append(" AND ReportPersonnelId='").Append(reportPersonnelId).Append("'");
            strbSql.Append(" AND BankAccountName='").Append(bankAccountName).Append("'");

            string bankAccount = string.Empty;
            using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, strbSql.ToString(), null))
            {
                if (dr != null && dr.Read())
                {
                    bankAccount = GetString(dr, "BankAccount");
                }
            }
            return bankAccount;
        }

        #region[获取申报]
        /// <summary>
        /// 获取申报
        /// </summary>
        /// <returns></returns>
        public IList<CostReportInfo> GetReportList()
        {
            return GetReportCostData(SQL_GET_REPORT);
        }

        /// <summary>
        /// 获取押金列表(条件：是押金且已完成的预借款类型的单据)
        /// </summary>
        /// <param name="type">回收状态(0:未回收;1:已回收) </param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="reportPersonnelIds">申请人</param>
        /// <param name="num">预借款编号/押金编号/回收单据号</param>
        /// <returns></returns>
        /// zal 2016-08-17
        public IList<CostReportInfo> GetReportListForDeposit(int type, DateTime startTime, DateTime endTime, List<Guid> reportPersonnelIds, string num)
        {
            var sql = SQL_GET_REPORT_DEPOSITRECOVERY;

            if (type == 0)
            {
                sql += " where 1=1 and (select COUNT(0) from lmShop_CostReportDepositRecovery DR where DR.ReportId=CR.ReportId)=0";

                if (startTime != DateTime.MinValue)
                {
                    sql += " and CR.ExecuteDate>='" + startTime + "'";
                }
                if (endTime != DateTime.MinValue)
                {
                    sql += "  and CR.ExecuteDate<'" + endTime + "'";
                }
            }
            else
            {
                sql += " inner join lmShop_CostReportDepositRecovery t on t.ReportId = CR.ReportId where 1=1 ";
                sql += " and (select COUNT(0) from lmShop_CostReportDepositRecovery DR where DR.ReportId=CR.ReportId)>0";
                if (startTime != DateTime.MinValue)
                {
                    sql += " and t.RecoveryDate>='" + startTime + "'";
                }
                if (endTime != DateTime.MinValue)
                {
                    sql += "  and t.RecoveryDate<'" + endTime + "'";
                }
            }

            sql += " and ReportKind=1 and [State]=7 and Deposit=1";

            if (reportPersonnelIds != null && reportPersonnelIds.Count > 0)
            {
                var personnelIdStr = "'" + string.Join("','", reportPersonnelIds.ToArray()) + "'";
                sql += " and ReportPersonnelId IN(" + personnelIdStr + ")";
            }

            if (!string.IsNullOrEmpty(num))
            {
                sql += " and (CR.ReportNo='" + num + "' or CR.DepositNo='" + num + "')";
            }
            sql += " order by ReportDate desc";
            return GetReportCostData(sql);
        }

        public IList<CostReportInfo> GetReportList(DateTime startTime, DateTime endTime)
        {
            string sql = string.Format(SQL_GET_REPORT + " WHERE  ReportDate>='{0}' AND ReportDate<'{1}'", startTime, endTime);
            return GetReportCostData(sql);
        }

        /// <summary>
        /// 获取申报
        /// </summary>
        /// <param name="reportFilialeId">申报人公司</param>
        /// <param name="reportBranchId">申报人部门</param>
        /// <returns></returns>
        public IList<CostReportInfo> GetReportList(Guid reportFilialeId, Guid reportBranchId)
        {
            string sql = String.Format(SQL_GET_REPORT + " WHERE ReportFilialeID='{0}' AND ReportBranchID='{1}'", reportFilialeId, reportBranchId);
            return GetReportCostData(sql);
        }

        public IList<CostReportInfo> GetReportList(IList<int> states)
        {
            var state = string.Join(",", states.ToArray());
            string sql = string.Format(@"
SELECT 
    ReportId
    ,ReportNo
    ,cr.CompanyClassId
    ,ccc.CompanyClassName AS ReportClassName
    ,ReportName
    ,ReportCost
    ,PayCost
    ,PayCompany
    ,BankAccountName
    ,BankAccount
    ,ReportMemo
    ,CostType
    ,ReportKind
    ,InvoiceType
    ,InvoiceId
    ,[State]
    ,AuditingMemo
    ,Memo
    ,ReportPersonnelId
    ,ReportDate
    ,CompanyId
    ,AuditingMan
    ,RealityCost
    ,PayBankAccountID
    ,ReportFilialeID
    ,ReportBranchID
    ,Mobile
    ,RefundmentKind
    ,RefundmentBankAccountID
    ,ExecuteDate
    ,Poundage
    ,ExecuteBankId
    ,AuditingDate
    ,FinishDate
    ,AssumeFilialeId
    ,AssumeBranchId
    ,AssumeGroupId
    ,AuditingBeforePay
    ,UrgentOrDefer,UrgentReason,TradeNo,ApplyForCost,ActualAmount,InvoiceNo,InvoiceTitle,InvoiceTitleFilialeId,ReceiptNo,AssumeShopId,BA.BankName,Deposit,DepositNo,GoodsCode,AcceptDate,ReviewState,cr.CostsVarieties 
FROM lmShop_CostReport cr
INNER JOIN lmShop_CostCompanyClass ccc ON ccc.CompanyClassId = cr.CompanyClassId
LEFT JOIN lmShop_BankAccounts BA ON cr.PayBankAccountID=BA.BankAccountsId
WHERE [State] IN ({0}) 
ORDER BY ReportDate DESC
", state);
            IList<CostReportInfo> list = new List<CostReportInfo>();
            using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, sql))
            {
                while (dr.Read())
                {
                    var info = new CostReportInfo
                    {
                        ReportId = GetGuid(dr, "ReportId"),
                        ReportNo = GetString(dr, "ReportNo"),
                        CompanyClassId = GetGuid(dr, "CompanyClassId"),
                        CompanyId = GetGuid(dr, "CompanyId"),
                        ReportName = GetString(dr, "ReportName"),
                        ReportCost = GetDecimal(dr, "ReportCost"),
                        PayCost = GetDecimal(dr, "PayCost"),
                        PayCompany = GetString(dr, "PayCompany"),
                        BankAccountName = GetString(dr, "BankAccountName"),
                        BankAccount = GetString(dr, "BankAccount"),
                        ReportMemo = GetString(dr, "ReportMemo"),
                        CostType = GetInt(dr, "CostType"),
                        ReportKind = GetInt(dr, "ReportKind"),
                        InvoiceType = GetInt(dr, "InvoiceType"),
                        InvoiceId = GetGuid(dr, "InvoiceId"),
                        State = GetInt(dr, "State"),
                        AuditingMemo = GetString(dr, "AuditingMemo"),
                        Memo = GetString(dr, "Memo"),
                        ReportPersonnelId = GetGuid(dr, "ReportPersonnelId"),
                        ReportDate = GetDateTime(dr, "ReportDate"),
                        AuditingMan = GetGuid(dr, "AuditingMan"),
                        RealityCost = GetDecimal(dr, "RealityCost"),
                        PayBankAccountId = GetGuid(dr, "PayBankAccountID"),
                        ReportFilialeId = GetGuid(dr, "ReportFilialeID"),
                        ReportBranchId = GetGuid(dr, "ReportBranchID"),
                        Mobile = GetString(dr, "Mobile"),
                        RefundmentKind = GetInt(dr, "RefundmentKind"),
                        RefundmentBankAccountId = GetGuid(dr, "RefundmentBankAccountID"),
                        ExecuteDate = GetDateTime(dr, "ExecuteDate"),
                        Poundage = GetDecimal(dr, "Poundage"),
                        ExecuteBankId = GetGuid(dr, "ExecuteBankId"),
                        AuditingDate = GetDateTime(dr, "AuditingDate"),
                        FinishDate = GetDateTime(dr, "FinishDate"),
                        AssumeFilialeId = GetGuid(dr, "AssumeFilialeId"),
                        AssumeBranchId = GetGuid(dr, "AssumeBranchId"),
                        AssumeGroupId = GetGuid(dr, "AssumeGroupId"),
                        AuditingBeforePay = GetInt(dr, "AuditingBeforePay"),
                        UrgentOrDefer = GetInt(dr, "UrgentOrDefer"),
                        UrgentReason = GetString(dr, "UrgentReason"),
                        TradeNo = GetString(dr, "TradeNo"),
                        ApplyForCost = GetDecimal(dr, "ApplyForCost"),
                        ActualAmount = GetDecimal(dr, "ActualAmount"),
                        InvoiceNo = GetString(dr, "InvoiceNo"),
                        InvoiceTitle = GetString(dr, "InvoiceTitle"),
                        InvoiceTitleFilialeId = GetGuid(dr, "InvoiceTitleFilialeId"),
                        ReceiptNo = GetString(dr, "ReceiptNo"),
                        AssumeShopId = GetGuid(dr, "AssumeShopId"),
                        BankName = GetString(dr, "BankName"),
                        Deposit = GetInt(dr, "Deposit"),
                        DepositNo = GetString(dr, "DepositNo"),
                        GoodsCode = GetString(dr, "GoodsCode"),
                        AcceptDate = GetDateTime(dr, "AcceptDate"),
                        ReviewState = GetInt(dr, "ReviewState"),
                        CostsVarieties = GetInt(dr, "CostsVarieties")
                    };
                    list.Add(info);
                }
            }
            return list;
        }

        public IList<CostReportInfo> GetReportList(DateTime startTime, DateTime endTime, string branchIdString, string invoiceType, string state)
        {
            string sql = string.Format(@"
SELECT 
    ReportId
    ,ReportNo
    ,cr.CompanyClassId
    ,ccc.CompanyClassName AS ReportClassName
    ,ReportName
    ,ReportCost
    ,PayCost
    ,PayCompany
    ,BankAccountName
    ,BankAccount
    ,ReportMemo
    ,CostType
    ,ReportKind
    ,InvoiceType
    ,InvoiceId
    ,[State]
    ,AuditingMemo
    ,Memo
    ,ReportPersonnelId
    ,ReportDate
    ,CompanyId
    ,AuditingMan
    ,RealityCost
    ,PayBankAccountID
    ,ReportFilialeID
    ,ReportBranchID
    ,Mobile
    ,RefundmentKind
    ,RefundmentBankAccountID
    ,ExecuteDate
    ,Poundage
    ,ExecuteBankId
    ,AuditingDate
    ,FinishDate
    ,AssumeFilialeId
    ,AssumeBranchId
    ,AssumeGroupId
    ,AuditingBeforePay
    ,UrgentOrDefer,UrgentReason,TradeNo,ApplyForCost,ActualAmount,InvoiceNo,InvoiceTitle,InvoiceTitleFilialeId,ReceiptNo,AssumeShopId,BA.BankName  
FROM lmShop_CostReport cr
INNER JOIN lmShop_CostCompanyClass ccc ON ccc.CompanyClassId = cr.CompanyClassId
LEFT JOIN lmShop_BankAccounts BA ON cr.PayBankAccountID=BA.BankAccountsId
WHERE  ReportDate >= @StartTime AND ReportDate < @EndTime
AND InvoiceType IN ({0}) 
AND [State] {1}
ORDER BY ReportDate DESC
", invoiceType, state);
            using (var db = DatabaseFactory.Create())
            {
                return db.Select<CostReportInfo>(true, sql, new Parameter("StartTime", startTime), new Parameter("EndTime", endTime)).ToList();
            }
        }

        #endregion

        #region[根据用户ID获取申报]
        /// <summary>
        /// 根据用户ID获取申报
        /// </summary>
        /// <param name="reportPersonnelId">用户ID</param>
        /// <returns></returns>
        public IList<CostReportInfo> GetReportList(Guid reportPersonnelId)
        {
            var parm = new SqlParameter(PARM_REPORT_PERSONNEL_ID, SqlDbType.UniqueIdentifier) { Value = reportPersonnelId };
            var sql = new StringBuilder(SQL_GET_REPORT);
            sql.Append(" WHERE ReportPersonnelId=@ReportPersonnelId");
            return GetReportCostData(sql.ToString(), parm);
        }
        #endregion

        #region[根据用户IDlist获取申报]
        /// <summary>
        /// 根据用户IDlist获取申报
        /// </summary>
        /// <param name="reportPersonnelIds">用户IDlist</param>
        /// <returns></returns>
        public IList<CostReportInfo> GetReportList(List<Guid> reportPersonnelIds)
        {
            var personnelIdStr = "'" + string.Join("','", reportPersonnelIds.ToArray()) + "'";
            var sql = new StringBuilder(SQL_GET_REPORT);
            sql.Append(" WHERE ReportPersonnelId IN(" + personnelIdStr + ")");
            return GetReportCostData(sql.ToString(), null);
        }
        #endregion

        #region[获取申报,根据申报ID获取出申报内容]
        /// <summary>
        /// 获取申报,根据申报ID获取出申报内容
        /// </summary>
        /// <param name="reportId">申报ID</param>
        /// <returns></returns>
        public CostReportInfo GetReportByReportId(Guid reportId)
        {
            var info = new CostReportInfo();
            var parm = new SqlParameter(PARM_REPORT_ID, SqlDbType.UniqueIdentifier) { Value = reportId };
            var sql = new StringBuilder(SQL_GET_REPORT);
            sql.Append(" WHERE ReportId=@ReportId");
            using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, sql.ToString(), parm))
            {
                if (dr.Read())
                {
                    info = new CostReportInfo
                    {
                        ReportId = GetGuid(dr, "ReportId"),
                        ReportNo = GetString(dr, "ReportNo"),
                        CompanyClassId = GetGuid(dr, "CompanyClassId"),
                        CompanyId = GetGuid(dr, "CompanyId"),
                        ReportName = GetString(dr, "ReportName"),
                        ReportCost = GetDecimal(dr, "ReportCost"),
                        PayCost = GetDecimal(dr, "PayCost"),
                        PayCompany = GetString(dr, "PayCompany"),
                        BankAccountName = GetString(dr, "BankAccountName"),
                        BankAccount = GetString(dr, "BankAccount"),
                        ReportMemo = GetString(dr, "ReportMemo"),
                        CostType = GetInt(dr, "CostType"),
                        ReportKind = GetInt(dr, "ReportKind"),
                        InvoiceType = GetInt(dr, "InvoiceType"),
                        InvoiceId = GetGuid(dr, "InvoiceId"),
                        State = GetInt(dr, "State"),
                        AuditingMemo = GetString(dr, "AuditingMemo"),
                        Memo = GetString(dr, "Memo"),
                        ReportPersonnelId = GetGuid(dr, "ReportPersonnelId"),
                        ReportDate = GetDateTime(dr, "ReportDate"),
                        AuditingMan = GetGuid(dr, "AuditingMan"),
                        RealityCost = GetDecimal(dr, "RealityCost"),
                        PayBankAccountId = GetGuid(dr, "PayBankAccountID"),
                        ReportFilialeId = GetGuid(dr, "ReportFilialeID"),
                        ReportBranchId = GetGuid(dr, "ReportBranchID"),
                        Mobile = GetString(dr, "Mobile"),
                        RefundmentKind = GetInt(dr, "RefundmentKind"),
                        RefundmentBankAccountId = GetGuid(dr, "RefundmentBankAccountID"),
                        ExecuteDate = GetDateTime(dr, "ExecuteDate"),
                        Poundage = GetDecimal(dr, "Poundage"),
                        ExecuteBankId = GetGuid(dr, "ExecuteBankId"),
                        AuditingDate = GetDateTime(dr, "AuditingDate"),
                        FinishDate = GetDateTime(dr, "FinishDate"),
                        AssumeFilialeId = GetGuid(dr, "AssumeFilialeId"),
                        AssumeBranchId = GetGuid(dr, "AssumeBranchId"),
                        AssumeGroupId = GetGuid(dr, "AssumeGroupId"),
                        AuditingBeforePay = GetInt(dr, "AuditingBeforePay"),
                        UrgentOrDefer = GetInt(dr, "UrgentOrDefer"),
                        UrgentReason = GetString(dr, "UrgentReason"),
                        TradeNo = GetString(dr, "TradeNo"),
                        ApplyForCost = GetDecimal(dr, "ApplyForCost"),
                        ActualAmount = GetDecimal(dr, "ActualAmount"),
                        InvoiceNo = GetString(dr, "InvoiceNo"),
                        InvoiceTitle = GetString(dr, "InvoiceTitle"),
                        InvoiceTitleFilialeId = GetGuid(dr, "InvoiceTitleFilialeId"),
                        ReceiptNo = GetString(dr, "ReceiptNo"),
                        AssumeShopId = GetGuid(dr, "AssumeShopId"),
                        IsOut = GetBoolean(dr, "IsOut"),
                        BankName = GetString(dr, "BankName"),
                        StartTime = GetDateTime(dr, "StartTime"),
                        EndTime = GetDateTime(dr, "EndTime"),
                        CostsVarieties = GetInt(dr, "CostsVarieties"),
                        Deposit = GetInt(dr, "Deposit"),
                        DepositNo = GetString(dr, "DepositNo"),
                        GoodsCode = GetString(dr, "GoodsCode"),
                        AcceptDate = GetDateTime(dr, "AcceptDate"),
                        ReviewState = GetInt(dr, "ReviewState"),
                        IsLastTime = GetBoolean(dr, "IsLastTime"),
                        IsSystem = GetBoolean(dr, "IsSystem"),
                        ApplyNumber = GetInt(dr, "ApplyNumber"),
                        IsEnd = GetBoolean(dr, "IsEnd")
                    };
                }
            }
            return info;
        }
        #endregion

        #region[获取申报统计]

        /// <summary>
        /// 获取申报统计
        /// </summary>
        /// <param name="branch">申报部门</param>
        /// <param name="classId">费用分类</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">截止日期</param>
        /// <param name="filialeId"> </param>
        /// <returns></returns>
        public IList<CostReportInfo> GetReportStatistics(DateTime startDate, DateTime endDate, Guid classId, Guid branch, Guid filialeId)
        {
            var sql = new StringBuilder("SELECT CompanyId,SUM(PayCost) AS amount FROM lmShop_CostReport WHERE [State] IN(6,7,10)");
            if (classId == Guid.Empty)
            {
                sql = new StringBuilder("SELECT CompanyClassId,SUM(PayCost) AS amount FROM lmShop_CostReport WHERE [State] IN(6,7,10) ");
            }
            if (startDate != DateTime.MinValue)
            {
                sql.Append(" AND ReportDate>='" + startDate + "'");
            }
            if (endDate != DateTime.MinValue)
            {
                sql.Append(" AND ReportDate<'" + endDate + "'");
            }
            if (filialeId != Guid.Empty)
            {
                sql.Append("AND AssumeFilialeId='" + filialeId + "'");
            }
            if (branch != Guid.Empty)
            {
                sql.Append(" AND AssumeBranchId='" + branch + "'");
            }
            if (classId != Guid.Empty)
            {
                sql.Append(" AND CompanyClassId='" + classId + "'");
                sql.Append(" GROUP BY CompanyId");
            }
            else
            {
                sql.Append(" GROUP BY CompanyClassId");
            }
            IList<CostReportInfo> list = new List<CostReportInfo>();
            using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, sql.ToString(), null))
            {
                while (dr.Read())
                {
                    var info = new CostReportInfo(dr.GetGuid(0), dr.GetDecimal(1));
                    list.Add(info);
                }
            }
            return list;
        }
        #endregion

        #region[根据部门获取部门申报统计]

        /// <summary>
        /// 根据部门获取部门申报统计
        /// </summary>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">截止日期</param>
        /// <param name="branchId"></param>
        /// <param name="filialeId"> </param>
        /// <returns></returns>
        public IList<CostReportInfo> GetReportStatisticsByBranch(DateTime startDate, DateTime endDate, Guid branchId, Guid filialeId)
        {
            const string SQL = @"SELECT CompanyId AS CompanyId,SUM(PayCost) AS TotalCost FROM lmShop_CostReport WHERE [State] IN(6,7,10)";
            var childSql = new StringBuilder(SQL);
            if (startDate != DateTime.MinValue)
            {
                childSql.Append(" AND ReportDate>='" + startDate + "'");
            }
            if (endDate != DateTime.MinValue)
            {
                childSql.Append(" AND ReportDate<'" + endDate + "'");
            }
            if (filialeId != Guid.Empty)
            {
                childSql.Append("AND AssumeFilialeId='" + filialeId + "'");
            }
            if (branchId != Guid.Empty)
            {
                childSql.Append(" AND AssumeBranchId='" + branchId + "'");
            }
            childSql.Append(" GROUP BY CompanyId ORDER BY TotalCost DESC");

            IList<CostReportInfo> list = new List<CostReportInfo>();
            using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, childSql.ToString(), null))
            {
                while (dr.Read())
                {
                    var info = new CostReportInfo(GetGuid(dr, "CompanyId"), dr.GetDecimal(1));
                    list.Add(info);
                }
            }
            return list;
        }
        #endregion

        #region[取首页提示数据 dhq]
        public IList<CostReportInfo> GetAllPrompinfo(Guid filialeId, Guid branchId, Guid positionId)
        {
            string sql = "SELECT DISTINCT [State],[ReportId] FROM [lmShop_CostReport] INNER JOIN [lmShop_CostReportAuditing] power on [ReportDate] BETWEEN @start AND @end AND power.[AuditingFilialeId]=@filialeId AND power.[AuditingBranchId]=@branchId AND power.[AuditingPositionId]=@positionId AND ReportFilialeID=power.AuditingFilialeId AND CharIndex(CAST(lmShop_CostReport.ReportBranchID as varchar(36)),power.ReportBranchId)>0 AND (([ReportCost]<=power.[MaxAmount] AND ReportCost>=power.[MinAmount] AND power.[Kind]=" + (int)CostReportAuditingType.Auditing + ") OR ([state]=" + (int)CostReportState.NoAuditing + " AND power.[Kind]=" + (int)CostReportAuditingType.Invoice + "))";

            var sqlparams = new[]{
                new SqlParameter("@positionId",positionId),
                new SqlParameter("@filialeId",filialeId),
                new SqlParameter("@branchId",branchId),
                new SqlParameter("@start",DateTime.Now.AddMonths(-3)),
                new SqlParameter("@end",DateTime.Now)
            };

            IList<CostReportInfo> list = new List<CostReportInfo>();
            using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, sql, sqlparams))
            {
                while (dr.Read())
                {
                    var entity = new CostReportInfo { State = dr.GetInt32(0) };
                    list.Add(entity);
                }
            }
            return list;
        }
        #endregion

        /// <summary>根据费用申报编号、名称、收款人、备注获取费用申报
        /// </summary>
        /// <param name="reportNo">申报编号</param>
        /// <param name="reportName">申报名称</param>
        /// <param name="payCompany">收款人</param>
        /// <param name="reportPersonnelId">申报人Id </param>
        /// <param name="memo">备注（模糊）</param>
        /// <returns></returns>
        public IList<CostReportInfo> GetCostReportInfoList(string reportNo, string reportName, string payCompany, Guid reportPersonnelId, string memo)
        {
            var sb = new StringBuilder();
            sb.AppendLine(SQL_GET_REPORT);
            sb.Append(" WHERE 1=1");
            if (!string.IsNullOrWhiteSpace(reportNo))
            {
                sb.Append(" AND ReportNo='").Append(reportNo).Append("'");
            }
            if (!string.IsNullOrWhiteSpace(reportName))
            {
                sb.Append(" AND ReportName LIKE '%").Append(reportName).Append("%'");
            }
            if (!string.IsNullOrWhiteSpace(payCompany))
            {
                sb.Append(" AND PayCompany='").Append(payCompany).Append("'");
            }
            if (reportPersonnelId != Guid.Empty)
            {
                sb.Append(" AND ReportPersonnelId='").Append(reportPersonnelId).Append("'");
            }
            if (!string.IsNullOrWhiteSpace(memo))
            {
                sb.Append(" AND Memo LIKE '%").Append(memo).Append("%'");
            }
            sb.Append(" ORDER BY ReportDate DESC ");
            IList<CostReportInfo> list = new List<CostReportInfo>();
            using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, sb.ToString(), null))
            {
                while (dr.Read())
                {
                    var info = new CostReportInfo
                    {
                        ReportId = GetGuid(dr, "ReportId"),
                        ReportNo = GetString(dr, "ReportNo"),
                        CompanyClassId = GetGuid(dr, "CompanyClassId"),
                        CompanyId = GetGuid(dr, "CompanyId"),
                        ReportName = GetString(dr, "ReportName"),
                        ReportCost = GetDecimal(dr, "ReportCost"),
                        PayCost = GetDecimal(dr, "PayCost"),
                        PayCompany = GetString(dr, "PayCompany"),
                        BankAccountName = GetString(dr, "BankAccountName"),
                        BankAccount = GetString(dr, "BankAccount"),
                        ReportMemo = GetString(dr, "ReportMemo"),
                        CostType = GetInt(dr, "CostType"),
                        ReportKind = GetInt(dr, "ReportKind"),
                        InvoiceType = GetInt(dr, "InvoiceType"),
                        InvoiceId = GetGuid(dr, "InvoiceId"),
                        State = GetInt(dr, "State"),
                        AuditingMemo = GetString(dr, "AuditingMemo"),
                        Memo = GetString(dr, "Memo"),
                        ReportPersonnelId = GetGuid(dr, "ReportPersonnelId"),
                        ReportDate = GetDateTime(dr, "ReportDate"),
                        AuditingMan = GetGuid(dr, "AuditingMan"),
                        RealityCost = GetDecimal(dr, "RealityCost"),
                        PayBankAccountId = GetGuid(dr, "PayBankAccountID"),
                        ReportFilialeId = GetGuid(dr, "ReportFilialeID"),
                        ReportBranchId = GetGuid(dr, "ReportBranchID"),
                        Mobile = GetString(dr, "Mobile"),
                        RefundmentKind = GetInt(dr, "RefundmentKind"),
                        RefundmentBankAccountId = GetGuid(dr, "RefundmentBankAccountID"),
                        ExecuteDate = GetDateTime(dr, "ExecuteDate"),
                        Poundage = GetDecimal(dr, "Poundage"),
                        ExecuteBankId = GetGuid(dr, "ExecuteBankId"),
                        AuditingDate = GetDateTime(dr, "AuditingDate"),
                        FinishDate = GetDateTime(dr, "FinishDate"),
                        AssumeFilialeId = GetGuid(dr, "AssumeFilialeId"),
                        AssumeBranchId = GetGuid(dr, "AssumeBranchId"),
                        AssumeGroupId = GetGuid(dr, "AssumeGroupId"),
                        AuditingBeforePay = GetInt(dr, "AuditingBeforePay"),
                        UrgentOrDefer = GetInt(dr, "UrgentOrDefer"),
                        UrgentReason = GetString(dr, "UrgentReason"),
                        TradeNo = GetString(dr, "TradeNo"),
                        ApplyForCost = GetDecimal(dr, "ApplyForCost"),
                        ActualAmount = GetDecimal(dr, "ActualAmount"),
                        InvoiceNo = GetString(dr, "InvoiceNo"),
                        InvoiceTitle = GetString(dr, "InvoiceTitle"),
                        InvoiceTitleFilialeId = GetGuid(dr, "InvoiceTitleFilialeId"),
                        ReceiptNo = GetString(dr, "ReceiptNo"),
                        AssumeShopId = GetGuid(dr, "AssumeShopId"),
                        BankName = GetString(dr, "BankName")
                    };
                    list.Add(info);
                }
            }
            return list;
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public IList<CostReportInfo> GetReportCostData(string sql, params SqlParameter[] parameters)
        {
            IList<CostReportInfo> list = new List<CostReportInfo>();
            using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, sql, parameters))
            {
                while (dr.Read())
                {
                    var info = new CostReportInfo
                    {
                        ReportId = GetGuid(dr, "ReportId"),
                        ReportNo = GetString(dr, "ReportNo"),
                        CompanyClassId = GetGuid(dr, "CompanyClassId"),
                        CompanyId = GetGuid(dr, "CompanyId"),
                        ReportName = GetString(dr, "ReportName"),
                        ReportCost = GetDecimal(dr, "ReportCost"),
                        PayCost = GetDecimal(dr, "PayCost"),
                        PayCompany = GetString(dr, "PayCompany"),
                        BankAccountName = GetString(dr, "BankAccountName"),
                        BankAccount = GetString(dr, "BankAccount"),
                        ReportMemo = GetString(dr, "ReportMemo"),
                        CostType = GetInt(dr, "CostType"),
                        ReportKind = GetInt(dr, "ReportKind"),
                        InvoiceType = GetInt(dr, "InvoiceType"),
                        InvoiceId = GetGuid(dr, "InvoiceId"),
                        State = GetInt(dr, "State"),
                        AuditingMemo = GetString(dr, "AuditingMemo"),
                        Memo = GetString(dr, "Memo"),
                        ReportPersonnelId = GetGuid(dr, "ReportPersonnelId"),
                        ReportDate = GetDateTime(dr, "ReportDate"),
                        AuditingMan = GetGuid(dr, "AuditingMan"),
                        RealityCost = GetDecimal(dr, "RealityCost"),
                        PayBankAccountId = GetGuid(dr, "PayBankAccountID"),
                        ReportFilialeId = GetGuid(dr, "ReportFilialeID"),
                        ReportBranchId = GetGuid(dr, "ReportBranchID"),
                        Mobile = GetString(dr, "Mobile"),
                        RefundmentKind = GetInt(dr, "RefundmentKind"),
                        RefundmentBankAccountId = GetGuid(dr, "RefundmentBankAccountID"),
                        ExecuteDate = GetDateTime(dr, "ExecuteDate"),
                        Poundage = GetDecimal(dr, "Poundage"),
                        ExecuteBankId = GetGuid(dr, "ExecuteBankId"),
                        AuditingDate = GetDateTime(dr, "AuditingDate"),
                        FinishDate = GetDateTime(dr, "FinishDate"),
                        AssumeFilialeId = GetGuid(dr, "AssumeFilialeId"),
                        AssumeBranchId = GetGuid(dr, "AssumeBranchId"),
                        AssumeGroupId = GetGuid(dr, "AssumeGroupId"),
                        AuditingBeforePay = GetInt(dr, "AuditingBeforePay"),
                        UrgentOrDefer = GetInt(dr, "UrgentOrDefer"),
                        UrgentReason = GetString(dr, "UrgentReason"),
                        TradeNo = GetString(dr, "TradeNo"),
                        ApplyForCost = GetDecimal(dr, "ApplyForCost"),
                        ActualAmount = GetDecimal(dr, "ActualAmount"),
                        InvoiceNo = GetString(dr, "InvoiceNo"),
                        InvoiceTitle = GetString(dr, "InvoiceTitle"),
                        InvoiceTitleFilialeId = GetGuid(dr, "InvoiceTitleFilialeId"),
                        ReceiptNo = GetString(dr, "ReceiptNo"),
                        AssumeShopId = GetGuid(dr, "AssumeShopId"),
                        BankName = GetString(dr, "BankName"),
                        StartTime = GetDateTime(dr, "StartTime"),
                        EndTime = GetDateTime(dr, "EndTime"),
                        CostsVarieties = GetInt(dr, "CostsVarieties"),
                        Deposit = GetInt(dr, "Deposit"),
                        DepositNo = GetString(dr, "DepositNo"),
                        GoodsCode = GetString(dr, "GoodsCode"),
                        IsLastTime = GetBoolean(dr, "IsLastTime"),
                        IsSystem = GetBoolean(dr, "IsSystem"),
                        ApplyNumber = GetInt(dr, "ApplyNumber"),
                        IsEnd = GetBoolean(dr, "IsEnd")
                    };
                    list.Add(info);
                }
            }
            return list;
        }

        /// <summary>
        /// 预借款预警-数据查询
        /// </summary>
        /// <param name="reportKind"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="reportPersonnleId"></param>
        /// <param name="reportState"></param>
        /// <param name="reportNo"></param>
        /// <param name="reportName"></param>
        /// <param name="type">0：完成时间；1：申报时间</param>
        /// <param name="billState">票据状态(0:未提交;1:已提交;2:已接收;3:已认证;4:已核销)</param>
        /// <returns></returns>
        public IList<CostReportInfo> GetCostReportForWarningsInfos(int reportKind, DateTime? startTime, DateTime? endTime, Guid reportPersonnleId, string reportState, string reportNo, string reportName, string type, string billState)
        {
            var builder = new StringBuilder(@"SELECT ReportId,ReportNo ,cr.CompanyClassId ,ccc.CompanyClassName AS ReportClassName ,ReportName,ReportCost,PayCost
    ,PayCompany,BankAccountName,BankAccount,ReportMemo,CostType,ReportKind,InvoiceType,InvoiceId,[State] ,AuditingMemo,Memo,ReportPersonnelId
    ,ReportDate,CompanyId,AuditingMan,RealityCost,PayBankAccountID,ReportFilialeID,ReportBranchID,Mobile,RefundmentKind
    ,RefundmentBankAccountID,ExecuteDate,Poundage,ExecuteBankId,AuditingDate,FinishDate,AssumeFilialeId,AssumeBranchId,AssumeGroupId,AuditingBeforePay
    ,UrgentOrDefer,UrgentReason,TradeNo,ApplyForCost,ActualAmount,InvoiceNo,InvoiceTitle,InvoiceTitleFilialeId,ReceiptNo,AssumeShopId,StartTime,EndTime,CostsVarieties,ReviewState    
    FROM lmShop_CostReport cr INNER JOIN lmShop_CostCompanyClass ccc ON ccc.CompanyClassId = cr.CompanyClassId
    WHERE 1=1 ");

            if (!string.IsNullOrEmpty(billState))
            {
                builder.AppendFormat(@" AND (select COUNT(0) FROM lmShop_CostReportBill crb WHERE cr.ReportId=crb.ReportId AND crb.BillState='{0}')>0", billState);
            }

            if (!string.IsNullOrEmpty(type))
            {
                if (type.Equals("1"))
                {
                    if (startTime == null && endTime == null)
                    {
                        builder.AppendFormat(" AND ReportDate >= '{0}' ", DateTime.Now.ToString("yyyy-MM-01"));
                    }
                    else
                    {
                        if (startTime != null && startTime != DateTime.MinValue)
                        {
                            builder.AppendFormat(" AND ReportDate >= '{0}' ", startTime);
                        }
                        if (endTime != null && endTime != DateTime.MinValue)
                        {
                            builder.AppendFormat(" AND ReportDate < '{0}' ", endTime);
                        }
                    }
                }
                else if (type.Equals("0"))
                {
                    if (startTime != null && startTime != DateTime.MinValue)
                    {
                        builder.AppendFormat(" AND FinishDate >= '{0}' ", startTime);
                    }
                    if (endTime != null && endTime != DateTime.MinValue)
                    {
                        builder.AppendFormat(" AND FinishDate < '{0}' ", endTime);
                    }
                }
            }

            if (reportKind != -1)
            {
                builder.AppendFormat(" AND ReportKind={0} ", reportKind);
            }
            if (!string.IsNullOrEmpty(reportState))
            {
                builder.AppendFormat(" AND cr.[State] IN ({0})", reportState);
            }
            if (reportPersonnleId != Guid.Empty)
            {
                builder.AppendFormat(" AND ReportPersonnelId='{0}'", reportPersonnleId);
            }
            if (!string.IsNullOrEmpty(reportNo))
            {
                builder.AppendFormat(" AND ReportNo LIKE '{0}%' ", reportNo);
            }
            if (!string.IsNullOrEmpty(reportName))
            {
                builder.AppendFormat(" AND ReportName LIKE '%{0}%'", reportName);
            }
            builder.Append(" ORDER BY FinishDate,ReportDate DESC ");

            IList<CostReportInfo> list = new List<CostReportInfo>();
            using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, builder.ToString()))
            {
                while (dr.Read())
                {
                    var info = new CostReportInfo
                    {
                        ReportId = GetGuid(dr, "ReportId"),
                        ReportNo = GetString(dr, "ReportNo"),
                        CompanyClassId = GetGuid(dr, "CompanyClassId"),
                        CompanyId = GetGuid(dr, "CompanyId"),
                        ReportName = GetString(dr, "ReportName"),
                        ReportCost = GetDecimal(dr, "ReportCost"),
                        PayCost = GetDecimal(dr, "PayCost"),
                        PayCompany = GetString(dr, "PayCompany"),
                        BankAccountName = GetString(dr, "BankAccountName"),
                        BankAccount = GetString(dr, "BankAccount"),
                        ReportMemo = GetString(dr, "ReportMemo"),
                        CostType = GetInt(dr, "CostType"),
                        ReportKind = GetInt(dr, "ReportKind"),
                        InvoiceType = GetInt(dr, "InvoiceType"),
                        InvoiceId = GetGuid(dr, "InvoiceId"),
                        State = GetInt(dr, "State"),
                        AuditingMemo = GetString(dr, "AuditingMemo"),
                        Memo = GetString(dr, "Memo"),
                        ReportPersonnelId = GetGuid(dr, "ReportPersonnelId"),
                        ReportDate = GetDateTime(dr, "ReportDate"),
                        AuditingMan = GetGuid(dr, "AuditingMan"),
                        RealityCost = GetDecimal(dr, "RealityCost"),
                        PayBankAccountId = GetGuid(dr, "PayBankAccountID"),
                        ReportFilialeId = GetGuid(dr, "ReportFilialeID"),
                        ReportBranchId = GetGuid(dr, "ReportBranchID"),
                        Mobile = GetString(dr, "Mobile"),
                        RefundmentKind = GetInt(dr, "RefundmentKind"),
                        RefundmentBankAccountId = GetGuid(dr, "RefundmentBankAccountID"),
                        ExecuteDate = GetDateTime(dr, "ExecuteDate"),
                        Poundage = GetDecimal(dr, "Poundage"),
                        ExecuteBankId = GetGuid(dr, "ExecuteBankId"),
                        AuditingDate = GetDateTime(dr, "AuditingDate"),
                        FinishDate = GetDateTime(dr, "FinishDate"),
                        AssumeFilialeId = GetGuid(dr, "AssumeFilialeId"),
                        AssumeBranchId = GetGuid(dr, "AssumeBranchId"),
                        AssumeGroupId = GetGuid(dr, "AssumeGroupId"),
                        AuditingBeforePay = GetInt(dr, "AuditingBeforePay"),
                        UrgentOrDefer = GetInt(dr, "UrgentOrDefer"),
                        UrgentReason = GetString(dr, "UrgentReason"),
                        TradeNo = GetString(dr, "TradeNo"),
                        ApplyForCost = GetDecimal(dr, "ApplyForCost"),
                        ActualAmount = GetDecimal(dr, "ActualAmount"),
                        InvoiceNo = GetString(dr, "InvoiceNo"),
                        InvoiceTitle = GetString(dr, "InvoiceTitle"),
                        InvoiceTitleFilialeId = GetGuid(dr, "InvoiceTitleFilialeId"),
                        ReceiptNo = GetString(dr, "ReceiptNo"),
                        AssumeShopId = GetGuid(dr, "AssumeShopId"),
                        StartTime = GetDateTime(dr, "StartTime"),
                        EndTime = GetDateTime(dr, "EndTime"),
                        CostsVarieties = GetInt(dr, "CostsVarieties"),
                        ReviewState = GetInt(dr, "ReviewState")
                    };
                    list.Add(info);
                }
            }
            return list;
        }

        /// <summary>
        /// 获取预借款预警数
        /// </summary>
        /// <param name="reportKind"></param>
        /// <param name="dayTime"></param>
        /// <param name="reportPersonnleId"></param>
        /// <param name="reportStates"></param>
        /// <returns></returns>
        public int GetPreloanWarningsCount(int reportKind, DateTime dayTime, Guid reportPersonnleId, string reportStates)
        {
            string personnelWhere = reportPersonnleId != Guid.Empty
                ? String.Format(" AND ReportPersonnelId='{0}'", reportPersonnleId)
                : "";
            string sql = string.Format(@"SELECT COUNT(ReportId) FROM lmShop_CostReport WHERE ReportDate <= @DayTime 
              AND ReportKind='{0}' {1} AND [State] NOT IN ({2}) ",
           reportKind, personnelWhere, reportStates);
            using (var db = DatabaseFactory.Create())
            {
                return db.GetValue<int>(true, sql, new Parameter("DayTime", dayTime));
            }
        }

        /// <summary>
        /// 根据reportIdList统计有票据的申请单总数、发票总数、收据总数
        /// </summary>
        /// <param name="reportIdList"></param>
        /// <returns></returns>
        /// zal 2016-08-18
        public ArrayList GetSumInvoiceAccept(string[] reportIdList)
        {
            ArrayList arrayList = new ArrayList();
            if (!reportIdList.Any())
            {
                return arrayList;
            }
            var reportIdStr = "'" + string.Join("','", reportIdList) + "'";
            string sql = @"
            select COUNT(distinct a.ReportId) as 'SumReport',
            IsNull(SUM(
            case a.InvoiceType
            when 1 then 1 
            when 5 then 1
            else 0
            end),0) as 'SumInvoice',
            IsNull(SUM(case when a.InvoiceType=2 then 1 else 0 end),0) as 'SumReceipt'
            from lmShop_CostReport a with(nolock)
            left join lmShop_CostReportBill b with(nolock) on a.ReportId=b.ReportId and b.IsPass=0
            where a.ReportId in(" + reportIdStr + ")";

            using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, sql, null))
            {
                while (dr.Read())
                {
                    arrayList.Add(GetString(dr, "SumReport"));
                    arrayList.Add(GetString(dr, "SumInvoice"));
                    arrayList.Add(GetString(dr, "SumReceipt"));
                }
            }
            return arrayList;
        }

        /// <summary>
        /// 根据reportIdList统计申请单总数、申报金额、已付款金额
        /// </summary>
        /// <param name="reportIdList"></param>
        /// <returns></returns>
        /// zal 2016-08-18
        public ArrayList GetSumReport(string[] reportIdList)
        {
            ArrayList arrayList = new ArrayList();
            if (!reportIdList.Any())
            {
                return arrayList;
            }
            var reportIdStr = "'" + string.Join("','", reportIdList.ToArray()) + "'";
            string sql = @"
            select COUNT(distinct ReportId) as 'SumReport',
            IsNull(SUM(IsNull(ReportCost,0)),0) as 'SumReportCost',
            IsNull(SUM(case when ReportKind!=4 and IsNull(RealityCost,0)>0 then IsNull(RealityCost,0) end),0) as 'SumPayCost',
            IsNull(SUM(case when ReportKind=4 or IsNull(RealityCost,0)<0 then IsNull(ABS(RealityCost),0) end),0) as 'SumReceiveCost'
            from lmShop_CostReport with(nolock)
            where ReportId in(" + reportIdStr + ")";

            using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, sql, null))
            {
                while (dr.Read())
                {
                    arrayList.Add(GetString(dr, "SumReport"));
                    arrayList.Add(GetString(dr, "SumReportCost"));
                    arrayList.Add(GetString(dr, "SumPayCost"));
                    arrayList.Add(GetString(dr, "SumReceiveCost"));
                }
            }
            return arrayList;
        }

        /// <summary>
        /// 获取同一收款单位同一预估申报金额或者同一提交人同一预估申报金额的数据
        /// </summary>
        /// <param name="payCompany"></param>
        /// <param name="reportPersonnelId"></param>
        /// <param name="reportCost"></param>
        /// <param name="reportId"></param>
        /// <returns></returns>
        /// zal 2016-11-24
        public ArrayList[] ReportNoArrayList(string payCompany, Guid reportPersonnelId, decimal reportCost, Guid reportId)
        {
            ArrayList[] arrayList = new ArrayList[2];
            ArrayList arrayPayCompany = new ArrayList();
            ArrayList arrayPersonnel = new ArrayList();
            string sql = @"
            SELECT 
            CASE WHEN PayCompany=@PayCompany and ReportCost=@ReportCost THEN ReportNo END AS 'PayCompanyReportNo',
            CASE WHEN ReportPersonnelId=@ReportPersonnelId and ReportCost=@ReportCost THEN ReportNo END AS 'PersonnelReportNo' 
            FROM lmShop_CostReport
            WHERE STATE!=13 AND ((PayCompany=@PayCompany and ReportCost=@ReportCost) or (ReportPersonnelId=@ReportPersonnelId and ReportCost=@ReportCost))
            AND ReportId!=@ReportId AND((STATE=7 AND FinishDate>=CONVERT(VARCHAR(10),DateAdd(Month,-3,GETDATE()),120)) OR (STATE!=7 AND ReportDate>=CONVERT(VARCHAR(10),DateAdd(Month,-3,GETDATE()),120)))";

            var paras = new[]
            {
                new SqlParameter("@PayCompany",payCompany), 
                new SqlParameter("@ReportPersonnelId",reportPersonnelId), 
                new SqlParameter("@ReportCost",reportCost),
                new SqlParameter("@ReportId",reportId)
            };
            using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, sql, paras))
            {
                while (dr.Read())
                {
                    if (dr["PayCompanyReportNo"] != DBNull.Value)
                    {
                        arrayPayCompany.Add(GetString(dr, "PayCompanyReportNo"));
                    }
                    if (dr["PersonnelReportNo"] != DBNull.Value)
                    {
                        arrayPersonnel.Add(GetString(dr, "PersonnelReportNo"));
                    }
                }
                if (arrayPayCompany.Count > 0)
                {
                    arrayList[0] = arrayPayCompany;
                }
                if (arrayPersonnel.Count > 0)
                {
                    arrayList[1] = arrayPersonnel;
                }
            }
            return arrayList;
        }
    }
}
