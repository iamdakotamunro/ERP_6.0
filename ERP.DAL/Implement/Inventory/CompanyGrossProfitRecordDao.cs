using ERP.DAL.Interface.IInventory;
using ERP.Environment;
using ERP.Model.Report;
using Keede.DAL.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Transactions;

namespace ERP.DAL.Implement.Inventory
{
    public class CompanyGrossProfitRecordDao : ICompanyGrossProfitRecord
    {
        /// <summary>
        /// 删除特定时间内的临时数据
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public bool DeleteData(int year, int month)
        {
            const string SQL = @"DELETE CompanyGrossProfitRecord WHERE YEAR(DayTime)=@YEAR AND Month(DayTime)=@Month ";
            try
            {
                var parms = new[]
                {
                    new Parameter("@YEAR", year), 
                    new Parameter("@Month", month)
                };
                using (var db = DatabaseFactory.CreateRdb())
                {
                    db.Execute(false, SQL, parms);
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 是否存在公司毛利记录数据
        /// </summary>
        /// <param name="dayTime"></param>
        /// <returns></returns>
        public bool Exists(DateTime dayTime)
        {
            const string SQL = "SELECT COUNT(0) FROM CompanyGrossProfitRecord WHERE DayTime=@DayTime";
            var parms = new[]
            {
                new Parameter("@DayTime", dayTime)
            };
            using (var db = DatabaseFactory.CreateRdb())
            {
                return db.GetValue<int>(true, SQL, parms) > 0;
            }
        }

        /// <summary>
        /// 批量插入公司毛利记录
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public bool AddData(IList<CompanyGrossProfitRecordInfo> list)
        {
            var dics = new Dictionary<string, string>
                {
                    {"ID","ID"},{"SaleFilialeId","SaleFilialeId"},{"SalePlatformId","SalePlatformId"},{"SalesAmount","SalesAmount"},
                    {"GoodsAmount","GoodsAmount"},{"ShipmentIncome","ShipmentIncome"},{"PromotionsDeductible","PromotionsDeductible"},
                    {"PointsDeduction","PointsDeduction"},{"ShipmentCost","ShipmentCost"},{"PurchaseCosts","PurchaseCosts"},
                    {"CatCommission","CatCommission"},{"OrderType","OrderType"},{"DayTime","DayTime"}
                };
            return SqlHelper.BatchInsert(GlobalConfig.ERP_REPORT_DB_NAME, list, "CompanyGrossProfitRecord", dics) > 0;
        }

        /// <summary>
        /// 修改公司毛利记录(如果已存在则修改，不存在添加)
        /// </summary>
        /// <param name="companyGrossProfitRecordInfo"></param>
        /// <returns></returns>
        public bool UpdateCompanyGrossProfitRecordInfo(CompanyGrossProfitRecordInfo companyGrossProfitRecordInfo)
        {
            string sql = @"
    IF EXISTS(SELECT TOP 1 1 FROM CompanyGrossProfitRecord WHERE SaleFilialeId =@SaleFilialeId AND SalePlatformId =@SalePlatformId AND OrderType =@OrderType AND DayTime =@DayTime)
	BEGIN
	
        BEGIN TRY	
            BEGIN TRANSACTION         
			
            UPDATE CompanyGrossProfitRecord
            SET SalesAmount=SalesAmount+@SalesAmount,GoodsAmount=GoodsAmount+@GoodsAmount,ShipmentIncome=ShipmentIncome+@ShipmentIncome,
            PromotionsDeductible=PromotionsDeductible+@PromotionsDeductible,PointsDeduction=PointsDeduction+@PointsDeduction,
            ShipmentCost=ShipmentCost+@ShipmentCost,PurchaseCosts=PurchaseCosts+@PurchaseCosts,CatCommission=CatCommission+@CatCommission
            WHERE SaleFilialeId =@SaleFilialeId AND SalePlatformId =@SalePlatformId AND OrderType =@OrderType AND DayTime =@DayTime
            
            UPDATE CompanyGrossProfitRecordDetail
            SET [State]=1
            WHERE [State]=0 AND SaleFilialeId =@SaleFilialeId AND SalePlatformId =@SalePlatformId AND OrderType =@OrderType AND CONVERT(VARCHAR(7),OrderTime,120)+'-01' =@DayTime
            
            COMMIT TRANSACTION        
        END TRY
	    BEGIN CATCH
		    ROLLBACK TRANSACTION
	    END CATCH

	END
	ELSE
	BEGIN
		INSERT INTO  CompanyGrossProfitRecord (
		    [ID]
           ,[SaleFilialeId]
           ,[SalePlatformId]
           ,[SalesAmount]
           ,[GoodsAmount]
           ,[ShipmentIncome]
           ,[PromotionsDeductible]
           ,[PointsDeduction]
           ,[ShipmentCost]
           ,[PurchaseCosts]
           ,[CatCommission]
           ,[OrderType]
           ,[DayTime])
		VALUES(
		    NEWID()
           ,@SaleFilialeId
           ,@SalePlatformId
           ,@SalesAmount
           ,@GoodsAmount
           ,@ShipmentIncome
           ,@PromotionsDeductible
           ,@PointsDeduction
           ,@ShipmentCost
           ,@PurchaseCosts
           ,@CatCommission
           ,@OrderType
           ,@DayTime)
	END";

            using (var connection = Keede.DAL.RWSplitting.Databases.GetSqlConnection(GlobalConfig.ERP_REPORT_DB_NAME, false))
            {
                try
                {
                    connection.Open();
                    var command = new SqlCommand(sql, connection) { CommandTimeout = 600 };
                    command.Parameters.Add(new SqlParameter("@SalesAmount", companyGrossProfitRecordInfo.SalesAmount));
                    command.Parameters.Add(new SqlParameter("@GoodsAmount", companyGrossProfitRecordInfo.GoodsAmount));
                    command.Parameters.Add(new SqlParameter("@ShipmentIncome", companyGrossProfitRecordInfo.ShipmentIncome));
                    command.Parameters.Add(new SqlParameter("@PromotionsDeductible", companyGrossProfitRecordInfo.PromotionsDeductible));
                    command.Parameters.Add(new SqlParameter("@PointsDeduction", companyGrossProfitRecordInfo.PointsDeduction));
                    command.Parameters.Add(new SqlParameter("@ShipmentCost", companyGrossProfitRecordInfo.ShipmentCost));
                    command.Parameters.Add(new SqlParameter("@PurchaseCosts", companyGrossProfitRecordInfo.PurchaseCosts));
                    command.Parameters.Add(new SqlParameter("@CatCommission", companyGrossProfitRecordInfo.CatCommission));
                    command.Parameters.Add(new SqlParameter("@SaleFilialeId", companyGrossProfitRecordInfo.SaleFilialeId));
                    command.Parameters.Add(new SqlParameter("@SalePlatformId", companyGrossProfitRecordInfo.SalePlatformId));
                    command.Parameters.Add(new SqlParameter("@OrderType", companyGrossProfitRecordInfo.OrderType));
                    command.Parameters.Add(new SqlParameter("@DayTime", companyGrossProfitRecordInfo.DayTime.ToString("yyyy-MM-01")));
                    return command.ExecuteNonQuery() > 0;
                }
                catch (Exception ex)
                {
                    throw new Exception("公司毛利存档-处理完成时间超过一个自然月或一个自然月以上的数据异常", ex);
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        /// <summary>
        /// 查询历史月份公司毛利信息
        /// </summary>
        /// <param name="startTime">记录年月</param>
        /// <param name="endTime"></param>
        /// <param name="saleFilialeId">销售公司</param>
        /// <param name="salePlatformIds">销售平台</param>
        /// <param name="orderTypes">订单类型(0:网络发货订单;1:门店采购订单;2:帮门店发货订单;)</param>
        /// <returns></returns>
        public IList<CompanyGrossProfitRecordInfo> SelectCompanyGrossProfitInfos(DateTime startTime, DateTime? endTime, Guid saleFilialeId, string salePlatformIds, string orderTypes)
        {
            string salePlatformIdStr = string.Empty, orderTypeStr = string.Empty;
            var builder = new StringBuilder(@"
           SELECT [ID]
          ,[SaleFilialeId]
          ,[SalePlatformId]
          ,[SalesAmount]
          ,[GoodsAmount]
          ,[ShipmentIncome]
          ,[PromotionsDeductible]
          ,[PointsDeduction]
          ,[ShipmentCost]
          ,[PurchaseCosts]
          ,[CatCommission]
          ,[OrderType]
          ,[DayTime]
           FROM [CompanyGrossProfitRecord] WITH(NOLOCK) WHERE 1=1 ");
            if (endTime == null || endTime == DateTime.MinValue)
            {
                builder.AppendFormat(" AND DayTime='{0}' ", startTime);
            }
            else
            {
                builder.AppendFormat(" AND DayTime>='{0}' AND DayTime<'{1}'", startTime, endTime);
            }

            if (!string.IsNullOrEmpty(salePlatformIds))
            {
                salePlatformIdStr = salePlatformIds.Split(',').Aggregate(salePlatformIdStr, (current, item) => current + string.Format(",'{0}'", item)).Substring(1);
                builder.AppendFormat(" AND SalePlatformId IN({0}) ", salePlatformIdStr);
            }
            if (!string.IsNullOrEmpty(orderTypes))
            {
                orderTypeStr = orderTypes.Split(',').Aggregate(orderTypeStr, (current, item) => current + string.Format(",{0}", item)).Substring(1);
                builder.AppendFormat(" AND OrderType IN({0}) ", orderTypeStr);
            }

            if (saleFilialeId != Guid.Empty)
            {
                builder.AppendFormat(" AND SaleFilialeId = '{0}' ", saleFilialeId);
            }

            IList<CompanyGrossProfitRecordInfo> dataList = new List<CompanyGrossProfitRecordInfo>();

                using (var connection = Keede.DAL.RWSplitting.Databases.GetSqlConnection(GlobalConfig.ERP_REPORT_DB_NAME, Transaction.Current == null))
                {
                    IDataReader rdr = null;
                try
                {
                    connection.Open();
                    var command = new SqlCommand(builder.ToString(), connection) { CommandTimeout = 600 };
                    rdr = command.ExecuteReader();
                    while (rdr.Read())
                    {
                        dataList.Add(new CompanyGrossProfitRecordInfo
                        {
                            ID = SqlRead.GetGuid(rdr, "ID"),
                            SaleFilialeId = SqlRead.GetGuid(rdr, "SaleFilialeId"),
                            SalePlatformId = SqlRead.GetGuid(rdr, "SalePlatformId"),
                            SalesAmount = SqlRead.GetDecimal(rdr, "SalesAmount"),
                            GoodsAmount = SqlRead.GetDecimal(rdr, "GoodsAmount"),
                            ShipmentIncome = SqlRead.GetDecimal(rdr, "ShipmentIncome"),
                            PromotionsDeductible = SqlRead.GetDecimal(rdr, "PromotionsDeductible"),
                            PointsDeduction = SqlRead.GetDecimal(rdr, "PointsDeduction"),
                            ShipmentCost = SqlRead.GetDecimal(rdr, "ShipmentCost"),
                            PurchaseCosts = SqlRead.GetDecimal(rdr, "PurchaseCosts"),
                            CatCommission = SqlRead.GetDecimal(rdr, "CatCommission"),
                            OrderType = SqlRead.GetInt(rdr, "OrderType"),
                            DayTime = SqlRead.GetDateTime(rdr, "DayTime")
                        });
                    }
                }
                finally
                {
                    connection.Close();
                    if (rdr != null)
                        rdr.Close();
                }
            }
            return dataList;
        }

        /// <summary>
        /// 汇总同一公司不同平台的数据
        /// </summary>
        /// <param name="startTime">记录年月</param>
        /// <param name="endTime"></param>
        /// <param name="saleFilialeId">销售公司</param>
        /// <param name="salePlatformIds">销售平台</param>
        /// <param name="orderTypes">订单类型(0:网络发货订单;1:门店采购订单;2:帮门店发货订单;)</param>
        /// <returns></returns>
        public IList<CompanyGrossProfitRecordInfo> SumCompanyGrossProfitFromMonthBySaleFilialeId(DateTime startTime, DateTime? endTime, Guid saleFilialeId, string salePlatformIds, string orderTypes)
        {
            string salePlatformIdStr = string.Empty, orderTypeStr = string.Empty;
            var builder = new StringBuilder(@"
             SELECT 
             [SaleFilialeId]
            ,'00000000-0000-0000-0000-000000000000' AS [SalePlatformId]
            ,SUM([SalesAmount]) AS [SalesAmount]
            ,SUM([GoodsAmount]) AS [GoodsAmount]
            ,SUM([ShipmentIncome]) AS [ShipmentIncome]
            ,SUM([PromotionsDeductible]) AS [PromotionsDeductible]
            ,SUM([PointsDeduction]) AS [PointsDeduction]
            ,SUM([ShipmentCost]) AS [ShipmentCost]
            ,SUM([PurchaseCosts]) AS [PurchaseCosts]
            ,SUM([CatCommission]) AS [CatCommission]
            ,'-1' AS [OrderType]
             FROM [CompanyGrossProfitRecord] WITH(NOLOCK) WHERE 1=1 ");
            if (endTime == null || endTime == DateTime.MinValue)
            {
                builder.AppendFormat(" AND DayTime='{0}' ", startTime);
            }
            else
            {
                builder.AppendFormat(" AND DayTime>='{0}' AND DayTime<'{1}'", startTime, endTime);
            }

            if (!string.IsNullOrEmpty(salePlatformIds))
            {
                salePlatformIdStr = salePlatformIds.Split(',').Aggregate(salePlatformIdStr, (current, item) => current + string.Format(",'{0}'", item)).Substring(1);
                builder.AppendFormat(" AND SalePlatformId IN({0}) ", salePlatformIdStr);
            }
            if (!string.IsNullOrEmpty(orderTypes))
            {
                orderTypeStr = orderTypes.Split(',').Aggregate(orderTypeStr, (current, item) => current + string.Format(",{0}", item)).Substring(1);
                builder.AppendFormat(" AND OrderType IN({0}) ", orderTypeStr);
            }

            if (saleFilialeId != Guid.Empty)
            {
                builder.AppendFormat(" AND SaleFilialeId = '{0}' ", saleFilialeId);
            }
            builder.Append(" GROUP BY [SaleFilialeId]");

            IList<CompanyGrossProfitRecordInfo> dataList = new List<CompanyGrossProfitRecordInfo>();

            using (var connection = Keede.DAL.RWSplitting.Databases.GetSqlConnection(GlobalConfig.ERP_REPORT_DB_NAME, Transaction.Current == null))
            {
                IDataReader rdr = null;
                try
                {
                    connection.Open();
                    var command = new SqlCommand(builder.ToString(), connection) { CommandTimeout = 600 };
                    rdr = command.ExecuteReader();
                    while (rdr.Read())
                    {
                        dataList.Add(new CompanyGrossProfitRecordInfo
                        {
                            SaleFilialeId = SqlRead.GetGuid(rdr, "SaleFilialeId"),
                            SalePlatformId = SqlRead.GetGuid(rdr, "SalePlatformId"),
                            SalesAmount = SqlRead.GetDecimal(rdr, "SalesAmount"),
                            GoodsAmount = SqlRead.GetDecimal(rdr, "GoodsAmount"),
                            ShipmentIncome = SqlRead.GetDecimal(rdr, "ShipmentIncome"),
                            PromotionsDeductible = SqlRead.GetDecimal(rdr, "PromotionsDeductible"),
                            PointsDeduction = SqlRead.GetDecimal(rdr, "PointsDeduction"),
                            ShipmentCost = SqlRead.GetDecimal(rdr, "ShipmentCost"),
                            PurchaseCosts = SqlRead.GetDecimal(rdr, "PurchaseCosts"),
                            CatCommission = SqlRead.GetDecimal(rdr, "CatCommission"),
                            OrderType = SqlRead.GetInt(rdr, "OrderType")
                        });
                    }
                }
                finally
                {
                    connection.Close();
                    if (rdr != null)
                        rdr.Close();
                }
            }
            return dataList;
        }

        /// <summary>
        /// 汇总同一公司同一订单类型不同平台的数据  说明：“门店采购订单”和“帮门店发货订单”按公司和订单类型合计，网络发货订单不进行合计，即将门店数据汇总
        /// </summary>
        /// <param name="startTime">记录年月</param>
        /// <param name="endTime"></param>
        /// <param name="saleFilialeId">销售公司</param>
        /// <param name="salePlatformIds">销售平台</param>
        /// <param name="orderTypes">订单类型(0:网络发货订单;1:门店采购订单;2:帮门店发货订单;)</param>
        /// <returns></returns>
        public IList<CompanyGrossProfitRecordInfo> SumCompanyGrossProfitFromMonthBySaleFilialeIdAndOrderType(DateTime startTime, DateTime? endTime, Guid saleFilialeId, string salePlatformIds, string orderTypes)
        {
            string salePlatformIdStr = string.Empty, orderTypeStr = string.Empty;
            var parameter = new StringBuilder();
            if (endTime == null || endTime == DateTime.MinValue)
            {
                parameter.AppendFormat(" AND DayTime='{0}' ", startTime);
            }
            else
            {
                parameter.AppendFormat(" AND DayTime>='{0}' AND DayTime<'{1}'", startTime, endTime);
            }

            if (!string.IsNullOrEmpty(salePlatformIds))
            {
                salePlatformIdStr = salePlatformIds.Split(',').Aggregate(salePlatformIdStr, (current, item) => current + string.Format(",'{0}'", item)).Substring(1);
                parameter.AppendFormat(" AND SalePlatformId IN({0}) ", salePlatformIdStr);
            }
            if (!string.IsNullOrEmpty(orderTypes))
            {
                orderTypeStr = orderTypes.Split(',').Aggregate(orderTypeStr, (current, item) => current + string.Format(",{0}", item)).Substring(1);
                parameter.AppendFormat(" AND OrderType IN({0}) ", orderTypeStr);
            }

            if (saleFilialeId != Guid.Empty)
            {
                parameter.AppendFormat(" AND SaleFilialeId = '{0}' ", saleFilialeId);
            }

            var builder = new StringBuilder(@"
             SELECT 
             [SaleFilialeId]
            ,'00000000-0000-0000-0000-000000000000' AS [SalePlatformId]
            ,SUM([SalesAmount]) AS [SalesAmount]
            ,SUM([GoodsAmount]) AS [GoodsAmount]
            ,SUM([ShipmentIncome]) AS [ShipmentIncome]
            ,SUM([PromotionsDeductible]) AS [PromotionsDeductible]
            ,SUM([PointsDeduction]) AS [PointsDeduction]
            ,SUM([ShipmentCost]) AS [ShipmentCost]
            ,SUM([PurchaseCosts]) AS [PurchaseCosts]
            ,SUM([CatCommission]) AS [CatCommission]
            ,[OrderType]
             FROM [CompanyGrossProfitRecord] WITH(NOLOCK) 
             WHERE 1=1 AND [OrderType] IN(1,2)");
            builder.Append(parameter);
            builder.Append(" GROUP BY [SaleFilialeId],[OrderType]");
            builder.Append(" UNION");
            builder.Append(@" 
             SELECT 
             [SaleFilialeId]
            ,[SalePlatformId]
            ,SUM([SalesAmount]) AS[SalesAmount]
            ,SUM([GoodsAmount]) AS[GoodsAmount]
            ,SUM([ShipmentIncome]) AS[ShipmentIncome]
            ,SUM([PromotionsDeductible]) AS[PromotionsDeductible]
            ,SUM([PointsDeduction]) AS[PointsDeduction]
            ,SUM([ShipmentCost]) AS[ShipmentCost]
            ,SUM([PurchaseCosts]) AS[PurchaseCosts]
            ,SUM([CatCommission]) AS[CatCommission]
            ,[OrderType]
            FROM [CompanyGrossProfitRecord] WITH(NOLOCK) 
            WHERE 1=1 AND [OrderType]=0");
            builder.Append(parameter);
            builder.Append(" GROUP BY [SaleFilialeId],[SalePlatformId],[OrderType]");

            IList<CompanyGrossProfitRecordInfo> dataList = new List<CompanyGrossProfitRecordInfo>();

            using (var connection = Keede.DAL.RWSplitting.Databases.GetSqlConnection(GlobalConfig.ERP_REPORT_DB_NAME, Transaction.Current == null))
            {
                IDataReader rdr = null;
                try
                {
                    connection.Open();
                    var command = new SqlCommand(builder.ToString(), connection) { CommandTimeout = 600 };
                    rdr = command.ExecuteReader();
                    while (rdr.Read())
                    {
                        dataList.Add(new CompanyGrossProfitRecordInfo
                        {
                            SaleFilialeId = SqlRead.GetGuid(rdr, "SaleFilialeId"),
                            SalePlatformId = SqlRead.GetGuid(rdr, "SalePlatformId"),
                            SalesAmount = SqlRead.GetDecimal(rdr, "SalesAmount"),
                            GoodsAmount = SqlRead.GetDecimal(rdr, "GoodsAmount"),
                            ShipmentIncome = SqlRead.GetDecimal(rdr, "ShipmentIncome"),
                            PromotionsDeductible = SqlRead.GetDecimal(rdr, "PromotionsDeductible"),
                            PointsDeduction = SqlRead.GetDecimal(rdr, "PointsDeduction"),
                            ShipmentCost = SqlRead.GetDecimal(rdr, "ShipmentCost"),
                            PurchaseCosts = SqlRead.GetDecimal(rdr, "PurchaseCosts"),
                            CatCommission = SqlRead.GetDecimal(rdr, "CatCommission"),
                            OrderType = SqlRead.GetInt(rdr, "OrderType")
                        });
                    }
                }
                finally
                {
                    connection.Close();
                    if (rdr != null)
                        rdr.Close();
                }
            }
            return dataList;
        }
    }
}
