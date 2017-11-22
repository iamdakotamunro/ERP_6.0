using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using ERP.DAL.Interface.IInventory;
using ERP.Environment;
using Keede.DAL.Helper;
using Keede.Ecsoft.Model;

namespace ERP.DAL.Implement.Inventory
{
    /// <summary>
    /// 费用审批申报业务类
    /// </summary>
    public class CostReportAuditingPower : ICostReportAuditingPower
    {

        public CostReportAuditingPower(Environment.GlobalConfig.DB.FromType fromType)
        {

        }

        #region[SQL语句]
        /// <summary>
        /// 获取所有权限
        /// </summary>
        private const string SQL_GET_POWER = "SELECT PowerId,AuditingFilialeId,AuditingBranchId,AuditingPositionId,MinAmount,MaxAmount,ReportBranchId,Description,Kind FROM lmShop_CostReportAuditing";



        /// <summary>
        /// 添加权限
        /// </summary>
        private const string SQL_INSERT_POWER =
            @"INSERT INTO lmShop_CostReportAuditing(PowerId,AuditingFilialeId,AuditingBranchId,AuditingPositionId,MinAmount,MaxAmount,ReportBranchId,Description,Kind)
            VALUES(@PowerId,@AuditingFilialeId,@AuditingBranchId,@AuditingPositionId,@MinAmount,@MaxAmount,@ReportBranchId,@Description,@Kind)";
        /// <summary>
        /// 修改权限
        /// </summary>
        private const string SQL_UPDATE_POWER =
            @"UPDATE lmShop_CostReportAuditing SET AuditingFilialeId=@AuditingFilialeId,AuditingBranchId=@AuditingBranchId,
            AuditingPositionId=@AuditingPositionId,MinAmount=@MinAmount,MaxAmount=@MaxAmount,ReportBranchId=@ReportBranchId,Description=@Description WHERE PowerId=@PowerId";
        /// <summary>
        /// 增加申报部门权限
        /// </summary>
        private const string SQL_UPDATE_REPORT_POWER =
            @"UPDATE lmShop_CostReportAuditing SET ReportBranchId=@ReportBranchId WHERE PowerId=@PowerId";
        /// <summary>
        /// 删除权限
        /// </summary>
        private const string SQL_DELETE_POWER =
            @"DELETE lmShop_CostReportAuditing WHERE PowerId=@PowerId";
        #endregion

        #region[参数]
        /// <summary>
        /// 权限ID
        /// </summary>
        private const string PARM_POWER_ID = "@PowerId";
        /// <summary>
        /// 公司ID
        /// </summary>
        private const string PARM_AUDITING_FILIALE_ID = "@AuditingFilialeId";
        /// <summary>
        /// 部门ID
        /// </summary>
        private const string PARM_AUDITING_BRANCH_ID = "@AuditingBranchId";
        /// <summary>
        /// 职务ID
        /// </summary>
        private const string PARM_AUDITING_POSITION_ID = "@AuditingPositionId";
        /// <summary>
        /// 金额
        /// </summary>
        private const string PARM_MIN_AMOUNT = "@MinAmount";
        private const string PARM_MAX_AMOUNT = "@MaxAmount";
        /// <summary>
        /// 申报部门ID
        /// </summary>
        private const string PARM_REPORT_BRANCH_ID = "@ReportBranchId";
        /// <summary>
        /// 描述
        /// </summary>
        private const string PARM_DESCRIPTION = "@Description";
        /// <summary>
        /// 权限类型
        /// </summary>
        private const string PARM_KIND = "@Kind";
        #endregion

        #region[声明参数]
        /// <summary>
        /// 声明参数
        /// </summary>
        /// <returns></returns>
        private SqlParameter[] GetParameter()
        {
            var parms = new[]{
                new SqlParameter(PARM_POWER_ID,SqlDbType.UniqueIdentifier),
                new SqlParameter(PARM_AUDITING_FILIALE_ID,SqlDbType.UniqueIdentifier),
                new SqlParameter(PARM_AUDITING_BRANCH_ID,SqlDbType.UniqueIdentifier),
                new SqlParameter(PARM_AUDITING_POSITION_ID,SqlDbType.UniqueIdentifier),
                new SqlParameter(PARM_MIN_AMOUNT,SqlDbType.Decimal),
                new SqlParameter(PARM_MAX_AMOUNT,SqlDbType.Decimal),
                new SqlParameter(PARM_REPORT_BRANCH_ID,SqlDbType.VarChar),
                new SqlParameter(PARM_DESCRIPTION,SqlDbType.VarChar),
                new SqlParameter(PARM_KIND,SqlDbType.Int)
            };
            return parms;
        }
        #endregion

        #region[获取权限]

        public IList<string> GetCanAuditingBranchId(Guid filialeId, Guid branchId, Guid positionId)
        {
            const string SQL = @"
SELECT ReportBranchId
FROM lmShop_CostReportAuditing
WHERE AuditingFilialeId=@FilialeId
AND AuditingBranchId=@BranchId
AND AuditingPositionId=@PositionId
";
            var parms = new[]
                {
                    new Parameter("FilialeId",filialeId),
                    new Parameter("BranchId",branchId),
                    new Parameter("PositionId",positionId)

                };
            using (var db = DatabaseFactory.Create())
            {
                return db.GetValues<string>(true, SQL, parms).ToList();
            }
        }

        public IList<CostReportAuditingInfo> GetCanAuditingInfo(Guid filialeId, Guid branchId, Guid positionId)
        {
            const string SQL = @"
SELECT 
[PowerId]
      ,[AuditingFilialeId]
      ,[AuditingBranchId]
      ,[AuditingPositionId]
      ,[MinAmount]
      ,[MaxAmount]
      ,[ReportBranchId]
      ,[Description]
      ,[Kind]
FROM lmShop_CostReportAuditing
WHERE AuditingFilialeId=@FilialeId
AND AuditingBranchId=@BranchId
AND AuditingPositionId=@PositionId
";
            var parms = new[]
                {
                    new Parameter("FilialeId",filialeId),
                    new Parameter("BranchId",branchId),
                    new Parameter("PositionId",positionId)

                };
            using (var db = DatabaseFactory.Create())
            {
                return db.Select<CostReportAuditingInfo>(true, SQL, parms).ToList();
            }
        }

        /// <summary>
        /// 获取权限
        /// </summary>
        /// <returns></returns>
        public IList<CostReportAuditingInfo> GetPowerList()
        {
            IList<CostReportAuditingInfo> list = new List<CostReportAuditingInfo>();
            using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL_GET_POWER, null))
            {
                while (dr.Read())
                {
                    var info = new CostReportAuditingInfo(dr.GetGuid(0), dr.GetGuid(1), dr.GetGuid(2), dr.GetGuid(3), dr.GetDecimal(4), dr.GetDecimal(5), dr.GetString(6), dr[7] == DBNull.Value ? "" : dr.GetString(7), dr[8] == DBNull.Value ? 0 : dr.GetInt32(8));
                    list.Add(info);
                }
            }
            return list;
        }
        #endregion

        #region[添加权限]
        /// <summary>
        /// 添加权限
        /// </summary>
        /// <param name="info">权限模型</param>
        public void InsertPower(CostReportAuditingInfo info)
        {
            SqlParameter[] parms = GetParameter();
            parms[0].Value = info.PowerId;
            parms[1].Value = info.AuditingFilialeId;
            parms[2].Value = info.AuditingBranchId;
            parms[3].Value = info.AuditingPositionId;
            parms[4].Value = info.MinAmount;
            parms[4].Precision = 18;
            parms[4].Scale = 4;
            parms[5].Value = info.MaxAmount;
            parms[5].Precision = 18;
            parms[5].Scale = 4;
            parms[6].Value = info.ReportBranchId;
            parms[7].Value = info.Description;
            parms[8].Value = info.Kind;
            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL_INSERT_POWER, parms);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
        #endregion

        #region[修改权限]
        /// <summary>
        /// 修改权限
        /// </summary>
        /// <param name="info">权限模型</param>
        public void UpdatePower(CostReportAuditingInfo info)
        {
            SqlParameter[] parms = GetParameter();
            parms[0].Value = info.PowerId;
            parms[1].Value = info.AuditingFilialeId;
            parms[2].Value = info.AuditingBranchId;
            parms[3].Value = info.AuditingPositionId;
            parms[4].Value = info.MinAmount;
            parms[4].Precision = 18;
            parms[4].Scale = 4;
            parms[5].Value = info.MaxAmount;
            parms[5].Precision = 18;
            parms[5].Scale = 4;
            parms[6].Value = info.ReportBranchId;
            parms[7].Value = info.Description;
            parms[8].Value = info.Kind;
            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL_UPDATE_POWER, parms);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
        #endregion

        #region[申报部门权限]
        /// <summary>
        /// 申报部门权限
        /// </summary>
        /// <param name="info">权限模型</param>
        public void UpdateReportPower(CostReportAuditingInfo info)
        {
            var parms = new[]{
                new SqlParameter(PARM_POWER_ID,SqlDbType.UniqueIdentifier),
                new SqlParameter(PARM_REPORT_BRANCH_ID,SqlDbType.VarChar,256)
            };
            parms[0].Value = info.PowerId;
            parms[1].Value = info.ReportBranchId;
            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL_UPDATE_REPORT_POWER, parms);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
        #endregion

        #region[删除权限]
        /// <summary>
        /// 删除权限
        /// </summary>
        /// <param name="powerId">权限ID</param>
        public void DeletePower(Guid powerId)
        {
            var parm = new SqlParameter(PARM_POWER_ID, SqlDbType.UniqueIdentifier) { Value = powerId };
            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL_DELETE_POWER, parm);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
        #endregion
    }
}
