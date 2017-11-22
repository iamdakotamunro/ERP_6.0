using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Transactions;
using AllianceShop.Common.Extension;
using Dapper;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Environment;
using ERP.Model;
using ERP.Model.Finance;
using ERP.Model.Goods;
using ERP.Model.Report;
using Keede.DAL.Helper;
using Keede.DAL.RWSplitting;
using Keede.Ecsoft.Model.ShopFront;


namespace ERP.DAL.Implement.Inventory
{
    /// <summary>▄︻┻┳═一  商品出入库记录数据层  最后修改提交 陈重文  2014-12-25   （更新、删除、优化方法）
    /// </summary>
    public class StorageRecordDao : IStorageRecordDao
    {
        static Dictionary<string, string> _storageRecordDetailMappings = new Dictionary<string, string>
        {
            { "StockId","StockId"},
            { "RealGoodsId","RealGoodsId"},
            { "GoodsName","GoodsName"},
            { "GoodsCode","GoodsCode"},
            { "UnitPrice","UnitPrice"},
            { "Specification","Specification"},
            { "Quantity","Quantity"},
            { "NonceWarehouseGoodsStock","NonceWarehouseGoodsStock"},
            { "Description","Description"},
            { "GoodsId","GoodsId"},
            { "JoinPrice","JoinPrice"},
            { "GoodsType","GoodsType"},
            { "BatchNo","BatchNo"},
            { "EffectiveDate","EffectiveDate"},
            { "ShelfType","ShelfType"},
        };

        public StorageRecordDao(GlobalConfig.DB.FromType fromType)
        {

        }

        public StorageRecordDao()
        {

        }

        #region [数据库读取获取StorageRecordInfo表信息]

        /// <summary> 获取StorageRecordInfo表信息
        /// </summary>
        /// <param name="dr">IDataReader</param>
        /// <returns></returns>
        private static StorageRecordInfo ReaderStorageRecordInfo(IDataReader dr)
        {
            var storageRecordInfo = new StorageRecordInfo
            {
                StockId = dr["StockId"] == DBNull.Value ? Guid.Empty : new Guid(dr["StockId"].ToString()),
                FilialeId = dr["FilialeId"] == DBNull.Value ? Guid.Empty : new Guid(dr["FilialeId"].ToString()),
                ThirdCompanyID = dr["ThirdCompanyID"] == DBNull.Value ? Guid.Empty : new Guid(dr["ThirdCompanyID"].ToString()),
                TradeCode = dr["TradeCode"].ToString(),
                LinkTradeCode = dr["LinkTradeCode"].ToString(),
                DateCreated = Convert.ToDateTime(dr["DateCreated"].ToString()),
                Transactor = dr["Transactor"].ToString(),
                Description = dr["Description"].ToString(),
                AccountReceivable = Math.Abs(Convert.ToDecimal(dr["AccountReceivable"].ToString()))
            };
            storageRecordInfo.SubtotalQuantity = Math.Abs(Convert.ToDecimal(dr["SubtotalQuantity"].ToString()));
            storageRecordInfo.StockType = Convert.ToInt32(dr["StockType"].ToString());
            storageRecordInfo.StockState = Convert.ToInt32(dr["StockState"].ToString());
            storageRecordInfo.WarehouseId = dr["WarehouseId"] == DBNull.Value ? Guid.Empty : new Guid(dr["WarehouseId"].ToString());
            storageRecordInfo.RelevanceFilialeId = dr["RelevanceFilialeId"] == DBNull.Value ? Guid.Empty : new Guid(dr["RelevanceFilialeId"].ToString());
            storageRecordInfo.RelevanceWarehouseId = dr["RelevanceWarehouseId"] == DBNull.Value ? Guid.Empty : new Guid(dr["RelevanceWarehouseId"].ToString());
            storageRecordInfo.LinkTradeID = dr["LinkTradeID"] == DBNull.Value ? Guid.Empty : new Guid(dr["LinkTradeID"].ToString());
            storageRecordInfo.StockValidation = dr["StockValidation"] != DBNull.Value && bool.Parse(dr["StockValidation"].ToString());
            storageRecordInfo.LinkTradeType = Convert.ToInt32(dr["LinkTradeType"]);
            //storageRecordInfo.IsOut = Convert.ToBoolean(dr["IsOut"]);
            storageRecordInfo.AuditTime = dr["AuditTime"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(dr["AuditTime"]);
            storageRecordInfo.StorageType = dr["StorageType"] == DBNull.Value ? 0 : Convert.ToInt32(dr["StorageType"]);
            storageRecordInfo.BillNo = dr["BillNo"] == DBNull.Value ? string.Empty : dr["BillNo"].ToString();
            storageRecordInfo.LogisticsCode = dr["LogisticsCode"] == DBNull.Value ? string.Empty : dr["LogisticsCode"].ToString();
            storageRecordInfo.TradeBothPartiesType = Convert.ToInt32(dr["TradeBothPartiesType"]);
            return storageRecordInfo;
        }

        #endregion

        /// <summary>获取每月月末库存记录
        /// </summary>
        /// <param name="dayTime"></param>
        /// <returns></returns>
        public IList<MonthGoodsStockRecordInfo> SelectMonthGoodsStockRecordInfos(DateTime dayTime)
        {
            const string SQL = @"SELECT GoodsId, RealGoodsId, FilialeId, WarehouseId, Convert(date,Convert(varchar,SettleMonth)++'01') AS DayTime, SUM(StockNumber) AS NonceGoodsStock 
	FROM GoodsStockMonthSettleRecord  
	WHERE [SettleMonth]=@SettleMonth AND GoodsId!='00000000-0000-0000-0000-000000000000'
	group by GoodsId,RealGoodsId,FilialeId,WarehouseId,[SettleMonth] ";
            var yearStr = string.Format("{0}{1}", dayTime.Year, string.Format("{0}{1}", dayTime.Month >= 10 ? "" : "0", dayTime.Month));
            var parms = new[]
            {
                new Parameter("@SettleMonth", Convert.ToInt32(yearStr))
            };
            using (var db = DatabaseFactory.CreateRdb())
            {
                return db.Select<MonthGoodsStockRecordInfo>(true, SQL, parms).ToList();
            }
        }

        public IList<MonthGoodsStockInfo> SelectGoodsStockInfos(List<int> stockTypes, DateTime startTime, DateTime endTime)
        {
            var builder = new StringBuilder(@"SELECT D.GoodsId,SUM(cast(UnitPrice as real)*Quantity) AS TotalPrice,SUM(Quantity) AS Quantity
    FROM StorageRecordDetail AS D with(nolock) 
    inner join (
		SELECT StockId FROM StorageRecord with(nolock) 
		where AuditTime Between '{0}' and '{1}' 
		and StockState=" + (int)StorageRecordState.Finished + @" AND (");

            if (stockTypes != null && stockTypes.Count > 0)
            {
                builder.Append(" StockType IN(");
                builder.Append(string.Join(",", stockTypes.ToArray()));
                builder.Append(" ) ");
            }
            builder.Append(@" OR (StockType in(" + (int)StorageRecordType.BeyondStockIn + @"," + (int)StorageRecordType.LossStockOut + @") and LinkTradeCode LIKE 'MS%'))
	) as S on D.StockId=S.StockId
    GROUP BY D.GoodsId");
            IList<MonthGoodsStockInfo> list = new List<MonthGoodsStockInfo>();

            IDataReader sdr = null;
            try
            {
                sdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, string.Format(builder.ToString(), startTime, endTime));
                while (sdr.Read())
                {
                    list.Add(new MonthGoodsStockInfo
                    {
                        GoodsId = sdr.GetGuid(0),
                        TotalPrice = Convert.ToDecimal(sdr["TotalPrice"].ToString()),
                        Quantity = Convert.ToInt32(sdr["Quantity"].ToString())
                    });
                }
            }
            catch (Exception ex)
            {
                throw new Exception("当月采购进货退货数据查询异常", ex);
            }
            finally
            {
                if (sdr != null)
                    sdr.Close();
            }
            return list;
        }

        /// <summary>从临时出入库表中获取销售数据
        /// </summary>
        /// <returns></returns>
        public IList<TempStorageRecordDetailInfo> GeTempStorageRecordDetailInfos(DateTime startTime, DateTime endTime)
        {
            string sql = @"SELECT D.GoodsId,WarehouseId,SUM(Quantity) AS Quantity
    FROM StorageRecordDetail AS D with(nolock) 
    inner join (
		SELECT StockId,WarehouseId FROM StorageRecord with(nolock) 
		where AuditTime Between '{0}' and '{1}' 
		and StockState=" + (int)StorageRecordState.Finished + @" AND StockType IN(" + (int)StorageRecordType.SellStockOut + @"," + (int)StorageRecordType.SellReturnIn + @")
	) as S on D.StockId=S.StockId
    GROUP BY D.GoodsId,WarehouseId ";
            IList<TempStorageRecordDetailInfo> list = new List<TempStorageRecordDetailInfo>();
            IDataReader sdr = null;
            try
            {
                sdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, 600, string.Format(sql, startTime, endTime));
                {
                    while (sdr.Read())
                    {
                        list.Add(new TempStorageRecordDetailInfo
                        {
                            GoodsId = sdr.GetGuid(0),
                            WarehouseId = sdr.GetGuid(1),
                            Quantity = (int)sdr.GetDouble(2)
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("从临时出入库表中获取销售数据", ex);
            }
            finally
            {
                if (sdr != null)
                    sdr.Close();
            }
            return list;
        }

        /// <summary>根据“单据编号”获取数据
        /// </summary>
        /// <param name="tradeCode">单据编号(多个单据编号，用英文状态下的逗号隔开)</param>
        /// <returns></returns>
        /// zal 2016-01-21
        public IList<StorageRecordInfo> GetStorageRecordList(string tradeCode)
        {
            IList<StorageRecordInfo> storageRecordList = new List<StorageRecordInfo>();
            if (string.IsNullOrEmpty(tradeCode))
            {
                return storageRecordList;
            }
            else
            {
                tradeCode = "'" + tradeCode.Replace(",", "','") + "'";
                var sql = string.Format(@"
                        select s.StockId,s.FilialeId,s.ThirdCompanyID,s.WarehouseId,s.TradeCode,s.LinkTradeCode,
                        s.DateCreated,s.Transactor,s.[Description],s.AccountReceivable,s.SubtotalQuantity,s.StockType,
                        s.StockState,s.RelevanceFilialeId,s.RelevanceWarehouseId,s.LinkTradeID,s.StockValidation,
                        s.IsError,s.AuditTime,s.LinkTradeType,s.IsOut,s.StorageType ,s.BillNo,LogisticsCode,
                        s.TradeBothPartiesType
                        from StorageRecord s WITH(NOLOCK)   where TradeCode in({0}) ", tradeCode);

                using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, sql))
                {
                    while (rdr.Read())
                    {
                        storageRecordList.Add(ReaderStorageRecordInfo(rdr));
                    }
                }
                return storageRecordList;
            }
        }

        public bool DeleteStorageRecord(Guid stockId, out string errorMessage)
        {
            errorMessage = string.Empty;
            const string SQL_DELETE_SEMIGOODSSTOCK_BY_STOCKID = " DELETE FROM StorageRecordDetail WHERE StockId=@StockId ";
            const string SQL_DELETE_SEMISTOCK_BY_STOCKID = "DELETE FROM StorageRecord WHERE StockId=@StockId";
            var parm = new SqlParameter("@StockId", SqlDbType.UniqueIdentifier) { Value = stockId };
            using (var conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                conn.Open();
                SqlTransaction tran = conn.BeginTransaction();
                try
                {
                    SqlHelper.ExecuteNonQuery(tran, SQL_DELETE_SEMIGOODSSTOCK_BY_STOCKID, parm);
                    SqlHelper.ExecuteNonQuery(tran, SQL_DELETE_SEMISTOCK_BY_STOCKID, parm);
                    tran.Commit();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    errorMessage = ex.Message;
                    throw new ApplicationException(ex.Message);
                }
            }
            return true;
        }


        /// <summary>根据多条件搜索出入库商品汇总（历史数据库）
        /// </summary>
        /// <param name="stockType">单据类型</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">截止时间</param>
        /// <param name="companyId">往来单位ID</param>
        /// <param name="keepyear">保留几年数据</param>
        /// <param name="filialeId"></param>
        /// <returns></returns>
        public IList<GoodsOutInStorageStatisticsInfo> GetGoodsOutInStorageStatisticsList(int stockType, DateTime startTime, DateTime endTime, Guid companyId, int keepyear, Guid filialeId, Guid warehouseId)
        {
            string sqlStr = String.Format(@"
select GoodsName,COUNT(GoodsName) AS [Time],UnitPrice AS Price,SUM(Quantity) AS Quantity,StockType AS StorageType,MIN(UnitPrice) AS LowerPrice,FilialeId,IsOut  
from (
		SELECT srd.GoodsName,UnitPrice,Quantity,sr.StockType, FilialeId,
        sr.IsOut 
		FROM StorageRecordDetail srd  WITH(NOLOCK)
		INNER JOIN StorageRecord sr WITH(NOLOCK) ON sr.StockId = srd.StockId 
		WHERE sr.AuditTime >= '{0}' AND sr.AuditTime < '{1}' AND sr.StockState={2}
", startTime, endTime, (Int32)StorageRecordState.Finished);
            var sql = new StringBuilder(sqlStr);
            if (stockType != -1)
            {
                sql.AppendFormat(" AND StockType= {0}", stockType);
            }

            if (filialeId != Guid.Empty)
            {
                sql.AppendFormat(" AND sr.FilialeId='{0}'  ", filialeId);
            }

            if (companyId != Guid.Empty)
            {
                sql.AppendFormat(" AND ThirdCompanyID='{0}' ", companyId);
            }
            if (warehouseId != Guid.Empty)
            {
                sql.AppendFormat(" AND sr.WarehouseId='{0}'  ", warehouseId);
            }
            sql.Append(" ) s GROUP BY s.GoodsName,s.StockType,UnitPrice,FilialeId,IsOut ");
            sql.Append(" ORDER BY GoodsName ");
            try
            {
                using (var db = DatabaseFactory.Create(startTime.Year))
                {
                    return db.Select<GoodsOutInStorageStatisticsInfo>(true, sql.ToString(), null).ToList();
                }
            }
            catch (Exception ex)
            {
            }
            return new List<GoodsOutInStorageStatisticsInfo>();
        }


        /// <summary>根据出入库单据编号获取调拨商品的数量和状态
        /// </summary>
        /// <param name="linkTradeCode">原始单据号</param>
        /// <param name="goodsIdList">商品ID集合</param>
        /// <returns></returns>
        public IList<GoodsSemiStockStateInfo> GetSemiGoodsQuanityWithState(string linkTradeCode, IList<Guid> goodsIdList)
        {
            IList<GoodsSemiStockStateInfo> goodIdList = new List<GoodsSemiStockStateInfo>();
            string sql =
                @"SELECT [RealGoodsId]      ,[Quantity]      ,StorageRecord.StockState
  FROM StorageRecordDetail WITH(NOLOCK) 
INNER JOIN StorageRecord WITH(NOLOCK)  ON StorageRecord.StockId = StorageRecordDetail.StockId AND StorageRecord.StockState>0
INNER JOIN Shopfront_ApplyStock ON Shopfront_ApplyStock.TradeCode = StorageRecord.LinkTradeCode
WHERE Shopfront_ApplyStock.TradeCode=@LinkTradeCode ";
            var sb = new StringBuilder();
            if (goodsIdList.Count > 0)
            {
                sql = sql + " AND [RealGoodsId] IN ({0}) ";
            }
            for (var i = 0; i < goodsIdList.Count; i++)
            {
                sb.Append("'" + goodsIdList[i] + "'");
                if (i < (goodsIdList.Count - 1))
                {
                    sb.Append(",");
                }
            }
            using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, goodsIdList.Count > 0 ? string.Format(sql, sb) : sql, new SqlParameter("@LinkTradeCode", linkTradeCode)))
            {
                while (dr.Read())
                {
                    goodIdList.Add(new GoodsSemiStockStateInfo
                    {
                        GoodsId = Guid.Parse(dr["RealGoodsId"].ToString()),
                        Quantity = Convert.ToInt32(dr["Quantity"]),
                        State = Convert.ToInt32(dr["StockState"].ToInt())
                    });
                }
            }
            return goodIdList;
        }


        /// <summary>根据出入库来源单据ID和出入库单据状态获取其对应的出入库记录单据号集合
        /// </summary>
        /// <param name="linkTradeId"></param>
        /// <param name="storageRecordState"></param>
        /// <returns></returns>
        public IList<String> GetStorageRecordTradeNos(Guid linkTradeId, StorageRecordState storageRecordState)
        {
            var sql = String.Format(@"SELECT [TradeCode] FROM [StorageRecord] WITH(NOLOCK) WHERE LinkTradeID='{0}' and  StockState={1}", linkTradeId, (Int32)storageRecordState);
            IList<String> tradeNos = new List<String>();
            using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, sql, null))
            {
                while (dr.Read())
                {
                    tradeNos.Add(dr.GetString(0));
                }
            }
            return tradeNos;
        }

        /// <summary>根据出入库来源单据ID和出入库单据状态获取其对应的出入库记录集合
        /// </summary>
        /// <param name="linkTradeId"></param>
        /// <param name="storageRecordState"></param>
        /// <returns></returns>
        public IList<StorageRecordInfo> GetStorageRecordsByLinkTradeId(Guid linkTradeId, StorageRecordState storageRecordState)
        {
            IList<StorageRecordInfo> storageRecordList = new List<StorageRecordInfo>();
            var sql = string.Format(@"
                        select s.StockId,s.FilialeId,s.ThirdCompanyID,s.WarehouseId,s.TradeCode,s.LinkTradeCode,
                        s.DateCreated,s.Transactor,s.[Description],s.AccountReceivable,s.SubtotalQuantity,s.StockType,
                        s.StockState,s.RelevanceFilialeId,s.RelevanceWarehouseId,s.LinkTradeID,s.StockValidation,
                        s.IsError,s.AuditTime,s.LinkTradeType,s.IsOut,s.StorageType,s.BillNo,LogisticsCode,
                        s.TradeBothPartiesType
                        from StorageRecord s WITH(NOLOCK)   where LinkTradeID='{0}' and  StockState='{1}'", linkTradeId, (Int32)storageRecordState);

            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, sql))
            {
                while (rdr.Read())
                {
                    storageRecordList.Add(ReaderStorageRecordInfo(rdr));
                }
            }
            return storageRecordList;
        }

        /// <summary>根据出入库来源单据号和出入库单据状态获取其对应的出入库记录集合
        /// </summary>
        /// <param name="linkTradeNo"></param>
        /// <param name="storageRecordState"></param>
        /// <returns></returns>
        public IList<StorageRecordInfo> GetStorageRecordsByLinkTradeNo(String linkTradeNo,
            StorageRecordState storageRecordState)
        {
            IList<StorageRecordInfo> storageRecordList = new List<StorageRecordInfo>();
            var sql = string.Format(@"
                        select s.StockId,s.FilialeId,s.ThirdCompanyID,s.WarehouseId,s.TradeCode,s.LinkTradeCode,
                        s.DateCreated,s.Transactor,s.[Description],s.AccountReceivable,s.SubtotalQuantity,s.StockType,
                        s.StockState,s.RelevanceFilialeId,s.RelevanceWarehouseId,s.LinkTradeID,s.StockValidation,
                        s.IsError,s.AuditTime,s.LinkTradeType,s.IsOut,s.StorageType,BillNo,LogisticsCode,
                        s.TradeBothPartiesType
                        from StorageRecord s WITH(NOLOCK)   where LinkTradeCode='{0}' and  StockState='{1}'", linkTradeNo, (Int32)storageRecordState);

            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, sql))
            {
                while (rdr.Read())
                {
                    storageRecordList.Add(ReaderStorageRecordInfo(rdr));
                }
            }
            return storageRecordList;
        }

        /// <summary>根据来源单据号获取商品的出库数量
        /// </summary>
        /// <param name="linkTradeNo">来源单据号</param>
        /// <returns></returns>
        public Dictionary<Guid, Int32> GetStorageRecordNormalGoodsQuantityByLinkTradeNo(String linkTradeNo)
        {
            var sql = String.Format(@"SELECT SRD.RealGoodsId,SUM(SRD.Quantity) AS Quantity 
FROM StorageRecordDetail srd INNER JOIN StorageRecord SR
ON SRD.StockId=SR.StockId WHERE SR.LinkTradeCode='{0}' and StockState <>{1} GROUP BY SRD.RealGoodsId", linkTradeNo, (Int32)StorageRecordState.Canceled);

            Dictionary<Guid, int> goodsQuantityDic = null;
            using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, sql, null))
            {
                if (dr != null)
                {
                    goodsQuantityDic = new Dictionary<Guid, Int32>();
                    while (dr.Read())
                    {
                        var realGoodsId = dr["RealGoodsId"] == DBNull.Value ? Guid.Empty : new Guid(dr["RealGoodsId"].ToString());
                        var quantity = dr["Quantity"] == DBNull.Value ? 0 : int.Parse(dr["Quantity"].ToString());
                        goodsQuantityDic.Add(realGoodsId, Math.Abs(quantity));
                    }
                }
            }
            return goodsQuantityDic;
        }

        public IList<StorageRecordInfo> GetStorageRecordListToPages(Guid warehouseId, Guid companyId, string goodsName, string serarchKey,
            List<StorageRecordType> storageRecordTypes, List<StorageRecordState> storageRecordStates, int storageType, Guid filialeId, DateTime startTime, DateTime endTime, int mode,
            int keepYear, int startPage, int pageSize, out long recordCount)
        {
            var sqlStr = new StringBuilder(@"
SELECT 
    s.StockId,s.FilialeId,s.ThirdCompanyID,
    s.TradeCode,s.LinkTradeCode,s.DateCreated,s.Transactor,s.Description,s.AccountReceivable,
    s.SubtotalQuantity,s.StockType,s.StockState,s.WarehouseId,s.RelevanceFilialeId,
    s.RelevanceWarehouseId,s.LinkTradeID,s.StockValidation,s.AuditTime, s.LinkTradeType, s.IsOut, s.StorageType,s.BillNo,s.TradeBothPartiesType 
FROM StorageRecord AS s WITH(NOLOCK)");

            if (!string.IsNullOrEmpty(goodsName))
            {
                sqlStr.Append(" INNER JOIN (SELECT distinct(StockId) FROM StorageRecordDetail WITH(NOLOCK) WHERE GoodsName LIKE '%").Append(goodsName).Append("%') srd on s.StockId = srd.StockId");
            }
            sqlStr.Append(" LEFT JOIN lmShop_CompanyCussent AS lc ON lc.CompanyId = s.ThirdCompanyID");
            sqlStr.Append(" WHERE s.DateCreated>='").Append(startTime).Append("'");
            sqlStr.Append(" AND s.DateCreated<='").Append(endTime.AddDays(1).AddMilliseconds(-1)).Append("'");
            if (companyId != Guid.Empty)
            {
                sqlStr.AppendFormat(" AND s.ThirdCompanyID ='{0}' ", companyId);
            }
            else
            {
                if (mode > 0)
                {
                    sqlStr.AppendFormat(
                        mode == 2
                            ? " AND (lc.RelevanceFilialeId = '{0}' OR s.ThirdCompanyID ='{0}')"
                            : " AND lc.RelevanceFilialeId <> '{0}'", Guid.Empty);
                }
            }
            if (!String.IsNullOrWhiteSpace(serarchKey))
            {
                sqlStr.Append(" AND (s.TradeCode LIKE '").Append(serarchKey).Append("%'");
                sqlStr.Append(" OR s.LinkTradeCode LIKE '").Append(serarchKey).Append("%'");
                sqlStr.Append(" OR s.BillNo LIKE '%").Append(serarchKey).Append("%')");
            }
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
            if (warehouseId != default(Guid))
            {
                sqlStr.Append(" AND s.WarehouseId='").Append(warehouseId).Append("'");
            }
            if (filialeId != default(Guid))
            {
                sqlStr.Append(" AND s.FilialeId='").Append(filialeId).Append("'");
            }
            if (storageType != default(Int32))
            {
                sqlStr.Append(" AND s.StorageType=").Append(storageType);
            }

            using (var db = DatabaseFactory.Create())
            {
                const string ORDER_BY = " DateCreated DESC ";
                var pageQuery = new Keede.DAL.Helper.Sql.PageQuery(startPage, pageSize, sqlStr.ToString(), ORDER_BY);

                var pageItem = db.SelectByPage<StorageRecordInfo>(true, pageQuery);
                recordCount = pageItem.RecordCount;
                return pageItem.Items.ToList();
            }
        }

        /// <summary>多条件搜索出入库明细信息集合（历史数据库）
        /// </summary>
        /// <param name="goodsId">商品ID</param>
        /// <param name="starttime">开始时间</param>
        /// <param name="endtime">截止时间</param>
        /// <param name="filiaid">公司ID</param>
        /// <param name="warehouseId">仓库ID</param>
        /// <param name="orderby">排序字段</param>
        /// <param name="keepyear">保留几年数据</param>
        /// <returns></returns>
        /// 
        public IList<GoodsStockRunning> GetStockRunning(Guid goodsId, DateTime starttime, DateTime endtime, Guid warehouseId, int keepyear, List<StorageRecordType> storageRecordTypes)
        {
            var strStorageRecordTypes = storageRecordTypes.Aggregate(String.Empty, (ent, type) => ent + ((int)type + ","));
            string strStockType = String.Empty;
            if (!String.IsNullOrWhiteSpace(strStorageRecordTypes))
            {
                strStockType = "and StockType IN (" + strStorageRecordTypes.Substring(0, strStorageRecordTypes.Length - 1) + @")";
            }
            string sql = string.Format(@"
select 
    t1.StockId,t3.CompanyName,t2.TradeCode,t1.RealGoodsId,
    rtrim(ltrim(replace(replace(replace(replace(substring(Specification,1,
    case charindex('瞳',Specification) when 0 then len(Specification) 
    else charindex('瞳',Specification)-1 end),'左眼 ',''),'右眼 ',''),'L ','') ,'R ',''))) as 
    Specification,
   t1.Quantity,t1.NonceWarehouseGoodsStock,t2.DateCreated,t2.AuditTime,t2.StockType
from  StorageRecordDetail t1  with(nolock)
inner join (
	select
	    StockId,TradeCode,DateCreated,AuditTime,StockType,ThirdCompanyID
	from  StorageRecord
	with(nolock)
	where StockState ={4} 
	and WarehouseId = '{0}'
	and DateCreated >='{1}'
	and DateCreated <='{2}'
    " + strStockType + @"
) t2 on t1.StockId  = t2.StockId
left join  lmShop_CompanyCussent t3
on t2.ThirdCompanyID  = t3.CompanyId
where  t1.GoodsId  = '{3}' ", warehouseId, starttime, endtime, goodsId, (int)StorageRecordState.Finished);
            var builder = new StringBuilder(sql);
            IList<GoodsStockRunning> goodsStockRunning = new List<GoodsStockRunning>();
            using (var dr = SqlHelper.ExecuteReader(GlobalConfig.GetErpDbName(starttime.Year), true, builder.ToString(), null))
            {
                while (dr.Read())
                {
                    var goodsstockrunning = new GoodsStockRunning
                    {
                        Companyname = dr["CompanyName"].ToString(),
                        DateCreated = dr["AuditTime"] == DBNull.Value ? Convert.ToDateTime(dr["DateCreated"]) : Convert.ToDateTime(dr["AuditTime"]),
                        GoodsId = new Guid(dr["RealGoodsId"].ToString()),
                        NonceWarehouseGoodsStock = Convert.ToDouble(dr["NonceWarehouseGoodsStock"]),
                        Quantity = Convert.ToDouble(dr["Quantity"]),
                        Specification = dr["Specification"].ToString(),
                        StockId = new Guid(dr["StockId"].ToString()),
                        TradeCode = dr["TradeCode"].ToString(),
                        StorageType = Convert.ToInt32(dr["StockType"])
                    };
                    goodsStockRunning.Add(goodsstockrunning);
                }
            }
            return goodsStockRunning;
        }

        /// <summary>判断商品是否存在未审核的出入库单据(用于删除商品时判断)
        /// </summary>
        /// <param name="goodsId">商品Id</param>
        /// <param name="realGoodsIds"> 子商品Ids</param>
        /// <returns></returns>
        public bool IsExistNormalStorageRecord(Guid goodsId, List<Guid> realGoodsIds)
        {
            var builder = new StringBuilder(String.Format(@"SELECT MAX(DateCreated) FROM StorageRecord SR WITH(NOLOCK)
 INNER JOIN StorageRecordDetail SRD WITH(NOLOCK) on SRD.StockId=SR.StockId WHERE GoodsId='{0}'  AND SR.StockState<>'{1}' ", goodsId, (Int32)StorageRecordState.Canceled));
            if (realGoodsIds != null && realGoodsIds.Count > 0)
            {
                var strb = new StringBuilder();
                foreach (var id in realGoodsIds)
                {
                    if (strb.Length == 0)
                        strb.Append(id);
                    else
                        strb.Append(",").Append(id);
                }
                builder.Append(" AND SRD.RealGoodsId IN (SELECT id as RealGoodsId FROM splitToTable('" + strb + "',','))");
            }
            var obj = SqlHelper.ExecuteScalar(GlobalConfig.ERP_DB_NAME, true, builder.ToString(), null);
            return obj != DBNull.Value && obj.ToString() != "";
        }

        public Dictionary<Guid, String> GetStorageRecordStockIdAndTradeCodeDic(Guid companyId, StorageRecordType[] storageRecordTypes, String searchKey)
        {
            var sql = new StringBuilder(String.Format(@"SELECT TOP {0} StockId,TradeCode FROM StorageRecord s WITH(NOLOCK) WHERE ThirdCompanyId='{1}' and StockState={2} ", 10, companyId, (Int32)StorageRecordState.Finished));
            var storageRecordTypeStr = String.Empty;
            foreach (var storageRecordType in storageRecordTypes)
            {
                if (String.IsNullOrWhiteSpace(storageRecordTypeStr))
                    storageRecordTypeStr += (int)storageRecordType;
                else
                    storageRecordTypeStr += "," + (int)storageRecordType;
            }
            if (!String.IsNullOrWhiteSpace(storageRecordTypeStr))
            {
                sql.AppendLine();
                sql.Append(" AND StockType IN (").Append(storageRecordTypeStr).Append(")");
            }

            if (!String.IsNullOrEmpty(searchKey))
            {
                sql.AppendFormat(" AND TradeCode LIKE '{0}%'", searchKey);
            }
            sql.Append(" ORDER BY s.DateCreated DESC ");
            var dics = new Dictionary<Guid, String>();
            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, sql.ToString(), null))
            {
                while (rdr.Read())
                {
                    dics.Add(rdr.GetGuid(0), rdr.GetString(1));
                }
            }
            return dics;
        }

        public Boolean SetStorageRecordDetailToJoinPrice(Guid stockId, Guid goodsId, Decimal joinPrice)
        {
            var strSql = "UPDATE StorageRecordDetail SET JoinPrice=" + joinPrice + " WHERE StockId='" + stockId + "' AND GoodsId='" + goodsId + "'";
            try
            {
                return SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, strSql) > 0;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        public Boolean IsExistNormalStorageRecord(String tradeCode)
        {
            var sql = String.Format(@"select count(0) from StorageRecord where TradeCode='{0}' and StockState<>{1} ", tradeCode, (Int32)StorageRecordState.Canceled);
            using (var db = DatabaseFactory.Create())
            {
                return db.GetValue<Int32>(true, sql) > 0;
            }
        }

        public IList<StorageRecordInfo> GetStorageRecordByLinkTradeCode(String linkTradeCode)
        {
            var sql = String.Format(@"SELECT  [StockId]
      ,[FilialeId]
      ,[ThirdCompanyID]
      ,[WarehouseId]
      ,[TradeCode]
      ,[LinkTradeCode]
      ,[DateCreated]
      ,[Transactor]
      ,[Description]
      ,[AccountReceivable]
      ,[SubtotalQuantity]
      ,[StockType]
      ,[StockState]
      ,[RelevanceFilialeId]
      ,[RelevanceWarehouseId]
      ,[LinkTradeID]
      ,[StockValidation]
      ,[IsError]
      ,[AuditTime]
      ,[LinkTradeType]
      ,[IsOut]
      ,[StorageType],[BillNo],LogisticsCode
      ,[TradeBothPartiesType]
  FROM [StorageRecord] with(nolock) where LinkTradeCode='{0}'", linkTradeCode);
            IList<StorageRecordInfo> storageRecordList = new List<StorageRecordInfo>();
            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, sql))
            {
                while (rdr.Read())
                {
                    storageRecordList.Add(ReaderStorageRecordInfo(rdr));
                }
            }
            return storageRecordList;
        }

        /// <summary> 根据门店采购申请编号获取(待审核)商品明细实际出库数量
        /// </summary>
        /// <param name="linkTradeCode">门店采购申请编号</param>
        /// <param name="filialeId"></param>
        /// <param name="thirdCompanyId"></param>
        /// <param name="stockType">出入库类型</param>
        /// <param name="states">审核状态列表</param>
        /// <returns></returns>
        public Dictionary<Guid, int> GetSendQuantityByLinkTradeCode(string linkTradeCode, Guid filialeId, Guid thirdCompanyId, int stockType, List<int> states)
        {
            var builder = new StringBuilder(@"SELECT SRD.RealGoodsId,SUM(SRD.Quantity) AS Quantity 
FROM StorageRecordDetail srd WITH(NOLOCK) INNER JOIN StorageRecord SR WITH(NOLOCK)
ON SRD.StockId=SR.StockId WHERE SR.LinkTradeCode='");
            builder.Append(linkTradeCode).AppendFormat("' AND SR.FilialeId='{0}' AND SR.ThirdCompanyID='{1}'", filialeId, thirdCompanyId);
            if (stockType != -1)
            {
                builder.Append(" AND SR.StockType=").Append(stockType);
            }
            if (states != null && states.Count > 0)
            {
                builder.Append(" AND SR.StockState IN(");
                for (int i = 0; i < states.Count; i++)
                {
                    builder.Append(states[i]);
                    if (i != states.Count - 1)
                        builder.Append(",");
                }
                builder.Append(")");
            }
            builder.Append(" GROUP BY SRD.RealGoodsId ");
            Dictionary<Guid, int> dict = null;
            using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, builder.ToString(), null))
            {
                if (dr != null)
                {
                    dict = new Dictionary<Guid, int>();
                    while (dr.Read())
                    {
                        var realGoodsId = dr["RealGoodsId"] == DBNull.Value ? Guid.Empty : new Guid(dr["RealGoodsId"].ToString());
                        var quantity = dr["Quantity"] == DBNull.Value ? 0 : int.Parse(dr["Quantity"].ToString());
                        dict.Add(realGoodsId, Math.Abs(quantity));
                    }
                }
            }
            return dict;
        }


        #region 新出入库使用方法
        /// <summary> 根据出入库记录ID获取出入库明细
        /// </summary>
        /// <param name="stockId">出入库记录ID</param>
        /// <returns></returns>
        public IList<StorageRecordDetailInfo> GetStorageRecordDetailListByStockId(Guid stockId)
        {
            const string SQL = @"
            SELECT 
            gs.StockId,s.FilialeId,s.ThirdCompanyID,s.TradeCode,s.LinkTradeCode,s.DateCreated,s.Transactor,
            gs.GoodsId,gs.RealGoodsId,gs.GoodsName,gs.GoodsCode,gs.Specification,gs.Quantity,
            gs.UnitPrice,NonceWarehouseGoodsStock,s.StockType,s.StockState,
            gs.Description,s.StockValidation,s.WarehouseId,gs.JoinPrice,BatchNo,ShelfType 
            FROM StorageRecordDetail AS gs WITH(NOLOCK) 
            INNER JOIN StorageRecord AS s WITH(NOLOCK) ON gs.StockId=s.StockId AND gs.StockId=@StockId
            ORDER BY gs.GoodsName,gs.Specification ASC";

            var parm = new SqlParameter("@StockId", SqlDbType.UniqueIdentifier) { Value = stockId };
            IList<StorageRecordDetailInfo> goodsStockList = new List<StorageRecordDetailInfo>();
            using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL, parm))
            {
                while (dr.Read())
                {
                    var stockInfo = new StorageRecordDetailInfo
                    {
                        StockId = new Guid(dr["StockId"].ToString()),
                        GoodsId = dr["GoodsId"] == DBNull.Value ? Guid.Empty : new Guid(dr["GoodsId"].ToString()),
                        RealGoodsId = dr["RealGoodsId"] == DBNull.Value ? Guid.Empty : new Guid(dr["RealGoodsId"].ToString()),
                        Specification = dr["Specification"] == DBNull.Value ? "" : dr["Specification"].ToString(),
                        Quantity = Math.Abs(int.Parse(dr["Quantity"].ToString())),
                        UnitPrice = decimal.Parse(dr["UnitPrice"].ToString()),
                        Description = dr["Description"] == DBNull.Value ? "" : dr["Description"].ToString(),
                        GoodsName = dr["GoodsName"] == DBNull.Value ? "" : dr["GoodsName"].ToString(),
                        GoodsCode = dr["GoodsCode"] == DBNull.Value ? "" : dr["GoodsCode"].ToString(),
                        NonceWarehouseGoodsStock = int.Parse(dr["NonceWarehouseGoodsStock"].ToString()),
                        JoinPrice = dr["JoinPrice"] == DBNull.Value ? 0 : decimal.Parse(dr["JoinPrice"].ToString()),
                        BatchNo = dr["BatchNo"] == DBNull.Value ? "" : dr["BatchNo"].ToString(),
                        ShelfType = dr["ShelfType"] == DBNull.Value ? Byte.MinValue : Convert.ToByte(dr["ShelfType"])
                    };
                    goodsStockList.Add(stockInfo);
                }
            }
            return goodsStockList;
        }


        /// <summary> 
        /// 通过关联单号获取出入库明细
        /// </summary>
        /// <param name="linkTradeCode"></param>
        /// <returns></returns>
        public IList<StorageRecordDetailInfo> GetStorageRecordDetailListByLinkTradeCode(string linkTradeCode)
        {
            const string SQL = @"
            SELECT gs.StockId,s.FilialeId,s.ThirdCompanyID,s.TradeCode,s.LinkTradeCode,s.DateCreated,s.Transactor,
            gs.GoodsId,gs.RealGoodsId,gs.GoodsName,gs.GoodsCode,gs.Specification,gs.Quantity,
            gs.UnitPrice,NonceWarehouseGoodsStock,s.StockType,s.StockState,
            gs.Description,s.StockValidation,s.WarehouseId,gs.JoinPrice,BatchNo,ShelfType 
            FROM StorageRecordDetail AS gs WITH(NOLOCK) 
            INNER JOIN StorageRecord AS s WITH(NOLOCK) ON gs.StockId=s.StockId and s.LinkTradeCode=@LinkTradeCode
            ORDER BY gs.GoodsName,gs.Specification ASC";

            var parm = new SqlParameter("@LinkTradeCode", SqlDbType.VarChar) { Value = linkTradeCode };
            IList<StorageRecordDetailInfo> goodsStockList = new List<StorageRecordDetailInfo>();
            using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL, parm))
            {
                while (dr.Read())
                {
                    var stockInfo = new StorageRecordDetailInfo
                    {
                        StockId = new Guid(dr["StockId"].ToString()),
                        GoodsId = dr["GoodsId"] == DBNull.Value ? Guid.Empty : new Guid(dr["GoodsId"].ToString()),
                        RealGoodsId = dr["RealGoodsId"] == DBNull.Value ? Guid.Empty : new Guid(dr["RealGoodsId"].ToString()),
                        Specification = dr["Specification"] == DBNull.Value ? "" : dr["Specification"].ToString(),
                        Quantity = Math.Abs(int.Parse(dr["Quantity"].ToString())),
                        UnitPrice = decimal.Parse(dr["UnitPrice"].ToString()),
                        Description = dr["Description"] == DBNull.Value ? "" : dr["Description"].ToString(),
                        GoodsName = dr["GoodsName"] == DBNull.Value ? "" : dr["GoodsName"].ToString(),
                        GoodsCode = dr["GoodsCode"] == DBNull.Value ? "" : dr["GoodsCode"].ToString(),
                        NonceWarehouseGoodsStock = int.Parse(dr["NonceWarehouseGoodsStock"].ToString()),
                        JoinPrice = dr["JoinPrice"] == DBNull.Value ? 0 : decimal.Parse(dr["JoinPrice"].ToString()),
                        BatchNo = dr["BatchNo"] == DBNull.Value ? "" : dr["BatchNo"].ToString(),
                        ShelfType = dr["ShelfType"] == DBNull.Value ? Byte.MinValue : Convert.ToByte(dr["ShelfType"])
                    };
                    goodsStockList.Add(stockInfo);
                }
            }
            return goodsStockList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stockId"></param>
        /// <returns></returns>
        public StorageRecordInfo GetStorageRecord(Guid stockId)
        {
            const string SQL = @"SELECT StockId,s.FilialeId,ThirdCompanyID,TradeCode,LinkTradeCode,
DateCreated,Transactor,s.[Description],AccountReceivable,SubtotalQuantity,StockType,StockState,
s.WarehouseId,s.RelevanceFilialeId,RelevanceWarehouseId,LinkTradeID,s.StockValidation,s.StorageType,s.LinkTradeType,s.IsOut,s.AuditTime,s.StorageType,s.BillNo,LogisticsCode,
s.TradeBothPartiesType
FROM StorageRecord s WITH(NOLOCK) WHERE StockId=@StockId;";
            var parm = new SqlParameter("@StockId", SqlDbType.UniqueIdentifier) { Value = stockId };
            var stockInfo = new StorageRecordInfo();
            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL, parm))
            {
                if (rdr.Read())
                {
                    stockInfo = ReaderStorageRecordInfo(rdr);
                }
            }
            return stockInfo;
        }


        /// <summary>根据出入库单据号获取出入库记录信息
        /// </summary>
        /// <param name="tradeCode">出入库单据号</param>
        /// <returns></returns>
        public StorageRecordInfo GetStorageRecord(String tradeCode)
        {
            const string SQL = @"SELECT StockId,s.FilialeId,ThirdCompanyID,TradeCode,LinkTradeCode,
DateCreated,Transactor,s.[Description],AccountReceivable,SubtotalQuantity,StockType,StockState,
s.WarehouseId,s.RelevanceFilialeId,RelevanceWarehouseId,LinkTradeID,s.StockValidation,s.StorageType,s.LinkTradeType,s.IsOut,s.AuditTime,s.StorageType,BillNo,LogisticsCode,
s.TradeBothPartiesType
FROM StorageRecord s WITH(NOLOCK) WHERE TradeCode=@TradeCode;";

            var parm = new SqlParameter("@TradeCode", SqlDbType.VarChar) { Value = tradeCode };
            var stockInfo = new StorageRecordInfo();
            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL, parm))
            {
                if (rdr.Read())
                {
                    stockInfo = ReaderStorageRecordInfo(rdr);
                }
            }
            return stockInfo;
        }

        /// <summary>根据出入库单据号获取出入库记录信息
        /// </summary>
        /// <param name="billNo">WMS进出货单号</param>
        /// <param name="tradeCode">要排除的出入库单据号</param>
        /// <returns></returns>
        public StorageRecordInfo GetStorageRecordByBillNoExcludeTradeCode(string billNo, string tradeCode)
        {
            const string SQL = @"SELECT StockId,s.FilialeId,ThirdCompanyID,TradeCode,LinkTradeCode,
DateCreated,Transactor,s.[Description],AccountReceivable,SubtotalQuantity,StockType,StockState,
s.WarehouseId,s.RelevanceFilialeId,RelevanceWarehouseId,LinkTradeID,s.StockValidation,s.StorageType,s.LinkTradeType,s.IsOut,s.AuditTime,s.StorageType,BillNo,LogisticsCode,
s.TradeBothPartiesType
FROM StorageRecord s WITH(NOLOCK) WHERE BillNo=@BillNo and TradeCode<>@TradeCode;";

            var parm1 = new SqlParameter("@TradeCode", SqlDbType.VarChar) { Value = tradeCode };
            var parm2 = new SqlParameter("@BillNo", SqlDbType.VarChar) { Value = billNo };
            var stockInfo = new StorageRecordInfo();
            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL, parm1, parm2))
            {
                if (rdr.Read())
                {
                    stockInfo = ReaderStorageRecordInfo(rdr);
                }
            }
            return stockInfo;
        }


        /// <summary>
        /// 根据入库仓库，托管公司，供应商获取出入库集合
        /// </summary>
        /// <param name="warehouseId">入库仓库</param>
        /// <param name="storageType">入库储</param>
        /// <param name="filialeId">托管公司</param>
        /// <param name="companyId">供应商</param>
        /// <param name="stockType">入库类型</param>
        /// <param name="stockState">单据状态</param>
        /// <param name="strKey">单据编号</param>
        /// <returns></returns>
        public IList<StorageRecordInfo> GetStorageRecordListByWarehouseIdAndCompanyId(Guid warehouseId, int storageType,
            Guid filialeId, Guid companyId, int stockType, int stockState, string strKey)
        {
            string SQL =
                @"SELECT StockId, TradeCode FROM [StorageRecord] WITH(NOLOCK)
                                    WHERE WarehouseId=@WarehouseId 
                                    AND ThirdCompanyID=@ThirdCompanyID     
                                    AND StorageType=@StorageType
                                    AND FilialeId=@FilialeId 
                                    AND StockType=@StockType 
                                    AND StockState=@StockState";
            if (!string.IsNullOrEmpty(strKey))
            {
                SQL += " AND TradeCode like'%" + strKey + "%'";
            }
            SQL += " ORDER BY DateCreated DESC";
            var parms = new[]
            {
                new SqlParameter("@ThirdCompanyID", SqlDbType.UniqueIdentifier) {Value = companyId},
                new SqlParameter("@WarehouseId", SqlDbType.UniqueIdentifier) {Value = warehouseId},
                new SqlParameter("@StorageType", SqlDbType.Int) {Value = storageType},
                new SqlParameter("@FilialeId", SqlDbType.UniqueIdentifier) {Value = filialeId},
                new SqlParameter("@StockType", SqlDbType.Int) {Value = stockType},
                new SqlParameter("@StockState", SqlDbType.Int) {Value = stockState}
            };
            IList<StorageRecordInfo> stockList = new List<StorageRecordInfo>();
            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL, parms))
            {
                while (rdr.Read())
                {
                    var storageRecordInfo = new StorageRecordInfo
                    {
                        StockId = rdr["StockId"] == DBNull.Value ? Guid.Empty : new Guid(rdr["StockId"].ToString()),
                        TradeCode = rdr["TradeCode"].ToString()
                    };
                    stockList.Add(storageRecordInfo);
                }
            }
            return stockList;
        }

        /// <summary>
        /// 根据入库仓库，供应商获取出入库集合
        /// </summary>
        /// <param name="warehouseId">入库仓库</param>
        /// <param name="companyId">供应商</param>
        /// <param name="stockType">入库类型</param>
        /// <param name="stockState">单据状态</param>
        /// <param name="strKey">单据编号</param>
        /// <returns></returns>
        public IList<StorageRecordInfo> GetStorageRecordListByWarehouseAndCompany(Guid warehouseId, Guid companyId, int stockType, int stockState, string strKey)
        {
            string SQL =
                @"SELECT StockId, TradeCode FROM [StorageRecord] WITH(NOLOCK)
                                    WHERE WarehouseId=@WarehouseId 
                                    AND ThirdCompanyID=@ThirdCompanyID   
                                    AND StockType=@StockType 
                                    AND StockState=@StockState";
            if (!string.IsNullOrEmpty(strKey))
            {
                SQL += " AND TradeCode like'%" + strKey + "%'";
            }
            SQL += " ORDER BY DateCreated DESC";
            var parms = new[]
            {
                new SqlParameter("@ThirdCompanyID", SqlDbType.UniqueIdentifier) {Value = companyId},
                new SqlParameter("@WarehouseId", SqlDbType.UniqueIdentifier) {Value = warehouseId},
                new SqlParameter("@StockType", SqlDbType.Int) {Value = stockType},
                new SqlParameter("@StockState", SqlDbType.Int) {Value = stockState}
            };
            IList<StorageRecordInfo> stockList = new List<StorageRecordInfo>();
            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL, parms))
            {
                while (rdr.Read())
                {
                    var storageRecordInfo = new StorageRecordInfo
                    {
                        StockId = rdr["StockId"] == DBNull.Value ? Guid.Empty : new Guid(rdr["StockId"].ToString()),
                        TradeCode = rdr["TradeCode"].ToString()
                    };
                    stockList.Add(storageRecordInfo);
                }
            }
            return stockList;
        }

        /// <summary>
        /// 获取出入库Id和出入库单号
        /// </summary>
        /// <param name="warehouseId">仓库Id</param>
        /// <param name="companyId">第三方公司Id</param>
        /// <param name="hostingFilialeId">托管公司Id</param>
        /// <param name="tradeCode">出入库单号</param>
        /// <returns></returns>
        /// zal 2017-02-07
        public Dictionary<Guid, string> GetDicStockIdAndTradeCode(Guid warehouseId, Guid companyId, Guid hostingFilialeId, string tradeCode)
        {
            string sql = @"
            SELECT StockId,TradeCode FROM StorageRecord WITH(NOLOCK)
            WHERE WarehouseId = @WarehouseId AND ThirdCompanyID = @ThirdCompanyID AND FilialeId = @FilialeId";

            if (!string.IsNullOrEmpty(tradeCode))
            {
                sql += " AND TradeCode='" + tradeCode + "'";
            }
            SqlParameter[] pars = new SqlParameter[]
            {
                new SqlParameter("@WarehouseId", warehouseId),
                new SqlParameter("@ThirdCompanyID", companyId),
                new SqlParameter("@FilialeId", hostingFilialeId)
             };

            var dicStockIdAndTradeCode = new Dictionary<Guid, string>();
            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, sql, pars))
            {
                while (rdr.Read())
                {
                    dicStockIdAndTradeCode.Add(new Guid(rdr["StockId"].ToString()), rdr["TradeCode"].ToString());
                }
            }
            return dicStockIdAndTradeCode;
        }

        public Dictionary<Guid, int> GetEffictiveSellReturn(string tradeCode, int stockType, string currentTradeCode)
        {
            var dics = new Dictionary<Guid, int>();
            const string SQL_Add = @"SELECT T.RealGoodsId,SUM(Quantity) AS Quantity FROM (
select RealGoodsId,SUM(SR.Quantity) AS Quantity from StorageRecordDetail AS SR 
INNER JOIN StorageRecord AS S ON SR.StockId=S.StockId 
WHERE TradeCode=@TradeCode
GROUP BY RealGoodsId
UNION ALL
select RealGoodsId,SUM(SR.Quantity) AS Quantity from StorageRecordDetail AS SR 
INNER JOIN StorageRecord AS S ON SR.StockId=S.StockId AND StockType=@StockType
WHERE LinkTradeCode=@TradeCode
GROUP BY RealGoodsId) AS T
GROUP BY T.RealGoodsId
HAVING SUM(Quantity)<0";

            const string SQL_Edit = @"SELECT T.RealGoodsId,SUM(Quantity) AS Quantity FROM (
select RealGoodsId,SUM(SR.Quantity) AS Quantity from StorageRecordDetail AS SR 
INNER JOIN StorageRecord AS S ON SR.StockId=S.StockId 
WHERE TradeCode=@TradeCode
GROUP BY RealGoodsId
UNION ALL
select RealGoodsId,SUM(SR.Quantity) AS Quantity from StorageRecordDetail AS SR 
INNER JOIN StorageRecord AS S ON SR.StockId=S.StockId AND StockType=@StockType
WHERE LinkTradeCode=@TradeCode AND TradeCode<>@CurrentTradeCode
GROUP BY RealGoodsId) AS T
GROUP BY T.RealGoodsId
HAVING SUM(Quantity)<0";

            var parms = string.IsNullOrEmpty(currentTradeCode) ? new[]
            {
                new SqlParameter("@StockType", SqlDbType.Int) {Value = stockType},
                new SqlParameter("@TradeCode", SqlDbType.VarChar) {Value = tradeCode}
            } : new[]
            {
                new SqlParameter("@StockType", SqlDbType.Int) {Value = stockType},
                new SqlParameter("@TradeCode", SqlDbType.VarChar) {Value = tradeCode},
                new SqlParameter("@CurrentTradeCode", SqlDbType.VarChar) {Value = currentTradeCode}
            }
            ;
            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, string.IsNullOrEmpty(currentTradeCode) ? SQL_Add : SQL_Edit, parms))
            {
                while (rdr.Read())
                {
                    var realGoodsId = rdr["RealGoodsId"] == DBNull.Value ? Guid.Empty : new Guid(rdr["RealGoodsId"].ToString());
                    if (realGoodsId != Guid.Empty)
                    {
                        dics.Add(realGoodsId, rdr["Quantity"] == DBNull.Value ? 0 : Convert.ToInt32(rdr["Quantity"].ToString()));
                    }
                }
            }
            return dics;
        }


        /// <summary>新增或修改最后一次进货价
        /// </summary>
        /// <param name="goodsPurchaseLastPriceInfo">最后一次进货价模型</param>
        public Boolean InsertLastPrice(GoodsPurchaseLastPriceInfo goodsPurchaseLastPriceInfo)
        {
            const string STORAGERECORDINFO_INTERORUPDATE = @"
IF EXISTS (SELECT TOP 1 Id FROM GoodsPurchaseLastPrice WHERE [RealGoodsId]=@RealGoodsId AND ThirdCompanyID=@ThirdCompanyID AND WarehouseId=@WarehouseId)
    BEGIN
	    UPDATE GoodsPurchaseLastPrice SET UnitPrice=@UnitPrice,LastPriceDate=@LastPriceDate WHERE [RealGoodsId]=@RealGoodsId AND ThirdCompanyID=@ThirdCompanyID AND WarehouseId=@WarehouseId;
    END
ELSE
    BEGIN
	    INSERT INTO [GoodsPurchaseLastPrice]
           (id, GoodsId, RealGoodsId, ThirdCompanyID, WarehouseId,UnitPrice, LastPriceDate)
     VALUES
           (@id, @GoodsId, @RealGoodsId, @ThirdCompanyID,@WarehouseId, @UnitPrice, @LastPriceDate);
    END
";
            var parms = new[]
                {
                    new Parameter("@id", goodsPurchaseLastPriceInfo.Id),
                    new Parameter("@GoodsId", goodsPurchaseLastPriceInfo.GoodsId),
                    new Parameter("@RealGoodsId", goodsPurchaseLastPriceInfo.RealGoodsId),
                    new Parameter("@ThirdCompanyID", goodsPurchaseLastPriceInfo.ThirdCompanyId),
                    new Parameter("@WarehouseId", goodsPurchaseLastPriceInfo.WarehouseId),
                    new Parameter("@UnitPrice", goodsPurchaseLastPriceInfo.UnitPrice),
                    new Parameter("@LastPriceDate", goodsPurchaseLastPriceInfo.LastPriceDate)
                };

            using (var db = DatabaseFactory.Create())
            {
                return db.Execute(false, STORAGERECORDINFO_INTERORUPDATE, parms);
            }
        }

        /// <summary>
        /// 获取商品的最后一次进货价信息
        /// </summary>
        /// <param name="warehouseId">仓库id</param>
        /// <returns></returns>
        public List<GoodsPurchaseLastPriceInfo> GetGoodsPurchaseLastPriceInfoByWarehouseId(Guid warehouseId)
        {
            List<GoodsPurchaseLastPriceInfo> goodsPurchaseLastPriceInfoList = new List<GoodsPurchaseLastPriceInfo>();
            var strbSql = new StringBuilder();

            strbSql.Append(@"
            SELECT Id,GoodsId,RealGoodsId,ThirdCompanyID,WarehouseId,UnitPrice,LastPriceDate FROM
            (
                SELECT ROW_NUMBER() OVER (Partition BY GoodsId,ThirdCompanyID,WarehouseId ORDER BY LastPriceDate DESC) AS rowNum,Id,GoodsId,RealGoodsId,ThirdCompanyID,WarehouseId,UnitPrice,LastPriceDate FROM GoodsPurchaseLastPrice AS A WITH(NOLOCK)
                WHERE WarehouseId='").Append(warehouseId).Append(@"' 
            )#temp WHERE #temp.rowNum=1
            ");

            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, strbSql.ToString(), null))
            {
                while (rdr.Read())
                {
                    goodsPurchaseLastPriceInfoList.Add(new GoodsPurchaseLastPriceInfo
                    {
                        Id = new Guid(rdr["Id"].ToString()),
                        GoodsId = new Guid(rdr["GoodsId"].ToString()),
                        RealGoodsId = new Guid(rdr["RealGoodsId"].ToString()),
                        ThirdCompanyId = new Guid(rdr["ThirdCompanyID"].ToString()),
                        WarehouseId = new Guid(rdr["WarehouseId"].ToString()),
                        UnitPrice = decimal.Parse(rdr["UnitPrice"].ToString()),
                        LastPriceDate = DateTime.Parse(rdr["LastPriceDate"].ToString())
                    });
                }
            }
            return goodsPurchaseLastPriceInfoList;
        }

        /// <summary>更新出入库单据状态和描述
        /// </summary>
        /// ADD ww  
        /// 2016-08-01
        /// <param name="stockId">出入库单据ID</param>
        /// <param name="state">单据状态</param>
        /// <param name="description">描述</param>
        public bool NewSetStateStorageRecord(Guid stockId, StorageRecordState state, string description)
        {
            const string SQL = @"Update StorageRecord SET StockState=@StockState,[Description]=[Description]+@Description ";
            var strSql = new StringBuilder(SQL);
            if (state == StorageRecordState.Finished)
            {
                strSql.Append(",AuditTime=GETDATE() ");
            }
            strSql.Append(" WHERE StockId=@StockId");
            var parm = new[]{
                              new SqlParameter("@StockId",stockId),
                              new SqlParameter("@StockState", (int)state),
                              new SqlParameter("@Description",description)
                                       };
            try
            {
                return SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, strSql.ToString(), parm) > 0;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// 更新出入库单据状态、单据总额、描述
        /// </summary>
        /// <param name="stockId">出入库单据ID</param>
        /// <param name="state">单据状态</param>
        /// <param name="accountReceivable">单据总额</param>
        /// <param name="description">描述</param>
        /// zal 2017-08-17
        public bool SetStateAndAccountReceivableForStorageRecord(Guid stockId, StorageRecordState state, decimal accountReceivable, string description)
        {
            const string SQL = @"Update StorageRecord SET StockState=@StockState,AccountReceivable=@AccountReceivable,[Description]=[Description]+@Description ";
            var strSql = new StringBuilder(SQL);
            if (state == StorageRecordState.Finished)
            {
                strSql.Append(",AuditTime=GETDATE() ");
            }
            strSql.Append(" WHERE StockId=@StockId");
            var parm = new[]{
                new SqlParameter("@StockId",stockId),
                new SqlParameter("@StockState", (int)state),
                new SqlParameter("@AccountReceivable", accountReceivable),
                new SqlParameter("@Description",description)
            };
            try
            {
                return SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, strSql.ToString(), parm) > 0;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }


        /// <summary>
        /// 更新出入库记录和出入库库明细
        /// </summary>
        /// ADD ww 
        /// 2016-07-29
        /// <param name="storageRecordInfo">出入库记录</param>
        /// <param name="storageRecordDetailInfoList">出入库明细</param>
        public void NewUpdateStockAndGoods(StorageRecordInfo storageRecordInfo, IList<StorageRecordDetailInfo> storageRecordDetailInfoList)
        {
            bool isOut = storageRecordInfo.StockType == (int)StorageRecordType.LendOut ||
                         storageRecordInfo.StockType == (int)StorageRecordType.AfterSaleOut ||
                         storageRecordInfo.StockType == (int)StorageRecordType.BuyStockOut ||
                         storageRecordInfo.StockType == (int)StorageRecordType.SellStockOut ||
                         storageRecordInfo.StockType == (int)StorageRecordType.BorrowOut;
            foreach (var storageRecordDetailInfo in storageRecordDetailInfoList)
            {
                storageRecordDetailInfo.Quantity = storageRecordDetailInfo.Quantity == 0
                                                               ? 0
                                                               : isOut ? -Math.Abs(storageRecordDetailInfo.Quantity)
                                                               : Math.Abs(storageRecordDetailInfo.Quantity);

                storageRecordDetailInfo.UnitPrice = Math.Abs(storageRecordDetailInfo.UnitPrice);
            }

            storageRecordInfo.SubtotalQuantity = storageRecordDetailInfoList.Sum(ent => ent.Quantity);
            storageRecordInfo.AccountReceivable = storageRecordDetailInfoList.Sum(ent => ent.Quantity * ent.UnitPrice);


            var parms = new[]
                {
                    new SqlParameter("@StockId", storageRecordInfo.StockId),
                    new SqlParameter("@FilialeId", storageRecordInfo.FilialeId),
                    new SqlParameter("@ThirdCompanyID", storageRecordInfo.ThirdCompanyID),
                    new SqlParameter("@WarehouseId", storageRecordInfo.WarehouseId),
                    new SqlParameter("@LinkTradeCode", storageRecordInfo.LinkTradeCode),
                    new SqlParameter("@Description", storageRecordInfo.Description),
                    new SqlParameter("@AccountReceivable", Math.Round(storageRecordInfo.AccountReceivable, 2)),
                    new SqlParameter("@SubtotalQuantity", storageRecordInfo.SubtotalQuantity),
                    new SqlParameter("@StockState", storageRecordInfo.StockState),
                    new SqlParameter("@LinkTradeID", storageRecordInfo.LinkTradeID == Guid.Empty ? (object)DBNull.Value : storageRecordInfo.LinkTradeID),
                    new SqlParameter("@StorageType", storageRecordInfo.StorageType),
                    new SqlParameter("@BillNo",storageRecordInfo.BillNo??""),

                };

            using (var conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();
                SqlTransaction trans = conn.BeginTransaction();
                try
                {
                    var deleteparms = new[]
                                          {
                                              new SqlParameter("@StockId",storageRecordInfo.StockId)
                                          };
                    const string SQL_DELETE_SEMIGOODSSTOCK_BY_STOCKID = " DELETE FROM StorageRecordDetail WHERE StockId=@StockId ";
                    const string SQL_UPDATE_SEMISTOCK = "UPDATE StorageRecord SET [FilialeId]=@FilialeId, [ThirdCompanyID]=@ThirdCompanyID, [WarehouseId]=@WarehouseId, [LinkTradeCode]=@LinkTradeCode, [Description]=@Description, [AccountReceivable]=@AccountReceivable, [SubtotalQuantity]=@SubtotalQuantity, [StockState]=@StockState, [LinkTradeID]=@LinkTradeID, [StorageType]=@StorageType,[BillNo]=@BillNo WHERE [StockId]=@StockId";

                    const string SQL_UPDATE_SEMIGOODSSTOCK = @"
IF EXISTS (SELECT [StockId] FROM StorageRecordDetail WHERE [StockId]=@StockId AND [RealGoodsId]=@RealGoodsId AND [UnitPrice]=@UnitPrice)
BEGIN
	UPDATE StorageRecordDetail set [Quantity]=@Quantity WHERE [StockId]=@StockId AND [RealGoodsId]=@RealGoodsId AND [UnitPrice]=@UnitPrice
END
ELSE
BEGIN
	INSERT INTO StorageRecordDetail([StockId],[GoodsId],[RealGoodsId],[Specification],[Quantity],[UnitPrice],[NonceWarehouseGoodsStock],[Description],[GoodsName],[GoodsCode],[JoinPrice],[GoodsType],[BatchNo],[EffectiveDate],[ShelfType]) 
    VALUES(@StockId,@GoodsId,@RealGoodsId,@Specification,@Quantity,@UnitPrice,@NonceWarehouseGoodsStock,@Description,@GoodsName,@GoodsCode,@JoinPrice,@GoodsType,@BatchNo,@EffectiveDate,@ShelfType);
END
";
                    SqlHelper.ExecuteNonQuery(trans, SQL_DELETE_SEMIGOODSSTOCK_BY_STOCKID, deleteparms);
                    SqlHelper.ExecuteNonQuery(trans, SQL_UPDATE_SEMISTOCK, parms);
                    foreach (StorageRecordDetailInfo storageRecordDetailInfo in storageRecordDetailInfoList)
                    {
                        var parmsDetail = new[]
                        {
                            new SqlParameter("@StockId", storageRecordInfo.StockId),
                            new SqlParameter("@RealGoodsId", storageRecordDetailInfo.RealGoodsId),
                            new SqlParameter("@GoodsName", storageRecordDetailInfo.GoodsName),
                            new SqlParameter("@GoodsCode", storageRecordDetailInfo.GoodsCode),
                            new SqlParameter("@UnitPrice", storageRecordDetailInfo.UnitPrice),
                            new SqlParameter("@Specification", storageRecordDetailInfo.Specification == "&nbsp;"
                                ? ""
                                : storageRecordDetailInfo.Specification),
                            new SqlParameter("@Quantity", storageRecordDetailInfo.Quantity),
                            new SqlParameter("@NonceWarehouseGoodsStock",
                                storageRecordDetailInfo.NonceWarehouseGoodsStock),
                            new SqlParameter("@Description", storageRecordDetailInfo.Description),
                            new SqlParameter("@GoodsId", storageRecordDetailInfo.GoodsId),
                            new SqlParameter("@JoinPrice", storageRecordDetailInfo.JoinPrice),
                            new SqlParameter("@GoodsType", storageRecordDetailInfo.GoodsType),
                            new SqlParameter("@BatchNo", storageRecordDetailInfo.BatchNo??""),
                            new SqlParameter("@EffectiveDate", storageRecordDetailInfo.EffectiveDate!=DateTime.MinValue?storageRecordDetailInfo.EffectiveDate:(object)DBNull.Value),
                            new SqlParameter("@ShelfType",storageRecordDetailInfo.ShelfType==Byte.MinValue?(object)DBNull.Value:storageRecordDetailInfo.ShelfType)
                        };
                        SqlHelper.ExecuteNonQuery(trans, SQL_UPDATE_SEMIGOODSSTOCK, parmsDetail);
                    }
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw new ApplicationException(ex.Message);
                }
            }
        }



        /// <summary>
        /// 新增出入库记录（含判断是否存在）
        /// ADD ww
        /// 2016-07-29
        /// </summary>
        /// <param name="storageRecordInfo">出入库记录</param>
        /// <param name="storageRecordDetailList">出入库详细记录</param>
        public bool NewSaveStoreRecord(StorageRecordInfo storageRecordInfo, IList<StorageRecordDetailInfo> storageRecordDetailList)
        {
            const string STORAGERECORDINFO_INTERORUPDATE = @"
IF EXISTS (SELECT TOP 1 StockId FROM StorageRecord WHERE [StockId]=@StockId)
    BEGIN
	    UPDATE StorageRecord SET AccountReceivable+=@AccountReceivable,SubtotalQuantity+=@SubtotalQuantity  WHERE [StockId]=@StockId;
    END
ELSE
    BEGIN
	    INSERT INTO [StorageRecord]
           ([StockId]           ,[FilialeId]           ,[ThirdCompanyID]           ,[WarehouseId]
           ,[TradeCode]           ,[LinkTradeCode]           ,[DateCreated]           ,[Transactor]
           ,[Description]           ,[AccountReceivable]           ,[SubtotalQuantity]           ,[StockType]
           ,[StockState]           ,[RelevanceFilialeId]           ,[RelevanceWarehouseId]           ,[LinkTradeID]
           ,[StockValidation]          ,[LinkTradeType]
           ,[IsOut],[AuditTime],[StorageType],[BillNo],[LogisticsCode]
           ,[TradeBothPartiesType])
     VALUES
           (@StockId           ,@FilialeId           ,@ThirdCompanyID           ,@WarehouseId
           ,@TradeCode           ,@LinkTradeCode           ,@DateCreated           ,@Transactor
           ,@Description           ,@AccountReceivable           ,@SubtotalQuantity           ,@StockType
           ,@StockState           ,@RelevanceFilialeId           ,@RelevanceWarehouseId           ,@LinkTradeID
           ,@StockValidation         ,@LinkTradeType
           ,@IsOut,@AuditTime,@StorageType,@BillNo,@LogisticsCode
           ,@TradeBothPartiesType);
    END
";

            const string STORAGERECORDDETAIINFO_INERTORUPDATE = @"
IF EXISTS(SELECT TOP 1 StockId FROM StorageRecordDetail WHERE [StockId]=@StockId AND [RealGoodsId]=@RealGoodsId AND [Specification]=@Specification AND [UnitPrice]=@UnitPrice)
    BEGIN
        UPDATE StorageRecordDetail
        SET 
			[Quantity]=@Quantity
			,[UnitPrice]=@UnitPrice
			,[NonceWarehouseGoodsStock]=@NonceWarehouseGoodsStock
			,[Description]=@Description
			,[GoodsName]=@GoodsName
			,[GoodsCode]=@GoodsCode
            ,[JoinPrice]=@JoinPrice
        WHERE [StockId]=@StockId AND [RealGoodsId]=@RealGoodsId AND [Specification]=@Specification AND  [UnitPrice]=@UnitPrice
    END
ELSE
    BEGIN
        INSERT INTO [StorageRecordDetail]
           ([ID],[StockId]           ,[RealGoodsId]           ,[GoodsName]           ,[GoodsCode]
           ,[UnitPrice]           ,[Specification]           ,[Quantity]           ,[NonceWarehouseGoodsStock]
           ,[Description]     ,[GoodsId]       ,[JoinPrice], [GoodsType],[BatchNo],[EffectiveDate],[ShelfType])
     VALUES
           (NEWID(),@StockId           ,@RealGoodsId           ,@GoodsName           ,@GoodsCode
           ,@UnitPrice           ,@Specification           ,@Quantity           ,@NonceWarehouseGoodsStock
           ,@Description     ,@GoodsId         ,@JoinPrice, @GoodsType,@BatchNo,@EffectiveDate,@ShelfType);
    END
";
            bool isOut = storageRecordInfo.StockType == (int)StorageRecordType.LendOut ||
                         storageRecordInfo.StockType == (int)StorageRecordType.AfterSaleOut ||
                         storageRecordInfo.StockType == (int)StorageRecordType.BuyStockOut ||
                         storageRecordInfo.StockType == (int)StorageRecordType.SellStockOut ||
                         storageRecordInfo.StockType == (int)StorageRecordType.BorrowOut;

            foreach (var storageRecordDetailInfo in storageRecordDetailList)
            {
                storageRecordDetailInfo.Quantity = storageRecordDetailInfo.Quantity == 0
                                                               ? 0
                                                               : isOut ? -Math.Abs(storageRecordDetailInfo.Quantity)
                                                               : Math.Abs(storageRecordDetailInfo.Quantity);
                storageRecordDetailInfo.UnitPrice = Math.Abs(storageRecordDetailInfo.UnitPrice);
            }
            storageRecordInfo.SubtotalQuantity = storageRecordDetailList.Sum(ent => ent.Quantity);
            storageRecordInfo.AccountReceivable = storageRecordDetailList.Sum(ent => ent.Quantity * ent.UnitPrice);

            var parms = new[]
                {
                    new Parameter("@StockId", storageRecordInfo.StockId),
                    new Parameter("@FilialeId", storageRecordInfo.FilialeId),
                    new Parameter("@ThirdCompanyID", storageRecordInfo.ThirdCompanyID),
                    new Parameter("@WarehouseId", storageRecordInfo.WarehouseId),
                    new Parameter("@TradeCode", storageRecordInfo.TradeCode),
                    new Parameter("@LinkTradeCode", storageRecordInfo.LinkTradeCode),
                    new Parameter("@DateCreated", storageRecordInfo.DateCreated),
                    new Parameter("@Transactor", storageRecordInfo.Transactor),
                    new Parameter("@Description", storageRecordInfo.Description),
                    new Parameter("@AccountReceivable", Math.Round(storageRecordInfo.AccountReceivable, 2)),
                    new Parameter("@SubtotalQuantity", storageRecordInfo.SubtotalQuantity),
                    new Parameter("@StockType", storageRecordInfo.StockType),
                    new Parameter("@StockState", storageRecordInfo.StockState),
                    new Parameter("@RelevanceFilialeId", storageRecordInfo.RelevanceFilialeId == Guid.Empty ? (object)DBNull.Value : storageRecordInfo.RelevanceFilialeId),
                    new Parameter("@RelevanceWarehouseId", storageRecordInfo.RelevanceWarehouseId == Guid.Empty ? (object)DBNull.Value : storageRecordInfo.RelevanceWarehouseId),
                    new Parameter("@LinkTradeID", storageRecordInfo.LinkTradeID == Guid.Empty ? (object)DBNull.Value : storageRecordInfo.LinkTradeID),
                    new Parameter("@StockValidation", storageRecordInfo.StockValidation),
                    new Parameter("@LinkTradeType", storageRecordInfo.LinkTradeType),
                    new Parameter("@IsOut", storageRecordInfo.IsOut),
                    new Parameter("@AuditTime", storageRecordInfo.StockState.Equals((int)StorageRecordState.Finished)?storageRecordInfo.DateCreated:(DateTime?)null),
                    new Parameter("@StorageType", storageRecordInfo.StorageType),
                    new Parameter("@BillNo",storageRecordInfo.BillNo??""),
                    new Parameter("@LogisticsCode", storageRecordInfo.LogisticsCode),
                    new Parameter("@TradeBothPartiesType", storageRecordInfo.TradeBothPartiesType)
                };

            bool result;
            using (var db = DatabaseFactory.Create())
            {
                db.BeginTransaction();
                result = db.Execute(false, STORAGERECORDINFO_INTERORUPDATE, parms);
                foreach (StorageRecordDetailInfo storageRecordDetailInfo in storageRecordDetailList)
                {
                    var parmsDetail = new[]
                    {
                        new Parameter("@StockId", storageRecordInfo.StockId),
                        new Parameter("@RealGoodsId", storageRecordDetailInfo.RealGoodsId),
                        new Parameter("@GoodsName", storageRecordDetailInfo.GoodsName),
                        new Parameter("@GoodsCode", storageRecordDetailInfo.GoodsCode),
                        new Parameter("@UnitPrice", storageRecordDetailInfo.UnitPrice),
                        new Parameter("@Specification", storageRecordDetailInfo.Specification == "&nbsp;"
                            ? ""
                            : storageRecordDetailInfo.Specification),
                        new Parameter("@Quantity", storageRecordDetailInfo.Quantity),
                        new Parameter("@NonceWarehouseGoodsStock", storageRecordDetailInfo.NonceWarehouseGoodsStock),
                        new Parameter("@Description", storageRecordDetailInfo.Description),
                        new Parameter("@GoodsId", storageRecordDetailInfo.GoodsId),
                        new Parameter("@JoinPrice", storageRecordDetailInfo.JoinPrice),
                        new Parameter("@GoodsType",storageRecordDetailInfo.GoodsType),
                        new Parameter("@BatchNo", storageRecordDetailInfo.BatchNo??""),
                        new Parameter("@EffectiveDate", storageRecordDetailInfo.EffectiveDate!=DateTime.MinValue?storageRecordDetailInfo.EffectiveDate:(object)DBNull.Value),
                        new Parameter("@ShelfType",storageRecordDetailInfo.ShelfType==Byte.MinValue?(object)DBNull.Value:storageRecordDetailInfo.ShelfType)
                    };
                    result = db.Execute(false, STORAGERECORDDETAIINFO_INERTORUPDATE, parmsDetail);
                }
                db.CompleteTransaction();
            }
            return result;
        }


        #region 无事物添加  外层必须添加事物
        public bool NewSaveStoreRecordNoTrans(StorageRecordInfo storageRecordInfo, IList<StorageRecordDetailInfo> storageRecordDetailList)
        {
            const string STORAGERECORDINFO_INTERORUPDATE = @"
IF EXISTS (SELECT TOP 1 StockId FROM StorageRecord WHERE [StockId]=@StockId)
    BEGIN
	    UPDATE StorageRecord SET AccountReceivable+=@AccountReceivable,SubtotalQuantity+=@SubtotalQuantity  WHERE [StockId]=@StockId;
    END
ELSE
    BEGIN
	    INSERT INTO [StorageRecord]
           ([StockId]           ,[FilialeId]           ,[ThirdCompanyID]           ,[WarehouseId]
           ,[TradeCode]           ,[LinkTradeCode]           ,[DateCreated]           ,[Transactor]
           ,[Description]           ,[AccountReceivable]           ,[SubtotalQuantity]           ,[StockType]
           ,[StockState]           ,[RelevanceFilialeId]           ,[RelevanceWarehouseId]           ,[LinkTradeID]
           ,[StockValidation]          ,[LinkTradeType]
           ,[IsOut],[AuditTime],[StorageType],[BillNo],[LogisticsCode]
           ,[TradeBothPartiesType])
     VALUES
           (@StockId           ,@FilialeId           ,@ThirdCompanyID           ,@WarehouseId
           ,@TradeCode           ,@LinkTradeCode           ,@DateCreated           ,@Transactor
           ,@Description           ,@AccountReceivable           ,@SubtotalQuantity           ,@StockType
           ,@StockState           ,@RelevanceFilialeId           ,@RelevanceWarehouseId           ,@LinkTradeID
           ,@StockValidation         ,@LinkTradeType
           ,@IsOut,@AuditTime,@StorageType,@BillNo,@LogisticsCode
           ,@TradeBothPartiesType);
    END
";

            const string STORAGERECORDDETAIINFO_INERTORUPDATE = @"
IF EXISTS(SELECT TOP 1 StockId FROM StorageRecordDetail WHERE [StockId]=@StockId AND [RealGoodsId]=@RealGoodsId AND [Specification]=@Specification AND [UnitPrice]=@UnitPrice)
    BEGIN
        UPDATE StorageRecordDetail
        SET 
			[Quantity]=@Quantity
			,[UnitPrice]=@UnitPrice
			,[NonceWarehouseGoodsStock]=@NonceWarehouseGoodsStock
			,[Description]=@Description
			,[GoodsName]=@GoodsName
			,[GoodsCode]=@GoodsCode
            ,[JoinPrice]=@JoinPrice
        WHERE [StockId]=@StockId AND [RealGoodsId]=@RealGoodsId AND [Specification]=@Specification AND  [UnitPrice]=@UnitPrice
    END
ELSE
    BEGIN
        INSERT INTO [StorageRecordDetail]
           ([ID],[StockId]           ,[RealGoodsId]           ,[GoodsName]           ,[GoodsCode]
           ,[UnitPrice]           ,[Specification]           ,[Quantity]           ,[NonceWarehouseGoodsStock]
           ,[Description]     ,[GoodsId]       ,[JoinPrice], [GoodsType],[BatchNo],[EffectiveDate],[ShelfType])
     VALUES
           (NEWID(),@StockId           ,@RealGoodsId           ,@GoodsName           ,@GoodsCode
           ,@UnitPrice           ,@Specification           ,@Quantity           ,@NonceWarehouseGoodsStock
           ,@Description     ,@GoodsId         ,@JoinPrice, @GoodsType,@BatchNo,@EffectiveDate,@ShelfType);
    END
";
            bool isOut = storageRecordInfo.StockType == (int)StorageRecordType.LendOut ||
                         storageRecordInfo.StockType == (int)StorageRecordType.AfterSaleOut ||
                         storageRecordInfo.StockType == (int)StorageRecordType.BuyStockOut ||
                         storageRecordInfo.StockType == (int)StorageRecordType.SellStockOut ||
                         storageRecordInfo.StockType == (int)StorageRecordType.BorrowOut;

            foreach (var storageRecordDetailInfo in storageRecordDetailList)
            {
                storageRecordDetailInfo.Quantity = storageRecordDetailInfo.Quantity == 0
                                                               ? 0
                                                               : isOut ? -Math.Abs(storageRecordDetailInfo.Quantity)
                                                               : Math.Abs(storageRecordDetailInfo.Quantity);
                storageRecordDetailInfo.UnitPrice = Math.Abs(storageRecordDetailInfo.UnitPrice);
            }
            storageRecordInfo.SubtotalQuantity = storageRecordDetailList.Sum(ent => ent.Quantity);
            storageRecordInfo.AccountReceivable = storageRecordDetailList.Sum(ent => ent.Quantity * ent.UnitPrice);

            var parms = new[]
                {
                    new Parameter("@StockId", storageRecordInfo.StockId),
                    new Parameter("@FilialeId", storageRecordInfo.FilialeId),
                    new Parameter("@ThirdCompanyID", storageRecordInfo.ThirdCompanyID),
                    new Parameter("@WarehouseId", storageRecordInfo.WarehouseId),
                    new Parameter("@TradeCode", storageRecordInfo.TradeCode),
                    new Parameter("@LinkTradeCode", storageRecordInfo.LinkTradeCode),
                    new Parameter("@DateCreated", storageRecordInfo.DateCreated),
                    new Parameter("@Transactor", storageRecordInfo.Transactor),
                    new Parameter("@Description", storageRecordInfo.Description),
                    new Parameter("@AccountReceivable", Math.Round(storageRecordInfo.AccountReceivable, 2)),
                    new Parameter("@SubtotalQuantity", storageRecordInfo.SubtotalQuantity),
                    new Parameter("@StockType", storageRecordInfo.StockType),
                    new Parameter("@StockState", storageRecordInfo.StockState),
                    new Parameter("@RelevanceFilialeId", storageRecordInfo.RelevanceFilialeId == Guid.Empty ? (object)DBNull.Value : storageRecordInfo.RelevanceFilialeId),
                    new Parameter("@RelevanceWarehouseId", storageRecordInfo.RelevanceWarehouseId == Guid.Empty ? (object)DBNull.Value : storageRecordInfo.RelevanceWarehouseId),
                    new Parameter("@LinkTradeID", storageRecordInfo.LinkTradeID == Guid.Empty ? (object)DBNull.Value : storageRecordInfo.LinkTradeID),
                    new Parameter("@StockValidation", storageRecordInfo.StockValidation),
                    new Parameter("@LinkTradeType", storageRecordInfo.LinkTradeType),
                    new Parameter("@IsOut", storageRecordInfo.IsOut),
                    new Parameter("@AuditTime", storageRecordInfo.StockState.Equals((int)StorageRecordState.Finished)?storageRecordInfo.DateCreated:(DateTime?)null),
                    new Parameter("@StorageType", storageRecordInfo.StorageType),
                    new Parameter("@BillNo",storageRecordInfo.BillNo??""),
                    new Parameter("@LogisticsCode", storageRecordInfo.LogisticsCode),
                    new Parameter("@TradeBothPartiesType", storageRecordInfo.TradeBothPartiesType)
                };

            bool result;
            using (var db = DatabaseFactory.Create())
            {
                result = db.Execute(false, STORAGERECORDINFO_INTERORUPDATE, parms);
                foreach (StorageRecordDetailInfo storageRecordDetailInfo in storageRecordDetailList)
                {
                    var parmsDetail = new[]
                    {
                        new Parameter("@StockId", storageRecordInfo.StockId),
                        new Parameter("@RealGoodsId", storageRecordDetailInfo.RealGoodsId),
                        new Parameter("@GoodsName", storageRecordDetailInfo.GoodsName),
                        new Parameter("@GoodsCode", storageRecordDetailInfo.GoodsCode),
                        new Parameter("@UnitPrice", storageRecordDetailInfo.UnitPrice),
                        new Parameter("@Specification", storageRecordDetailInfo.Specification == "&nbsp;"
                            ? ""
                            : storageRecordDetailInfo.Specification),
                        new Parameter("@Quantity", storageRecordDetailInfo.Quantity),
                        new Parameter("@NonceWarehouseGoodsStock", storageRecordDetailInfo.NonceWarehouseGoodsStock),
                        new Parameter("@Description", storageRecordDetailInfo.Description),
                        new Parameter("@GoodsId", storageRecordDetailInfo.GoodsId),
                        new Parameter("@JoinPrice", storageRecordDetailInfo.JoinPrice),
                        new Parameter("@GoodsType",storageRecordDetailInfo.GoodsType),
                        new Parameter("@BatchNo", storageRecordDetailInfo.BatchNo??""),
                        new Parameter("@EffectiveDate", storageRecordDetailInfo.EffectiveDate!=DateTime.MinValue?storageRecordDetailInfo.EffectiveDate:(object)DBNull.Value),
                        new Parameter("@ShelfType",storageRecordDetailInfo.ShelfType==Byte.MinValue?(object)DBNull.Value:storageRecordDetailInfo.ShelfType)
                    };
                    result = db.Execute(false, STORAGERECORDDETAIINFO_INERTORUPDATE, parmsDetail);
                }
            }
            return result;
        }

        /// <summary>
        /// 添加出入库记录（明细采用SQL批量插入）
        /// </summary>
        /// <param name="storageRecordInfo"></param>
        /// <param name="storageRecordDetailList"></param>
        public bool AddStorageRecord(StorageRecordInfo storageRecordInfo, IList<StorageRecordDetailInfo> storageRecordDetailList)
        {
            const string STORAGERECORDINFO_INTERORUPDATE = @"
INSERT INTO [StorageRecord]
    ([StockId]           ,[FilialeId]           ,[ThirdCompanyID]           ,[WarehouseId]
    ,[TradeCode]           ,[LinkTradeCode]           ,[DateCreated]           ,[Transactor]
    ,[Description]           ,[AccountReceivable]           ,[SubtotalQuantity]           ,[StockType]
    ,[StockState]           ,[RelevanceFilialeId]           ,[RelevanceWarehouseId]           ,[LinkTradeID]
    ,[StockValidation]          ,[LinkTradeType]
    ,[IsOut],[AuditTime],[StorageType],[BillNo],[LogisticsCode]
    ,[TradeBothPartiesType])
VALUES
    (@StockId           ,@FilialeId           ,@ThirdCompanyID           ,@WarehouseId
    ,@TradeCode           ,@LinkTradeCode           ,@DateCreated           ,@Transactor
    ,@Description           ,@AccountReceivable           ,@SubtotalQuantity           ,@StockType
    ,@StockState           ,@RelevanceFilialeId           ,@RelevanceWarehouseId           ,@LinkTradeID
    ,@StockValidation         ,@LinkTradeType
    ,@IsOut,@AuditTime,@StorageType,@BillNo,@LogisticsCode
    ,@TradeBothPartiesType);
";

            bool isOut = storageRecordInfo.StockType == (int)StorageRecordType.LendOut ||
                         storageRecordInfo.StockType == (int)StorageRecordType.AfterSaleOut ||
                         storageRecordInfo.StockType == (int)StorageRecordType.BuyStockOut ||
                         storageRecordInfo.StockType == (int)StorageRecordType.SellStockOut ||
                         storageRecordInfo.StockType == (int)StorageRecordType.BorrowOut;

            foreach (var storageRecordDetailInfo in storageRecordDetailList)
            {
                storageRecordDetailInfo.Quantity = storageRecordDetailInfo.Quantity == 0
                    ? 0
                    : isOut ? -Math.Abs(storageRecordDetailInfo.Quantity)
                        : Math.Abs(storageRecordDetailInfo.Quantity);
                storageRecordDetailInfo.UnitPrice = Math.Abs(storageRecordDetailInfo.UnitPrice);

                if (storageRecordDetailInfo.Specification == "&nbsp;")
                {
                    storageRecordDetailInfo.Specification = "";
                }
                if (storageRecordDetailInfo.EffectiveDate == DateTime.MinValue)
                {
                    storageRecordDetailInfo.EffectiveDate = new DateTime(1900, 1, 1);
                }
            }
            storageRecordInfo.SubtotalQuantity = storageRecordDetailList.Sum(ent => ent.Quantity);
            storageRecordInfo.AccountReceivable = storageRecordDetailList.Sum(ent => ent.Quantity * ent.UnitPrice);

            var parms = new SqlParameter[]
            {
                new SqlParameter("@StockId", storageRecordInfo.StockId),
                new SqlParameter("@FilialeId", storageRecordInfo.FilialeId),
                new SqlParameter("@ThirdCompanyID", storageRecordInfo.ThirdCompanyID),
                new SqlParameter("@WarehouseId", storageRecordInfo.WarehouseId),
                new SqlParameter("@TradeCode", storageRecordInfo.TradeCode),
                new SqlParameter("@LinkTradeCode", storageRecordInfo.LinkTradeCode),
                new SqlParameter("@DateCreated", storageRecordInfo.DateCreated),
                new SqlParameter("@Transactor", storageRecordInfo.Transactor),
                new SqlParameter("@Description", storageRecordInfo.Description),
                new SqlParameter("@AccountReceivable", Math.Round(storageRecordInfo.AccountReceivable, 2)),
                new SqlParameter("@SubtotalQuantity", storageRecordInfo.SubtotalQuantity),
                new SqlParameter("@StockType", storageRecordInfo.StockType),
                new SqlParameter("@StockState", storageRecordInfo.StockState),
                new SqlParameter("@RelevanceFilialeId", storageRecordInfo.RelevanceFilialeId == Guid.Empty ? (object)DBNull.Value : storageRecordInfo.RelevanceFilialeId),
                new SqlParameter("@RelevanceWarehouseId", storageRecordInfo.RelevanceWarehouseId == Guid.Empty ? (object)DBNull.Value : storageRecordInfo.RelevanceWarehouseId),
                new SqlParameter("@LinkTradeID", storageRecordInfo.LinkTradeID == Guid.Empty ? (object)DBNull.Value : storageRecordInfo.LinkTradeID),
                new SqlParameter("@StockValidation", storageRecordInfo.StockValidation),
                new SqlParameter("@LinkTradeType", storageRecordInfo.LinkTradeType),
                new SqlParameter("@IsOut", storageRecordInfo.IsOut),
                new SqlParameter("@AuditTime", storageRecordInfo.StockState.Equals((int)StorageRecordState.Finished)?storageRecordInfo.DateCreated:(DateTime?)null),
                new SqlParameter("@StorageType", storageRecordInfo.StorageType),
                new SqlParameter("@BillNo",storageRecordInfo.BillNo??""),
                new SqlParameter("@LogisticsCode", storageRecordInfo.LogisticsCode),
                new SqlParameter("@TradeBothPartiesType", storageRecordInfo.TradeBothPartiesType)
            };

            var result = true;
            using (var conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        SqlHelper.ExecuteNonQuery(trans, STORAGERECORDINFO_INTERORUPDATE, parms);
                        result = SqlHelper.BatchInsert(trans, storageRecordDetailList, "StorageRecordDetail", _storageRecordDetailMappings) > 0;
                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
            return result;
        }
        #endregion

        /// <summary>
        /// 进货/出货 单据核退（更新ERP入库单据状态）。
        /// </summary>
        /// <param name="no">单号</param>
        /// <param name="description">描述</param>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-06-28
        public bool RefuseInorOutGoodsBill(string no, string description)
        {
            var list = GetInnerPurchaseRelationInfoRefuse(no);
            if (list == null)
            {
                return false;
            }
            const string SQL = @"IF EXISTS(SELECT TOP 1 T.OutStockId FROM StorageRecord S
INNER JOIN InnerPurchaseRelation T ON T.OutStockId = S.StockId
WHERE S.TradeCode = @TradeCode)
    BEGIN
         DELETE FROM lmShop_Purchasing WHERE PurchasingID = @PurchasingId;
         DELETE FROM lmShop_PurchasingDetail WHERE PurchasingID = @PurchasingId;
         DELETE FROM StorageRecordDetail WHERE StockId = @InStockId;
         DELETE FROM StorageRecord WHERE StockId = @InStockId;
         Update StorageRecord SET LinkTradeCode = @LinkTradeCode,LinkTradeID = @LinkTradeID,StockState=@StockState,[Description]=[Description]+@Description WHERE TradeCode=@TradeCode;
    END
ELSE
    BEGIN
        Update StorageRecord SET StockState=@StockState,[Description]=[Description]+@Description WHERE TradeCode=@TradeCode;
    END";
            //const string SQL = @"Update StorageRecord SET StockState=@StockState ";
            var strSql = new StringBuilder(SQL);
            //strSql.Append(",[Description]=[Description]+@Description ");
            //strSql.Append(" WHERE TradeCode=@TradeCode");
            var parms = new[]{
                new SqlParameter("@PurchasingId",SqlDbType.UniqueIdentifier){Value = list.PurchasingId},
                new SqlParameter("@InStockId",SqlDbType.UniqueIdentifier){Value = list.InStockId},
                new SqlParameter("@TradeCode",SqlDbType.VarChar){Value = no},
                new SqlParameter("@StockState", SqlDbType.Int){Value = (Int32)StorageRecordState.Refuse},
                new SqlParameter("@LinkTradeID", SqlDbType.UniqueIdentifier){Value = Guid.Empty},
                new SqlParameter("@LinkTradeCode", SqlDbType.VarChar){Value = String.Empty},
                new SqlParameter("@Description",SqlDbType.VarChar){Value = description}};

            try
            {
                return SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, strSql.ToString(), parms) > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 更新出入库记录采购单
        /// </summary>
        /// ADD ww 
        /// 2016-07-29
        /// <param name="storageRecordInfo">出入库记录</param>
        /// <param name="storageRecordDetailInfoList">出入库明细</param>
        public bool UpdateStockPurchse(StorageRecordInfo storageRecordInfo)
        {
            const string SQL = @"Update StorageRecord SET LinkTradeCode=@LinkTradeCode,LinkTradeID=@LinkTradeID,LinkTradeType=@LinkTradeType ";
            var strSql = new StringBuilder(SQL);
            strSql.Append(" WHERE StockId=@StockId");
            var parms = new[]{
                new SqlParameter("@LinkTradeCode", SqlDbType.VarChar){Value = storageRecordInfo.LinkTradeCode},
                new SqlParameter("@LinkTradeID", SqlDbType.UniqueIdentifier){Value = storageRecordInfo.LinkTradeID},
                new SqlParameter("@LinkTradeType", SqlDbType.Int){Value = storageRecordInfo.LinkTradeType},
                new SqlParameter("@StockId",SqlDbType.UniqueIdentifier){Value = storageRecordInfo.StockId}};
            try
            {
                return SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, strSql.ToString(), parms) > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool UpdateStorageState(string tradeCode, int storageState, string description, out string errorMsg)
        {
            const string SQL = @"Update StorageRecord SET StockState=@StockState ";
            var strSql = new StringBuilder(SQL);
            strSql.Append(",[Description]=[Description]+@Description ");
            strSql.Append(" WHERE TradeCode=@TradeCode");
            var parms = new[]{
                new SqlParameter("@StockState", SqlDbType.Int){Value = storageState},
                new SqlParameter("@TradeCode", SqlDbType.VarChar){Value = tradeCode},
                new SqlParameter("@Description",SqlDbType.VarChar){Value = description}};
            try
            {
                errorMsg = "";
                return SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, strSql.ToString(), parms) > 0;
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                return false;
            }
        }

        /// <summary>新增出入库单据含明细
        /// </summary>
        /// <param name="storageRecordInfo">出入库单据模型</param>
        /// <param name="storageRecordDetailList">出入库单据详细模型</param>
        /// <param name="errorMessage">异常信息</param>
        public bool Insert(StorageRecordInfo storageRecordInfo, IList<StorageRecordDetailInfo> storageRecordDetailList, out string errorMessage)
        {
            if (storageRecordInfo == null)
            {
                errorMessage = ("出入库单据对象不能为空！");
                return false;
            }

            //新增出入库
            const string SQL_INSERT_STORAGERECORD = @"INSERT INTO [StorageRecord]
           ([StockId]           ,[FilialeId]           ,[ThirdCompanyID]           ,[WarehouseId]
           ,[TradeCode]           ,[LinkTradeCode]           ,[DateCreated]           ,[Transactor]
           ,[Description]           ,[AccountReceivable]           ,[SubtotalQuantity]           ,[StockType]
           ,[StockState]           ,[RelevanceFilialeId]           ,[RelevanceWarehouseId]           ,[LinkTradeID]
           ,[StockValidation]        ,[LinkTradeType]
           ,[IsOut],[AuditTime],[StorageType],[BillNo]
           ,[TradeBothPartiesType])
     VALUES
           (@StockId           ,@FilialeId           ,@ThirdCompanyID           ,@WarehouseId
           ,@TradeCode           ,@LinkTradeCode           ,@DateCreated           ,@Transactor
           ,@Description           ,@AccountReceivable           ,@SubtotalQuantity           ,@StockType
           ,@StockState           ,@RelevanceFilialeId           ,@RelevanceWarehouseId           ,@LinkTradeID
           ,@StockValidation        ,@LinkTradeType
           ,@IsOut,@AuditTime,@StorageType,@BillNo
           ,@TradeBothPartiesType)";

            //新增出入库明细
            const string SQL_INSERT_STORAGERECORDDETAIL = @"INSERT INTO [StorageRecordDetail]
           ([StockId]           ,[RealGoodsId]           ,[GoodsName]           ,[GoodsCode]
           ,[UnitPrice]           ,[Specification]           ,[Quantity]           ,[NonceWarehouseGoodsStock]
           ,[Description]          ,[GoodsId]        ,[JoinPrice],[GoodsType],[BatchNo],[EffectiveDate],[ShelfType])
     VALUES
           (@StockId           ,@RealGoodsId           ,@GoodsName           ,@GoodsCode
           ,@UnitPrice           ,@Specification           ,@Quantity           ,@NonceWarehouseGoodsStock
           ,@Description           ,@GoodsId          ,@JoinPrice,@GoodsType,@BatchNo,@EffectiveDate,@ShelfType)";

            bool isOut = storageRecordInfo.StockType == (int)StorageRecordType.LendOut ||
                         storageRecordInfo.StockType == (int)StorageRecordType.AfterSaleOut ||
                         storageRecordInfo.StockType == (int)StorageRecordType.BuyStockOut ||
                         storageRecordInfo.StockType == (int)StorageRecordType.SellStockOut ||
                         storageRecordInfo.StockType == (int)StorageRecordType.BorrowOut;
            foreach (var storageRecordDetailInfo in storageRecordDetailList)
            {
                storageRecordDetailInfo.Quantity = storageRecordDetailInfo.Quantity == 0
                                                               ? 0
                                                               : isOut ? -Math.Abs(storageRecordDetailInfo.Quantity)
                                                               : Math.Abs(storageRecordDetailInfo.Quantity);

                storageRecordDetailInfo.UnitPrice = Math.Abs(storageRecordDetailInfo.UnitPrice);
            }

            storageRecordInfo.SubtotalQuantity = storageRecordDetailList.Sum(ent => ent.Quantity);
            storageRecordInfo.AccountReceivable = storageRecordDetailList.Sum(ent => ent.Quantity * ent.UnitPrice);

            errorMessage = string.Empty;
            using (var db = DatabaseFactory.Create())
            {
                db.BeginTransaction();

                #region [插入出入记录]
                var parms = new[]
                {
                    new Parameter("@StockId", storageRecordInfo.StockId),
                    new Parameter("@FilialeId", storageRecordInfo.FilialeId),
                    new Parameter("@ThirdCompanyID", storageRecordInfo.ThirdCompanyID),
                    new Parameter("@WarehouseId", storageRecordInfo.WarehouseId),
                    new Parameter("@TradeCode", storageRecordInfo.TradeCode),
                    new Parameter("@LinkTradeCode", storageRecordInfo.LinkTradeCode),
                    new Parameter("@DateCreated", storageRecordInfo.DateCreated),
                    new Parameter("@Transactor", storageRecordInfo.Transactor),
                    new Parameter("@Description", storageRecordInfo.Description),
                    new Parameter("@AccountReceivable", Math.Round(storageRecordInfo.AccountReceivable, 2)),
                    new Parameter("@SubtotalQuantity", storageRecordInfo.SubtotalQuantity),
                    new Parameter("@StockType", storageRecordInfo.StockType),
                    new Parameter("@StockState", storageRecordInfo.StockState),
                    new Parameter("@RelevanceFilialeId", storageRecordInfo.RelevanceFilialeId == Guid.Empty ? (object)DBNull.Value : storageRecordInfo.RelevanceFilialeId),
                    new Parameter("@RelevanceWarehouseId", storageRecordInfo.RelevanceWarehouseId == Guid.Empty ? (object)DBNull.Value : storageRecordInfo.RelevanceWarehouseId),
                    new Parameter("@LinkTradeID", storageRecordInfo.LinkTradeID == Guid.Empty ? (object)DBNull.Value : storageRecordInfo.LinkTradeID),
                    new Parameter("@StockValidation", storageRecordInfo.StockValidation),
                    new Parameter("@LinkTradeType", storageRecordInfo.LinkTradeType),
                    new Parameter("@IsOut", storageRecordInfo.IsOut),
                    new Parameter("@AuditTime", storageRecordInfo.StockState.Equals((int)StorageRecordState.Finished)?storageRecordInfo.DateCreated:(DateTime?)null),
                    new Parameter("@StorageType", storageRecordInfo.StorageType),
                    new Parameter("@BillNo",storageRecordInfo.BillNo??""),
                    new Parameter("@TradeBothPartiesType", storageRecordInfo.TradeBothPartiesType)
                };

                db.Execute(false, SQL_INSERT_STORAGERECORD, parms);
                #endregion

                #region [插入出入库明细]

                foreach (var storageRecordDetailInfo in storageRecordDetailList)
                {
                    var parmsDetail = new[]
                    {
                        new Parameter("@StockId", storageRecordDetailInfo.StockId),
                        new Parameter("@RealGoodsId", storageRecordDetailInfo.RealGoodsId),
                        new Parameter("@GoodsName", storageRecordDetailInfo.GoodsName),
                        new Parameter("@GoodsCode", storageRecordDetailInfo.GoodsCode),
                        new Parameter("@UnitPrice", storageRecordDetailInfo.UnitPrice),
                        new Parameter("@Specification",
                            storageRecordDetailInfo.Specification == "&nbsp;"
                                ? ""
                                : storageRecordDetailInfo.Specification),
                        new Parameter("@Quantity", storageRecordDetailInfo.Quantity),
                        new Parameter("@NonceWarehouseGoodsStock", storageRecordDetailInfo.NonceWarehouseGoodsStock),
                        new Parameter("@Description", storageRecordDetailInfo.Description),
                        new Parameter("@GoodsId", storageRecordDetailInfo.GoodsId),
                        new Parameter("@JoinPrice", storageRecordDetailInfo.JoinPrice),
                        new Parameter("@GoodsType", storageRecordDetailInfo.GoodsType),
                        new Parameter("@BatchNo", storageRecordDetailInfo.BatchNo??""),
                        new Parameter("@EffectiveDate", storageRecordDetailInfo.EffectiveDate!=DateTime.MinValue?storageRecordDetailInfo.EffectiveDate:(object)DBNull.Value),
                        new Parameter("@ShelfType",storageRecordDetailInfo.ShelfType==Byte.MinValue?(object)DBNull.Value:storageRecordDetailInfo.ShelfType)
                    };

                    db.Execute(false, SQL_INSERT_STORAGERECORDDETAIL, parmsDetail);
                }

                #endregion

                return db.CompleteTransaction();
            }
        }

        /// <summary>
        /// 获取商品的未出库数(商品需求查询页面使用)
        /// ADD ww
        /// 2016-08-17
        /// </summary>
        /// <param name="goodsId">商品id</param>
        /// <param name="storageRecordTypes">入库类型</param>
        /// <param name="storageRecordStates">入库状态</param>
        /// <param name="filialeId">公司id</param>
        /// <param name="warehouseId">仓库id</param>
        /// <returns></returns>
        public int GetNotOutQuantity(Guid goodsId, Guid filialeId, Guid warehouseId,
            List<StorageRecordType> storageRecordTypes, List<StorageRecordState> storageRecordStates)
        {
            var strStorageRecordTypes = storageRecordTypes.Aggregate(String.Empty,
                (ent, type) => ent + ((int)type + ","));
            var strStorageRecordState = storageRecordStates.Aggregate(String.Empty,
                (ent, type) => ent + ((int)type + ","));

            var strbSql = new StringBuilder();
            strbSql.Append(
                "select isnull(sum(Quantity),0) from StorageRecordDetail a inner join StorageRecord b on a.stockId=b.stockId");
            strbSql.Append(" WHERE a.RealGoodsId='").Append(goodsId).Append("'");
            strbSql.Append(" AND b.WarehouseId='").Append(warehouseId).Append("'");
            strbSql.Append(" AND b.FilialeId='").Append(filialeId).Append("'");
            strbSql.Append(" AND b.StockState IN (")
                .Append(strStorageRecordState.Substring(0, strStorageRecordState.Length - 1))
                .Append(")");
            strbSql.Append(" AND b.StockType IN (")
                .Append(strStorageRecordTypes.Substring(0, strStorageRecordTypes.Length - 1))
                .Append(")");
            var result = SqlHelper.ExecuteScalar(GlobalConfig.ERP_DB_NAME, true, strbSql.ToString(), null);
            return int.Parse(result.ToString());
        }


        /// <summary>
        /// 获取商品的未出库数(商品需求查询页面使用)
        /// </summary>
        /// <param name="realGoodsIdIds">子商品id</param>
        /// <param name="storageRecordTypes">入库类型</param>
        /// <param name="storageRecordStates">入库状态</param>
        /// <param name="filialeId">公司id</param>
        /// <param name="warehouseId">仓库id</param>
        /// <returns></returns>
        /// zal 2017-03-03
        public Dictionary<Guid, int> GetDicNotOutQuantity(List<Guid> realGoodsIdIds, Guid filialeId, Guid warehouseId,
            List<StorageRecordType> storageRecordTypes, List<StorageRecordState> storageRecordStates)
        {
            var strStorageRecordTypes = storageRecordTypes.Aggregate(String.Empty,
                (ent, type) => ent + ((int)type + ","));
            var strStorageRecordState = storageRecordStates.Aggregate(String.Empty,
                (ent, type) => ent + ((int)type + ","));

            var realGoodsIdsStr = string.Format("SELECT id AS RealGoodsId FROM splitToTable('{0}',',')", string.Join(",", realGoodsIdIds.ToArray()));
            var strbSql = new StringBuilder();
            strbSql.Append("SELECT a.RealGoodsId,ISNULL(SUM(Quantity),0) AS SumQuantity  FROM StorageRecordDetail a WITH(NOLOCK) INNER JOIN StorageRecord b WITH(NOLOCK) ON a.stockId=b.stockId");
            strbSql.Append(" INNER JOIN(").Append(realGoodsIdsStr).Append(")t ON a.RealGoodsId=t.RealGoodsId");
            strbSql.Append(" WHERE 1=1 ");
            strbSql.Append(" AND b.WarehouseId='").Append(warehouseId).Append("'");
            strbSql.Append(" AND b.FilialeId='").Append(filialeId).Append("'");
            strbSql.Append(" AND b.StockState IN (")
                .Append(strStorageRecordState.Substring(0, strStorageRecordState.Length - 1))
                .Append(")");
            strbSql.Append(" AND b.StockType IN (")
                .Append(strStorageRecordTypes.Substring(0, strStorageRecordTypes.Length - 1))
                .Append(")");
            strbSql.Append(" GROUP BY a.RealGoodsId");

            var dicRealGoodsIdAndSumQuantity = new Dictionary<Guid, int>();
            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, strbSql.ToString(), null))
            {
                while (rdr.Read())
                {
                    dicRealGoodsIdAndSumQuantity.Add(new Guid(rdr["RealGoodsId"].ToString()), Convert.ToInt32(rdr["SumQuantity"].ToString()));
                }
            }
            return dicRealGoodsIdAndSumQuantity;
        }


        /// <summary>
        /// 获取未出库数出库单信息(商品需求查询页面使用)
        /// ADD ww
        /// 2016-08-19
        /// </summary>
        /// <param name="goodsId">商品id</param>
        /// <param name="storageRecordTypes">入库类型</param>
        /// <param name="storageRecordStates">入库状态</param>
        /// <param name="filialeId">公司id</param>
        /// <param name="warehouseId">仓库id</param>
        /// <returns></returns>
        public List<StorageRecordInfo> GetStorageRecordListByNotOutQuantity(Guid goodsId, Guid filialeId, Guid warehouseId,
            List<StorageRecordType> storageRecordTypes, List<StorageRecordState> storageRecordStates)
        {
            var strStorageRecordTypes = storageRecordTypes.Aggregate(String.Empty,
               (ent, type) => ent + ((int)type + ","));
            var strStorageRecordState = storageRecordStates.Aggregate(String.Empty,
                (ent, type) => ent + ((int)type + ","));

            var strbSql = new StringBuilder();
            strbSql.Append(@"select a.StockId, a.FilialeId, a.ThirdCompanyID, a.WarehouseId, a.TradeCode, a.LinkTradeCode, a.DateCreated, a.Transactor, a.Description, a.AccountReceivable, a.SubtotalQuantity, a.StockType, a.StockState, 
                a.RelevanceFilialeId, a.RelevanceWarehouseId, a.LinkTradeID, a.StockValidation, a.AuditTime, a.LinkTradeType, a.IsOut, a.StorageType, a.IsError,a.BillNo,LogisticsCode,
                a.TradeBothPartiesType
from StorageRecord a inner join StorageRecordDetail b on a.stockId=b.stockId");
            strbSql.Append(" WHERE b.RealGoodsId='").Append(goodsId).Append("'");
            strbSql.Append(" AND a.WarehouseId='").Append(warehouseId).Append("'");
            strbSql.Append(" AND a.FilialeId='").Append(filialeId).Append("'");
            strbSql.Append(" AND a.StockState IN (")
                .Append(strStorageRecordState.Substring(0, strStorageRecordState.Length - 1))
                .Append(")");
            strbSql.Append(" AND a.StockType IN (")
                .Append(strStorageRecordTypes.Substring(0, strStorageRecordTypes.Length - 1))
                .Append(")");
            var stockList = new List<StorageRecordInfo>();
            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, strbSql.ToString()))
            {
                while (rdr.Read())
                {
                    stockList.Add(ReaderStorageRecordInfo(rdr));
                }
            }
            return stockList;
        }

        /// <summary>退货验证状态更改
        /// </summary>
        /// <param name="stockId">出入库记录ID</param>
        /// <param name="stockValidationType">是否验证</param>
        public void UpdateStorageRecordDetailByStockId(Guid stockId, bool stockValidationType)
        {
            var parms = new[] {
                                new SqlParameter("@StockId", SqlDbType.UniqueIdentifier),
                                new SqlParameter("@StockValidation",stockValidationType)
                            };
            parms[0].Value = stockId;
            parms[1].Value = stockValidationType;
            try
            {
                const string SQL = @"UPDATE StorageRecord SET StockValidation=@StockValidation where StockId=@StockId";
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL, parms);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        public Boolean UpdateNonCurrentStockByStockId(Guid stockId, Dictionary<Guid, int> realGoodsStockQuantitys)
        {
            const string SQL = @"update StorageRecordDetail set NonceWarehouseGoodsStock=@NonceWarehouseGoodsStock where StockId=@StockId AND RealGoodsId=@RealGoodsId ";
            using (var db = DatabaseFactory.Create())
            {
                db.BeginTransaction();
                foreach (var realGoodsStock in realGoodsStockQuantitys)
                {
                    var parmsDetail = new[]
                    {
                        new Parameter("@StockId", stockId),
                        new Parameter("@RealGoodsId", realGoodsStock.Key),
                        new Parameter("@NonceWarehouseGoodsStock", realGoodsStock.Value)
                    };
                    db.Execute(false, SQL, parmsDetail);
                }
                return db.CompleteTransaction();
            }
        }

        public bool UpdateNonCurrentStockByRealGoodsId(Guid stockId, Guid realGoodsId, int quantity)
        {
            const string SQL = @"update StorageRecordDetail set NonceWarehouseGoodsStock=@NonceWarehouseGoodsStock where StockId=@StockId AND RealGoodsId=@RealGoodsId ";
            using (var db = DatabaseFactory.Create())
            {
                var parmsDetail = new[]
                    {
                        new Parameter("@StockId", stockId),
                        new Parameter("@RealGoodsId",realGoodsId),
                        new Parameter("@NonceWarehouseGoodsStock", quantity)
                    };
                return db.Execute(false, SQL, parmsDetail);
            }
        }

        /// <summary>
        /// 查找未审核的售后退货单据
        /// </summary>
        /// <param name="warehouseId"></param>
        /// <param name="filialeId"></param>
        /// <param name="storageRecordType"></param>
        /// <param name="storageRecordState"></param>
        /// <param name="storageType"></param>
        /// <returns></returns>
        public List<StorageRecordInfo> GetStorageRecordList(Guid warehouseId, Guid filialeId, int storageRecordType, int storageRecordState, int storageType)
        {
            const String SQL = @"select StockId, FilialeId, ThirdCompanyID, WarehouseId, TradeCode, LinkTradeCode, DateCreated, Transactor, Description, AccountReceivable, SubtotalQuantity, StockType, 
StockState, RelevanceFilialeId, RelevanceWarehouseId, LinkTradeID, StockValidation, AuditTime, LinkTradeType, IsOut, StorageType, IsError,BillNo,LogisticsCode,
TradeBothPartiesType
from StorageRecord 
WHERE WarehouseId=@WarehouseId and FilialeId=@FilialeId and StockType=@StockType and StockState=@StockState and StorageType=@StorageType order by DateCreated desc";
            var stockList = new List<StorageRecordInfo>();
            var parmsDetail = new[]
                    {
                        new SqlParameter("@WarehouseId", warehouseId),
                        new SqlParameter("@FilialeId", filialeId),
                        new SqlParameter("@StockType", storageRecordType),
                        new SqlParameter("@StockState", storageRecordState),
                        new SqlParameter("@StorageType", storageType)
                    };
            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL, parmsDetail))
            {
                while (rdr.Read())
                {
                    stockList.Add(ReaderStorageRecordInfo(rdr));
                }
            }
            return stockList;
        }

        /// <summary>
        /// 查找指定时间段内物流公司的销售出库单据
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="storageRecordType"></param>
        /// <param name="storageRecordState"></param>
        /// <param name="linkTradeType"></param>
        /// <returns></returns>
        public List<StorageRecordInfo> GetStorageRecordList(DateTime startTime, DateTime endTime, int storageRecordType, int storageRecordState, int linkTradeType)
        {
            const String SQL = @"select StockId, FilialeId, ThirdCompanyID, WarehouseId,AccountReceivable, SubtotalQuantity from StorageRecord 
WHERE StockType=@StockType and StockState=@StockState and LinkTradeType=@LinkTradeType And AuditTime BETWEEN @StartTime AND @EndTime And ThirdCompanyID<>'00000000-0000-0000-0000-000000000000'";
            var stockList = new List<StorageRecordInfo>();
            var parmsDetail = new[]
                    {
                        new SqlParameter("@StartTime", startTime),
                        new SqlParameter("@EndTime", endTime),
                        new SqlParameter("@StockType", storageRecordType),
                        new SqlParameter("@StockState", storageRecordState),
                        new SqlParameter("@LinkTradeType", linkTradeType)
                    };
            using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL, parmsDetail))
            {
                while (dr.Read())
                {
                    stockList.Add(new StorageRecordInfo
                    {
                        StockId = dr["StockId"] == DBNull.Value ? Guid.Empty : new Guid(dr["StockId"].ToString()),
                        FilialeId = dr["FilialeId"] == DBNull.Value ? Guid.Empty : new Guid(dr["FilialeId"].ToString()),
                        ThirdCompanyID = dr["ThirdCompanyID"] == DBNull.Value ? Guid.Empty : new Guid(dr["ThirdCompanyID"].ToString()),
                        WarehouseId = dr["WarehouseId"] == DBNull.Value ? Guid.Empty : new Guid(dr["WarehouseId"].ToString()),
                        SubtotalQuantity = Convert.ToDecimal(dr["SubtotalQuantity"].ToString()),
                        AccountReceivable = Math.Abs(Convert.ToDecimal(dr["AccountReceivable"].ToString()))
                    });
                }
            }
            return stockList;
        }

        /// <summary>
        /// 查找指定时间段内物流公司的销售出库单据明细
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="storageRecordType"></param>
        /// <param name="storageRecordState"></param>
        /// <param name="linkTradeType"></param>
        /// <returns></returns>
        public List<StorageRecordDetailInfo> GetStorageRecordDetailList(DateTime startTime, DateTime endTime, int storageRecordType, int storageRecordState, int linkTradeType)
        {
            const String SQL = @"SELECT gs.StockId,s.FilialeId,s.ThirdCompanyID,s.TradeCode,s.LinkTradeCode,s.DateCreated,s.Transactor,
            gs.GoodsId,gs.RealGoodsId,gs.GoodsName,gs.GoodsCode,gs.Specification,gs.Quantity,
            gs.UnitPrice,NonceWarehouseGoodsStock,s.StockType,s.StockState,
            gs.Description,s.StockValidation,s.WarehouseId,gs.JoinPrice,BatchNo,ShelfType  FROM StorageRecordDetail gs SD WITH(NOLOCK)
INNER JOIN StorageRecord AS s  WITH(NOLOCK) ON SD.StockId=S.StockId 
WHERE s.StockType=@StockType and s.StockState=@StockState and s.LinkTradeType=@LinkTradeType And s.AuditTime BETWEEN @StartTime AND @EndTime  And ThirdCompanyID<>'00000000-0000-0000-0000-000000000000'";
            var stockList = new List<StorageRecordDetailInfo>();
            var parmsDetail = new[]
                    {
                        new SqlParameter("@StartTime", startTime),
                        new SqlParameter("@EndTime", endTime),
                        new SqlParameter("@StockType", storageRecordType),
                        new SqlParameter("@StockState", storageRecordState),
                        new SqlParameter("@LinkTradeType", linkTradeType)
                    };
            using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL, parmsDetail))
            {
                while (dr.Read())
                {
                    stockList.Add(new StorageRecordDetailInfo
                    {
                        StockId = new Guid(dr["StockId"].ToString()),
                        GoodsId = dr["GoodsId"] == DBNull.Value ? Guid.Empty : new Guid(dr["GoodsId"].ToString()),
                        RealGoodsId = dr["RealGoodsId"] == DBNull.Value ? Guid.Empty : new Guid(dr["RealGoodsId"].ToString()),
                        Specification = dr["Specification"] == DBNull.Value ? "" : dr["Specification"].ToString(),
                        Quantity = Math.Abs(int.Parse(dr["Quantity"].ToString())),
                        UnitPrice = decimal.Parse(dr["UnitPrice"].ToString()),
                        Description = dr["Description"] == DBNull.Value ? "" : dr["Description"].ToString(),
                        GoodsName = dr["GoodsName"] == DBNull.Value ? "" : dr["GoodsName"].ToString(),
                        GoodsCode = dr["GoodsCode"] == DBNull.Value ? "" : dr["GoodsCode"].ToString(),
                        NonceWarehouseGoodsStock = int.Parse(dr["NonceWarehouseGoodsStock"].ToString()),
                        JoinPrice = dr["JoinPrice"] == DBNull.Value ? 0 : decimal.Parse(dr["JoinPrice"].ToString()),
                        BatchNo = dr["BatchNo"] == DBNull.Value ? "" : dr["BatchNo"].ToString(),
                        ShelfType = dr["ShelfType"] == DBNull.Value ? Byte.MinValue : Convert.ToByte(dr["ShelfType"])
                    });
                }
            }
            return stockList;
        }

        public Dictionary<Guid, int> GetReturnRealGoods(Guid warehouseId, int storageRecordType, int storageType, List<int> storageRecordStates, Guid realGoodsId)
        {
            var stockList = new Dictionary<Guid, int>();
            string storageRecordState = String.Empty;
            if (storageRecordStates != null && storageRecordStates.Count > 0)
            {
                storageRecordState = " and StockState in(" + String.Join(",", storageRecordStates) + ")";
            }
            string SQL = String.Format(@"select RealGoodsId,Sum(Quantity) AS Quantity from StorageRecordDetail SR 
inner join StorageRecord S ON SR.StockId=S.StockId
where s.WarehouseId=@WarehouseId
and s.StockType=@StockType and StorageType =@StorageType AND RealGoodsId = @RealGoodsId {0}
group by RealGoodsId", storageRecordState);

            var parmsDetail = new[]
                    {
                        new SqlParameter("@WarehouseId", warehouseId),
                        new SqlParameter("@StockType", storageRecordType),
                        new SqlParameter("@StorageType", storageType),
                        new SqlParameter("@storageRecordState",storageRecordState),
                        new SqlParameter("@RealGoodsId",realGoodsId)
                    };
            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL, parmsDetail))
            {
                while (rdr.Read())
                {
                    stockList.Add(new Guid(rdr["RealGoodsId"].ToString()), Convert.ToInt16(rdr["Quantity"].ToString()));
                }
            }
            return stockList;
        }

        public Dictionary<Guid, int> GetCancelRealGoods(Guid warehouseId, int storageRecordType, int storageType, int storageRecordState, Guid realGoodsId)
        {
            string SQL = @"select top 1 RealGoodsId,Sum(Quantity) AS Quantity from StorageRecordDetail SR 
inner join StorageRecord S ON SR.StockId=S.StockId
where s.WarehouseId=@WarehouseId
and s.StockType=@StockType and StorageType =@StorageType AND RealGoodsId = @RealGoodsId AND StockState= @StockState
group by RealGoodsId order by S.DateCreated desc";
            var stockList = new Dictionary<Guid, int>();
            var parmsDetail = new[]
                    {
                        new SqlParameter("@WarehouseId", warehouseId),
                        new SqlParameter("@StockType", storageRecordType),
                        new SqlParameter("@StorageType", storageType),
                        new SqlParameter("@StockState",storageRecordState),
                        new SqlParameter("@RealGoodsId",realGoodsId)
                    };
            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL, parmsDetail))
            {
                while (rdr.Read())
                {
                    stockList.Add(new Guid(rdr["RealGoodsId"].ToString()), Convert.ToInt16(rdr["Quantity"].ToString()));
                }
            }
            return stockList;
        }

        public Dictionary<Guid, int> GetAllReturnRealGoods(Guid warehouseId, int storageRecordType, int storageType, List<int> storageRecordStates)
        {
            var stockList = new Dictionary<Guid, int>();
            string storageRecordState = String.Empty;
            if (storageRecordStates != null && storageRecordStates.Count > 0)
            {
                storageRecordState = " and StockState in(" + String.Join(",", storageRecordStates) + ")";
            }
            string SQL = String.Format(@"select RealGoodsId,Sum(Quantity) AS Quantity from StorageRecordDetail SR 
inner join StorageRecord S ON SR.StockId=S.StockId
where s.WarehouseId=@WarehouseId
and s.StockType=@StockType and StorageType =@StorageType {0}
group by RealGoodsId", storageRecordState);
            var parmsDetail = new[]
                    {
                        new SqlParameter("@WarehouseId", warehouseId),
                        new SqlParameter("@StockType", storageRecordType),
                        new SqlParameter("@StorageType", storageType)
                    };
            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL, parmsDetail))
            {
                while (rdr.Read())
                {
                    stockList.Add(new Guid(rdr["RealGoodsId"].ToString()), Convert.ToInt16(rdr["Quantity"].ToString()));
                }
            }
            return stockList;
        }
        #endregion

        public bool SetBillNo(Guid stockId, string billNo)
        {
            const string SQL = @"UPDATE StorageRecord SET BillNo=@BillNo WHERE StockId=@StockId";
            var parms = new[]
                {
                    new Parameter("@StockId", stockId),
                    new Parameter("@BillNo", billNo)
                };

            using (var db = DatabaseFactory.Create())
            {
                return db.Execute(false, SQL, parms);
            }
        }

        public bool IsAbeyancedThirdComapny(string tradeCode)
        {
            const string SQL = @"select COUNT(*) from StorageRecord AS S with(nolock)
inner join lmShop_CompanyCussent AS C with(nolock) ON S.ThirdCompanyID=C.CompanyId AND C.[State]=@State
where TradeCode=@TradeCode";
            var parms = new[]
                {
                    new Parameter("@TradeCode", tradeCode),
                    new Parameter("@State",(int)State.Disable)
                };

            using (var db = DatabaseFactory.Create())
            {
                return db.GetValue<int>(true, SQL, parms) > 0;
            }
        }

        public IList<Guid> GetGoodsIdListFromStorageRecordDetail(string tradeCode)
        {
            const string SQL = @"	
            SELECT DISTINCT([RealGoodsId]) FROM [StorageRecordDetail] WITH(NOLOCK)
                WHERE [StockId] IN 
	            (
		            SELECT [StockId] FROM [StorageRecord] WITH(NOLOCK) WHERE TradeCode=@TradeCode
	            )";
            IList<Guid> list = new List<Guid>();
            using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL, new SqlParameter("@TradeCode", tradeCode)))
            {
                while (dr.Read())
                {
                    list.Add(dr["RealGoodsId"] == DBNull.Value ? Guid.Empty : new Guid(dr["RealGoodsId"].ToString()));
                }
            }
            return list;
        }

        public InnerPurchaseRelationInfo GetInnerPurchaseRelationInfo(Guid outStockId)
        {
            const string SQL = @"SELECT [OutStockId]
      ,[InStockId]
      ,[PurchasingId]
      ,[OutWarehouseId]
      ,[OutHostingFilialeId]
      ,[OutStorageType]
      ,[InWarehouseId]
      ,[InHostingFilialeId]
      ,[InStorageType]
  FROM [InnerPurchaseRelation] WITH(NOLOCK) WHERE OutStockId=@OutStockId;";

            var parm = new SqlParameter("@OutStockId", SqlDbType.VarChar) { Value = outStockId.ToString() };
            var stockInfo = new InnerPurchaseRelationInfo();
            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL, parm))
            {
                if (rdr.Read())
                {
                    stockInfo.OutStockId = rdr["OutStockId"] == DBNull.Value
                        ? Guid.Empty
                        : new Guid(rdr["OutStockId"].ToString());
                    stockInfo.InStockId = rdr["InStockId"] == DBNull.Value
                        ? Guid.Empty
                        : new Guid(rdr["InStockId"].ToString());
                    stockInfo.PurchasingId = rdr["PurchasingId"] == DBNull.Value
                        ? Guid.Empty
                        : new Guid(rdr["PurchasingId"].ToString());
                    stockInfo.OutWarehouseId = rdr["OutWarehouseId"] == DBNull.Value
                       ? Guid.Empty
                       : new Guid(rdr["OutWarehouseId"].ToString());
                    stockInfo.OutHostingFilialeId = rdr["OutHostingFilialeId"] == DBNull.Value
                        ? Guid.Empty
                        : new Guid(rdr["OutHostingFilialeId"].ToString());
                    stockInfo.OutStorageType = rdr["OutStorageType"] == DBNull.Value
                        ? 0
                        : Convert.ToInt32(rdr["OutStorageType"].ToString());
                    stockInfo.InWarehouseId = rdr["InWarehouseId"] == DBNull.Value
                      ? Guid.Empty
                      : new Guid(rdr["InWarehouseId"].ToString());
                    stockInfo.InHostingFilialeId = rdr["InHostingFilialeId"] == DBNull.Value
                        ? Guid.Empty
                        : new Guid(rdr["InHostingFilialeId"].ToString());
                    stockInfo.InStorageType = rdr["InStorageType"] == DBNull.Value
                        ? 0
                        : Convert.ToInt32(rdr["InStorageType"].ToString());
                }
            }
            return stockInfo;
        }

        public InnerPurchaseRelationInfo GetInnerPurchaseRelationInfoIn(Guid inStockId)
        {
            const string SQL = @"SELECT [OutStockId]
      ,[InStockId]
      ,[PurchasingId]
      ,[OutWarehouseId]
      ,[OutHostingFilialeId]
      ,[OutStorageType]
      ,[InWarehouseId]
      ,[InHostingFilialeId]
      ,[InStorageType]
  FROM [InnerPurchaseRelation] WITH(NOLOCK) WHERE InStockId=@InStockId;";

            var parm = new SqlParameter("@InStockId", SqlDbType.VarChar) { Value = inStockId.ToString() };
            var stockInfo = new InnerPurchaseRelationInfo();
            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL, parm))
            {
                if (rdr.Read())
                {
                    stockInfo.OutStockId = rdr["OutStockId"] == DBNull.Value
                        ? Guid.Empty
                        : new Guid(rdr["OutStockId"].ToString());
                    stockInfo.InStockId = rdr["InStockId"] == DBNull.Value
                        ? Guid.Empty
                        : new Guid(rdr["InStockId"].ToString());
                    stockInfo.PurchasingId = rdr["PurchasingId"] == DBNull.Value
                        ? Guid.Empty
                        : new Guid(rdr["PurchasingId"].ToString());
                    stockInfo.OutWarehouseId = rdr["OutWarehouseId"] == DBNull.Value
                       ? Guid.Empty
                       : new Guid(rdr["OutWarehouseId"].ToString());
                    stockInfo.OutHostingFilialeId = rdr["OutHostingFilialeId"] == DBNull.Value
                        ? Guid.Empty
                        : new Guid(rdr["OutHostingFilialeId"].ToString());
                    stockInfo.OutStorageType = rdr["OutStorageType"] == DBNull.Value
                        ? 0
                        : Convert.ToInt32(rdr["OutStorageType"].ToString());
                    stockInfo.InWarehouseId = rdr["InWarehouseId"] == DBNull.Value
                      ? Guid.Empty
                      : new Guid(rdr["InWarehouseId"].ToString());
                    stockInfo.InHostingFilialeId = rdr["InHostingFilialeId"] == DBNull.Value
                        ? Guid.Empty
                        : new Guid(rdr["InHostingFilialeId"].ToString());
                    stockInfo.InStorageType = rdr["InStorageType"] == DBNull.Value
                        ? 0
                        : Convert.ToInt32(rdr["InStorageType"].ToString());
                }
            }
            return stockInfo;
        }

        private InnerPurchaseRelationInfo GetInnerPurchaseRelationInfoRefuse(String tradeCode)
        {
            const string SQL = @"SELECT [OutStockId]
      ,[InStockId]
      ,[PurchasingId]
      ,[OutWarehouseId]
      ,[OutHostingFilialeId]
      ,[OutStorageType]
      ,[InWarehouseId]
      ,[InHostingFilialeId]
      ,[InStorageType]
  FROM [InnerPurchaseRelation] WITH(NOLOCK)
INNER JOIN StorageRecord S ON S.StockId = OutStockId
WHERE S.TradeCode = @TradeCode;";

            var parm = new SqlParameter("@TradeCode", SqlDbType.VarChar) { Value = tradeCode };
            var stockInfo = new InnerPurchaseRelationInfo();
            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL, parm))
            {
                if (rdr.Read())
                {
                    stockInfo.OutStockId = rdr["OutStockId"] == DBNull.Value
                        ? Guid.Empty
                        : new Guid(rdr["OutStockId"].ToString());
                    stockInfo.InStockId = rdr["InStockId"] == DBNull.Value
                        ? Guid.Empty
                        : new Guid(rdr["InStockId"].ToString());
                    stockInfo.PurchasingId = rdr["PurchasingId"] == DBNull.Value
                        ? Guid.Empty
                        : new Guid(rdr["PurchasingId"].ToString());
                    stockInfo.OutWarehouseId = rdr["OutWarehouseId"] == DBNull.Value
                       ? Guid.Empty
                       : new Guid(rdr["OutWarehouseId"].ToString());
                    stockInfo.OutHostingFilialeId = rdr["OutHostingFilialeId"] == DBNull.Value
                        ? Guid.Empty
                        : new Guid(rdr["OutHostingFilialeId"].ToString());
                    stockInfo.OutStorageType = rdr["OutStorageType"] == DBNull.Value
                        ? 0
                        : Convert.ToInt32(rdr["OutStorageType"].ToString());
                    stockInfo.InWarehouseId = rdr["InWarehouseId"] == DBNull.Value
                      ? Guid.Empty
                      : new Guid(rdr["InWarehouseId"].ToString());
                    stockInfo.InHostingFilialeId = rdr["InHostingFilialeId"] == DBNull.Value
                        ? Guid.Empty
                        : new Guid(rdr["InHostingFilialeId"].ToString());
                    stockInfo.InStorageType = rdr["InStorageType"] == DBNull.Value
                        ? 0
                        : Convert.ToInt32(rdr["InStorageType"].ToString());
                }
            }
            return stockInfo;
        }

        public void InsertInnerPurchaseRelationInfo(Guid outStockId, Guid inStockId, Guid purchasingId, Guid outWarehouseId, Guid outHostingFilialeId, int outStorageType, Guid inWarehouseId, Guid inHostingFilialeId, int inStorageType)
        {
            const string SQL = @"INSERT INTO [InnerPurchaseRelation] ([OutStockId]
           ,[InStockId]
           ,[PurchasingId]
           ,[OutWarehouseId]
           ,[OutHostingFilialeId]
           ,[OutStorageType]
           ,[InWarehouseId]
           ,[InHostingFilialeId]
           ,[InStorageType]) VALUES (@OutStockId ,@InStockId,@PurchasingId,@OutWarehouseId,@OutHostingFilialeId,@OutStorageType,@InWarehouseId,@InHostingFilialeId,@InStorageType)";
            try
            {

                var parms = new[] {
                                new SqlParameter("@OutStockId", SqlDbType.UniqueIdentifier),
                                new SqlParameter("@InStockId",SqlDbType.UniqueIdentifier),
                                new SqlParameter("@PurchasingId",SqlDbType.UniqueIdentifier),
                                new SqlParameter("@OutWarehouseId", SqlDbType.UniqueIdentifier),
                                new SqlParameter("@OutHostingFilialeId",SqlDbType.UniqueIdentifier),
                                new SqlParameter("@OutStorageType",SqlDbType.Int),
                                new SqlParameter("@InWarehouseId", SqlDbType.UniqueIdentifier),
                                new SqlParameter("@InHostingFilialeId",SqlDbType.UniqueIdentifier),
                                new SqlParameter("@InStorageType",SqlDbType.Int)
                            };
                parms[0].Value = outStockId;
                parms[1].Value = inStockId;
                parms[2].Value = purchasingId;
                parms[3].Value = outWarehouseId;
                parms[4].Value = outHostingFilialeId;
                parms[5].Value = outStorageType;
                parms[6].Value = inWarehouseId;
                parms[7].Value = inHostingFilialeId;
                parms[8].Value = inStorageType;

                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL, parms);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        public void UpdateInnerPurchaseRelationInfo(Guid outStockId, Guid inStockId, Guid purchasingId, Guid outWarehouseId, Guid outHostingFilialeId, int outStorageType, Guid inWarehouseId, Guid inHostingFilialeId, int inStorageType)
        {
            const string SQL = @"UPDATE [InnerPurchaseRelation] SET [InStockId] = @InStockId,[PurchasingId] = @PurchasingId,[OutWarehouseId] =@OutWarehouseId
           ,[OutHostingFilialeId] = @OutHostingFilialeId
           ,[OutStorageType] = @OutStorageType
           ,[InWarehouseId] = @InWarehouseId
           ,[InHostingFilialeId] = @InHostingFilialeId
           ,[InStorageType] = @InStorageType WHERE  [OutStockId] = @OutStockId";
            try
            {

                var parms = new[] {
                                new SqlParameter("@OutStockId", SqlDbType.UniqueIdentifier),
                                new SqlParameter("@InStockId",SqlDbType.UniqueIdentifier),
                                new SqlParameter("@PurchasingId",SqlDbType.UniqueIdentifier),
                                new SqlParameter("@OutWarehouseId", SqlDbType.UniqueIdentifier),
                                new SqlParameter("@OutHostingFilialeId",SqlDbType.UniqueIdentifier),
                                new SqlParameter("@OutStorageType",SqlDbType.Int),
                                new SqlParameter("@InWarehouseId", SqlDbType.UniqueIdentifier),
                                new SqlParameter("@InHostingFilialeId",SqlDbType.UniqueIdentifier),
                                new SqlParameter("@InStorageType",SqlDbType.Int)
                            };
                parms[0].Value = outStockId;
                parms[1].Value = inStockId;
                parms[2].Value = purchasingId;
                parms[3].Value = outWarehouseId;
                parms[4].Value = outHostingFilialeId;
                parms[5].Value = outStorageType;
                parms[6].Value = inWarehouseId;
                parms[7].Value = inHostingFilialeId;
                parms[8].Value = inStorageType;

                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL, parms);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        public void UpdateEditInnerPurchaseRelationInfo(Guid outStockId, Guid outWarehouseId, Guid outHostingFilialeId, int outStorageType, Guid inWarehouseId, Guid inHostingFilialeId, int inStorageType)
        {
            const string SQL = @"UPDATE [InnerPurchaseRelation] SET [OutWarehouseId] =@OutWarehouseId
           ,[OutHostingFilialeId] = @OutHostingFilialeId
           ,[OutStorageType] = @OutStorageType
           ,[InWarehouseId] = @InWarehouseId
           ,[InHostingFilialeId] = @InHostingFilialeId
           ,[InStorageType] = @InStorageType WHERE  [OutStockId] = @OutStockId";
            try
            {

                var parms = new[] {
                                new SqlParameter("@OutStockId", SqlDbType.UniqueIdentifier),
                                new SqlParameter("@OutWarehouseId", SqlDbType.UniqueIdentifier),
                                new SqlParameter("@OutHostingFilialeId",SqlDbType.UniqueIdentifier),
                                new SqlParameter("@OutStorageType",SqlDbType.Int),
                                new SqlParameter("@InWarehouseId", SqlDbType.UniqueIdentifier),
                                new SqlParameter("@InHostingFilialeId",SqlDbType.UniqueIdentifier),
                                new SqlParameter("@InStorageType",SqlDbType.Int)
                            };
                parms[0].Value = outStockId;
                parms[1].Value = outWarehouseId;
                parms[2].Value = outHostingFilialeId;
                parms[3].Value = outStorageType;
                parms[4].Value = inWarehouseId;
                parms[5].Value = inHostingFilialeId;
                parms[6].Value = inStorageType;

                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL, parms);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        public bool DeleteStorageRecordByLinkTradeCode(string orderNo, out string errorMessage)
        {
            errorMessage = string.Empty;
            const string SQL_DELETE_SEMIGOODSSTOCK_BY_ORDERNO = " DELETE FROM StorageRecordDetail WHERE StockId IN(SELECT StockId FROM StorageRecord WHERE LinkTradeCode=@LinkTradeCode) ";
            const string SQL_DELETE_SEMISTOCK_BY_ORDERNO = "DELETE FROM StorageRecord WHERE LinkTradeCode=@LinkTradeCode";
            var parm = new SqlParameter("@LinkTradeCode", SqlDbType.VarChar) { Value = orderNo };
            using (var conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                conn.Open();
                SqlTransaction tran = conn.BeginTransaction();
                try
                {
                    SqlHelper.ExecuteNonQuery(tran, SQL_DELETE_SEMIGOODSSTOCK_BY_ORDERNO, parm);
                    SqlHelper.ExecuteNonQuery(tran, SQL_DELETE_SEMISTOCK_BY_ORDERNO, parm);
                    tran.Commit();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    errorMessage = ex.Message;
                    throw new ApplicationException(ex.Message);
                }
            }
            return true;
        }
        public bool CancelStorageRecordByLinkTradeCode(string orderNo, out string errorMessage)
        {
            errorMessage = string.Empty;
            const string SQL_UPDATE_SEMISTOCK_BY_ORDERNO = "UPDATE StorageRecord SET StockState=@StockState WHERE LinkTradeCode=@LinkTradeCode";
            var parms = new[]
                {
                    new Parameter("@LinkTradeCode", orderNo),
                    new Parameter("@StockState", (int)StorageRecordState.Canceled)
                };

            using (var db = DatabaseFactory.Create())
            {
                return db.Execute(false, SQL_UPDATE_SEMISTOCK_BY_ORDERNO, parms);
            }
        }

        public Dictionary<Guid, decimal> GetSellOutGoodsUnitPriceDic(Guid hostingFilialeId, Guid saleFilialeId, string billNo)
        {
            Dictionary<Guid, decimal> dics = new Dictionary<Guid, decimal>();
            const string SQL = @"select s.FilialeId,s.ThirdCompanyID,s.StockType,sr.GoodsId,sr.UnitPrice,sr.Quantity  into #TempStorageDetail from  StorageRecordDetail  as  sr  with(nolock)
inner  join  StorageRecord  as  s  with(nolock)  on  sr.StockId=s.StockId
where  BillNo=@BillNo

SELECT GoodsId,SUM(UnitPrice*Quantity)/SUM(Quantity) AS  UnitPrice FROM #TempStorageDetail WHERE FilialeId=@HostingFilialeId
AND ThirdCompanyID=@SaleFilialeId AND StockType=@StockType GROUP BY GoodsId

DROP TABLE #TempStorageDetail";

            var parm = new[]
            {
                new SqlParameter("@HostingFilialeId", SqlDbType.UniqueIdentifier) { Value = hostingFilialeId },
                new SqlParameter("@SaleFilialeId", SqlDbType.UniqueIdentifier) { Value = saleFilialeId },
                new SqlParameter("@StockType", SqlDbType.Int) { Value = (Int32)StorageRecordType.SellStockOut},
                new SqlParameter("@BillNo", SqlDbType.VarChar) { Value = billNo }
            };

            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL, parm))
            {
                while (rdr.Read() && rdr["GoodsId"] != DBNull.Value)
                {
                    dics.Add(new Guid(rdr["GoodsId"].ToString()), Convert.ToDecimal(rdr["UnitPrice"]));
                }
            }
            return dics;
        }

        public List<StockBillDTO> GetStockBillDtos(DateTime startTime, DateTime endTime, int stockState,
            IEnumerable<int> stockTypes)
        {
            const string SQL_DETAIL = @" SELECT T.TradeCode,T.FilialeId,T.ThirdCompanyId,CC.CompanyName AS CompanyCode,T.Transactor,T.DateCreated,T.[Description],T.GoodsId,T.GoodsCode,T.Quantity,T.UnitPrice FROM (SELECT SR.TradeCode,SR.FilialeId,SR.ThirdCompanyId,SR.Transactor,SR.DateCreated,SR.[Description],SD.GoodsId,SD.GoodsCode,SUM(SD.Quantity) AS Quantity,SD.UnitPrice FROM StorageRecord AS SR 
 INNER JOIN StorageRecordDetail AS SD ON SR.StockId=SD.StockId 
 WHERE SR.AuditTime BETWEEN @StartTime AND @EndTime AND SR.StockState=@StockState AND SR.StockType IN('{0}') 
GROUP BY SR.TradeCode,SR.FilialeId,SR.ThirdCompanyId,SR.Transactor,SR.DateCreated,SR.[Description],SD.GoodsId,SD.GoodsCode,SD.UnitPrice) AS T 
 LEFT JOIN lmShop_CompanyCussent AS CC ON T.ThirdCompanyID=CC.CompanyId";
            var parm = new[]
            {
                new SqlParameter("@StartTime", SqlDbType.DateTime) { Value = startTime },
                new SqlParameter("@EndTime", SqlDbType.DateTime) { Value = endTime },
                new SqlParameter("@StockState", SqlDbType.Int) { Value = stockState}
            };
            
            Dictionary<string,StockBillDTO> result=new Dictionary<string, StockBillDTO>();
            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, string.Format(SQL_DETAIL,string.Join(",",stockTypes)), parm))
            {
                while (rdr.Read() && rdr["TradeCode"] != DBNull.Value)
                {
                    var tradeCode = rdr["TradeCode"].ToString();
                    if (result.ContainsKey(tradeCode))
                    {
                        result[tradeCode].Details.Add(new StockBillDTO.Detail
                        {
                            GoodsCode = rdr["GoodsCode"].ToString(),
                            GoodsId = new Guid(rdr["GoodsId"].ToString()),
                            Quantity = Math.Abs(Convert.ToInt32(rdr["Quantity"])),
                            UnitPrice = Convert.ToDecimal(rdr["UnitPrice"])
                        });
                    }
                    else
                    {
                        result.Add(tradeCode,new StockBillDTO
                        {
                            Transactor = rdr["Transactor"].ToString(),
                            CompanyCode = rdr["CompanyCode"].ToString(),
                            FilialeId = new Guid(rdr["FilialeId"].ToString()),
                            DateCreated = Convert.ToDateTime(rdr["DateCreated"]),
                            Description = rdr["Description"].ToString(),
                            TradeCode = tradeCode,
                            Details = new List<StockBillDTO.Detail>
                            {
                                new StockBillDTO.Detail
                                {
                                    GoodsCode = rdr["GoodsCode"].ToString(),
                                    GoodsId = new Guid(rdr["GoodsId"].ToString()),
                                    Quantity =Math.Abs( Convert.ToInt32(rdr["Quantity"])),
                                    UnitPrice = Convert.ToDecimal(rdr["UnitPrice"])
                                }
                            }
                        });
                    }
                }
            }
            return result.Values.ToList();
        }

        public List<SourceBindGoods> GetStorageRecordDetailsByLinkTradeCode(IEnumerable<string> linkTradeCodes)
        {
            const string SQL = @"SELECT SD.GoodsId,SD.GoodsCode,AVG(SD.UnitPrice) AS UnitPrice,
 ABS(SUM(SD.Quantity)) AS Quantity,SR.TradeCode,SR.LinkTradeCode FROM StorageRecordDetail AS SD
INNER JOIN StorageRecord AS SR ON SD.StockId=SR.StockId
 WHERE LinkTradeCode IN('{0}') AND StockType=@StockType AND StockState=@StockState AND TradeBothPartiesType=@TradeBothPartiesType  
 GROUP BY GoodsId,GoodsCode,TradeCode,LinkTradeCode";
            var parms = new[]
            {
                new Parameter("@TradeBothPartiesType",(int)TradeBothPartiesType.Other),
                new Parameter("@StockState",  (int)StorageRecordState.Finished),
                new Parameter("@StockType",  (int)StorageRecordType.SellStockOut)
            };

            using (var db = DatabaseFactory.Create())
            {
                return db.Select<SourceBindGoods>(true, string.Format(SQL,string.Join(",",linkTradeCodes)), parms).ToList();
            }
        }
    }
}
