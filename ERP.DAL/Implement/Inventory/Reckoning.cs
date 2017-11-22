using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Environment;
using ERP.Model.ASYN;
using Keede.DAL.Helper;
using Keede.Ecsoft.Model;

namespace ERP.DAL.Implement.Inventory
{
    /// <summary>▄︻┻┳═一 往来账数据层  最后修改提交  陈重文  2014-12-24  （全局新公司体系SQL更新优化，去除无用方法）
    /// </summary>
    public partial class Reckoning : IReckoning
    {
        readonly ConfigDAL _configDal;
        readonly CompanyCussent _companyCussent;
        readonly Environment.GlobalConfig.DB.FromType _fromType;
        private static Guid _elseFilialeId;

        public Reckoning(Environment.GlobalConfig.DB.FromType fromType)
        {
            _fromType = fromType;
            _configDal = new ConfigDAL(fromType);
            _companyCussent = new CompanyCussent(fromType);
            _elseFilialeId = new Guid(_configDal.GetConfigValue("RECKONING_ELSE_FILIALEID"));
        }

        #region [新增往来账]

        /// <summary>新增往来账
        /// </summary>
        /// <param name="reckoningInfo">帐务记录条目类</param>
        /// <param name="errorMessage"></param>
        public bool Insert(ReckoningInfo reckoningInfo, out string errorMessage)
        {
            reckoningInfo.IsOut = reckoningInfo.FilialeId != _elseFilialeId;
            string sqlInsertReckoning = @"
BEGIN TRAN
	DECLARE @thenonceTotal DECIMAL(18,4) 
	DECLARE @thenonceTotalDetail DECIMAL(18,4)
    DECLARE @currentTotal DECIMAL(18,2)
    SET @currentTotal=0
	--验证往来单位是否存在余额，不存在新增
	IF NOT EXISTS(SELECT NonceBalance FROM lmShop_CompanyBalance WITH(NOLOCK) WHERE CompanyId=@ThirdCompanyID)
		BEGIN 
			INSERT INTO lmShop_CompanyBalance(CompanyId,NonceBalance) VALUES (@ThirdCompanyID,0)
		END
	--验证往来单位与销售公司是否存在余额明细，不存在新增
	IF NOT EXISTS(SELECT NonceBalance FROM lmShop_CompanyBalanceDetail WITH(NOLOCK) WHERE CompanyId=@ThirdCompanyID And FilialeId=@FilialeId)
		BEGIN 
			INSERT INTO lmShop_CompanyBalanceDetail(CompanyId,FilialeId,NonceBalance) VALUES (@ThirdCompanyID,@FilialeId,0)
		END";

            var strSQL = @"
		    --直接插入已审核的往来账
			IF @AuditingState=1
				BEGIN
                    UPDATE lmShop_CompanyBalanceDetail SET @thenonceTotalDetail=NonceBalance=NonceBalance+@AccountReceivable WHERE CompanyId=@ThirdCompanyID AND FilialeId=@FilialeId;
				    	
					--更新并获取往来单位总余额
					UPDATE lmShop_CompanyBalance SET @thenonceTotal=NonceBalance=NonceBalance+@AccountReceivable WHERE CompanyId=@ThirdCompanyID;
					--插入往来账	
                    if(1=@IsChecked)
                    BEGIN
                        SET @currentTotal=@thenonceTotalDetail
                    END			
					INSERT INTO lmShop_Reckoning([ReckoningId],[FilialeId],[ThirdCompanyID],[TradeCode],[DateCreated],
					[Description],[AccountReceivable],[NonceTotalled],[ReckoningType],[State],
					[IsChecked],[AuditingState],[LinkTradeCode],[WarehouseId],[ReckoningCheckType],LinkTradeType,IsOut,[ComCurrBalance],[CurrentTotalled]) 
					VALUES(@ReckoningId,@FilialeId,@ThirdCompanyID,@TradeCode,GETDATE(),@Description,@AccountReceivable,
					@thenonceTotalDetail,@ReckoningType,@State,@IsChecked,@AuditingState,@LinkTradeCode,@WarehouseId,@ReckoningCheckType,@LinkTradeType,@IsOut,@ComCurrBalance,@currentTotal);
				END 
			ELSE --插入待审核往来账
				BEGIN 
                    SELECT @thenonceTotalDetail=NonceBalance+@AccountReceivable FROM lmShop_CompanyBalanceDetail WHERE CompanyId=@ThirdCompanyID And FilialeId=@FilialeId;
                        
					--获取并更新往来单位总余额
					SELECT @thenonceTotal=NonceBalance+@AccountReceivable FROM lmShop_CompanyBalance WHERE CompanyId=@ThirdCompanyID;	
                    if(1=@IsChecked)
                    BEGIN
                        SET @currentTotal=@thenonceTotalDetail
                    END					
					INSERT INTO lmShop_Reckoning([ReckoningId],[FilialeId],[ThirdCompanyID],[TradeCode],[DateCreated],
					[Description],[AccountReceivable],[NonceTotalled],[ReckoningType],[State],
					[IsChecked],[AuditingState],[LinkTradeCode],[WarehouseId],[ReckoningCheckType],LinkTradeType,IsOut,[ComCurrBalance],[CurrentTotalled]) 
					VALUES(@ReckoningId,@FilialeId,@ThirdCompanyID,@TradeCode,GETDATE(),@Description,@AccountReceivable,	@thenonceTotalDetail,@ReckoningType,@State,@IsChecked,@AuditingState,@LinkTradeCode,@WarehouseId,@ReckoningCheckType,@LinkTradeType,@IsOut,@ComCurrBalance,@currentTotal);
				END";

            if (!reckoningInfo.IsAllow)
            {
                sqlInsertReckoning += @"
	--验证此条往来账是否存在
	IF NOT EXISTS(SELECT ReckoningId FROM lmShop_Reckoning WITH(NOLOCK) WHERE FilialeId=@FilialeId AND ThirdCompanyID=@ThirdCompanyID AND ReckoningType=@ReckoningType AND State=@State AND AuditingState=@AuditingState AND LinkTradeCode=@LinkTradeCode AND [AccountReceivable]=@AccountReceivable)
        BEGIN " + strSQL + " END";
            }
            else
            {
                sqlInsertReckoning += strSQL;
            }

            sqlInsertReckoning += " COMMIT";

            errorMessage = string.Empty;
            var parms = new[] {
                new SqlParameter("@ReckoningId", SqlDbType.UniqueIdentifier),
                new SqlParameter("@FilialeId", SqlDbType.UniqueIdentifier),
                new SqlParameter("@ThirdCompanyID",SqlDbType.UniqueIdentifier),
                new SqlParameter("@TradeCode", SqlDbType.VarChar),
                new SqlParameter("@DateCreated",SqlDbType.DateTime),
                new SqlParameter("@Description", SqlDbType.VarChar),
                new SqlParameter("@AccountReceivable", SqlDbType.Float),
                new SqlParameter("@ReckoningType", SqlDbType.Int),
                new SqlParameter("@State", SqlDbType.Int),
                new SqlParameter("@AuditingState",SqlDbType.Int),
                new SqlParameter("@IsChecked",SqlDbType.Int),
                new SqlParameter("@LinkTradeCode",SqlDbType.VarChar),
                new SqlParameter("@WarehouseId",SqlDbType.UniqueIdentifier),
                new SqlParameter("@ReckoningCheckType",SqlDbType.Int),
                new SqlParameter("@LinkTradeType",SqlDbType.Int),
                new SqlParameter("@IsOut",SqlDbType.Int),
                new SqlParameter("@ComCurrBalance",SqlDbType.Float)
            };
            parms[0].Value = reckoningInfo.ReckoningId;
            parms[1].Value = reckoningInfo.FilialeId;
            parms[2].Value = reckoningInfo.ThirdCompanyID;
            parms[3].Value = reckoningInfo.TradeCode;
            parms[4].Value = DateTime.Now;
            parms[5].Value = reckoningInfo.Description;
            parms[6].Value = Math.Round(reckoningInfo.AccountReceivable, 2); //硬性转换2位小数位
            parms[7].Value = reckoningInfo.ReckoningType;
            parms[8].Value = reckoningInfo.State;
            parms[9].Value = reckoningInfo.AuditingState;
            parms[10].Value = reckoningInfo.IsChecked;
            parms[11].Value = reckoningInfo.LinkTradeCode;
            parms[12].Value = reckoningInfo.WarehouseId;
            parms[13].Value = reckoningInfo.ReckoningCheckType == 0 ? (int)ReckoningCheckType.Other : reckoningInfo.ReckoningCheckType;
            parms[14].Value = reckoningInfo.LinkTradeType;
            parms[15].Value = true;
            parms[16].Value = GetTotalled(reckoningInfo.ThirdCompanyID) + reckoningInfo.AccountReceivable;
            string sql = sqlInsertReckoning;
            if (reckoningInfo.WarehouseId == Guid.Empty)
            {
                sql = sql.Replace(",[WarehouseId]", "").Replace(",@WarehouseId", "");
            }
            try
            {
                return SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, sql, parms) > 0;
            }
            catch (Exception exp)
            {
                errorMessage = exp.Message;
                SAL.LogCenter.LogService.LogError("插入往来帐错误", "仓库管理", exp);
                return false;
            }
        }

        public bool IsExist(ReckoningInfo reckoningInfo, out string errorMsg)
        {
            var parms = new[] {
                new SqlParameter("@FilialeId", SqlDbType.UniqueIdentifier) {Value = reckoningInfo.FilialeId},
                new SqlParameter("@ThirdCompanyID",SqlDbType.UniqueIdentifier) {Value = reckoningInfo.ThirdCompanyID},
                new SqlParameter("@Description", SqlDbType.VarChar) {Value = reckoningInfo.Description},
                new SqlParameter("@AccountReceivable", SqlDbType.Float) {Value = reckoningInfo.AccountReceivable},
                new SqlParameter("@ReckoningType", SqlDbType.Int) {Value = reckoningInfo.ReckoningType},
                new SqlParameter("@State", SqlDbType.Int) {Value = reckoningInfo.State},
                new SqlParameter("@AuditingState",SqlDbType.Int) {Value = reckoningInfo.AuditingState},
                new SqlParameter("@LinkTradeCode",SqlDbType.VarChar) {Value = reckoningInfo.LinkTradeCode}
            };
            const string SQL = @"SELECT COUNT(*) FROM lmShop_Reckoning WITH(NOLOCK) WHERE FilialeId=@FilialeId AND ThirdCompanyID=@ThirdCompanyID AND ReckoningType=@ReckoningType AND State=@State AND AuditingState=@AuditingState AND LinkTradeCode=@LinkTradeCode AND [AccountReceivable]=@AccountReceivable AND Description=@Description";
            try
            {
                errorMsg = "";
                return SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, true, SQL, parms) > 0;
            }
            catch (Exception exp)
            {
                errorMsg = exp.Message;
                return false;
            }
        }

        /// <summary>插入异步往来账
        /// </summary>
        /// <param name="asynInfo">异步往来账模型</param>
        /// <returns></returns>
        public bool InsertAsyn(ASYNReckoningInfo asynInfo)
        {
            const string SQL = @"
    INSERT INTO ASYN_Reckoning
    ([ID],[IdentifyKey],[IdentifyId],[ReckoningFromType],[CreateTime])
    VALUES
    (@ID,@IdentifyKey,@IdentifyId,@ReckoningFromType,@CreateTime)
    ";
            using (var db = DatabaseFactory.Create())
            {
                return db.Run(SQL)
                    .AddParameter("ID", asynInfo.ID)
                    .AddParameter("IdentifyKey", asynInfo.IdentifyKey)
                    .AddParameter("IdentifyId", asynInfo.IdentifyId)
                    .AddParameter("ReckoningFromType", asynInfo.ReckoningFromType)
                    .AddParameter("CreateTime", asynInfo.CreateTime)
                    .Execute(false);
            }
        }

        #endregion

        #region [更新往来账]

        /// <summary>根据往来账记录ID修改往来账AccountReceivable，追加Description，DateCreated
        /// </summary>
        /// <param name="reckoningInfo"></param>
        public void Update(ReckoningInfo reckoningInfo)
        {
            const string SQL_UPDATE = "UPDATE lmShop_Reckoning SET AccountReceivable=@AccountReceivable,Description=Description+@Description,DateCreated=@DateCreated WHERE ReckoningId=@ReckoningId;";
            var parms = new[] {
                new SqlParameter("@AccountReceivable", SqlDbType.Float),
                new SqlParameter("@Description", SqlDbType.VarChar, 256),
                new SqlParameter("@ReckoningId", SqlDbType.UniqueIdentifier),
                new SqlParameter("@DateCreated",SqlDbType.DateTime)
            };
            parms[0].Value = reckoningInfo.AccountReceivable;
            parms[1].Value = reckoningInfo.Description;
            parms[2].Value = reckoningInfo.ReckoningId;
            parms[3].Value = reckoningInfo.DateCreated;
            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL_UPDATE, parms);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>批量修改往来账对账类型（对账服务用）
        /// </summary>
        /// <param name="lstModify">往来账集合</param>
        /// <param name="checkType">对账类型 1 已对账 0 未对账 2 异常对账</param>
        public void UpdateCheckState(IList<ReckoningInfo> lstModify, Int32 checkType)
        {
            string strQuery = String.Empty;
            var strQueryInNormal = new StringBuilder();
            foreach (ReckoningInfo rInfo in lstModify)
            {
                strQueryInNormal.Append("'").Append(rInfo.ReckoningId).Append("'").Append(",");
            }
            int lenQuestion = strQueryInNormal.Length;
            if (lenQuestion >= 1)
            {
                string str = strQueryInNormal.ToString().Substring(0, lenQuestion - 1);
                strQuery = String.Format("UPDATE [lmShop_Reckoning] SET [IsChecked]={1} WHERE ReckoningID IN ({0})", str, checkType);
            }
            SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, strQuery, null);
        }

        /// <summary>
        /// 修改往来账对账类型
        /// </summary>
        /// <param name="linkTradeCode">原始单据号</param>
        /// <param name="linkTradeType">对应单据类型</param>
        /// <param name="reckoningType">账单类型：0收入，1支出</param>
        /// <param name="reckoningCheckType">往来账对账类型</param>
        /// <param name="isChecked">对账类型 1 已对账 0 未对账 2 异常对账</param>
        /// zal 2016-06-05
        public bool UpdateCheckState(string linkTradeCode, int linkTradeType, int reckoningType, int reckoningCheckType, int isChecked)
        {
            var sql = "UPDATE [lmShop_Reckoning] SET [IsChecked]=@IsChecked WHERE LinkTradeCode =@LinkTradeCode and  LinkTradeType = @LinkTradeType  and ReckoningType = @ReckoningType and ReckoningCheckType=@ReckoningCheckType";
            var prms = new[]
            {
                new SqlParameter("@LinkTradeCode",linkTradeCode),
                new SqlParameter("@LinkTradeType",linkTradeType),
                new SqlParameter("@ReckoningType",reckoningType),
                new SqlParameter("@ReckoningCheckType",reckoningCheckType),
                new SqlParameter("@IsChecked",isChecked)

            };
            return SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, sql, prms) > 0;
        }

        /// <summary> 更新往来账 追加 Description
        /// </summary>
        /// <param name="reckoningId">往来账ID</param>
        /// <param name="description"> </param>
        public void UpdateDescription(Guid reckoningId, String description)
        {
            const string SQL_UPDATE_DESCRIPTION = "UPDATE [lmShop_Reckoning] SET  [Description] = [Description] + @Description  WHERE [ReckoningId] = @ReckoningId";
            var prms = new[]
            {
                new SqlParameter("@ReckoningId",SqlDbType.UniqueIdentifier),
                new SqlParameter("@Description",SqlDbType.VarChar,256)
            };
            prms[0].Value = reckoningId;
            prms[1].Value = description;
            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL_UPDATE_DESCRIPTION, prms);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>单据审核追加备注信息（审核人信息）
        /// </summary>
        /// <param name="tradeCode">单据编号</param>
        /// <param name="description"></param>
        public void UpdateDescriptionForAuditing(string tradeCode, string description)
        {
            const string SQL_UPDATE_DESCRIPTION_FOR_AUDITING = "UPDATE lmShop_Reckoning SET Description=Description+@Description WHERE TradeCode=@TradeCode;";
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

        /// <summary>往来账==》审核
        /// </summary>
        /// <param name="tradeCode">单据编号</param>
        public void Auditing(string tradeCode)
        {
            const string SQL_AUDITING = @"
BEGIN TRAN
declare @rowCount int;
declare @ReckoningId uniqueidentifier;
declare @ThirdCompanyID uniqueidentifier;
declare @FilialeId uniqueidentifier;
declare @AccountReceivable decimal(18,4);
declare @NonceTotalled decimal(18,4);
declare @NonceTotalledDetail decimal(18,4);
declare @tmpTbl table(ReckoningId uniqueidentifier,ThirdCompanyID uniqueidentifier,FilialeId uniqueidentifier,AccountReceivable decimal(18,4),IsChecked int);
DECLARE @IsOut BIT;
declare @isChecked int;
insert @tmpTbl(ReckoningId,ThirdCompanyID,FilialeId,AccountReceivable,IsChecked) 
select ReckoningId,ThirdCompanyID,FilialeId,AccountReceivable,IsChecked from lmShop_Reckoning where TradeCode=@TradeCode AND AuditingState <> 1;
SELECT @IsOut=IsOut FROM lmShop_Reckoning WHERE TradeCode=@TradeCode AND AuditingState <> 1
select @rowCount=count(ReckoningId) from @tmpTbl;
while(@rowCount>0)
begin
   select top 1 @ReckoningId=ReckoningId,@ThirdCompanyID=ThirdCompanyID,@FilialeId=FilialeId,@AccountReceivable=AccountReceivable,@isChecked=IsChecked from @tmpTbl;
	IF NOT EXISTS(SELECT NonceBalance FROM lmShop_CompanyBalance WHERE CompanyId=@ThirdCompanyID)
	    BEGIN 
		    INSERT INTO lmShop_CompanyBalance VALUES (@ThirdCompanyID,0)
	    END
    IF NOT EXISTS(SELECT NonceBalance FROM lmShop_CompanyBalanceDetail WHERE CompanyId=@ThirdCompanyID And FilialeId=@FilialeId)
	    BEGIN 
		    INSERT INTO lmShop_CompanyBalanceDetail VALUES (@ThirdCompanyID,@FilialeId,0)
	    END
   
   --查询当前往来单位余额
   SELECT @NonceTotalled=NonceBalance+@AccountReceivable FROM lmShop_CompanyBalance WHERE CompanyId=@ThirdCompanyID;
   --更新当前往来单位总余额
   UPDATE lmShop_CompanyBalance SET NonceBalance=@NonceTotalled WHERE CompanyId=@ThirdCompanyID;
   
	--查询当前往来单位余额明细
	SELECT @NonceTotalledDetail=NonceBalance+@AccountReceivable FROM lmShop_CompanyBalanceDetail WHERE CompanyId=@ThirdCompanyID AND FilialeId=@FilialeId;
	--更新当前往来单位余额明细
	UPDATE lmShop_CompanyBalanceDetail SET NonceBalance=@NonceTotalledDetail WHERE CompanyId=@ThirdCompanyID AND FilialeId=@FilialeId;

   --更新往来账
   if(1=@IsChecked)
   BEGIN
        UPDATE lmShop_Reckoning SET CurrentTotalled=@NonceTotalledDetail WHERE ReckoningId=@ReckoningId; 
   END 
   UPDATE lmShop_Reckoning SET NonceTotalled=@NonceTotalledDetail,AuditingState=1,DateCreated=GETDATE() WHERE ReckoningId=@ReckoningId;   
   DELETE FROM @tmpTbl WHERE ReckoningId=@ReckoningId;
   SELECT @rowCount=COUNT(ReckoningId) FROM @tmpTbl;
END 
COMMIT";

            var parm = new SqlParameter("@TradeCode", SqlDbType.VarChar, 32) { Value = tradeCode };
            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL_AUDITING, parm);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>更新往来帐状态（红冲使用）
        /// </summary>
        /// <param name="linkTradeCode">原始单据号 </param>
        public void CancellationReckoning(string linkTradeCode)
        {
            var reckoningId = GetReckoningInfoId(linkTradeCode, null);
            const string SQL = @"UPDATE lmShop_Reckoning SET [State]=@State WHERE ReckoningId=@ReckoningId";
            var parms = new[]
            {
                    new SqlParameter("@State", SqlDbType.Int){Value =(int)ReckoningStateType.Cancellation},
                    new SqlParameter("@ReckoningId", SqlDbType.UniqueIdentifier){Value =reckoningId }
            };
            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL, parms);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        #region 供应商对账使用 add by liangcanren at 2015-05-29
        /// <summary>
        ///  获取入库单对应的未对账的往来帐
        /// </summary>
        /// <param name="tradeCodeOrLinkTradeCode"></param>
        /// <returns></returns>
        public Guid GetReckoningInfoByTradeCode(string tradeCodeOrLinkTradeCode)
        {
            return GetReckoningInfoId(tradeCodeOrLinkTradeCode, (int)CheckType.NotCheck);
        }

        /// <summary>
        /// 判断入库单对应的往来帐是否存在
        /// </summary>
        /// <param name="tradeCodeOrLinkTradeCode"></param>
        /// <returns></returns>
        public bool IsExists(string tradeCodeOrLinkTradeCode)
        {
            const string SQL = "SELECT TOP 1 ReckoningId FROM lmShop_Reckoning with(nolock) WHERE LinkTradeCode=@LinkTradeCode OR TradeCode = @LinkTradeCode";
            var parm = new[]
            {
                new SqlParameter("@LinkTradeCode", tradeCodeOrLinkTradeCode)
            };
            object obj = SqlHelper.ExecuteScalar(GlobalConfig.ERP_DB_NAME, true, SQL, parm);
            return obj != DBNull.Value && obj != null && Guid.Empty.ToString() != obj.ToString();
        }

        /// <summary>
        /// 将往来帐标识为已对账
        /// </summary>
        /// <param name="reckoningid"></param>
        /// <param name="isChecked"></param>
        /// <param name="startTime"></param>
        /// <returns></returns>
        public bool UpdateReckoningIsChecked(Guid reckoningid, int isChecked, DateTime startTime)
        {
            const string SQL_DELETE = "UPDATE lmShop_Reckoning SET IsChecked=@IsChecked,CurrentTotalled=NonceTotalled WHERE ReckoningId=@ReckoningId ;";
            var parm = new[] { new SqlParameter("@IsChecked", isChecked), new SqlParameter("@ReckoningId", reckoningid) };
            return SqlHelper.ExecuteNonQuery(GlobalConfig.GetErpDbName(startTime.Year), false, SQL_DELETE, parm) > 0;
        }

        /// <summary>
        /// 获取根据日期付款未对账的往来帐总额
        /// </summary>
        /// <param name="companyId">往来单位</param>
        /// <param name="filialeId"></param>
        /// <param name="startTime">起始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="isChecked">是否对账</param>
        /// <param name="stockNos">包含入库单据</param>
        /// <param name="removerNos">排除单据</param>
        /// <returns></returns>
        public Dictionary<Guid, decimal> GetTotalledByDate(Guid companyId, Guid filialeId, DateTime startTime, DateTime endTime, int isChecked,
            IList<string> stockNos, IList<string> removerNos)
        {
            var dics = new Dictionary<Guid, decimal>();
            stockNos = stockNos.Where(act => !string.IsNullOrEmpty(act)).ToList();
            removerNos = removerNos.Where(act => !string.IsNullOrEmpty(act)).ToList();
            var builder = new StringBuilder(@"SELECT ReckoningId,AccountReceivable FROM [lmShop_Reckoning] WITH(NOLOCK) WHERE 
            ThirdCompanyID=@ThirdCompanyID AND [IsChecked]=@IsChecked AND AuditingState=1 
            AND State<>2 AND LEFT(LinkTradeCode,2) NOT IN('LI','LO','BI','BO') and (TradeCode not like 'GT%' and TradeCode not like 'AJ%') ");
            if (filialeId != Guid.Empty)
            {
                var elseReckingId = _configDal.GetConfigValue("RECKONING_ELSE_FILIALEID");
                if (elseReckingId == filialeId.ToString().ToUpper())
                {
                    builder.Append(" AND IsOut=0 ");
                }
                else
                {
                    builder.AppendFormat(" AND FilialeId='{0}' AND IsOut=1", filialeId);
                }
            }
            if (stockNos.Count > 0 || removerNos.Count > 0)
            {
                var strs = new StringBuilder();
                var stockNoStr = "'" + string.Join("','", stockNos.ToArray()) + "'";
                strs.Append(stockNoStr);

                var strss = new StringBuilder();
                var removerNoStr = "'" + string.Join("','", removerNos.ToArray()) + "'";
                strss.Append(removerNoStr);

                builder.AppendFormat(" AND (([DateCreated] BETWEEN @startDate AND @endDate {0}) {1})",
                removerNos.Count > 0 ? string.Format(" AND LinkTradeCode NOT IN({0}) ", strss) : "",
                         stockNos.Count > 0 ? string.Format(" OR LinkTradeCode IN({0}) ", strs) : " "
                    );
            }
            else
            {
                builder.Append(" AND [DateCreated] BETWEEN @startDate AND @endDate ");
            }
            var parms = new[]
            {
                new SqlParameter("@ThirdCompanyID", companyId),
                new SqlParameter("@startDate", startTime),
                new SqlParameter("@endDate", endTime),
                new SqlParameter("@IsChecked", isChecked)
            };

            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.GetErpDbName(startTime.Year), true, builder.ToString(), parms))
            {
                while (rdr.Read())
                {
                    dics.Add(rdr.GetGuid(0), rdr.GetDecimal(1));
                }
            }
            return dics;
        }

        /// <summary>
        /// 获取采购单已对账的往来帐
        /// </summary>
        /// <param name="filialeId"></param>
        /// <param name="companyId"></param>
        /// <param name="linkTradeId"></param>
        /// <param name="isChecked"></param>
        /// <param name="reckoningType"></param>
        /// <param name="linkTradeCodeType"></param>
        /// <returns></returns>
        public decimal GetTotalledAccountReceivableByLinkTradeId(Guid filialeId, Guid companyId, Guid linkTradeId, int isChecked, int reckoningType, IList<int> linkTradeCodeType)
        {
            const string SQL_SELECT_RECKONING = @"SELECT ISNULL(SUM(r.AccountReceivable),0) FROM lmShop_Reckoning as r with(nolock)
INNER JOIN StorageRecord as sr with(nolock) ON r.LinkTradeCode=sr.TradeCode
WHERE sr.FilialeId=@FilialeId
AND sr.ThirdCompanyID=@ThirdCompanyID
AND sr.LinkTradeID=@LinkTradeID 
AND r.IsChecked=1 AND r.ReckoningType=@ReckoningType AND r.LinkTradeType IN({0})";

            var parm = new[]
            {
                new SqlParameter("@FilialeId", filialeId),
                new SqlParameter("@ThirdCompanyID", companyId),
                new SqlParameter("@LinkTradeID", linkTradeId),
                new SqlParameter("@IsChecked", isChecked),
                new SqlParameter("@ReckoningType", reckoningType)
            };
            var total = SqlHelper.ExecuteScalar(GlobalConfig.ERP_DB_NAME, true,
                string.Format(SQL_SELECT_RECKONING, string.Join(",", linkTradeCodeType)), parm);
            return total != null && total != DBNull.Value ? Convert.ToDecimal(total) : 0;
        }

        public decimal GetTotalledAccountReceivable(Guid filialeId, Guid companyId, string purchasingNo, int isChecked, int reckoningType, IList<int> linkTradeCodeType)
        {
            const string SQL_SELECT_RECKONING = @"SELECT ISNULL(SUM(AccountReceivable),0) FROM lmShop_Reckoning WITH(NOLOCK) WHERE FilialeId=@FilialeId AND ThirdCompanyID=@ThirdCompanyID AND
LinkTradeCode=@LinkTradeCode AND ReckoningType=@ReckoningType AND IsChecked=@IsChecked AND LinkTradeType IN({0}) ";

            var parm = new[]
            {
                new SqlParameter("@FilialeId", filialeId),
                new SqlParameter("@ThirdCompanyID", companyId),
                new SqlParameter("@LinkTradeCode", purchasingNo),
                new SqlParameter("@IsChecked", isChecked),
                new SqlParameter("@ReckoningType", reckoningType)
            };
            var total = SqlHelper.ExecuteScalar(GlobalConfig.ERP_DB_NAME, true,
                string.Format(SQL_SELECT_RECKONING, string.Join(",", linkTradeCodeType)), parm);
            return total != null && total != DBNull.Value ? Convert.ToDecimal(total) : 0;
        }

        /// <summary>根据单据号获取往来总帐
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="linkTradeCode">往来单位编号</param>
        public decimal GetTotalledByLinkTradeCode(Guid companyId, string linkTradeCode)
        {
            const string SQL_SELECT_RECKONING_TOTALLED = "SELECT TOP 1 ComCurrBalance FROM lmShop_Reckoning WHERE ThirdCompanyID=@ThirdCompanyID AND LinkTradeCode=@LinkTradeCode ORDER BY DateCreated DESC";
            var parm = new[]
            {
                new SqlParameter("@ThirdCompanyID", companyId),
                new SqlParameter("@LinkTradeCode", linkTradeCode)
            };
            decimal totalled = 0;
            object obj = SqlHelper.ExecuteScalar(GlobalConfig.ERP_DB_NAME, true, SQL_SELECT_RECKONING_TOTALLED, parm);
            if (obj != DBNull.Value)
                totalled = Convert.ToDecimal(obj);
            return totalled;
        }
        #endregion

        #endregion

        #region [删除往来账]

        /// <summary> 根据单据编号删除往来账信息
        /// </summary>
        /// <param name="tradeCode">单据编号</param>
        public void Delete(string tradeCode)
        {
            const string SQL_DELETE = "DELETE FROM lmShop_Reckoning WHERE TradeCode=@TradeCode AND AuditingState<>1;";
            var parm = new SqlParameter("@TradeCode", SqlDbType.VarChar, 32) { Value = tradeCode };
            SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL_DELETE, parm);
        }

        /// <summary>根据异步数据ID删除异步往来账
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteAsyn(Guid id)
        {
            const string SQL = @"
DELETE ASYN_Reckoning WHERE ID=@ID
";
            using (var db = DatabaseFactory.Create())
            {
                return db.Run(SQL).AddParameter("ID", id).Execute(false);
            }
        }

        #endregion

        #region [获取往来账集合]

        /// <summary>按是否对账,日期,账单类型获取往来账（未对账）用于列表显示   (B2C服务用，已无用)
        /// </summary>
        /// <param name="companyClass">往来单位分类</param>
        /// <param name="companyId">往来单位ID</param>
        /// <param name="filialeId">公司ID</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <param name="cType">对账类型</param>
        /// <param name="auditingState">审核状态</param>
        /// <param name="receiptType">收入/支出</param>
        /// <param name="tradeCode">单据编号</param>
        /// <param name="warehouseId">仓库ID</param>
        /// <param name="keepyear">保留几年数据</param>
        /// <param name="recordCount">总记录数</param>
        /// <param name="money">金额</param>
        /// <param name="start">当前页</param>
        /// <param name="limit">每页显示条数</param>
        /// <returns></returns>
        public IList<ReckoningInfo> GetValidateDataPage(Guid companyClass, Guid companyId, Guid filialeId, DateTime startDate, DateTime endDate, CheckType cType,
            AuditingState auditingState, ReceiptType receiptType, String tradeCode, Guid warehouseId, int keepyear, long start, int limit, out int recordCount, params int[] money)
        {
            using (var db = DatabaseFactory.Create(startDate.Year))
            {
                var sqlStr = new StringBuilder();
                if (companyClass != Guid.Empty)
                {
                    sqlStr.Append(@"
SELECT 
    ReckoningId,FilialeId,R.[ThirdCompanyID],TradeCode,R.[DateCreated],R.[Description],AccountReceivable,NonceTotalled,ReckoningType,IsChecked,AuditingState,LinkTradeCode,ReckoningCheckType,IsOut,LinkTradeType 
FROM lmShop_Reckoning  AS R 
LEFT JOIN lmShop_CompanyCussent AS CCu ON R.[ThirdCompanyID] = CCu.[CompanyId] 
WHERE 1=1 AND 
CCu.[CompanyClassId] IN 
(
  SELECT [CompanyClassId] FROM [lmShop_CompanyClass] WHERE [ParentCompanyClassId] ='").Append(companyClass).Append("' OR [CompanyClassId] = '").Append(companyClass).Append("')");
                }
                else
                {
                    sqlStr.Append(
                        @"SELECT ReckoningId,FilialeId,R.[ThirdCompanyID],TradeCode,R.[DateCreated],R.[Description],AccountReceivable,NonceTotalled,ReckoningType,[State],IsChecked,AuditingState,LinkTradeCode,ReckoningCheckType,LinkTradeType FROM 
lmShop_Reckoning  AS R 
WHERE 1=1 ");
                }
                if (companyId != Guid.Empty)
                    sqlStr.Append(" AND R.[ThirdCompanyID]='").Append(companyId).Append("'");
                if (startDate != DateTime.MinValue)
                {
                    sqlStr.Append(" AND R.[DateCreated] >= '").Append(startDate).Append("'");
                }

                if (endDate != DateTime.MinValue)
                {
                    sqlStr.Append(" AND R.[DateCreated] <= '").Append(endDate).Append("'");
                }
                if (cType == CheckType.NotCheck)
                {
                    sqlStr.Append(" AND (R.[IsChecked]=").Append(0).Append(" OR R.[IsChecked] is null)");
                }
                else if (cType == CheckType.IsChecked)
                {
                    sqlStr.Append(" AND (R.[IsChecked]=").Append(1).Append(")");
                }
                else if (cType == CheckType.QueChecked)
                {
                    sqlStr.Append(" AND (R.[IsChecked]=").Append(2).Append(")");
                }

                if (filialeId != Guid.Empty)
                {
                    sqlStr.Append(" AND R.[FilialeId]='").Append(filialeId).Append("'");
                }

                sqlStr.Append(" AND R.[AuditingState]=").Append((int)auditingState).Append("");

                if (warehouseId != Guid.Empty)
                {
                    sqlStr.Append(" AND R.[WarehouseId]='").Append(warehouseId).Append("'");
                }

                if (money != null && money.Length > 0)
                {
                    if (money[0] != int.MinValue && money[1] != int.MaxValue)
                        sqlStr.Append(" AND R.[AccountReceivable] BETWEEN " + money[0] + " AND " + money[1]);
                    if (money[0] == int.MinValue)
                        sqlStr.Append(" AND R.[AccountReceivable]<" + money[1]);
                    if (money[1] == int.MaxValue)
                        sqlStr.Append(" AND R.[AccountReceivable]>" + money[0]);
                }

                if (receiptType != ReceiptType.All)
                {
                    if (receiptType == ReceiptType.Expenditure)
                        sqlStr.Append(" AND R.[AccountReceivable] <0 ");
                    else if (receiptType == ReceiptType.Income)
                        sqlStr.Append(" AND R.[AccountReceivable] >0 ");
                }
                if (!String.IsNullOrEmpty(tradeCode))
                {
                    sqlStr.Append(" AND (R.[TradeCode] = '" + tradeCode.Trim() + "' OR R.[LinkTradeCode]='" + tradeCode.Trim() + "' OR R.[Description] LIKE '%" + tradeCode.Replace("[", "[[]") + "%')");
                }
                const string ORDER_BY = " [DateCreated] DESC ";
                var pageQuery = new Keede.DAL.Helper.Sql.PageQuery(start, limit, sqlStr.ToString(), ORDER_BY);
                var pageItemInfo = db.SelectByPage<ReckoningInfo>(true, pageQuery);
                recordCount = (int)pageItemInfo.RecordCount;
                return pageItemInfo.Items.ToList();
            }
        }

        /// <summary>按是否对账,日期,账单类型获取往来账（未对账）用于列表显示   (ERP往来帐页面显示用)
        /// </summary>
        /// <param name="companyClassId">往来单位分类</param>
        /// <param name="companyId">往来单位ID</param>
        /// <param name="filialeId">公司ID</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <param name="cType">对账类型</param>
        /// <param name="auditingState">审核状态</param>
        /// <param name="receiptType">收入/支出</param>
        /// <param name="tradeCode">单据编号</param>
        /// <param name="warehouseId">仓库ID</param>
        /// <param name="keepyear">保留几年数据</param>
        /// <param name="recordCount">总记录数</param>
        /// <param name="type"> </param>
        /// <param name="isOut"></param>
        /// <param name="money">金额</param>
        /// <param name="start">当前页</param>
        /// <param name="limit">每页显示条数</param>
        /// <param name="isCheck"> </param>
        /// <returns></returns>
        public IList<ReckoningInfo> GetValidateDataPage(Guid companyClassId, Guid companyId, Guid filialeId, DateTime startDate, DateTime endDate, CheckType cType,
            AuditingState auditingState, ReceiptType receiptType, String tradeCode, Guid warehouseId, int keepyear, long start, int limit, out int recordCount, int isCheck, int type, bool? isOut, params int[] money)
        {
            using (var db = DatabaseFactory.Create(startDate.Year))
            {
                var sqlStr = new StringBuilder();
                sqlStr.Append(@"
SELECT 
	R.ReckoningId,R.FilialeId,R.[ThirdCompanyID],TradeCode,R.[DateCreated],R.[Description]
	,AccountReceivable,NonceTotalled,ComCurrBalance,ReckoningType,State,IsChecked,[AuditingState]
	,LinkTradeCode,ReckoningCheckType,LinkTradeType,CC.CompanyName
	,ISNULL(RC.Memo,'EMPTY') AS Memo,IsOut,OrderIndex,CurrentTotalled  
FROM (
    SELECT ReckoningId, FilialeId, ThirdCompanyID, TradeCode, DateCreated, Description, AccountReceivable, NonceTotalled, ReckoningType, State, IsChecked, AuditingState, LinkTradeCode, WarehouseId, ReckoningCheckType, ComCurrBalance, LinkTradeType, IsOut, OrderIndex, CurrentTotalled FROM lmShop_Reckoning WHERE 1=1
");
                if (companyId != Guid.Empty)
                {
                    sqlStr.AppendLine();
                    sqlStr.Append(" AND [ThirdCompanyID]='").Append(companyId).Append("'");
                }
                if (startDate != DateTime.MinValue)
                {
                    sqlStr.AppendLine();
                    sqlStr.Append(" AND [DateCreated] >= '").Append(startDate).Append("'");
                }
                if (endDate != DateTime.MinValue)
                {
                    sqlStr.AppendLine();
                    sqlStr.Append(" AND [DateCreated] < '").Append(endDate).Append("'");
                }
                if (isOut != null)
                {
                    sqlStr.AppendLine();
                    sqlStr.Append(" AND [IsOut] = '").Append((bool)isOut ? "1" : "0").Append("'");
                }

                if (cType == CheckType.NotCheck)
                {
                    sqlStr.AppendLine();
                    sqlStr.Append(" AND ([IsChecked]=").Append(0).Append(" OR [IsChecked] is null)");
                }
                else if (cType == CheckType.IsChecked)
                {
                    sqlStr.AppendLine();
                    sqlStr.Append(" AND ([IsChecked]=").Append(1).Append(")");
                }
                else if (cType == CheckType.QueChecked)
                {
                    sqlStr.AppendLine();
                    sqlStr.Append(" AND ([IsChecked]=").Append(2).Append(")");
                }
                if (filialeId != Guid.Empty)
                {
                    var elseReckingId = _configDal.GetConfigValue("RECKONING_ELSE_FILIALEID");
                    if (elseReckingId == filialeId.ToString().ToUpper())
                    {
                        sqlStr.Append(" AND IsOut=0 ");
                    }
                    else
                    {
                        sqlStr.AppendFormat(" AND FilialeId='{0}' AND IsOut=1", filialeId);
                    }
                }
                sqlStr.AppendLine();
                sqlStr.Append(" AND [AuditingState]=").Append((int)auditingState);

                if (isCheck != -1)
                {
                    if (isCheck == 0)
                    {
                        sqlStr.AppendLine();
                        sqlStr.Append(" AND ReckoningId NOT IN (SELECT ReckoningId from ReckoningCheck)");
                    }
                    else if (isCheck == 1)
                    {
                        sqlStr.AppendLine();
                        sqlStr.Append(" AND ReckoningId IN (SELECT ReckoningId from ReckoningCheck)");
                    }
                }

                if (warehouseId != Guid.Empty)
                {
                    sqlStr.AppendLine();
                    sqlStr.Append(" AND [WarehouseId]='").Append(warehouseId).Append("'");
                }
                if (money != null && money.Length > 0)
                {
                    if (money[0] != int.MinValue && money[1] != int.MaxValue)
                    {
                        sqlStr.AppendLine();
                        sqlStr.Append(" AND [AccountReceivable] BETWEEN " + money[0] + " AND " + money[1]);
                    }
                    if (money[0] == int.MinValue)
                    {
                        sqlStr.AppendLine();
                        sqlStr.Append(" AND [AccountReceivable]<" + money[1]);
                    }
                    if (money[1] == int.MaxValue)
                    {
                        sqlStr.AppendLine();
                        sqlStr.Append(" AND [AccountReceivable]>" + money[0]);
                    }
                }
                if (receiptType != ReceiptType.All)
                {
                    sqlStr.AppendLine();
                    if (receiptType == ReceiptType.Expenditure)
                        sqlStr.Append(" AND [AccountReceivable] <0 ");
                    else if (receiptType == ReceiptType.Income)
                        sqlStr.Append(" AND [AccountReceivable] >0 ");
                }
                if (!String.IsNullOrEmpty(tradeCode))
                {
                    sqlStr.AppendLine();
                    sqlStr.Append(" AND ([TradeCode] = '" + tradeCode.Trim() + "' OR [LinkTradeCode]='" + tradeCode.Trim() + "' OR [Description] LIKE '%" + tradeCode.Replace("[", "[[]") + "%')");
                }
                if (type != -1)
                {
                    sqlStr.AppendLine();
                    switch (type)
                    {
                        case (int)ReckoningCheckType.Carriage:
                            sqlStr.Append(" AND [ReckoningCheckType]=1");
                            break;
                        case (int)ReckoningCheckType.Collection:
                            sqlStr.Append(" AND [ReckoningCheckType]=2");
                            break;
                        case (int)ReckoningCheckType.Other:
                            sqlStr.Append(" AND [ReckoningCheckType]=3");
                            break;
                    }
                }
                sqlStr.AppendLine();
                sqlStr.Append(") R");
                if (companyClassId != Guid.Empty)
                {
                    sqlStr.AppendLine();
                    sqlStr.Append("LEFT JOIN(");
                    sqlStr.Append("SELECT CompanyId,CompanyName FROM lmShop_CompanyCussent WHERE State=1 ");
                    sqlStr.Append("AND (CompanyClassId='").Append(companyClassId).Append("' OR CompanyClassId IN (SELECT CompanyClassId FROM lmShop_CompanyClass WHERE ParentCompanyClassId='").Append(companyClassId).Append("'))");
                    sqlStr.Append(") CC ON CC.CompanyId=R.[ThirdCompanyID]");
                }
                else
                {
                    sqlStr.AppendLine();
                    sqlStr.Append("LEFT JOIN(SELECT CompanyId,CompanyName FROM lmShop_CompanyCussent WHERE State=1 ) CC ON CC.CompanyId=R.[ThirdCompanyID]");
                }
                sqlStr.AppendLine();
                sqlStr.Append("LEFT JOIN ReckoningCheck RC ON RC.ReckoningId=R.ReckoningId");

                const string ORDER_BY = " DateCreated DESC,OrderIndex DESC";

                var pageQuery = new Keede.DAL.Helper.Sql.PageQuery(start, limit, sqlStr.ToString(), ORDER_BY);
                var pageItemInfo = db.SelectByPage<ReckoningInfo>(true, pageQuery);
                recordCount = (int)pageItemInfo.RecordCount;
                return pageItemInfo.Items.ToList();

            }
        }

        /// <summary>获取往来帐集合（对账服务使用） ADD  2015-03-16  陈重文 
        /// </summary>
        /// <param name="companyId">往来单位ID</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">截至时间</param>
        /// <param name="reckoningCheckType">对账类型</param>
        /// <param name="checkType">对账状态</param>
        /// <returns></returns>
        public IList<ReckoningInfo> GetReckoningListByReconciliation(Guid companyId, DateTime startTime, DateTime endTime, ReckoningCheckType reckoningCheckType, CheckType checkType)
        {
            const string SQL = @"SELECT R.[ReckoningId],R.[FilialeId],F.Name AS FilialeName,R.[ThirdCompanyId],R.[TradeCode],R.[DateCreated],
R.[Description],R.[AccountReceivable],R.[NonceTotalled],R.[ReckoningType],
R.[State],R.[IsChecked],R.[AuditingState],
R.[LinkTradeCode],R.[ReckoningCheckType],GOD.TotalWeight  AS [Weight],GOD.CarriageFee AS [Carriage],IsOut,WarehouseId,LinkTradeType 
FROM [lmShop_Reckoning] AS R WITH(NOLOCK)
LEFT JOIN GoodsOrderDeliver GOD on R.LinkTradeCode=GOD.ExpressNo 
LEFT JOIN Filiale F ON F.ID=R.FilialeId
--往来单位ID
WHERE ThirdCompanyID=@ThirdCompanyID
--已审核
AND AuditingState=1
--时间
AND DateCreated BETWEEN @StartTime AND @EndTime 
--对账类型
AND ReckoningCheckType=@ReckoningCheckType
--对账状态 
AND IsChecked=@CheckType";
            var parms = new[]
                {
                    new Parameter("ThirdCompanyID",companyId),
                    new Parameter("StartTime",startTime),
                    new Parameter("EndTime",endTime),
                    new Parameter("ReckoningCheckType",(int)reckoningCheckType),
                    new Parameter("CheckType",(int)checkType)
                };
            using (var db = DatabaseFactory.Create())
            {
                return db.Select<ReckoningInfo>(true, SQL, parms).ToList();
            }
        }

        /// <summary>获取异步往来账
        /// </summary>
        /// <param name="top">top</param>
        /// <returns></returns>
        public IList<ASYNReckoningInfo> GetAsynList(int top)
        {
            const string SQL = @"
SELECT TOP {0} ID, IdentifyKey, IdentifyId, ReckoningFromType, CreateTime, FailMessage, IsError FROM ASYN_Reckoning
ORDER BY CreateTime
";
            using (var db = DatabaseFactory.Create())
            {
                return db.Select<ASYNReckoningInfo>(true, string.Format(SQL, top)).ToList();
            }
        }

        #endregion

        #region [获取往来账信息]

        /// <summary> 根据往来账记录ID获取往来账信息
        /// </summary>
        /// <param name="reckoningId">帐务记录Id</param>
        /// <returns></returns>
        public ReckoningInfo GetReckoning(Guid reckoningId)
        {
            const string SQL_SELECT_RECKONING = "SELECT ReckoningId,FilialeId,ThirdCompanyID,TradeCode,DateCreated,Description,AccountReceivable,NonceTotalled,ReckoningType,[State],IsChecked,AuditingState,LinkTradeCode,ReckoningCheckType,IsOut,LinkTradeType FROM lmShop_Reckoning WHERE ReckoningId=@ReckoningId;";

            var parm = new SqlParameter("@ReckoningId", SqlDbType.UniqueIdentifier) { Value = reckoningId };

            ReckoningInfo reckoningInfo;

            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL_SELECT_RECKONING, parm))
            {
                reckoningInfo = rdr.Read() ?
                    new ReckoningInfo(rdr.GetGuid(0), rdr.GetGuid(1), rdr.GetGuid(2), rdr.GetString(3), rdr.GetDateTime(4), rdr.GetString(5), rdr.GetDecimal(6), rdr.GetDecimal(7), rdr.GetInt32(8), rdr.GetInt32(9), rdr[10] == DBNull.Value ? 0 : rdr.GetInt32(10), rdr.GetInt32(11), rdr.GetValue(12) == DBNull.Value ? null : rdr.GetString(12))
                    {
                        ReckoningCheckType = rdr.GetInt32(13),
                        IsOut = rdr.GetBoolean(14),//Convert.ToBoolean(rdr["IsOut"])
                        LinkTradeType = rdr.IsDBNull(15) ? 0 : rdr.GetInt32(15)
                    } : new ReckoningInfo();
            }
            return reckoningInfo;
        }

        /// <summary>根据往来账记录ID获取往来账信息（含历史数据库）
        /// </summary>
        /// <param name="reckoningId">往来账ID</param>
        /// <param name="dateTime">时间</param>
        /// <param name="keepyear">保存今年数据</param>
        /// <returns></returns>
        public ReckoningInfo GetReckoning(Guid reckoningId, DateTime dateTime, int keepyear)
        {
            const string SQL_SELECT_RECKONING = "SELECT ReckoningId,FilialeId,ThirdCompanyID,TradeCode,DateCreated,Description,AccountReceivable,NonceTotalled,ReckoningType,[State],IsChecked,AuditingState,LinkTradeCode,ReckoningCheckType,LinkTradeType FROM lmShop_Reckoning WHERE ReckoningId=@ReckoningId;";

            var parm = new SqlParameter("@ReckoningId", SqlDbType.UniqueIdentifier) { Value = reckoningId };

            ReckoningInfo reckoningInfo;
            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.GetErpDbName(dateTime.Year), true, SQL_SELECT_RECKONING, parm))
            {
                reckoningInfo = rdr.Read() ? new ReckoningInfo(rdr.GetGuid(0), rdr.GetGuid(1), rdr.GetGuid(2), rdr.GetString(3), rdr.GetDateTime(4), rdr.GetString(5), rdr.GetDecimal(6), rdr.GetDecimal(7), rdr.GetInt32(8), rdr.GetInt32(9), rdr[10] == DBNull.Value ? 0 : rdr.GetInt32(10), rdr.GetInt32(11), rdr.GetValue(12) == DBNull.Value ? null : rdr.GetString(12))
                {
                    ReckoningCheckType = rdr.GetInt32(13),
                    LinkTradeType = rdr.IsDBNull(14) ? 0 : rdr.GetInt32(14)
                } : new ReckoningInfo();
            }
            return reckoningInfo;
        }

        /// <summary>根据往来单位ID、原始单据号、对账类型获取往来账信息
        /// </summary>
        /// <param name="companyId">往来单位ID</param>
        /// <param name="linkTradeCode">原始单据号</param>
        /// <param name="checkType">对账类型</param>
        /// <param name="reckoningCheckType"></param>
        /// <returns></returns>
        public ReckoningInfo GetReckoningInfo(Guid companyId, string linkTradeCode, CheckType checkType, int reckoningCheckType = 2)
        {
            ReckoningInfo rckInfo = null;

            var parms = new[]{
                new SqlParameter("@LinkTradeCode",linkTradeCode),
                new SqlParameter("@IsChecked",(int)checkType),
                new SqlParameter("@ThirdCompanyID",companyId),
                new SqlParameter("@ReckoningCheckType",reckoningCheckType)
            };
            const string SQL = @"SELECT TOP 1 ReckoningId,FilialeId,ThirdCompanyID,TradeCode,DateCreated,Description,AccountReceivable,NonceTotalled,ReckoningType,[State],IsChecked,AuditingState,LinkTradeCode,[WarehouseId],[ReckoningCheckType],LinkTradeType FROM lmShop_Reckoning WHERE ThirdCompanyID=@ThirdCompanyID AND LinkTradeCode=@LinkTradeCode AND IsChecked=@IsChecked AND ReckoningCheckType=@ReckoningCheckType ORDER BY DateCreated DESC";
            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL, parms))
            {
                if (rdr.Read())
                {
                    rckInfo = new ReckoningInfo(rdr.GetGuid(0), rdr.GetGuid(1), rdr.GetGuid(2), rdr.GetString(3), rdr.GetDateTime(4), rdr.GetString(5), rdr.GetDecimal(6), rdr.GetDecimal(7), rdr.GetInt32(8), rdr.GetInt32(9), rdr[10] == DBNull.Value ? 0 : rdr.GetInt32(10), rdr.GetInt32(11), rdr.GetValue(12) == DBNull.Value ? null : rdr.GetString(12))
                    {
                        WarehouseId = rdr.IsDBNull(13) ? Guid.Empty : rdr.GetGuid(13),
                        ReckoningCheckType = rdr.GetInt32(14),
                        LinkTradeType = rdr.IsDBNull(15) ? 0 : rdr.GetInt32(15)
                    };
                }
            }
            return rckInfo;
        }

        /// <summary>根据原始单据号获取往来账信息
        /// </summary>
        /// <param name="linkTradeCode">原始单据号</param>
        /// <returns></returns>
        public ReckoningInfo GetReckoningInfo(string linkTradeCode)
        {
            const string SQL_SELECT_RECKONING = "SELECT ReckoningId,FilialeId,ThirdCompanyID,TradeCode,DateCreated,Description,AccountReceivable,NonceTotalled,ReckoningType,[State],IsChecked,AuditingState,LinkTradeCode,ReckoningCheckType,LinkTradeType FROM lmShop_Reckoning WHERE TradeCode=@TradeCode;";

            var parm = new SqlParameter("@TradeCode", SqlDbType.VarChar) { Value = linkTradeCode };

            ReckoningInfo reckoningInfo;

            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL_SELECT_RECKONING, parm))
            {
                reckoningInfo = rdr.Read() ?
                    new ReckoningInfo(rdr.GetGuid(0), rdr.GetGuid(1), rdr.GetGuid(2), rdr.GetString(3), rdr.GetDateTime(4), rdr.GetString(5), rdr.GetDecimal(6), rdr.GetDecimal(7), rdr.GetInt32(8), rdr.GetInt32(9), rdr[10] == DBNull.Value ? 0 : rdr.GetInt32(10), rdr.GetInt32(11), rdr.GetValue(12) == DBNull.Value ? null : rdr.GetString(12))
                    {
                        ReckoningCheckType = rdr.GetInt32(13),
                        LinkTradeType = rdr.IsDBNull(14) ? 0 : rdr.GetInt32(14)
                    } : new ReckoningInfo();
            }
            return reckoningInfo;
        }

        #endregion

        #region [获取往来总账]

        /// <summary>根据往来单位ID获取往来总帐
        /// </summary>
        /// <param name="companyId">往来单位编号</param>
        public decimal GetTotalled(Guid companyId)
        {
            const string SQL_SELECT_RECKONING_TOTALLED = "SELECT NonceBalance FROM lmShop_CompanyBalance WHERE CompanyId=@CompanyId ";
            var parm = new SqlParameter("@CompanyId", SqlDbType.UniqueIdentifier) { Value = companyId };
            decimal totalled = 0;
            object obj = SqlHelper.ExecuteScalar(GlobalConfig.ERP_DB_NAME, true, SQL_SELECT_RECKONING_TOTALLED, parm);
            if (obj != DBNull.Value)
                totalled = Convert.ToDecimal(obj);
            return totalled;
        }

        /// <summary>根据往来单位，公司，日期，获取往来总帐（收款获取）
        /// </summary>
        /// <param name="companyId">往来单位ID</param>
        /// <param name="filialeId">公司ID</param>
        /// <param name="endDate">日期 </param>
        /// <returns></returns>
        public decimal GetReckoningNonceTotalledByFilialeId(Guid companyId, Guid filialeId, DateTime endDate)
        {
            //是否是往来单位
            var isCompanyInfo = _companyCussent.IsExistCompanyInfo(companyId);
            var sqlStr = new StringBuilder();
            SqlParameter[] sqlParams;
            if (isCompanyInfo)
            {
                sqlStr.Append(" SELECT  NonceBalance FROM lmShop_CompanyBalanceDetail  WHERE [CompanyId] = @CompanyId AND [FilialeId]=@FilialeId ");
                sqlParams = new[] { new SqlParameter("@CompanyId", companyId), new SqlParameter("@FilialeId", filialeId) };
            }
            else
            {
                sqlStr.Append(@"SELECT TOP 1 ComCurrBalance FROM lmShop_Reckoning WHERE 
                                            ThirdCompanyID=@CompanyId
                                            AND [State]=1
                                            ORDER BY DateCreated DESC");
                sqlParams = new[] { new SqlParameter("@CompanyId", companyId) };
            }
            decimal totalled = 0;
            var obj = SqlHelper.ExecuteScalar(GlobalConfig.ERP_DB_NAME, true, sqlStr.ToString(), sqlParams);
            if (obj != DBNull.Value)
                totalled = Convert.ToDecimal(obj);
            return totalled;
        }


        /// <summary>根据往来单位，公司，获取往来总帐（出库审核）
        /// </summary>
        /// <param name="companyId">往来单位ID</param>
        /// <param name="filialeId">公司ID</param>
        /// <returns></returns>
        public decimal GetNonceTotalled(Guid companyId, Guid filialeId)
        {
            //是否是往来单位
            var isCompanyInfo = _companyCussent.IsExistCompanyInfo(companyId);
            var sqlStr = new StringBuilder();
            SqlParameter[] sqlParams = null;
            if (isCompanyInfo)
            {
                sqlStr.Append(" SELECT  NonceBalance FROM lmShop_CompanyBalanceDetail  WHERE [CompanyId] = @CompanyId AND [FilialeId]=@FilialeId ");
                sqlParams = new[] { new SqlParameter("@CompanyId", companyId), new SqlParameter("@FilialeId", filialeId) };
            }
            decimal totalled = 0;
            var obj = SqlHelper.ExecuteScalar(GlobalConfig.ERP_DB_NAME, true, sqlStr.ToString(), sqlParams);
            if (obj != DBNull.Value)
                totalled = Convert.ToDecimal(obj);
            return totalled;
        }

        /// <summary>根据公司、往来单位、日期、单据状态获取往来总账（付款获取）
        /// </summary>
        /// <param name="filialeId">公司ID </param>
        /// <param name="companyId">往来单位</param>
        /// <param name="endDate">结束日期</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="reckoningState">账务类型</param>
        /// <returns></returns>
        public decimal GetReckoningNonceTotalled(Guid filialeId, Guid companyId, DateTime startDate, DateTime endDate, int reckoningState)
        {
            //是否是往来单位
            var isCompanyInfo = _companyCussent.IsExistCompanyInfo(companyId);
            var sqlStr = new StringBuilder();
            if (isCompanyInfo)
            {
                sqlStr.Append(@"SELECT TOP 1 ISNULL(NonceTotalled,0) as NonceTotalled FROM [lmShop_Reckoning] WHERE 
            ThirdCompanyID=@ThirdCompanyID
            AND [DateCreated] BETWEEN @startDate AND  @endDate
            AND [State]=@State");
            }
            else
            {
                sqlStr.Append(@"SELECT TOP 1 ISNULL(ComCurrBalance,0) as NonceTotalled FROM [lmShop_Reckoning] WHERE 
            ThirdCompanyID=@ThirdCompanyID
            AND [DateCreated] BETWEEN @startDate AND  @endDate
            AND [State]=@State");
            }

            var sqlParams = new[] {
                new SqlParameter("@FilialeId",filialeId),
                new SqlParameter("@State",reckoningState),
                new SqlParameter("@startDate",startDate),
                new SqlParameter("@endDate",endDate),
                new SqlParameter("@ThirdCompanyID",companyId)
            };
            if (filialeId != Guid.Empty)
            {
                sqlStr.Append(" AND [FilialeId]='").Append(filialeId).Append("'");
            }
            sqlStr.Append(" ORDER BY [DateCreated] DESC");
            decimal totalled = 0;
            object obj = SqlHelper.ExecuteScalar(GlobalConfig.ERP_DB_NAME, true, sqlStr.ToString(), sqlParams);
            if (obj != DBNull.Value)
                totalled = Convert.ToDecimal(obj);
            return totalled;
        }

        #endregion

        #region [获取往来账单据类型 （0收入，1支出，-1未获取到）]

        /// <summary> 根据往来账ID获取往来账单据类型（0收入，1支出，-1未获取到）
        /// </summary>
        /// <param name="reckoningId"></param>
        public int GetReckoningType(Guid reckoningId)
        {
            const string SQL_SELECT_RECKONINGTYPE = "SELECT ReckoningType FROM lmShop_Reckoning WHERE ReckoningId=@ReckoningId;";
            var parm = new SqlParameter("@ReckoningId", SqlDbType.UniqueIdentifier) { Value = reckoningId };
            int reckoningType = -1;
            object obj = SqlHelper.ExecuteScalar(GlobalConfig.ERP_DB_NAME, true, SQL_SELECT_RECKONINGTYPE, parm);
            if (obj != DBNull.Value)
            {
                reckoningType = Convert.ToInt32(obj);
            }
            return reckoningType;
        }

        #endregion

        #region [判断往来账是否存在]

        /// <summary>根据公司，往来单位，单据类型，原始单据号判断是否存在该往来账
        /// </summary>
        /// <param name="filialeID">公司ID</param>
        /// <param name="companyID">往来单位ID</param>
        /// <param name="reckoningType">往来账单位类型</param>
        /// <param name="linkTradeCode">原始单据号</param>
        /// <returns></returns>
        public bool Exists(Guid filialeID, Guid companyID, int reckoningType, string linkTradeCode)
        {
            const string SQL = @"
IF EXISTS(SELECT 1 FROM [lmShop_Reckoning] WITH(NOLOCK) WHERE ThirdCompanyID=@ThirdCompanyID AND FilialeId=@FilialeId AND LinkTradeCode=@LinkTradeCode AND ReckoningType=@ReckoningType)
	BEGIN
		SELECT 1
	END
ELSE
	BEGIN
		SELECT 0
	END
";
            using (var db = DatabaseFactory.Create())
            {
                return db.GetValue<int>(true, SQL, new Parameter("FilialeId", filialeID), new Parameter("ThirdCompanyID", companyID), new Parameter("ReckoningType", reckoningType), new Parameter("LinkTradeCode", linkTradeCode)) == 1;
            }
        }

        #endregion

        #region [获取往来账ID]

        /// <summary>获取往来单位按日期最近的一条记录ID，用于记录差额说明
        /// </summary>
        /// <param name="filialeId">公司ID</param>
        /// <param name="companyId">往来单位ID</param>
        /// <param name="startTime">开始日期</param>
        /// <param name="endTime">截至日期</param>
        /// <returns></returns>
        public Guid GetReckoningInfoByDateLast(Guid filialeId, Guid companyId, DateTime startTime, DateTime endTime)
        {
            var sqlStr = new StringBuilder();
            sqlStr.Append(@"  
SELECT TOP 1 ReckoningId FROM [lmShop_Reckoning] WHERE 
            ThirdCompanyID=@ThirdCompanyID
            AND [DateCreated] BETWEEN @StartDate AND  @EndDate
            AND [State]=1");

            if (filialeId != Guid.Empty)
            {
                sqlStr.Append(" AND [FilialeId]='").Append(filialeId).Append("'");
            }

            sqlStr.Append("ORDER BY [DateCreated] DESC");
            var sqlParams = new[] {
                new SqlParameter("@ThirdCompanyID",companyId),
                new SqlParameter("@StartDate",startTime),
                new SqlParameter("@EndDate",endTime)
            };
            Guid reckoingId = Guid.Empty;
            object obj = SqlHelper.ExecuteScalar(GlobalConfig.ERP_DB_NAME, true, sqlStr.ToString(), sqlParams);
            if (obj != null)
            {
                reckoingId = new Guid(obj.ToString());
            }
            return reckoingId;
        }

        /// <summary>根据单据编号或原始单据号获取往来帐ID
        /// </summary>
        /// <param name="tradeCode"></param>
        /// <param name="isChecked"></param>
        /// <returns></returns>
        public Guid GetReckoningInfoId(string tradeCode, int? isChecked)
        {
            Guid reckoingId = Guid.Empty;
            var otehrBuilder = new StringBuilder(@"SELECT TOP 1 ReckoningId FROM [lmShop_Reckoning] with(nolock) WHERE (LinkTradeCode=@LinkTradeCode OR TradeCode=@LinkTradeCode )");
            if (isChecked != null)
            {
                otehrBuilder.AppendFormat(" AND IsChecked={0}", isChecked);
            }
            var sqlParams1 = new[] { new SqlParameter("@LinkTradeCode", tradeCode) };
            object obj1 = SqlHelper.ExecuteScalar(GlobalConfig.ERP_DB_NAME, true, otehrBuilder.ToString(), sqlParams1);
            if (obj1 != null && obj1 != DBNull.Value)
            {
                reckoingId = new Guid(obj1.ToString());
            }
            return reckoingId;
        }

        #endregion

        #region

        /// <summary>
        /// 根据采购单更新对应的往来帐对帐状态
        /// </summary>
        /// <param name="purchaseOrders"></param>
        /// <param name="thirdCompanyId"></param>
        /// <param name="startTime"></param>
        /// <param name="filialeId"></param>
        /// <returns></returns>
        public bool CheckByPurchaseOrder(IEnumerable<string> purchaseOrders, Guid filialeId, Guid thirdCompanyId, DateTime startTime)
        {
            const string SQL_CHECK = @"UPDATE lmShop_Reckoning SET IsChecked=@IsChecked,CurrentTotalled=NonceTotalled WHERE FilialeId=@FilialeId AND 
            ThirdCompanyID=@ThirdCompanyID AND (LinkTradeCode IN(select TradeCode from StorageRecord where FilialeId=@FilialeId AND 
            ThirdCompanyID=@ThirdCompanyID AND LinkTradeCode IN('{0}')) OR LinkTradeCode IN(select O.TradeCode from InnerPurchaseRelation as ip WITH(NOLOCK)
inner join StorageRecord as I WITH(NOLOCK) on ip.InStockId=I.StockId
inner join StorageRecord as O WITH(NOLOCK) on ip.OutStockId=O.StockId
where I.LinkTradeCode IN('{0}'))); ";

            var parm = new[]
            {
                new SqlParameter("@IsChecked", (int)CheckType.IsChecked),
                new SqlParameter("@FilialeId", filialeId),new SqlParameter("@ThirdCompanyID", thirdCompanyId)
            };
            return SqlHelper.ExecuteNonQuery(GlobalConfig.GetErpDbName(startTime.Year), false, string.Format(SQL_CHECK, string.Join("','", purchaseOrders)), parm) > 0;
        }

        /// <summary>
        /// 根据出入库单据列表更新对应的往来帐对帐状态
        /// </summary>
        /// <param name="tradeCodes"></param>
        /// <param name="thirdCompanyId"></param>
        /// <param name="startTime"></param>
        /// <param name="filialeId"></param>
        /// <returns></returns>
        public bool CheckByStorageTradeCode(IEnumerable<string> tradeCodes, Guid filialeId, Guid thirdCompanyId, DateTime startTime)
        {
            const string SQL_CHECK = @"UPDATE lmShop_Reckoning SET IsChecked=@IsChecked,CurrentTotalled=NonceTotalled WHERE FilialeId=@FilialeId AND ThirdCompanyID=@ThirdCompanyID AND (LinkTradeCode IN('{0}') OR LinkTradeCode IN(select O.TradeCode from InnerPurchaseRelation as ip WITH(NOLOCK)
inner join StorageRecord as I WITH(NOLOCK) on ip.InStockId=I.StockId
inner join StorageRecord as O WITH(NOLOCK) on ip.OutStockId=O.StockId
where I.TradeCode IN('{0}')))  ";

            var parm = new[]
            {
                new SqlParameter("@IsChecked", (int)CheckType.IsChecked),
                new SqlParameter("@FilialeId", filialeId),new SqlParameter("@ThirdCompanyID", thirdCompanyId)
            };
            return SqlHelper.ExecuteNonQuery(GlobalConfig.GetErpDbName(startTime.Year), false, string.Format(SQL_CHECK, string.Join("','", tradeCodes)), parm) > 0;
        }

        public bool CheckByDate(Guid companyId, Guid filialeId, DateTime startTime, DateTime endTime, int isChecked,
            IList<string> stockNos, IList<string> removerNos)
        {
            stockNos = stockNos.Where(act => !string.IsNullOrEmpty(act)).ToList();
            removerNos = removerNos.Where(act => !string.IsNullOrEmpty(act)).ToList();
            var builder = new StringBuilder(@" UPDATE lmShop_Reckoning SET IsChecked=" + (int)CheckType.IsChecked + @",CurrentTotalled=NonceTotalled WHERE FilialeId=@FilialeId AND 
            ThirdCompanyID=@ThirdCompanyID AND [IsChecked]=@IsChecked AND AuditingState=1 
            AND State<>2 AND LEFT(LinkTradeCode,2) NOT IN('LI','LO','BI','BO') and (TradeCode not like 'GT%' and TradeCode not like 'AJ%') ");
            if (stockNos.Count > 0 || removerNos.Count > 0)
            {
                builder.AppendFormat(" AND (([DateCreated] BETWEEN @startDate AND @endDate {0}) {1})",
                removerNos.Count > 0 ? string.Format(@" AND LinkTradeCode NOT IN('{0}') AND LinkTradeCode NOT IN(SELECT O.TradeCode from InnerPurchaseRelation as ip WITH(NOLOCK)
inner join StorageRecord as I WITH(NOLOCK) on ip.InStockId=I.StockId
inner join StorageRecord as O WITH(NOLOCK) on ip.OutStockId=O.StockId
where I.TradeCode in ('{0}')) ", string.Join("','", removerNos)) : "",
                         stockNos.Count > 0 ? string.Format(@" OR LinkTradeCode IN('{0}') OR LinkTradeCode IN(SELECT O.TradeCode from InnerPurchaseRelation as ip WITH(NOLOCK)
inner join StorageRecord as I WITH(NOLOCK) on ip.InStockId = I.StockId
inner join StorageRecord as O WITH(NOLOCK) on ip.OutStockId = O.StockId
where I.TradeCode in ('{0}'))", string.Join("','", stockNos)) : " "
                    );
            }
            else
            {
                builder.Append(" AND [DateCreated] BETWEEN @startDate AND @endDate ");
            }
            var parms = new[]
            {
                new SqlParameter("@FilialeId", filialeId),
                new SqlParameter("@ThirdCompanyID", companyId),
                new SqlParameter("@startDate", startTime),
                new SqlParameter("@endDate", endTime),
                new SqlParameter("@IsChecked", isChecked)
            };

            return SqlHelper.ExecuteNonQuery(GlobalConfig.GetErpDbName(startTime.Year), false, builder.ToString(), parms) > 0;
        }

        #endregion
    }
}