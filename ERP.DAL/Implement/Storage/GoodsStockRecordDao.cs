using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using ERP.DAL.Interface.IStorage;
using ERP.Model;
using ERP.Model.Report;
using ERP.DAL.Interface.IInventory;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IUtilities;
using ERP.DAL.Factory;
using ERP.Environment;
using Keede.DAL.RWSplitting;
using Dapper;
using Keede.DAL.Helper;
using System.Transactions;

namespace ERP.DAL.Implement.Storage
{
    public class GoodsStockRecordDao : IGoodsStockRecord
    {
        readonly IPurchaseSet _purchaseSet = new PurchaseSet(ERP.Environment.GlobalConfig.DB.FromType.Write);
        readonly IUtility _utility = InventoryInstance.GetUtilityDalDao(GlobalConfig.DB.FromType.Write);
        #region
        /// <summary>
        ///   datetime.AddDays(1 - datetime.Day);   获取月份第一天
        /// </summary>
        /// <param name="recordInfo"></param>
        /// <returns></returns>
        public bool InsertSettlePriceRecord(GoodsStockPriceRecordInfo recordInfo)
        {
            const string SQL = @"IF EXISTS(SELECT 1 FROM GoodsStockPriceRecord WHERE GoodsId=@GoodsId AND DayTime=@DayTime)
		BEGIN
			UPDATE GoodsStockPriceRecord SET AvgSettlePrice=@AvgSettlePrice,MonthAvgPrice=@MonthAvgPrice
			 WHERE GoodsId=@GoodsId AND DayTime=@DayTime
		END
	ELSE
		BEGIN
			INSERT INTO GoodsStockPriceRecord(GoodsId,DayTime,AvgSettlePrice,MonthAvgPrice) 
			VALUES(@GoodsId,@DayTime,@AvgSettlePrice,@MonthAvgPrice)
		END";

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_REPORT_DB_NAME, false))
            {
                return conn.Execute(SQL, new
                {
                    GoodsId = recordInfo.GoodsId,
                    DayTime = recordInfo.DayTime,
                    AvgSettlePrice = recordInfo.AvgSettlePrice,
                    MonthAvgPrice = recordInfo.MonthAvgPrice
                }) > 0;
            }
        }

        /// <summary>
        /// 是否存在记录数据
        /// </summary>
        /// <param name="dayTime"></param>
        /// <returns></returns>
        public bool IsExistsSettlePriceRecord(DateTime dayTime)
        {
            const string SQL = "SELECT COUNT(0) FROM GoodsStockPriceRecord WHERE DayTime=@DayTime";

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_REPORT_DB_NAME, false))
            {
                return conn.Execute(SQL, new
                {
                    DayTime = dayTime,
                }) > 0;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="goodsId"></param>
        /// <param name="dayTime"></param>
        /// <returns></returns>
        public GoodsStockPriceRecordInfo SelectStockPriceRecordInfo(Guid goodsId, DateTime dayTime)
        {
            const string SQL = @"SELECT TOP 1 GoodsId,DayTime,AvgSettlePrice,MonthAvgPrice FROM GoodsStockPriceRecord WITH(NOLOCK) WHERE GoodsId=@GoodsId AND DayTime=@DayTime ";

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_REPORT_DB_NAME, Transaction.Current == null))
            {
                return conn.QueryFirstOrDefault<GoodsStockPriceRecordInfo>(SQL, new
                {
                    GoodsId = goodsId,
                    DayTime = dayTime,
                });
            }
        }

        /// <summary>
        /// 获取上月的商品结算价列表
        /// </summary>
        /// <param name="dayTime"></param>
        /// <param name="goodsId"></param>
        /// <returns></returns>
        public IList<GoodsStockPriceRecordInfo> SelectGoodsStockPriceRecordInfos(DateTime? dayTime, Guid? goodsId)
        {
            var builder = new StringBuilder(@"SELECT GoodsId, DayTime,AvgSettlePrice,MonthAvgPrice FROM GoodsStockPriceRecord WITH(NOLOCK) WHERE 1=1 ");
            if (goodsId != null && goodsId.ToString() != Guid.Empty.ToString())
            {
                builder.AppendFormat(" AND GoodsId='{0}' ", goodsId);
            }
            if (dayTime != null)
            {
                builder.AppendFormat(" AND DayTime='{0}' ", dayTime);
            }

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_REPORT_DB_NAME, Transaction.Current == null))
            {
                return conn.Query<GoodsStockPriceRecordInfo>(builder.ToString()).ToList();
            }
        }
        #endregion

        #region    库存金额、结算价
        public bool InsertGoodsStockRecord(MonthGoodsStockRecordInfo recordInfo)
        {
            const string SQL = @"IF EXISTS(SELECT 1 FROM MonthGoodsStockRecord WHERE RealGoodsId=@RealGoodsId AND DayTime=@DayTime
 AND FilialeId=@FilialeId AND WarehouseId=@WarehouseId)
		BEGIN
			UPDATE MonthGoodsStockRecord SET NonceGoodsStock=@NonceGoodsStock,
			GoodsType=@GoodsType WHERE RealGoodsId=@RealGoodsId AND DayTime=@DayTime
 AND FilialeId=@FilialeId AND WarehouseId=@WarehouseId
		END
	ELSE
		BEGIN
			INSERT INTO MonthGoodsStockRecord(ID,GoodsId,RealGoodsId,FilialeId,WarehouseId,NonceGoodsStock,,GoodsType,DayTime) 
			VALUES(NEWID(),@GoodsId,@RealGoodsId,@FilialeId,@WarehouseId,@NonceGoodsStock,,@GoodsType,@DayTime)
		END";

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_REPORT_DB_NAME, false))
            {
                return conn.Execute(SQL, new
                {
                    GoodsId = recordInfo.GoodsId,
                    RealGoodsId = recordInfo.RealGoodsId,
                    FilialeId = recordInfo.FilialeId,
                    WarehouseId = recordInfo.WarehouseId,
                    NonceGoodsStock = recordInfo.NonceGoodsStock,
                    GoodsType = recordInfo.GoodsType,
                    DayTime = new DateTime(recordInfo.DayTime.Year, recordInfo.DayTime.Month, 1),
                }) > 0;
            }
        }

        /// <summary>
        /// 批量添加商品结算价和商品
        /// </summary>
        /// <param name="priceRecordInfos"></param>
        /// <returns></returns>
        public bool BatchInsert(IList<GoodsStockPriceRecordInfo> priceRecordInfos)
        {
            var dics = new Dictionary<string, string>
            {
                {"GoodsId", "GoodsId"},
                {"DayTime", "DayTime"},
                {"AvgSettlePrice", "AvgSettlePrice"},
                {"MonthAvgPrice", "MonthAvgPrice"}
            };
            return SqlHelper.BatchInsert(GlobalConfig.ERP_REPORT_DB_NAME, priceRecordInfos, "GoodsStockPriceRecord", dics) >0;
        }

        /// <summary>
        /// 月末库存备份
        /// </summary>
        /// <param name="stockRecordInfos"></param>
        /// <returns></returns>
        public bool CopyMonthGoodsStockInfos(IList<MonthGoodsStockRecordInfo> stockRecordInfos)
        {
            var dicStocks = new Dictionary<string, string>
                {
                    {"ID","ID"},{"GoodsId","GoodsId"},{"RealGoodsId","RealGoodsId"},{"FilialeId","FilialeId"},{"WarehouseId","WarehouseId"}
                    ,{"NonceGoodsStock","NonceGoodsStock"},{"GoodsType","GoodsType"},{"DayTime","DayTime"},{"DateCreated","DateCreated"}
                };
            return SqlHelper.BatchInsert(GlobalConfig.ERP_REPORT_DB_NAME, stockRecordInfos, "MonthGoodsStockRecord", dicStocks) >0;
        }

        /// <summary>
        /// 是否存在记录数据
        /// </summary>
        /// <param name="dayTime"></param>
        /// <returns></returns>
        public bool IsExistsGoodsStockRecord(DateTime dayTime)
        {
            const string SQL = "SELECT COUNT(0) FROM MonthGoodsStockRecord WHERE DayTime=@DayTime";

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_REPORT_DB_NAME, false))
            {
                return conn.ExecuteScalar<int>(SQL, new
                {
                    DayTime = dayTime,
                }) > 0;
            }
        }

        public MonthGoodsStockRecordInfo SelectMonthGoodsStockRecordInfo(Guid realGoodsId, Guid filialeId, Guid warehouseId,
            DateTime dayTime)
        {
            const string SQL = @"SELECT TOP 1 A.GoodsId,A.RealGoodsId,A.FilialeId,A.WarehouseId,A.NonceGoodsStock,
B.AvgSettlePrice AS SettlePrice,A.GoodsType,A.DayTime,A.DateCreated FROM MonthGoodsStockRecord A
INNER JOIN GoodsStockPriceRecord B ON A.GoodsId=B.GoodsId AND A.DayTime=B.DayTime 
 WHERE A.RealGoodsId=@RealGoodsId AND A.DayTime=@DayTime
 AND A.FilialeId=@FilialeId AND A.WarehouseId=@WarehouseId";

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_REPORT_DB_NAME, Transaction.Current == null))
            {
                return conn.QueryFirstOrDefault<MonthGoodsStockRecordInfo>(SQL, new
                {
                    RealGoodsId = realGoodsId,
                    DayTime = dayTime,
                    FilialeId = filialeId,
                    WarehouseId = warehouseId,
                });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dayTime"></param>
        /// <param name="filialeId"></param>
        /// <param name="warehouseId"></param>
        /// <param name="goodsId"></param>
        /// <param name="goodsType"></param>
        /// <returns></returns>
        public IList<MonthGoodsStockRecordInfo> SelectMonthGoodsStockRecordInfos(DateTime dayTime, Guid? filialeId, Guid? warehouseId, Guid? goodsId, int? goodsType)
        {
            var builder = new StringBuilder(@"SELECT A.GoodsId,A.RealGoodsId,A.FilialeId,A.WarehouseId,A.NonceGoodsStock,
B.AvgSettlePrice AS SettlePrice,A.GoodsType,A.DayTime,A.DateCreated FROM MonthGoodsStockRecord A WITH(NOLOCK) 
INNER JOIN GoodsStockPriceRecord B ON A.GoodsId=B.GoodsId AND A.DayTime=B.DayTime 
 WHERE A.DayTime=@DayTime  ");
            if (filialeId != null && filialeId != Guid.Empty)
            {
                builder.AppendFormat(" AND A.FilialeId='{0}'  ", filialeId);
            }
            if (warehouseId != null && warehouseId != Guid.Empty)
            {
                builder.AppendFormat(" AND A.WarehouseId='{0}'  ", warehouseId);
            }
            if (goodsId != null && goodsId != Guid.Empty)
            {
                builder.AppendFormat(" AND A.GoodsId='{0}'  ", goodsId);
            }
            if (goodsType != null && goodsType >= 0)
            {
                builder.AppendFormat(" AND A.GoodsType={0}  ", goodsType);
            }

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_REPORT_DB_NAME, Transaction.Current == null))
            {
                return conn.Query<MonthGoodsStockRecordInfo>(builder.ToString(), new
                {
                    DayTime = new DateTime(dayTime.Year, dayTime.Month, 1),
                }).AsList();
            }
        }

        /// <summary>
        /// 通过商品类型获取月商品库存记录
        /// </summary>
        /// <param name="goodsType"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public IList<MonthGoodsStockRecordInfo> SelectMonthGoodsStockRecordInfos(int goodsType, int year, int month)
        {
            const string SQL = @"SELECT A.GoodsId,A.NonceGoodsStock,A.DayTime,B.AvgSettlePrice AS SettlePrice FROM (
SELECT GoodsId,SUM(NonceGoodsStock) AS NonceGoodsStock,DayTime 
FROM MonthGoodsStockRecord WITH(NOLOCK) 
WHERE GoodsType=@GoodsType AND DayTime=@DayTime
 GROUP BY GoodsId,DayTime) AS A 
 INNER JOIN GoodsStockPriceRecord B ON A.GoodsId=B.GoodsId AND A.DayTime=B.DayTime ";

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_REPORT_DB_NAME, Transaction.Current == null))
            {
                return conn.Query<MonthGoodsStockRecordInfo>(SQL, new
                {
                    GoodsType = goodsType,
                    DayTime = new DateTime(year, month, 1),
                }).AsList();
            }
        }

        /// <summary>
        /// 库存金额月报
        /// </summary>
        /// <param name="year"></param>
        /// <param name="warehouseId"></param>
        /// <returns></returns>
        public IList<MonthGoodsStockReportInfo> SelectMonthGoodsStockReportInfos(int year, Guid warehouseId)
        {
            string sql = @"
            declare
            @BeginDate  varchar(20),@EndDate   varchar(10)
            set  @BeginDate  = convert(varchar(4),@Year,120)+'-01-01'
            set  @EndDate    = convert(varchar(4),@Year,120)+'-12-31' 
            SELECT GoodsType,@Year AS CurrentYear,  
            ISNULL(sum(case when Month(A.DayTime)=1 then ISNULL(A.NonceGoodsStock*B.AvgSettlePrice,0) ELSE 0 end),0) as January,
            ISNULL(sum(case when Month(A.DayTime)=2 then ISNULL(A.NonceGoodsStock*B.AvgSettlePrice,0) ELSE 0 end),0) as February,
            ISNULL(sum(case when Month(A.DayTime)=3 then ISNULL(A.NonceGoodsStock*B.AvgSettlePrice,0) ELSE 0 end),0) as March,
            ISNULL(sum(case when Month(A.DayTime)=4 then ISNULL(A.NonceGoodsStock*B.AvgSettlePrice,0) ELSE 0 end),0) as April, 
            ISNULL(sum(case when Month(A.DayTime)=5 then ISNULL(A.NonceGoodsStock*B.AvgSettlePrice,0) ELSE 0 end),0) as May, 
            ISNULL(sum(case when Month(A.DayTime)=6 then ISNULL(A.NonceGoodsStock*B.AvgSettlePrice,0) ELSE 0 end),0) as June, 
            ISNULL(sum(case when Month(A.DayTime)=7 then ISNULL(A.NonceGoodsStock*B.AvgSettlePrice,0) ELSE 0 end),0) as July, 
            ISNULL(sum(case when Month(A.DayTime)=8 then ISNULL(A.NonceGoodsStock*B.AvgSettlePrice,0) ELSE 0 end),0) as August, 
            ISNULL(sum(case when Month(A.DayTime)=9 then ISNULL(A.NonceGoodsStock*B.AvgSettlePrice,0) ELSE 0 end),0) as September, 
            ISNULL(sum(case when Month(A.DayTime)=10 then ISNULL(A.NonceGoodsStock*B.AvgSettlePrice,0) ELSE 0 end),0) as October, 
            ISNULL(sum(case when Month(A.DayTime)=11 then ISNULL(A.NonceGoodsStock*B.AvgSettlePrice,0) ELSE 0 end),0) as November, 
            ISNULL(sum(case when Month(A.DayTime)=12 then ISNULL(A.NonceGoodsStock*B.AvgSettlePrice,0) ELSE 0 end),0) as December 
            FROM MonthGoodsStockRecord A WITH(NOLOCK)
            INNER JOIN GoodsStockPriceRecord B 
            ON A.DayTime=B.DayTime AND A.GoodsId=B.GoodsId
            WHERE A.DayTime between  @BeginDate  and @EndDate ";

            if (warehouseId!=Guid.Empty)
            {
                sql = sql + "and A.WarehouseId='" + warehouseId + "'";
            }

            sql = sql + "GROUP BY GoodsType";

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_REPORT_DB_NAME, Transaction.Current == null))
            {
                return conn.Query<MonthGoodsStockReportInfo>(sql, new
                {
                    Year = year,
                }).AsList();
            }
        }
        #endregion

        /// <summary>
        /// 获取商品特定时间下最近的结算价存档
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public Dictionary<Guid, decimal> GetGoodsSettlePriceDicts(DateTime dateTime)
        {
            const string SQL = @"SELECT I1.GoodsId,I2.AvgSettlePrice FROM (
				SELECT GoodsId,Max(DayTime) DayTime 
				FROM GoodsStockPriceRecord with(nolock)
				WHERE DayTime<='{0}' GROUP BY GoodsId) 
		   AS I1
		  INNER JOIN GoodsStockPriceRecord I2 with(nolock)
	      on I1.GoodsId=I2.GoodsId AND I1.DayTime=I2.DayTime";

            string newSql = string.Format(SQL, new DateTime(dateTime.Year, dateTime.Month, 1));
            var dics = new Dictionary<Guid, decimal>();

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_REPORT_DB_NAME, Transaction.Current == null))
            {
                try
                {
                    return conn.Query(newSql).Select(m => new KeyValuePair<Guid, Decimal>((Guid)m.GoodsId, (decimal)m.AvgSettlePrice))
                        .ToDictionary(kv => kv.Key, kv => kv.Value);
                }
                catch (Exception ex)
                {
                    throw new Exception("获取商品最近结算价失败！", ex);
                }
            }
        }

        /// <summary>
        /// 获取商品特定时间下最近的结算价存档，如果最近结算价没有(即表示是新添加的商品)，则取该商品的采购价
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        /// zal 2016-05-19
        public Dictionary<Guid, decimal> GetGoodsSettlePriceOrPurchasePriceDicts(DateTime dateTime)
        {
            var dicSettlePrices = GetGoodsSettlePriceDicts(dateTime);
            var purchasePriceList = _purchaseSet.GetPurchaseSetList();
            foreach (var item in purchasePriceList.Where(item => !dicSettlePrices.ContainsKey(item.GoodsId)))
            {
                dicSettlePrices.Add(item.GoodsId, item.PurchasePrice);
            }
            return dicSettlePrices;
        }
    }
}
