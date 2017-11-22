using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Dapper;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Environment;
using ERP.Model;
using Keede.DAL.Helper;
using Keede.DAL.RWSplitting;
using Keede.Ecsoft.Model;

namespace ERP.DAL.Implement.Inventory
{
    /// <summary>▄︻┻┳═一 资金流数据层  最后修改提交  陈重文  2014-12-25  （全局新公司体系SQL更新优化，去除无用方法）
    /// </summary>
    public class WasteBook : IWasteBook
    {
        public WasteBook(Environment.GlobalConfig.DB.FromType fromType)
        {

        }

        #region [新增资金流]

        /// <summary>新增资金流
        /// </summary>
        /// <param name="wasteBook">资金流</param>
        public int Insert(WasteBookInfo wasteBook)
        {
            const string SQL_INSERT_WASTEBOOK = @"
BEGIN TRAN
DECLARE @TheBalance DECIMAL(18,4) 
	IF NOT EXISTS(SELECT NonceBalance FROM lmShop_BankAccountsBalance WITH(NOLOCK) WHERE BankAccountsId=@BankAccountsId)
			BEGIN 
				INSERT INTO lmShop_BankAccountsBalance VALUES (@BankAccountsId,0)
			END
	IF @AuditingState=1
	BEGIN
		UPDATE lmShop_BankAccountsBalance SET @TheBalance=NonceBalance=NonceBalance+@Income WHERE BankAccountsId=@BankAccountsId;
		IF EXISTS (SELECT WasteBookId FROM lmShop_WasteBook WITH(NOLOCK) WHERE TradeCode=@TradeCode AND Income=@Income)
		BEGIN
			SELECT 1
		END  ELSE
		BEGIN
		    INSERT INTO lmShop_WasteBook 
				    (WasteBookId,BankAccountsId,TradeCode,DateCreated,[Description],Income,NonceBalance,AuditingState,WasteBookType,SaleFilialeId,LinkTradeType,IsOut,BankTradeCode,LinkTradeCode,[State],WasteSource) 		
		    VALUES  (@WasteBookId,@BankAccountsId,@TradeCode,GETDATE(),@Description,@Income,@TheBalance,@AuditingState,@WasteBookType,@SaleFilialeId,@LinkTradeType,@IsOut,@BankTradeCode,@LinkTradeCode,@State,@WasteSource);
		END	
	END
	ELSE
	BEGIN
		SELECT @TheBalance=NonceBalance+@Income FROM lmShop_BankAccountsBalance 
        WHERE BankAccountsId=@BankAccountsId;
		INSERT INTO lmShop_WasteBook 
				(WasteBookId,BankAccountsId,TradeCode,DateCreated,[Description],Income,NonceBalance,AuditingState,WasteBookType,SaleFilialeId,LinkTradeType,IsOut,BankTradeCode,LinkTradeCode,[State],WasteSource) 		
		VALUES  (@WasteBookId,@BankAccountsId,@TradeCode,GETDATE(),@Description,@Income,@TheBalance,@AuditingState,@WasteBookType,@SaleFilialeId,@LinkTradeType,@IsOut,@BankTradeCode,@LinkTradeCode,@State,@WasteSource);
	END
IF @@ERROR>0
	BEGIN
		ROLLBACK
	END
ELSE
	BEGIN
		COMMIT
	END ";

            var parms = new[] {
                new Parameter("@WasteBookId",  wasteBook.WasteBookId),
                new Parameter("@BankAccountsId", wasteBook.BankAccountsId),
                new Parameter("@TradeCode", wasteBook.TradeCode),
                new Parameter("@DateCreated", DateTime.Now),
                new Parameter("@Description",wasteBook.Description),
                new Parameter("@Income", Math.Round(wasteBook.Income, 2)),
                new Parameter("@AuditingState",wasteBook.AuditingState),
                new Parameter("@WasteBookType",wasteBook.WasteBookType),
                new Parameter("@SaleFilialeId",wasteBook.SaleFilialeId),
                new Parameter("@LinkTradeCode",wasteBook.LinkTradeCode),
                new Parameter("@LinkTradeType",wasteBook.LinkTradeType),
                new Parameter("@BankTradeCode",wasteBook.BankTradeCode),
                new Parameter("@State",wasteBook.State),
                new Parameter("@IsOut",wasteBook.IsOut),
                new Parameter("@WasteSource",wasteBook.WasteSource)
            };

            const string CHECK_SQL = @"SELECT WasteBookId FROM lmShop_WasteBook WITH(NOLOCK) WHERE WasteBookId =@WasteBookId";
            const string CHECK_SQL_2 = @"SELECT COUNT(0) FROM lmShop_WasteBook WITH(NOLOCK) WHERE TradeCode=@TradeCode AND Income=@Income";
            using (var db = DatabaseFactory.Create())
            {
                var count = db.GetValue<int>(true, CHECK_SQL_2, new Parameter("TradeCode", wasteBook.TradeCode), new Parameter("Income", wasteBook.Income));
                if (count != 0)
                {
                    SAL.LogCenter.LogService.LogError(string.Format("资金流重复推送,wasteBook={0}", new Framework.Core.Serialize.JsonSerializer().Serialize(wasteBook)), "仓库管理");
                }
                var wid = db.GetValue<Guid>(true, CHECK_SQL, new Parameter("WasteBookId", wasteBook.WasteBookId));
                if (wid == wasteBook.WasteBookId)
                {
                    return -1;
                }
                if (wasteBook.LinkTradeType == (UInt32)WasteBookLinkTradeType.CompanyFundReceipt)
                {
                    var checkSql3 = String.Format(@"SELECT COUNT(0) FROM lmShop_WasteBook WITH(NOLOCK) WHERE BankAccountsId='{0}' and Income='{1}' and LinkTradeCode='{2}' ", wasteBook.BankAccountsId, wasteBook.Income, wasteBook.LinkTradeCode);
                    var record = db.GetValue<Int32>(true, checkSql3);
                    if (record >= 1)
                    {
                        return -1;
                    }
                }
                return db.Execute(false, SQL_INSERT_WASTEBOOK, parms) ? 1 : 0;
            }
        }

        #endregion

        #region [更新审核资金流]

        ///<summary>更新资金流信息
        /// </summary>
        /// <param name="wasteBook">资金流</param>
        public void Update(WasteBookInfo wasteBook)
        {
            const string SQL_UPDATE = @"
DECLARE @NonceBalance decimal(18,4);
IF  @BankAccountsId='00000000-0000-0000-0000-000000000000'
BEGIN
UPDATE lmShop_WasteBook SET DateCreated=@DateCreated,[Description]=[Description]+@Description,Income=@Income WHERE WasteBookId=@WasteBookId 
END
ELSE
BEGIN
IF NOT EXISTS(SELECT NonceBalance FROM lmShop_BankAccountsBalance WITH(NOLOCK) WHERE BankAccountsId=@BankAccountsId)
BEGIN 
	INSERT INTO lmShop_BankAccountsBalance VALUES (@BankAccountsId,0)
END
SELECT @NonceBalance=NonceBalance FROM lmShop_BankAccountsBalance WHERE BankAccountsId=@BankAccountsId;
UPDATE lmShop_WasteBook SET DateCreated=@DateCreated,[Description]=[Description]+@Description,Income=@Income,BankAccountsId=@BankAccountsId WHERE WasteBookId=@WasteBookId 
END";
            var parms = new[]
            {
                new SqlParameter("@DateCreated", SqlDbType.DateTime){Value = wasteBook.DateCreated},
                new SqlParameter("@Description", SqlDbType.VarChar, 256){Value = wasteBook.Description},
                new SqlParameter("@Income", SqlDbType.Float){Value = Math.Round(wasteBook.Income, 2)},
                new SqlParameter("@WasteBookId", SqlDbType.UniqueIdentifier){Value =wasteBook.WasteBookId },
                new SqlParameter("@BankAccountsId",SqlDbType.UniqueIdentifier){Value = wasteBook.BankAccountsId}
            };
            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL_UPDATE, parms);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>更新DateCreated字段
        /// </summary>
        /// <param name="tradeCode">单据编号</param>
        public void UpdateDateTime(string tradeCode)
        {
            const string SQL_UPDATE_DATETIME = "UPDATE lmShop_WasteBook SET DateCreated=GETDATE() WHERE TradeCode=@TradeCode;";
            var parms = new[] { new SqlParameter("@TradeCode", SqlDbType.VarChar, 32) { Value = tradeCode } };
            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL_UPDATE_DATETIME, parms);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>更新资金流描述
        /// </summary>
        /// <param name="wastebookId">资金流单据ID</param>
        /// <param name="description">描述</param>
        public void UpdateDescription(Guid wastebookId, string description)
        {
            const string SQL_UPDATE_DESCRIPTION = "UPDATE [lmShop_WasteBook] SET [Description] = [Description] + @Description  WHERE [WasteBookId] = @WasteBookId";
            var prms = new[]
            {
                new SqlParameter("@WasteBookId",SqlDbType.UniqueIdentifier){Value = wastebookId},
                new SqlParameter("@Description",SqlDbType.VarChar,256){Value = description}
            };
            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL_UPDATE_DESCRIPTION, prms);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary> 资金流审核时添加审核人描述信息
        /// </summary>
        /// <param name="tradeCode">单据编号</param>
        /// <param name="description">描述</param>
        public void UpdateDescriptionForAuditing(string tradeCode, string description)
        {
            const string SQL_UPDATE_DESCRIPTION_FOR_AUDITING = "UPDATE lmShop_WasteBook SET Description=Description+@Description WHERE TradeCode=@TradeCode;";
            var parms = new[]
            {
                new SqlParameter("@TradeCode", SqlDbType.VarChar, 32),
                new SqlParameter("@Description", SqlDbType.VarChar, 256)
            };
            parms[0].Value = tradeCode;
            parms[1].Value = description;
            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL_UPDATE_DESCRIPTION_FOR_AUDITING, parms);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>更新资金流手续费
        /// </summary>
        /// <param name="tradeCode">单据编号</param>
        /// <param name="poundage">手续费</param>
        /// <param name="dateTime">时间</param>
        public void UpdatePoundage(string tradeCode, decimal poundage, DateTime dateTime)
        {
            const string SQL_UPDATE_POUNDAGE = @"  
            declare @MinData decimal(18,4),@MaxData decimal(18,4)
            SELECT @MinData=MIN(Income) FROM lmShop_WasteBook WHERE TradeCode=@TradeCode
            SELECT @MaxData=MAX(Income) FROM lmShop_WasteBook WHERE TradeCode=@TradeCode

            UPDATE lmShop_WasteBook SET Income=@Income,DateCreated=GETDATE() 
            WHERE TradeCode=@TradeCode 
            AND Income > @MinData 
            AND Income < @MaxData";
            var parms = new[]{
                new SqlParameter("@TradeCode", SqlDbType.VarChar, 32){Value = tradeCode},
                new SqlParameter("@Income", SqlDbType.Decimal){Value = poundage}
            };
            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL_UPDATE_POUNDAGE, parms);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>更改资金流手续费
        /// </summary>
        /// <param name="wastebookId">资金流ID</param>
        /// <param name="dateCreated">时间</param>
        /// <param name="poundage">手续费</param>
        public void UpdatePoundageForReckoning(Guid wastebookId, DateTime dateCreated, decimal poundage)
        {
            const string SQL_UPDATE_POUNDAGE_RECKONING = "UPDATE lmShop_WasteBook SET DateCreated=GETDATE(),Income=@Income WHERE WasteBookId=@WasteBookId;";
            var parms = new[]{
                new SqlParameter("@WasteBookId", SqlDbType.UniqueIdentifier){Value = wastebookId},
                new SqlParameter("@Income", SqlDbType.Decimal){Value = poundage}
            };
            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL_UPDATE_POUNDAGE_RECKONING, parms);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary> 修改费用记录时修改资金流
        /// </summary>
        /// <param name="tradeCode">单据ID</param>
        /// <param name="accountReceivable">金额</param>
        /// <param name="description">描述</param>
        /// <param name="dateTime">时间</param>
        public void UpdateForReckoningCost(string tradeCode, double accountReceivable, string description, DateTime dateTime)
        {
            const string SQL_UPDATE_FOR_RECKONINGCOST = "UPDATE lmShop_WasteBook SET Income=@Income,Description=@Description,DateCreated=GETDATE() WHERE TradeCode=@TradeCode";
            var parms = new[]
            {
                new SqlParameter("@TradeCode", SqlDbType.VarChar, 32){Value = tradeCode},
                new SqlParameter("@Income", SqlDbType.Float){Value = accountReceivable},
                new SqlParameter("@Description", SqlDbType.VarChar, 256){Value = description}
            };
            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL_UPDATE_FOR_RECKONINGCOST, parms);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary> 更新资金流 银行账号ID
        /// </summary>
        /// <param name="wasteBookId">资金流ID</param>
        /// <param name="bankAccountsId">银行账号ID</param>
        public void UpdateBankAccountsId(Guid wasteBookId, Guid bankAccountsId)
        {
            const string SQL_UPDATE_BANKACCOUNTSID = "UPDATE lmShop_WasteBook SET BankAccountsId = @BankAccountsId WHERE WasteBookId = @WasteBookId;";

            var parms = new[] {
                new SqlParameter("@WasteBookId", SqlDbType.UniqueIdentifier){Value = wasteBookId},
                new SqlParameter("@BankAccountsId", SqlDbType.UniqueIdentifier){Value = bankAccountsId}
            };
            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL_UPDATE_BANKACCOUNTSID, parms);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        ///<summary>审核资金流
        /// </summary>
        /// <param name="tradeCode">账单编号</param>
        public void Auditing(string tradeCode)
        {
            const string SQL_AUDITING = @"
BEGIN TRAN   
declare @rowCount int;
declare @WasteBookId uniqueidentifier;
declare @BankAccountsId uniqueidentifier;
declare @Income decimal(18,4);
declare @NonceBalance decimal(18,4);
declare @tmpTbl table(WasteBookId uniqueidentifier,BankAccountsId uniqueidentifier,Income decimal(18,4))
insert @tmpTbl(WasteBookId,BankAccountsId,Income) 
select WasteBookId,BankAccountsId,Income from lmShop_WasteBook 
where TradeCode=@TradeCode AND AuditingState <> 1 
ORDER BY ABS(Income) DESC;
select @rowCount=count(WasteBookId) from @tmpTbl;
while(@rowCount>0)
begin
  select top 1 @WasteBookId=WasteBookId,@BankAccountsId=BankAccountsId,@Income=Income from @tmpTbl;
  IF NOT EXISTS(SELECT NonceBalance FROM lmShop_BankAccountsBalance WITH(NOLOCK) WHERE BankAccountsId=@BankAccountsId)
	    BEGIN 
		    INSERT INTO lmShop_BankAccountsBalance VALUES (@BankAccountsId,0)
	    END
  SELECT @NonceBalance=NonceBalance+@Income FROM lmShop_BankAccountsBalance WHERE BankAccountsId=@BankAccountsId;
  update lmShop_BankAccountsBalance set NonceBalance=@NonceBalance WHERE BankAccountsId=@BankAccountsId;
  update lmShop_WasteBook set NonceBalance=@NonceBalance,AuditingState=1,DateCreated=GETDATE() where WasteBookId=@WasteBookId;
  delete from @tmpTbl where WasteBookId=@WasteBookId;
  select @rowCount=count(WasteBookId) from @tmpTbl;
end 
COMMIT    ";
            var parms = new[] { new SqlParameter("@TradeCode", SqlDbType.VarChar, 32) { Value = tradeCode } };
            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL_AUDITING, parms);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>更新资金流IsOut字段为True,同时更新订单(前台事后申请发票用，其他地方慎用)
        /// </summary>
        /// <param name="orderIds">订单Ids </param>
        /// <param name="paidNo">交易流水号</param>
        /// <returns></returns>
        public Boolean RenewalWasteBookByIsOut(IEnumerable<Guid> orderIds, IEnumerable<string> paidNo)
        {
            var orderIdsStr = string.Join("','", orderIds.ToArray());
            var paidNoStr = string.Join("','", paidNo.ToArray());
            string SQL = string.Format(@"
BEGIN TRAN   
	UPDATE lmShop_GoodsOrder SET IsOut = 1 WHERE OrderId IN('{0}');
	UPDATE lmShop_WasteBook SET IsOut = 1 WHERE TradeCode IN('{1}');
COMMIT", orderIdsStr, paidNoStr);
            try
            {
                return SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL, null) > 0;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// 更新交易佣金的操作状态
        /// </summary>
        /// <returns></returns>
        /// zal 2016-09-21 操作状态(0:未处理；1:已处理)
        public bool UpdateOperateState()
        {
            string sql = @"
            UPDATE lmShop_WasteBook
            SET [OperateState]=1
            WHERE WasteSource=1 AND OperateState=0
            ";

            return SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, sql, null) > 0;
        }
        #endregion

        #region [删除资金流]

        /// <summary>删除资金流
        /// </summary>
        /// <param name="tradeCode">单据编号</param>
        public void DeleteWasteBook(string tradeCode)
        {
            const string SQL_DELETE_WASTEBOOK = "DELETE FROM lmShop_WasteBook WHERE TradeCode=@TradeCode AND AuditingState <> 1;";
            var parm = new SqlParameter("@TradeCode", SqlDbType.VarChar, 32) { Value = tradeCode };
            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL_DELETE_WASTEBOOK, parm);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary> 删除资金流手续费的记录
        /// </summary>
        /// <param name="tradeCode">单据编号</param>
        /// <param name="poundage">手续费</param>
        public void DeleteWasteBookPoundage(string tradeCode, decimal poundage)
        {
            const string SQL_DELETE_POUNDAGE = "DELETE FROM lmShop_WasteBook WHERE TradeCode=@TradeCode AND Income=@Income AND AuditingState <> 1;";
            var parms = new[]{
                new SqlParameter("@TradeCode", SqlDbType.VarChar, 32){Value = tradeCode},
                new SqlParameter("@Income", SqlDbType.Decimal){Value = poundage}
            };
            SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL_DELETE_POUNDAGE, parms);
        }

        #endregion

        #region [获取银行账号余额，手续费，记录数]

        /// <summary>获取银行账号余额
        /// </summary>
        /// <param name="bankAccountsId">银行账户ID</param>
        /// <returns></returns>
        public decimal GetBalance(Guid bankAccountsId)
        {
            const string SQL_SELECT_WASTEBOOK_BALANCE = @"
declare @NonceBalance decimal(18,4);
IF NOT EXISTS(SELECT NonceBalance FROM lmShop_BankAccountsBalance WITH(NOLOCK) WHERE BankAccountsId=@BankAccountsId)
	    BEGIN 
		    INSERT INTO lmShop_BankAccountsBalance VALUES (@BankAccountsId,0)
	    END
SELECT @NonceBalance=NonceBalance FROM lmShop_BankAccountsBalance WHERE BankAccountsId=@BankAccountsId;
SELECT @NonceBalance;";

            var parm = new SqlParameter("@BankAccountsId", SqlDbType.UniqueIdentifier) { Value = bankAccountsId };
            decimal balance = 0;
            object obj = SqlHelper.ExecuteScalar(GlobalConfig.ERP_DB_NAME, false, SQL_SELECT_WASTEBOOK_BALANCE, parm);
            if (obj != DBNull.Value)
            {
                balance = Convert.ToDecimal(obj);
            }
            return balance;
        }

        /// <summary>获取资金流手续费
        /// </summary>
        /// <param name="tradeCode">单据编号</param>
        /// <returns></returns>
        public decimal GetPoundage(string tradeCode)
        {
            const string SQL_SELECT_POUNDAGE = @"
SELECT Income into #temp FROM lmShop_WasteBook 
WHERE TradeCode=@TradeCode 

select Income from #temp
where Income>(select min(Income) from #temp) 
and Income<(select max(Income) from #temp)

truncate table #temp
drop table #temp";
            var parm = new SqlParameter("@TradeCode", SqlDbType.VarChar, 32) { Value = tradeCode };
            decimal incomePoundage = 0;
            object obj = SqlHelper.ExecuteScalar(GlobalConfig.ERP_DB_NAME, true, SQL_SELECT_POUNDAGE, parm);
            if (obj != DBNull.Value)
            {
                incomePoundage = Convert.ToDecimal(obj);
            }
            return incomePoundage;
        }

        /// <summary>获取资金流手续费
        /// </summary>
        /// <param name="tradeCode">单据编号</param>
        /// <returns></returns>
        public decimal GetPoundageForReckoning(string tradeCode)
        {
            const string SQL_SELECT_POUNDAGE2 = "SELECT MIN(ABS(Income)) FROM lmShop_WasteBook WHERE TradeCode=@TradeCode;";
            var parm = new SqlParameter("@TradeCode", SqlDbType.VarChar, 32) { Value = tradeCode };
            decimal poundage = 0;
            object obj = SqlHelper.ExecuteScalar(GlobalConfig.ERP_DB_NAME, true, SQL_SELECT_POUNDAGE2, parm);
            if (obj != DBNull.Value)
            {
                poundage = Convert.ToDecimal(obj);
            }
            return poundage;
        }

        /// <summary>获取同一单据号资金流记录数（判断是否有手续费）
        /// </summary>
        /// <param name="tradeCode">单据号</param>
        /// <returns></returns>
        public decimal GetTradeCodeNum(string tradeCode)
        {
            const string SQL_SELECT_COUNTTRADECODE = "SELECT COUNT(TradeCode) FROM lmShop_WasteBook WHERE TradeCode=@TradeCode;";
            var parm = new SqlParameter("@TradeCode", SqlDbType.VarChar, 32) { Value = tradeCode };
            decimal tcNum = 0;
            object obj = SqlHelper.ExecuteScalar(GlobalConfig.ERP_DB_NAME, true, SQL_SELECT_COUNTTRADECODE, parm);
            if (obj != DBNull.Value)
            {
                tcNum = Convert.ToDecimal(obj);
            }
            return tcNum;
        }

        #endregion

        #region [获取资金流ID]

        /// <summary>单据编号获取手续费的资金流ID
        /// </summary>
        /// <param name="tradeCode">单据编号</param>
        /// <returns></returns>
        public string GetWasteBookId(string tradeCode)
        {
            const string SQL_SELECT_POUNDAGEWASTEBOOKID = "SELECT WasteBookId FROM lmShop_WasteBook WHERE TradeCode=@TradeCode AND Income > (SELECT MIN(Income) FROM lmShop_WasteBook WHERE TradeCode=@TradeCode) AND Income < (SELECT MAX(Income) FROM lmShop_WasteBook WHERE TradeCode=@TradeCode);";
            var parm = new SqlParameter("@TradeCode", SqlDbType.VarChar, 32) { Value = tradeCode };
            string wasteBookId = "";
            object obj = SqlHelper.ExecuteScalar(GlobalConfig.ERP_DB_NAME, true, SQL_SELECT_POUNDAGEWASTEBOOKID, parm);
            if (obj != DBNull.Value)
            {
                wasteBookId = Convert.ToString(obj);
            }
            return wasteBookId;
        }

        /// <summary>获取资金流手续费的资金流ID
        /// </summary>
        /// <param name="tradeCode">单据编号</param>
        /// <returns></returns>
        public string GetWasteBookIdForUpdate(string tradeCode)
        {
            const string SQL_SELECT_WASTEBOOKID = "SELECT WasteBookId FROM lmShop_WasteBook WHERE TradeCode=@TradeCode ORDER BY Income DESC;";
            var parm = new SqlParameter("@TradeCode", SqlDbType.VarChar, 32) { Value = tradeCode };
            string wasteBookId = string.Empty;
            object obj = SqlHelper.ExecuteScalar(GlobalConfig.ERP_DB_NAME, true, SQL_SELECT_WASTEBOOKID, parm);
            if (obj != DBNull.Value)
            {
                wasteBookId = Convert.ToString(obj);
            }
            return wasteBookId;
        }

        /// <summary> 获取手续费的资金流ID
        /// </summary>
        /// <param name="tradeCode">单据编号</param>
        /// <returns></returns>
        public string GetWasteBookIdForReckoning(string tradeCode)
        {
            const string SQL_SELECT_WASTEBOOKID_MAXINCOME = "SELECT WasteBookId FROM lmShop_WasteBook WHERE TradeCode=@TradeCode ORDER BY Income ASC;";
            var parm = new SqlParameter("@TradeCode", SqlDbType.VarChar, 32) { Value = tradeCode };
            string wasteBookId = string.Empty;
            object obj = SqlHelper.ExecuteScalar(GlobalConfig.ERP_DB_NAME, true, SQL_SELECT_WASTEBOOKID_MAXINCOME, parm);
            if (obj != DBNull.Value)
            {
                wasteBookId = Convert.ToString(obj);
            }
            return wasteBookId;
        }

        #endregion

        #region [获取资金流]

        /// <summary>获取资金流信息
        /// </summary>
        /// <param name="wasteBookId">资金流ID</param>
        /// <returns></returns>
        public WasteBookInfo GetWasteBook(Guid wasteBookId)
        {
            const string SQL_SELECT_WASTEBOOK = "SELECT WasteBookId,BankAccountsId,TradeCode,DateCreated,Description,Income,NonceBalance,AuditingState,WasteBookType,SaleFilialeId,LinkTradeCode,LinkTradeType FROM lmShop_WasteBook WHERE WasteBookId=@WasteBookId;";

            var parm = new SqlParameter("@WasteBookId", SqlDbType.UniqueIdentifier) { Value = wasteBookId };
            WasteBookInfo wasteBookInfo;
            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL_SELECT_WASTEBOOK, parm))
            {
                wasteBookInfo = rdr.Read()
                                    ? new WasteBookInfo(rdr.GetGuid(0), rdr.GetGuid(1), rdr.GetString(2),
                                                        rdr.GetDateTime(3), rdr.GetString(4), rdr.GetDecimal(5),
                                                        rdr.GetDecimal(6), rdr.GetInt32(7),
                                                        rdr[8] == DBNull.Value ? 0 : rdr.GetInt32(8),
                                                        rdr[9] == DBNull.Value ? Guid.Empty : rdr.GetGuid(9))
                                    {
                                        LinkTradeCode = rdr[10] == DBNull.Value ? string.Empty : rdr.GetString(10),
                                        LinkTradeType = rdr[11] == DBNull.Value ? 0 : rdr.GetInt32(11)
                                    }
                                    : new WasteBookInfo();
            }
            return wasteBookInfo;
        }

        /// <summary>根据单据编号获取资金流
        /// </summary>
        /// <param name="tradeCode">单据编号</param>
        /// <returns></returns>
        public WasteBookInfo GetWasteBookByBankAccountsId(string tradeCode)
        {
            const string SQL = @"SELECT WasteBookId,BankAccountsId,TradeCode,
DateCreated,Description,Income,NonceBalance,AuditingState,WasteBookType,SaleFilialeId,LinkTradeCode,LinkTradeType 
FROM lmShop_WasteBook WHERE TradeCode=@TradeCode;
";
            using (var db = DatabaseFactory.Create())
            {
                return db.Single<WasteBookInfo>(true, SQL, new Parameter("@TradeCode", tradeCode));
            }
        }

        /// <summary>根据单据编号获取资金流
        /// </summary>
        /// <param name="linkTradeCode">关联单据编号</param>
        /// <param name="wasteSource">1:天猫、京东、第三方交易佣金;2:积分代扣;3:订单交易金额</param>
        /// <returns></returns>
        /// zal 2016-06-15
        public WasteBookInfo GetWasteBookByLinkTradeCodeAndWasteSource(string linkTradeCode, int wasteSource)
        {
            const string SQL = @"
            SELECT WasteBookId,BankAccountsId,TradeCode,DateCreated,Description,Income,NonceBalance,
            AuditingState,WasteBookType,SaleFilialeId,LinkTradeCode,LinkTradeType,WasteSource 
            FROM lmShop_WasteBook WHERE LinkTradeCode=@LinkTradeCode and WasteSource=@WasteSource;
            ";
            using (var db = DatabaseFactory.Create())
            {
                var parms = new[]
                {
                    new Parameter("@LinkTradeCode", linkTradeCode),
                    new Parameter("@WasteSource", wasteSource)
                };

                return db.Single<WasteBookInfo>(true, SQL, parms);
            }
        }

        /// <summary>获取资金流信息（历史数据库）
        /// </summary>
        /// <param name="wasteBookId">资金流ID</param>
        /// <param name="dateTime">时间</param>
        /// <param name="keepyear">保留年份</param>
        /// <returns></returns>
        public WasteBookInfo GetWasteBook(Guid wasteBookId, DateTime dateTime, int keepyear)
        {
            const string SQL_SELECT_WASTEBOOK = "SELECT WasteBookId,BankAccountsId,TradeCode,DateCreated,Description,Income,NonceBalance,AuditingState,WasteBookType,SaleFilialeId,LinkTradeCode,LinkTradeType FROM lmShop_WasteBook WHERE WasteBookId=@WasteBookId;";
            var parm = new SqlParameter("@WasteBookId", SqlDbType.UniqueIdentifier) { Value = wasteBookId };
            WasteBookInfo wasteBookInfo;

            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL_SELECT_WASTEBOOK, parm))
            {
                wasteBookInfo = rdr.Read()
                                    ? new WasteBookInfo(rdr.GetGuid(0), rdr.GetGuid(1), rdr.GetString(2),
                                                        rdr.GetDateTime(3), rdr.GetString(4), rdr.GetDecimal(5),
                                                        rdr.GetDecimal(6), rdr.GetInt32(7),
                                                        rdr[8] == DBNull.Value ? 0 : rdr.GetInt32(8),
                                                        rdr[9] == DBNull.Value ? Guid.Empty : rdr.GetGuid(9))
                                    {
                                        LinkTradeCode = rdr[10] == DBNull.Value ? string.Empty : rdr.GetString(10),
                                        LinkTradeType = rdr[11] == DBNull.Value ? 0 : rdr.GetInt32(11)
                                    }
                                    : new WasteBookInfo();
            }
            return wasteBookInfo;
        }

        /// <summary> 获取资金流单据类型
        /// </summary>
        /// <param name="tradeCode">单据编号</param>
        /// <returns></returns>
        public WasteTypeInfo GetWasteBookInfo(String tradeCode)
        {
            const string SQL_SELECT_BY_TRADECODE = "SELECT WasteBookId,BankAccountsId,TradeCode,DateCreated,Description,Income,NonceBalance,AuditingState,WasteBookType,SaleFilialeId,LinkTradeCode,LinkTradeType FROM lmShop_WasteBook WHERE TradeCode=@TradeCode ORDER BY Income;";

            var parm = new SqlParameter("@TradeCode", SqlDbType.VarChar, 32) { Value = tradeCode };
            var wasteTypeInfo = new WasteTypeInfo();
            int num = 0;
            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL_SELECT_BY_TRADECODE, parm))
            {
                while (rdr.Read())
                {
                    var wasteBookInfo = new WasteBookInfo(rdr.GetGuid(0), rdr.GetGuid(1), rdr.GetString(2),
                                                          rdr.GetDateTime(3), rdr.GetString(4), rdr.GetDecimal(5),
                                                          rdr.GetDecimal(6), rdr.GetInt32(7),
                                                          rdr[8] == DBNull.Value ? 0 : rdr.GetInt32(8),
                                                          rdr[9] == DBNull.Value ? Guid.Empty : rdr.GetGuid(9))
                    {
                        LinkTradeCode = rdr[10] == DBNull.Value ? string.Empty : rdr.GetString(10),
                        LinkTradeType = rdr[11] == DBNull.Value ? 0 : rdr.GetInt32(11)
                    };
                    if (rdr.GetInt32(8) == (int)WasteBookType.Increase)
                    {
                        wasteTypeInfo.Increase = wasteBookInfo;
                        wasteTypeInfo.WasteBookType = WasteBookType.Increase;
                    }
                    else if (rdr.GetInt32(8) == (int)WasteBookType.Decrease)
                    {
                        if (num == 0)
                        {
                            wasteTypeInfo.Decrease = wasteBookInfo;
                            wasteTypeInfo.WasteBookType = WasteBookType.Decrease;
                        }
                        else
                        {
                            wasteTypeInfo.TransferFee = wasteBookInfo;
                            wasteTypeInfo.WasteBookType = WasteBookType.Decrease;
                        }
                        num += 1;
                    }
                }
            }
            return wasteTypeInfo;
        }

        #endregion

        #region [获取资金流集合]

        /// <summary>根据有操作权限银行账号的资金流
        /// </summary>
        /// <param name="bankAccountsId">银行帐号Id</param>
        /// <param name="personnelId">员工ID</param>
        /// <returns></returns>
        public IList<WasteBookInfo> GetWasteBookList(Guid bankAccountsId, Guid personnelId)
        {
            const string SQL_SELECT_WASTEBOOK_LIST = "SELECT WasteBookId,BankAccountsId,TradeCode,DateCreated,Description,Income,NonceBalance,AuditingState,WasteBookType,SaleFilialeId,LinkTradeCode,LinkTradeType FROM lmShop_WasteBook WHERE BankAccountsId=@BankAccountsId and AuditingState=@AuditingState and BankAccountsId in (select BankAccountsId from BankAccountPermission  where PersonnelId=@PersonnelId)  ORDER BY DateCreated DESC;";
            var parms = new[]{
                new SqlParameter("@BankAccountsId", SqlDbType.UniqueIdentifier){Value = bankAccountsId},
                new SqlParameter("@PersonnelId", SqlDbType.UniqueIdentifier){Value =personnelId }

            };
            IList<WasteBookInfo> wasteBookList = new List<WasteBookInfo>();
            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL_SELECT_WASTEBOOK_LIST, parms))
            {
                while (rdr.Read())
                {
                    var wasteBookInfo = new WasteBookInfo(rdr.GetGuid(0), rdr.GetGuid(1), rdr.GetString(2),
                                                          rdr.GetDateTime(3), rdr.GetString(4), rdr.GetDecimal(5),
                                                          rdr.GetDecimal(6), rdr.GetInt32(7),
                                                          rdr[8] == DBNull.Value ? 0 : rdr.GetInt32(8),
                                                          rdr[9] == DBNull.Value ? Guid.Empty : rdr.GetGuid(9))
                    {
                        LinkTradeCode = rdr[10] == DBNull.Value ? string.Empty : rdr.GetString(10),
                        LinkTradeType = rdr[11] == DBNull.Value ? 0 : rdr.GetInt32(11)
                    };
                    wasteBookList.Add(wasteBookInfo);
                }
            }
            return wasteBookList;
        }

        /// <summary>获取资金列表
        /// </summary>
        /// <param name="bankAccountsId">银行账号ID</param>
        /// <param name="startDate">开始时间</param>
        /// <param name="endDate">截止时间</param>
        /// <param name="receiptType">单据类型</param>
        /// <param name="auditingState">审核状态</param>
        /// <param name="minIncome">金额范围 小</param>
        /// <param name="maxIncome">金额范围 大</param>
        /// <param name="tradeCode">单据编号</param>
        /// <param name="saleFilialeId">公司ID</param>
        /// <param name="branchId">部门ID</param>
        /// <param name="positionId">职务ID</param>
        /// <param name="keepyear">保留几年数据</param>
        /// <returns></returns>
        public IList<WasteBookInfo> GetWasteBookList(Guid bankAccountsId, DateTime startDate, DateTime endDate, ReceiptType receiptType,
            AuditingState auditingState, double minIncome, double maxIncome, string tradeCode, Guid saleFilialeId, Guid branchId, Guid positionId, int keepyear)
        {
            var prms = new[]
                           {
                               new SqlParameter("@BankAccountsId", SqlDbType.UniqueIdentifier){Value = bankAccountsId},
                               new SqlParameter("@StartDate", SqlDbType.DateTime){Value = startDate == DateTime.MinValue ? DateTime.Now : startDate},
                               new SqlParameter("@EndDate", SqlDbType.DateTime){Value = endDate == DateTime.MinValue ? DateTime.Now : endDate},
                               new SqlParameter("@ReceipType", SqlDbType.Int){Value = (int)receiptType},
                               new SqlParameter("@AuditingState", SqlDbType.Int){Value =auditingState },
                               new SqlParameter("@MinIncome", SqlDbType.Float){Value = minIncome},
                               new SqlParameter("@MaxIncome", SqlDbType.Float){Value =maxIncome },
                               new SqlParameter("@TradeCode", SqlDbType.VarChar, 64){Value = tradeCode.Replace("[", "[[]")},
                               new SqlParameter("@SaleFilialeId", SqlDbType.UniqueIdentifier){Value =saleFilialeId },
                               new SqlParameter("@BranchId", SqlDbType.UniqueIdentifier){Value =branchId },
                               new SqlParameter("@PositionId", SqlDbType.UniqueIdentifier){Value =positionId }
                           };
            var sqlStr = new StringBuilder();
            sqlStr.Append(
                "SELECT WasteBookId,BankAccountsId,TradeCode,DateCreated,Description,Income,NonceBalance,AuditingState,WasteBookType,SaleFilialeId,LinkTradeCode,LinkTradeType FROM lmShop_WasteBook WHERE 1=1 ");
            sqlStr.Append(
                "and BankAccountsId in (select BankAccountsId from BankAccountPermission where FilialeId=@SaleFilialeId and BranchId=@BranchId and PositionId=@PositionId union select '00000000-0000-0000-0000-000000000000')");
            if (bankAccountsId != Guid.Empty)
                sqlStr.Append(" AND BankAccountsId=@BankAccountsId");
            sqlStr.Append(" AND AuditingState=@AuditingState");

            if (startDate != DateTime.MinValue)
            {
                sqlStr.Append(" AND DateCreated >= @StartDate");
            }

            if (endDate != DateTime.MinValue)
            {
                sqlStr.Append(" AND DateCreated <= @EndDate");
            }

            if (receiptType != ReceiptType.All)
            {
                if (receiptType == ReceiptType.Expenditure)
                    sqlStr.Append(" AND Income<0");
                else if (receiptType == ReceiptType.Income)
                    sqlStr.Append(" AND Income >0");
            }
            if (minIncome != Double.MinValue)
            {
                sqlStr.Append(" AND ABS(Income) >=@MinIncome");
            }
            if (maxIncome != Double.MaxValue)
            {
                sqlStr.Append(" AND ABS(Income) <=@MaxIncome");
            }
            if (tradeCode != String.Empty)
            {
                sqlStr.Append(" AND (TradeCode = @TradeCode OR Description LIKE '%'+@TradeCode+'%')");
            }
            sqlStr.Append(" ORDER BY DateCreated DESC,Income DESC;");

            IList<WasteBookInfo> wasteBookList = new List<WasteBookInfo>();
            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, sqlStr.ToString(), prms))
            {
                while (rdr.Read())
                {
                    var wasteBookInfo = new WasteBookInfo(rdr.GetGuid(0), rdr.GetGuid(1), rdr.GetString(2),
                                                          rdr.GetDateTime(3), rdr.GetString(4), rdr.GetDecimal(5),
                                                          rdr.GetDecimal(6), rdr.GetInt32(7),
                                                          rdr[8] == DBNull.Value ? 0 : rdr.GetInt32(8),
                                                          rdr[9] == DBNull.Value ? Guid.Empty : rdr.GetGuid(9))
                    {
                        LinkTradeCode = rdr[10] == DBNull.Value ? string.Empty : rdr.GetString(10),
                        LinkTradeType = rdr[11] == DBNull.Value ? 0 : rdr.GetInt32(11)
                    };
                    wasteBookList.Add(wasteBookInfo);
                }
            }
            return wasteBookList;
        }

        /// <summary>获取资金列表（分页）
        /// </summary>
        /// <param name="bankAccountsId">银行账号ID</param>
        /// <param name="startDate">开始时间</param>
        /// <param name="endDate">截止时间</param>
        /// <param name="receiptType">单据类型</param>
        /// <param name="auditingState">审核状态</param>
        /// <param name="minIncome">金额范围 小</param>
        /// <param name="maxIncome">金额范围 大</param>
        /// <param name="tradeCode">单据编号</param>
        /// <param name="saleFilialeId">公司ID</param>
        /// <param name="branchId">部门ID</param>
        /// <param name="positionId">职务ID</param>
        /// <param name="keepyear">保留几年数据</param>
        /// <param name="startPage">当前页</param>
        /// <param name="pageSize">每页显示记录数</param>
        /// <param name="recordCount">总记录数</param>
        /// <returns></returns>
        public IList<WasteBookInfo> GetWasteBookListToPage(Guid bankAccountsId, DateTime startDate, DateTime endDate, ReceiptType receiptType,
            AuditingState auditingState, double minIncome, double maxIncome, string tradeCode, Guid saleFilialeId, Guid branchId, Guid positionId, int keepyear,
            int startPage, int pageSize, out long recordCount)
        {
            var sqlStr = new StringBuilder();
            sqlStr.Append("SELECT WasteBookId,BankAccountsId,TradeCode,DateCreated,Description,Income,NonceBalance,AuditingState,WasteBookType,SaleFilialeId,LinkTradeCode,LinkTradeType FROM lmShop_WasteBook WHERE 1=1 ");
            sqlStr.Append("and BankAccountsId in (select BankAccountsId from BankAccountPermission where FilialeId=@SaleFilialeId and BranchId=@BranchId and PositionId=@PositionId union select '00000000-0000-0000-0000-000000000000')");
            if (bankAccountsId != Guid.Empty)
                sqlStr.Append(" AND BankAccountsId=@BankAccountsId");
            sqlStr.Append(" AND AuditingState=@AuditingState");

            if (startDate != DateTime.MinValue)
            {
                sqlStr.Append(" AND DateCreated >= @StartDate");
            }

            if (endDate != DateTime.MinValue)
            {
                sqlStr.Append(" AND DateCreated <= @EndDate");
            }

            if (receiptType != ReceiptType.All)
            {
                if (receiptType == ReceiptType.Expenditure)
                    sqlStr.Append(" AND Income<0");
                else if (receiptType == ReceiptType.Income)
                    sqlStr.Append(" AND Income >0");
            }
            if (minIncome != Double.MinValue)
            {
                sqlStr.Append(" AND ABS(Income) >=@MinIncome");
            }
            if (maxIncome != Double.MaxValue)
            {
                sqlStr.Append(" AND ABS(Income) <=@MaxIncome");
            }
            if (tradeCode != String.Empty)
            {
                sqlStr.Append(" AND (TradeCode = @TradeCode OR Description LIKE '%'+@TradeCode+'%')");
            }

            var paramList = new List<Parameter>
                                {
                                    new Parameter("@BankAccountsId", bankAccountsId),
                                    new Parameter("@StartDate", startDate == DateTime.MinValue ? DateTime.Now : startDate),
                                    new Parameter("@EndDate", endDate == DateTime.MinValue ? DateTime.Now : endDate),
                                    new Parameter("@ReceipType", (int) receiptType),
                                    new Parameter("@AuditingState", (int) auditingState),
                                    new Parameter("@MinIncome", minIncome),
                                    new Parameter("@MaxIncome", maxIncome),
                                    new Parameter("@TradeCode", tradeCode.Replace("[", "[[]")),
                                    new Parameter("@SaleFilialeId", saleFilialeId),
                                    new Parameter("@BranchId", branchId),
                                    new Parameter("@PositionId", positionId)
                                };

            using (var db = DatabaseFactory.Create(startDate.Year))
            {
                var pageQuery = new Keede.DAL.Helper.Sql.PageQuery(startPage, pageSize, sqlStr.ToString(), " DateCreated DESC,Income DESC ");
                var pageItem = db.SelectByPage<WasteBookInfo>(true, pageQuery, paramList.ToArray());
                recordCount = pageItem.RecordCount;
                return pageItem.Items.ToList();
            }
        }

        /// <summary>获取资金列表（分页）
        /// </summary>
        /// <param name="bankAccountsId">银行账号ID</param>
        /// <param name="startDate">开始时间</param>
        /// <param name="endDate">截止时间</param>
        /// <param name="receiptType">单据类型</param>
        /// <param name="auditingState">审核状态</param>
        /// <param name="minIncome">金额范围 小</param>
        /// <param name="maxIncome">金额范围 大</param>
        /// <param name="tradeCode">单据编号</param>
        /// <param name="saleFilialeId">公司ID</param>
        /// <param name="branchId">部门ID</param>
        /// <param name="positionId">职务ID</param>
        /// <param name="keepyear">保留几年数据</param>
        /// <param name="isCheck"> 是否对账</param>
        /// <param name="startPage">当前页</param>
        /// <param name="pageSize">每页显示记录数</param>
        /// <param name="recordCount">总记录数</param>
        /// <returns></returns>
        public IList<WasteBookInfo> GetWasteBookListToPage(Guid bankAccountsId, DateTime startDate, DateTime endDate, ReceiptType receiptType,
            AuditingState auditingState, double minIncome, double maxIncome, string tradeCode, Guid saleFilialeId, Guid branchId, Guid positionId, int keepyear, int isCheck,
            int startPage, int pageSize, out long recordCount)
        {
            var sqlCount = new StringBuilder();
            sqlCount.Append(@"
SELECT COUNT(0) AS TotalCount  
FROM lmShop_WasteBook WITH(NOLOCK) ");

            var sqlSelect = new StringBuilder();
            sqlSelect.Append(@"
SELECT ROW_NUMBER() OVER(ORDER BY DateCreated DESC,Income DESC) AS rowNum,WasteBookId,BankAccountsId,TradeCode,DateCreated,Description,Income,NonceBalance,AuditingState,WasteBookType,SaleFilialeId,LinkTradeCode,LinkTradeType  
FROM lmShop_WasteBook WITH(NOLOCK) ");

            #region Parameter
            var sqlParameter = new StringBuilder();
            sqlParameter.Append(String.Format("WHERE BankAccountsId IN (SELECT BankAccountsId FROM BankAccountPermission WITH(NOLOCK) WHERE FilialeId='{0}' AND BranchId='{1}' AND PositionId='{2}' "+ (bankAccountsId.Equals(Guid.Empty)?"":"AND BankAccountsId='"+ bankAccountsId + "'") + ")", saleFilialeId, branchId, positionId));
            sqlParameter.Append(String.Format(" AND AuditingState={0}", (int)auditingState));

            if (startDate != DateTime.MinValue)
            {
                sqlParameter.Append(String.Format(" AND DateCreated >= '{0}'", startDate));
            }

            if (endDate != DateTime.MinValue)
            {
                sqlParameter.Append(String.Format(" AND DateCreated < '{0}'", endDate));
            }

            if (receiptType != ReceiptType.All)
            {
                if (receiptType == ReceiptType.Expenditure)
                    sqlParameter.Append(" AND Income<0");
                else if (receiptType == ReceiptType.Income)
                    sqlParameter.Append(" AND Income >0");
            }
            if (isCheck != -1)
            {
                if (isCheck == 0)
                    sqlParameter.Append(" AND WasteBookId NOT IN (SELECT WasteBookId FROM WasteBookCheck WITH(NOLOCK))");
                else if (isCheck == 1)
                    sqlParameter.Append(" AND WasteBookId IN (SELECT WasteBookId FROM WasteBookCheck WITH(NOLOCK))");
            }
            if (minIncome != Double.MinValue)
            {
                sqlParameter.Append(String.Format(" AND ABS(Income) >={0}", minIncome));
            }
            if (maxIncome != Double.MaxValue)
            {
                sqlParameter.Append(String.Format(" AND ABS(Income) <={0}", maxIncome));
            }
            if (tradeCode != String.Empty)
            {
                sqlParameter.Append(String.Format(" AND (TradeCode = '{0}' OR LinkTradeCode = '{0}' OR Description LIKE '%{0}%')", tradeCode));
            }
            #endregion

            var sqlTotalCount = sqlCount.Append(sqlParameter);
            recordCount = int.Parse(SqlHelper.ExecuteScalar(GlobalConfig.GetErpDbName(startDate.Year),true, sqlTotalCount.ToString()).ToString());

            var sqlPageQuery =
                "SELECT WasteBookId,BankAccountsId,TradeCode,DateCreated,Description,Income,NonceBalance,AuditingState,WasteBookType,SaleFilialeId,LinkTradeCode,LinkTradeType FROM ( "
                + sqlSelect.Append(sqlParameter)
                + " )temp WHERE temp.rowNum BETWEEN " + ((startPage - 1) * pageSize + 1) + " AND " + startPage * pageSize;

            IList<WasteBookInfo> wasteBookInfoList = new List<WasteBookInfo>();
            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.GetErpDbName(startDate.Year), true, sqlPageQuery))
            {
                while (rdr.Read())
                {
                    var wasteBookInfo = new WasteBookInfo
                    {
                        WasteBookId = rdr["WasteBookId"] == DBNull.Value ? Guid.Empty : new Guid(rdr["WasteBookId"].ToString()),
                        BankAccountsId = rdr["BankAccountsId"] == DBNull.Value ? Guid.Empty : new Guid(rdr["BankAccountsId"].ToString()),
                        TradeCode = rdr["TradeCode"] == DBNull.Value ? string.Empty : rdr["TradeCode"].ToString(),
                        DateCreated = rdr["DateCreated"] == DBNull.Value ? Convert.ToDateTime("1900-01-01") : Convert.ToDateTime(rdr["DateCreated"].ToString()),
                        Description = rdr["Description"] == DBNull.Value ? string.Empty : rdr["Description"].ToString(),
                        Income = rdr["Income"] == DBNull.Value ? 0 : decimal.Parse(rdr["Income"].ToString()),
                        NonceBalance = rdr["NonceBalance"] == DBNull.Value ? 0 : decimal.Parse(rdr["NonceBalance"].ToString()),
                        AuditingState = rdr["AuditingState"] == DBNull.Value ? -1 : int.Parse(rdr["AuditingState"].ToString()),
                        WasteBookType = rdr["WasteBookType"] == DBNull.Value ? -1 : int.Parse(rdr["WasteBookType"].ToString()),
                        SaleFilialeId = rdr["SaleFilialeId"] == DBNull.Value ? Guid.Empty : new Guid(rdr["SaleFilialeId"].ToString()),
                        LinkTradeCode = rdr["LinkTradeCode"] == DBNull.Value ? string.Empty : rdr["LinkTradeCode"].ToString(),
                        LinkTradeType = rdr["LinkTradeType"] == DBNull.Value ? -1 : int.Parse(rdr["LinkTradeType"].ToString())
                    };
                    wasteBookInfoList.Add(wasteBookInfo);
                }
            }

            return wasteBookInfoList;
        }

        /// <summary>获取资金列表（分页）
        /// </summary>
        /// <param name="saleFilialeId">公司ID</param>
        /// <param name="startDate">开始时间</param>
        /// <param name="endDate">截止时间</param>
        /// <param name="receiptType">单据类型</param>
        /// <param name="auditingState">审核状态</param>
        /// <param name="minIncome">金额范围 小</param>
        /// <param name="maxIncome">金额范围 大</param>
        /// <param name="tradeCode">单据编号</param>
        /// <param name="filialeId">员工公司ID</param>
        /// <param name="branchId">部门ID</param>
        /// <param name="positionId">职务ID</param>
        /// <param name="keepyear">保存几年数据</param>
        /// <param name="isCheck">是否对账</param>
        /// <param name="startPage">当前页</param>
        /// <param name="pageSize">每页显示记录数</param>
        /// <param name="recordCount">总记录数</param>
        /// <returns></returns>
        public IList<WasteBookInfo> GetWasteBookListBySaleFilialeIdToPage(Guid saleFilialeId, DateTime startDate, DateTime endDate, ReceiptType receiptType, AuditingState auditingState, double minIncome, double maxIncome, string tradeCode, Guid filialeId, Guid branchId, Guid positionId, int keepyear, int isCheck, int startPage, int pageSize, out long recordCount)
        {
            var sqlCount = new StringBuilder();
            sqlCount.Append(@"
SELECT COUNT(0) AS TotalCount  
FROM lmShop_WasteBook WITH(NOLOCK) ");

            var sqlSelect = new StringBuilder();
            sqlSelect.Append(@"
SELECT ROW_NUMBER() OVER(ORDER BY DateCreated DESC,Income DESC) AS rowNum,WasteBookId,BankAccountsId,TradeCode,DateCreated,Description,Income,NonceBalance,AuditingState,WasteBookType,SaleFilialeId,LinkTradeCode,LinkTradeType  
FROM lmShop_WasteBook WITH(NOLOCK) ");

            var sqlParameter = new StringBuilder();
            sqlParameter.Append(@"
WHERE BankAccountsId IN (
    SELECT A.[BankAccountsId] AS BankAccountsId FROM BankAccountBinding AS A WITH(NOLOCK)
    INNER JOIN lmShop_BankAccounts AS B WITH(NOLOCK) ON A.BankAccountsId=B.BankAccountsId AND B.IsUse=1
    WHERE TargetId=@SaleFilialeId 
    INTERSECT
    SELECT ba.BankAccountsId AS BankAccountsId FROM (
		SELECT BankAccountsId FROM BankAccountPermission WITH(NOLOCK) 
		WHERE FilialeId=@FilialeId and BranchId=@BranchId and PositionId=@PositionId 
		UNION SELECT '00000000-0000-0000-0000-000000000000'
	) ba
)
AND AuditingState=@AuditingState");

            #region Parameter
            if (startDate != DateTime.MinValue)
            {
                sqlParameter.Append(" AND DateCreated >= @StartDate");
            }

            if (endDate != DateTime.MinValue)
            {
                sqlParameter.Append(" AND DateCreated < @EndDate");
            }

            if (receiptType != ReceiptType.All)
            {
                if (receiptType == ReceiptType.Expenditure)
                    sqlParameter.Append(" AND Income<0");
                else if (receiptType == ReceiptType.Income)
                    sqlParameter.Append(" AND Income >0");
            }
            if (isCheck != -1)
            {
                if (isCheck == 0)
                    sqlParameter.Append(" AND WasteBookId NOT IN (SELECT WasteBookId FROM WasteBookCheck WITH(NOLOCK))");
                else if (isCheck == 1)
                    sqlParameter.Append(" AND WasteBookId IN (SELECT WasteBookId FROM WasteBookCheck WITH(NOLOCK))");
            }
            if (minIncome != Double.MinValue)
            {
                sqlParameter.Append(" AND ABS(Income) >=@MinIncome");
            }
            if (maxIncome != Double.MaxValue)
            {
                sqlParameter.Append(" AND ABS(Income) <=@MaxIncome");
            }
            if (tradeCode != String.Empty)
            {
                sqlParameter.Append(" AND (TradeCode = @TradeCode OR Description LIKE '%'+@TradeCode+'%')");
            }
            #endregion

            var paras = new SqlParameter[]{
                new SqlParameter("@SaleFilialeId", saleFilialeId),
                new SqlParameter("@StartDate", startDate == DateTime.MinValue ? DateTime.Now : startDate),
                new SqlParameter("@EndDate", endDate == DateTime.MinValue ? DateTime.Now : endDate),
                new SqlParameter("@ReceipType", (int) receiptType),
                new SqlParameter("@AuditingState", (int) auditingState),
                new SqlParameter("@MinIncome", minIncome),
                new SqlParameter("@MaxIncome", maxIncome),
                new SqlParameter("@TradeCode", tradeCode.Replace("[", "[[]")),
                new SqlParameter("@FilialeId", filialeId),
                new SqlParameter("@BranchId", branchId),
                new SqlParameter("@PositionId", positionId)
            };

            var sqlTotalCount = sqlCount.Append(sqlParameter);
            recordCount = int.Parse(SqlHelper.ExecuteScalar(GlobalConfig.GetErpDbName(startDate.Year), true, sqlTotalCount.ToString(), paras).ToString());

            var sqlPageQuery =
                "SELECT WasteBookId,BankAccountsId,TradeCode,DateCreated,Description,Income,NonceBalance,AuditingState,WasteBookType,SaleFilialeId,LinkTradeCode,LinkTradeType FROM ( "
                + sqlSelect.Append(sqlParameter)
                + " )temp WHERE temp.rowNum BETWEEN " + ((startPage - 1) * pageSize + 1) + " AND " + startPage * pageSize;

            IList<WasteBookInfo> wasteBookInfoList = new List<WasteBookInfo>();
            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.GetErpDbName(startDate.Year), true, sqlPageQuery, paras))
            {
                while (rdr.Read())
                {
                    var wasteBookInfo = new WasteBookInfo
                    {
                        WasteBookId = rdr["WasteBookId"] == DBNull.Value ? Guid.Empty : new Guid(rdr["WasteBookId"].ToString()),
                        BankAccountsId = rdr["BankAccountsId"] == DBNull.Value ? Guid.Empty : new Guid(rdr["BankAccountsId"].ToString()),
                        TradeCode = rdr["TradeCode"] == DBNull.Value ? string.Empty : rdr["TradeCode"].ToString(),
                        DateCreated = rdr["DateCreated"] == DBNull.Value ? Convert.ToDateTime("1900-01-01") : Convert.ToDateTime(rdr["DateCreated"].ToString()),
                        Description = rdr["Description"] == DBNull.Value ? string.Empty : rdr["Description"].ToString(),
                        Income = rdr["Income"] == DBNull.Value ? 0 : decimal.Parse(rdr["Income"].ToString()),
                        NonceBalance = rdr["NonceBalance"] == DBNull.Value ? 0 : decimal.Parse(rdr["NonceBalance"].ToString()),
                        AuditingState = rdr["AuditingState"] == DBNull.Value ? -1 : int.Parse(rdr["AuditingState"].ToString()),
                        WasteBookType = rdr["WasteBookType"] == DBNull.Value ? -1 : int.Parse(rdr["WasteBookType"].ToString()),
                        SaleFilialeId = rdr["SaleFilialeId"] == DBNull.Value ? Guid.Empty : new Guid(rdr["SaleFilialeId"].ToString()),
                        LinkTradeCode = rdr["LinkTradeCode"] == DBNull.Value ? string.Empty : rdr["LinkTradeCode"].ToString(),
                        LinkTradeType = rdr["LinkTradeType"] == DBNull.Value ? -1 : int.Parse(rdr["LinkTradeType"].ToString())
                    };
                    wasteBookInfoList.Add(wasteBookInfo);
                }
            }

            return wasteBookInfoList;
        }

        /// <summary>
        /// 【公司毛利使用】获取订单佣金(此佣金是一个订单号对应的所有佣金的和，原因：第三方订单和本系统订单存在多对多的情况，导致本系统一个订单会产生多笔佣金)
        /// </summary>
        /// <returns></returns>
        /// zal 2016-09-20
        public IList<WasteBookInfo> GetWasteBookByDateCreatedForProfits()
        {
            const string SQL = @"
            SELECT SUM(ISNULL(ABS(Income),0)) AS Income,LinkTradeCode FROM lmShop_WasteBook WITH(NOLOCK) 
            WHERE WasteSource=1 AND OperateState=0 
            GROUP BY LinkTradeCode
            ";
            var list = new List<WasteBookInfo>();
            var sdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL);

            while (sdr.Read())
            {
                list.Add(new WasteBookInfo
                {
                    Income = sdr["Income"] == DBNull.Value ? 0 : decimal.Parse(sdr["Income"].ToString()),
                    LinkTradeCode = sdr["LinkTradeCode"] == DBNull.Value ? string.Empty : sdr["LinkTradeCode"].ToString()
                });
            }
            sdr.Close();

            return list;
        }
        #endregion

        #region 资金查询的相关方法

        /// <summary>
        /// 获取公司绑定银行的收款情况
        /// </summary>
        /// <param name="keepyear"></param>
        /// <param name="year"></param>
        /// <param name="saleFilialeId"></param>
        /// <param name="bankName"></param>
        /// <returns></returns>
        public IList<FundPaymentDaysInfo> GetFundPaymentDaysInfos(int keepyear, int year, Guid saleFilialeId, string bankName)
        {
            var bankSelectStr = string.Empty;
            if (saleFilialeId == new Guid("30aa2f62-0da9-49cc-8948-4ddee198072d")) //ERP 公司
            {
                bankSelectStr =
                    string.Format(@" select BankAccountsId from lmShop_BankAccounts where IsMain=0 and IsUse=1");
            }
            else
            {
                bankSelectStr = string.Format(@" select bb.BankAccountsId from [BankAccountBinding] bb with(nolock)
					                                    where 
					                                    TargetId='{0}'", saleFilialeId);
            }
            var bankStr = string.Empty;
            if (!string.IsNullOrEmpty(bankName)) bankStr = string.Format(@" and ls_ba.BankName like '{0}%'", bankName);
            string str = string.Format(@"select 	
		                                    ls_ba.BankAccountsId as BankAccountsId,
		                                    ls_ba.BankName as BankName,
		                                    isnull(i2.[1],0)  as [Jan]
		                                    ,isnull(i2.[2],0)  as [Feb]
		                                    ,isnull(i2.[3],0)  as [Mar]
		                                    ,isnull(i2.[4],0)  as [Apr]
		                                    ,isnull(i2.[5],0)  as [May]
		                                    ,isnull(i2.[6],0)  as [Jun]
		                                    ,isnull(i2.[7],0)  as [July]
		                                    ,isnull(i2.[8],0)  as [Aug]
		                                    ,isnull(i2.[9],0)  as [Sept]
		                                    ,isnull(i2.[10],0)  as [Oct]
		                                    ,ISNULL(i2.[11],0) as[Nov]
		                                    ,ISNULL(I2.[12],0) as[December]
	                                    from  (                                            
		                                    select
				                                 wb.BankAccountsId as BankAccountsId,month(DateCreated) as [DaT],sum(Income)  as Non
			                                From   lmShop_WasteBook wb with(nolock)
			                                inner join (
					                                {1}
				                                ) Tab1 on wb.BankAccountsId=Tab1.BankAccountsId
			                                where 
			                                DateCreated   >='{0}-01-01'   and DateCreated  <'{0}-12-31 23:59:59.997' 
			                                and AuditingState=1 and LinkTradeType  in(1,5,4) 
			                                group by  wb.BankAccountsId,month(DateCreated)
	                                    ) I1 pivot  (
		                                    sum(Non) for  Dat in ([1],[2],[3],[4],[5],[6],[7],[8],[9],[10],[11],[12])
	                                    )I2
	                                    inner join lmShop_BankAccounts ls_ba on i2.BankAccountsId=ls_ba.BankAccountsId and ls_ba.IsUse=1
	                                    where 1=1 {2}
                                        ", year, bankSelectStr, bankStr);

            IList<FundPaymentDaysInfo> wasteBookList = new List<FundPaymentDaysInfo>();
            //var connectionString = GetConnectionString(keepyear, year, _sqlConnectionString);
            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, str))
            {
                while (rdr.Read())
                {
                    var fundPaymentDaysInfo = new FundPaymentDaysInfo
                    {
                        BankAccountsId = rdr[0] == DBNull.Value ? Guid.Empty : rdr.GetGuid(0),
                        BankName = rdr[1] == DBNull.Value ? string.Empty : rdr.GetString(1),
                        Jan = rdr[2] == DBNull.Value ? 0 : rdr.GetDecimal(2),
                        Feb = rdr[3] == DBNull.Value ? 0 : rdr.GetDecimal(3),
                        Mar = rdr[4] == DBNull.Value ? 0 : rdr.GetDecimal(4),
                        Apr = rdr[5] == DBNull.Value ? 0 : rdr.GetDecimal(5),
                        May = rdr[6] == DBNull.Value ? 0 : rdr.GetDecimal(6),
                        Jun = rdr[7] == DBNull.Value ? 0 : rdr.GetDecimal(7),
                        July = rdr[8] == DBNull.Value ? 0 : rdr.GetDecimal(8),
                        Aug = rdr[9] == DBNull.Value ? 0 : rdr.GetDecimal(9),
                        Sept = rdr[10] == DBNull.Value ? 0 : rdr.GetDecimal(10),
                        Oct = rdr[11] == DBNull.Value ? 0 : rdr.GetDecimal(11),
                        Nov = rdr[12] == DBNull.Value ? 0 : rdr.GetDecimal(12)
                    };
                    wasteBookList.Add(fundPaymentDaysInfo);
                }
            }
            return wasteBookList;
        }

        /// <summary>
        /// 获取公司的银行账户期末金额
        /// </summary>
        /// <param name="keepyear"></param>
        /// <param name="filialeId"></param>
        /// <param name="year"></param>
        /// <param name="bankName"></param>
        /// <returns></returns>
        public IList<FundPaymentDaysInfo> GetFundPaymentDaysBankInfos(int keepyear, Guid filialeId, int year, string bankName)
        {
            var bankSelectStr = string.Empty;
            if (filialeId == new Guid("30aa2f62-0da9-49cc-8948-4ddee198072d")) //ERP 公司
            {
                bankSelectStr =
                    string.Format(@" select BankAccountsId from lmShop_BankAccounts where IsMain=0 and IsUse=1");
            }
            else
            {
                bankSelectStr = string.Format(@" select bb.BankAccountsId from [BankAccountBinding] bb with(nolock)
					                                    where 
					                                    TargetId='{0}'", filialeId);
            }
            var bankStr = string.Empty;
            if (!string.IsNullOrEmpty(bankName)) bankStr = string.Format(@" and ls_ba.BankName like '{0}%'", bankName);
            string str = string.Format(@"select 
		                                    ls_ba.BankName as BankName,		 
		                                    isnull(i2.[1],0)  as [MaxJan]
		                                    ,isnull(i2.[2],0)  as [MaxFeb]
		                                    ,isnull(i2.[3],0)  as [MaxMar]
		                                    ,isnull(i2.[4],0)  as [MaxApr]
		                                    ,isnull(i2.[5],0)  as [MaxMay]
		                                    ,isnull(i2.[6],0)  as [MaxJun]
		                                    ,isnull(i2.[7],0)  as [MaxJuly]
		                                    ,isnull(i2.[8],0)  as [MaxAug]
		                                    ,isnull(i2.[9],0)  as [MaxSept]
		                                    ,isnull(i2.[10],0) as [MaxOct]
		                                    ,ISNULL(i2.[11],0) as[MaxNov]
		                                    ,ISNULL(I2.[12],0) as[MaxDecember] 
                                            ,ls_ba.BankAccountsId as BankAccountsId
	                                    from (
		                                    select t1.BankAccountsId,t1.dat,wb.NonceBalance as NonceBalance from lmShop_WasteBook wb 
		                                    with(nolock) inner join 
		                                    (
			                                    select  wb.BankAccountsId,MAX(DateCreated) as DateCreated,MONTH(DateCreated) as dat 
			                                    from lmShop_WasteBook wb with(nolock)
                                                inner join (
                                                    {1}
                                                ) Tab1 on wb.BankAccountsId=Tab1.BankAccountsId
			                                    where AuditingState=1 
			                                    and DateCreated   >='{0}-01-01' and DateCreated  <'{0}-12-31 23:59:59.997'
			                                    group by wb.BankAccountsId,MONTH(DateCreated)
		                                    ) t1 on wb.BankAccountsId=t1.BankAccountsId and wb.DateCreated=t1.DateCreated
		                                    where AuditingState=1
	                                    )
	                                    I1 pivot  (
	                                     sum(NonceBalance) for  Dat in ([1],[2],[3],[4],[5],[6],[7],[8],[9],[10],[11],[12])
	                                    ) I2 
	                                    inner join lmShop_BankAccounts ls_ba on i2.BankAccountsId=ls_ba.BankAccountsId and ls_ba.IsUse=1
	                                    where 1=1 {2}", year, bankSelectStr, bankStr);

            IList<FundPaymentDaysInfo> wasteBookList = new List<FundPaymentDaysInfo>();
            //var connectionString = GetConnectionString(keepyear, year, _sqlConnectionString);
            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, str))
            {
                while (rdr.Read())
                {
                    var fundPaymentDaysInfo = new FundPaymentDaysInfo
                    {
                        BankName = rdr[0] == DBNull.Value ? string.Empty : rdr.GetString(0),
                        MaxJan = rdr[1] == DBNull.Value ? 0 : rdr.GetDecimal(1),
                        MaxFeb = rdr[2] == DBNull.Value ? 0 : rdr.GetDecimal(2),
                        MaxMar = rdr[3] == DBNull.Value ? 0 : rdr.GetDecimal(3),
                        MaxApr = rdr[4] == DBNull.Value ? 0 : rdr.GetDecimal(4),
                        MaxMay = rdr[5] == DBNull.Value ? 0 : rdr.GetDecimal(5),
                        MaxJun = rdr[6] == DBNull.Value ? 0 : rdr.GetDecimal(6),
                        MaxJuly = rdr[7] == DBNull.Value ? 0 : rdr.GetDecimal(7),
                        MaxAug = rdr[8] == DBNull.Value ? 0 : rdr.GetDecimal(8),
                        MaxSept = rdr[9] == DBNull.Value ? 0 : rdr.GetDecimal(9),
                        MaxOct = rdr[10] == DBNull.Value ? 0 : rdr.GetDecimal(10),
                        MaxNov = rdr[11] == DBNull.Value ? 0 : rdr.GetDecimal(11),
                        MaxDecember = rdr[12] == DBNull.Value ? 0 : rdr.GetDecimal(12),
                        BankAccountsId = rdr[13] == DBNull.Value ? Guid.Empty : rdr.GetGuid(13)
                    };
                    wasteBookList.Add(fundPaymentDaysInfo);
                }
            }
            return wasteBookList;
        }

        #endregion
    }
}
