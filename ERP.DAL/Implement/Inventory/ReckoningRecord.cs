using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using ERP.Environment;
using ERP.Model.Report;
using Keede.DAL.Helper;
using System.Data;

namespace ERP.DAL.Implement.Inventory
{
    public partial class Reckoning 
    {
        #region  往来帐存档、应付款查询、入库金额统计  ADD BY LiangCanren AT 2015-08-17

        /// <summary>
        /// 查询当前月份入库往来帐及账期往来帐
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        public IList<RecordReckoningInfo> SelectRecordReckoningInfos(DateTime startTime, DateTime endTime)
        {
            const string SQL_SELECT = @"SELECT [ReckoningId],FilialeId,[ThirdCompanyID] AS CompanyId,R.[TradeCode],'{0}' AS DayTime,(CASE WHEN CC.PaymentDays>0 THEN DATEADD(MM,CC.PaymentDays,'{0}') ELSE '{0}' END) AS [PaymentDayTime],[AccountReceivable],TradeCode,R.[State],[IsChecked],[LinkTradeType],[LinkTradeCode] FROM lmShop_Reckoning AS R with(nolock) 
            INNER JOIN lmshop_CompanyCussent CC ON R.ThirdCompanyID=CC.CompanyID AND CC.CompanyType=1
	        where R.DateCreated between  '{0}' and  '{1}'
	        and AuditingState=1 AND LEFT(LinkTradeCode,2) NOT IN('LI','LO','BI','BO')  
	        and TradeCode not like 'GT%' 
 ";
            IList<RecordReckoningInfo> list = new List<RecordReckoningInfo>();
            IDataReader sdr = null;
            try
            {
                sdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true,
                    string.Format(string.Format(SQL_SELECT, startTime, endTime), new DateTime(startTime.Year, startTime.Month, 1)));

                while (sdr.Read())
                {
                    list.Add(new RecordReckoningInfo
                    {
                        ReckoningId = SqlRead.GetGuid(sdr, "ReckoningId"),
                        FilialeId = SqlRead.GetGuid(sdr, "FilialeId"),
                        CompanyId = SqlRead.GetGuid(sdr, "CompanyId"),
                        TradeCode = SqlRead.GetString(sdr, "TradeCode"),
                        DayTime = SqlRead.GetDateTime(sdr, "DayTime"),
                        PaymentDayTime = SqlRead.GetDateTime(sdr, "PaymentDayTime"),
                        AccountReceivable = SqlRead.GetDecimal(sdr, "AccountReceivable"),
                        LinkTradeType = SqlRead.GetInt(sdr, "LinkTradeType"),
                        State = SqlRead.GetInt(sdr, "State"),
                        LinkTradeCode = SqlRead.GetString(sdr, "LinkTradeCode"),
                        IsChecked = SqlRead.GetInt(sdr, "IsChecked")
                    });
                }
            }
            catch (Exception ex)
            {
                throw new Exception("当前往来帐明细存档异常", ex);
            }
            finally
            {
                sdr?.Close();
            }
            return list;
        }

        #endregion

        #region  从当前数据库中获取  实时获取
        /// <summary>
        /// 获取公司当前月份的应付款金额  
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public IList<SupplierPaymentsRecordInfo> SelectCurrentMonthPaymentsRecords(int year, int month)
        {
            const string SQL = @"DECLARE @CompanyID      UNIQUEIDENTIFIER            --往来单位ID 
		,@PaymentDays    INT                         --账期 
       ,@time1          date              --当前开始日期
       ,@time2          date               --账期日期(根据账期计算账期日期)(结束日期)
       ,@time3          date                --账期日期(根据账期计算账期日期)(开始日期)
		,@TheMonthNum      int                       --月份循环值
		
DECLARE  @TempFilialeRecord TABLE(
	FilialeId UNIQUEIDENTIFIER null,
	TotalAmount decimal(18,4) null,
	TotalNoPayed decimal(18,4) null
)

SET @time1= '{0}'--参数2 开始日期
SET @time2= dateadd(mm,0,DATEADD(MS,-3,DATEADD(MM, DATEDIFF(MM,0,@time1)+1, 0)))  
 
--------------------------------无账期应付款总额-------------------------------------------           		
 INSERT INTO @TempFilialeRecord([FilialeId],[TotalAmount],[TotalNoPayed]) 
 SELECT FilialeId,
 ISNULL(sum(case when LinkTradeType = 1 and AccountReceivable< 0 then AccountReceivable else 0 end),0) AS TotalAmount ,
 ISNULL(sum(case when IsChecked =0  then AccountReceivable else 0 end),0) AS TotalNoPayed
  FROM lmShop_Reckoning AS R with(nolock) 
    INNER JOIN lmshop_CompanyCussent CC ON R.ThirdCompanyID=CC.CompanyID AND CC.CompanyType=1 AND CC.PaymentDays=0
	where R.DateCreated between  @time1 and  @time2 
	and AuditingState=1 AND R.[State]<>2 and LinkTradeType in (1,2) 
	and (
        LinkTradeCode like 'RK%' or LinkTradeCode like 'SL%' or LinkTradeCode like 'SO%'
		or LinkTradeCode like 'SI%'   or LinkTradeCode like 'TSI%'  or LinkTradeCode like 'TSO%'
		or LinkTradeCode like 'TR%') and TradeCode not like 'GT%'
	GROUP BY FilialeId,IsOut
       
--------------------------------有账期应付款总额-------------------------------------------               	
--循环变量
DECLARE @W_Count INT, @W_Num INT 
----临时往来单位表
SELECT ROW_NUMBER() OVER(ORDER BY CompanyID) AS RowID,CompanyId,PaymentDays INTO #TempCompanyCussent 
FROM lmShop_CompanyCussent WHERE CompanyType=1 AND PaymentDays>0 
--循环变量赋值
SET @W_Num =1
SELECT @W_Count=COUNT(0) FROM #TempCompanyCussent
--循环往来单位
WHILE(@W_Num <= @W_Count)
BEGIN
        --获取往来单位
		SELECT @CompanyID = CompanyId,@PaymentDays=PaymentDays FROM #TempCompanyCussent  WHERE RowID = @w_num
		SET @time3= dateadd(mm,-@PaymentDays,'{0}')--参数2 开始日期
        SET @time2= dateadd(mm,-@PaymentDays,DATEADD(MS,-3,DATEADD(MM, DATEDIFF(MM,0,@time1)+1, 0)))  
            
        INSERT INTO @TempFilialeRecord([FilialeId],[TotalAmount],[TotalNoPayed])     		
     	SELECT FilialeId,
     	ISNULL(sum(case when LinkTradeType = 1 and AccountReceivable<0  then AccountReceivable else 0 end),0) AS TotalAmount ,
 ISNULL(sum(case when IsChecked =0  then AccountReceivable else 0 end),0) AS TotalNoPayed FROM lmshop_Reckoning as R with(nolock) 
	where R.[ThirdCompanyID]=@CompanyID AND R.DateCreated between  @time3 and  @time2 
	and AuditingState=1 AND R.[State]<>2 and LinkTradeType in (1,2) 
	and (
		LinkTradeCode like 'RK%' or LinkTradeCode like 'SL%' or LinkTradeCode like 'SO%'
		or LinkTradeCode like 'SI%'   or LinkTradeCode like 'TSI%'  or LinkTradeCode like 'TSO%'
		or LinkTradeCode like 'TR%') and TradeCode not like 'GT%'
	group by FilialeId,IsOut
		
   SET 	@W_Num +=1	 --循环累加
END
DROP TABLE #TempCompanyCussent

SELECT FilialeId,@time1 AS DayTime,SUM(TotalAmount) AS TotalAmount,SUM(TotalNoPayed) AS TotalNoPayed 
FROM @TempFilialeRecord GROUP BY FilialeId ORDER BY TotalAmount DESC ";
            var startTime = new DateTime(year, month, 1);
            var newSQL = string.Format(SQL, startTime);
            using (var db = DatabaseFactory.Create())
            {
                return db.Select<SupplierPaymentsRecordInfo>(true, newSQL).ToList();
            }
        }

        /// <summary>
        /// 公司对应往来单位当月应付款总额
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="filialeId"></param>
        /// <param name="companyName"></param>
        /// <returns></returns>
        public IList<SupplierPaymentsRecordInfo> SelectCurrentMonthPaymentsRecordsDetail(int year, int month, Guid filialeId, string companyName)
        {
            const string SQL = @"DECLARE @CompanyID      UNIQUEIDENTIFIER            --往来单位ID 
         ,@CompanyName varchar(128)                  -- 公司名称
		,@PaymentDays    INT                         --账期 
       ,@time1          date              --开始日期
       ,@time2          date               --账期日期(根据账期计算账期日期)(结束日期)
       ,@time3          date                --账期日期(根据账期计算账期日期)(开始日期)  
		,@TheMonthNum      int                       --月份循环值
		
DECLARE  @TempCompanyIdRecord TABLE(
	CompanyId UNIQUEIDENTIFIER null,
    CompanyName varchar(128) null,
    PaymentDays int null,
	TotalAmount decimal(18,4) null,
	TotalNoPayed decimal(18,4) null
)

SET @time1= '{0}'--参数2 开始日期
SET @time2= dateadd(mm,0,DATEADD(MS,-3,DATEADD(MM, DATEDIFF(MM,0,@time1)+1, 0)))  
            		
 INSERT INTO @TempCompanyIdRecord([CompanyId],[CompanyName],[PaymentDays],[TotalAmount],[TotalNoPayed]) 
 SELECT R.ThirdCompanyID AS CompanyId,CC.CompanyName,0 AS PaymentDays ,
 ISNULL(sum(case when LinkTradeType = 1 and AccountReceivable< 0 then AccountReceivable else 0 end),0) AS TotalAmount ,
 ISNULL(sum(case when IsChecked =0  then AccountReceivable else 0 end),0) AS TotalNoPayed
  FROM lmShop_Reckoning AS R with(nolock) 
    INNER JOIN lmshop_CompanyCussent CC ON R.ThirdCompanyID=CC.CompanyID AND CC.CompanyType=1 AND CC.PaymentDays=0 {2}
	where R.DateCreated between  @time1 and  @time2 {1}
	and AuditingState=1 AND R.[State]<>2 and LinkTradeType in (1,2) 
	and (
		LinkTradeCode like 'RK%' or LinkTradeCode like 'SL%'   or LinkTradeCode like 'SO%'
		or LinkTradeCode like 'SI%'   or LinkTradeCode like 'TSI%'  or LinkTradeCode like 'TSO%'
		or LinkTradeCode like 'TR%') and TradeCode not like 'GT%'
	GROUP BY R.ThirdCompanyID,CC.CompanyName

--循环变量
DECLARE @W_Count INT, @W_Num INT 
----临时往来单位表
SELECT ROW_NUMBER() OVER(ORDER BY CompanyID) AS RowID,CompanyId,CompanyName,PaymentDays INTO #TempCompanyCussent 
FROM lmShop_CompanyCussent AS CC WHERE CC.CompanyType=1 AND CC.PaymentDays>0 {2}
--循环变量赋值
SET @W_Num =1
SELECT @W_Count=COUNT(0) FROM #TempCompanyCussent
--循环往来单位
WHILE(@W_Num <= @W_Count)
BEGIN
        --获取往来单位
		SELECT @CompanyID = CompanyId,@CompanyName=CompanyName,@PaymentDays=PaymentDays FROM #TempCompanyCussent  WHERE RowID = @w_num
		SET @time3= dateadd(mm,-@PaymentDays,'{0}')--参数2 开始日期
        SET @time2= dateadd(mm,-@PaymentDays,DATEADD(MS,-3,DATEADD(MM, DATEDIFF(MM,0,@time1)+1, 0)))  
            
        INSERT INTO @TempCompanyIdRecord([CompanyId],[CompanyName],[PaymentDays],[TotalAmount],[TotalNoPayed])     		
     	SELECT @CompanyID AS CompanyId,@CompanyName AS CompanyName,@PaymentDays AS PaymentDays,
     	ISNULL(sum(case when LinkTradeType = 1 and AccountReceivable< 0 then AccountReceivable else 0 end),0) AS TotalAmount ,
 ISNULL(sum(case when IsChecked =0  then AccountReceivable else 0 end),0) AS TotalNoPayed FROM lmshop_Reckoning as R with(nolock) 
	where R.[ThirdCompanyID]=@CompanyID AND R.DateCreated between  @time3 and  @time2 {1}
	and AuditingState=1 AND R.[State]<>2 and LinkTradeType in (1,2) 
	and (
		LinkTradeCode like 'RK%' or LinkTradeCode like 'SL%'   or LinkTradeCode like 'SO%'
		or LinkTradeCode like 'SI%'   or LinkTradeCode like 'TSI%'  or LinkTradeCode like 'TSO%'
		or LinkTradeCode like 'TR%') and TradeCode not like 'GT%'
		
   SET 	@W_Num +=1	 --循环累加
END
DROP TABLE #TempCompanyCussent

SELECT CompanyId,CompanyName,PaymentDays,@time1 AS DayTime,TotalAmount,TotalNoPayed FROM @TempCompanyIdRecord WHERE TotalAmount!=0 ORDER BY TotalAmount DESC ";
            string filieleStr = string.Empty;
            if (filialeId != Guid.Empty)
            {
                filieleStr = string.Format(" AND R.FilialeId='{0}' ", filialeId);
            }
            var startTime = new DateTime(year, month, 1);
            var newSQL = string.Format(SQL, startTime, filieleStr, string.IsNullOrEmpty(companyName) ? ""
                : string.Format(" AND CC.CompanyName LIKE '%{0}%'", companyName));
            using (var db = DatabaseFactory.Create())
            {
                return db.Select<SupplierPaymentsRecordInfo>(true, newSQL).ToList();
            }
        }

        /// <summary>
        /// 当前月份公司的采购入库总金额
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public IList<SupplierPaymentsRecordInfo> SelectCurrentMonthStockRecords(int year, int month)
        {
            const string SQL = @"DECLARE @time1 date  --账期日期(根据账期计算账期日期)(开始日期)
       ,@time2  date        --账期日期(根据账期计算账期日期)(结束日期)

SET @time1= '{0}'--参数2 开始日期
SET @time2= dateadd(mm,0,DATEADD(MS,-3,DATEADD(MM, DATEDIFF(MM,0,@time1)+1, 0)))  
            		
 SELECT FilialeId,@time1 AS DayTime,SUM(TotalAmount) AS TotalAmount FROM 
 (
 SELECT  FilialeId,
 ISNULL(sum(case when LinkTradeType = 1 and AccountReceivable<0 then abs(AccountReceivable) else 0 end),0) AS TotalAmount 
  FROM lmShop_Reckoning AS R with(nolock) 
    INNER JOIN lmshop_CompanyCussent CC ON R.ThirdCompanyID=CC.CompanyID AND CC.CompanyType=1
	where R.DateCreated between  @time1 and  @time2 
	and AuditingState=1 AND R.[State]<>2 and LinkTradeType in (1,2) 
	and (
		LinkTradeCode like 'RK%' or LinkTradeCode like 'SL%'   or LinkTradeCode like 'SO%'
		or LinkTradeCode like 'SI%'   or LinkTradeCode like 'TSI%'  or LinkTradeCode like 'TSO%'
		or LinkTradeCode like 'TR%') and TradeCode not like 'GT%'
	GROUP BY FilialeId) AS I
	GROUP BY I.FilialeId ORDER BY TotalAmount DESC ";
            var startTime = new DateTime(year, month, 1);
            var newSQL = string.Format(SQL, startTime);
            using (var db = DatabaseFactory.Create())
            {
                return db.Select<SupplierPaymentsRecordInfo>(true, newSQL).ToList();
            }
        }

        /// <summary>
        /// 当前月份供应商的采购入库总金额
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="filialeId"></param>
        /// <param name="companyName"></param>
        /// <returns></returns>
        public IList<SupplierPaymentsRecordInfo> SelectCurrentMonthStockRecordsDetail(int year, int month, Guid filialeId, string companyName)
        {
            const string SQL = @"DECLARE @time1 date  --账期日期(根据账期计算账期日期)(开始日期)
       ,@time2  date        --账期日期(根据账期计算账期日期)(结束日期)

SET @time1= '{0}'--参数2 开始日期
SET @time2= dateadd(mm,0,DATEADD(MS,-3,DATEADD(MM, DATEDIFF(MM,0,@time1)+1, 0)))  
            		
 SELECT R.ThirdCompanyID AS CompanyId,CC.CompanyName,@time1 AS DayTime,
 ISNULL(sum(case when LinkTradeType = 1 and AccountReceivable<0  then abs(AccountReceivable) else 0 end),0) AS TotalAmount 
  FROM lmShop_Reckoning AS R with(nolock) 
    INNER JOIN lmshop_CompanyCussent CC ON R.ThirdCompanyID=CC.CompanyID AND CC.CompanyType=1 {2}
	where R.DateCreated between  @time1 and  @time2 {1}
	and AuditingState=1 AND R.[State]<>2 and LinkTradeType in (1,2) 
	and (
		LinkTradeCode like 'RK%' or LinkTradeCode like 'SL%'   or LinkTradeCode like 'SO%'
		or LinkTradeCode like 'SI%'   or LinkTradeCode like 'TSI%'  or LinkTradeCode like 'TSO%'
		or LinkTradeCode like 'TR%') and TradeCode not like 'GT%'
	GROUP BY R.ThirdCompanyID,CC.CompanyName ORDER BY TotalAmount DESC ";
            string filieleStr = string.Empty;
            if (filialeId != Guid.Empty)
            {
                filieleStr =string.Format(" AND R.FilialeId='{0}'", filialeId);
            }
            var startTime = new DateTime(year, month, 1);
            var newSQL = string.Format(SQL, startTime, filieleStr, string.IsNullOrEmpty(companyName) ? ""
                : string.Format(" AND CC.CompanyName LIKE '%{0}%'", companyName));
            using (var db = DatabaseFactory.Create())
            {
                return db.Select<SupplierPaymentsRecordInfo>(true, newSQL).ToList();
            }
        }
        #endregion
    }
}
