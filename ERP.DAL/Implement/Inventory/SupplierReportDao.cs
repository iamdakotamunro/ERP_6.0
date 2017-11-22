using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using ERP.DAL.Interface.IInventory;
using ERP.Environment;
using ERP.Model.Report;
using Keede.DAL.Helper;
using Keede.DAL.RWSplitting;

namespace ERP.DAL.Implement.Inventory
{
    public class SupplierReportDao : ISupplierReport
    {
        #region  从存档的报表数据查询
        /// <summary>
        /// 应付款查询、公司对应每月的应付款金额报表数据  
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        public IList<SupplierPaymentsReportInfo> SelectPaymentsReprotsGroupByFilialeId(int year)
        {
            const string SQL = @"SELECT FilialeId,
ISNULL(SUM(CASE WHEN MONTH(DayTime)=1 THEN TotalAmount ELSE 0 END),0) AS January,
ISNULL(SUM(CASE WHEN MONTH(DayTime)=2 THEN TotalAmount ELSE 0 END),0) AS February,
ISNULL(SUM(CASE WHEN MONTH(DayTime)=3 THEN TotalAmount ELSE 0 END),0) AS March,
ISNULL(SUM(CASE WHEN MONTH(DayTime)=4 THEN TotalAmount ELSE 0 END),0) AS April, 
ISNULL(SUM(CASE WHEN MONTH(DayTime)=5 THEN TotalAmount ELSE 0 END),0) AS May, 
ISNULL(SUM(CASE WHEN MONTH(DayTime)=6 THEN TotalAmount ELSE 0 END),0) AS June, 
ISNULL(SUM(CASE WHEN MONTH(DayTime)=7 THEN TotalAmount ELSE 0 END),0) AS July, 
ISNULL(SUM(CASE WHEN MONTH(DayTime)=8 THEN TotalAmount ELSE 0 END),0) AS August, 
ISNULL(SUM(CASE WHEN MONTH(DayTime)=9 THEN TotalAmount ELSE 0 END),0) AS September, 
ISNULL(SUM(CASE WHEN MONTH(DayTime)=10 THEN TotalAmount ELSE 0 END),0) AS October, 
ISNULL(SUM(CASE WHEN MONTH(DayTime)=11 THEN TotalAmount ELSE 0 END),0) AS November, 
ISNULL(SUM(CASE WHEN MONTH(DayTime)=12 THEN TotalAmount ELSE 0 END),0) AS December,
ISNULL(SUM(CASE WHEN MONTH(DayTime)=1 THEN TotalNoPayed ELSE 0 END),0) AS January1,
ISNULL(SUM(CASE WHEN MONTH(DayTime)=2 THEN TotalNoPayed ELSE 0 END),0) AS February2,
ISNULL(SUM(CASE WHEN MONTH(DayTime)=3 THEN TotalNoPayed ELSE 0 END),0) AS March3,
ISNULL(SUM(CASE WHEN MONTH(DayTime)=4 THEN TotalNoPayed ELSE 0 END),0) AS April4, 
ISNULL(SUM(CASE WHEN MONTH(DayTime)=5 THEN TotalNoPayed ELSE 0 END),0) AS May5, 
ISNULL(SUM(CASE WHEN MONTH(DayTime)=6 THEN TotalNoPayed ELSE 0 END),0) AS June6, 
ISNULL(SUM(CASE WHEN MONTH(DayTime)=7 THEN TotalNoPayed ELSE 0 END),0) AS July7, 
ISNULL(SUM(CASE WHEN MONTH(DayTime)=8 THEN TotalNoPayed ELSE 0 END),0) AS August8, 
ISNULL(SUM(CASE WHEN MONTH(DayTime)=9 THEN TotalNoPayed ELSE 0 END),0) AS September9, 
ISNULL(SUM(CASE WHEN MONTH(DayTime)=10 THEN TotalNoPayed ELSE 0 END),0) AS October10, 
ISNULL(SUM(CASE WHEN MONTH(DayTime)=11 THEN TotalNoPayed ELSE 0 END),0) AS November11, 
ISNULL(SUM(CASE WHEN MONTH(DayTime)=12 THEN TotalNoPayed ELSE 0 END),0) AS December12 
FROM SupplierPaymentsRecord WITH(NOLOCK)
WHERE YEAR(DayTime)={0}
GROUP BY FilialeId";
            var newSQL = string.Format(SQL, year);
            using (var db = DatabaseFactory.CreateRdb()) 
            {
                return db.Select<SupplierPaymentsReportInfo>(true, newSQL).ToList();
            }
        }

        /// <summary>
        /// 应付款查询，根据公司和查询供应商的应付款明细  
        /// </summary>
        /// <param name="year"></param>
        /// <param name="filialeId"></param>
        /// <param name="companyName"></param>
        /// <returns></returns>
        public IList<SupplierPaymentsReportInfo> SelectPaymentsReportsGroupByCompanyId(int year, 
            Guid filialeId,string companyName)
        {
            const string SQL = @"SELECT '{0}' AS FilialeId,S.CompanyId,
ISNULL(SUM(CASE WHEN MONTH(DayTime)=1 THEN TotalAmount ELSE 0 END),0) AS January,
ISNULL(SUM(CASE WHEN MONTH(DayTime)=2 THEN TotalAmount ELSE 0 END),0) AS February,
ISNULL(SUM(CASE WHEN MONTH(DayTime)=3 THEN TotalAmount ELSE 0 END),0) AS March,
ISNULL(SUM(CASE WHEN MONTH(DayTime)=4 THEN TotalAmount ELSE 0 END),0) AS April, 
ISNULL(SUM(CASE WHEN MONTH(DayTime)=5 THEN TotalAmount ELSE 0 END),0) AS May, 
ISNULL(SUM(CASE WHEN MONTH(DayTime)=6 THEN TotalAmount ELSE 0 END),0) AS June, 
ISNULL(SUM(CASE WHEN MONTH(DayTime)=7 THEN TotalAmount ELSE 0 END),0) AS July, 
ISNULL(SUM(CASE WHEN MONTH(DayTime)=8 THEN TotalAmount ELSE 0 END),0) AS August, 
ISNULL(SUM(CASE WHEN MONTH(DayTime)=9 THEN TotalAmount ELSE 0 END),0) AS September, 
ISNULL(SUM(CASE WHEN MONTH(DayTime)=10 THEN TotalAmount ELSE 0 END),0) AS October, 
ISNULL(SUM(CASE WHEN MONTH(DayTime)=11 THEN TotalAmount ELSE 0 END),0) AS November, 
ISNULL(SUM(CASE WHEN MONTH(DayTime)=12 THEN TotalAmount ELSE 0 END),0) AS December,
ISNULL(SUM(CASE WHEN MONTH(DayTime)=1 THEN TotalNoPayed ELSE 0 END),0) AS January1,
ISNULL(SUM(CASE WHEN MONTH(DayTime)=2 THEN TotalNoPayed ELSE 0 END),0) AS February2,
ISNULL(SUM(CASE WHEN MONTH(DayTime)=3 THEN TotalNoPayed ELSE 0 END),0) AS March3,
ISNULL(SUM(CASE WHEN MONTH(DayTime)=4 THEN TotalNoPayed ELSE 0 END),0) AS April4, 
ISNULL(SUM(CASE WHEN MONTH(DayTime)=5 THEN TotalNoPayed ELSE 0 END),0) AS May5, 
ISNULL(SUM(CASE WHEN MONTH(DayTime)=6 THEN TotalNoPayed ELSE 0 END),0) AS June6, 
ISNULL(SUM(CASE WHEN MONTH(DayTime)=7 THEN TotalNoPayed ELSE 0 END),0) AS July7, 
ISNULL(SUM(CASE WHEN MONTH(DayTime)=8 THEN TotalNoPayed ELSE 0 END),0) AS August8, 
ISNULL(SUM(CASE WHEN MONTH(DayTime)=9 THEN TotalNoPayed ELSE 0 END),0) AS September9, 
ISNULL(SUM(CASE WHEN MONTH(DayTime)=10 THEN TotalNoPayed ELSE 0 END),0) AS October10, 
ISNULL(SUM(CASE WHEN MONTH(DayTime)=11 THEN TotalNoPayed ELSE 0 END),0) AS November11, 
ISNULL(SUM(CASE WHEN MONTH(DayTime)=12 THEN TotalNoPayed ELSE 0 END),0) AS December12 
FROM SupplierPaymentsRecord AS S WITH(NOLOCK)
WHERE FilialeId='{0}' AND YEAR(DayTime)={1} 
GROUP BY S.CompanyId";
            var newSQL = string.Format(SQL, filialeId, year);
            using (var db = DatabaseFactory.CreateRdb()) 
            {
                return db.Select<SupplierPaymentsReportInfo>(true, newSQL).ToList();
            }
        }

        /// <summary>
        /// 公司每月对应的采购出入库总金额
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        public IList<SupplierPaymentsReportInfo> SelectStockReprotsGroupByFilialeId(int year)
        {
            const string SQL = @"SELECT FilialeId,
ISNULL(SUM(CASE WHEN MONTH(DayTime)=1 THEN TotalAmount ELSE 0 END),0) AS January,
ISNULL(SUM(CASE WHEN MONTH(DayTime)=2 THEN TotalAmount ELSE 0 END),0) AS February,
ISNULL(SUM(CASE WHEN MONTH(DayTime)=3 THEN TotalAmount ELSE 0 END),0) AS March,
ISNULL(SUM(CASE WHEN MONTH(DayTime)=4 THEN TotalAmount ELSE 0 END),0) AS April, 
ISNULL(SUM(CASE WHEN MONTH(DayTime)=5 THEN TotalAmount ELSE 0 END),0) AS May, 
ISNULL(SUM(CASE WHEN MONTH(DayTime)=6 THEN TotalAmount ELSE 0 END),0) AS June, 
ISNULL(SUM(CASE WHEN MONTH(DayTime)=7 THEN TotalAmount ELSE 0 END),0) AS July, 
ISNULL(SUM(CASE WHEN MONTH(DayTime)=8 THEN TotalAmount ELSE 0 END),0) AS August, 
ISNULL(SUM(CASE WHEN MONTH(DayTime)=9 THEN TotalAmount ELSE 0 END),0)  AS September, 
ISNULL(SUM(CASE WHEN MONTH(DayTime)=10 THEN TotalAmount ELSE 0 END),0) AS October, 
ISNULL(SUM(CASE WHEN MONTH(DayTime)=11 THEN TotalAmount ELSE 0 END),0) AS November, 
ISNULL(SUM(CASE WHEN MONTH(DayTime)=12 THEN TotalAmount ELSE 0 END),0) AS December 
FROM SupplierPurchasingRecord WITH(NOLOCK)
WHERE YEAR(DayTime)={0}
GROUP BY FilialeId";
            var newSQL = string.Format(SQL,year);
            using (var db = DatabaseFactory.CreateRdb()) 
            {
                return db.Select<SupplierPaymentsReportInfo>(true, newSQL).ToList();
            }
        }

        /// <summary>
        /// 供应商每月采购出入库总金额
        /// </summary>
        /// <param name="year"></param>
        /// <param name="filialeId"></param>
        /// <param name="companyName"></param>
        /// <returns></returns>
        public IList<SupplierPaymentsReportInfo> SelectStockReportsGroupByCompanyId(int year, Guid filialeId,string companyName)
        {
            const string SQL = @"SELECT S.CompanyId,
ISNULL(SUM(CASE WHEN MONTH(DayTime)=1 THEN TotalAmount ELSE 0 END),0) AS January,
ISNULL(SUM(CASE WHEN MONTH(DayTime)=2 THEN TotalAmount ELSE 0 END),0) AS February,
ISNULL(SUM(CASE WHEN MONTH(DayTime)=3 THEN TotalAmount ELSE 0 END),0) AS March,
ISNULL(SUM(CASE WHEN MONTH(DayTime)=4 THEN TotalAmount ELSE 0 END),0) AS April, 
ISNULL(SUM(CASE WHEN MONTH(DayTime)=5 THEN TotalAmount ELSE 0 END),0) AS May, 
ISNULL(SUM(CASE WHEN MONTH(DayTime)=6 THEN TotalAmount ELSE 0 END),0) AS June, 
ISNULL(SUM(CASE WHEN MONTH(DayTime)=7 THEN TotalAmount ELSE 0 END),0) AS July, 
ISNULL(SUM(CASE WHEN MONTH(DayTime)=8 THEN TotalAmount ELSE 0 END),0) AS August, 
ISNULL(SUM(CASE WHEN MONTH(DayTime)=9 THEN TotalAmount ELSE 0 END),0)  AS September, 
ISNULL(SUM(CASE WHEN MONTH(DayTime)=10 THEN TotalAmount ELSE 0 END),0) AS October, 
ISNULL(SUM(CASE WHEN MONTH(DayTime)=11 THEN TotalAmount ELSE 0 END),0) AS November, 
ISNULL(SUM(CASE WHEN MONTH(DayTime)=12 THEN TotalAmount ELSE 0 END),0) AS December 
FROM SupplierPurchasingRecord AS S WITH(NOLOCK) 
WHERE YEAR(S.DayTime)={0} {1} 
GROUP BY S.CompanyId";
            string filieleStr = string.Empty;
            if (filialeId != Guid.Empty)
            {
                filieleStr = string.Format(" AND S.FilialeId='{0}' ", filialeId);
            }
            var newSQL = string.Format(SQL, year, filieleStr);
            using (var db = DatabaseFactory.CreateRdb()) 
            {
                return db.Select<SupplierPaymentsReportInfo>(true, newSQL).ToList();
            }
        }
        #endregion

        #region  出入库红冲更新存档数据

        /// <summary>
        /// 出入库红冲更新存档数据
        /// </summary>
        /// <param name="filialeId"></param>
        /// <param name="companyId"></param>
        /// <param name="dayTime"></param>
        /// <param name="tradeCode"></param>
        /// <returns></returns>
        public void ModifyRecord(Guid filialeId, Guid companyId, DateTime dayTime,string tradeCode)
        {
            const string SQL = @"declare @payDayTime date 
declare @accountReceiveble decimal(18,4)
SELECT TOP 1 @payDayTime=PaymentDayTime,@accountReceiveble=SUM(AccountReceivable) FROM [SupplierReckoningRecord] WITH(NOLOCK) 
WHERE FilialeId=@FilialeId 
AND CompanyId=@CompanyId
AND DayTime =@DayTime
AND LinkTradeCode=@LinkTradeCode
GROUP BY PaymentDayTime

IF(@accountReceiveble IS NOT NULL)
BEGIN
    UPDATE [SupplierReckoningRecord] SET [State]=2 WHERE LinkTradeCode=@LinkTradeCode    

	UPDATE [SupplierPurchasingRecord] SET TotalAmount=TotalAmount-@accountReceiveble 
	WHERE FilialeId=@FilialeId AND CompanyId=@CompanyId  
	AND DayTime=@DayTime

	UPDATE [SupplierPaymentsRecord] SET TotalAmount=TotalAmount-@accountReceiveble 
	WHERE FilialeId=@FilialeId AND CompanyId=@CompanyId 
	AND	DayTime=@payDayTime 
END";
            var parms = new[] {
                new SqlParameter("@FilialeId", filialeId),                
                new SqlParameter("@CompanyId", companyId),
                new SqlParameter("@DayTime", new DateTime(dayTime.Year,dayTime.Month,1)),
                new SqlParameter("@LinkTradeCode",tradeCode)
            };
            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_REPORT_DB_NAME, false, SQL, parms);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
        #endregion

        #region  针对当前月数据或者方案相关方法

        /// <summary>
        /// 每月一日
        /// </summary>
        /// <param name="dayTime"></param>
        /// <returns></returns>
        public bool ReRunEveryData(DateTime dayTime)
        {
            const string SQL_INSERT_STOCK = @"--  采购入库数据存档 
    INSERT INTO SupplierPurchasingRecord(FilialeId,CompanyId,DayTime,TotalAmount)
    SELECT
		 FilialeId,R.CompanyId,'{0}' AS DayTime,
		(ISNULL(sum(case when AccountReceivable < 0  then abs(AccountReceivable) else 0   end),0)
		-ISNULL(sum(case when AccountReceivable > 0 then abs(AccountReceivable) else 0   end),0)) AS TotalAmount
	FROM [SupplierReckoningRecord] AS R with(nolock) 
	where R.DayTIme='{0}' AND R.[State]<>2 
	GROUP BY R.FilialeId,R.CompanyId
 ";
            const string SQL_INSERT_PAYMENT = @"--  应付款数据存档数据存档 
    INSERT INTO SupplierPaymentsRecord(FilialeId,CompanyId,DayTime,TotalAmount,TotalNoPayed)
    SELECT
		 FilialeId,R.CompanyId,'{0}' AS DayTime,
		(ISNULL(sum(case when AccountReceivable < 0  then abs(AccountReceivable) else 0   end),0) AS TotalAmount,
		(ISNULL(sum(case when AccountReceivable < 0 and IsChecked =0  then abs(AccountReceivable) else 0   end),0) 
		-ISNULL(sum(case when AccountReceivable > 0 and IsChecked =0  then AccountReceivable else 0   end),0)) as TotalNoPayed  
	FROM [SupplierReckoningRecord] AS R with(nolock) 
	where R.PaymentDayTime ='{0}' AND R.[State]<>2 
	GROUP BY R.FilialeId,R.CompanyID
 ";
            using (var connection = Databases.GetSqlConnection(GlobalConfig.ERP_REPORT_DB_NAME, false))
            {
                connection.Open();
                SqlTransaction trans = connection.BeginTransaction();

                try
                {
                    SqlHelper.ExecuteNonQuery(trans,string.Format(SQL_INSERT_PAYMENT, dayTime));
                    SqlHelper.ExecuteNonQuery(trans,string.Format(SQL_INSERT_STOCK, dayTime));
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw new ApplicationException(ex.Message);
                }
            }
            return true;
        }
        #endregion

        #region  报表往来帐明细相关

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isTemp"></param>
        /// <param name="reckoningInfos"></param>
        /// <returns></returns>
        public bool InsertRececkoning(bool isTemp, IList<RecordReckoningInfo> reckoningInfos)
        {
            var dics = new Dictionary<string, string>
                {
                    {"ReckoningId","ReckoningId"},{"FilialeId","FilialeId"},{"CompanyId","CompanyId"},{"TradeCode","TradeCode"},
                    {"DayTime","DayTime"},{"PaymentDayTime","PaymentDayTime"},{"AccountReceivable","AccountReceivable"},{"State","State"},
                    {"LinkTradeType","LinkTradeType"},{"LinkTradeCode","LinkTradeCode"},{"IsChecked","IsChecked"}
                };
            if (isTemp)
            {
                const string SQL = @" DELETE TempSupplierReckoningRecord ";
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_REPORT_DB_NAME, false, SQL);
                return SqlHelper.BatchInsert(GlobalConfig.ERP_REPORT_DB_NAME, reckoningInfos, "TempSupplierReckoningRecord", dics) > 0;
            }
            return SqlHelper.BatchInsert(GlobalConfig.ERP_REPORT_DB_NAME, reckoningInfos, "SupplierReckoningRecord", dics) > 0;
        }

        /// <summary>
        /// 判断当月往来帐是否记录
        /// </summary>
        /// <param name="dayTime"></param>
        /// <returns></returns>
        public bool IsExists(DateTime dayTime)
        {
            const string SQL = @"SELECT COUNT(*) FROM SupplierReckoningRecord WITH(NOLOCK) WHERE DayTime='{0}'";
            using (var db = DatabaseFactory.CreateRdb())
            {
                return db.GetValue<int>(true, string.Format(SQL, dayTime)) > 0;
            }
        }

        /// <summary>
        /// 判断当月往来帐是否记录 （某天）
        /// </summary>
        /// <param name="dayTime"></param>
        /// <returns></returns>
        public bool IsExistsRecent(DateTime dayTime)
        {
            const string SQL = @"SELECT COUNT(0) FROM TempSupplierReckoningRecord WITH(NOLOCK) WHERE DayTime='{0}'";
            using (var db = DatabaseFactory.CreateRdb())
            {
                return db.GetValue<int>(true, string.Format(SQL, dayTime)) > 0;
            }
        }
        #endregion

        #region  从临时表中获取当月应付款、采购入库数据

        /// <summary>
        /// 获取当月公司往来单位应付款
        /// </summary>
        /// <returns></returns>
        public IList<SupplierPaymentsRecordInfo> GetPaymentsByForFiliale()
        {
            var startTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            IList<SupplierPaymentsRecordInfo> list = new List<SupplierPaymentsRecordInfo>();
            const string SQL = @"SELECT
		 FilialeId,
		(ISNULL(sum(case when AccountReceivable < 0  then abs(AccountReceivable) else 0   end),0)
		-ISNULL(sum(case when AccountReceivable >0 then AccountReceivable else 0   end),0)) AS TotalAmount,
		(ISNULL(sum(case when AccountReceivable < 0 and IsChecked =0  then abs(AccountReceivable) else 0   end),0) 
		-ISNULL(sum(case when AccountReceivable > 0 and IsChecked =0  then AccountReceivable else 0   end),0)) as TotalNoPayed  
	FROM [SupplierReckoningRecord] AS R with(nolock) 
	where R.PaymentDayTime ='{0}' AND R.[State]<>2 
	GROUP BY R.FilialeId";
            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_REPORT_DB_NAME, true, string.Format(SQL, startTime)))
            {
                while (rdr.Read())
                {
                    list.Add(new SupplierPaymentsRecordInfo
                    {
                        FilialeId = rdr.GetGuid(0),
                        TotalAmount = rdr.GetDecimal(1),
                        TotalNoPayed = rdr.GetDecimal(2)
                    });
                }
            }
            return list;
        }

        /// <summary>
        /// 获取当月往来单位往来单位应付款
        /// </summary>
        /// <returns></returns>
        public IList<SupplierPaymentsRecordInfo> GetPaymentsByForCompany(Guid filialeId)
        {
            var startTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            IList<SupplierPaymentsRecordInfo> list = new List<SupplierPaymentsRecordInfo>();
            const string SQL = @"SELECT R.CompanyId,
		(ISNULL(sum(case when AccountReceivable < 0  then abs(AccountReceivable) else 0   end),0)
		-ISNULL(sum(case when AccountReceivable >0 then AccountReceivable else 0   end),0)) AS TotalAmount,
		(ISNULL(sum(case when AccountReceivable < 0 and IsChecked =0  then abs(AccountReceivable) else 0   end),0) 
		-ISNULL(sum(case when AccountReceivable > 0 and IsChecked =0  then AccountReceivable else 0   end),0)) as TotalNoPayed  
	FROM [SupplierReckoningRecord] AS R with(nolock) 
	where R.PaymentDayTime ='{0}' AND FilialeId='{1}' AND R.[State]<>2 
	GROUP BY R.CompanyID";
            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_REPORT_DB_NAME, true, string.Format(SQL, startTime, filialeId)))
            {
                while (rdr.Read())
                {
                    list.Add(new SupplierPaymentsRecordInfo
                    {
                        FilialeId = filialeId,
                        CompanyId = rdr.GetGuid(0),
                        TotalAmount = rdr.GetDecimal(1),
                        TotalNoPayed = rdr.GetDecimal(2)
                    });
                }
            }
            return list;
        }


        /// <summary>
        /// 获取当月公司采购入库统计金额
        /// </summary>
        /// <returns></returns>
        public Dictionary<Guid, decimal> GetPurchasingByForFiliale()
        {
            const string SQL = @"SELECT FilialeId,
		(ISNULL(sum(case when AccountReceivable < 0  then abs(AccountReceivable) else 0   end),0)
		-ISNULL(sum(case when AccountReceivable > 0 then abs(AccountReceivable) else 0   end),0)) AS TotalAmount
	FROM [TempSupplierReckoningRecord] AS R with(nolock) 
	where R.[State]<>2 
	GROUP BY R.FilialeId";
            var dics = new Dictionary<Guid, decimal>();
            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_REPORT_DB_NAME, true, SQL))
            {
                while (rdr.Read())
                {
                    dics.Add(rdr.GetGuid(0), rdr.GetDecimal(1));
                }
            }
            return dics;
        }

        /// <summary>
        /// 获取当月往来单位往来单位应付款
        /// </summary>
        /// <returns></returns>
        public Dictionary<Guid, decimal> GetPurchasingByForCompany(Guid filialeId)
        {
            const string SQL = @"SELECT CompanyId,
		(ISNULL(sum(case when AccountReceivable < 0  then abs(AccountReceivable) else 0   end),0)
		-ISNULL(sum(case when AccountReceivable > 0 then abs(AccountReceivable) else 0   end),0)) AS TotalAmount
	FROM [TempSupplierReckoningRecord] AS R with(nolock) 
	where FilialeId='{0}' AND R.[State]<>2 
	GROUP BY R.CompanyId";
            var dics = new Dictionary<Guid, decimal>();
            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_REPORT_DB_NAME, true, string.Format(SQL, filialeId)))
            {
                while (rdr.Read())
                {
                    dics.Add(rdr.GetGuid(0), rdr.GetDecimal(1));
                }
            }
            return dics;
        }
        #endregion
    }
}
