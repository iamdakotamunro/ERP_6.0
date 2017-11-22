using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using ERP.Model;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Environment;
using Keede.DAL.Helper;
using Keede.Ecsoft.Model;
namespace ERP.DAL.Implement.Inventory
{
    /// <summary>
    /// 库存预警类
    /// </summary>
    public class StockWarning : IStockWarning
    {
        public StockWarning(GlobalConfig.DB.FromType fromType)
        {

        }

        private const string PARM_GOODSID = "@GoodsId";
        private const string PARM_WAREHOUSEID = "@WarehouseId";

        /// <summary>
        /// 获取指定时间范围内指定公司某产品下具体产品的库存预警信息
        /// 最后修改时间:2010/11/8 17:43 by wh
        /// </summary>
        /// <param name="warehouseId"></param>
        /// <param name="hostingFilialeId"></param>
        /// <param name="realGoodsList">子商品集合</param>
        /// <param name="days"> </param>
        /// <returns></returns>
        public IList<StockWarningInfo> GetStockWarningList(Guid warehouseId,Guid hostingFilialeId, List<Guid> realGoodsList, int days)
        {
            IList<StockWarningInfo> stockWarningList = new List<StockWarningInfo>();
            if (realGoodsList.Count > 0)
            {
                var strbGoodsIds = new StringBuilder(string.Empty);
                for (int i = 0; i < realGoodsList.Count; i++)
                {
                    if (i == 0)
                    {
                        strbGoodsIds.Append(realGoodsList[i]);
                    }
                    else
                    {
                        strbGoodsIds.Append((i + 1) % 100 == 0 ? "&" : ",");
                        strbGoodsIds.Append(realGoodsList[i]);
                    }
                }

                var parms = new[] {
                    new SqlParameter(PARM_WAREHOUSEID,SqlDbType.UniqueIdentifier),
                    new SqlParameter(PARM_GOODSID, SqlDbType.VarChar),
                    new SqlParameter("@FilialeId",SqlDbType.UniqueIdentifier), 
                };
                parms[0].Value = warehouseId;
                parms[1].Value = strbGoodsIds.ToString();
                parms[2].Value = hostingFilialeId;

                using (var rdr = SqlHelper.ExecuteReaderSP(GlobalConfig.ERP_DB_NAME, false, CommandType.StoredProcedure, "P_StockStatistics", parms))
                {
                    while (rdr.Read())
                    {
                        var stockWarningInfo = new StockWarningInfo
                        {
                            GoodsId = rdr.GetGuid(0),
                            FirstNumberThreeStockUpSale = Convert.ToInt32(rdr[1]),
                            FirstNumberTwoStockUpSale = Convert.ToInt32(rdr[2]),
                            FirstNumberOneStockUpSale = Convert.ToInt32(rdr[3]),
                            SubtractPurchasingQuantity = Convert.ToInt32(rdr[4]),
                            StockDay = days
                        };
                        stockWarningList.Add(stockWarningInfo);
                    }
                }
            }
            return stockWarningList;
        }

        public IList<GoodsDaySalesInfo> GetGoodsDaySalesInfos(List<Guid> realGoodsIdList, Guid warehouseId,DateTime startTime, DateTime endTime,Guid hostingFilialeId)
        {
            if (!realGoodsIdList.Any())
            {
                return new List<GoodsDaySalesInfo>();
            }
            var sql = string.Format(@"SELECT 
	RealGoodsId,DeliverWarehouseId AS WarehouseId,CONVERT(DATE,DayTime) as DayTime,SUM(GoodsSales) as GoodsSales,ISNULL(Specification,'-') AS Specification
FROM [lmshop_GoodsDaySalesStatistics]
WHERE 
	DayTime >= @StartTime AND DayTime < @EndTime
	AND RealGoodsId IN ($RealGoodsIds$) 
	AND DeliverWarehouseId=@WarehouseId {0}
GROUP BY DeliverWarehouseId,RealGoodsID,CONVERT(DATE,DayTime),Specification
ORDER BY SUM(GoodsSales) DESC", hostingFilialeId == Guid.Empty?"": " AND HostingFilialeId=@HostingFilialeId");
            //获取子商品的销售信息
            var strbSql = new StringBuilder(sql);
            var strbRealGoodsIds = new StringBuilder();
            foreach (var realGoodsId in realGoodsIdList)
            {
                if (string.IsNullOrEmpty(strbRealGoodsIds.ToString()))
                    strbRealGoodsIds.Append("'" + realGoodsId + "'");
                else
                    strbRealGoodsIds.Append(",'" + realGoodsId + "'");
            }
            strbSql.Replace("$RealGoodsIds$", strbRealGoodsIds.ToString());
            var parms = hostingFilialeId == Guid.Empty? new[]
                            {
                                new Parameter("@WarehouseId", warehouseId),
                                new Parameter("@StartTime", startTime),
                                new Parameter("@EndTime", endTime)
                            }:
                            new[]
                            {
                                new Parameter("@WarehouseId", warehouseId),
                                new Parameter("@StartTime", startTime),
                                new Parameter("@EndTime", endTime),
                                new Parameter("@HostingFilialeId",hostingFilialeId), 
                            };
            using (var db = DatabaseFactory.Create())
            {
                return db.Select<GoodsDaySalesInfo>(true, strbSql.ToString(), parms).ToList();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="realGoodsIdList"></param>
        /// <param name="warehouseId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="saleFilialeIds"></param>
        /// <returns></returns>
        public IList<GoodsDaySalesInfo> GetGoodsDaySalesInfos(List<Guid> realGoodsIdList, Guid warehouseId,
            DateTime startTime, DateTime endTime, IEnumerable<Guid> saleFilialeIds)
        {
            if (!realGoodsIdList.Any())
            {
                return new List<GoodsDaySalesInfo>();
            }
            var sql = string.Format(@"SELECT 
	RealGoodsId,DeliverWarehouseId AS WarehouseId,CONVERT(DATE,DayTime) as DayTime,SUM(GoodsSales) as GoodsSales,ISNULL(Specification,'-') AS Specification
FROM [lmshop_GoodsDaySalesStatistics]
WHERE 
	DayTime >= @StartTime AND DayTime < @EndTime
	AND RealGoodsId IN ($RealGoodsIds$) 
	AND DeliverWarehouseId=@WarehouseId AND SaleFilialeId IN('{0}') 
GROUP BY DeliverWarehouseId,RealGoodsID,CONVERT(DATE,DayTime),Specification
ORDER BY SUM(GoodsSales) DESC",string.Join("','",saleFilialeIds));
            //获取子商品的销售信息
            var strbSql = new StringBuilder(sql);
            var strbRealGoodsIds = new StringBuilder();
            foreach (var realGoodsId in realGoodsIdList)
            {
                if (string.IsNullOrEmpty(strbRealGoodsIds.ToString()))
                    strbRealGoodsIds.Append("'" + realGoodsId + "'");
                else
                    strbRealGoodsIds.Append(",'" + realGoodsId + "'");
            }
            strbSql.Replace("$RealGoodsIds$", strbRealGoodsIds.ToString());
            var parms = new[]
                {
                    new Parameter("@WarehouseId", warehouseId),
                    new Parameter("@StartTime", startTime),
                    new Parameter("@EndTime", endTime)
                };
            using (var db = DatabaseFactory.Create())
            {
                return db.Select<GoodsDaySalesInfo>(true, strbSql.ToString(), parms).ToList();
            }
        }

        public Dictionary<Guid,int> GetSaleDays(List<Guid> realGoodsIdList, Guid warehouseId, DateTime dateTime, Guid hostingFilialeId)
        {
            var sql = string.Format(@"SELECT 
	RealGoodsId AS [Key],DATEDIFF(DD,MIN(DayTime),@DateTime) AS [Value]
FROM [lmshop_GoodsDaySalesStatistics]
WHERE RealGoodsId IN ($RealGoodsIds$) 
	AND DeliverWarehouseId=@WarehouseId {0}
GROUP BY RealGoodsID ", hostingFilialeId == Guid.Empty ? "" : " AND HostingFilialeId=@HostingFilialeId");

            //获取子商品的销售信息
            var strbSql = new StringBuilder(sql);
            var strbRealGoodsIds = new StringBuilder();
            foreach (var realGoodsId in realGoodsIdList)
            {
                if (string.IsNullOrEmpty(strbRealGoodsIds.ToString()))
                    strbRealGoodsIds.Append("'" + realGoodsId + "'");
                else
                    strbRealGoodsIds.Append(",'" + realGoodsId + "'");
            }
            strbSql.Replace("$RealGoodsIds$", strbRealGoodsIds.ToString());
            var parms = hostingFilialeId == Guid.Empty ? new[]
                            {
                                new Parameter("@WarehouseId", warehouseId),
                                new Parameter("@DateTime", dateTime),
                            } :
                            new[]
                            {
                                new Parameter("@WarehouseId", warehouseId),
                                new Parameter("@DateTime", dateTime),
                                new Parameter("@HostingFilialeId",hostingFilialeId), 
                            };
            using (var db = DatabaseFactory.Create())
            {
                var data = db.Select<KeyAndValue>(true, strbSql.ToString(), parms);
                return data!=null && data.Any()?data.ToDictionary(k=>k.Key,v=>v.Value):new Dictionary<Guid, int>();
            }
        }


        public Dictionary<Guid, int> GetSubtotalQuantity(Guid warehouseId, List<Guid> realGoodsIdList, List<StorageRecordType> storageRecordTypes, List<StorageRecordState> storageRecordStates)
        {
            var sqlStr = new StringBuilder(@"SELECT 
    d.RealGoodsId AS [Key],
    SUM(d.Quantity) AS [Value]
FROM StorageRecord AS s WITH(NOLOCK) 
inner join StorageRecordDetail as d WITH(NOLOCK) on d.StockId = s.StockId WHERE s.WarehouseId='" + warehouseId + @"'");
            if (storageRecordTypes.Any())
            {
                var strStorageRecordTypes = storageRecordTypes.Aggregate(String.Empty, (ent, type) => ent + ((int)type + ","));
                sqlStr.Append(" AND s.StockType IN (").Append(strStorageRecordTypes.Substring(0, strStorageRecordTypes.Length - 1)).Append(")");
            }
            if (storageRecordStates.Any())
            {
                var strStorageRecordState = storageRecordStates.Aggregate(String.Empty, (ent, type) => ent + ((int)type + ","));
                sqlStr.Append(" AND s.StockState IN (").Append(strStorageRecordState.Substring(0, strStorageRecordState.Length - 1)).Append(")");
            }
            if (realGoodsIdList.Any())
            {
                var strbRealGoodsIds = new StringBuilder();
                foreach (var realGoodsId in realGoodsIdList)
                {
                    if (string.IsNullOrEmpty(strbRealGoodsIds.ToString()))
                        strbRealGoodsIds.Append("'" + realGoodsId + "'");
                    else
                        strbRealGoodsIds.Append(",'" + realGoodsId + "'");
                }
                sqlStr.Append("AND RealGoodsId IN ($RealGoodsIds$) ");
                sqlStr.Replace("$RealGoodsIds$", strbRealGoodsIds.ToString());
            }
            sqlStr.Append(" group by d.RealGoodsId");
            using (var db = DatabaseFactory.Create())
            {
                var data = db.Select<KeyAndValue>(true, sqlStr.ToString());
                return data != null && data.Any() ? data.ToDictionary(k => k.Key, v => v.Value) : new Dictionary<Guid, int>();
            }
        }

        public List<StorageLackQuantity> GetSubtotalStorageRecord(Guid warehouseId, Guid realGoodsId,
            List<StorageRecordType> storageRecordTypes, List<StorageRecordState> storageRecordStates)
        {
            var sqlStr = new StringBuilder(@"SELECT d.StockId,s.TradeCode,s.FilialeId,SUM(d.Quantity) AS [Quantity]
FROM StorageRecord AS s WITH(NOLOCK) 
inner join StorageRecordDetail as d WITH(NOLOCK) on d.StockId = s.StockId WHERE s.WarehouseId='{0}' and d.RealGoodsId='{1}'");
            if (storageRecordTypes.Any())
            {
                var strStorageRecordTypes = storageRecordTypes.Aggregate(String.Empty, (ent, type) => ent + ((int)type + ","));
                sqlStr.Append(" AND s.StockType IN (").Append(strStorageRecordTypes.Substring(0, strStorageRecordTypes.Length - 1)).Append(")");
            }
            if (storageRecordStates.Any())
            {
                var strStorageRecordState = storageRecordStates.Aggregate(String.Empty, (ent, type) => ent + ((int)type + ","));
                sqlStr.Append(" AND s.StockState IN (").Append(strStorageRecordState.Substring(0, strStorageRecordState.Length - 1)).Append(")");
            }
            sqlStr.Append(" group by d.StockId,s.TradeCode,s.FilialeId");
            using (var db = DatabaseFactory.Create())
            {
                var data = db.Select<StorageLackQuantity>(true, string.Format(sqlStr.ToString(),warehouseId,realGoodsId));
                return data != null && data.Any() ? data.ToList() : new List<StorageLackQuantity>();
            }
        }
    }
}
