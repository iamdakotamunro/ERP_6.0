using ERP.DAL.Interface.FinanceModule;
using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Model.FinanceModule;
using ERP.Environment;
using System.Data.SqlClient;
using System.Data;
using ERP.Enum;
using Keede.DAL.RWSplitting;
using Dapper;
using System.Transactions;

namespace ERP.DAL.Implement.FinanceModule
{
    /// <summary>
    /// 销售订单商品结算价 数据访问层
    /// </summary>
    public class SaleOrderGoodsSettlementDal : ISaleOrderGoodsSettlementDal
    {
        public SaleOrderGoodsSettlementDal(GlobalConfig.DB.FromType fromType) { }

        public IList<SampleSaleOrderInfo> GetUnsavedB2COrderList(DateTime statisticDate, int maxRowCount)
        {
            DateTime startDate = statisticDate.Date;
            DateTime endDate = statisticDate.Date.AddDays(1).AddSeconds(-1);
            const string SQL = @"
select	distinct t1.OrderNo TradeNo
		,t1.ConsignTime OccurTime
		,t1.SaleFilialeId FilialeId
		,t2.GoodsId
from
(
	SELECT	top {0} OrderId,OrderNo,ConsignTime,SaleFilialeId
	FROM lmshop_GoodsOrder WITH(NOLOCK)
	WHERE OrderState=@OrderState AND ConsignTime between @StatisticStartDate and @StatisticEndDate
) t1
INNER JOIN lmShop_GoodsOrderDetail t2 WITH(NOLOCK) on t2.OrderId=t1.OrderId
where not exists (
		select top 1 1
		from SaleOrderGoodsSettlement SOGS WITH(NOLOCK)
		where SOGS.FilialeId=t1.SaleFilialeId and SOGS.GoodsId=t2.GoodsId and SOGS.RelatedTradeNo=t1.OrderNo
	)
";
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.Query(string.Format(SQL, maxRowCount), new
                {
                    OrderState = (int)OrderState.Consignmented,
                    StatisticStartDate = startDate,
                    StatisticEndDate = endDate
                }).GroupBy(g => (string)g.TradeNo)
                .Select(m => new SampleSaleOrderInfo
                {
                    TradeType = SaleOrderGoodsSettlementRelatedTradeType.B2COrder,
                    TradeNo = m.Key,
                    FilialeId = (Guid)m.Last().FilialeId,
                    OccurTime = (DateTime)m.Last().OccurTime,
                    GoodsIds = new List<Guid>(m.Select(s => (Guid)s.GoodsId))
                })
                .AsList();
            }
        }

        public IList<SampleSaleOrderInfo> GetUnsavedSaleStockOutForShopList(DateTime statisticDate, int maxRowCount)
        {
            DateTime startDate = statisticDate.Date;
            DateTime endDate = statisticDate.Date.AddDays(1).AddSeconds(-1);
            const string SQL = @"
select	distinct t1.TradeCode TradeNo
		,t1.AuditTime OccurTime
		,t1.FilialeId
		,t2.GoodsId
from (
	SELECT top {0} A.StockId,A.TradeCode,A.AuditTime,A.FilialeId
	FROM StorageRecord A WITH(NOLOCK) 
	INNER JOIN Shopfront_ApplyStock B WITH(NOLOCK) ON A.LinkTradeCode=B.TradeCode
	WHERE A.StockState=@StockState AND A.AuditTime between @StatisticStartDate and @StatisticEndDate
) t1
inner join StorageRecordDetail t2 WITH(NOLOCK) on t2.StockId=t1.StockId
where not exists (
		select top 1 1
		from SaleOrderGoodsSettlement SOGS
		where SOGS.FilialeId=t1.FilialeId and SOGS.GoodsId=t2.GoodsId and SOGS.RelatedTradeNo=t1.TradeCode
	)
";
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.Query(string.Format(SQL, maxRowCount), new
                {
                    StockState = (int)StorageRecordState.Finished,
                    StatisticStartDate = startDate,
                    StatisticEndDate = endDate
                }).GroupBy(g => (string)g.TradeNo)
                .Select(m => new SampleSaleOrderInfo
                {
                    TradeType = SaleOrderGoodsSettlementRelatedTradeType.SaleStockOutForShop,
                    TradeNo = m.Key,
                    FilialeId = (Guid)m.Last().FilialeId,
                    OccurTime = (DateTime)m.Last().OccurTime,
                    GoodsIds = new List<Guid>(m.Select(s => (Guid)s.GoodsId))
                })
                .AsList();
            }
        }

        public IList<SampleSaleOrderInfo> GetUnsavedSaleStockOutForSaleFilialeList(DateTime statisticDate, int maxRowCount)
        {
            DateTime startDate = statisticDate.Date;
            DateTime endDate = statisticDate.Date.AddDays(1).AddSeconds(-1);
            const string SQL = @"
select	distinct t1.TradeCode TradeNo
		,t1.AuditTime OccurTime
		,t1.FilialeId
		,t2.GoodsId
from (
	SELECT top {0} StockId,TradeCode,AuditTime,FilialeId
	FROM StorageRecord WITH(NOLOCK) 
	WHERE StockState=@StockState AND AuditTime between @StatisticStartDate and @StatisticEndDate
        and TradeBothPartiesType=@TradeBothPartiesType
) t1
inner join StorageRecordDetail t2 WITH(NOLOCK) on t2.StockId=t1.StockId
where not exists (
		select top 1 1
		from SaleOrderGoodsSettlement SOGS
		where SOGS.FilialeId=t1.FilialeId and SOGS.GoodsId=t2.GoodsId and SOGS.RelatedTradeNo=t1.TradeCode
	)
";
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.Query(string.Format(SQL, maxRowCount), new
                {
                    StockState = (int)StorageRecordState.Finished,
                    StatisticStartDate = startDate,
                    StatisticEndDate = endDate,
                    TradeBothPartiesType = (int)TradeBothPartiesType.HostingToSale
                }).GroupBy(g => (string)g.TradeNo)
                .Select(m => new SampleSaleOrderInfo
                {
                    TradeType = SaleOrderGoodsSettlementRelatedTradeType.SaleStockOutForSaleFiliale,
                    TradeNo = m.Key,
                    FilialeId = (Guid)m.Last().FilialeId,
                    OccurTime = (DateTime)m.Last().OccurTime,
                    GoodsIds = new List<Guid>(m.Select(s => (Guid)s.GoodsId))
                })
                .AsList();
            }
        }

        public IList<SampleSaleOrderInfo> GetUnsavedSaleStockOutForHostingFilialeList(DateTime statisticDate, int maxRowCount)
        {
            DateTime startDate = statisticDate.Date;
            DateTime endDate = statisticDate.Date.AddDays(1).AddSeconds(-1);
            const string SQL = @"
select	distinct t1.TradeCode TradeNo
		,t1.AuditTime OccurTime
		,t1.FilialeId
		,t2.GoodsId
from (
	SELECT top {0} StockId,TradeCode,AuditTime,FilialeId
	FROM StorageRecord WITH(NOLOCK) 
	WHERE StockState=@StockState AND AuditTime between @StatisticStartDate and @StatisticEndDate
        and TradeBothPartiesType=@TradeBothPartiesType
) t1
inner join StorageRecordDetail t2 WITH(NOLOCK) on t2.StockId=t1.StockId
where not exists (
		select top 1 1
		from SaleOrderGoodsSettlement SOGS
		where SOGS.FilialeId=t1.FilialeId and SOGS.GoodsId=t2.GoodsId and SOGS.RelatedTradeNo=t1.TradeCode
	)
";
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.Query(string.Format(SQL, maxRowCount), new
                {
                    StockState = (int)StorageRecordState.Finished,
                    StatisticStartDate = startDate,
                    StatisticEndDate = endDate,
                    TradeBothPartiesType = (int)TradeBothPartiesType.HostingToSale
                }).GroupBy(g => (string)g.TradeNo)
                .Select(m => new SampleSaleOrderInfo
                {
                    TradeType = SaleOrderGoodsSettlementRelatedTradeType.SaleStockOutForHostingFiliale,
                    TradeNo = m.Key,
                    FilialeId = (Guid)m.Last().FilialeId,
                    OccurTime = (DateTime)m.Last().OccurTime,
                    GoodsIds = new List<Guid>(m.Select(s => (Guid)s.GoodsId))
                })
                .AsList();
            }
        }

        public void Save(SaleOrderGoodsSettlementInfo info)
        {
            if (info == null)
            {
                return;
            }
            const string SQL = @"
if not exists(select top 1 1 from SaleOrderGoodsSettlement where RelatedTradeNo=@RelatedTradeNo and GoodsId=@GoodsId)
begin
    INSERT INTO [SaleOrderGoodsSettlement]
               ([FilialeId]
               ,[GoodsId]
               ,[SettlementPrice]
               ,[RelatedTradeType]
               ,[RelatedTradeNo]
               ,[OccurTime]
               ,[CreateTime])
         VALUES
                (@FilialeId
                ,@GoodsId
                ,@SettlementPrice
                ,@RelatedTradeType
                ,@RelatedTradeNo
                ,@OccurTime
                ,getdate())
end
";
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                conn.Execute(SQL, new
                {
                    FilialeId = info.FilialeId,
                    GoodsId = info.GoodsId,
                    SettlementPrice = info.SettlementPrice,
                    RelatedTradeType = info.RelatedTradeType,
                    RelatedTradeNo = info.RelatedTradeNo,
                    OccurTime = info.OccurTime,
                });
            }
        }
    }
}
