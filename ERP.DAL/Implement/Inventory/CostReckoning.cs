using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using ERP.DAL.Interface.IInventory;
using Keede.Ecsoft.Model;
using Keede.DAL.Helper;
using ERP.Environment;

namespace ERP.DAL.Implement.Inventory
{
    public class CostReckoning : ICostReckoning
    {
        public CostReckoning(Environment.GlobalConfig.DB.FromType fromType) { }

        private const string SQL_INSERT_RECKONING = "INSERT INTO lmShop_CostReckoning([ReckoningId],[FilialeId],[AssumeFilialeId],[CompanyId],[TradeCode],[DateCreated],[Description],[AccountReceivable],[NonceTotalled],[ReckoningType],[ReckoningState],[AuditingState]) VALUES(@ReckoningId,@FilialeId,@AssumeFilialeId,@CompanyId,@TradeCode,@DateCreated,@Description,@AccountReceivable,@NonceTotalled,@ReckoningType,@ReckoningState,@AuditingState);";
        private const string SQL_SELECT_RECKONING = "SELECT ReckoningId,FilialeId,AssumeFilialeId,CompanyId,TradeCode,DateCreated,Description,AccountReceivable,NonceTotalled,ReckoningType,ReckoningState,IsChecked,AuditingState FROM lmShop_CostReckoning WHERE ReckoningId=@ReckoningId;";
        private const string SQL_SELECT_RECKONING_TOTALLED = "SELECT NonceTotalled FROM lmShop_CostReckoning WHERE CompanyId=@CompanyId AND AuditingState = 1 ORDER BY DateCreated DESC;";
        private const string SQL_SELECT_RECKONING_LIST = "SELECT ReckoningId,FilialeId,AssumeFilialeId,CompanyId,TradeCode,DateCreated,Description,AccountReceivable,NonceTotalled,ReckoningType,ReckoningState,IsChecked,AuditingState FROM lmShop_CostReckoning WHERE CompanyId=@CompanyId ORDER BY DateCreated DESC;";
        private const string SQL_UPDATE_RECKONING = "UPDATE lmShop_CostReckoning SET AccountReceivable = @AccountReceivable, Description=Description+@Description,DateCreated=@DateCreated WHERE ReckoningId=@ReckoningId";
        private const string SQL_DELETE_RECKONING = "DELETE lmShop_CostReckoning WHERE TradeCode = @TradeCode;";
        private const String SQL_UPDATE_DESCRIPTION = "UPDATE [lmShop_CostReckoning]  SET [Description] = [Description] + @Description WHERE [ReckoningId] = @ReckoningId";
        private const string SQL_UPDATE_RECKONING_AUDITING = @"declare @CompanyId uniqueidentifier;
declare @NonceTotalled decimal(18,4);
declare @AccountReceivable decimal(18,4);
SELECT @CompanyId=CompanyId FROM lmShop_CostReckoning WHERE TradeCode = @TradeCode;
SELECT @AccountReceivable=AccountReceivable FROM lmShop_CostReckoning WHERE TradeCode = @TradeCode;
SELECT TOP 1 @NonceTotalled=NonceTotalled FROM lmShop_CostReckoning WHERE CompanyId=@CompanyId AND AuditingState = 1 ORDER BY DateCreated DESC;
if @NonceTotalled is null
begin
set @NonceTotalled =0;
end
UPDATE lmShop_CostReckoning SET AuditingState = 1,DateCreated=@DateCreated,NonceTotalled=@NonceTotalled+@AccountReceivable WHERE TradeCode = @TradeCode;";

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
        private const String PARM_FILIALE_ID = "@FilialeId";
        private const String PARM_BRANCH_ID = "@BranchId";
        // Begin add by tianys at 2009-05-25 
        /// <summary>
        /// 按日期,账单类型获取往来账
        /// </summary>
        /// <param name="companyId">往来单位编号</param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="receiptType"></param>
        /// <param name="auditingState"></param>
        /// <param name="filialeId"></param>
        /// <param name="branchId"></param>
        /// <param name="assumeFilialeId">结算公司id</param>
        /// <returns></returns>
        public IList<CostReckoningInfo> GetReckoningList(Guid companyId, DateTime startDate, DateTime endDate, int receiptType, float auditingState, Guid filialeId, Guid branchId, Guid assumeFilialeId)
        {
            var parms = new[] {
                                            new SqlParameter(PARM_FILIALE_ID, SqlDbType.UniqueIdentifier){Value = filialeId},
                                            new SqlParameter(PARM_BRANCH_ID, SqlDbType.UniqueIdentifier){Value = branchId}
             };
            var sqlStr = new StringBuilder();
            sqlStr.Append("SELECT ReckoningId,FilialeId,AssumeFilialeId,CompanyId,TradeCode,DateCreated,Description,AccountReceivable,NonceTotalled,ReckoningType,ReckoningState,IsChecked,AuditingState FROM lmShop_CostReckoning");
            sqlStr.Append(" WHERE CompanyId='").Append(companyId).Append("'");
            sqlStr.Append(@"and companyid in (select CompanyId  from CostPermission 
 where FilialeId=@FilialeId and BranchId=@BranchId) ");
            if (startDate != DateTime.MinValue)
            {
                sqlStr.Append(" AND DateCreated >= '").Append(startDate).Append("'");
            }
            if (endDate != DateTime.MinValue)
            {
                sqlStr.Append(" AND DateCreated <= '").Append(endDate).Append("'");
            }
            if (receiptType != -1)
            {
                sqlStr.Append(" AND ReckoningType = ").Append(receiptType);
            }
            sqlStr.Append(" AND AuditingState =").Append(auditingState);
            if (assumeFilialeId!=Guid.Empty)
            {
                sqlStr.Append(" AND AssumeFilialeId ='").Append(assumeFilialeId).Append("'");
            }
            sqlStr.Append(" ORDER BY DateCreated DESC;");
            IList<CostReckoningInfo> reckoningList = new List<CostReckoningInfo>();
            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, sqlStr.ToString(), parms))
            {
                while (rdr.Read())
                {
                    var reckoningInfo = new CostReckoningInfo(rdr.GetGuid(0), rdr.GetGuid(1), rdr.GetGuid(2), rdr.GetGuid(3), rdr.GetString(4), rdr.GetDateTime(5), rdr.GetString(6), rdr.GetDecimal(7), rdr.GetDecimal(8), rdr.GetInt32(9), rdr.GetInt32(10), rdr[11] == DBNull.Value ? 0 : rdr.GetInt32(11), rdr.GetInt32(12));
                    reckoningList.Add(reckoningInfo);
                }
            }
            return reckoningList;
        }
        // End add

        /// <summary>
        /// 添加帐务记录数据
        /// </summary>
        /// <param name="reckoning">帐务记录条目类</param>
        public int Insert(CostReckoningInfo reckoning)
        {
            var parms = new[] {
                new SqlParameter(PARM_RECKONINGID, SqlDbType.UniqueIdentifier),
                new SqlParameter(PARM_FILIALEID, SqlDbType.UniqueIdentifier),
                new SqlParameter(PARM_COMPANYID,SqlDbType.UniqueIdentifier),
				new SqlParameter(PARM_TRADECODE, SqlDbType.VarChar, 32),
                new SqlParameter(PARM_DATECREATED,SqlDbType.DateTime),
                new SqlParameter(PARM_DESCRIPTION, SqlDbType.VarChar, 256),
                new SqlParameter(PARM_ACCOUNTRECEIVABLE, SqlDbType.Float),
                new SqlParameter(PARM_NONCETOTALLED, SqlDbType.Float),
				new SqlParameter(PARM_RECKONINGTYPE, SqlDbType.Int),
                new SqlParameter(PARM_RECKONINGSTATE, SqlDbType.Int),
                new SqlParameter(PARM_AUDITINGSTATE, SqlDbType.Int),
                new SqlParameter("@AssumeFilialeId", reckoning.AssumeFilialeId)
                
            };
            parms[0].Value = reckoning.ReckoningId;
            parms[1].Value = reckoning.FilialeId;
            parms[2].Value = reckoning.CompanyId;
            parms[3].Value = reckoning.TradeCode;
            parms[4].Value = reckoning.DateCreated;
            parms[5].Value = reckoning.Description;
            parms[6].Value = Math.Round(reckoning.AccountReceivable, 2); //硬性转换2位小数位
            parms[7].Value = Math.Round(reckoning.NonceTotalled, 2); //硬性转换2位小数位
            parms[8].Value = reckoning.ReckoningType;
            parms[9].Value = reckoning.ReckoningState;
            parms[10].Value = reckoning.AuditingState;


            try
            {
                return SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL_INSERT_RECKONING, parms);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// 获取指定帐务记录
        /// </summary>
        /// <param name="reckoningId">帐务记录Id</param>
        /// <returns></returns>
        public CostReckoningInfo GetReckoning(Guid reckoningId)
        {
            var parm = new SqlParameter(PARM_RECKONINGID, SqlDbType.UniqueIdentifier) {Value = reckoningId};
            CostReckoningInfo reckoningInfo;
            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL_SELECT_RECKONING, parm))
            {
                reckoningInfo = rdr.Read() ? new CostReckoningInfo(rdr.GetGuid(0), rdr.GetGuid(1), rdr.GetGuid(2), rdr.GetGuid(3), rdr.GetString(4), rdr.GetDateTime(5), rdr.GetString(6), rdr.GetDecimal(7), rdr.GetDecimal(8), rdr.GetInt32(9), rdr.GetInt32(10), rdr[11] == DBNull.Value ? 0 : rdr.GetInt32(11), rdr.GetInt32(12)) : new CostReckoningInfo();
            }
            return reckoningInfo;
        }

        /// <summary>
        /// 获取往来账
        /// </summary>
        /// <param name="companyId">往来单位编号</param>
        /// <returns></returns>
        public IList<CostReckoningInfo> GetReckoningList(Guid companyId)
        {
            var parm = new SqlParameter(PARM_COMPANYID, SqlDbType.UniqueIdentifier) {Value = companyId};
            IList<CostReckoningInfo> reckoningList = new List<CostReckoningInfo>();
            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL_SELECT_RECKONING_LIST, parm))
            {
                while (rdr.Read())
                {
                    var reckoningInfo = new CostReckoningInfo(rdr.GetGuid(0), rdr.GetGuid(1), rdr.GetGuid(2), rdr.GetGuid(3), rdr.GetString(4), rdr.GetDateTime(5), rdr.GetString(6), rdr.GetDecimal(7), rdr.GetDecimal(8), rdr.GetInt32(9), rdr.GetInt32(10), rdr[11] == DBNull.Value ? 0 : rdr.GetInt32(11), rdr.GetInt32(12));
                    reckoningList.Add(reckoningInfo);
                }
            }
            return reckoningList;
        }

        /// <summary>
        /// 获取指定往来单位的往来总帐
        /// </summary>
        /// <param name="companyId">往来单位编号</param>
        public decimal GetTotalled(Guid companyId)
        {
            var parm = new SqlParameter(PARM_COMPANYID, SqlDbType.UniqueIdentifier) {Value = companyId};
            decimal totalled = 0;
            object obj = SqlHelper.ExecuteScalar(GlobalConfig.ERP_DB_NAME, true, SQL_SELECT_RECKONING_TOTALLED, parm);
            if (obj != DBNull.Value)
                totalled = Convert.ToDecimal(obj);
            return totalled;
        }

        /// <summary>
        /// 审核费用记录
        /// </summary>
        /// <param name="tradeCode"></param>
        public void Auditing(string tradeCode)
        {
            DateTime dateTime = DateTime.Now;
            var parms = new[] {
                new SqlParameter(PARM_TRADECODE, SqlDbType.VarChar, 32),
                new SqlParameter(PARM_DATECREATED,SqlDbType.DateTime)
            };
            parms[0].Value = tradeCode;
            parms[1].Value = dateTime;

            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL_UPDATE_RECKONING_AUDITING, parms);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// 修改费用记录
        /// </summary>
        /// <param name="reckoningId"></param>
        /// <param name="accountReceivable"></param>
        /// <param name="description"></param>
        /// <param name="dateCreated"></param>
        public void Update(Guid reckoningId, decimal accountReceivable, string description, DateTime dateCreated)
        {
            var parms = new[]
            {
               new SqlParameter(PARM_RECKONINGID, SqlDbType.UniqueIdentifier),
               new SqlParameter(PARM_ACCOUNTRECEIVABLE, SqlDbType.Decimal),
               new SqlParameter(PARM_DESCRIPTION, SqlDbType.VarChar, 256),
               new SqlParameter(PARM_DATECREATED,SqlDbType.DateTime)
            };
            parms[0].Value = reckoningId;
            parms[1].Value = accountReceivable;
            parms[2].Value = description;
            parms[3].Value = dateCreated;

            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL_UPDATE_RECKONING, parms);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// 更新时累加备注信息
        /// </summary>
        /// <param name="reckoningId"></param>
        /// <param name="description"></param>
        public void UpdateDescription(Guid reckoningId, String description)
        {
            var prms = new[]
            {
                new SqlParameter(PARM_RECKONINGID,SqlDbType.UniqueIdentifier),
                new SqlParameter(PARM_DESCRIPTION,SqlDbType.VarChar,256)
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

        /// <summary>
        /// 删除费用记录
        /// </summary>
        /// <param name="tradeCode"></param>
        public void Delete(string tradeCode)
        {
            var parm = new SqlParameter(PARM_TRADECODE, SqlDbType.VarChar, 32) {Value = tradeCode};

            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL_DELETE_RECKONING, parm);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
    }
}
