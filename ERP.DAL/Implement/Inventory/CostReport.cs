using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using ERP.DAL.Interface.IInventory;
using ERP.Environment;
using Keede.DAL.Helper;
using Keede.Ecsoft.Model;

namespace ERP.DAL.Implement.Inventory
{
    /// <summary>
    /// 费用审批申报操作类
    /// </summary>
    public partial class CostReport : ICostReport
    {
        public CostReport(GlobalConfig.DB.FromType fromType)
        {

        }

        #region --> Basis

        private static Guid GetGuid(IDataReader dr, string column)
        {
            return dr[column] == DBNull.Value ? Guid.Empty : new Guid(dr[column].ToString());
        }
        private static string GetString(IDataReader dr, string column)
        {
            return dr[column] == DBNull.Value ? string.Empty : dr[column].ToString();
        }
        private bool GetBoolean(IDataReader dr, string column)
        {
            return dr[column] != DBNull.Value && bool.Parse(dr[column].ToString());
        }
        private static decimal GetDecimal(IDataReader dr, string column)
        {
            return dr[column] == DBNull.Value ? 0 : decimal.Parse(dr[column].ToString());
        }
        private static int GetInt(IDataReader dr, string column)
        {
            return dr[column] == DBNull.Value ? 0 : int.Parse(dr[column].ToString());
        }
        private static DateTime GetDateTime(IDataReader dr, string column)
        {
            return dr[column] == DBNull.Value ? DateTime.Parse("1900-01-01") : DateTime.Parse(dr[column].ToString());
        }

        #endregion

        #region[SQL语句]

        /// <summary>
        /// 获取申报信息
        /// </summary>
        private const string SQL_GET_REPORT = @"
SELECT 
    ReportId,ReportNo,CompanyClassId,ReportName,ReportCost,PayCost,PayCompany,BankAccountName,BankAccount,ReportMemo,
    CostType,ReportKind,InvoiceType,InvoiceId,State,AuditingMemo,Memo,ReportPersonnelId,ReportDate,CompanyId,
    AuditingMan,RealityCost,PayBankAccountID,ReportFilialeID,ReportBranchID,Mobile,RefundmentKind,RefundmentBankAccountID,ExecuteDate,Poundage,ExecuteBankId,
    AuditingDate,FinishDate,AssumeFilialeId,AssumeBranchId,AssumeGroupId,AuditingBeforePay,UrgentOrDefer,UrgentReason,TradeNo,ApplyForCost,
    ActualAmount,InvoiceNo,InvoiceTitle,InvoiceTitleFilialeId,ReceiptNo,AssumeShopId,BA.BankName,CR.StartTime,CR.EndTime,CR.CostsVarieties,CR.IsOut,CR.Deposit,CR.DepositNo,CR.GoodsCode,CR.AcceptDate,CR.ReviewState,CR.IsLastTime,CR.IsSystem,CR.ApplyNumber,CR.IsEnd  
FROM lmShop_CostReport CR
LEFT JOIN lmShop_BankAccounts BA ON CR.PayBankAccountID=BA.BankAccountsId ";

        /// <summary>
        /// 获取申报信息(押金回收使用)
        /// </summary>
        private const string SQL_GET_REPORT_DEPOSITRECOVERY = @"
SELECT 
    distinct CR.ReportId,ReportNo,CompanyClassId,ReportName,ReportCost,PayCost,PayCompany,BankAccountName,BankAccount,ReportMemo,
    CostType,ReportKind,InvoiceType,InvoiceId,State,AuditingMemo,Memo,ReportPersonnelId,ReportDate,CompanyId,
    AuditingMan,RealityCost,PayBankAccountID,ReportFilialeID,ReportBranchID,Mobile,RefundmentKind,RefundmentBankAccountID,ExecuteDate,Poundage,ExecuteBankId,
    AuditingDate,FinishDate,AssumeFilialeId,AssumeBranchId,AssumeGroupId,AuditingBeforePay,UrgentOrDefer,UrgentReason,TradeNo,ApplyForCost,
    ActualAmount,InvoiceNo,InvoiceTitle,InvoiceTitleFilialeId,ReceiptNo,AssumeShopId,BA.BankName,CR.StartTime,CR.EndTime,CR.CostsVarieties,CR.IsOut,CR.Deposit,CR.DepositNo,CR.GoodsCode,CR.AcceptDate,CR.IsLastTime,CR.IsSystem,CR.ApplyNumber,CR.IsEnd 
FROM lmShop_CostReport CR
LEFT JOIN lmShop_BankAccounts BA ON CR.PayBankAccountID=BA.BankAccountsId ";

        /// <summary>
        /// 修改申报状态审批人
        /// </summary>
        private const string SQL_UPDATE_REPORT_STATE_AUDITING_MAN =
            @"UPDATE lmShop_CostReport SET State=@State,AuditingMemo=@AuditingMemo,Memo=Memo+@Memo,AuditingMan=@AuditingMan WHERE ReportId=@ReportId";
        /// <summary>
        /// 修改申报状态
        /// </summary>
        private const string SQL_UPDATE_REPORT_STATE =
            @"UPDATE lmShop_CostReport SET State=@State,AuditingMemo=@AuditingMemo,Memo=Memo+@Memo WHERE ReportId=@ReportId";
        /// <summary>
        /// 修改申报状态
        /// </summary>
        private const string SQL_UPDATE_REPORT_STATE_MEMO =
            @"UPDATE lmShop_CostReport SET State=@State,Memo=Memo+@Memo WHERE ReportId=@ReportId";
        /// <summary>
        /// 修改申报已支付金额、状态、备注
        /// </summary>
        private const string SQL_UPDATE_REPORT_PAY_COST_STATE_AND_MEMO =
            @"UPDATE lmShop_CostReport SET PayCost=PayCost+@PayCost,State=@State,Memo=Memo+@Memo WHERE ReportId=@ReportId";
        /// <summary>
        /// 修改申报金额、已支付金额、状态、备注
        /// </summary>
        private const string SQL_UPDATE_REPORT_REPORT_COST_PAY_COST_STATE_AND_MEMO =
            @"UPDATE lmShop_CostReport SET ReportCost=@ReportCost,PayCost=PayCost+@PayCost,State=@State,
            Memo=Memo+@Memo,ExecuteDate=@ExecuteDate WHERE ReportId=@ReportId";
        /// <summary>
        /// 修改申报状态、备注、付款类型银行账号
        /// </summary>
        private const string SQL_UPDATE_REPORT_STATE_AND_MEMO =
            @"UPDATE lmShop_CostReport SET State=@State,BankAccountName=@BankAccountName,BankAccount=@BankAccount,
            Memo=Memo+@Memo,CompanyId=@CompanyId,AuditingMan=@AuditingMan,PayBankAccountID=@PayBankAccountID,CompanyClassId=@CompanyClassId WHERE ReportId=@ReportId";

        /// <summary>
        /// 修改申报实际金额
        /// </summary>
        private const string SQL_UPDATE_REPORT_REALITY_COST =
            @"UPDATE lmShop_CostReport SET RealityCost=@RealityCost WHERE ReportId=@ReportId";

        /// <summary>
        /// 修改手续费
        /// </summary>
        private const string SQL_UPDATE_REPORT_POUNDAGE = @"UPDATE lmShop_CostReport SET Poundage=@Poundage WHERE ReportId=@ReportId";
        /// <summary>
        /// 修改本公司打款帐号
        /// </summary>
        private const string SQL_UPDATE_REPORT_EXECUTEBANKID = @"UPDATE lmShop_CostReport SET PayBankAccountID=@ExecuteBankId,ExecuteBankID=@ExecuteBankId WHERE ReportId=@ReportId";

        #endregion

        #region[参数]
        /// <summary>
        /// 申报ID
        /// </summary>
        private const string PARM_REPORT_ID = "@ReportId";
        ///// <summary>
        ///// 申报单号
        ///// </summary>
        //private const string PARM_REPORT_NO = "@ReportNo";
        /// <summary>
        /// 费用分类
        /// </summary>
        private const string PARM_COMPANY_CLASS_ID = "@CompanyClassId";
        /// <summary>
        /// 费用名称
        /// </summary>
        private const string PARM_REPORT_NAME = "@ReportName";
        /// <summary>
        /// 申报金额
        /// </summary>
        private const string PARM_REPORT_COST = "@ReportCost";
        /// <summary>
        /// 以打款金额
        /// </summary>
        private const string PARM_PAY_COST = "@PayCost";
        /// <summary>
        /// 付款单位
        /// </summary>
        private const string PARM_PAY_COMPANY = "@PayCompany";
        /// <summary>
        /// 账户名
        /// </summary>
        private const string PARM_BANK_ACCOUNT_NAME = "@BankAccountName";
        /// <summary>
        /// 账号
        /// </summary>
        private const string PARM_BANK_ACCOUNT = "@BankAccount";
        /// <summary>
        /// 申报说明
        /// </summary>
        private const string PARM_REPORT_MEMO = "@ReportMemo";
        /// <summary>
        /// 费用类别
        /// </summary>
        private const string PARM_COST_TYPE = "@CostType";
        /// <summary>
        /// 申报类型
        /// </summary>
        private const string PARM_REPORT_KIND = "@ReportKind";
        /// <summary>
        /// 票据类型
        /// </summary>
        private const string PARM_INVOICE_TYPE = "@InvoiceType";
        /// <summary>
        /// 票据ID
        /// </summary>
        private const string PARM_INVOICE_ID = "@InvoiceId";
        /// <summary>
        /// 状态
        /// </summary>
        private const string PARM_STATE = "@State";
        /// <summary>
        /// 审核备注
        /// </summary>
        private const string PARM_AUDITING_MEMO = "@AuditingMemo";
        /// <summary>
        /// 备注
        /// </summary>
        private const string PARM_MEMO = "@Memo";
        /// <summary>
        /// 申报人ID
        /// </summary>
        private const string PARM_REPORT_PERSONNEL_ID = "@ReportPersonnelId";
        /// <summary>
        /// 申报日期
        /// </summary>
        private const string PARM_REPORT_DATE = "@ReportDate";
        /// <summary>
        /// 打款分类
        /// </summary>
        private const string PARM_COMPANY_ID = "@CompanyId";
        /// <summary>
        /// 审批人
        /// </summary>
        private const string PARM_AUDITING_MAN = "@AuditingMan";
        /// <summary>
        /// 实际金额
        /// </summary>
        private const string PARM_REALITY_COST = "@RealityCost";
        /// <summary>
        /// 打款账号ID
        /// </summary>
        private const string PARM_PAY_BANK_ACCOUNT_ID = "@PayBankAccountID";

        /// <summary>
        /// 申报人手机号码
        /// </summary>
        private const string PARM_MOBILE = "@Mobile";

        /// <summary>
        /// 打款时间
        /// </summary>
        private const string PARM_EXECUTEDATE = "@ExecuteDate";
        /// <summary>
        /// 手续费
        /// </summary>
        private const string PARM_POUNDAGE = "@Poundage";
        /// <summary>
        /// 本公司打款帐号
        /// </summary>
        private const string PARM_EXECUTEBANKID = "@ExecuteBankId";
        #endregion

        #region[添加申报]
        /// <summary>
        /// 添加申报
        /// </summary>
        /// <param name="info">申报详细模型</param>
        /// <param name="errorMessage"></param>
        public bool InsertReport(CostReportInfo info, out string errorMessage)
        {
            errorMessage = string.Empty;
            const string SQL = @"
INSERT INTO lmshop_CostReport(
    ReportId,ReportNo,CompanyClassId,ReportName,ReportCost,PayCost,PayCompany,BankAccountName,BankAccount,ReportMemo,
    CostType,ReportKind,InvoiceType,InvoiceId,State,AuditingMemo,Memo,ReportPersonnelId,ReportDate,ReportFilialeID,
    ReportBranchID,Mobile,AssumeFilialeId,AssumeBranchId,AssumeGroupId,UrgentOrDefer,UrgentReason,ApplyForCost,ActualAmount,CompanyId,
    InvoiceNo,InvoiceTitle,InvoiceTitleFilialeId,ReceiptNo,PayBankAccountID,RefundmentKind,AssumeShopId,StartTime,EndTime,CostsVarieties,RealityCost,IsOut,Deposit,DepositNo,GoodsCode,AcceptDate,ReviewState,IsLastTime,IsSystem,ApplyNumber,IsEnd
) 
VALUES(
    @ReportId,@ReportNo,@CompanyClassId,@ReportName,@ReportCost,@PayCost,@PayCompany,@BankAccountName,@BankAccount,@ReportMemo,
    @CostType,@ReportKind,@InvoiceType,@InvoiceId,@State,@AuditingMemo,@Memo,@ReportPersonnelId,@ReportDate,@ReportFilialeID,
    @ReportBranchID,@Mobile,@AssumeFilialeId,@AssumeBranchId,@AssumeGroupId,@UrgentOrDefer,@UrgentReason,@ApplyForCost,@ActualAmount,@CompanyId,
    @InvoiceNo,@InvoiceTitle,@InvoiceTitleFilialeId,@ReceiptNo,@PayBankAccountID,@RefundmentKind,@AssumeShopId,@StartTime,@EndTime,@CostsVarieties,@RealityCost,@IsOut,@Deposit,@DepositNo,@GoodsCode,@AcceptDate,@ReviewState,@IsLastTime,@IsSystem,@ApplyNumber,@IsEnd
)";

            #region 参数赋值
            var parms = new[]{
                new SqlParameter("@ReportId",SqlDbType.UniqueIdentifier), // 0
                new SqlParameter("@ReportNo",SqlDbType.VarChar,24), // 1
                new SqlParameter("@CompanyClassId",SqlDbType.UniqueIdentifier), // 2
                new SqlParameter("@ReportName",SqlDbType.VarChar,50), // 3
                new SqlParameter("@ReportCost",SqlDbType.Decimal), // 4
                new SqlParameter("@PayCost",SqlDbType.Decimal), // 5
                new SqlParameter("@PayCompany",SqlDbType.VarChar,128), // 6
                new SqlParameter("@BankAccountName",SqlDbType.VarChar,128), // 7
                new SqlParameter("@BankAccount",SqlDbType.VarChar,128), // 8
                new SqlParameter("@ReportMemo",SqlDbType.VarChar), // 9
                new SqlParameter("@CostType",SqlDbType.Int), // 10
                new SqlParameter("@ReportKind",SqlDbType.Int), // 11
                new SqlParameter("@InvoiceType",SqlDbType.Int), // 12
                new SqlParameter("@InvoiceId",SqlDbType.UniqueIdentifier), // 13
                new SqlParameter("@State",SqlDbType.Int), // 14
                new SqlParameter("@AuditingMemo",SqlDbType.VarChar), // 15
                new SqlParameter("@Memo",SqlDbType.VarChar), // 16
                new SqlParameter("@ReportPersonnelId",SqlDbType.UniqueIdentifier), // 17
                new SqlParameter("@ReportDate",SqlDbType.DateTime), // 18
                new SqlParameter("@ReportFilialeID",SqlDbType.UniqueIdentifier), // 19
                new SqlParameter("@ReportBranchID",SqlDbType.UniqueIdentifier), // 20
                new SqlParameter("@Mobile",SqlDbType.VarChar,24), // 21
                new SqlParameter("@AssumeFilialeId",info.AssumeFilialeId), // 22
                new SqlParameter("@AssumeBranchId",info.AssumeBranchId), // 23
                new SqlParameter("@AssumeGroupId",info.AssumeGroupId), // 24
                new SqlParameter("@UrgentOrDefer",info.UrgentOrDefer), // 25
                new SqlParameter("@UrgentReason",info.UrgentReason), // 26
                new SqlParameter("@ApplyForCost",info.ApplyForCost), // 27
                new SqlParameter("@ActualAmount",info.ActualAmount), // 28
                new SqlParameter("@CompanyId",info.CompanyId), // 29
                new SqlParameter("@InvoiceNo",info.InvoiceNo), // 30
                new SqlParameter("@InvoiceTitle",info.InvoiceTitle), // 31 
                new SqlParameter("@InvoiceTitleFilialeId",info.InvoiceTitleFilialeId),
                new SqlParameter("@ReceiptNo",info.ReceiptNo), // 32
                new SqlParameter("@PayBankAccountID",info.PayBankAccountId), // 32,
                new SqlParameter("@RefundmentKind",info.RefundmentKind),
                new SqlParameter("@AssumeShopId",info.AssumeShopId),
                new SqlParameter("@StartTime",info.StartTime), 
                new SqlParameter("@EndTime",info.EndTime),
                new SqlParameter("@CostsVarieties",info.CostsVarieties),
                new SqlParameter("@RealityCost",info.RealityCost),
                new SqlParameter("@IsOut",info.IsOut),
                new SqlParameter("@Deposit",info.Deposit),
                new SqlParameter("@DepositNo",info.DepositNo),
                new SqlParameter("@GoodsCode",info.GoodsCode),
                new SqlParameter("@AcceptDate",info.AcceptDate),
                new SqlParameter("@ReviewState",info.ReviewState),
                new SqlParameter("@IsLastTime",info.IsLastTime),
                new SqlParameter("@IsSystem",info.IsSystem),
                new SqlParameter("@ApplyNumber",info.ApplyNumber),
                new SqlParameter("@IsEnd",info.IsEnd)
            };
            parms[0].Value = info.ReportId;
            parms[1].Value = info.ReportNo;
            parms[2].Value = info.CompanyClassId;
            parms[3].Value = info.ReportName;
            parms[4].Value = info.ReportCost;
            parms[4].Precision = 18;
            parms[4].Scale = 4;
            parms[5].Value = info.PayCost;
            parms[5].Precision = 18;
            parms[5].Scale = 4;
            parms[6].Value = info.PayCompany;
            parms[7].Value = info.BankAccountName;
            parms[8].Value = info.BankAccount;
            parms[9].Value = info.ReportMemo;
            parms[10].Value = info.CostType;
            parms[11].Value = info.ReportKind;
            parms[12].Value = info.InvoiceType;
            parms[13].Value = info.InvoiceId;
            parms[14].Value = info.State;
            parms[15].Value = info.AuditingMemo;
            parms[16].Value = info.Memo;
            parms[17].Value = info.ReportPersonnelId;
            parms[18].Value = info.ReportDate;
            parms[19].Value = info.ReportFilialeId;
            parms[20].Value = info.ReportBranchId;
            parms[21].Value = info.Mobile;
            #endregion

            try
            {
                return SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL, parms) > 0;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                throw new ApplicationException(ex.Message);
            }
        }
        #endregion

        #region[修改申报]
        /// <summary>
        /// 修改申报
        /// </summary>
        /// <param name="info">申报详细模型</param>
        public void UpdateReport(CostReportInfo info)
        {
            const string SQL = @"
    UPDATE lmShop_CostReport SET 
    CompanyClassId=@CompanyClassId,
    CompanyId=@CompanyId,
    ReportName=@ReportName,
    ReportCost=@ReportCost,
    PayCompany=@PayCompany,
    BankAccountName=@BankAccountName,
    BankAccount=@BankAccount,
    ReportMemo=@ReportMemo,
    CostType=@CostType,
    ReportKind=@ReportKind,
    InvoiceType=@InvoiceType,
    InvoiceId=@InvoiceId,
    ReportDate=@ReportDate,
    State=@State,
    Memo=Memo+@Memo,
    Mobile=@Mobile,
    PayBankAccountID=@PayBankAccountID,
    RefundmentKind=@RefundmentKind,
    RefundmentBankAccountID=@RefundmentBankAccountID,
    UrgentOrDefer=@UrgentOrDefer,
    UrgentReason=@UrgentReason,
    InvoiceNo=@InvoiceNo,
    InvoiceTitle=@InvoiceTitle,
    InvoiceTitleFilialeId=@InvoiceTitleFilialeId,
    ReceiptNo=@ReceiptNo,
    ApplyForCost=@ApplyForCost,
    ActualAmount=@ActualAmount,
    AuditingBeforePay=@AuditingBeforePay,
    AssumeBranchId=@AssumeBranchId,
    AssumeGroupId=@AssumeGroupId,
    AssumeFilialeId=@AssumeFilialeId,
    AssumeShopId=@AssumeShopId,
    AuditingMan=@AuditingMan,
    StartTime=@StartTime,
    EndTime=@EndTime,
    CostsVarieties=@CostsVarieties,
    TradeNo=@TradeNo,
    ExecuteBankID=@ExecuteBankID,
    Poundage=@Poundage,
    AuditingDate=@AuditingDate,
    FinishDate=@FinishDate,
    IsOut=@IsOut,
    RealityCost=@RealityCost,
    AuditingMemo=@AuditingMemo,  
    Deposit=@Deposit,
    DepositNo=@DepositNo,
    GoodsCode=@GoodsCode,
    AcceptDate=@AcceptDate,
    ReviewState=@ReviewState,
    IsLastTime=@IsLastTime,
    IsSystem=@IsSystem,
    ApplyNumber=@ApplyNumber,
    IsEnd=@IsEnd
    WHERE ReportId=@ReportId";
            var parms = new[]{
                new SqlParameter(PARM_REPORT_ID,SqlDbType.UniqueIdentifier),
                new SqlParameter(PARM_COMPANY_CLASS_ID,SqlDbType.UniqueIdentifier),
                new SqlParameter(PARM_REPORT_NAME,SqlDbType.VarChar,50),
                new SqlParameter(PARM_REPORT_COST,SqlDbType.Decimal),
                new SqlParameter(PARM_PAY_COMPANY,SqlDbType.VarChar,128),
                new SqlParameter(PARM_BANK_ACCOUNT_NAME,SqlDbType.VarChar,128),
                new SqlParameter(PARM_BANK_ACCOUNT,SqlDbType.VarChar,128),
                new SqlParameter(PARM_REPORT_MEMO,SqlDbType.VarChar),
                new SqlParameter(PARM_COST_TYPE,SqlDbType.Int),
                new SqlParameter(PARM_REPORT_KIND,SqlDbType.Int),
                new SqlParameter(PARM_INVOICE_TYPE,SqlDbType.Int),
                new SqlParameter(PARM_INVOICE_ID,SqlDbType.UniqueIdentifier),
                new SqlParameter(PARM_REPORT_DATE,SqlDbType.DateTime),
                new SqlParameter(PARM_STATE,SqlDbType.Int),
                new SqlParameter(PARM_MEMO,info.Memo),
                new SqlParameter(PARM_MOBILE,SqlDbType.VarChar,24),
                new SqlParameter("@PayBankAccountID",SqlDbType.UniqueIdentifier),
                new SqlParameter("@RefundmentKind",SqlDbType.Int),
                new SqlParameter("@RefundmentBankAccountID",SqlDbType.UniqueIdentifier),
                new SqlParameter("@CompanyId",SqlDbType.UniqueIdentifier),
                new SqlParameter("@UrgentOrDefer",SqlDbType.Int),
                new SqlParameter("@UrgentReason",SqlDbType.VarChar),
                new SqlParameter("@InvoiceNo",SqlDbType.VarChar),
                new SqlParameter("@InvoiceTitle",SqlDbType.VarChar),
                new SqlParameter("@ReceiptNo",SqlDbType.VarChar),
                new SqlParameter("@InvoiceTitleFilialeId",info.InvoiceTitleFilialeId),
                new SqlParameter("@ApplyForCost",info.ApplyForCost),
                new SqlParameter("@ActualAmount",info.ActualAmount),
                new SqlParameter("@AuditingBeforePay",info.AuditingBeforePay),
                new SqlParameter("@AssumeBranchId",info.AssumeBranchId), 
                new SqlParameter("@AssumeGroupId",info.AssumeGroupId),
                new SqlParameter("@AssumeFilialeId",info.AssumeFilialeId) ,
                new SqlParameter("@AssumeShopId",info.AssumeShopId),
                new SqlParameter("@AuditingMan",info.AuditingMan),
                new SqlParameter("@StartTime",info.StartTime),
                new SqlParameter("@EndTime",info.EndTime),
                new SqlParameter("@CostsVarieties",info.CostsVarieties),
                new SqlParameter("@TradeNo",info.TradeNo),
                new SqlParameter("@ExecuteBankID",info.ExecuteBankId),
                new SqlParameter("@Poundage",info.Poundage),
                new SqlParameter("@AuditingDate",info.AuditingDate),
                new SqlParameter("@FinishDate",info.FinishDate),
                new SqlParameter("@IsOut",info.IsOut),
                new SqlParameter("@RealityCost",info.RealityCost),
                new SqlParameter("@AuditingMemo",info.AuditingMemo),
                new SqlParameter("@Deposit",info.Deposit),
                new SqlParameter("@DepositNo",info.DepositNo),
                new SqlParameter("@GoodsCode",info.GoodsCode),
                new SqlParameter("@AcceptDate",info.AcceptDate),
                new SqlParameter("@ReviewState",info.ReviewState),
                new SqlParameter("@IsLastTime",info.IsLastTime),
                new SqlParameter("@IsSystem",info.IsSystem),
                new SqlParameter("@ApplyNumber",info.ApplyNumber),
                new SqlParameter("@IsEnd",info.IsEnd)
            };
            parms[0].Value = info.ReportId;
            parms[1].Value = info.CompanyClassId;
            parms[2].Value = info.ReportName;
            parms[3].Value = info.ReportCost;
            parms[3].Precision = 18;
            parms[3].Scale = 4;
            parms[4].Value = info.PayCompany;
            parms[5].Value = info.BankAccountName;
            parms[6].Value = info.BankAccount;
            parms[7].Value = info.ReportMemo;
            parms[8].Value = info.CostType;
            parms[9].Value = info.ReportKind;
            parms[10].Value = info.InvoiceType;
            parms[11].Value = info.InvoiceId;
            parms[12].Value = info.ReportDate;
            parms[13].Value = info.State;
            parms[14].Value = info.Memo;
            parms[15].Value = info.Mobile;
            parms[16].Value = info.PayBankAccountId;
            parms[17].Value = info.RefundmentKind;
            parms[18].Value = info.RefundmentBankAccountId;
            parms[19].Value = info.CompanyId;
            parms[20].Value = info.UrgentOrDefer;
            parms[21].Value = info.UrgentReason;
            parms[22].Value = info.InvoiceNo;
            parms[23].Value = info.InvoiceTitle;
            parms[24].Value = info.ReceiptNo;
            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL, parms);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
        #endregion

        #region[修改申报状态]
        ///  <summary>
        /// 修改申报状态
        /// </summary>
        /// <param name="reportId">申报ID</param>
        /// <param name="state">状态</param>
        /// <param name="auditingMemo">审核说明</param>
        /// <param name="memo">备注</param>
        /// <param name="auditingMan">审批人(为Empty则不更新该字段)</param>
        public void UpdateReport(Guid reportId, int state, string auditingMemo, string memo, Guid auditingMan)
        {
            var parms = new[]{
                new SqlParameter(PARM_STATE,SqlDbType.Int),
                new SqlParameter(PARM_AUDITING_MEMO,SqlDbType.VarChar,1024),
                new SqlParameter(PARM_MEMO,SqlDbType.VarChar,1024),
                new SqlParameter(PARM_REPORT_ID,SqlDbType.UniqueIdentifier),
                new SqlParameter(PARM_AUDITING_MAN,SqlDbType.UniqueIdentifier)
            };
            parms[0].Value = state;
            parms[1].Value = auditingMemo;
            parms[2].Value = memo;
            parms[3].Value = reportId;
            parms[4].Value = auditingMan;
            string sql = auditingMan == Guid.Empty ? SQL_UPDATE_REPORT_STATE : SQL_UPDATE_REPORT_STATE_AUDITING_MAN;
            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, sql, parms);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
        #endregion

        #region[修改申报已支付金额、状态、备注]
        ///  <summary>
        /// 修改申报已支付金额、状态、备注
        /// </summary>
        /// <param name="reportId">申报ID</param>
        /// <param name="state">状态</param>
        /// <param name="payCost">审核说明</param>
        /// <param name="memo">备注</param>
        public void UpdateReport(Guid reportId, int state, Decimal payCost, string memo)
        {
            var parms = new[]{
                new SqlParameter(PARM_STATE,SqlDbType.Int),
                new SqlParameter(PARM_PAY_COST,SqlDbType.Decimal),
                new SqlParameter(PARM_MEMO,SqlDbType.VarChar,1024),
                new SqlParameter(PARM_REPORT_ID,SqlDbType.UniqueIdentifier)
            };
            parms[0].Value = state;
            parms[1].Value = payCost;
            parms[1].Precision = 18;
            parms[1].Scale = 4;
            parms[2].Value = memo;
            parms[3].Value = reportId;
            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL_UPDATE_REPORT_PAY_COST_STATE_AND_MEMO, parms);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
        #endregion

        #region[修改申报金额、已支付金额、状态、备注]
        ///  <summary>
        /// 修改申报金额、已支付金额、状态、备注
        /// </summary>
        /// <param name="reportId">申报ID</param>
        /// <param name="state">状态</param>
        /// <param name="reportCost">申报金额</param>
        /// <param name="payCost">审核说明</param>
        /// <param name="memo">备注</param>
        public void UpdateReport(Guid reportId, int state, Decimal reportCost, Decimal payCost, string memo)
        {
            var parms = new[]{
                new SqlParameter(PARM_STATE,SqlDbType.Int),
                new SqlParameter(PARM_PAY_COST,SqlDbType.Decimal),
                new SqlParameter(PARM_MEMO,SqlDbType.VarChar,1024),
                new SqlParameter(PARM_REPORT_ID,SqlDbType.UniqueIdentifier),
                new SqlParameter(PARM_REPORT_COST,SqlDbType.Decimal),
                new SqlParameter(PARM_EXECUTEDATE,SqlDbType.DateTime) 
            };
            parms[0].Value = state;
            parms[1].Value = payCost;
            parms[1].Precision = 18;
            parms[1].Scale = 4;
            parms[2].Value = memo;
            parms[3].Value = reportId;
            parms[4].Value = reportCost;
            parms[4].Precision = 18;
            parms[4].Scale = 4;
            parms[5].Value = DateTime.Now;
            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL_UPDATE_REPORT_REPORT_COST_PAY_COST_STATE_AND_MEMO, parms);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
        #endregion

        #region 修改申报金额
        /// <summary>
        /// 修改实际金额
        /// </summary>
        /// <param name="reportId">申报费用id</param>
        /// <param name="state">状态</param>
        /// <param name="realityCost">实际金额</param>
        /// <param name="memo">备注</param>
        /// zal 2016-01-11
        public void UpdateRealityCost(Guid reportId, int state, Decimal realityCost, string memo)
        {
            string sql = "UPDATE lmShop_CostReport SET State=@State,RealityCost=@RealityCost,Memo=Memo+@Memo WHERE ReportId=@ReportId";
            var parms = new[]{
                new SqlParameter("@ReportId",reportId),
                new SqlParameter("@State",state),
                new SqlParameter("@RealityCost",realityCost),
                new SqlParameter("@Memo",memo) 
            };
            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, sql, parms);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
        #endregion

        #region[修改申报状态、备注、付款类型银行账号]

        ///  <summary>
        /// 修改申报状态、备注、付款类型银行账号
        /// </summary>
        /// <param name="reportId">申报ID</param>
        /// <param name="state">状态</param>
        /// <param name="companyId">打款分类</param>
        /// <param name="bankAccountName">收款账户</param>
        /// <param name="bankAccount">收款款银行</param>
        /// <param name="auditingMan">审核人</param>
        /// <param name="payBankAccountId">打款账号ID</param>
        /// <param name="memo">备注</param>
        /// <param name="companyClassId">费用分类</param>
        public void UpdateReportAuditing(Guid reportId, int state, string memo, Guid companyId, string bankAccountName, string bankAccount, Guid auditingMan, Guid payBankAccountId, Guid companyClassId)
        {
            var parms = new[]{
                new SqlParameter(PARM_STATE,SqlDbType.Int),
                new SqlParameter(PARM_MEMO,SqlDbType.VarChar),
                new SqlParameter(PARM_REPORT_ID,SqlDbType.UniqueIdentifier),
                new SqlParameter(PARM_COMPANY_ID,SqlDbType.UniqueIdentifier),
                new SqlParameter(PARM_BANK_ACCOUNT_NAME,SqlDbType.VarChar,128),
                new SqlParameter(PARM_BANK_ACCOUNT,SqlDbType.VarChar,128),
                new SqlParameter(PARM_AUDITING_MAN,SqlDbType.UniqueIdentifier),
                new SqlParameter(PARM_PAY_BANK_ACCOUNT_ID,SqlDbType.UniqueIdentifier),
                new SqlParameter(PARM_COMPANY_CLASS_ID,SqlDbType.UniqueIdentifier)
            };
            parms[0].Value = state;
            parms[1].Value = memo;
            parms[2].Value = reportId;
            parms[3].Value = companyId;
            parms[4].Value = bankAccountName;
            parms[5].Value = bankAccount;
            parms[6].Value = auditingMan;
            parms[7].Value = payBankAccountId;
            parms[8].Value = companyClassId;
            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL_UPDATE_REPORT_STATE_AND_MEMO, parms);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
        #endregion

        #region[修改申报实际金额]
        /// <summary>
        /// 修改申报实际金额
        /// </summary>
        /// <param name="reportId">申报ID</param>
        /// <param name="realityCost">实际金额</param>
        public void UpdateReportRealityCost(Guid reportId, decimal realityCost)
        {
            var parms = new[]{
                new SqlParameter(PARM_REPORT_ID,SqlDbType.UniqueIdentifier),
                new SqlParameter(PARM_REALITY_COST,SqlDbType.Decimal)
            };
            parms[0].Value = reportId;
            parms[1].Value = realityCost;
            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL_UPDATE_REPORT_REALITY_COST, parms);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
        #endregion

        #region[修改申报手续费]
        ///  <summary>
        /// 修改申报手续费
        /// Add by liucaijun at 2012-02-09
        /// </summary>
        /// <param name="reportId">申报ID</param>
        /// <param name="poundage">手续费</param>
        public void UpdateReportPoundage(Guid reportId, Decimal poundage)
        {
            var parms = new[]{
                new SqlParameter(PARM_REPORT_ID,SqlDbType.UniqueIdentifier),
                new SqlParameter(PARM_POUNDAGE,SqlDbType.Decimal) 
            };
            parms[0].Value = reportId;
            parms[1].Value = poundage;
            parms[1].Precision = 18;
            parms[1].Scale = 4;
            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL_UPDATE_REPORT_POUNDAGE, parms);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
        #endregion

        #region[修改申报本公司打款银行]
        ///  <summary>
        /// 修改申报本公司打款银行
        /// Add by liucaijun at 2012-02-09
        /// </summary>
        /// <param name="reportId">申报ID</param>
        /// <param name="executeBankId">本公司打款银行</param>
        public void UpdateReportExecuteBankId(Guid reportId, Guid executeBankId)
        {
            var parms = new[]{
                new SqlParameter(PARM_REPORT_ID,SqlDbType.UniqueIdentifier),
                new SqlParameter(PARM_EXECUTEBANKID,SqlDbType.UniqueIdentifier) 
            };
            parms[0].Value = reportId;
            parms[1].Value = executeBankId;
            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL_UPDATE_REPORT_EXECUTEBANKID, parms);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
        #endregion

        #region[修改申报状态]
        ///  <summary>
        /// 修改申报状态
        /// </summary>
        /// <param name="reportId">申报ID</param>
        /// <param name="state">状态</param>
        /// <param name="memo">备注</param>
        public void UpdateReport(Guid reportId, int state, string memo)
        {
            var parms = new[]{
                new SqlParameter(PARM_STATE,SqlDbType.Int),
                new SqlParameter(PARM_MEMO,SqlDbType.VarChar,1024),
                new SqlParameter(PARM_REPORT_ID,SqlDbType.UniqueIdentifier)
            };
            parms[0].Value = state;
            parms[1].Value = memo;
            parms[2].Value = reportId;
            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL_UPDATE_REPORT_STATE_MEMO, parms);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
        #endregion

        #region[修改申报时间]
        ///  <summary>
        /// 根据申报类型修改相应类型的时间(1:审核时间，2:预付款时间，3:完成时间)
        /// </summary>
        /// <param name="reportId">申报ID</param>
        /// <param name="dateTime">时间</param>
        /// <param name="type">类型，1审核时间，2预付款时间，3完成时间</param>
        public void UpdateReportDate(Guid reportId, DateTime dateTime, int type)
        {
            const string SQL_UPDATE_REPORT_DATE = @"UPDATE lmShop_CostReport SET  ";
            var sqlstr = new StringBuilder(SQL_UPDATE_REPORT_DATE);
            if (type == 1)
            {
                sqlstr.Append("AuditingDate=@Date ");
            }
            if (type == 2)
            {
                sqlstr.Append("ExecuteDate=@Date ");
            }
            if (type == 3)
            {
                sqlstr.Append("FinishDate=@Date ");
            }
            sqlstr.Append(" WHERE ReportId=@ReportId ");

            var parms = new[]{
                new SqlParameter("@Date",SqlDbType.DateTime),
                new SqlParameter(PARM_REPORT_ID,SqlDbType.UniqueIdentifier)
            };
            parms[0].Value = dateTime;
            parms[1].Value = reportId;
            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, sqlstr.ToString(), parms);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
        #endregion

        /// <summary>
        /// 修改费用承担公司部门小组
        /// </summary>
        /// <param name="reportId"></param>
        /// <param name="assumeFilialeId"></param>
        /// <param name="assumeBranchId"></param>
        /// <param name="assumeGroupId"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public bool UpdateReportAssumeFilialeIdBranchIdGroupId(Guid reportId, Guid assumeFilialeId, Guid assumeBranchId, Guid assumeGroupId, out string errorMessage)
        {
            errorMessage = string.Empty;
            var strbSql = new StringBuilder(@"UPDATE lmShop_CostReport SET ");
            strbSql.Append(" AssumeFilialeId='").Append(assumeFilialeId).Append("'");
            strbSql.Append(" ,AssumeBranchId='").Append(assumeBranchId).Append("'");
            strbSql.Append(" ,AssumeGroupId='").Append(assumeGroupId).Append("'");
            strbSql.Append(" WHERE ReportId='").Append(reportId).Append("'");

            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, strbSql.ToString());
                return true;
            }
            catch (SqlException ex)
            {
                errorMessage = ex.Message;
                throw new ApplicationException(errorMessage);
            }
        }

        /// <summary>
        /// 记录交易流水号
        /// </summary>
        /// <param name="reportId"></param>
        /// <param name="tradeNo"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public bool UpdateReportTradeNo(Guid reportId, string tradeNo, out string errorMessage)
        {
            errorMessage = string.Empty;
            var strbSql = new StringBuilder();
            strbSql.Append("UPDATE lmShop_CostReport SET TradeNo='").Append(tradeNo).Append("'");
            strbSql.Append(" WHERE ReportId='").Append(reportId).Append("'");

            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, strbSql.ToString());
                return true;
            }
            catch (SqlException ex)
            {
                errorMessage = ex.Message;
                throw new ApplicationException(errorMessage);
            }
        }

        /// <summary>
        /// 修改申报直营店
        /// </summary>
        /// <param name="reportId"></param>
        /// <param name="shopId"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public bool UpdateAssumeShopId(Guid reportId, Guid shopId, out string errorMessage)
        {
            errorMessage = string.Empty;
            var strbSql = new StringBuilder();
            strbSql.Append("UPDATE lmShop_CostReport SET AssumeShopId='").Append(shopId).Append("'");
            strbSql.Append(" WHERE ReportId='").Append(reportId).Append("'");

            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, strbSql.ToString());
                return true;
            }
            catch (SqlException ex)
            {
                errorMessage = ex.Message;
                throw new ApplicationException(errorMessage);
            }
        }


        /// <summary>
        /// 预借款审核通过，待预借款(待收款)
        /// </summary>
        /// <param name="reportId"></param>
        /// <param name="auditingMan"></param>
        /// <param name="executeBandId"></param>
        /// <param name="memo"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public bool UpdateReportBeforeAuditingPass(Guid reportId, Guid auditingMan, Guid executeBandId, string memo, out string errorMessage)
        {
            errorMessage = string.Empty;
            var strbSql = new StringBuilder(@"UPDATE lmShop_CostReport SET State=4");
            strbSql.Append(" ,ExecuteBankId='").Append(executeBandId).Append("'");
            strbSql.Append(" ,Memo=Memo+'").Append(memo).Append("'");
            strbSql.Append(" ,AuditingMan='").Append(auditingMan).Append("'");
            strbSql.Append(" ,AuditingDate='").Append(DateTime.Now).Append("'");
            strbSql.Append(" WHERE ReportId='").Append(reportId).Append("'");

            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, strbSql.ToString());
                return true;
            }
            catch (SqlException ex)
            {
                errorMessage = ex.Message;
                throw new ApplicationException(errorMessage);
            }
        }
        /// <summary>
        /// 预借款审核不通过
        /// </summary>
        /// <param name="reportId"></param>
        /// <param name="auditingMan"></param>
        /// <param name="auditingMemo"></param>
        /// <param name="memo"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public bool UpdateReportBeforeAuditingNoPass(Guid reportId, Guid auditingMan, string auditingMemo, string memo, out string errorMessage)
        {
            errorMessage = string.Empty;
            var strbSql = new StringBuilder(@"UPDATE lmShop_CostReport SET State=3");
            strbSql.Append(" ,AuditingMan='").Append(auditingMan).Append("'");
            strbSql.Append(" ,AuditingDate='").Append(DateTime.Now).Append("'");
            strbSql.Append(" ,AuditingMemo='").Append(auditingMemo).Append("'");
            strbSql.Append(" ,Memo=Memo+'").Append(memo).Append("'");
            strbSql.Append(" WHERE ReportId='").Append(reportId).Append("'");

            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, strbSql.ToString());
                return true;
            }
            catch (SqlException ex)
            {
                errorMessage = ex.Message;
                throw new ApplicationException(errorMessage);
            }
        }

        /// <summary>
        /// 预借款打款通过，11待打款（已打款）
        /// </summary>
        /// <param name="reportId"></param>
        /// <param name="executeBandId"></param>
        /// <param name="tradeNo">交易流水号</param>
        /// <param name="poundage">手续费</param>
        /// <param name="memo"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public bool UpdateReportBeforeExecutePass(Guid reportId, Guid executeBandId, string tradeNo, decimal poundage, string memo, out string errorMessage)
        {
            errorMessage = string.Empty;
            var strbSql = new StringBuilder(@"UPDATE lmShop_CostReport SET State=11");
            strbSql.Append(" ,ExecuteBankId='").Append(executeBandId).Append("'");
            strbSql.Append(" ,TradeNo='").Append(tradeNo).Append("'");
            strbSql.Append(" ,Poundage=").Append(poundage);
            strbSql.Append(" ,Memo=Memo+'").Append(memo).Append("'");
            strbSql.Append(" ,AuditingDate='").Append(DateTime.Now).Append("'");
            strbSql.Append(" WHERE ReportId='").Append(reportId).Append("'");

            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, strbSql.ToString());
                return true;
            }
            catch (SqlException ex)
            {
                errorMessage = ex.Message;
                throw new ApplicationException(errorMessage);
            }
        }

        public int UpdateReportAuditingBeforePayByReportId(Guid reportId, int auditingBeforePay, out string errorMessage)
        {
            errorMessage = string.Empty;
            var strbSql = new StringBuilder(@"UPDATE lmShop_CostReport");
            strbSql.Append(" SET AuditingBeforePay=").Append(auditingBeforePay);
            strbSql.Append(" WHERE ReportId='").Append(reportId).Append("'");

            try
            {
                return SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, strbSql.ToString());
            }
            catch (SqlException ex)
            {
                errorMessage = ex.Message;
                throw new ApplicationException(errorMessage);
            }
        }

        public int UpdateCostReportRefundmentBankAccountIdByReportId(Guid reportId, Guid refundmentBankAccountId)
        {
            var strbSql = new StringBuilder();
            strbSql.Append(@"UPDATE lmShop_CostReport SET RefundmentBankAccountID='").Append(refundmentBankAccountId).Append("'");
            strbSql.Append(" WHERE ReportId='").Append(reportId).Append("'");
            try
            {
                return SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, strbSql.ToString());
            }
            catch (SqlException ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        public int UpdateCostReportPayBankAccountIdByReportId(Guid reportId, Guid payBankAccountId)
        {
            var strbSql = new StringBuilder();
            strbSql.Append(@"UPDATE lmShop_CostReport SET PayBankAccountID='").Append(payBankAccountId).Append("'");
            strbSql.Append(" WHERE ReportId='").Append(reportId).Append("'");
            try
            {
                return SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, strbSql.ToString());
            }
            catch (SqlException ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>更新费用申报IsOut为True ADD 陈重文 2015-1-28 
        /// </summary>
        /// <param name="reportId"></param>
        public void UpdateCostReportIsOut(Guid reportId)
        {
            var strbSql = new StringBuilder();
            strbSql.Append(@"UPDATE lmShop_CostReport SET IsOut=1");
            strbSql.Append(" WHERE ReportId='").Append(reportId).Append("'");
            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, strbSql.ToString());
            }
            catch (SqlException ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// 根据reportId修改“已支付金额”和“执行收付款时间”
        /// </summary>
        /// <param name="reportId"></param>
        /// <param name="payCost">已支付金额</param>
        /// <returns></returns>
        /// zal 2015-11-19
        public bool UpdatePayCostAndExecuteDate(Guid reportId, decimal payCost)
        {
            string sql = "UPDATE lmShop_CostReport SET PayCost=PayCost+@PayCost,ExecuteDate=@ExecuteDate WHERE ReportId=@ReportId";
            var parms = new[]{
                new SqlParameter("@ReportId", reportId),
                new SqlParameter("@PayCost", payCost),
                new SqlParameter("@ExecuteDate",DateTime.Now) 
            };
            try
            {
                return SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, sql, parms) > 0;
            }
            catch (SqlException ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }


        #region[修改申报]
        /// <summary>
        /// 根据申报IDp批量修改申报申请人
        /// </summary>
        public bool UpdateReportPersonnelIdByReportId(List<Guid> reportIds, Guid reportPersonnelId)
        {
            var personnelIdStr = "'" + string.Join("','", reportIds.ToArray()) + "'";
            var sql = @"
    UPDATE lmShop_CostReport SET 
    ReportPersonnelId=@ReportPersonnelId
    WHERE ReportId IN(" + personnelIdStr + ")";
            var parms = new[]{
                new SqlParameter("@ReportPersonnelId",SqlDbType.UniqueIdentifier)
            };
            parms[0].Value = reportPersonnelId;

            try
            {
                return SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, sql, parms) > 0;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
        #endregion
    }
}
