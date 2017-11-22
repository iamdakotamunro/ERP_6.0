using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Model;
using ERP.Model.ShopFront;
using Keede.DAL.RWSplitting;
using ERP.Environment;
using Dapper;
using System.Transactions;

namespace ERP.DAL.Implement.Inventory
{
    /// <summary>
    /// 银行帐号数据库操作类
    /// </summary>
    public class BankAccounts : IBankAccounts
    {
        public BankAccounts(Environment.GlobalConfig.DB.FromType fromType) { }

        #region[SQL]

        private const string SQL_INSERT_BANKACCOUNTS =
                @"INSERT INTO lmShop_BankAccounts([BankAccountsId],[BankName],[PaymentInterfaceId],[AccountsName],[Accounts],[AccountsKey],[PaymentType],[BankIcon],[OrderIndex],[Description],[IsUse],[IsFinish],[IsBacktrack],[IsDisplay],[AccountType]) VALUES(@BankAccountsId,@BankName,@PaymentInterfaceId,@AccountsName,@Accounts,@AccountsKey,@PaymentType,@BankIcon,(select ISNULL(MAX(OrderIndex),0)+1 from lmShop_BankAccounts),@Description,@IsUse,@IsFinish,@IsBacktrack,@IsDisplay,@AccountType);
INSERT INTO lmShop_BankAccountsBalance([BankAccountsId],[NonceBalance]) VALUES(@BankAccountsId,0);";


        private const string SQL_UPDATE_BANKACCOUNTS =
                @"UPDATE lmShop_BankAccounts SET BankName=@BankName,PaymentInterfaceId=@PaymentInterfaceId,AccountsName=@AccountsName,Accounts=@Accounts,AccountsKey=@AccountsKey,PaymentType=@PaymentType,BankIcon=@BankIcon,Description=@Description,IsUse=@IsUse,IsFinish=@IsFinish,IsBacktrack=@IsBacktrack,IsDisplay=@IsDisplay,AccountType=@AccountType WHERE BankAccountsId=@BankAccountsId;";
        private const string SQL_DELETE_BANKACCOUNTS =
            @"DELETE FROM lmShop_WasteBook WHERE BankAccountsId=@BankAccountsId;
            DELETE lmShop_BankAccountsBalance WHERE BankAccountsId=@BankAccountsId;
            
            update lmShop_BankAccounts
			set OrderIndex=OrderIndex-1
			where OrderIndex >(select OrderIndex from lmShop_BankAccounts WHERE BankAccountsId=@BankAccountsId)
            DELETE FROM lmShop_BankAccounts WHERE BankAccountsId=@BankAccountsId;
";

        private const string SQL_SELECT_BANKACCOUNTS = @"
SELECT BankAccountsId,BankName,PaymentInterfaceId,AccountsName,Accounts,
AccountsKey,PaymentType,BankIcon,OrderIndex,Description,IsUse,IsFinish,IsBacktrack,IsMain,IsDisplay,AccountType FROM lmShop_BankAccounts WHERE BankAccountsId=@BankAccountsId;";

        private const string SQL_SELECT_BANKACCOUNTS_LIST = @"select a.BankAccountsId,BankName,AccountsName,PaymentInterfaceId,Accounts,AccountsKey,PaymentType
 ,BankIcon,OrderIndex,[Description],IsUse,IsFinish,IsBacktrack,IsMain,IsDisplay,AccountType 
 FROM lmShop_BankAccounts a
 inner join BankAccountPermission b on a.BankAccountsId=b.BankAccountsId
 where 1=1 and FilialeId=@FilialeId and BranchId=@BranchId and PositionId=@PositionId   ORDER BY OrderIndex ASC;";

        //        private const string SQL_SELECT_BANKACCOUNTS_ALLLIST = @" SELECT BankAccountsId,BankName,PaymentInterfaceId,BankName+'('+AccountsName+')' AS AccountsName,Accounts,AccountsKey,PaymentType
        //         ,BankIcon,OrderIndex,Description,IsUse,IsFinish FROM lmShop_BankAccounts WHERE 1=1 ORDER BY OrderIndex ASC;";

        private const string SQL_SELECT_BANKACCOUNTS_LIST_BY_PAYMENTTYPE = "SELECT BankAccountsId,BankName,PaymentInterfaceId,AccountsName,Accounts,AccountsKey,PaymentType,BankIcon,OrderIndex,Description,IsUse,IsFinish,IsBacktrack,[IsMain],[IsDisplay],AccountType FROM lmShop_BankAccounts WHERE PaymentType=@PaymentType ORDER BY OrderIndex ASC;";
        private const string SQL_SELECT_BANKACCOUNTS_LIST_BY_PAYMENTTYPES = "SELECT BankAccountsId,BankName,PaymentInterfaceId,AccountsName,Accounts,AccountsKey,PaymentType,BankIcon,OrderIndex,Description,IsUse,IsFinish,IsBacktrack,[IsMain],[IsDisplay],AccountType FROM lmShop_BankAccounts WHERE PaymentType IN {0} ORDER BY OrderIndex ASC;";
        private const string SQL_UPDATE_BANKACCOUNTS_UPORDERINDEX = "DECLARE @OrderIndex INT; SELECT @OrderIndex=OrderIndex FROM lmShop_BankAccounts WHERE BankAccountsId=@BankAccountsId;IF @OrderIndex>1 BEGIN UPDATE lmShop_BankAccounts SET OrderIndex=OrderIndex+1 WHERE OrderIndex=@OrderIndex-1;UPDATE lmShop_BankAccounts SET OrderIndex=OrderIndex-1 WHERE BankAccountsId=@BankAccountsId; END";
        private const string SQL_UPDATE_BANKACCOUNTS_DOWNORDERINDEX = "DECLARE @OrderIndex INT; DECLARE @MaxOrderIndex INT; SELECT @OrderIndex=OrderIndex FROM lmShop_BankAccounts WHERE BankAccountsId=@BankAccountsId;SELECT @MaxOrderIndex=MAX(OrderIndex) FROM lmShop_BankAccounts;IF @OrderIndex<@MaxOrderIndex BEGIN UPDATE lmShop_BankAccounts SET OrderIndex=OrderIndex-1 WHERE OrderIndex=@OrderIndex+1;UPDATE lmShop_BankAccounts SET OrderIndex=OrderIndex+1 WHERE BankAccountsId=@BankAccountsId; END";
        private const string SQL_SELECT_BANKACCOUNTS_BY_ORDERINDEX = "SELECT MAX(OrderIndex) FROM lmShop_BankAccounts;";
        private const string SQL_SELECT_BANKACCOUNTS_ISFASE = "SELECT BankAccountsId, BankName, PaymentInterfaceId, AccountsName, Accounts, AccountsKey, PaymentType, BankIcon, OrderIndex, [Description], IsUse, IsFinish, IsBacktrack, IsMain, IsDisplay, AccountType FROM lmShop_BankAccounts WHERE BankAccountsId=@BankAccountsId AND PaymentType IN (0,2,3,4);";
        private const string SQL_GET_ACCOUNT_INTERFACE = "select BA.BankAccountsId,PIN.paymentinterfacename from lmShop_BankAccounts  AS BA left join lmShop_PaymentInterface AS PIN ON PIN.paymentinterfaceid = BA.paymentinterfaceid";
        //取某个银行帐户 当前总额
        private const string SQL_GETBANKACCOUNTSID_NONCE = "SELECT NonceBalance FROM lmShop_BankAccountsBalance WHERE BankAccountsId=@BankAccountsId;";

        /// <summary>
        /// 根据公司ID、部门ID、银行ID、职务ID获取相关的权限
        /// Add by liucaijun at 2011-January-30th
        /// </summary>
        private const string SQL_GET_BANK_ACCOUNTS_BY_FILIALE_BRANCH_POSITION_BANK =
            @"SELECT FilialeId, BranchId, PositionId, BankAccountsId FROM BankAccountPermission WHERE BankAccountsId=@bankAccountsId 
            AND FilialeId=@FilialeId AND BranchId=@BranchId AND PositionId=@PositionId";

        #endregion

        #region[参数]
        private const string PARM_BANKACCOUNTSID = "@BankAccountsId";
        private const string PARM_BANKNAME = "@BankName";
        private const string PARM_PAYMENTINTERFACEID = "PaymentInterfaceId";
        private const string PARM_ACCOUNTSNAME = "@AccountsName";
        private const string PARM_ACCOUNTS = "@Accounts";
        private const string PARM_ACCOUNTSKEY = "@AccountsKey";
        private const string PARM_PAYMENTTYPE = "@PaymentType";
        private const String PARM_FILIALE_ID = "@FilialeId";
        private const String PARM_BRANCH_ID = "@BranchId";
        private const String PARM_POSITION_ID = "@PositionId";
        private const string PARM_BANKICON = "@BankIcon";
        private const string PARM_ORDERINDEX = "@OrderIndex";
        private const string PARM_DESCRIPTION = "@Description";
        private const string PARM_ISUSE = "@IsUse";
        private const string PARM_ISFINISH = "@IsFinish";
        private const string PARM_ISBACKTRACK = "@IsBacktrack";
        private const string PARM_ISDISPLAY = "@IsDisplay";
        private const string PARM_ACCOUNTTYPE = "@AccountType";
        #endregion

        #region[添加资金帐号]
        /// <summary>
        /// 添加资金帐号
        /// </summary>
        /// <param name="bankAccounts">帐号实例</param>
        public void Insert(BankAccountInfo bankAccounts)
        {
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                conn.Execute(SQL_INSERT_BANKACCOUNTS, new
                {
                    BankAccountsId = bankAccounts.BankAccountsId,
                    BankName = bankAccounts.BankName,
                    PaymentInterfaceId = bankAccounts.PaymentInterfaceId,
                    AccountsName = bankAccounts.AccountsName,
                    Accounts = bankAccounts.Accounts,
                    AccountsKey = string.IsNullOrEmpty(bankAccounts.AccountsKey) ? (Object)DBNull.Value : bankAccounts.AccountsKey,
                    PaymentType = bankAccounts.PaymentType,
                    BankIcon = bankAccounts.BankIcon,
                    Description = bankAccounts.Description,
                    IsUse = bankAccounts.IsUse,
                    IsFinish = bankAccounts.IsFinish,
                    IsBacktrack = bankAccounts.IsBacktrack,
                    IsDisplay = bankAccounts.IsDisplay,
                    AccountType = bankAccounts.AccountType,
                });
            }
        }
        #endregion

        #region[更新资金帐号]
        /// <summary>
        /// 更新资金帐号
        /// </summary>
        /// <param name="bankAccounts">帐号实例</param>
        public void Update(BankAccountInfo bankAccounts)
        {
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                conn.Execute(SQL_UPDATE_BANKACCOUNTS, new
                {
                    BankAccountsId = bankAccounts.BankAccountsId,
                    BankName = bankAccounts.BankName,
                    PaymentInterfaceId = bankAccounts.PaymentInterfaceId,
                    AccountsName = bankAccounts.AccountsName,
                    Accounts = bankAccounts.Accounts,
                    AccountsKey = string.IsNullOrEmpty(bankAccounts.AccountsKey) ? (Object)DBNull.Value : bankAccounts.AccountsKey,
                    PaymentType = bankAccounts.PaymentType,
                    BankIcon = bankAccounts.BankIcon,
                    Description = bankAccounts.Description,
                    IsUse = bankAccounts.IsUse,
                    IsFinish = bankAccounts.IsFinish,
                    IsBacktrack = bankAccounts.IsBacktrack,
                    IsDisplay = bankAccounts.IsDisplay,
                    AccountType = bankAccounts.AccountType,
                });
            }
        }
        #endregion

        #region[删除资金帐号]
        /// <summary>
        /// 删除资金帐号
        /// </summary>
        /// <param name="bankAccountsId">付款帐号Id</param>
        public void Delete(Guid bankAccountsId)
        {
            var parm = new SqlParameter(PARM_BANKACCOUNTSID, SqlDbType.UniqueIdentifier) { Value = bankAccountsId };
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {

                        conn.Execute(SQL_DELETE_BANKACCOUNTS, new
                        {
                            BankAccountsId = bankAccountsId,
                        });
                        trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        throw ex;
                    }
                }
            }
        }
        #endregion

        #region[取得某银行当前金额]
        /// <summary>
        /// 取得某银行当前金额
        /// </summary>
        /// <param name="bankAccountsId">银行ID</param>
        /// <returns></returns>
        public double GetBankAccountsNonce(Guid bankAccountsId)
        {
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.ExecuteScalar<double>(SQL_GETBANKACCOUNTSID_NONCE, new
                {
                    BankAccountsId = bankAccountsId,
                });
            }
        }
        #endregion

        #region[获取资金帐号]
        /// <summary>
        /// 获取资金帐号
        /// </summary>
        /// <param name="bankAccountsId">付款帐号Id</param>
        /// <returns></returns>
        public BankAccountInfo GetBankAccounts(Guid bankAccountsId)
        {
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                return conn.QueryFirstOrDefault<BankAccountInfo>(SQL_SELECT_BANKACCOUNTS, new
                {
                    BankAccountsId = bankAccountsId,
                });
            }
        }
        #endregion

        #region[根据公司部门职务获取资金帐号列表]
        /// <summary>
        /// 根据公司部门职务获取资金帐号列表
        /// </summary>
        /// <returns></returns>
        public IList<BankAccountInfo> GetBankAccountsList(Guid filialeId, Guid branchId, Guid positionId)
        {
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.Query<BankAccountInfo>(SQL_SELECT_BANKACCOUNTS_LIST, new
                {
                    FilialeId = filialeId,
                    BranchId = branchId,
                    PositionId = positionId
                }).AsList();
            }
        }
        #endregion

        #region[获取资金帐号列表]
        /// <summary>
        /// 获取资金帐号列表
        /// </summary>
        /// <returns></returns>
        public IList<BankAccountInfo> GetBankAccountsList()
        {
            const string SQL = @"
SELECT BA.[BankAccountsId]
      ,BAB.TargetId AS TargetId
      ,[BankName]
      ,[PaymentInterfaceId]
      ,[AccountsName]
      ,[Accounts]
      ,[AccountsKey]
      ,[PaymentType]
      ,[BankIcon]
      ,[OrderIndex]
      ,[Description]
      ,[IsUse]
      ,[IsFinish]
      ,[IsBacktrack]
      ,[IsMain]
      ,[IsDisplay]
      ,[AccountType]
  FROM [lmShop_BankAccounts] BA
LEFT JOIN BankAccountBinding BAB ON BAB.BankAccountsId = BA.BankAccountsId
";
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.Query<BankAccountInfo>(SQL).AsList();
            }
        }
        #endregion

        /// <summary>
        /// 获取资金帐号列表不关联BankAccountBinding
        /// </summary>
        /// <returns></returns>
        public IList<BankAccountInfo> GetList()
        {
            const string SQL = @"
SELECT BA.[BankAccountsId]
      ,[BankName]
      ,[PaymentInterfaceId]
      ,[AccountsName]
      ,[Accounts]
      ,[AccountsKey]
      ,[PaymentType]
      ,[BankIcon]
      ,[OrderIndex]
      ,[Description]
      ,[IsUse]
      ,[IsFinish],[IsBacktrack],[IsMain],[IsDisplay],[AccountType]
  FROM [lmShop_BankAccounts] BA
  order by OrderIndex asc
";
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.Query<BankAccountInfo>(SQL).AsList();
            }
        }

        #region[获取资金帐号列表]
        /// <summary>
        /// 获取资金帐号列表
        /// </summary>
        /// <returns></returns>
        public IList<BankAccountInfo> GetBankAccountsNoBindingList()
        {
            const string SQL = @"
SELECT [BankAccountsId]
      ,[BankName]
      ,[PaymentInterfaceId]
      ,[AccountsName]
      ,[Accounts]
      ,[AccountsKey]
      ,[PaymentType]
      ,[BankIcon]
      ,[OrderIndex]
      ,[Description]
      ,[IsUse]
      ,[IsFinish],[IsBacktrack],[IsMain],[IsDisplay],[AccountType]
  FROM [lmShop_BankAccounts]
WHERE BankAccountsId NOT IN (SELECT BankAccountsId FROM BankAccountBinding)
";
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.Query<BankAccountInfo>(SQL).AsList();
            }
        }
        #endregion

        #region[获取指定支付类型的帐号]
        /// <summary>
        /// 获取指定支付类型的帐号
        /// </summary>
        /// <returns></returns>
        public IList<BankAccountInfo> GetBankAccountsList(PaymentType paymentType)
        {
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.Query<BankAccountInfo>(SQL_SELECT_BANKACCOUNTS_LIST_BY_PAYMENTTYPE, new
                {
                    PaymentType = (int)paymentType
                }).AsList();
            }
        }
        #endregion

        #region[获取指定支付类型组的帐号]
        /// <summary>
        /// 获取指定支付类型组的帐号
        /// </summary>
        /// <returns></returns>
        public IList<BankAccountInfo> GetBankAccountsList(PaymentType[] paymentTypes)
        {
            int numPaymentType = paymentTypes.Length;

            string payTypes = string.Empty;
            //生成执行字符串
            for (int i = 0; i < numPaymentType; i++)
            {
                if (i < (numPaymentType - 1))
                    payTypes += ((int)paymentTypes[i]) + ",";
                else
                    payTypes += ((int)paymentTypes[i]).ToString(CultureInfo.InvariantCulture);
            }
            var sql = string.Format(SQL_SELECT_BANKACCOUNTS_LIST_BY_PAYMENTTYPES, "(" + payTypes + ")");

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.Query<BankAccountInfo>(sql).AsList();
            }
        }
        #endregion

        #region[是否在前台陈列范围内]
        /// <summary>
        /// 是否在前台陈列范围内
        /// </summary>
        /// <param name="bankAccountsId">银行编号</param>
        /// <returns></returns>
        public bool IsFace(Guid bankAccountsId)
        {
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.Query(SQL_SELECT_BANKACCOUNTS_ISFASE, new
                {
                    BankAccountsId = bankAccountsId
                }).Any();
            }
        }
        #endregion

        #region[获取当前顺序号]
        /// <summary>
        /// 获取当前顺序号
        /// </summary>
        /// <returns></returns>
        public int GetOrderIndex()
        {
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.ExecuteScalar<int>(SQL_SELECT_BANKACCOUNTS_BY_ORDERINDEX) + 1;
            }
        }
        #endregion

        /// <summary>
        /// 更新银行帐号序号
        /// </summary>
        /// <param name="bankAccountsId">银行帐号ID</param>
        /// <param name="orderIndex">序号</param>
        /// <returns></returns>
        /// zal 2015-09-22
        public bool UpdateOrderIndex(Guid bankAccountsId, int orderIndex)
        {
            const string SQL = @"
            declare @index int,@Total int
	        select @index=OrderIndex from lmShop_BankAccounts 
	        where BankAccountsId=@BankAccountsId
            select @Total=COUNT(*) from lmShop_BankAccounts
	
            if(@OrderIndex>@Total)
		        begin
		            set @OrderIndex=@Total
		        end
	
	        begin tran
	
	        if(@OrderIndex<@index)
		        begin
			        --从大序号变到小序号(例如从3到1)
			        update lmShop_BankAccounts
			        set OrderIndex=OrderIndex+1
			        where OrderIndex between @OrderIndex and @index
		        end
	        else
		        begin
			        --从小序号变到大序号(例如从1到3)
			        update lmShop_BankAccounts
			        set OrderIndex=OrderIndex-1
			        where OrderIndex between @index and @OrderIndex
		        end
		
	        update lmShop_BankAccounts
	        set OrderIndex=@OrderIndex
	        where BankAccountsId=@BankAccountsId
    
            if(@@ERROR!=0)
	        rollback
	        else
	        commit";

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                return conn.Execute(SQL, new
                {
                    BankAccountsId = bankAccountsId,
                    OrderIndex = orderIndex,
                }) > 0;
            }
        }



        #region[得到银行以及其接口名字]
        /// <summary>
        /// Func : 得到银行以及其接口名字
        /// Coder: dyy
        /// Date : 2010 Jan.4th
        /// </summary>
        /// <returns></returns>
        public Dictionary<Guid, String> GetBankInterface()
        {
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.Query(SQL_GET_ACCOUNT_INTERFACE).ToDictionary(kv => (Guid)kv.BankAccountsId, kv => (string)kv.paymentinterfacename);
            }
        }
        #endregion

        #region 资金流权限查询
        /// <summary>
        /// 资金流权限查询
        /// </summary>
        /// <param name="filialeId"></param>
        /// <param name="branchId"></param>
        /// <param name="postionId"></param>
        /// <param name="bankAccountsId"></param>
        /// <param name="filialeBankAccoutid"></param>
        /// <returns></returns>
        public IList<BankAccountPermissionInfo> GetBankPersionByBankId(Guid filialeId, Guid branchId, Guid postionId, Guid bankAccountsId, Guid filialeBankAccoutid)
        {
            string sqlbankAccountsinfo =
       @"  Select distinct b.BankAccountsId,BankName,p.FilialeId,p.BranchId,p.PositionId
 from BankAccountPermission p 
  left join lmShop_BankAccounts b on b.BankAccountsId=p.BankAccountsId  
  left join BankAccountBinding c on c.BankAccountsId=b.BankAccountsId ";

            if (filialeId != Guid.Empty)
            {
                sqlbankAccountsinfo += " and p.FilialeId=@FilialeId";
            }
            if (branchId != Guid.Empty)
            {
                sqlbankAccountsinfo += " and p.BranchId=@BranchId";
            }
            if (postionId != Guid.Empty)
            {
                sqlbankAccountsinfo += " and p.PositionId=@PositionId ";
            }
            if (filialeBankAccoutid != Guid.Empty)
            {
                sqlbankAccountsinfo += " and c.FilialeId=@filialeBankAccoutid ";
            }
            if (bankAccountsId != Guid.Empty)
            {
                sqlbankAccountsinfo += " where p.BankAccountsId=@BankAccountsId";
            }
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.Query<BankAccountPermissionInfo>(sqlbankAccountsinfo, new
                {
                    BankAccountsId = bankAccountsId,
                    FilialeId = filialeId,
                    BranchId = branchId,
                    PositionId = postionId,
                    filialeBankAccoutid = filialeBankAccoutid,
                }).AsList();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bankAccountsId"></param>
        /// <param name="filialeId"></param>
        /// <param name="branchId"></param>
        /// <param name="postionId"></param>
        public void DeleteBankPersion(Guid bankAccountsId, Guid filialeId, Guid branchId, Guid postionId)
        {
            const string SQL_BANK_PERSION_DELETE = @"delete BankAccountPermission where bankAccountsId=@bankAccountsId and FilialeId=@FilialeId and BranchId=@BranchId and PositionId=@PositionId";

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        conn.Execute(SQL_BANK_PERSION_DELETE, new
                        {
                            BankAccountsId = bankAccountsId,
                            FilialeId = filialeId,
                            BranchId = branchId,
                            PositionId = postionId,
                        });
                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }

        public void AddBankPersion(Guid bankAccountsId, Guid filialeId, Guid branchId, Guid postionId)
        {
            const string SQL_BANK_PERSION_ADD = @"insert BankAccountPermission(bankAccountsId,FilialeId,BranchId,PositionId) values(@bankAccountsId,@FilialeId,@BranchId,@PositionId)";

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                conn.Execute(SQL_BANK_PERSION_ADD, new
                {
                    BankAccountsId = bankAccountsId,
                    FilialeId = filialeId,
                    BranchId = branchId,
                    PositionId = postionId,
                });
            }
        }
        #endregion

        public IEnumerable<BankAccountPermissionInfo> GetPermissionList(Guid bankAccountId)
        {
            const string SQL = @"
WITH TP AS
(
	SELECT BAP.FilialeId,BAP.BranchId,BAP.PositionId,BA.BankAccountsId,BA.BankName
	FROM lmShop_BankAccounts BA
	--INNER JOIN BankAccountBinding BAB ON BAB.BankAccountsId = BA.BankAccountsId 
	INNER JOIN BankAccountPermission BAP on BA.BankAccountsId=BAP.BankAccountsId
)
SELECT FilialeId,BranchId,PositionId,BankAccountsId,BankName FROM TP WHERE 1=1 AND BankAccountsId=@BankAccountsId
";
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.Query<BankAccountPermissionInfo>(SQL, new
                {
                    BankAccountsId = bankAccountId,
                }).AsList();
            }
        }

        #region[根据银行账号删除银行账号时删除该账号相关的所有权限]
        /// <summary>
        /// Add by Liucaijun at 2010-january-07th
        /// 删除银行账号时使用,删除该账号相关的所有权限
        /// </summary>
        public void DeleteBankPersion(Guid bankAccountsId)
        {
            const string SQL_BANK_PERSION_DELETE = @"DELETE BankAccountPermission WHERE bankAccountsId=@bankAccountsId";

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                conn.Execute(SQL_BANK_PERSION_DELETE, new
                {
                    BankAccountsId = bankAccountsId,
                });
            }
        }
        #endregion

        #region[根据公司ID、部门ID、银行ID、职务ID获取相关的权限]
        /// <summary>
        /// 根据公司ID、部门ID、银行ID、职务ID获取相关的权限
        /// Add by liucaijun at 2011-January-30th
        /// </summary>
        /// <returns></returns>
        public BankAccountPermissionInfo GetPersonnelBankAccountsList(Guid bankId, Guid filialeId, Guid branchId, Guid positionId)
        {
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.QueryFirstOrDefault<BankAccountPermissionInfo>(SQL_GET_BANK_ACCOUNTS_BY_FILIALE_BRANCH_POSITION_BANK, new
                {
                    BankAccountsId = bankId,
                    FilialeId = filialeId,
                    BranchId = branchId,
                    PositionId = positionId,
                });
            }
        }
        #endregion

        #region -- 插入公司银行账户绑定关系
        /// <summary>
        /// 插入公司银行账户绑定关系
        /// </summary>
        /// <returns></returns>
        public bool InsertBindBankAccounts(Guid filialeId, Guid bankAccountsId)
        {
            const string SQL = @"
IF (NOT EXISTS (SELECT TOP 1 TargetId,BankAccountsId FROM [BankAccountBinding] WHERE TargetId=@TargetId AND BankAccountsId=@BankAccountsId))
    BEGIN
        INSERT INTO [BankAccountBinding] (TargetId,BankAccountsId) VALUES (@TargetId,@BankAccountsId)
    END
";
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                return conn.Execute(SQL, new
                {
                    TargetId = filialeId,
                    BankAccountsId = bankAccountsId,
                }) > 0;
            }
        }

        public IList<BankAccountBalanceInfo> GetBalanceList()
        {
            const string SQL = @"
SELECT bb.TargetId,bab.[BankAccountsId] AS BankAccountId,[NonceBalance]
  FROM [lmshop_BankAccountsBalance] bab
INNER JOIN dbo.BankAccountBinding bb ON bb.BankAccountsId = bab.BankAccountsId
";
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.Query<BankAccountBalanceInfo>(SQL).AsList();
            }
        }

        IList<BankAccountInfo> IBankAccounts.GetBankAccountsList(Guid targetId, Guid filialeId, Guid branchId, Guid positionId)
        {
            return GetBankAccountsList(targetId, filialeId, branchId, positionId);
        }

        /// <summary>设置银行账号是否是主账号
        /// </summary>
        /// <param name="bankAccountsId">银行账号ID</param>
        /// <param name="isMain">是否主账号</param>
        /// <returns></returns>
        public bool SetBankAccountsIsMain(Guid bankAccountsId, bool isMain)
        {
            const string SQL = @"
UPDATE [lmShop_BankAccounts]
   SET [IsMain] =@IsMain
 WHERE BankAccountsId=@BankAccountsId
";
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                return conn.Execute(SQL, new
                {
                    IsMain = isMain,
                    BankAccountsId = bankAccountsId,
                }) > 0;
            }
        }

        /// <summary>获取所有非主帐号信息含账户余额（资金流页面用） ADD 2015-03-03  陈重文
        /// </summary>
        /// <returns></returns>
        public IList<BankAccountInfo> GetBankAccountsListByNotIsMain()
        {
            const string SQL = @"
SELECT B.BankAccountsId,BankName,AccountsName,IsUse,NonceBalance FROM lmShop_BankAccounts AS B 

INNER JOIN [lmshop_BankAccountsBalance] AS BAB ON B.BankAccountsId=BAB.BankAccountsId

WHERE IsMain=0 ORDER BY B.OrderIndex ";

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.Query<BankAccountInfo>(SQL).AsList();
            }
        }

        #endregion

        #region[获取绑定的银行帐号]
        /// <summary>
        /// 获取绑定的银行帐号
        /// Add by liucaijun at 2011-August-10th
        /// </summary>
        /// <returns></returns>
        public IList<BankAccountInfo> GetBindBankAccounts()
        {
            const string SQL = @"
SELECT 
	BA.BankAccountsId,BankName,PaymentInterfaceId,AccountsName,Accounts,
	AccountsKey,PaymentType,BankIcon,OrderIndex,Description,
	IsUse,IsFinish,IsBacktrack,[IsMain],[IsDisplay]
FROM lmShop_BankAccounts BA
LEFT JOIN (select BankAccountsId from BankAccountBinding group by BankAccountsId)BAB ON BAB.BankAccountsId = BA.BankAccountsId
WHERE BA.BankAccountsId IN (
	SELECT AccountsId FROM (
		SELECT InvoiceAccountsId AS AccountsId FROM CostCompany 
		  WHERE InvoiceAccountsId IS NOT NULL AND InvoiceAccountsId!='00000000-0000-0000-0000-000000000000' GROUP BY InvoiceAccountsId
		UNION
		SELECT VoucherAccountsId AS AccountsId FROM CostCompany 
		  WHERE VoucherAccountsId IS NOT NULL AND VoucherAccountsId!='00000000-0000-0000-0000-000000000000' GROUP BY VoucherAccountsId
		UNION
		SELECT CashAccountsId AS AccountsId FROM CostCompany 
		  WHERE CashAccountsId IS NOT NULL AND CashAccountsId!='00000000-0000-0000-0000-000000000000' GROUP BY CashAccountsId
		UNION
		SELECT NoVoucherAccountsId AS AccountsId 
		FROM CostCompany 
		  WHERE NoVoucherAccountsId IS NOT NULL AND NoVoucherAccountsId!='00000000-0000-0000-0000-000000000000' GROUP BY NoVoucherAccountsId
	) a GROUP BY a.AccountsId
)";
            //SQL_SELECT_BIND_BANKACCOUNT

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.Query<BankAccountInfo>(SQL).AsList();
            }
        }
        #endregion

        /// <summary>根据订单销售公司或平台ID获取对应有权限的银行账号列表
        /// </summary>
        /// <param name="targetId">公司或平台ID</param>
        /// <param name="filialeId"></param>
        /// <param name="branchId"></param>
        /// <param name="positionId"></param>
        /// <returns></returns>
        public IList<BankAccountInfo> GetBankAccountsList(Guid targetId, Guid filialeId, Guid branchId, Guid positionId)
        {
            const string SQL = @"SELECT b.BankName,b.Accounts,b.AccountsName,a.BankAccountsId,TargetId,b.IsUse,b.PaymentType,b.IsMain,b.IsDisplay  
FROM BankAccountBinding AS a
INNER JOIN lmShop_BankAccounts AS b ON a.BankAccountsId=b.BankAccountsId AND b.IsUse=1
INNER JOIN BankAccountPermission AS c ON c.BankAccountsId=a.BankAccountsId
WHERE 
TargetId=@TargetId
AND FilialeId=@FilialeId
AND BranchId=@BranchId
AND PositionId=@PositionId
 ORDER BY b.OrderIndex ";


            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.Query<BankAccountInfo>(SQL, new
                {
                    TargetId = targetId,
                    FilialeId = filialeId,
                    BranchId = branchId,
                    PositionId = positionId,
                }).AsList();
            }
        }

        /// <summary>
        /// 根据店铺获取帐户(用于联盟店往来单位收付款)
        /// modify by lcj at 2015.11.4
        /// 取消 B.IsMain=0条件
        /// </summary>
        /// <returns></returns>
        public IList<ShopBankAccountsInfo> GetBankAccountsByShopId()
        {
            const string SQL =
                @"select B.BankAccountsId,B.BankName,BB.NonceBalance from lmShop_BankAccounts B 
left join lmshop_BankAccountsBalance BB ON B.BankAccountsId=BB.BankAccountsId
WHERE B.IsUse=1  GROUP BY B.BankAccountsId,B.BankName,NonceBalance ";

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.Query<ShopBankAccountsInfo>(SQL).AsList();
            }
        }

        /// <summary>获取有权限的非主帐号信息含账户余额 2015-05-05  陈重文
        /// </summary>
        /// <param name="filialeId">公司ID</param>
        /// <param name="branchId">部门ID</param>
        /// <param name="positionId">职务ID</param>
        /// <returns></returns>
        public IList<BankAccountInfo> GetBankAccountsListByNotIsMain(Guid filialeId, Guid branchId, Guid positionId)
        {
            const string sql = @"SELECT B.BankAccountsId,BankName,AccountsName,IsUse,NonceBalance FROM lmShop_BankAccounts AS B 
INNER JOIN [lmshop_BankAccountsBalance] AS BAB ON B.BankAccountsId=BAB.BankAccountsId
INNER JOIN BankAccountPermission AS BP ON B.BankAccountsId=BP.BankAccountsId
WHERE IsMain=0
AND FilialeId=@FilialeId
and BranchId=@BranchId
and PositionId=@PositionId
ORDER BY OrderIndex ASC";

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.Query<BankAccountInfo>(sql, new
                {
                    FilialeId = filialeId,
                    BranchId = branchId,
                    PositionId = positionId
                }).AsList();
            }
        }

        /// <summary>获取所有银行帐号的余额  2015-05-15  陈重文
        /// </summary>
        /// <returns></returns>
        public double GetBankAccountsAllNonceBalance()
        {
            const string SQL = "SELECT SUM(NonceBalance) FROM [lmshop_BankAccountsBalance]";

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.ExecuteScalar<double>(SQL);
            }
        }
    }
}
