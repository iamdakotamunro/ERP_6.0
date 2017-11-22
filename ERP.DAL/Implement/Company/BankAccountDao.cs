using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using ERP.DAL.Interface.ICompany;
using ERP.Model;
using ERP.Environment;
using Keede.DAL.RWSplitting;
using Dapper;
using System.Transactions;

namespace ERP.DAL.Implement.Company
{
    public sealed class BankAccountDao : IBankAccountDao
    {
        public BankAccountDao(GlobalConfig.DB.FromType fromType) { }

        #region T_SQL
        //insert
        private const string SQL_INSERT = "INSERT INTO BankAccountBinding(TargetId,BankAccountsId) VALUES (@TargetId,@BankAccountsId);";
        //delete
        private const string SQL_DELETE = "DELETE BankAccountBinding WHERE BankAccountsId=@BankAccountsId and TargetId=@TargetId";
        //update 
        private const string SQL_UPDATE = "UPDATE BankAccountBinding SET BankAccountsId=@BankAccountsId WHERE BankAccountsId=@OldBankAccountsId AND TargetId=@TargetId";

        //select
        private const string SQL_SELECT_BY_BANKACCOUNTSID = @"
select b.BankName,b.Accounts,b.AccountsName,a.BankAccountsId,TargetId,b.IsUse,b.IsMain,b.IsDisplay,b.AccountType  
from BankAccountBinding as a inner join lmShop_BankAccounts as b on a.BankAccountsId=b.BankAccountsId 
where a.BankAccountsId=@BankAccountsId
";
        private const string SQL_SELECT_BY_TARGETID =
@"select b.BankName,b.Accounts,b.AccountsName,a.BankAccountsId,TargetId,b.IsUse,b.PaymentType,b.IsMain,b.IsDisplay,bb.NonceBalance from BankAccountBinding as a 
inner join lmShop_BankAccounts as b on a.BankAccountsId=b.BankAccountsId
inner join lmshop_BankAccountsBalance bb on b.BankAccountsId=bb.BankAccountsId
where TargetId=@TargetId and b.IsUse=1 ORDER BY b.OrderIndex ";
        private const string SQL_SELECT_BY_BANKACCOUNTSID_FROMSOUCEID_FROMTYPE = "SELECT BankAccountsId,TargetId FROM BankAccountBinding where BankAccountsId=@BankAccountsId and TargetId=@TargetId";
        //private const string SQL_SELECT_LIST = "SELECT BankAccountsId,TargetId FROM BankAccountBinding";

        #endregion

        #region param
        private const string PARM_BANKACCOUNTSID = "@BankAccountsId";
        private const string PARM_TARGETID = "@TargetId";
        #endregion

        /// <summary>
        /// 插入一条记录
        /// </summary>
        public void Insert(BankAccountInfo info)
        {
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                conn.Execute(SQL_INSERT, new { BankAccountsId = info.BankAccountsId, TargetId = info.TargetId });
            }
        }

        public void Update(Guid oldBankAccountsId, BankAccountInfo info)
        {
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                conn.Execute(SQL_UPDATE, new { BankAccountsId = info.BankAccountsId, TargetId = info.TargetId, OldBankAccountsId = oldBankAccountsId });
            }
        }

        /// <summary>
        /// 删除一条记录
        /// </summary>
        public void Delete(Guid bankAccountsId, Guid targetId)
        {
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                conn.Execute(SQL_DELETE, new { BankAccountsId = bankAccountsId, TargetId = targetId });
            }
        }

        #region SELECT

        /// <summary>
        /// 根据资金帐号ID获取LIST
        /// </summary>
        /// <param name="bankaccountid"></param>
        /// <returns></returns>
        public IList<BankAccountInfo> GetListByBankAccountId(Guid bankaccountid)
        {
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.Query<BankAccountInfo>(SQL_SELECT_BY_BANKACCOUNTSID, new { BankAccountsId = bankaccountid }).AsList();
            }
        }

        /// <summary>
        /// 根据来源ID,站点类型来获取
        /// </summary>
        /// <returns></returns>
        public BankAccountInfo Get(BankAccountInfo sinfo)
        {
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                return conn.QueryFirstOrDefault<BankAccountInfo>(SQL_SELECT_BY_BANKACCOUNTSID_FROMSOUCEID_FROMTYPE, new { BankAccountsId = sinfo.BankAccountsId, TargetId = sinfo.TargetId });
            }
        }

        public IList<BankAccountInfo> GetListByTargetId(Guid targetId)
        {
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.Query<BankAccountInfo>(SQL_SELECT_BY_TARGETID, new { TargetId = targetId }).AsList();
            }
        }

        /// <summary>获取绑定的资金账户(主账号/非主账号)
        /// </summary>
        /// <param name="isMain">是否主账号（true 主）</param>
        /// <returns></returns>
        public IList<BankAccountInfo> GetListByNotIsMain(Boolean isMain)
        {
            const string SQL = @"SELECT BankName,Accounts,AccountsName,B.BankAccountsId,
IsUse,PaymentType,IsMain,IsDisplay,BB.NonceBalance FROM lmShop_BankAccounts B 
INNER JOIN lmshop_BankAccountsBalance BB ON B.BankAccountsId=BB.BankAccountsId
WHERE IsMain=@IsMain AND B.IsUse=1 ORDER BY B.OrderIndex ";

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.Query<BankAccountInfo>(SQL, new { IsMain = isMain }).AsList();
            }
        }

        public IEnumerable<BankAccountInfo> GetList()
        {
            const string SQL = @"
SELECT BA.*,BAB.TargetId FROM lmShop_BankAccounts BA
LEFT JOIN BankAccountBinding BAB ON BAB.BankAccountsId = BA.BankAccountsId
";
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.Query<BankAccountInfo>(SQL).AsList();
            }
        }

        #endregion
    }
}
