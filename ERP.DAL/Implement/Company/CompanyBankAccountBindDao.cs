using System;
using System.Data.SqlClient;
using ERP.DAL.Interface.ICompany;
using ERP.Environment;
using ERP.Model;
using Dapper;
using Keede.DAL.RWSplitting;
using System.Transactions;

namespace ERP.DAL.Implement.Company
{
    public class CompanyBankAccountBindDao : ICompanyBankAccountBind
    {
        public CompanyBankAccountBindDao(GlobalConfig.DB.FromType fromType) { }

        #region SQL
        private const string SQL_INSERT = @"IF EXISTS (SELECT FilialeId FROM CompanyBankAccountBind WHERE CompanyId=@CompanyId AND FilialeId=@FilialeId)
BEGIN
	UPDATE CompanyBankAccountBind SET BankAccounts=@BankAccounts,AccountsNumber=@AccountsNumber,WebSite=@WebSite 
	WHERE CompanyId=@CompanyId AND FilialeId=@FilialeId
END
ELSE
BEGIN
	INSERT INTO CompanyBankAccountBind(CompanyId,FilialeId,BankAccounts,AccountsNumber,WebSite) 
     VALUES (@CompanyId,@FilialeId,@BankAccounts,@AccountsNumber,@WebSite)
END;";

        private const string SQL_DELETE = @"DELETE CompanyBankAccountBind WHERE CompanyId=@CompanyId ";

        private const string SQL_SELECT_BY_COMPANYID_FILIALEID = @"SELECT TOP 1 CompanyId,FilialeId,BankAccounts,AccountsNumber,WebSite
FROM CompanyBankAccountBind WHERE CompanyId=@CompanyId AND FilialeId=@FilialeId";

        private const string SQL_SELECT_BY_COMPANYID = @"SELECT CompanyId,FilialeId,BankAccounts,AccountsNumber,WebSite
FROM CompanyBankAccountBind WHERE CompanyId=@CompanyId ";

        #endregion

        /// <summary>
        /// 有则更新无则插入绑定数据
        /// </summary>
        /// <param name="bankAccountBindInfo"></param>
        /// <returns></returns>
        public bool InsertCompanyBankAccountBind(CompanyBankAccountBindInfo bankAccountBindInfo)
        {
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                return conn.Execute(SQL_INSERT, new
                {
                    CompanyId = bankAccountBindInfo.CompanyId,
                    FilialeId = bankAccountBindInfo.FilialeId,
                    BankAccounts = bankAccountBindInfo.BankAccounts,
                    AccountsNumber = bankAccountBindInfo.AccountsNumber,
                    WebSite = bankAccountBindInfo.WebSite,
                }) > 0;
            }
        }

        /// <summary>
        /// 有则更新无则插入绑定数据
        /// </summary>
        /// <param name="bankAccountBindInfo"></param>
        /// <returns></returns>
        public bool InsertCompanyBankAccountBindWithFiliale(CompanyBankAccountBindInfo bankAccountBindInfo)
        {
            const string SQL = @"IF EXISTS (SELECT FilialeId FROM CompanyBankAccounts WHERE CompanyId=@CompanyId AND FilialeId=@FilialeId)
BEGIN
	UPDATE CompanyBankAccounts SET BankAccountsId=@BankAccountsId WHERE CompanyId=@CompanyId AND FilialeId=@FilialeId
END
ELSE
BEGIN
	INSERT INTO CompanyBankAccounts(CompanyId,FilialeId,BankAccountsId) VALUES (@CompanyId,@FilialeId,@BankAccountsId)
END;";
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                return conn.Execute(SQL, new
                {
                    CompanyId = bankAccountBindInfo.CompanyId,
                    FilialeId = bankAccountBindInfo.FilialeId,
                    BankAccountsId = bankAccountBindInfo.BankAccountsId,
                }) > 0;
            }
        }

        /// <summary>
        /// 删除往来单位和公司绑定的银行账户
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public bool DeleteCompanyBankAccountBind(Guid companyId)
        {
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                return conn.Execute(SQL_DELETE, new
                {
                    CompanyId = companyId,
                }) >= 0;
            }
        }

        /// <summary>
        /// 根据公司和往来单位获取绑定的银行账户
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="filialeId"></param>
        /// <returns></returns>
        public CompanyBankAccountBindInfo GetCompanyBankAccountBindInfo(Guid companyId, Guid filialeId)
        {
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                return conn.QueryFirstOrDefault<CompanyBankAccountBindInfo>(SQL_SELECT_BY_COMPANYID_FILIALEID, new
                {
                    CompanyId = companyId,
                    FilialeId = filialeId
                });
            }
        }

        /// <summary>
        /// 获取往来单位绑定公司收款信息数
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public int GetBindCount(Guid companyId)
        {
            const string SQL = @"SELECT COUNT(0) FROM CompanyBankAccountBind WHERE CompanyId=@CompanyId ";

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.ExecuteScalar<int>(SQL, new
                {
                    CompanyId = companyId,
                });
            }
        }


        public CompanyBankAccountBindInfo GetCompanyBankAccountIdBind(Guid companyId, Guid filialeId)
        {
            const string SQL = @"SELECT TOP 1 B.CompanyId,FilialeId,A.AccountsName AS BankAccounts,A.Accounts AS AccountsNumber,A.BankName AS WebSite, B.BankAccountsId   
FROM CompanyBankAccounts as B WITH(NOLOCK)
INNER JOIN lmShop_BankAccounts as A WITH(NOLOCK)
ON A.BankAccountsId=B.BankAccountsId
 WHERE B.CompanyId=@CompanyId AND FilialeId=@FilialeId  ";

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.QueryFirstOrDefault<CompanyBankAccountBindInfo>(SQL, new
                {
                    CompanyId = companyId,
                    FilialeId = filialeId,
                });
            }
        }
    }
}
