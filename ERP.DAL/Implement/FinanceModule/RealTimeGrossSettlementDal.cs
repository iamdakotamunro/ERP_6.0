using Dapper;
using Dapper.Extension;
using ERP.DAL.Interface.FinanceModule;
using ERP.Environment;
using ERP.Model;
using ERP.Model.FinanceModule;
using Keede.DAL.RWSplitting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Transactions;

namespace ERP.DAL.Implement.FinanceModule
{
    /// <summary>
    /// 即时结算价数据访问层
    /// </summary>
    public class RealTimeGrossSettlementDal : IRealTimeGrossSettlementDal
    {
        public RealTimeGrossSettlementDal(GlobalConfig.DB.FromType fromType) { }

        /// <summary>
        /// 获取最新的结算价信息
        /// </summary>
        /// <param name="filialeId"></param>
        /// <param name="goodsId"></param>
        /// <returns></returns>
        public decimal GetLatestUnitPrice(Guid filialeId, Guid goodsId)
        {
            const string SQL = @"
select  top 1 UnitPrice
from RealTimeGrossSettlement
where FilialeId=@FilialeId and GoodsId=@GoodsId
order by OccurTime desc
";
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                return conn.ExecuteScalar<decimal>(SQL, new { FilialeId = filialeId, GoodsId = goodsId });
            }
        }

        /// <summary>
        /// 在指定时间之前，获取最新的结算价信息
        /// </summary>
        /// <param name="filialeId"></param>
        /// <param name="goodsId"></param>
        /// <param name="specificTime">指定时间</param>
        /// <returns></returns>
        public decimal GetLatestUnitPriceBeforeSpecificTime(Guid filialeId, Guid goodsId, DateTime specificTime)
        {
            const string SQL = @"
select  top 1 UnitPrice
from RealTimeGrossSettlement
where FilialeId=@FilialeId and GoodsId=@GoodsId and OccurTime<=@SpecificTime
order by OccurTime desc
";
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                return conn.ExecuteScalar<decimal>(SQL, new { FilialeId = filialeId, GoodsId = goodsId, SpecificTime = specificTime });
            }
        }

        /// <summary>
        /// 按主商品列表获取最新即时结算价列表
        /// </summary>
        /// <param name="filialeId">公司ID</param>
        /// <param name="goodsId">主商品ID列表</param>
        /// <returns></returns>
        public IDictionary<Guid, decimal> GetLatestUnitPriceListByMultiGoods(Guid filialeId, IEnumerable<Guid> goodsIds)
        {
            Dictionary<Guid, decimal> result = new Dictionary<Guid, decimal>();
            if (goodsIds == null || !goodsIds.Any())
            {
                return result;
            }
            const string SQL = @"
select	GoodsId
		,UnitPrice
from (
	select	ROW_NUMBER() over (partition by GoodsId order by OccurTime desc) g
			,GoodsId
			,UnitPrice
	from RealTimeGrossSettlement
    where FilialeId=@FilialeId and GoodsId in ('{0}')
) t1
where t1.g=1
";
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                return conn.Query(string.Format(SQL, string.Join("','", goodsIds.Select(m => m.ToString()))), new { FilialeId = filialeId }).ToDictionary(kv => (Guid)kv.GoodsId, kv => (decimal)kv.UnitPrice);
            }
        }

        /// <summary>
        /// 在指定时间之前，按主商品列表获取最新即时结算价列表
        /// </summary>
        /// <param name="filialeId">公司ID</param>
        /// <param name="goodsId">主商品ID列表</param>
        /// <param name="specificTime">指定时间</param>
        /// <returns></returns>
        public IDictionary<Guid, decimal> GetLatestUnitPriceListBeforeSpecificTimeByMultiGoods(Guid filialeId, IEnumerable<Guid> goodsIds, DateTime specificTime)
        {
            Dictionary<Guid, decimal> result = new Dictionary<Guid, decimal>();
            if (goodsIds == null || !goodsIds.Any())
            {
                return result;
            }
            const string SQL = @"
select	GoodsId
		,UnitPrice
from (
	select	ROW_NUMBER() over (partition by GoodsId order by OccurTime desc) g
			,GoodsId
			,UnitPrice
	from RealTimeGrossSettlement
    where FilialeId=@FilialeId and GoodsId in ('{0}') and OccurTime<=@SpecificTime
) t1
where t1.g=1
";
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                return conn.Query(string.Format(SQL, string.Join("','", goodsIds.Select(m => m.ToString()))), new
                {
                    FilialeId = filialeId,
                    SpecificTime = specificTime,
                }).ToDictionary(kv => (Guid)kv.GoodsId, kv => (decimal)kv.UnitPrice);
            }
        }

        /// <summary>
        /// 获取最新的结算价
        /// </summary>
        /// <param name="filialeId"></param>
        /// <param name="goodsId"></param>
        /// <returns></returns>
        public RealTimeGrossSettlementInfo GetLatest(Guid filialeId, Guid goodsId)
        {
            const string SQL = @"
select	top 1 [FilialeId]
		,[GoodsId]
		,[UnitPrice]
		,[StockQuantity]
		,[GoodsQuantityInBill]
		,[GoodsAmountInBill]
		,[RelatedTradeType]
		,[RelatedTradeNo]
		,[OccurTime]
		,[CreateTime]
        ,IsNull([ExtField_1],0) [ExtField_1]
from RealTimeGrossSettlement
where FilialeId=@FilialeId and GoodsId=@GoodsId
order by OccurTime desc
";

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                return conn.QueryFirstOrDefault<RealTimeGrossSettlementInfo>(SQL, new { FilialeId = filialeId, GoodsId = goodsId });
            }
        }
        
        /// <summary>
        /// 获取最新的结算价
        /// </summary>
        /// <param name="filialeId"></param>
        /// <param name="goodsId"></param>
        /// <returns></returns>
        public RealTimeGrossSettlementInfo GetLatestBeforeSpecificTime(Guid filialeId, Guid goodsId, DateTime specificTime)
        {
            const string SQL = @"
select	top 1 [FilialeId]
		,[GoodsId]
		,[UnitPrice]
		,[StockQuantity]
		,[GoodsQuantityInBill]
		,[GoodsAmountInBill]
		,[RelatedTradeType]
		,[RelatedTradeNo]
		,[OccurTime]
		,[CreateTime]
        ,IsNull([ExtField_1],0) [ExtField_1]
from RealTimeGrossSettlement
where FilialeId=@FilialeId and GoodsId=@GoodsId and OccurTime<=@SpecificTime
order by OccurTime desc
";
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                return conn.QueryFirstOrDefault<RealTimeGrossSettlementInfo>(SQL, new { FilialeId = filialeId, GoodsId = goodsId, SpecificTime = specificTime });
            }
        }

        /// <summary>
        /// 按主商品列表获取最新即时结算价列表
        /// </summary>
        /// <param name="filialeId">公司ID</param>
        /// <param name="goodsIds">主商品ID列表</param>
        /// <returns></returns>
        public IDictionary<Guid, RealTimeGrossSettlementInfo> GetLatestListByMultiGoods(Guid filialeId, IEnumerable<Guid> goodsIds)
        {
            Dictionary<Guid, RealTimeGrossSettlementInfo> result = new Dictionary<Guid, RealTimeGrossSettlementInfo>();
            if (goodsIds == null || !goodsIds.Any())
            {
                return result;
            }
            const string SQL = @"
select	[FilialeId]
		,[GoodsId]
		,[UnitPrice]
		,[StockQuantity]
		,[GoodsQuantityInBill]
		,[GoodsAmountInBill]
		,[RelatedTradeType]
		,[RelatedTradeNo]
		,[OccurTime]
		,[CreateTime]
        ,IsNull([ExtField_1],0) [ExtField_1]
from (
	select	ROW_NUMBER() over (partition by GoodsId order by OccurTime desc) g
			,[FilialeId]
	        ,[GoodsId]
	        ,[UnitPrice]
	        ,[StockQuantity]
	        ,[GoodsQuantityInBill]
	        ,[GoodsAmountInBill]
	        ,[RelatedTradeType]
	        ,[RelatedTradeNo]
	        ,[OccurTime]
	        ,[CreateTime]
            ,[ExtField_1]
	from RealTimeGrossSettlement
    where FilialeId=@FilialeId and GoodsId in ('{0}')
) t1
where t1.g=1
";
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                return conn.Query<RealTimeGrossSettlementInfo>(string.Format(SQL, string.Join("','", goodsIds.Select(m => m.ToString()))), new
                {
                    FilialeId = filialeId,
                }).ToDictionary(kv => kv.GoodsId, kv => kv);
            }
        }

        /// <summary>
        /// 在指定时间之前，按主商品列表获取最新即时结算价列表
        /// </summary>
        /// <param name="filialeId">公司ID</param>
        /// <param name="goodsId">主商品ID列表</param>
        /// <param name="specificTime">指定时间</param>
        /// <returns></returns>
        public IDictionary<Guid, RealTimeGrossSettlementInfo> GetLatestListBeforeSpecificTimeByMultiGoods(Guid filialeId, IEnumerable<Guid> goodsIds, DateTime specificTime)
        {
            Dictionary<Guid, RealTimeGrossSettlementInfo> result = new Dictionary<Guid, RealTimeGrossSettlementInfo>();
            if (goodsIds == null || !goodsIds.Any())
            {
                return result;
            }
            const string SQL = @"
select	[FilialeId]
		,[GoodsId]
		,[UnitPrice]
		,[StockQuantity]
		,[GoodsQuantityInBill]
		,[GoodsAmountInBill]
		,[RelatedTradeType]
		,[RelatedTradeNo]
		,[OccurTime]
		,[CreateTime]
        ,IsNull([ExtField_1],0) [ExtField_1]
from (
	select	ROW_NUMBER() over (partition by GoodsId order by OccurTime desc) g
			,[FilialeId]
	        ,[GoodsId]
	        ,[UnitPrice]
	        ,[StockQuantity]
	        ,[GoodsQuantityInBill]
	        ,[GoodsAmountInBill]
	        ,[RelatedTradeType]
	        ,[RelatedTradeNo]
	        ,[OccurTime]
	        ,[CreateTime]
            ,[ExtField_1]
	from RealTimeGrossSettlement
    where FilialeId=@FilialeId and GoodsId in ('{0}') and OccurTime<=@SpecificTime
) t1
where t1.g=1
";
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                return conn.Query<RealTimeGrossSettlementInfo>(string.Format(SQL, string.Join("','", goodsIds.Select(m => m.ToString()))), new
                {
                    FilialeId = filialeId,
                    SpecificTime = specificTime,
                }).ToDictionary(kv => kv.GoodsId, kv => kv);
            }
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="info"></param>
        public void Save(RealTimeGrossSettlementInfo info)
        {
            if (info == null)
            {
                return;
            }
            const string SQL = @"
INSERT INTO [RealTimeGrossSettlement]
           ([FilialeId]
           ,[GoodsId]
           ,[UnitPrice]
           ,[StockQuantity]
           ,[GoodsQuantityInBill]
           ,[GoodsAmountInBill]
           ,[RelatedTradeType]
           ,[RelatedTradeNo]
           ,[OccurTime]
           ,[CreateTime]
           ,[ExtField_1])
     VALUES
            (@FilialeId
            ,@GoodsId
            ,@UnitPrice
            ,@StockQuantity
            ,@GoodsQuantityInBill
            ,@GoodsAmountInBill
            ,@RelatedTradeType
            ,@RelatedTradeNo
            ,@OccurTime
            ,getdate()
            ,@ExtField_1)
";
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                conn.Execute(SQL, new
                {
                    FilialeId = info.FilialeId,
                    GoodsId = info.GoodsId,
                    UnitPrice = info.UnitPrice,
                    StockQuantity = info.StockQuantity,
                    GoodsQuantityInBill = info.GoodsQuantityInBill,
                    GoodsAmountInBill = info.GoodsAmountInBill,
                    RelatedTradeType = info.RelatedTradeType,
                    RelatedTradeNo = info.RelatedTradeNo,
                    OccurTime = info.OccurTime,
                    ExtField_1= info.ExtField_1,
                });
            }
        }

        /// <summary>
        /// 归档上个月的结算价
        /// </summary>
        public void ArchiveLastMonth()
        {
            const string SQL = @"
insert into GoodsGrossSettlementByMonth(FilialeId,GoodsId,UnitPrice,StatisticMonth,CreateTime)
select	FilialeId
		,GoodsId
		,UnitPrice
		,convert(varchar(7),@OccurTime, 120) StatisticMonth
		,GETDATE()
from (
	select	ROW_NUMBER() over (partition by FilialeId,GoodsId order by OccurTime desc) g
			,FilialeId
			,GoodsId
			,UnitPrice
	from RealTimeGrossSettlement
    where OccurTime<=@OccurTime
) t1
where t1.g=1
	and not exists(select top 1 1 from GoodsGrossSettlementByMonth t2 where t2.FilialeId=t1.FilialeId and t2.GoodsId=t1.GoodsId and t2.StatisticMonth=convert(varchar(7),@OccurTime, 120))
";
            var today = DateTime.Today;
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                conn.Execute(SQL, new
                {
                    OccurTime = today.Add(new TimeSpan(1 - today.Day, 0, 0, -1)),
                });
            }
        }

        /// <summary>
        /// 获取已按月归档的商品结算价历史列表
        /// </summary>
        /// <param name="filialeId"></param>
        /// <param name="goodsId"></param>
        /// <returns></returns>
        public IList<GoodsGrossSettlementByMonthInfo> GetArchivedUnitPriceHistoryList(Guid filialeId, Guid goodsId)
        {
            const string SQL = @"
select	StatisticMonth
		,UnitPrice
from GoodsGrossSettlementByMonth
where FilialeId=@FilialeId and GoodsId=@GoodsId
order by StatisticMonth desc
";
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.Query<GoodsGrossSettlementByMonthInfo>(SQL, new
                {
                    FilialeId = filialeId,
                    GoodsId = goodsId,
                }).AsList();
            }
        }

        /// <summary>
        /// 获取指定时间下某公司的某商品的最近结算价
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        /// zal 2017-07-28
        public Dictionary<Guid, Dictionary<Guid, decimal>> GetFilialeIdGoodsIdAvgSettlePrice(DateTime dateTime)
        {
            const string SQL = @"
            SELECT FilialeId,GoodsId,AvgSettlePrice
	        FROM (
		        SELECT ROW_NUMBER() OVER(PARTITION BY A.FilialeId,A.GoodsId ORDER BY A.OccurTime DESC) AS rowNum,A.FilialeId,A.GoodsId,A.UnitPrice AS AvgSettlePrice
		        FROM dbo.RealTimeGrossSettlement AS A WITH(NOLOCK)
		        WHERE 1=1 AND A.OccurTime<=@OccurTime
	        )#temp
	        WHERE #temp.rowNum = 1
			ORDER BY #temp.FilialeId
            ";

            var dicFilialeIdAndGoodsIdAvgSettlePrice = new Dictionary<Guid, Dictionary<Guid, decimal>>();

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                dicFilialeIdAndGoodsIdAvgSettlePrice = conn.Query(SQL, new
                {
                    OccurTime = dateTime,
                }).AsList()
                .GroupBy(g => (Guid)g.FilialeId).ToDictionary(kv => kv.Key, kv => kv.GroupBy(g2 => (Guid)g2.GoodsId).ToDictionary(kv2 => kv2.Key, kv2 => (decimal)kv2.Max(m => m.AvgSettlePrice)));
            }
            if (dicFilialeIdAndGoodsIdAvgSettlePrice == null)
            {
                dicFilialeIdAndGoodsIdAvgSettlePrice = new Dictionary<Guid, Dictionary<Guid, decimal>>();
            }
            return dicFilialeIdAndGoodsIdAvgSettlePrice;
        }
    }
}
