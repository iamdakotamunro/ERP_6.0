using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using ERP.DAL.Interface.IGoods;
using ERP.Environment;
using ERP.Model;
using Keede.DAL.RWSplitting;
using Dapper;
using System.Transactions;

namespace ERP.DAL.Implement.Goods
{
    public class GoodsStockPile : IGoodsStockPile
    {
        public GoodsStockPile(GlobalConfig.DB.FromType fromType) { }

        /// <summary>得到平均库存周转 2015-04-29  陈重文
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">截至时间</param>
        /// <param name="warehouseId">仓库ID</param>
        /// <param name="goodsIds">商品ID集合</param>
        /// <param name="state">0全部，1下架或缺货且有库存，2无销售商品</param>
        /// <returns></returns>
        public IList<StockTurnOverInfo> GetAvgStockTurnOver(DateTime startTime, DateTime endTime, Guid warehouseId, List<Guid> goodsIds, int state)
        {
            var sql = @"select DayTime as CreateDate, convert(decimal(18,4),sum(Result)) as  AvgTurnOver from 
(
	select 
		 i1.GoodsId,i1.DayTime,i1.GoodsStock,i1.SaleNum,i1.TurnOver,i2.TotalGoodsStockPrice
		 ,(
			 (
			 case 
			  when  i1.TurnOver = 0 then  0 
			  else  case when i2.TotalGoodsStockPrice=0 then 0 else ((i1.SettlementPrice*i1.GoodsStock)/i2.TotalGoodsStockPrice)* i1.TurnOver end
			 end
			 )
		 ) as Result
	from  (
			SELECT
				  GoodsId,(
						  case when AvgSaleNum=0 then 0
				          else GoodsStock/AvgSaleNum
				          end) as TurnOver ,SaleNum,GoodsStock,DayTime ,SettlementPrice
			FROM [dbo].[GoodsStockTurnOver]  
			where  DayTime between @StartTime and @EndTime and WarehouseId=@WarehouseId ";

            string sql2 = @" ) I1  inner join (
		SELECT 
			   SUM(GoodsStock*SettlementPrice) AS TotalGoodsStockPrice,DayTime 
		FROM [dbo].[GoodsStockTurnOver]  
        where  DayTime between @StartTime and @EndTime and WarehouseId=@WarehouseId ";

            const string SQL3 = @"group by DayTime
	) I2  on  i1.DayTime   = i2.DayTime
) i3 
group by i3.DayTime
order by DayTime asc";

            if (goodsIds.Count > 0)
            {
                var goodsIdsStr = string.Format(" and GoodsId in(select id as GoodsId from splitToTable('{0}',','))", string.Join(",", goodsIds.ToArray()));
                sql = sql + goodsIdsStr;
                sql2 = sql2 + goodsIdsStr;
            }
            //下架或缺货且有库存
            if (state == 1)
            {
                const string STR_SQL = " and (IsScarcity=1 or PurchaseState=0 and GoodsStock<>0)";
                sql = sql + STR_SQL;
                sql2 = sql2 + STR_SQL;
            }
            //无销售商品
            if (state == 2)
            {
                const string STR_SQL = " and SaleNum=0";
                sql = sql + STR_SQL;
                sql2 = sql2 + STR_SQL;
            }
            var realSql = sql + sql2 + SQL3;

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.Query<StockTurnOverInfo>(realSql, new
                {
                    StartTime = startTime,
                    EndTime = endTime,
                    WarehouseId = warehouseId
                }).AsList();
            }
        }

        /// <summary>获取商品的库存周转率  2015-04-30 陈重文
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="goodsId">商品ID</param>
        /// <param name="warehouseId">仓库ID</param>
        /// <returns></returns>
        public IList<StockTurnOverInfo> GetGoodsStockTurnOverByGoodsId(DateTime startTime, DateTime endTime, Guid goodsId, Guid warehouseId)
        {
            const string SQL = @"
SELECT GoodsId,( case when AvgSaleNum=0 then 0
				          else GoodsStock/AvgSaleNum
				          end) as AvgTurnOver,SaleNum SaleNums,GoodsStock StockNums,DayTime CreateDate
FROM [dbo].[GoodsStockTurnOver]  
where  DayTime between @StartTime and @EndTime and WarehouseId=@WarehouseId
			      and GoodsId=@GoodsId			      
 order by DayTime";

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.Query<StockTurnOverInfo>(SQL, new
                {
                    StartTime = startTime,
                    EndTime = endTime,
                    GoodsId = goodsId,
                    WarehouseId = warehouseId
                }).AsList();
            }
        }

        /// <summary>获取商品近3个月内销量情况  2015-06-16  陈重文
        /// </summary>
        /// <param name="warehouseId">仓库ID</param>
        /// <returns></returns>
        public IList<SalesVolumeInfo> GetGoodsSalesVolume(Guid warehouseId)
        {
            #region [SQL]
            const string SQL = @"
select 
		i1.GoodsID, sum(i1.ThirtyDaySales) as ThirtyDaySales,sum(i1.SixtyDaySales) as SixtyDaySales,sum(i1.NinetyDaySales) as NinetyDaySales
 from 
(
		select GoodsID,
		sum(goodsSales) as ThirtyDaySales,
		0 as  SixtyDaySales,
		0 as  NinetyDaySales
		from lmshop_GoodsDaySalesStatistics with(nolock)
		where DayTime between dateadd(day,-30,convert(varchar(10),GETDATE(),120)) and GETDATE() 
		and  DeliverWarehouseId=@WarehouseId
		group by GoodsID --第前1个月销量
		union all
		select GoodsID,
		0 as ThirtyDaySales,
		sum(goodsSales) as SixtyDaySales ,
		0 as NinetyDaySales
		from lmshop_GoodsDaySalesStatistics with(nolock)
		where DayTime between dateadd(day,-60,convert(varchar(10),GETDATE(),120)) and dateadd(day,-30,convert(varchar(10),GETDATE(),120)) 
		and  DeliverWarehouseId=@WarehouseId
		group by GoodsID  --第前2个月销量
		union all
		select GoodsID,
		0 as ThirtyDaySales,
		0 as  SixtyDaySales,
		sum(goodsSales) as  NinetyDaySales 
		from lmshop_GoodsDaySalesStatistics with(nolock)
		where DayTime between dateadd(day,-90,convert(varchar(10),GETDATE(),120)) and dateadd(day,-60,convert(varchar(10),GETDATE(),120)) 
		and  DeliverWarehouseId=@WarehouseId
		group by GoodsID  --第前3个月销量
) i1 group by i1.GoodsID
order by ThirtyDaySales desc,SixtyDaySales desc,NinetyDaySales desc";
            #endregion

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.Query<SalesVolumeInfo>(SQL, new
                {
                    WarehouseId = warehouseId
                }).AsList();
            }
        }
    }
}
