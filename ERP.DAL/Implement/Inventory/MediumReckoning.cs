using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using ERP.DAL.Interface.IInventory;
using ERP.Environment;
using ERP.Model;
using Keede.DAL.Helper;

namespace ERP.DAL.Implement.Inventory
{
    public class MediumReckoning : IMediumReckoning
    {
        public MediumReckoning(Environment.GlobalConfig.DB.FromType fromType)
        {

        }

        #region  sql

        /// <summary>
        /// 添加
        /// </summary>
        private const string SQL_INSERT_DATA = @"
BEGIN TRAN
	IF EXISTS(SELECT 1 FROM lmShop_TempReckoning WHERE ReckoningId=@ReckoningId)
		BEGIN		  
          UPDATE lmShop_TempReckoning SET HandleType=0 WHERE [ReckoningId]=@ReckoningId;      
		END  
    ELSE BEGIN 		
			INSERT INTO lmShop_TempReckoning([ReckoningId],[FilialeId],[CompanyId],[TradeCode],[DateCreated],
			[Description],[AccountReceivable],[NonceTotalled],[ReckoningType],[ReckoningState],
			[IsChecked],[AuditingState],[OriginalTradeCode],[WarehouseId],[ReckoningCheckType],[CheckId],[HandleType],[DiffMoney],[IsOut],[ComCurrBalance]) 
			VALUES(@ReckoningId,@FilialeId,@CompanyId,@TradeCode,@DateCreated,@Description,@AccountReceivable,0,@ReckoningType,@ReckoningState,@IsChecked,@AuditingState,@OriginalTradeCode,@WarehouseId,@ReckoningCheckType,@CheckId,@HandleType,@DiffMoney,@IsOut,0);	
		END 
COMMIT";

        /// <summary>
        /// 添加
        /// </summary>
        private const string SQL_INSERT = @"INSERT INTO lmShop_TempReckoning([ReckoningId],[FilialeId],[CompanyId],[TradeCode],[DateCreated],
					[Description],[AccountReceivable],[NonceTotalled],[ReckoningType],[ReckoningState],[IsChecked],[AuditingState],[OriginalTradeCode],[WarehouseId],[ReckoningCheckType],[CheckId],[HandleType],[DiffMoney],[IsOut]) 
					VALUES(@ReckoningId,@FilialeId,@CompanyId,@TradeCode,@DateCreated,@Description,@AccountReceivable,
					@NonceTotalled,@ReckoningType,@ReckoningState,@IsChecked,@AuditingState,@OriginalTradeCode,@WarehouseId,@ReckoningCheckType,@CheckId,@HandleType,@DiffMoney,@IsOut)";

        /// <summary>
        /// 删除
        /// </summary>
        private const string SQL_DELETE = "DELETE lmShop_TempReckoning ";

        /// <summary>
        /// 更新往来帐
        /// </summary>
        private const string SQL_UPDATE = @"UPDATE lmShop_Reckoning SET IsChecked=T.IsChecked,[Description]=T.[Description] FROM lmShop_Reckoning R 
          INNER JOIN lmShop_TempReckoning T ON R.ReckoningId=T.ReckoningId WHERE CheckId=@CheckId AND DiffMoney=0 AND HandleType=0";

        /// <summary>
        /// 转移往来帐
        /// </summary>
        private const string SQL_TRANSFER = @"INSERT INTO lmShop_Reckoning SELECT [ReckoningId],[FilialeId],[CompanyId],[TradeCode],[DateCreated],
     [Description],[AccountReceivable],[NonceTotalled],[ReckoningType],[ReckoningState],[IsChecked],[AuditingState],[OriginalTradeCode],
      [WarehouseId],[ReckoningCheckType],ComCurrBalance,LinkTradeType,IsOut FROM lmShop_TempReckoning       
      WHERE CheckId=@CheckId AND (DiffMoney<>0 OR HandleType=2) ";

        private const string SQL_NEWTRANSFER = @"
 begin tran
    --删除临时存储的往来帐数据
	delete from CoverTempReckoning	
    DECLARE @elseFiliale UNIQUEIDENTIFIER
    --其他公司ID
	SELECT  @elseFiliale=Value FROM Config WHERE [KEY]='RECKONING_ELSE_FILIALEID'
	--1.（其他公司余额）先把需要处理的往来帐插入到临时表中
	insert into  CoverTempReckoning (RowID,ReckoningId,AccountReceivable,ComCurrBalance)	     
	 select  ROW_NUMBER() OVER(ORDER BY OrderIndex) AS RowID,
		 ReckoningId,AccountReceivable,NonceTotalled 
	 FROM  lmShop_TempReckoning WITH(NOLOCK)		
	 WHERE CheckId=@CheckId
	 AND (DiffMoney<>0 OR HandleType=2)
	 AND IsOut=0
	 ORDER BY OrderIndex ASC	 
	 declare @else_Reck int ,@else_num int,@else_AccountReceivable decimal(18,4), @else_LastComCurrBalance  decimal(18,4),@else_PreB decimal(18,4)
	 select @else_Reck= count(0) from CoverTempReckoning	 
	 select @else_LastComCurrBalance=NonceBalance from lmShop_CompanyBalanceDetail with(updlock) where  CompanyId=@CompanyId and FilialeId=@elseFiliale
	 set @else_num=1 
	 while(@else_num<=@else_Reck)
	 begin
		select @else_AccountReceivable = AccountReceivable  from CoverTempReckoning where RowID  = @else_num
		if(@else_num=1) begin
		  update  CoverTempReckoning set  ComCurrBalance=@else_AccountReceivable +@else_LastComCurrBalance
		  where  RowID  = @else_num		  
		  set  @else_PreB = @else_AccountReceivable  +@else_LastComCurrBalance
		end else begin	  
		  update  CoverTempReckoning set  ComCurrBalance=@else_AccountReceivable+@else_PreB
		  where  RowID=@else_num			  
		  set  @else_PreB=@else_AccountReceivable+@else_PreB		    
		end
		set @else_num+=1    
	 end 
	--更新临时往来帐其他公司的余额
	update  lmShop_TempReckoning set NonceTotalled =t2.ComCurrBalance
	from  lmShop_TempReckoning t1
	inner join  CoverTempReckoning t2
	on t1.ReckoningId  = t2.ReckoningId	
	 
	--2.（具体公司余额）先把需要处理的往来帐插入到临时表中
	--2.1获取当前对账记录的公司ID
     select
          row_number() over(order by FilialeId) as RowID,FilialeId
          into #FilialeId
     from  (
		 select   distinct  FilialeId   
		 from lmShop_TempReckoning
		 where  CheckId=@CheckId
		AND (DiffMoney<>0 OR HandleType=2)
		AND IsOut=1
     ) I1	
	declare @FilialeId  uniqueidentifier ,@F_num  int,@F_cnt   int, @filiale_LastComCurrBalance  decimal(18,4)
    select @F_cnt= count(0) from  #FilialeId
    set @F_num  = 1
	while(@F_num <= @F_cnt) 
	begin
		 set @filiale_LastComCurrBalance=0
		 select @FilialeId  = FilialeId  from  #FilialeId where  RowID  = @F_num
		 select @filiale_LastComCurrBalance=NonceBalance from lmShop_CompanyBalanceDetail with(updlock) where CompanyId=@CompanyId and FilialeId=@FilialeId
		 --清除临时表数据
		 delete from CoverTempReckoning
		 insert into  CoverTempReckoning (RowID,ReckoningId,AccountReceivable,ComCurrBalance)	     
		 select ROW_NUMBER() OVER(ORDER BY OrderIndex) AS RowID,
			 ReckoningId,AccountReceivable,ComCurrBalance 
		 FROM  lmShop_TempReckoning WITH(NOLOCK)		
		 WHERE CheckId=@CheckId
		 AND (DiffMoney<>0 OR HandleType=2)
		 AND IsOut=1
		 AND FilialeId=@FilialeId
		 ORDER BY OrderIndex ASC	     
		declare @filiale_Reck int ,@filiale_num int,
			@filiale_AccountReceivable decimal(18,4),@filiale_PreB decimal(18,4)
		select @filiale_Reck= count(0) from CoverTempReckoning
		set @filiale_num=1
		while(@filiale_num<=@filiale_Reck)
		begin
			select  @filiale_AccountReceivable = AccountReceivable  from CoverTempReckoning where RowID  = @filiale_num
			if(@filiale_num  = 1) begin
			  update  CoverTempReckoning set  ComCurrBalance=@filiale_AccountReceivable +@filiale_LastComCurrBalance
			  where  RowID  = @filiale_num			  
			  set  @filiale_PreB = @filiale_AccountReceivable  +@filiale_LastComCurrBalance
			end else begin	  
			  update CoverTempReckoning set  ComCurrBalance=@filiale_AccountReceivable+@filiale_PreB
			  where  RowID=@filiale_num			  
			  set @filiale_PreB=@filiale_AccountReceivable+@filiale_PreB		    
			end
			set @filiale_num+=1  
		end
		--更新临时往来帐当前公司的余额
		update  lmShop_TempReckoning set NonceTotalled =t2.ComCurrBalance
		from  lmShop_TempReckoning t1
		inner join  CoverTempReckoning t2
		on t1.ReckoningId  = t2.ReckoningId
		set @F_num +=1 
	end
 
     --3.计算总余额     
     --3.1获取当前往来单位最近一笔总余额账
     --清除临时表数据
     delete from CoverTempReckoning
	 declare @LastComCurrBalance  decimal(18,4)
	 select  @LastComCurrBalance = NonceBalance from   lmShop_CompanyBalance 
	 with(updlock)
	 where CompanyID=@CompanyId	     
	 --3.2将往来帐插入一张临时表 
	 insert into  CoverTempReckoning (RowID,ReckoningId,AccountReceivable,ComCurrBalance)	     
	 select ROW_NUMBER() OVER(ORDER BY OrderIndex) AS RowID,
		 ReckoningId,AccountReceivable,ComCurrBalance 
	 FROM  lmShop_TempReckoning WITH(NOLOCK)		
	 WHERE CheckId=@CheckId
	 AND (DiffMoney<>0 OR HandleType=2)
	 ORDER BY OrderIndex ASC	 	 
	 --3.3循环此临时往来帐数据
	 declare @CurrB decimal(18,4),@PreB decimal(18,4),@ReckoningId uniqueidentifier,
	 @AccountReceivable decimal(18,4),
	 @cnt_Reck int ,@num int
	 select @cnt_Reck= count(0) from  CoverTempReckoning  
	 set @num = 1
	 while(@num  <=  @cnt_Reck) 
	 begin
	  select @AccountReceivable  = AccountReceivable  from CoverTempReckoning where RowID  = @num   
	  if(@num  = 1) begin
		  update  CoverTempReckoning set  ComCurrBalance   = @AccountReceivable  +@LastComCurrBalance
		  where  RowID  = @num		  
		  set  @PreB   = @AccountReceivable  +@LastComCurrBalance
	  end else begin	  
		  update  CoverTempReckoning set  ComCurrBalance   = @AccountReceivable  +@PreB
		  where  RowID  = @num			  
		  set  @PreB   = @AccountReceivable  +@PreB		    
	  end
	  set @num +=1 
	 end	 
	--3.4将临时表里往来帐数据更新到对应表 
	update  lmShop_TempReckoning set ComCurrBalance =t2.ComCurrBalance
	from  lmShop_TempReckoning t1
	inner join  CoverTempReckoning t2
	on t1.ReckoningId  = t2.ReckoningId

	--4.更新到正式往来帐数据
	INSERT INTO lmShop_Reckoning SELECT [ReckoningId],[FilialeId],[CompanyId],[TradeCode],[DateCreated],
		  [Description],[AccountReceivable],[NonceTotalled],[ReckoningType],[ReckoningState],[IsChecked],[AuditingState],[OriginalTradeCode],
		  [WarehouseId],[ReckoningCheckType],ComCurrBalance,LinkTradeType,IsOut,CurrentTotalled 
		  FROM lmShop_TempReckoning       
	WHERE CheckId=@CheckId AND (DiffMoney<>0 OR HandleType=2) order by OrderIndex ASC	
	drop table #FilialeId
commit  --提交事务";


        /// <summary>
        /// 获取应收总调账金额
        /// </summary>
        private const string SQL_ACCOUNT_RECEIVABLE = "SELECT SUM(DiffMoney) FROM lmShop_TempReckoning WHERE CheckId=@CheckId AND DiffMoney>0";

        /// <summary>
        ///  获取应付总调账金额
        /// </summary>
        private const string SQL_ACCOUNT_PAYABLE = "SELECT SUM(DiffMoney) FROM lmShop_TempReckoning WHERE CheckId=@CheckId AND DiffMoney<0";

        //private const string SQL_SELECT = @"SELECT [ReckoningId],[TradeCode],[AccountReceivable],[Description] FROM lmShop_TempReckoning WHERE CheckId=@CheckId ";
        #endregion

        #region  参数
        private const string PARM_RECKONINGID = "@ReckoningId";
        private const string PARM_FILIALEID = "@FilialeId";
        private const string PARM_COMPANYID = "@CompanyId";
        private const string PARM_TRADECODE = "@TradeCode";
        private const string PARM_DATECREATED = "@DateCreated";
        private const string PARM_DESCRIPTION = "@Description";
        private const string PARM_ACCOUNTRECEIVABLE = "@AccountReceivable";
        private const string PARM_NONCETOTALLED = "@NonceTotalled";
        private const string PARM_RECKONINGTYPE = "@ReckoningType";
        private const string PARM_RECKONINGSTATE = "@ReckoningState";
        private const string PARM_AUDITINGSTATE = "@AuditingState";
        private const string PARM_IS_CHECKED = "@IsChecked";
        private const string PARM_ORIGINAL_TRADE_CODE = "@OriginalTradeCode";
        private const string PARM_WAREHOUSEID = "@WarehouseId";
        private const string PARM_RECKONGCHECKTYPE = "@ReckoningCheckType";
        private const string PARM_CHECKID = "@CheckId";
        private const string PARM_HANDLETYPE = "@HandleType";
        private const string PARM_DIFFMONEY = "@DiffMoney";
        #endregion

        /// <summary>
        /// 将往来帐插入临时往来帐表中
        /// </summary>
        /// <param name="info"></param>
        public void Insert(MediumReckoningInfo info)
        {
            var parms = new List<SqlParameter> {
                new SqlParameter(PARM_RECKONINGID, info.ReckoningId),
                new SqlParameter(PARM_FILIALEID, info.FilialeId),
                new SqlParameter(PARM_COMPANYID,info.CompanyId),
				new SqlParameter(PARM_TRADECODE, info.TradeCode),
                new SqlParameter(PARM_DATECREATED,DateTime.Now),
                new SqlParameter(PARM_DESCRIPTION, info.Description),
                new SqlParameter(PARM_ACCOUNTRECEIVABLE, Math.Round(info.AccountReceivable, 2)),
				new SqlParameter(PARM_RECKONINGTYPE, info.ReckoningType),
                new SqlParameter(PARM_RECKONINGSTATE, info.ReckoningState),
                new SqlParameter(PARM_AUDITINGSTATE,info.AuditingState),
                new SqlParameter(PARM_IS_CHECKED,info.IsChecked),
                new SqlParameter(PARM_ORIGINAL_TRADE_CODE,info.OriginalTradeCode), 
                new SqlParameter(PARM_RECKONGCHECKTYPE,info.ReckoningCheckType == 0 ? (int)Enum.ReckoningCheckType.Other : info.ReckoningCheckType),
                new SqlParameter(PARM_CHECKID,info.CheckId),
                new SqlParameter(PARM_HANDLETYPE,info.HandleType),
                new SqlParameter(PARM_DIFFMONEY,info.DiffMoney),
                new SqlParameter("@IsOut",info.IsOut)
            };
            string sql = info.HandleType != 0 ? SQL_INSERT_DATA : SQL_INSERT;
            if (info.HandleType == 0)
            {
                parms.Add(new SqlParameter(PARM_NONCETOTALLED, info.NonceTotalled));
            }
            if (info.WarehouseId == Guid.Empty)
            {
                sql = sql.Replace(",[WarehouseId]", "").Replace(",@WarehouseId", "");
            }
            else
            {
                parms.Add(new SqlParameter(PARM_WAREHOUSEID, info.WarehouseId));
            }
            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, sql, parms.ToArray());
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// 删除已修改和转移的往来帐
        /// </summary>
        /// <param name="checkId"></param>
        /// <param name="reckoningId"> </param>
        /// <param name="handleType"> </param>
        /// <param name="count"> </param>
        public void Delete(Guid checkId, Guid reckoningId, int handleType, int count)
        {
            var parms = new List<SqlParameter> { new SqlParameter(PARM_CHECKID, checkId) };
            var builder = new StringBuilder(SQL_DELETE);
            if (reckoningId != Guid.Empty)
            {
                builder.Append(" WHERE CheckId=@CheckId AND ReckoningId=@ReckoningId ");
                parms.Add(new SqlParameter(PARM_RECKONINGID, reckoningId));
            }
            else
            {
                builder.Append(count == 0
                                   ? " WHERE CheckId=@CheckId "
                                   : " WHERE ReckoningId IN(SELECT TOP {0} ReckoningId FROM lmShop_TempReckoning WHERE CheckId=@CheckId ");

                if (handleType != -1)
                {
                    builder.Append(" AND HandleType=" + handleType);
                }
                if (count != 0)
                {
                    builder.Append(" ORDER BY DateCreated )");
                }
            }
            string sql = reckoningId == Guid.Empty && count != 0 ? string.Format(builder.ToString(), count) : builder.ToString();
            if (reckoningId == Guid.Empty)
            {
                if (count == 0)
                    sql = sql.Replace(" TOP {0} ", " ");
            }
            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, sql, parms.ToArray());
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// 将临时往来帐表中的数据更新到往来帐表中
        /// </summary>
        /// <param name="checkId"></param>
        public void UpdateData(Guid checkId)
        {
            var parms = new[] { new SqlParameter(PARM_CHECKID, checkId) };
            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL_UPDATE, parms);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// 将临时往来帐中新增的往来帐批量添加到往来帐中
        /// </summary>
        /// <param name="checkId"></param>
        /// <param name="companyId">对账往来单位ID</param>
        public void TransferData(Guid checkId, Guid companyId)
        {
            var parms = new[]
            {
                new SqlParameter(PARM_CHECKID, checkId),
                new SqlParameter("@CompanyId", companyId) 
            };
            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL_NEWTRANSFER, parms);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>获取公司异常总调账金额
        /// </summary>
        /// <param name="checkId"></param>
        /// <param name="isReceivable">是否应收，(true：应收；false：应付) </param>
        /// <returns></returns>
        public IList<ExceptionReckoningInfo> GetAccountReceivableByFilialeId(Guid checkId, Boolean isReceivable)
        {
            var SQL = string.Format(@"
            WITH TEMP AS 
            (
                SELECT ISNULL(SUM(DiffMoney),0) DiffMoney,FilialeId FROM lmShop_TempReckoning AS T
                WHERE 
                CheckId=@CheckId
                AND IsOut=1
                AND DiffMoney{0}0
                GROUP BY FilialeId
            )
            SELECT T.FilialeId,F.Name AS FilialeName,T.DiffMoney FROM TEMP AS T
            LEFT JOIN Filiale F on T.FilialeId=F.ID", isReceivable ? ">" : "<");
            var parm = new SqlParameter(PARM_CHECKID, SqlDbType.UniqueIdentifier) { Value = checkId };
            IList<ExceptionReckoningInfo> exceptionReckoningList = new List<ExceptionReckoningInfo>();
            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL, parm))
            {
                while (rdr.Read())
                {
                    var reckoning = new ExceptionReckoningInfo
                    {
                        FilialeId = rdr["FilialeId"] == DBNull.Value ? Guid.Empty : new Guid(rdr["FilialeId"].ToString()),
                        FilialeName = rdr["FilialeName"] == DBNull.Value ? String.Empty : rdr["FilialeName"].ToString(),
                        DiffMoney = rdr["DiffMoney"] == DBNull.Value ? 0 : Convert.ToDecimal(rdr["DiffMoney"]),
                        IsOut = true
                    };
                    exceptionReckoningList.Add(reckoning);
                }
            }
            return exceptionReckoningList;
        }

        /// <summary>获取其他公司异常总调账金额
        /// </summary>
        /// <param name="checkId"></param>
        /// <param name="isReceivable">是否应收，(true：应收；false：应付) </param>
        /// <returns></returns>
        public ExceptionReckoningInfo GetAccountReceivableByElseFilialeId(Guid checkId, Boolean isReceivable)
        {
            var SQL = string.Format(@"SELECT SUM(DiffMoney) AS DiffMoney  FROM lmShop_TempReckoning 
                    WHERE 
                    CheckId=@CheckId
                    AND IsOut=0
                    AND DiffMoney{0}0", isReceivable ? ">" : "<");
            var parm = new SqlParameter(PARM_CHECKID, SqlDbType.UniqueIdentifier) { Value = checkId };
            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL, parm))
            {
                if (rdr.Read())
                {
                    return new ExceptionReckoningInfo
                    {
                        FilialeId = Guid.Empty,
                        FilialeName = "其他公司",
                        DiffMoney = rdr["DiffMoney"] == DBNull.Value ? 0 : Convert.ToDecimal(rdr["DiffMoney"]),
                        IsOut = false
                    };
                }
            }
            return null;
        }

        /// <summary>
        /// 获取应付款总调账金额
        /// </summary>
        /// <param name="checkId"></param>
        /// <returns></returns>
        public decimal GetAccountPayable(Guid checkId)
        {
            var parm = new SqlParameter(PARM_CHECKID, SqlDbType.UniqueIdentifier) { Value = checkId };
            decimal totalled = 0;
            object obj = SqlHelper.ExecuteScalar(GlobalConfig.ERP_DB_NAME, true, SQL_ACCOUNT_PAYABLE, parm);
            if (obj != DBNull.Value)
                totalled = Convert.ToDecimal(obj);
            return totalled;
        }

        /// <summary>
        /// 获取对应对账记录的往来帐记录
        /// </summary>
        /// <param name="checkId"></param>
        /// <param name="handleType"></param>
        /// <param name="count">查询条数</param>
        /// <returns></returns>
        public IList<MediumReckoningInfo> GetList(Guid checkId, int handleType, int count)
        {
            var parms = new List<SqlParameter> { new SqlParameter(PARM_CHECKID, checkId) };
            var builder = new StringBuilder(@"
SELECT TOP {0} [ReckoningId],[FilialeId],[CompanyId],[TradeCode],[DateCreated],
					T.[Description],[AccountReceivable],[OriginalTradeCode],[CheckId],
					[HandleType],[DiffMoney],F.Name AS FilialeName,[IsOut]					
FROM lmShop_TempReckoning AS T
LEFT JOIN  Filiale F ON T.FilialeId=F.ID
WHERE CheckId=@CheckId");
            if (handleType != -1)
            {
                builder.Append(" AND HandleType=@HandleType ");
                parms.Add(new SqlParameter(PARM_HANDLETYPE, handleType));
            }
            builder.Append(" ORDER BY DateCreated ");
            string sql = count > 0 ? string.Format(builder.ToString(), count) : builder.ToString().Replace(" TOP {0} ", " ");
            IList<MediumReckoningInfo> reckoningList = new List<MediumReckoningInfo>();
            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, sql, parms.ToArray()))
            {
                while (rdr.Read())
                {
                    var reckoning = new MediumReckoningInfo
                    {
                        ReckoningId = rdr.GetGuid(0),
                        FilialeId = rdr.GetGuid(1),
                        CompanyId = rdr.GetGuid(2),
                        TradeCode = rdr.GetString(3),
                        DateCreated = rdr.GetDateTime(4),
                        Description = rdr[5] == DBNull.Value ? "" : rdr.GetString(5),
                        AccountReceivable = rdr.GetDecimal(6),
                        OriginalTradeCode = rdr[7] == DBNull.Value ? "" : rdr.GetString(7),
                        CheckId = rdr.GetGuid(8),
                        HandleType = rdr.GetInt32(9),
                        DiffMoney = rdr.GetDecimal(10),
                        IsOut = rdr["IsOut"] != DBNull.Value && Convert.ToBoolean(rdr["IsOut"]),
                        FilialeName = rdr["FilialeName"] == DBNull.Value ? string.Empty : rdr["FilialeName"].ToString()
                    };
                    reckoningList.Add(reckoning);
                }
            }
            return reckoningList;
        }

        /// <summary>
        /// 批量插入数据
        /// </summary>
        /// <param name="addList"></param>
        /// <param name="tableName"></param>
        /// <param name="dics"></param>
        /// <returns></returns>
        public int BitchInsert(IList<MediumReckoningInfo> addList, string tableName, Dictionary<string, string> dics)
        {
            return SqlHelper.BatchInsert(GlobalConfig.ERP_DB_NAME, addList, tableName, dics);
        }

        /// <summary>对账完成删除临时往来帐数据（对账记录生成收付款后）  2015-04-16  陈重文
        /// </summary>
        /// <param name="checkId"></param>
        public void DeleteTempReckoning(Guid checkId)
        {
            var parms = new List<SqlParameter> { new SqlParameter(PARM_CHECKID, checkId) };
            const string sql = "DELETE lmShop_TempReckoning WHERE CheckId=@CheckId";
            SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, sql, parms.ToArray());
        }
    }
}
