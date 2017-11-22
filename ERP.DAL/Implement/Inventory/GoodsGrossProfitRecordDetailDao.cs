using ERP.DAL.Interface.IInventory;
using ERP.Model.Report;
using Keede.DAL.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using ERP.Environment;

namespace ERP.DAL.Implement.Inventory
{
    /// <summary>
    /// 商品毛利明细
    /// zal 2016-07-04
    /// </summary>
    public class GoodsGrossProfitRecordDetailDao : IGoodsGrossProfitRecordDetail
    {
        /// <summary>
        /// 是否存在商品毛利数据
        /// </summary>
        /// <param name="dayTime"></param>
        /// <returns></returns>
        public bool Exists(DateTime dayTime)
        {
            const string SQL = @"SELECT COUNT(0) FROM GoodsGrossProfitRecordDetail WITH(NOLOCK) WHERE DayTime=@DayTime";
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
        /// 批量插入商品毛利记录明细
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public bool AddDataDetail(IList<GoodsGrossProfitRecordDetailInfo> list)
        {
            var dics = new Dictionary<string, string>
                {
                    {"ID","ID"},{"GoodsId","GoodsId"},{"GoodsType","GoodsType"},{"Quantity","Quantity"},{"SaleFilialeId","SaleFilialeId"},
                    {"SalePlatformId","SalePlatformId"},{"PurchaseCostTotal","PurchaseCostTotal"},{"SalesPriceTotal","SalesPriceTotal"},
                    {"OrderType","OrderType"},{"OrderTime","OrderTime"},{"DayTime","DayTime"},{"State","State"}
                };
            return SqlHelper.BatchInsert(GlobalConfig.ERP_REPORT_DB_NAME, list, "GoodsGrossProfitRecordDetail", dics) > 0;
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
            UPDATE GoodsGrossProfitRecordDetail
            SET [State]=1
            WHERE [State]=0 AND OrderTime>='{0}' AND OrderTime<'{1}'
            ";
            sql = string.Format(sql, startTime, endTime);

            try
            {
                return SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_REPORT_DB_NAME, false, sql) > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("商品毛利存档-修改商品毛利明细异常", ex);
            }
        }

        /// <summary>
        /// 根据条件合计商品毛利明细表数据
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="goodsTypes">商品类型</param>
        /// <param name="saleFilialeId">销售公司</param>
        /// <param name="salePlatformIds">销售平台</param>
        /// <param name="orderTypes">订单类型(0:网络发货订单;1:门店采购订单;2:帮门店发货订单;)</param>
        /// <returns></returns>
        public IList<GoodsGrossProfitInfo> SumGoodsGrossProfitRecordDetailInfos(DateTime startTime, DateTime endTime, string goodsTypes, Guid saleFilialeId, string salePlatformIds, string orderTypes)
        {
            string goodsTypeStr = string.Empty, salePlatformIdStr = string.Empty, orderTypeStr = string.Empty;
            var builder = new StringBuilder(@"
              SELECT 
              [GoodsId]
             ,[GoodsType]
             ,SUM([Quantity]) AS Quantity
             ,[SaleFilialeId]
             ,[SalePlatformId]
             ,SUM([SalesPriceTotal]) AS SalesPriceTotal
             ,SUM([PurchaseCostTotal]) AS PurchaseCostTotal
             ,[OrderType]
              FROM [GoodsGrossProfitRecordDetail] WITH(NOLOCK) WHERE 1=1 AND State=0 ");
            if (endTime == DateTime.MinValue)
            {
                builder.AppendFormat(" AND OrderTime='{0}' ", startTime);
            }
            else
            {
                builder.AppendFormat(" AND OrderTime>='{0}' AND OrderTime<'{1}'", startTime, endTime);
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
            builder.Append(" GROUP BY [GoodsId],[GoodsType],[SaleFilialeId],[SalePlatformId],[OrderType]");
            IList<GoodsGrossProfitInfo> dataList = new List<GoodsGrossProfitInfo>();
            IDataReader rdr = null;
            try
            {
                rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_REPORT_DB_NAME, true, builder.ToString());
                while (rdr.Read())
                {
                    dataList.Add(new GoodsGrossProfitInfo
                    {
                        ID = Guid.NewGuid(),
                        GoodsId = SqlRead.GetGuid(rdr, "GoodsId"),
                        GoodsType = SqlRead.GetInt(rdr, "GoodsType"),
                        Quantity = SqlRead.GetInt(rdr, "Quantity"),
                        SaleFilialeId = SqlRead.GetGuid(rdr, "SaleFilialeId"),
                        SalePlatformId = SqlRead.GetGuid(rdr, "SalePlatformId"),
                        SalesPriceTotal = SqlRead.GetDecimal(rdr, "SalesPriceTotal"),
                        PurchaseCostTotal = SqlRead.GetDecimal(rdr, "PurchaseCostTotal"),
                        OrderType = SqlRead.GetInt(rdr, "OrderType"),
                        DayTime = new DateTime(startTime.Year, startTime.Month, startTime.Day)
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
        /// 查询商品毛利中超过一个自然月或一个自然月以上未完成的数据（例如：当前是7月份，订单时间是5月份的订单在7月1号之前没有完成的数据）
        /// </summary>
        /// <param name="dayTime">完成时间</param>
        /// <returns></returns>
        public IList<GoodsGrossProfitInfo> GetGoodsGrossProfitRecordDetailInfosForMoreMonth(DateTime dayTime)
        {
            var builder = new StringBuilder(@"
            SELECT GoodsId,GoodsType,SUM(Quantity) AS Quantity,SaleFilialeId,SalePlatformId,SUM(SalesPriceTotal) AS SalesPriceTotal,SUM(PurchaseCostTotal) AS PurchaseCostTotal,OrderType,(CONVERT(VARCHAR(7),OrderTime,120)+'-01') AS OrderTime
            FROM GoodsGrossProfitRecordDetail WITH(NOLOCK) WHERE State=0 AND DayTime>=dateadd(month,2,CONVERT(VARCHAR(7),OrderTime,120)+'-01') ");
            builder.AppendFormat(" AND DayTime<'{0}' ", dayTime);
            builder.Append(" GROUP BY [GoodsId],[GoodsType],[SaleFilialeId],[SalePlatformId],[OrderType],(CONVERT(VARCHAR(7),OrderTime,120)+'-01')");

            IList<GoodsGrossProfitInfo> dataList = new List<GoodsGrossProfitInfo>();
            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_REPORT_DB_NAME, true, builder.ToString()))
            {
                if (rdr != null)
                {
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
                            OrderType = SqlRead.GetInt(rdr, "OrderType"),
                            DayTime = SqlRead.GetDateTime(rdr, "OrderTime")
                        });
                    }
                }
            }
            return dataList;
        }

        /// <summary>
        /// 汇总同一商品同一公司不同平台的数据
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="goodsTypes">商品类型</param>
        /// <param name="saleFilialeId">销售公司</param>
        /// <param name="salePlatformIds">销售平台</param>
        /// <param name="orderTypes">订单类型(0:网络发货订单;1:门店采购订单;2:帮门店发货订单;)</param>
        /// <returns></returns>
        /// zal 2017-07-18
        public IList<GoodsGrossProfitInfo> SumGoodsGrossProfitByGoodsIdAndSaleFilialeId(DateTime startTime, DateTime endTime, string goodsTypes, Guid saleFilialeId, string salePlatformIds, string orderTypes)
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
              FROM [GoodsGrossProfitRecordDetail] WITH(NOLOCK) WHERE 1=1 AND State=0 ");
            if (endTime == DateTime.MinValue)
            {
                builder.AppendFormat(" AND OrderTime='{0}' ", startTime);
            }
            else
            {
                builder.AppendFormat(" AND OrderTime>='{0}' AND OrderTime<'{1}'", startTime, endTime);
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
                    if (rdr != null)
                        rdr.Close();
                }

            return dataList;
        }
    }
}
