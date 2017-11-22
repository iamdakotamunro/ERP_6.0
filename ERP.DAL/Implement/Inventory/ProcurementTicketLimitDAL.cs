using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using ERP.DAL.Interface.IInventory;
using ERP.Environment;
using ERP.Model;
using Keede.DAL.Helper;

namespace ERP.DAL.Implement.Inventory
{
    /// <summary>▄︻┻┳═一 采购索票额度数据层   ADD 2014-12-19  陈重文
    /// </summary>
    public class ProcurementTicketLimitDAL : IProcurementTicketLimit
    {
        public ProcurementTicketLimitDAL(Environment.GlobalConfig.DB.FromType fromType)
        {

        }
        
        /// <summary>保存公司具体年份和月份的采购索票额度
        /// </summary>
        /// <param name="procurementTicketLimitInfo">采购索票额度</param>
        /// <returns>Return：true/false</returns>
        public bool SaveProcurementTicketLimit(ProcurementTicketLimitInfo procurementTicketLimitInfo)
        {
            const string SQL = @"
IF NOT EXISTS(SELECT TakerTicketLimit FROM [ProcurementTicketLimit] WHERE FilialeId=@FilialeId AND CompanyId=@CompanyId AND  DateYear=@DateYear AND DateMonth=@DateMonth)
    INSERT INTO [ProcurementTicketLimit](FilialeId,CompanyId,DateYear,DateMonth,TakerTicketLimit) VALUES(@FilialeId,@CompanyId,@DateYear,@DateMonth,@TakerTicketLimit);
ELSE
    UPDATE [ProcurementTicketLimit] SET TakerTicketLimit=@TakerTicketLimit WHERE FilialeId=@FilialeId AND CompanyId=@CompanyId AND DateYear=@DateYear AND DateMonth=@DateMonth;";
            var parms = new[]
                           {
                               new SqlParameter("@FilialeId", SqlDbType.UniqueIdentifier),
                               new SqlParameter("@CompanyId", SqlDbType.UniqueIdentifier),
                               new SqlParameter("@DateYear", SqlDbType.Int),
                               new SqlParameter("@DateMonth", SqlDbType.Int),
                               new SqlParameter("@TakerTicketLimit", SqlDbType.Decimal)
                           };
            try
            {
                parms[0].Value = procurementTicketLimitInfo.FilialeId;
                parms[1].Value = procurementTicketLimitInfo.CompanyId;
                parms[2].Value = procurementTicketLimitInfo.DateYear;
                parms[3].Value = procurementTicketLimitInfo.DateMonth;
                parms[4].Value = procurementTicketLimitInfo.TakerTicketLimit;
                return SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL, parms) > 0;
            }
            catch (Exception exp)
            {
                SAL.LogCenter.LogService.LogError(string.Format("采购索票额度保存失败,FilialeId={0},CompanyId={1},DateYear={2},DateMonth={3},TakerTicketLimit={4}", procurementTicketLimitInfo.FilialeId, procurementTicketLimitInfo.CompanyId, procurementTicketLimitInfo.DateYear, procurementTicketLimitInfo.DateMonth, procurementTicketLimitInfo.TakerTicketLimit), "仓库管理", exp);
                return false;
            }
        }

        /// <summary>获取所有公司采购索取额度
        /// </summary>
        /// <param name="dateYear">年份</param>
        /// <param name="dateMonth">月份</param>
        /// <returns>Return:采购索票额度集合</returns>
        public IList<ProcurementTicketLimitInfo> GetProcurementTicketLimitList(int dateYear, int dateMonth)
        {
            const string SQL = @" SELECT SUM(TakerTicketLimit) AS TotalTakerTicketLimit,FilialeId FROM [ProcurementTicketLimit]  
  WHERE DateYear=@DateYear AND DateMonth=@DateMonth  
  GROUP BY FilialeId";
            var parms = new[] {
                new SqlParameter("@DateYear", SqlDbType.Int){Value = dateYear},
                new SqlParameter("@DateMonth", SqlDbType.Int){Value = dateMonth}
             };
            IList<ProcurementTicketLimitInfo> list = new List<ProcurementTicketLimitInfo>();
            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL, parms))
            {
                while (rdr.Read())
                {
                    var info = new ProcurementTicketLimitInfo
                    {
                        FilialeId = new Guid(rdr["FilialeId"].ToString()),
                        TotalTakerTicketLimit = Convert.ToDecimal(rdr["TotalTakerTicketLimit"])
                    };
                    list.Add(info);
                }
            }
            return list;
        }

        /// <summary>获取公司对应具体供应商采购索票额度集合
        /// </summary>
        /// <param name="filialeId">公司ID</param>
        /// <param name="dateYear">年份</param>
        /// <param name="dateMonth">月份</param>
        /// <param name="pageIndex">当前页码 </param>
        /// <param name="pageSize"> 每页显示条数</param>
        /// <param name="totalCount">总记录数 </param>
        /// <returns>Return:供应商采购索票额度集合</returns>
        public IList<ProcurementTicketLimitInfo> GetProcurementTicketLimitDetailList(Guid filialeId, int dateYear, int dateMonth, int pageIndex, int pageSize, out int totalCount)
        {
            const string SQL = @"
SELECT 
CC.CompanyId,CC.CompanyName,
ISNULL(PTL.TakerTicketLimit,0) AS TakerTicketLimit,ISNULL(FilialeId,'00000000-0000-0000-0000-000000000000') AS FilialeId 
FROM lmShop_CompanyCussent AS CC  
LEFT JOIN  
  (  
    SELECT CompanyId,TakerTicketLimit,FilialeId FROM [ProcurementTicketLimit] 
    WHERE DateYear=@DateYear AND DateMonth=@DateMonth
    {0}{1}
  )
AS PTL ON CC.CompanyId=PTL.CompanyId 
WHERE CC.CompanyId 
    IN(SELECT CompanyId FROM [CompanyBindingFiliale] {2} {1} GROUP BY CompanyId)";

            var parms = new[] {
                new Parameter("@DateYear", dateYear),
                new Parameter("@DateMonth", dateMonth),
                new Parameter("@FilialeId",filialeId)
             };
            using (var db = DatabaseFactory.Create())
            {
                //var pageQuery = new Keede.DAL.Helper.Sql.PageQuery(pageIndex, pageSize, string.Format(SQL,filialeId!=Guid.Empty?" AND ":"",
                //    filialeId != Guid.Empty ? " FilialeId=@FilialeId " : "", filialeId != Guid.Empty ? " WHERE " : ""), " CompanyName ASC");
                var pageItem = db.Select<ProcurementTicketLimitInfo>(true, string.Format(SQL, filialeId != Guid.Empty ? " AND " : "",
                    filialeId != Guid.Empty ? " FilialeId=@FilialeId " : "", filialeId != Guid.Empty ? " WHERE " : ""), parms);
                var procurementTicketLimitInfos = pageItem as ProcurementTicketLimitInfo[] ?? pageItem.ToArray();
                totalCount = pageItem != null ? procurementTicketLimitInfos.Count() : 0;
                return procurementTicketLimitInfos.ToList();
            }
        }

        /// <summary>获取具体供应商对应采购索取额度
        /// </summary>
        /// <param name="filialeId">公司ID</param>
        /// <param name="companyIds">供应商Ids</param>
        /// <param name="dateYear">年份</param>
        /// <param name="dateMonth">月份</param>
        /// <returns></returns>
        public IList<ProcurementTicketLimitInfo> GetProcurementTicketLimitDetailByCompanyIds(Guid filialeId, string companyIds, int dateYear, int dateMonth)
        {
            const string SQL = @"
SELECT CompanyId,TakerTicketLimit AS ElseCompanyIdTakerTicketLimit,FilialeId AS ElseFilialeId FROM [ProcurementTicketLimit] 
	WHERE DateYear=@DateYear AND DateMonth=@DateMonth
    AND CompanyId IN ({0})
    AND FilialeId!=@FilialeId";
            var parms = new[] {
                new SqlParameter("@FilialeId",SqlDbType.UniqueIdentifier){Value = filialeId} ,
                new SqlParameter("@DateYear", SqlDbType.Int){Value = dateYear},
                new SqlParameter("@DateMonth", SqlDbType.Int){Value = dateMonth}
             };
            IList<ProcurementTicketLimitInfo> list = new List<ProcurementTicketLimitInfo>();
            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, string.Format(SQL, companyIds), parms))
            {
                while (rdr.Read())
                {
                    var info = new ProcurementTicketLimitInfo
                    {
                        CompanyId = new Guid(rdr["CompanyId"].ToString()),
                        ElseFilialeId = new Guid(rdr["ElseFilialeId"].ToString()),
                        ElseCompanyIdTakerTicketLimit = Convert.ToDecimal(rdr["ElseCompanyIdTakerTicketLimit"]),
                    };
                    list.Add(info);
                }
            }
            return list;
        }

        /// <summary>获取当前年月供应商设置的采购索取额度
        /// </summary>
        /// <param name="companyId">供应商ID</param>
        /// <param name="dateYear">年份</param>
        /// <param name="dateMonth">月份 </param>
        /// <returns></returns>
        public IList<ProcurementTicketLimitInfo> GetProcurementTicketLimitDetailByCompanyId(Guid companyId, int dateYear, int dateMonth)
        {
            const string SQL = @"
SELECT 
       [FilialeId]
      ,[CompanyId]
      ,[TakerTicketLimit]
      ,ISNULL([AlreadyCompleteLimit],0) AS AlreadyCompleteLimit
      ,[TakerTicketLimit]-ISNULL([AlreadyCompleteLimit],0) AS SurplusLimit
    FROM [ProcurementTicketLimit] 
	WHERE DateYear=@DateYear AND DateMonth=@DateMonth
    AND CompanyId =@CompanyId";
            var parms = new[] {
                new SqlParameter("@CompanyId",SqlDbType.UniqueIdentifier){Value = companyId} ,
                new SqlParameter("@DateYear", SqlDbType.Int){Value = dateYear},
                new SqlParameter("@DateMonth", SqlDbType.Int){Value = dateMonth}
             };
            IList<ProcurementTicketLimitInfo> list = new List<ProcurementTicketLimitInfo>();
            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL, parms))
            {
                while (rdr.Read())
                {
                    var info = new ProcurementTicketLimitInfo
                    {
                        FilialeId = new Guid(rdr["FilialeId"].ToString()),
                        CompanyId = new Guid(rdr["CompanyId"].ToString()),
                        TakerTicketLimit = Convert.ToDecimal(rdr["TakerTicketLimit"]),
                        AlreadyCompleteLimit = Convert.ToDecimal(rdr["AlreadyCompleteLimit"])
                    };
                    list.Add(info);
                }
            }
            return list;
        }

        /// <summary>更新已经采购额度
        /// </summary>
        /// <param name="filialeId">公司ID</param>
        /// <param name="companyId">供应商ID</param>
        /// <param name="alreadyCompleteLimit">额度</param>
        /// <param name="dateYear">年份</param>
        /// <param name="dateMonth">月份</param>
        /// <returns></returns>
        public bool RenewalProcurementTicketLimitAlreadyCompleteLimit(Guid filialeId, Guid companyId, decimal alreadyCompleteLimit, int dateYear, int dateMonth)
        {
            const string SQL = @"
UPDATE [ProcurementTicketLimit]
   SET [AlreadyCompleteLimit] = AlreadyCompleteLimit+@AlreadyCompleteLimit
 WHERE DateYear=@DateYear AND DateMonth=@DateMonth  AND CompanyId =@CompanyId AND FilialeId=@FilialeId";
            var parms = new[]
                           {
                               new SqlParameter("@FilialeId", SqlDbType.UniqueIdentifier){Value = filialeId},
                               new SqlParameter("@CompanyId", SqlDbType.UniqueIdentifier){Value = companyId},
                               new SqlParameter("@DateYear", SqlDbType.Int){Value = dateYear},
                               new SqlParameter("@DateMonth", SqlDbType.Int){Value = dateMonth},
                               new SqlParameter("@AlreadyCompleteLimit", SqlDbType.Decimal){Value =alreadyCompleteLimit }
                           };
            try
            {
                return SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL, parms) > 0;
            }
            catch (Exception exp)
            {
                SAL.LogCenter.LogService.LogError(string.Format("更新采购额度失败,filialeId={0}, companyId={1}, alreadyCompleteLimit={2}, dateYear={3}, dateMonth={4}", filialeId, companyId, alreadyCompleteLimit, dateYear, dateMonth), "仓库管理", exp);
                return false;
            }
        }
    }
}
