using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using ERP.DAL.Interface.IInventory;
using ERP.Environment;
using ERP.Model;
using Keede.DAL.Helper;

namespace ERP.DAL.Implement.Inventory
{
    /************************************************************************************ 
     * 创建人：  文雯
     * 创建时间：2016/7/5 9:47:07 
     * 描述    :押金回收
     * =====================================================================
     * 修改时间：2016/7/5 9:47:07 
     * 修改人  ：  
     * 描述    ：
     */
    public class CostReportDepositRecovery : ICostReportDepositRecovery
    {
        public CostReportDepositRecovery(GlobalConfig.DB.FromType fromType)
        {

        }

        #region[添加押金回收]
        /// <summary>
        /// 添加押金回收
        /// </summary>
        /// <param name="info">押金回收详细模型</param>
        public bool InsertDepositRecovery(CostReportDepositRecoveryInfo info)
        {
            const string SQL = @"
INSERT INTO lmShop_CostReportDepositRecovery(
    ReportId, DepositRecoveryReportId, RecoveryCost, RecoveryDate, RecoveryType, RecoveryRemarks,RecoveryPersonnelId
) 
VALUES(
    @ReportId, @DepositRecoveryReportId, @RecoveryCost, @RecoveryDate, @RecoveryType, @RecoveryRemarks,@RecoveryPersonnelId
)";

            var parms = new[]{
                new SqlParameter("@ReportId",SqlDbType.UniqueIdentifier), 
                new SqlParameter("@DepositRecoveryReportId",SqlDbType.UniqueIdentifier), 
                new SqlParameter("@RecoveryCost",SqlDbType.Decimal), 
                new SqlParameter("@RecoveryDate",SqlDbType.DateTime), 
                new SqlParameter("@RecoveryRemarks",SqlDbType.VarChar),
                new SqlParameter("@RecoveryPersonnelId",SqlDbType.UniqueIdentifier),
                new SqlParameter("@RecoveryType",info.RecoveryType)
            };
            parms[0].Value = info.ReportId;
            parms[1].Value = info.DepositRecoveryReportId;
            parms[2].Value = info.RecoveryCost;
            parms[3].Value = info.RecoveryDate;
            parms[4].Value = info.RecoveryRemarks;
            parms[5].Value = info.RecoveryPersonnelId;
            
            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL, parms);
                return true;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
        #endregion

        /// <summary>
        /// 根据费用申报ID获得押金回收记录
        /// </summary>
        /// <returns></returns>
        public List<CostReportDepositRecoveryInfo> GetDepositRecoveryList(Guid reportId)
        {
            const string SQL = @"select a.ReportId, a.DepositRecoveryReportId, a.RecoveryCost, a.RecoveryDate, a.RecoveryType, a.RecoveryRemarks ,a.RecoveryPersonnelId, b.ReportNo
                                    from lmShop_CostReportDepositRecovery a
                                    inner join lmShop_CostReport b
                                    on a.DepositRecoveryReportId = b.ReportId
                                    where a.ReportId=@ReportId";
            var parm = new SqlParameter("@ReportId", SqlDbType.UniqueIdentifier) { Value = reportId };

            var list = new List<CostReportDepositRecoveryInfo>();
            using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL, parm))
            {
                while (dr.Read())
                {
                    var info = new CostReportDepositRecoveryInfo
                    {
                        ReportId = dr["ReportId"] == DBNull.Value ? Guid.Empty : new Guid(dr["ReportId"].ToString()),
                        ReportNo = dr["ReportNo"] == DBNull.Value ? "" : dr["ReportNo"].ToString(),
                        DepositRecoveryReportId = dr["DepositRecoveryReportId"] == DBNull.Value ? Guid.Empty : new Guid(dr["DepositRecoveryReportId"].ToString()),
                        RecoveryCost = dr["RecoveryCost"] == DBNull.Value ? 0 : decimal.Parse(dr["RecoveryCost"].ToString()),
                        RecoveryDate = dr["RecoveryDate"] == DBNull.Value ? DateTime.Parse("1900-01-01") : DateTime.Parse(dr["RecoveryDate"].ToString()),
                        RecoveryType = dr["RecoveryType"] != DBNull.Value && bool.Parse(dr["RecoveryType"].ToString()),
                        RecoveryRemarks = dr["RecoveryRemarks"] == DBNull.Value ? "" : dr["RecoveryRemarks"].ToString(),
                        RecoveryPersonnelId = dr["RecoveryPersonnelId"] == DBNull.Value ? Guid.Empty : new Guid(dr["RecoveryPersonnelId"].ToString()),
                    };
                    list.Add(info);
                }
            }
            return list;
        }
    }
}
