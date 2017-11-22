using ERP.DAL.Interface.IInventory;
using ERP.Environment;
using ERP.Model.Report;
using Keede.DAL.Helper;
using Keede.DAL.RWSplitting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Transactions;

namespace ERP.DAL.Implement.Inventory
{
    public class CompanyGrossProfitRecordDetailDao : ICompanyGrossProfitRecordDetail
    {
        /// <summary>
        /// 是否存在公司毛利记录数据明细
        /// </summary>
        /// <param name="dayTime"></param>
        /// <returns></returns>
        public bool Exists(DateTime dayTime)
        {
            const string SQL = "SELECT COUNT(0) FROM CompanyGrossProfitRecordDetail WHERE DayTime=@DayTime";
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
        /// 批量插入公司毛利记录明细
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public bool AddDataDetail(IList<CompanyGrossProfitRecordDetailInfo> list)
        {
            var dics = new Dictionary<string, string>
                {
                    {"ID","ID"},{"StockAndOrderId","StockAndOrderId"},{"StockAndOrderNo","StockAndOrderNo"},{"SaleFilialeId","SaleFilialeId"},{"SalePlatformId","SalePlatformId"},{"SalesAmount","SalesAmount"},
                    {"GoodsAmount","GoodsAmount"},{"ShipmentIncome","ShipmentIncome"},{"PromotionsDeductible","PromotionsDeductible"},
                    {"PointsDeduction","PointsDeduction"},{"ShipmentCost","ShipmentCost"},{"PurchaseCosts","PurchaseCosts"},
                    {"CatCommission","CatCommission"},{"OrderType","OrderType"},{"OrderTime","OrderTime"},{"DayTime","DayTime"},{"State","State"}
                };
            return SqlHelper.BatchInsert(GlobalConfig.ERP_REPORT_DB_NAME, list, "CompanyGrossProfitRecordDetail", dics) > 0;
        }

        /// <summary>
        /// 根据订单时间修改数据状态
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns></returns>
        public bool UpdateState(DateTime startTime, DateTime endTime)
        {
            string sql = @"
            UPDATE CompanyGrossProfitRecordDetail
            SET [State]=1
            WHERE [State]=0 AND OrderTime>='{0}' AND OrderTime<'{1}'
            ";
            sql = string.Format(sql, startTime, endTime);

            using (var connection = Databases.GetSqlConnection(GlobalConfig.ERP_REPORT_DB_NAME, false))
            {
                try
                {
                    connection.Open();
                    var command = new SqlCommand(sql, connection) { CommandTimeout = 600 };
                    return command.ExecuteNonQuery() > 0;
                }
                catch (Exception ex)
                {
                    throw new Exception("公司毛利存档-修改公司毛利明细异常", ex);
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        /// <summary>
        /// 更新每天产生的交易佣金
        /// </summary>
        /// <returns></returns>
        /// zal 2016-09-20
        public bool UpdateCatCommission()
        {
            string sql = @"
            BEGIN TRY 
                BEGIN TRANSACTION        
				
                UPDATE CompanyGrossProfitRecordDetail
                SET CatCommission=CatCommission+B.Income
                FROM CompanyGrossProfitRecordDetail AS A WITH(NOLOCK) INNER JOIN WasteBook AS B WITH(NOLOCK)
                ON A.StockAndOrderNo=B.OrderNo AND B.State=0
                
                UPDATE CompanyGrossProfitRecord
                SET CatCommission=C.CatCommission+B.Income
                FROM CompanyGrossProfitRecordDetail AS A WITH(NOLOCK) INNER JOIN WasteBook AS B WITH(NOLOCK)
                ON A.StockAndOrderNo=B.OrderNo AND A.State=1 AND B.State=0 
                INNER JOIN CompanyGrossProfitRecord AS C WITH(NOLOCK)
                ON A.SaleFilialeId=C.SaleFilialeId AND A.SalePlatformId=C.SalePlatformId AND A.OrderType=C.OrderType AND CONVERT(VARCHAR(7),A.OrderTime,120)+'-01'=C.DayTime
                
                UPDATE WasteBook
                SET State=1
                WHERE State=0

                COMMIT TRANSACTION
            END TRY
            BEGIN CATCH
			    ROLLBACK TRANSACTION
	        END CATCH
            ";
            using (var connection = Databases.GetSqlConnection(GlobalConfig.ERP_REPORT_DB_NAME, false))
            {
                connection.Open();
                var command = new SqlCommand(sql, connection) { CommandTimeout = 600 };
                return command.ExecuteNonQuery() > 0;
            }
        }

        /// <summary>
        /// 根据条件合计公司毛利明细表数据
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="saleFilialeId">销售公司</param>
        /// <param name="salePlatformIds">销售平台</param>
        /// <param name="orderTypes">订单类型(0:网络发货订单;1:门店采购订单;2:帮门店发货订单;)</param>
        /// <returns></returns>
        public IList<CompanyGrossProfitRecordInfo> SumCompanyGrossProfitDetailInfos(DateTime startTime, DateTime endTime, Guid saleFilialeId, string salePlatformIds, string orderTypes)
        {
            string salePlatformIdStr = string.Empty, orderTypeStr = string.Empty;
            var builder = new StringBuilder(@"
           SELECT 
           [SaleFilialeId]
          ,[SalePlatformId]
          ,SUM([SalesAmount]) AS [SalesAmount]
          ,SUM([GoodsAmount]) AS [GoodsAmount]
          ,SUM([ShipmentIncome]) AS [ShipmentIncome]
          ,SUM([PromotionsDeductible]) AS [PromotionsDeductible]
          ,SUM([PointsDeduction]) AS [PointsDeduction]
          ,SUM([ShipmentCost]) AS [ShipmentCost]
          ,SUM([PurchaseCosts]) AS [PurchaseCosts]
          ,SUM([CatCommission]) AS [CatCommission]
          ,[OrderType]
           FROM [CompanyGrossProfitRecordDetail] WITH(NOLOCK) WHERE 1=1 AND State=0 ");
            if (endTime == DateTime.MinValue)
            {
                builder.AppendFormat(" AND OrderTime='{0}' ", startTime);
            }
            else
            {
                builder.AppendFormat(" AND OrderTime>='{0}' AND OrderTime<'{1}'", startTime, endTime);
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
            builder.Append(" GROUP BY [SaleFilialeId],[SalePlatformId],[OrderType]");
            IList<CompanyGrossProfitRecordInfo> dataList = new List<CompanyGrossProfitRecordInfo>();

            using (var connection = Databases.GetSqlConnection(GlobalConfig.ERP_REPORT_DB_NAME, Transaction.Current == null))
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
                            ID = Guid.NewGuid(),
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
                            DayTime = new DateTime(startTime.Year, startTime.Month, startTime.Day)
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
        /// 查询公司毛利中超过一个自然月或一个自然月以上未完成的数据（例如：当前是7月份，订单时间是5月份的订单在7月1号之前没有完成的数据）
        /// </summary>
        /// <param name="dayTime">完成时间</param>
        /// <returns></returns>
        public IList<CompanyGrossProfitRecordInfo> GetCompanyGrossProfitDetailInfosForMoreMonth(DateTime dayTime)
        {
            var builder = new StringBuilder(@"
           SELECT 
           [SaleFilialeId]
          ,[SalePlatformId]
          ,SUM([SalesAmount]) AS [SalesAmount]
          ,SUM([GoodsAmount]) AS [GoodsAmount]
          ,SUM([ShipmentIncome]) AS [ShipmentIncome]
          ,SUM([PromotionsDeductible]) AS [PromotionsDeductible]
          ,SUM([PointsDeduction]) AS [PointsDeduction]
          ,SUM([ShipmentCost]) AS [ShipmentCost]
          ,SUM([PurchaseCosts]) AS [PurchaseCosts]
          ,SUM([CatCommission]) AS [CatCommission]
          ,[OrderType]
          ,CONVERT(VARCHAR(7),[OrderTime],120)+'-01' AS [OrderTime]
           FROM [CompanyGrossProfitRecordDetail] WITH(NOLOCK) WHERE State=0 AND DayTime>=dateadd(month,2,CONVERT(VARCHAR(7),OrderTime,120)+'-01') ");
            builder.AppendFormat(" AND DayTime<'{0}' ", dayTime);
            builder.Append(" GROUP BY [SaleFilialeId],[SalePlatformId],[OrderType],CONVERT(VARCHAR(7),[OrderTime],120)+'-01'");
            IList<CompanyGrossProfitRecordInfo> dataList = new List<CompanyGrossProfitRecordInfo>();
            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_REPORT_DB_NAME, true, builder.ToString()))
            {
                if (rdr != null)
                {
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
                            OrderType = SqlRead.GetInt(rdr, "OrderType"),
                            DayTime = SqlRead.GetDateTime(rdr, "OrderTime")
                        });
                    }
                }
            }
            return dataList;
        }

        /// <summary>
        /// 汇总同一公司不同平台的数据
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="saleFilialeId">销售公司</param>
        /// <param name="salePlatformIds">销售平台</param>
        /// <param name="orderTypes">订单类型(0:网络发货订单;1:门店采购订单;2:帮门店发货订单;)</param>
        /// <returns></returns>
        /// zal 2017-07-17
        public IList<CompanyGrossProfitRecordInfo> SumCompanyGrossProfitBySaleFilialeId(DateTime startTime, DateTime endTime, Guid saleFilialeId, string salePlatformIds, string orderTypes)
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
             FROM [CompanyGrossProfitRecordDetail] WITH(NOLOCK) WHERE 1=1 AND State=0 ");
            if (endTime == DateTime.MinValue)
            {
                builder.AppendFormat(" AND OrderTime='{0}' ", startTime);
            }
            else
            {
                builder.AppendFormat(" AND OrderTime>='{0}' AND OrderTime<'{1}'", startTime, endTime);
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

            using (var connection = Databases.GetSqlConnection(GlobalConfig.ERP_REPORT_DB_NAME, Transaction.Current == null))
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
                            OrderType = SqlRead.GetInt(rdr, "OrderType"),
                            DayTime = new DateTime(startTime.Year, startTime.Month, startTime.Day)
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
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="saleFilialeId">销售公司</param>
        /// <param name="salePlatformIds">销售平台</param>
        /// <param name="orderTypes">订单类型(0:网络发货订单;1:门店采购订单;2:帮门店发货订单;)</param>
        /// <returns></returns>
        /// zal 2017-07-17
        public IList<CompanyGrossProfitRecordInfo> SumCompanyGrossProfitBySaleFilialeIdAndOrderType(DateTime startTime, DateTime endTime, Guid saleFilialeId, string salePlatformIds, string orderTypes)
        {
            string salePlatformIdStr = string.Empty, orderTypeStr = string.Empty;
            var parameter = new StringBuilder();
            if (endTime == DateTime.MinValue)
            {
                parameter.AppendFormat(" AND OrderTime='{0}' ", startTime);
            }
            else
            {
                parameter.AppendFormat(" AND OrderTime>='{0}' AND OrderTime<'{1}'", startTime, endTime);
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
             FROM [CompanyGrossProfitRecordDetail] WITH(NOLOCK) 
             WHERE 1=1 AND State=0 AND [OrderType] IN(1,2)");
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
            FROM[CompanyGrossProfitRecordDetail] WITH(NOLOCK) 
            WHERE 1=1 AND State = 0 AND [OrderType]=0");
            builder.Append(parameter);
            builder.Append(" GROUP BY [SaleFilialeId],[SalePlatformId],[OrderType]");

            IList<CompanyGrossProfitRecordInfo> dataList = new List<CompanyGrossProfitRecordInfo>();

            using (var connection = Databases.GetSqlConnection(GlobalConfig.ERP_REPORT_DB_NAME, Transaction.Current == null))
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
                            OrderType = SqlRead.GetInt(rdr, "OrderType"),
                            DayTime = new DateTime(startTime.Year, startTime.Month, startTime.Day)
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
