using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ERP.DAL.Interface.IInventory;
using ERP.Model.Report;
using Keede.DAL.Helper;
using System.Data.SqlClient;
using ERP.Environment;

namespace ERP.DAL.Implement.Inventory
{
    public class GoodsGrossProfitDao : IGoodsGrossProfit
    {
        /// <summary>
        /// 是否存在商品毛利数据
        /// </summary>
        /// <param name="dayTime"></param>
        /// <returns></returns>
        public bool Exists(DateTime dayTime)
        {
            const string SQL = @"SELECT COUNT(ID) FROM GoodsGrossProfitRecord WITH(NOLOCK) WHERE DayTime=@DayTime";
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
        /// 判断当前月份数据是否存在
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public bool CurrentIsExist(DateTime startTime, DateTime endTime)
        {
            const string SQL = @"SELECT COUNT(ID) FROM GoodsGrossProfitRecord WITH(NOLOCK) WHERE DayTime BETWEEN '{0}' AND '{1}'";
            using (var db = DatabaseFactory.CreateRdb())
            {
                return db.GetValue<int>(true, string.Format(SQL, startTime, endTime)) > 0;
            }
        }

        /// <summary>
        /// 删除特定时间内的临时数据
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public bool DeleteData(int year, int month)
        {
            const string SQL = @"DELETE GoodsGrossProfitRecord WHERE YEAR(DayTime)={0} AND Month(DayTime)={1} ";
            using (var db = DatabaseFactory.CreateRdb())
            {
                return db.Execute(false, string.Format(SQL, year, month));
            }
        }

        /// <summary>
        /// 批量插入商品毛利记录
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public bool AddData(IList<GoodsGrossProfitInfo> list)
        {
            var dics = new Dictionary<string, string>
                {
                    {"ID","ID"},{"GoodsId","GoodsId"},{"GoodsType","GoodsType"},{"Quantity","Quantity"},{"SaleFilialeId","SaleFilialeId"},
                    {"SalePlatformId","SalePlatformId"},{"PurchaseCostTotal","PurchaseCostTotal"},{"SalesPriceTotal","SalesPriceTotal"},
                    {"OrderType","OrderType"},{"DayTime","DayTime"}
                };
            return SqlHelper.BatchInsert(GlobalConfig.ERP_REPORT_DB_NAME, list, "GoodsGrossProfitRecord", dics) > 0;
        }

        /// <summary>
        /// 修改商品毛利记录(如果已存在则修改，不存在添加)
        /// </summary>
        /// <param name="goodsGrossProfitInfo"></param>
        /// <returns></returns>
        public bool UpdateGoodsGrossProfitInfo(GoodsGrossProfitInfo goodsGrossProfitInfo)
        {
            string sql = @"
    IF EXISTS(SELECT COUNT(0) FROM GoodsGrossProfitRecord WHERE GoodsId=@GoodsId AND SaleFilialeId =@SaleFilialeId AND SalePlatformId =@SalePlatformId AND OrderType =@OrderType AND DayTime =@DayTime)
	BEGIN
	
        BEGIN TRY	
            BEGIN TRANSACTION         
			
            UPDATE GoodsGrossProfitRecord
            SET SalesPriceTotal=SalesPriceTotal+@SalesPriceTotal,PurchaseCostTotal=PurchaseCostTotal+@PurchaseCostTotal,Quantity=Quantity+@Quantity
            WHERE GoodsId=@GoodsId AND SaleFilialeId =@SaleFilialeId AND SalePlatformId =@SalePlatformId AND OrderType =@OrderType AND DayTime =@DayTime
            
            UPDATE GoodsGrossProfitRecordDetail
            SET [State]=1
            WHERE [State]=0 AND GoodsId=@GoodsId AND SaleFilialeId =@SaleFilialeId AND SalePlatformId =@SalePlatformId AND OrderType =@OrderType AND CONVERT(VARCHAR(7),OrderTime,120)+'-01' =@DayTime 
            
            COMMIT TRANSACTION
        END TRY
	    BEGIN CATCH
		    ROLLBACK TRANSACTION
	    END CATCH

	END
	ELSE
	BEGIN
		INSERT INTO  GoodsGrossProfitRecord (
		    [ID]
           ,[GoodsId]
           ,[GoodsType]
           ,[Quantity]
           ,[SaleFilialeId]
           ,[SalePlatformId]
           ,[SalesPriceTotal]
           ,[PurchaseCostTotal]
           ,[OrderType]
           ,[DayTime])
		VALUES(
		    NEWID()
           ,@GoodsId
           ,@GoodsType
           ,@Quantity
           ,@SaleFilialeId
           ,@SalePlatformId
           ,@SalesPriceTotal
           ,@PurchaseCostTotal
           ,@OrderType
           ,@DayTime)
	END";

            try
            {
                var sqlparams = new[]{
                    new SqlParameter("@SalesPriceTotal", goodsGrossProfitInfo.SalesPriceTotal),
                    new SqlParameter("@PurchaseCostTotal", goodsGrossProfitInfo.PurchaseCostTotal),
                    new SqlParameter("@Quantity", goodsGrossProfitInfo.Quantity),
                    new SqlParameter("@GoodsId", goodsGrossProfitInfo.GoodsId),
                    new SqlParameter("@GoodsType", goodsGrossProfitInfo.GoodsType),
                    new SqlParameter("@SaleFilialeId", goodsGrossProfitInfo.SaleFilialeId),
                    new SqlParameter("@SalePlatformId", goodsGrossProfitInfo.SalePlatformId),
                    new SqlParameter("@OrderType", goodsGrossProfitInfo.OrderType),
                    new SqlParameter("@DayTime", goodsGrossProfitInfo.DayTime.ToString("yyyy-MM-01"))
                };
                return SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_REPORT_DB_NAME, false, sql, sqlparams) > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("商品毛利存档-处理完成时间超过一个自然月或一个自然月以上的数据异常", ex);
            }
        }

        /// <summary>
        /// 查询历史月份商品毛利信息
        /// </summary>
        /// <param name="startTime">记录年月</param>
        /// <param name="endTime"></param>
        /// <param name="goodsTypes">商品类型</param>
        /// <param name="saleFilialeId">销售公司</param>
        /// <param name="salePlatformIds">销售平台</param>
        /// <param name="orderTypes">订单类型(0:网络发货订单;1:门店采购订单;2:帮门店发货订单;)</param>
        /// <returns></returns>
        public IList<GoodsGrossProfitInfo> SelectGoodsGrossProfitInfos(DateTime startTime, DateTime? endTime, string goodsTypes, Guid saleFilialeId, string salePlatformIds, string orderTypes)
        {
            string goodsTypeStr = string.Empty, salePlatformIdStr = string.Empty, orderTypeStr = string.Empty;
            var builder = new StringBuilder(@"
             SELECT [ID]
              ,[GoodsId]
              ,[GoodsType]
              ,[Quantity]
              ,[SaleFilialeId]
              ,[SalePlatformId]
              ,[SalesPriceTotal]
              ,[PurchaseCostTotal]
              ,[OrderType]
              ,[DayTime]  
             FROM GoodsGrossProfitRecord WITH(NOLOCK) WHERE 1=1 ");
            if (endTime == null || endTime == DateTime.MinValue)
            {
                builder.AppendFormat(" AND DayTime='{0}' ", startTime);
            }
            else
            {
                builder.AppendFormat(" AND DayTime>='{0}' AND DayTime<'{1}'", startTime, endTime);
            }

            if (!string.IsNullOrEmpty(goodsTypes))
            {
                goodsTypeStr = goodsTypes.Split(',').Aggregate(goodsTypeStr, (current, item) => current + string.Format(",{0}", item)).Substring(1);
                builder.AppendFormat(" AND GoodsType IN({0}) ", goodsTypeStr);
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

            IList<GoodsGrossProfitInfo> dataList = new List<GoodsGrossProfitInfo>();

                IDataReader rdr = null;
                try
                {
                    rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_REPORT_DB_NAME, true, builder.ToString());
                    while (rdr.Read())
                    {
                        dataList.Add(new GoodsGrossProfitInfo
                        {
                            ID = SqlRead.GetGuid(rdr, "ID"),
                            GoodsId = SqlRead.GetGuid(rdr, "GoodsId"),
                            GoodsType = SqlRead.GetInt(rdr, "GoodsType"),
                            Quantity = SqlRead.GetInt(rdr, "Quantity"),
                            SaleFilialeId = SqlRead.GetGuid(rdr, "SaleFilialeId"),
                            SalePlatformId = SqlRead.GetGuid(rdr, "SalePlatformId"),
                            SalesPriceTotal = SqlRead.GetDecimal(rdr, "SalesPriceTotal"),
                            PurchaseCostTotal = SqlRead.GetDecimal(rdr, "PurchaseCostTotal"),
                            OrderType = SqlRead.GetInt(rdr, "OrderType"),
                            DayTime = SqlRead.GetDateTime(rdr, "DayTime")
                        });
                    }
                }
                finally
                {
                    if (rdr != null)
                        rdr.Close();
                }

            return dataList;
        }

        /// <summary>
        /// 汇总同一商品同一公司不同平台的数据
        /// </summary>
        /// <param name="startTime">记录年月</param>
        /// <param name="endTime"></param>
        /// <param name="goodsTypes">商品类型</param>
        /// <param name="saleFilialeId">销售公司</param>
        /// <param name="salePlatformIds">销售平台</param>
        /// <param name="orderTypes">订单类型(0:网络发货订单;1:门店采购订单;2:帮门店发货订单;)</param>
        /// <returns></returns>
        /// zal 2017-07-18
        public IList<GoodsGrossProfitInfo> SumGoodsGrossProfitFromMonthByGoodsIdAndSaleFilialeId(DateTime startTime, DateTime? endTime, string goodsTypes, Guid saleFilialeId, string salePlatformIds, string orderTypes)
        {
            string goodsTypeStr = string.Empty, salePlatformIdStr = string.Empty, orderTypeStr = string.Empty;
            var builder = new StringBuilder(@"
              SELECT 
              [GoodsId]
             ,[GoodsType]
             ,SUM([Quantity]) AS Quantity
             ,[SaleFilialeId]
             ,'00000000-0000-0000-0000-000000000000' AS [SalePlatformId]
             ,SUM([SalesPriceTotal]) AS SalesPriceTotal
             ,SUM([PurchaseCostTotal]) AS PurchaseCostTotal
             ,'-1' AS [OrderType]
              FROM [GoodsGrossProfitRecord] WITH(NOLOCK) WHERE 1=1 ");
            if (endTime == null || endTime == DateTime.MinValue)
            {
                builder.AppendFormat(" AND DayTime='{0}' ", startTime);
            }
            else
            {
                builder.AppendFormat(" AND DayTime>='{0}' AND DayTime<'{1}'", startTime, endTime);
            }

            if (!string.IsNullOrEmpty(goodsTypes))
            {
                goodsTypeStr = goodsTypes.Split(',').Aggregate(goodsTypeStr, (current, item) => current + string.Format(",{0}", item)).Substring(1);
                builder.AppendFormat(" AND GoodsType IN({0}) ", goodsTypeStr);
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
            builder.Append(" GROUP BY [GoodsId],[GoodsType],[SaleFilialeId]");
            IList<GoodsGrossProfitInfo> dataList = new List<GoodsGrossProfitInfo>();

                IDataReader rdr = null;
                try
                {
                    rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_REPORT_DB_NAME, true, builder.ToString());
                    while (rdr.Read())
                    {
                        dataList.Add(new GoodsGrossProfitInfo
                        {
                            GoodsId = SqlRead.GetGuid(rdr, "GoodsId"),
                            GoodsType = SqlRead.GetInt(rdr, "GoodsType"),
                            Quantity = SqlRead.GetInt(rdr, "Quantity"),
                            SaleFilialeId = SqlRead.GetGuid(rdr, "SaleFilialeId"),
                            SalePlatformId = SqlRead.GetGuid(rdr, "SalePlatformId"),
                            SalesPriceTotal = SqlRead.GetDecimal(rdr, "SalesPriceTotal"),
                            PurchaseCostTotal = SqlRead.GetDecimal(rdr, "PurchaseCostTotal"),
                            OrderType = SqlRead.GetInt(rdr, "OrderType")
                        });
                    }
                }
                finally
                {
                    rdr?.Close();
                }

            return dataList;
        }
    }
}
