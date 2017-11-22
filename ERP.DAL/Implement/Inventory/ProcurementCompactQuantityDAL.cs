using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using ERP.DAL.Interface.IInventory;
using ERP.Model;
using Keede.DAL.Helper;
using ERP.Enum;
using ERP.Environment;

namespace ERP.DAL.Implement.Inventory
{
    /// <summary>▄︻┻┳═一 采购合同量数据层   ADD 2014-12-18  陈重文
    /// </summary>
    public class ProcurementCompactQuantityDAL : IProcurementCompactQuantity
    {

        public ProcurementCompactQuantityDAL(Environment.GlobalConfig.DB.FromType fromType)
        {

        }

        /// <summary>保存公司具体年份对应供应商采购合同量
        /// </summary>
        /// <param name="procurementCompactQuantityInfo">供应商采购合同量</param>
        /// <returns>Return：true/false</returns>
        public bool SaveProcurementCompactQuantity(ProcurementCompactQuantityInfo procurementCompactQuantityInfo)
        {
            const string SQL = @"
IF NOT EXISTS(SELECT CompactMoney FROM [ProcurementCompactQuantity] WHERE  CompanyId=@CompanyId AND DateYear=@DateYear)
    INSERT INTO [ProcurementCompactQuantity](CompanyId,DateYear,CompactMoney) VALUES(@CompanyId,@DateYear,@CompactMoney);
ELSE
    UPDATE [ProcurementCompactQuantity] SET CompactMoney=@CompactMoney WHERE  CompanyId=@CompanyId AND DateYear=@DateYear;";
            var parms = new[]
                           {
                               new SqlParameter("@CompanyId", SqlDbType.UniqueIdentifier),
                               new SqlParameter("@DateYear", SqlDbType.Int),
                               new SqlParameter("@CompactMoney", SqlDbType.Decimal)
                           };
            try
            {
                parms[0].Value = procurementCompactQuantityInfo.CompanyId;
                parms[1].Value = procurementCompactQuantityInfo.DateYear;
                parms[2].Value = procurementCompactQuantityInfo.CompactMoney;
                return SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL, parms) > 0;
            }
            catch (Exception exp)
            {
                SAL.LogCenter.LogService.LogError(string.Format("采购合同量保存失败,CompanyId={0},DateYear={1},CompactMoney={2}", procurementCompactQuantityInfo.CompanyId, procurementCompactQuantityInfo.DateYear, procurementCompactQuantityInfo.CompactMoney), "仓库管理", exp);
                return false;
            }
        }

        /// <summary>根据年份获取供应商采购合同量
        /// </summary>
        /// <param name="year">年份</param>
        /// <returns>Return：供应商采购合同量集合</returns>
        public IList<ProcurementCompactQuantityInfo> GetProcurementCompactQuantityList(int year)
        {
            string sql = @"
DECLARE @time1  SMALLDATETIME ,@time2 SMALLDATETIME
SET @time1= DATEADD(MM,DATEDIFF(MM,0,GETDATE()),0)
SET @time2= DATEADD(MS,-3,DATEADD(MM, DATEDIFF(MM,0,GETDATE())+1, 0))

SELECT 
t1.CompanyId,t1.CompanyName,
ISNULL(t2.CompactMoney,0) AS CompactMoney,
(ISNULL(t3.TheMonthProcurementMoney,0)-ISNULL(SR1.TheMonthRetStockOutMoney,0)) AS TheMonthProcurementMoney,
(ISNULL(t4.FinishedCompactMoney,0)-ISNULL(SR2.TheYearRetStockOutMoney,0)) AS FinishedCompactMoney,
(ISNULL(CompactMoney,0)-ISNULL(FinishedCompactMoney,0)) AS SurplusCompactMoney
FROM  lmShop_CompanyCussent t1  
LEFT JOIN   
(
	SELECT CompanyId,CompactMoney FROM ProcurementCompactQuantity
    WHERE DateYear =@DateYear
) t2
ON t1.CompanyId = t2.CompanyId 
-----------获取公司供应商当月采购金额-----------------------------------
LEFT JOIN (
	SELECT CompanyID,SUM(RealityQuantity*Price) AS TheMonthProcurementMoney
		FROM lmShop_PurchasingDetail  pd
		INNER JOIN 
		( 
			SELECT PurchasingID FROM [lmShop_Purchasing] WHERE PurchasingState BETWEEN 1 AND 4
			AND StartTime >= @time1
			AND StartTime < @time2
		)
		AS PS ON pd.PurchasingID=ps.PurchasingID
	GROUP BY CompanyID 	
) t3 on t1.CompanyId = t3.CompanyID

LEFT JOIN (
   SELECT ThirdCompanyID,SUM(ABS(AccountReceivable)) AS TheMonthRetStockOutMoney  FROM StorageRecord WHERE StockType=" + (int)StorageRecordType.BuyStockOut + @"
   AND StockState=" + (int)StorageRecordState.Finished + @"
   AND DateCreated >= @time1
   AND DateCreated <@time2
   GROUP BY ThirdCompanyID
) 
SR1 ON SR1.ThirdCompanyID=t1.CompanyId


-------------END----------------------------------------------------------
-----------获取公司供应商当年已完成签约金额-------------------------------
LEFT JOIN (
	SELECT CompanyID,SUM(RealityQuantity*Price) AS FinishedCompactMoney
		FROM lmShop_PurchasingDetail  pd
		INNER JOIN 
		( 
			SELECT PurchasingID FROM [lmShop_Purchasing] WHERE PurchasingState BETWEEN 1 AND 4
			AND StartTime >=@StartTime
			AND StartTime < @EndTime
		)
			AS PS ON pd.PurchasingID=ps.PurchasingID
	GROUP BY CompanyID
)t4 on t1.CompanyId = t4.CompanyID

LEFT JOIN (
   SELECT ThirdCompanyID,SUM(ABS(AccountReceivable)) AS TheYearRetStockOutMoney  FROM StorageRecord WHERE StockType=" + (int)StorageRecordType.BuyStockOut + @"
   AND StockState=" + (int)StorageRecordState.Finished + @"
   AND DateCreated >= @StartTime
   AND DateCreated <@EndTime
   GROUP BY ThirdCompanyID
) 
SR2 ON SR2.ThirdCompanyID=t1.CompanyId


WHERE  T1.CompanyType=1--供应商 
 AND T1.[State]=1--启用
-------------END----------------------------------------------------------";

            var startTime = Convert.ToDateTime(string.Format("{0}-01-01 00:00:00", year));
            var endTime = Convert.ToDateTime(string.Format("{0}-01-01 00:00:00", year + 1));
            var parms = new[] {
                new SqlParameter("@DateYear", SqlDbType.Int){Value = year},
                new SqlParameter("@StartTime", SqlDbType.DateTime){Value = startTime},
                new SqlParameter("@EndTime", SqlDbType.DateTime){Value = endTime}
             };
            IList<ProcurementCompactQuantityInfo> list = new List<ProcurementCompactQuantityInfo>();
            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, sql, parms))
            {
                while (rdr.Read())
                {
                    var info = new ProcurementCompactQuantityInfo
                    {
                        CompanyId = new Guid(rdr["CompanyID"].ToString()),
                        CompanyName = rdr["CompanyName"].ToString(),
                        CompactMoney = Convert.ToDecimal(rdr["CompactMoney"]),
                        TheMonthProcurementMoney = Convert.ToDecimal(Convert.ToDecimal(rdr["TheMonthProcurementMoney"]).ToString("#0.00")),
                        FinishedCompactMoney = Convert.ToDecimal(Convert.ToDecimal(rdr["FinishedCompactMoney"]).ToString("#0.00")),
                        SurplusCompactMoney = Convert.ToDecimal(Convert.ToDecimal(rdr["SurplusCompactMoney"]).ToString("#0.00")),
                    };
                    list.Add(info);
                }
            }
            return list;
        }

        /// <summary>根据年份，供应商ID获取供应商采购合同量
        /// </summary>
        /// <param name="year">年份</param>
        /// <param name="companyId"></param>
        /// <returns>Return：供应商采购合同量集合</returns>
        public IList<ProcurementCompactQuantityInfo> GetProcurementCompactQuantityList(int year, Guid companyId)
        {
            string sql = @"
DECLARE @time1  SMALLDATETIME ,@time2 SMALLDATETIME
SET @time1= DATEADD(MM,DATEDIFF(MM,0,GETDATE()),0)
SET @time2= DATEADD(MS,-3,DATEADD(MM, DATEDIFF(MM,0,GETDATE())+1, 0))

SELECT 
t1.CompanyId,t1.CompanyName,
ISNULL(t2.CompactMoney,0) AS CompactMoney,
(ISNULL(t3.TheMonthProcurementMoney,0)-ISNULL(SR1.TheMonthRetStockOutMoney,0)) AS TheMonthProcurementMoney,
(ISNULL(t4.FinishedCompactMoney,0)-ISNULL(SR2.TheYearRetStockOutMoney,0)) AS FinishedCompactMoney,
(ISNULL(CompactMoney,0)-ISNULL(FinishedCompactMoney,0)) AS SurplusCompactMoney
FROM  lmShop_CompanyCussent t1  
LEFT JOIN   
(
	SELECT CompanyId,CompactMoney FROM ProcurementCompactQuantity
    WHERE DateYear =@DateYear
) t2
ON t1.CompanyId = t2.CompanyId 
-----------获取公司供应商当月采购金额-----------------------------------
LEFT JOIN (
	SELECT CompanyID,SUM(RealityQuantity*Price) AS TheMonthProcurementMoney
		FROM lmShop_PurchasingDetail  pd
		INNER JOIN 
		( 
			SELECT PurchasingID FROM [lmShop_Purchasing] WHERE PurchasingState BETWEEN 1 AND 4
			AND StartTime >= @time1
			AND StartTime < @time2
            AND CompanyId=@CompanyId
		)
		AS PS ON pd.PurchasingID=ps.PurchasingID
	    GROUP BY CompanyID 	
) t3 on t1.CompanyId = t3.CompanyID

LEFT JOIN (
   SELECT ThirdCompanyID,SUM(ABS(AccountReceivable)) AS TheMonthRetStockOutMoney  FROM StorageRecord WHERE StockType=" + (int)StorageRecordType.BuyStockOut + @"
   AND StockState=" + (int)StorageRecordState.Finished + @"
   AND DateCreated >= @time1
   AND DateCreated <@time2
   AND ThirdCompanyID=@CompanyId
   GROUP BY ThirdCompanyID
) 
SR1 ON SR1.ThirdCompanyID=t1.CompanyId
-------------END----------------------------------------------------------
-----------获取公司供应商当年已完成签约金额-------------------------------
LEFT JOIN (
	SELECT CompanyID,SUM(RealityQuantity*Price) AS FinishedCompactMoney
		FROM lmShop_PurchasingDetail  pd
		INNER JOIN 
		( 
			SELECT PurchasingID FROM [lmShop_Purchasing] WHERE PurchasingState BETWEEN 1 AND 4
			AND StartTime >=@StartTime
			AND StartTime < @EndTime
            AND CompanyId=@CompanyId
		)
		AS PS ON pd.PurchasingID=ps.PurchasingID
	    GROUP BY CompanyID
)t4 on t1.CompanyId = t4.CompanyID

LEFT JOIN (
   SELECT ThirdCompanyID,SUM(ABS(AccountReceivable)) AS TheYearRetStockOutMoney  FROM StorageRecord WHERE StockType=" + (int)StorageRecordType.BuyStockOut + @"
   AND StockState=" + (int)StorageRecordState.Finished + @"
   AND DateCreated >= @StartTime
   AND DateCreated <@EndTime
   AND ThirdCompanyID=@CompanyId
   GROUP BY ThirdCompanyID
) 
SR2 ON SR2.ThirdCompanyID=t1.CompanyId


WHERE  T1.CompanyId=@CompanyId
 AND T1.CompanyType=1--供应商 
 AND T1.[State]=1--启用
-------------END----------------------------------------------------------";

            var startTime = Convert.ToDateTime(string.Format("{0}-01-01 00:00:00", year));
            var endTime = Convert.ToDateTime(string.Format("{0}-01-01 00:00:00", year + 1));
            var parms = new[] {
                new SqlParameter("@DateYear", SqlDbType.Int){Value = year},
                new SqlParameter("@StartTime", SqlDbType.DateTime){Value = startTime},
                new SqlParameter("@EndTime", SqlDbType.DateTime){Value = endTime},
                new SqlParameter("@CompanyId",SqlDbType.UniqueIdentifier){Value = companyId}
             };
            return GetProcurementCompactQuantityList(sql, parms);
        }

        /// <summary>根据年份，采购人责任人ID获取供应商采购合同量
        /// </summary>
        /// <param name="year">年份</param>
        /// <param name="personnelId">采购人责任人ID</param>
        /// <returns>Return：供应商采购合同量集合</returns>
        public IList<ProcurementCompactQuantityInfo> GetProcurementCompactQuantityList(Guid personnelId, int year)
        {
            string sql = @"

DECLARE @time1  SMALLDATETIME ,@time2 SMALLDATETIME
SET @time1= DATEADD(MM,DATEDIFF(MM,0,GETDATE()),0)
SET @time2= DATEADD(MS,-3,DATEADD(MM, DATEDIFF(MM,0,GETDATE())+1, 0))

----临时往来单位Id
SELECT 
	DISTINCT CompanyId
	INTO #TempCompanyId
FROM [dbo].[lmshop_PurchaseSet]
WHERE PersonResponsible=@PersonnelId

SELECT 
t1.CompanyId,t1.CompanyName,
ISNULL(t2.CompactMoney,0) AS CompactMoney,
(ISNULL(t3.TheMonthProcurementMoney,0)-ISNULL(SR1.TheMonthRetStockOutMoney,0)) AS TheMonthProcurementMoney,
(ISNULL(t4.FinishedCompactMoney,0)-ISNULL(SR2.TheYearRetStockOutMoney,0)) AS FinishedCompactMoney,
(ISNULL(CompactMoney,0)-ISNULL(FinishedCompactMoney,0)) AS SurplusCompactMoney
FROM  lmShop_CompanyCussent t1  
INNER JOIN #TempCompanyId  AS Temp on t1.CompanyId=Temp.CompanyId  
LEFT JOIN   
(
	SELECT CompanyId,CompactMoney FROM ProcurementCompactQuantity
    WHERE DateYear =@DateYear
) t2
ON t1.CompanyId = t2.CompanyId 
-----------获取公司供应商当月采购金额-----------------------------------
LEFT JOIN (
	SELECT CompanyID,SUM(RealityQuantity*Price) AS TheMonthProcurementMoney
		FROM lmShop_PurchasingDetail  pd
		INNER JOIN 
		( 
			SELECT PurchasingID FROM [lmShop_Purchasing] AS TP1
			INNER JOIN #TempCompanyId  AS Temp1 on TP1.CompanyId=Temp1.CompanyId  
			WHERE PurchasingState BETWEEN 1 AND 4
			AND StartTime >= @time1
			AND StartTime < @time2
		)
		AS PS ON pd.PurchasingID=ps.PurchasingID
	    GROUP BY CompanyID
) t3 on t1.CompanyId = t3.CompanyID

LEFT JOIN (
   SELECT ThirdCompanyID,SUM(ABS(AccountReceivable)) AS TheMonthRetStockOutMoney  FROM StorageRecord AS TSR1
   INNER JOIN #TempCompanyId  AS Temp2 on TSR1.ThirdCompanyID=Temp2.CompanyId
   WHERE StockType=" + (int)StorageRecordType.BuyStockOut + @"
   AND StockState=" + (int)StorageRecordState.Finished + @"
   AND DateCreated >= @time1
   AND DateCreated <@time2
   AND ThirdCompanyID IN ( SELECT CompanyId FROM #TempCompanyId)
   GROUP BY ThirdCompanyID
) 
SR1 ON SR1.ThirdCompanyID=t1.CompanyId


-------------END----------------------------------------------------------
-----------获取公司供应商当年已完成签约金额-------------------------------
LEFT JOIN (
	SELECT CompanyID,SUM(RealityQuantity*Price) AS FinishedCompactMoney
		FROM lmShop_PurchasingDetail  pd
		INNER JOIN 
		( 
			SELECT PurchasingID FROM [lmShop_Purchasing] AS TP2
			INNER JOIN #TempCompanyId  AS Temp3 on TP2.CompanyId=Temp3.CompanyId   
			WHERE PurchasingState BETWEEN 1 AND 4
			AND StartTime >=@StartTime
			AND StartTime < @EndTime
		)
		AS PS ON pd.PurchasingID=ps.PurchasingID
	    GROUP BY CompanyID
)t4 on t1.CompanyId = t4.CompanyID

LEFT JOIN (
   SELECT ThirdCompanyID,SUM(ABS(AccountReceivable)) AS TheYearRetStockOutMoney  FROM StorageRecord  AS TSR2
   INNER JOIN #TempCompanyId  AS Temp4 on TSR2.ThirdCompanyID=Temp4.CompanyId    
   WHERE StockType=" + (int)StorageRecordType.BuyStockOut + @"
   AND StockState=" + (int)StorageRecordState.Finished + @"
   AND DateCreated >= @StartTime
   AND DateCreated < @EndTime
   GROUP BY ThirdCompanyID
) 
SR2 ON SR2.ThirdCompanyID=t1.CompanyId

WHERE  T1.CompanyType=1--供应商 
 AND T1.[State]=1--启用
 
DROP TABLE #TempCompanyId
-------------END----------------------------------------------------------";
            var startTime = Convert.ToDateTime(string.Format("{0}-01-01 00:00:00", year));
            var endTime = Convert.ToDateTime(string.Format("{0}-01-01 00:00:00", year + 1));
            var parms = new[] {
                new SqlParameter("@DateYear", SqlDbType.Int){Value = year},
                new SqlParameter("@StartTime", SqlDbType.DateTime){Value = startTime},
                new SqlParameter("@EndTime", SqlDbType.DateTime){Value = endTime},
                new SqlParameter("@PersonnelId",SqlDbType.UniqueIdentifier){Value = personnelId}
             };
            return GetProcurementCompactQuantityList(sql, parms);
        }

        /// <summary>根据年份，供应商ID，采购人责任人ID获取供应商采购合同量
        /// </summary>
        /// <param name="year">年份</param>
        /// <param name="companyId">供应商Id</param>
        /// <param name="personnelId">采购人责任人ID</param>
        /// <returns>Return：供应商采购合同量集合</returns>
        public IList<ProcurementCompactQuantityInfo> GetProcurementCompactQuantityList(int year, Guid companyId, Guid personnelId)
        {
            string sql = @"
DECLARE @time1  SMALLDATETIME ,@time2 SMALLDATETIME
SET @time1= DATEADD(MM,DATEDIFF(MM,0,GETDATE()),0)
SET @time2= DATEADD(MS,-3,DATEADD(MM, DATEDIFF(MM,0,GETDATE())+1, 0))

----临时往来单位Id
SELECT 
	DISTINCT CompanyId
	INTO #TempCompanyId
FROM [dbo].[lmshop_PurchaseSet]
WHERE PersonResponsible=@PersonnelId AND CompanyId=@CompanyId

SELECT 
t1.CompanyId,t1.CompanyName,
ISNULL(t2.CompactMoney,0) AS CompactMoney,
(ISNULL(t3.TheMonthProcurementMoney,0)-ISNULL(SR1.TheMonthRetStockOutMoney,0)) AS TheMonthProcurementMoney,
(ISNULL(t4.FinishedCompactMoney,0)-ISNULL(SR2.TheYearRetStockOutMoney,0)) AS FinishedCompactMoney,
(ISNULL(CompactMoney,0)-ISNULL(FinishedCompactMoney,0)) AS SurplusCompactMoney
FROM  lmShop_CompanyCussent t1  
INNER JOIN #TempCompanyId  AS Temp on t1.CompanyId=Temp.CompanyId  
LEFT JOIN   
(
	SELECT CompanyId,CompactMoney FROM ProcurementCompactQuantity
    WHERE DateYear =@DateYear
) t2
ON t1.CompanyId = t2.CompanyId 
-----------获取公司供应商当月采购金额-----------------------------------
LEFT JOIN (
	SELECT CompanyID,SUM(RealityQuantity*Price) AS TheMonthProcurementMoney
		FROM lmShop_PurchasingDetail  pd
		INNER JOIN 
		( 
			SELECT PurchasingID FROM [lmShop_Purchasing] AS TP1
			INNER JOIN #TempCompanyId  AS Temp1 on TP1.CompanyId=Temp1.CompanyId  
			WHERE PurchasingState BETWEEN 1 AND 4
			AND StartTime >= @time1
			AND StartTime < @time2
		)
		AS PS ON pd.PurchasingID=ps.PurchasingID
	    GROUP BY CompanyID
) t3 on t1.CompanyId = t3.CompanyID

LEFT JOIN (
   SELECT ThirdCompanyID,SUM(ABS(AccountReceivable)) AS TheMonthRetStockOutMoney  FROM StorageRecord AS TSR1
   INNER JOIN #TempCompanyId  AS Temp2 on TSR1.ThirdCompanyID=Temp2.CompanyId
   WHERE StockType=" + (int)StorageRecordType.BuyStockOut + @"
   AND StockState=" + (int)StorageRecordState.Finished + @"
   AND DateCreated >= @time1
   AND DateCreated <@time2
   AND ThirdCompanyID IN ( SELECT CompanyId FROM #TempCompanyId)
   GROUP BY ThirdCompanyID
) 
SR1 ON SR1.ThirdCompanyID=t1.CompanyId


-------------END----------------------------------------------------------
-----------获取公司供应商当年已完成签约金额-------------------------------
LEFT JOIN (
	SELECT CompanyID,SUM(RealityQuantity*Price) AS FinishedCompactMoney
		FROM lmShop_PurchasingDetail  pd
		INNER JOIN 
		( 
			SELECT PurchasingID FROM [lmShop_Purchasing] AS TP2
			INNER JOIN #TempCompanyId  AS Temp3 on TP2.CompanyId=Temp3.CompanyId   
			WHERE PurchasingState BETWEEN 1 AND 4
			AND StartTime >=@StartTime
			AND StartTime < @EndTime
		)
		AS PS ON pd.PurchasingID=ps.PurchasingID
	    GROUP BY CompanyID
)t4 on t1.CompanyId = t4.CompanyID

LEFT JOIN (
   SELECT ThirdCompanyID,SUM(ABS(AccountReceivable)) AS TheYearRetStockOutMoney  FROM StorageRecord  AS TSR2
   INNER JOIN #TempCompanyId  AS Temp4 on TSR2.ThirdCompanyID=Temp4.CompanyId    
   WHERE StockType=" + (int)StorageRecordType.BuyStockOut + @"
   AND StockState=" + (int)StorageRecordState.Finished + @"
   AND DateCreated >= @StartTime
   AND DateCreated < @EndTime
   GROUP BY ThirdCompanyID
) 
SR2 ON SR2.ThirdCompanyID=t1.CompanyId

WHERE  T1.CompanyType=1--供应商 
 AND T1.[State]=1--启用
 
DROP TABLE #TempCompanyId
-------------END----------------------------------------------------------";
            var startTime = Convert.ToDateTime(string.Format("{0}-01-01 00:00:00", year));
            var endTime = Convert.ToDateTime(string.Format("{0}-01-01 00:00:00", year + 1));
            var parms = new[] {
                new SqlParameter("@DateYear", SqlDbType.Int){Value = year},
                new SqlParameter("@StartTime", SqlDbType.DateTime){Value = startTime},
                new SqlParameter("@EndTime", SqlDbType.DateTime){Value = endTime},
                new SqlParameter("@CompanyId",SqlDbType.UniqueIdentifier){Value = companyId} ,
                new SqlParameter("@PersonnelId",SqlDbType.UniqueIdentifier){Value = personnelId}
             };
            return GetProcurementCompactQuantityList(sql, parms);
        }

        /// <summary>获取采购合同量集合
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="commandParameters">参数</param>
        /// <returns></returns>
        private IList<ProcurementCompactQuantityInfo> GetProcurementCompactQuantityList(string sql, SqlParameter[] commandParameters)
        {
            IList<ProcurementCompactQuantityInfo> list = new List<ProcurementCompactQuantityInfo>();
            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, sql, commandParameters))
            {
                while (rdr.Read())
                {
                    var info = new ProcurementCompactQuantityInfo
                    {
                        CompanyId = new Guid(rdr["CompanyID"].ToString()),
                        CompanyName = rdr["CompanyName"].ToString(),
                        CompactMoney = Convert.ToDecimal(rdr["CompactMoney"]),
                        TheMonthProcurementMoney = Convert.ToDecimal(Convert.ToDecimal(rdr["TheMonthProcurementMoney"]).ToString("#0.00")),
                        FinishedCompactMoney = Convert.ToDecimal(Convert.ToDecimal(rdr["FinishedCompactMoney"]).ToString("#0.00")),
                        SurplusCompactMoney = Convert.ToDecimal(Convert.ToDecimal(rdr["SurplusCompactMoney"]).ToString("#0.00")),
                    };
                    list.Add(info);
                }
            }
            return list;
        }
    }
}
