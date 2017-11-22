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
    public class CompanyGrossProfitRecordDetailChildDao : ICompanyGrossProfitRecordDetailChild
    {
        /// <summary>
        /// 获取指定时间内的订单或库存明细商品
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="saleFilialeId">销售公司</param>
        /// <param name="salePlatformIds">销售平台</param>
        /// <param name="orderTypes">订单类型(0:网络发货订单;1:门店采购订单;2:帮门店发货订单;)</param>
        /// <param name="type">0:完成时间超过一个自然月或一个自然月以上的数据;1:完成时间未超过一个自然月或一个自然月以上的数据</param>
        /// <returns></returns>
        /// zal 2016-07-20
        public IList<CompanyGrossProfitRecordDetailChildInfo> GetCompanyGrossProfitRecordDetailChild(DateTime startTime, DateTime endTime, Guid saleFilialeId, string salePlatformIds, string orderTypes, int type)
        {
            IList<CompanyGrossProfitRecordDetailChildInfo> companyGrossProfitRecordDetailChildList = new List<CompanyGrossProfitRecordDetailChildInfo>();
            string salePlatformIdStr = string.Empty, orderTypeStr = string.Empty;
            StringBuilder builder = new StringBuilder();
            builder.Append(@"
                     SELECT A.[GoodsId],SUM(A.[Quantity]) AS Quantity" + (type.Equals(1) ? "" : ",B.[OrderTime]") + @" FROM [CompanyGrossProfitRecordDetailChild] A WITH(NOLOCK)
                     INNER JOIN [CompanyGrossProfitRecordDetail] B WITH(NOLOCK) ON A.[StockAndOrderId]=B.[StockAndOrderId]
                     AND B.State=0
                     ");

            if (type.Equals(1))
            {
                if (startTime != DateTime.MinValue && endTime != DateTime.MinValue)
                {
                    builder.AppendFormat(" AND B.OrderTime BETWEEN '{0}' AND '{1}' ", startTime, endTime);
                }
            }
            else
            {
                if (startTime != DateTime.MinValue)
                {
                    builder.AppendFormat(" AND CONVERT(VARCHAR(7),B.OrderTime,120)+'-01'='{0}' ", startTime);
                }
            }

            if (!string.IsNullOrEmpty(salePlatformIds))
            {
                salePlatformIdStr = salePlatformIds.Split(',').Aggregate(salePlatformIdStr, (current, item) => current + string.Format(",'{0}'", item)).Substring(1);
                builder.AppendFormat(" AND B.[SalePlatformId] IN({0}) ", salePlatformIdStr);
            }
            if (!string.IsNullOrEmpty(orderTypes))
            {
                orderTypeStr = orderTypes.Split(',').Aggregate(orderTypeStr, (current, item) => current + string.Format(",'{0}'", item)).Substring(1);
                builder.AppendFormat(" AND B.[OrderType] IN({0}) ", orderTypeStr);
            }

            if (saleFilialeId != Guid.Empty)
            {
                builder.AppendFormat(" AND B.[SaleFilialeId] = '{0}' ", saleFilialeId);
            }
            builder.Append(" Group BY A.[GoodsId]" + (type.Equals(1) ? "" : ",B.[OrderTime]"));

            using (var connection = Databases.GetSqlConnection(GlobalConfig.ERP_REPORT_DB_NAME, Transaction.Current == null))
            {
                IDataReader rdr = null;
                try
                {
                    connection.Open();
                    var command = new SqlCommand(string.Format(builder.ToString(), startTime, endTime), connection) { CommandTimeout = 600 };
                    rdr = command.ExecuteReader();
                    while (rdr.Read())
                    {
                        var goodsOrderDetailInfo = new CompanyGrossProfitRecordDetailChildInfo
                        {
                            GoodsId = rdr["GoodsId"] == DBNull.Value ? Guid.Empty : new Guid(rdr["GoodsId"].ToString()),
                            Quantity = rdr["Quantity"] == DBNull.Value ? 0 : int.Parse(rdr["Quantity"].ToString()),
                            OrderTime = type.Equals(1) ? Convert.ToDateTime("1900-01-01") : rdr["OrderTime"] == DBNull.Value ? Convert.ToDateTime("1900-01-01") : DateTime.Parse(rdr["OrderTime"].ToString())
                        };
                        companyGrossProfitRecordDetailChildList.Add(goodsOrderDetailInfo);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("获取订单或者库存明细商品数据异常", ex);
                }
                finally
                {
                    connection.Close();
                    if (rdr != null)
                        rdr.Close();
                }
            }
            return companyGrossProfitRecordDetailChildList;
        }

        /// <summary>
        /// 批量插入公司毛利记录明细子表
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public bool AddDataDetailChild(IList<CompanyGrossProfitRecordDetailChildInfo> list)
        {
            var dics = new Dictionary<string, string>
                {
                    {"ID","ID"},{"StockAndOrderId","StockAndOrderId"},{"GoodsId","GoodsId"},{"Quantity","Quantity"}
                };
            return SqlHelper.BatchInsert(GlobalConfig.ERP_REPORT_DB_NAME, list, "CompanyGrossProfitRecordDetailChild", dics) > 0;
        }

        /// <summary>
        /// 根据订单id或者出入库id获取订单或库存明细商品
        /// </summary>
        /// <param name="stockAndOrderIds"></param>
        /// <returns></returns>
        /// zal 2016-07-20
        public IList<CompanyGrossProfitRecordDetailChildInfo> GetCompanyGrossProfitRecordDetailChildByStockAndOrderIds(List<Guid> stockAndOrderIds)
        {
            IList<CompanyGrossProfitRecordDetailChildInfo> companyGrossProfitRecordDetailChildList = new List<CompanyGrossProfitRecordDetailChildInfo>();
            StringBuilder builder = new StringBuilder();
            builder.Append(@"
                     SELECT A.[GoodsId],A.[Quantity] FROM [CompanyGrossProfitRecordDetailChild] A WITH(NOLOCK)
                     INNER JOIN [CompanyGrossProfitRecordDetail] B WITH(NOLOCK) ON A.[StockAndOrderId]=B.[StockAndOrderId]
                     AND B.State=0
                     ");

            string stockAndOrderIdsStr = "'" + string.Join(",", stockAndOrderIds.ToArray()) + "'";
            builder.Append((string.IsNullOrWhiteSpace(stockAndOrderIdsStr) ? "" : string.Format(" AND EXISTS(select id as StockAndOrderId from splitToTable({0},',') #temp WHERE #temp.id=B.[StockAndOrderId]) ", stockAndOrderIdsStr)));


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
                        var goodsOrderDetailInfo = new CompanyGrossProfitRecordDetailChildInfo
                        {
                            GoodsId = rdr["GoodsId"] == DBNull.Value ? Guid.Empty : new Guid(rdr["GoodsId"].ToString()),
                            Quantity = rdr["Quantity"] == DBNull.Value ? 0 : int.Parse(rdr["Quantity"].ToString())
                        };
                        companyGrossProfitRecordDetailChildList.Add(goodsOrderDetailInfo);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("获取订单或者库存明细商品数据异常", ex);
                }
                finally
                {
                    connection.Close();
                    if (rdr != null)
                        rdr.Close();
                }
            }
            return companyGrossProfitRecordDetailChildList;
        }

    }
}
