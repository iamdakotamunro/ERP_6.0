using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using ERP.DAL.Interface.IOrder;
using ERP.Enum;
using ERP.Environment;
using ERP.Model.Goods;
using ERP.Model.Report;
using Keede.Ecsoft.Model;
using ERP.Model;
using Keede.DAL.RWSplitting;
using Dapper;
using Keede.DAL.Helper;
using System.Transactions;

namespace ERP.DAL.Implement.Order
{
    public class GoodsOrderDetail : IGoodsOrderDetail
    {
        public GoodsOrderDetail(Environment.GlobalConfig.DB.FromType fromType) { }

        public GoodsOrderDetail() { }

        private const string SQL_INSERT_GOODSORDERDETAIL = "INSERT INTO lmShop_GoodsOrderDetail(OrderId,GoodsID,RealGoodsID,CompGoodsId,CompIndex,GoodsName,GoodsCode,Specification,MarketPrice,SellPrice,Quantity,TotalPrice,GiveScore,PurchaseSpecification,IsProcess,BuyType,GoodsScore,TotalGoodsScore,SellType,GoodsType,ScoreDeductionMoneyUpper,OriginalSellPrice,PromotionId,PromotionDeductionAmount) VALUES(@OrderId,@GoodsID,@RealGoodsID,@CompGoodsId,@CompIndex,@GoodsName,@GoodsCode,@Specification,@MarketPrice,@SellPrice,@Quantity,@TotalPrice,@GiveScore,@PurchaseSpecification,@IsProcess,@BuyType,@GoodsScore,@TotalGoodsScore,@SellType,@GoodsType,@ScoreDeductionMoneyUpper,@OriginalSellPrice,@PromotionId,@PromotionDeductionAmount);";

        private const string SQL_SELECT_GOODSORDERDETAIL_BY_ORDER = "SELECT OrderId,GoodsID,RealGoodsID,CompGoodsId,CompIndex,GoodsName,GoodsCode,Specification,MarketPrice,SellPrice,Quantity,TotalPrice,GiveScore,PurchaseSpecification,BuyType,GoodsScore,TotalGoodsScore,IsProcess,SellType,GoodsType,ScoreDeductionMoneyUpper,OriginalSellPrice FROM lmshop_GoodsOrderDetail WITH(NOLOCK) WHERE OrderId=@OrderId ORDER BY GoodsCode ASC,Specification ASC;";


        private const string SQL_SELECT_GOODSORDERDETAIL_BY_ORDERID = @"SELECT [OrderId]
																			  ,[GoodsID]
																			  ,[RealGoodsID]
																			  ,[CompGoodsId]
																			  ,[CompIndex]
																			  ,[GoodsName]
																			  ,[GoodsCode]
																			  ,[Specification]
																			  ,[MarketPrice]
																			  ,[SellPrice]
																			  ,[Quantity]
																			  ,[TotalPrice]
																			  ,[GiveScore]
																			  ,[PurchaseSpecification]
																			  ,[IsProcess]
																			  ,[BuyType]
																			  ,[GoodsScore]
																			  ,[TotalGoodsScore]
																			  ,[SellType]
																			  ,[GoodsType]
																			  ,[ScoreDeductionMoneyUpper]
                                                                              ,[OriginalSellPrice]
																		  FROM [lmShop_GoodsOrderDetail] WITH(NOLOCK) WHERE OrderId=@OrderId";

        /// <summary>
        /// 添加订单明细
        /// </summary>
        /// <param name="goodsOrderDetailList"></param>
        /// <param name="goodsOrder"></param>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        public bool Insert(IList<GoodsOrderDetailInfo> goodsOrderDetailList, GoodsOrderInfo goodsOrder, out string errorMsg)
        {
            errorMsg = string.Empty;
            if (goodsOrderDetailList == null)
            {
                errorMsg = "订单详细商品不能空";
                return false;
            }

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        foreach (GoodsOrderDetailInfo goodsOrderDetail in goodsOrderDetailList)
                        {
                            goodsOrderDetail.PurchaseSpecification = goodsOrderDetail.PurchaseSpecification ?? string.Empty;
                            if (!string.IsNullOrEmpty(goodsOrderDetail.Specification))
                            {
                                goodsOrderDetail.Specification = goodsOrderDetail.Specification.Trim().Replace("  ", " ") + " ";
                            }
                            if (!string.IsNullOrEmpty(goodsOrderDetail.PurchaseSpecification))
                            {
                                goodsOrderDetail.PurchaseSpecification = goodsOrderDetail.PurchaseSpecification.Trim().Replace("  ", " ") + " ";
                            }

                            decimal promotionDeductionAmount = 0;
                            decimal pointsDeductionAmount = 0;
                            if (goodsOrder.TotalPrice.Equals(0) || goodsOrderDetail.TotalPrice.Equals(0))
                            {
                                promotionDeductionAmount = 0;
                            }
                            else
                            {
                                promotionDeductionAmount = Math.Round((goodsOrderDetail.TotalPrice / goodsOrder.TotalPrice) * goodsOrder.PromotionValue, 2);

                                if (goodsOrder.ScoreDeduction.Equals(0) || goodsOrder.ScoreDeductionProportion.Equals(0))
                                {
                                    pointsDeductionAmount = 0;
                                }
                                else
                                {
                                    pointsDeductionAmount = Math.Round((goodsOrderDetail.TotalPrice / goodsOrder.TotalPrice) * Convert.ToDecimal(goodsOrder.ScoreDeduction / goodsOrder.ScoreDeductionProportion), 2);
                                }
                            }

                            goodsOrderDetail.PromotionDeductionAmount = promotionDeductionAmount + pointsDeductionAmount;

                            conn.Execute(SQL_INSERT_GOODSORDERDETAIL, new
                            {
                                OrderId = goodsOrderDetail.OrderId,
                                GoodsID = goodsOrderDetail.GoodsID,
                                RealGoodsID = goodsOrderDetail.RealGoodsID,
                                CompGoodsId = goodsOrderDetail.CompGoodsId,
                                CompIndex = goodsOrderDetail.CompIndex,
                                GoodsName = goodsOrderDetail.GoodsName,
                                GoodsCode = goodsOrderDetail.GoodsCode,
                                Specification = goodsOrderDetail.Specification,
                                MarketPrice = goodsOrderDetail.MarketPrice,
                                SellPrice = goodsOrderDetail.SellPrice,
                                Quantity = goodsOrderDetail.Quantity,
                                TotalPrice = goodsOrderDetail.TotalPrice,
                                GiveScore = goodsOrderDetail.GiveScore,
                                PurchaseSpecification = goodsOrderDetail.PurchaseSpecification,
                                IsProcess = goodsOrderDetail.IsProcess,
                                BuyType = goodsOrderDetail.BuyType,
                                GoodsScore = goodsOrderDetail.GoodsScore,
                                TotalGoodsScore = goodsOrderDetail.TotalGoodsScore,
                                SellType = goodsOrderDetail.SellType,
                                GoodsType = goodsOrderDetail.GoodsType,
                                ScoreDeductionMoneyUpper = goodsOrderDetail.ScoreDeductionMoneyUpper,
                                OriginalSellPrice = goodsOrderDetail.OriginalSellPrice,
                                PromotionId = goodsOrderDetail.PromotionId,
                                PromotionDeductionAmount = goodsOrderDetail.PromotionDeductionAmount
                            }, trans);
                        }

                        trans.Commit();
                        return true;

                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        errorMsg = ex.Message;
                        return false;
                    }
                }
            }
        }

        public IList<GoodsOrderDetailInfo> GetGoodsOrderDetailList(Guid orderId)
        {
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.Query<GoodsOrderDetailInfo>(SQL_SELECT_GOODSORDERDETAIL_BY_ORDER, new { OrderId = orderId }).AsList();
            }
        }

        public IList<GoodsOrderDetailInfo> GetGoodsOrderDetailList(Guid orderId, DateTime orderTime)
        {
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.GetErpDbName(orderTime.Year), Transaction.Current == null))
            {
                return conn.Query<GoodsOrderDetailInfo>(SQL_SELECT_GOODSORDERDETAIL_BY_ORDER, new { OrderId = orderId }).AsList();
            }
        }

        /// <summary> 保存商品每日销售量
        /// </summary>
        /// <param name="list"></param>
        /// <param name="flag">是否是售后(True:是;False:否;)</param>
        public void SaveGoodsSales(List<GoodsDaySalesStatisticsInfo> list, bool flag = false)
        {
            if (list.Count == 0) return;
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        foreach (var info in list)
                        {
                            string sql = string.Format(@"
DECLARE @NowGoodsSales INT,@TempSales decimal(18,2),@Id INT

SELECT @NowGoodsSales=GoodsSales,@Id=ID
FROM lmshop_GoodsDaySalesStatistics
WHERE DeliverWarehouseId=@DeliverWarehouseId 
AND SaleFilialeId=@SaleFilialeId 
AND SalePlatformId=@SalePlatformId 
AND RealGoodsId=@RealGoodsId 
AND DayTime=@DayTime {0} {1}	   

SET @TempSales = @NowGoodsSales+@GoodsSales

IF(@TempSales=0 OR @NowGoodsSales=0)	
 BEGIN 
	DELETE lmshop_GoodsDaySalesStatistics 
	WHERE ID = @Id
 END
ELSE
 BEGIN      
	IF EXISTS(SELECT TOP 1 GoodsId FROM lmshop_GoodsDaySalesStatistics
	 WHERE DeliverWarehouseId=@DeliverWarehouseId 
	 AND SaleFilialeId=@SaleFilialeId AND SalePlatformId=@SalePlatformId AND RealGoodsId=@RealGoodsId AND DayTime=@DayTime {0} {1})
		BEGIN	         
			UPDATE lmshop_GoodsDaySalesStatistics 
			SET GoodsSales=GoodsSales+@GoodsSales,Sellprice=Sellprice+@SellPrice,AvgSettlePrice=AvgSettlePrice+@AvgSettlePrice 
			WHERE DeliverWarehouseId=@DeliverWarehouseId 
			AND SaleFilialeId=@SaleFilialeId AND SalePlatformId=@SalePlatformId AND RealGoodsId=@RealGoodsId AND DayTime=@DayTime {0} {1}		
		END
	ELSE
	BEGIN
		INSERT INTO  lmshop_GoodsDaySalesStatistics (DeliverWarehouseId,SaleFilialeId,SalePlatformId,GoodsId,
		RealGoodsId,DayTime,GoodsSales,Specification,Sellprice,GoodsName,ClassId,GoodsCode,SeriesId,BrandId,AvgSettlePrice,HostingFilialeId)
		Values(@DeliverWarehouseId,@SaleFilialeId,@SalePlatformId,@GoodsId,@RealGoodsId,@DayTime,@GoodsSales,
		@Specification,@SellPrice,@GoodsName,@ClassId,@GoodsCode,@SeriesId,@BrandId,@AvgSettlePrice,@HostingFilialeId)
	END
END", info.SellPrice == 0 ? " AND Sellprice=0" : " AND Sellprice<>0", flag ? " AND GoodsSales<0" : " AND GoodsSales>=0");
                            conn.Execute(sql, new
                            {
                                DeliverWarehouseId = info.DeliverWarehouseId,
                                SaleFilialeId = info.SaleFilialeId,
                                SalePlatformId = info.SalePlatformId,
                                GoodsId = info.GoodsId,
                                RealGoodsId = info.RealGoodsId,
                                GoodsSales = info.GoodsSales,
                                DayTime = Convert.ToDateTime(info.DayTime.ToString("yyyy-MM-dd")),
                                Specification = info.Specification ?? string.Empty,
                                SellPrice = info.SellPrice,
                                GoodsName = info.GoodsName,
                                ClassId = info.ClassId,
                                GoodsCode = info.GoodsCode,
                                SeriesId = info.SeriesId,
                                BrandId = info.BrandId,
                                AvgSettlePrice = info.AvgSettlePrice,
                                HostingFilialeId = info.HostingFilialeId,
                            }, trans);
                        }
                        trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    finally
                    {
                        trans.Dispose();
                    }
                }
            }
        }

        /// <summary>根据订单ID查询订单信息
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public IList<GoodsOrderDetailInfo> GetGoodsOrderDetailByOrderId(Guid orderId)
        {
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.Query<GoodsOrderDetailInfo>(SQL_SELECT_GOODSORDERDETAIL_BY_ORDERID, new { OrderId = orderId }).AsList();
            }
        }

        /// <summary> 根据OrderId集合查询GoodsId
        /// </summary>
        /// <param name="orderIds"></param>
        /// <returns></returns>
        /// zal 2016-05-20
        public List<Guid> GetGoodsIdByOrderId(List<Guid> orderIds)
        {
            var orderIdList = new List<Guid>();
            if (orderIds.Any())
            {
                string sql = "select distinct GoodsId from lmShop_GoodsOrderDetail WITH(NOLOCK) where 1=1 ";
                string orderIdStr = "'" + string.Join("','", orderIds.ToArray()) + "'";
                string newSql = sql + string.Format(" AND OrderId IN({0}) ", orderIdStr);

                using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
                {
                    orderIdList = conn.Query<Guid>(newSql).AsList();
                }
            }
            return orderIdList;
        }

        /// <summary>
        /// 根据OrderId集合获取订单明细数据
        /// </summary>
        /// <param name="orderIds"></param>
        /// <returns></returns>
        /// zal 2016-07-08
        public IList<GoodsOrderDetailInfo> GetGoodsOrderDetailByOrderIds(List<Guid> orderIds)
        {
            IList<GoodsOrderDetailInfo> goodsOrderDetailList = new List<GoodsOrderDetailInfo>();
            if (!orderIds.Any())
            {
                return goodsOrderDetailList;
            }

            string sql = @"
                     SELECT N.[OrderId],N.[GoodsID],N.[Quantity],M.[OrderTime]
                     FROM [lmShop_GoodsOrderDetail] N WITH(NOLOCK)
                     INNER JOIN [lmShop_GoodsOrder] M WITH(NOLOCK) ON N.OrderId=M.OrderId	 
					 WHERE 1=1 ";

            string orderIdStr = "'" + string.Join(",", orderIds.ToArray()) + "'";
            string newSql = sql + (string.IsNullOrWhiteSpace(orderIdStr) ? "" : string.Format(" AND EXISTS(select id as OrderId from splitToTable({0},',') #temp WHERE #temp.id=M.OrderId) ", orderIdStr));

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                try
                {
                    return conn.Query<GoodsOrderDetailInfo>(newSql).AsList();
                }
                catch (Exception ex)
                {
                    throw new Exception("获取订单明细数据异常", ex);
                }
            }
        }

        /// <summary>根据订单时间获取日销量表中的goodsId
        /// </summary>
        /// <param name="startTime">订单时间</param>
        /// <param name="endTime">订单时间</param>
        /// <returns></returns>
        /// zal 2016-06-06
        public IList<Guid> GetGoodsDaySalesStatisticsList(DateTime startTime, DateTime endTime)
        {
            IList<Guid> goodsIds = new List<Guid>();
            string sql = @"select distinct GoodsID from lmshop_GoodsDaySalesStatistics with(nolock) where DayTime>='" + startTime + "' and DayTime<'" + endTime + "' ";

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                try
                {
                    return conn.Query<Guid>(sql).AsList();
                }
                catch (Exception ex)
                {
                    throw new Exception("获取日销量表GoodsId失败！", ex);
                }
            }
        }

        /// <summary> 根据goodsId修改指定时间内的“月平均结算价”
        /// </summary>
        /// <param name="goodsId"></param>
        /// <param name="startTime">订单时间</param>
        /// <param name="endTime">订单时间</param>
        /// <param name="avgSettlePrice">月平均结算价</param>
        /// <returns></returns>
        /// zal 2016-06-20
        public bool UpdateGoodsDaySalesStatisticsAvgSettlePrice(Guid goodsId, decimal avgSettlePrice, DateTime startTime, DateTime endTime)
        {
            string sql = @"
            update lmshop_GoodsDaySalesStatistics
            set AvgSettlePrice=@AvgSettlePrice
            where GoodsID=@GoodsID and DayTime>=@StartTime and DayTime<@EndTime";

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                try
                {
                    return conn.Execute(sql, new
                    {
                        GoodsID = goodsId,
                        AvgSettlePrice = avgSettlePrice,
                        StartTime = startTime,
                        EndTime = endTime,
                    }) > 0;
                }
                catch (Exception ex)
                {
                    throw new Exception("更新日销量表GoodsId失败！", ex);
                }
            }
        }

        public bool InsertASYN_GoodsDaySalesStatisticsInfo(ASYN_GoodsDaySalesStatisticsInfo asynGoodsDaySalesStatisticsInfo)
        {
            try
            {
                const string sql = @"INSERT INTO [dbo].[ASYN_GoodsDaySalesStatistics]
                                                   ([ID]
                                                   ,[OrderNo]
                                                   ,[OrderJsonStr]
                                                   ,[OrderDetailJsonStr]
                                                   ,[OldOrderDetailJsonStr]
                                                   ,[HandlingStatus]
                                                   ,[CreateDate]
                                                   ,[IsExecuted])
                                                    VALUES (NEWID(),@OrderNo,@OrderJsonStr,@OrderDetailJsonStr,@OldOrderDetailJsonStr,@HandlingStatus,GETDATE(),0)";

                using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
                {
                    return conn.Execute(sql, new
                    {
                        OrderNo = asynGoodsDaySalesStatisticsInfo.OrderNo,
                        OrderJsonStr = asynGoodsDaySalesStatisticsInfo.OrderJsonStr,
                        OrderDetailJsonStr = asynGoodsDaySalesStatisticsInfo.OrderDetailJsonStr,
                        OldOrderDetailJsonStr = asynGoodsDaySalesStatisticsInfo.OldOrderDetailJsonStr,
                        HandlingStatus = asynGoodsDaySalesStatisticsInfo.HandlingStatus,
                    }) > 0;
                }
            }
            catch (Exception ex)
            {
                SAL.LogCenter.LogService.LogError("Error", "InsertASYN_GoodsDaySalesStatisticsInfo.Error", ex);
                return false;
            }
        }

        public List<ASYN_GoodsDaySalesStatisticsInfo> GetAsynGoodsDaySalesStatisticsList()
        {
            var sql = String.Format(@"SELECT
                   [ID]
                  ,[OrderNo]
                  ,[OrderJsonStr]
                  ,[OrderDetailJsonStr]
                  ,[OldOrderDetailJsonStr]
                  ,[HandlingStatus]
                  ,[CreateDate]
                  ,[IsExecuted]
              FROM [dbo].[ASYN_GoodsDaySalesStatistics] 
              WHERE IsExecuted=0
              ORDER BY CreateDate ASC");

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.Query<ASYN_GoodsDaySalesStatisticsInfo>(sql).AsList();
            }
        }

        public void SetAsynGoodsDaySalesStatisticsExecuted(Guid id)
        {
            var sql = String.Format("UPDATE [dbo].[ASYN_GoodsDaySalesStatistics]  SET IsExecuted=1 WHERE ID='{0}'", id);
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                conn.Execute(sql);
            }
        }

        /// <summary> 根据订单号获得订单明细数据
        /// </summary>
        /// <param name="orderId"></param>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-06-30
        public List<GoodsOrderDetails> GetGoodsOrderDetailsByOrderId(Guid orderId)
        {
            var builder = new StringBuilder(@"select GoodsId,GoodsName,Quantity,Specification,SellPrice,TotalPrice from lmShop_GoodsOrderDetail where OrderId=@OrderId");

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.Query<GoodsOrderDetails>(builder.ToString(), new
                {
                    OrderId = orderId
                }).AsList();
            }
        }

        public List<GoodsOrderDetailBaseInfo> GetGoodsQuantityDics(Guid warehouseId, DateTime startTime,
            DateTime endTime, Guid personResponsible)
        {
            var datasource = new List<GoodsOrderDetailBaseInfo>();
            string warehouseStr = warehouseId != Guid.Empty
                ? string.Format(" AND WarehouseId='{0}'", warehouseId)
                : "";

            string SQL = @"
SELECT DISTINCT T1.OrderNo,T1.GoodsId,T1.GoodsName,T1.GoodsCode,T1.EffectiveTime,T1.Consignee,T1.SaleFilialeId FROM (
    SELECT OrderNo,GoodsId,RealGoodsId,GoodsName,GoodsCode,EffectiveTime,Consignee,SaleFilialeId,SUM(Quantity) AS Quantity 
    FROM lmShop_GoodsOrderDetail AS GD WITH(NOLOCK)
    INNER JOIN lmShop_GoodsOrder AS G WITH(NOLOCK) on GD.OrderId=G.OrderId 
    WHERE G.OrderState=@OrderState AND G.OrderTime BETWEEN @StartTime and @EndTime AND G.DeliverWarehouseId='" + warehouseId + @"' 
    GROUP BY GoodsId,RealGoodsId,OrderNo,GoodsName,GoodsCode,EffectiveTime,Consignee,SaleFilialeId
) AS T1
INNER JOIN lmshop_PurchaseSet AS PS WITH(NOLOCK) ON PS.GoodsId=T1.GoodsId 
WHERE PersonResponsible=@PersonResponsible {0}";

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.Query<GoodsOrderDetailBaseInfo>(string.Format(SQL, warehouseStr), new
                {
                    StartTime = startTime,
                    EndTime = endTime,
                    OrderState = (int)OrderState.RequirePurchase,
                    PersonResponsible = personResponsible,
                }).AsList();
            }
        }

        public List<GoodsOrderDemandInfo> GetGoodsOrderDemandInfos(Guid warehouseId, Guid realGoodsId)
        {
            var datasource = new List<GoodsOrderDemandInfo>();
            const string SQL = @"select G.OrderId,OrderNo,SaleFilialeId,SUM(Quantity) AS Quantity from lmshop_GoodsOrderDetail AS GD WITH(NOLOCK) 
inner join lmshop_GoodsOrder AS G WITH(NOLOCK) ON GD.OrderId=G.OrderId AND DeliverWarehouseId=@WarehouseId AND G.OrderState IN({0})
where RealGoodsId=@RealGoodsId 
GROUP BY G.OrderId,OrderNo,SaleFilialeId";

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.Query<GoodsOrderDemandInfo>(string.Format(SQL, string.Join(",", (int)OrderState.Redeploy, (int)OrderState.RequirePurchase, (int)OrderState.WaitOutbound)), new
                {
                    WarehouseId = warehouseId,
                    RealGoodsId = realGoodsId,
                }).AsList();
            }
        }

        /// <summary>根据年份和指定条件查询订单明细
        /// </summary>
        /// <param name="orderTime"></param>
        /// <param name="orderId"></param>
        /// <param name="orderNo"></param>
        /// <param name="keepyear"></param>
        /// <returns></returns>
        public IList<GoodsOrderDetailInfo> GetGoodsOrderDetailList(DateTime orderTime, Guid orderId, string orderNo, int keepyear)
        {
            IList<GoodsOrderDetailInfo> goodsOrderDetailList = new List<GoodsOrderDetailInfo>();
            if (!string.IsNullOrEmpty(orderNo) || orderId != Guid.Empty)
            {
                var sql = new StringBuilder();
                sql.Append("SELECT [OrderId],[GoodsID],[RealGoodsID],[CompGoodsId],[CompIndex],[GoodsName],[GoodsCode],[Specification],[MarketPrice],[SellPrice],[Quantity],[TotalPrice],[GiveScore],[PurchaseSpecification],[IsProcess],[BuyType],[GoodsScore],[TotalGoodsScore],[SellType],[GoodsType],[ScoreDeductionMoneyUpper] FROM [lmShop_GoodsOrderDetail] WITH(NOLOCK) ");
                if (!string.IsNullOrEmpty(orderNo) && orderId == Guid.Empty)
                {
                    sql.Append(" WHERE [OrderId] in (SELECT TOP 1 [OrderId] FROM [lmshop_GoodsOrder] WITH(NOLOCK) WHERE [OrderNo]=@OrderNo)");
                }
                else if (string.IsNullOrEmpty(orderNo) && orderId != Guid.Empty)
                {
                    sql.Append(" WHERE [OrderId]='").Append(orderId).Append("'");
                }
                else if (!string.IsNullOrEmpty(orderNo) && orderId != Guid.Empty)
                {
                    sql.Append(" WHERE [OrderId] in (SELECT TOP 1 [OrderId] FROM [lmshop_GoodsOrder] WITH(NOLOCK) WHERE [OrderNo]=@OrderNo OR [OrderId]='").Append(orderId).Append("')");
                }

                using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.GetErpDbName(orderTime.Year), Transaction.Current == null))
                {
                    goodsOrderDetailList = conn.Query<GoodsOrderDetailInfo>(sql.ToString(), new
                    {
                        OrderNo = orderNo,
                    }).AsList();
                }
            }
            return goodsOrderDetailList;
        }

        public List<NeedPurchasingGoods> GetNeedPurchasingGoodses(Guid warehouseId, Guid personResponsible, DateTime startTime, DateTime endTime, IEnumerable<int> orderStates)
        {
            const string SQL = @"
SELECT T1.GoodsId,T1.OrderNo,T1.RealGoodsId,T1.Quantity,PS.PersonResponsible FROM (
    SELECT OrderNo,GoodsId,RealGoodsId,SUM(Quantity) AS Quantity 
    FROM lmShop_GoodsOrderDetail AS GD WITH(NOLOCK)
    INNER JOIN lmShop_GoodsOrder AS G WITH(NOLOCK) on GD.OrderId=G.OrderId 
    WHERE G.OrderState IN({0}) AND G.OrderTime BETWEEN @StartTime AND @EndTime AND G.DeliverWarehouseId=@WarehouseId
    GROUP BY GoodsId,RealGoodsId,OrderNo
) AS T1
INNER JOIN lmshop_PurchaseSet AS PS WITH(NOLOCK) ON PS.GoodsId=T1.GoodsId 
WHERE WarehouseId=@WarehouseId AND PersonResponsible=@PersonResponsible";

            const string SQL_WITHOUT_PERSONEL = @"SELECT T1.GoodsId,T1.OrderNo,T1.RealGoodsId,T1.Quantity,PS.PersonResponsible FROM (
SELECT OrderNo,GoodsId,RealGoodsId,SUM(Quantity) AS Quantity FROM lmShop_GoodsOrderDetail AS GD WITH(NOLOCK)
INNER JOIN lmShop_GoodsOrder AS G WITH(NOLOCK) on GD.OrderId=G.OrderId WHERE G.OrderState IN({0}) AND G.OrderTime BETWEEN @StartTime AND @EndTime 
GROUP BY GoodsId,RealGoodsId,OrderNo) AS T1
INNER JOIN lmshop_PurchaseSet AS PS WITH(NOLOCK) ON PS.GoodsId=T1.GoodsId 
WHERE WarehouseId=@WarehouseId ";

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                var result = personResponsible != Guid.Empty ? conn.Query<NeedPurchasingGoods>(string.Format(SQL, string.Join(",", orderStates)), new
                {
                    StartTime = startTime,
                    EndTime = endTime,
                    WarehouseId = warehouseId,
                    PersonResponsible = personResponsible,
                }) : conn.Query<NeedPurchasingGoods>(string.Format(SQL_WITHOUT_PERSONEL, string.Join(",", orderStates)), new
                {
                    StartTime = startTime,
                    EndTime = endTime,
                    WarehouseId = warehouseId,
                });
                return result != null ? result.AsList() : new List<NeedPurchasingGoods>();
            }
        }

        #region 公司毛利、商品毛利
        #region 公司毛利
        /// <summary> 
        /// 计算指定时间段(指定时间段：目前为每天)内的公司毛利【订单】
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        /// zal 2017-07-13
        public bool CompanyGrossProfitForEveryDay_GoodsOrder(DateTime startTime, DateTime endTime)
        {
            var parms = new[] {
                new SqlParameter("@OrderState",(int)OrderState.Consignmented),
                new SqlParameter("@ConsignTimeStart", startTime),
                new SqlParameter("@ConsignTimeEnd",endTime)
            };

            var result = SqlHelper.ExecuteScalarSP(GlobalConfig.ERP_DB_NAME, false, CommandType.StoredProcedure, "P_CompanyGrossProfitForEveryDay_GoodsOrder", 600, parms);
            return int.Parse(result.ToString()).Equals(1);
        }

        /// <summary>
        /// 计算指定时间段(指定时间段：目前为每天)内的公司毛利【门店采购单对应的ERP出库单】
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        /// zal 2017-07-13
        public bool CompanyGrossProfitForEveryDay_StorageRecord(DateTime startTime, DateTime endTime)
        {
            var parms = new[] {
                new SqlParameter("@StockState",(int)StorageRecordState.Finished),
                new SqlParameter("@ConsignTimeStart", startTime),
                new SqlParameter("@ConsignTimeEnd",endTime)
            };
            var result = SqlHelper.ExecuteScalarSP(GlobalConfig.ERP_DB_NAME, false, CommandType.StoredProcedure, "P_CompanyGrossProfitForEveryDay_StorageRecord", 600, parms);
            return int.Parse(result.ToString()).Equals(1);
        }

        /// <summary> 
        /// 获取生成的公司毛利
        /// </summary>
        /// <param name="dayTime"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        /// zal 2017-07-13
        public IList<CompanyGrossProfitRecordDetailInfo> GetCompanyGrossProfit(DateTime dayTime, int pageIndex, int pageSize)
        {
            string sql = @"
            SELECT [ID],[StockAndOrderId],[StockAndOrderNo],[SaleFilialeId],[SalePlatformId],[SalesAmount],[GoodsAmount],[ShipmentIncome],[PromotionsDeductible],[PointsDeduction],[ShipmentCost],[PurchaseCosts],[CatCommission],[OrderType],[OrderTime],[DayTime],[State] FROM (
			    SELECT ROW_NUMBER() OVER(ORDER BY ID) AS rowNum,[ID],[StockAndOrderId],[StockAndOrderNo],[SaleFilialeId],[SalePlatformId],[SalesAmount],[GoodsAmount],[ShipmentIncome],[PromotionsDeductible],[PointsDeduction],[ShipmentCost],[PurchaseCosts],[CatCommission],[OrderType],[OrderTime],[DayTime],[State] 
			    FROM dbo.Company_CompanyGrossProfitForEveryDay WITH(NOLOCK)
                WHERE DayTime='" + dayTime + @"' 
            )#temp
            WHERE #temp.rowNum BETWEEN " + ((pageIndex - 1) * pageSize + 1) + " AND " + pageIndex * pageSize;

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.Query<CompanyGrossProfitRecordDetailInfo>(sql).AsList();
            }
        }

        /// <summary> 
        /// 删除已经转移的公司毛利数据
        /// </summary>
        /// <returns></returns>
        /// zal 2017-07-13
        public bool DeleteCompanyGrossProfit()
        {
            string sql = "TRUNCATE TABLE dbo.Company_CompanyGrossProfitForEveryDay";

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                return conn.Execute(sql) > 0;
            }
        }
        #endregion

        #region 商品毛利
        /// <summary> 
        /// 计算指定时间段(指定时间段：目前为每天)内的商品毛利【订单】
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        /// zal 2017-07-13
        public bool GoodsGrossProfitForEveryDay_GoodsOrder(DateTime startTime, DateTime endTime)
        {
            var parms = new[] {
                new SqlParameter("@OrderState",(int)OrderState.Consignmented),
                new SqlParameter("@ConsignTimeStart", startTime),
                new SqlParameter("@ConsignTimeEnd",endTime)
            };
            var result = SqlHelper.ExecuteScalarSP(GlobalConfig.ERP_DB_NAME, false, CommandType.StoredProcedure, "P_GoodsGrossProfitForEveryDay_GoodsOrder", 600, parms);
            return int.Parse(result.ToString()).Equals(1);
        }

        /// <summary>
        /// 计算指定时间段(指定时间段：目前为每天)内的商品毛利【门店采购单对应的ERP出库单】
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        /// zal 2017-07-13
        public bool GoodsGrossProfitForEveryDay_StorageRecord(DateTime startTime, DateTime endTime)
        {
            var parms = new[] {
                new SqlParameter("@StockState",(int)StorageRecordState.Finished),
                new SqlParameter("@ConsignTimeStart", startTime),
                new SqlParameter("@ConsignTimeEnd",endTime)
            };
            var result = SqlHelper.ExecuteScalarSP(GlobalConfig.ERP_DB_NAME, false, CommandType.StoredProcedure, "P_GoodsGrossProfitForEveryDay_StorageRecord", 600, parms);
            return int.Parse(result.ToString()).Equals(1);
        }

        /// <summary> 
        /// 获取生成的商品毛利【订单】
        /// </summary>
        /// <param name="dayTime"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns>Type:单据类型(1:订单；2:出入库单据)</returns>
        /// zal 2017-07-13
        public IList<GoodsGrossProfitRecordDetailInfo> GetGoodsGrossProfit_GoodsOrder(DateTime dayTime, int pageIndex, int pageSize)
        {
            string sql = @"
            SELECT [ID],[GoodsId],[GoodsType],[Quantity],[SaleFilialeId],[SalePlatformId],[SalesPriceTotal],[PurchaseCostTotal],[OrderType],[OrderTime],[DayTime],[State] FROM (
			    SELECT ROW_NUMBER() OVER(ORDER BY ID) AS rowNum,[ID],[GoodsId],[GoodsType],[Quantity],[SaleFilialeId],[SalePlatformId],[SalesPriceTotal],[PurchaseCostTotal],[OrderType],[OrderTime],[DayTime],[State] 
			    FROM dbo.Goods_GoodsGrossProfitForEveryDay WITH(NOLOCK) 
                WHERE [Type]=1 AND DayTime='" + dayTime + @"'
            )#temp
            WHERE #temp.rowNum BETWEEN " + ((pageIndex - 1) * pageSize + 1) + " AND " + pageIndex * pageSize;

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.Query<GoodsGrossProfitRecordDetailInfo>(sql).AsList();
            }
        }

        /// <summary> 
        /// 获取生成的商品毛利【订单(满减)】
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        /// zal 2017-07-13
        public IList<GoodsGrossProfitRecordDetailInfo> GetGoodsGrossProfit_GoodsOrder_Promotion(int pageIndex, int pageSize)
        {
            string sql = @"
            SELECT StockAndOrderId,GoodsId,GoodsType,TotalQuantity,SaleFilialeId,SalePlatformId,SalesPriceTotal,PurchaseCostTotal,OrderType,OrderTime,DayTime,[State] FROM (
			    SELECT ROW_NUMBER() OVER(ORDER BY OrderType) AS rowNum,StockAndOrderId,GoodsId,GoodsType,TotalQuantity,SaleFilialeId,SalePlatformId,SalesPriceTotal,PurchaseCostTotal,OrderType,OrderTime,DayTime,[State] 
			    FROM dbo.Goods_StockAndOrderPurchaseCosts WITH(NOLOCK)
                WHERE PromotionValue>0 
            )#temp
            WHERE #temp.rowNum BETWEEN " + ((pageIndex - 1) * pageSize) + 1 + " AND " + pageIndex * pageSize;

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.Query<GoodsGrossProfitRecordDetailInfo>(sql).AsList();
            }
        }

        /// <summary>
        /// 获取生成的商品毛利【门店采购单对应的ERP出库单】
        /// </summary>
        /// <param name="dayTime"></param>
        /// <param name = "pageIndex"></param >
        /// <param name="pageSize"></param>
        /// <returns>Type:单据类型(1:订单；2:出入库单据)</returns>
        /// zal 2017-07-13
        public IList<GoodsGrossProfitRecordDetailInfo> GetGoodsGrossProfit_StorageRecord(DateTime dayTime, int pageIndex, int pageSize)
        {
            string sql = @"
            SELECT [ID],[GoodsId],[GoodsType],[Quantity],[SaleFilialeId],[SalePlatformId],[SalesPriceTotal],[PurchaseCostTotal],[OrderType],[OrderTime],[DayTime],[State] FROM (
			    SELECT ROW_NUMBER() OVER(ORDER BY ID) AS rowNum,[ID],[GoodsId],[GoodsType],[Quantity],[SaleFilialeId],[SalePlatformId],[SalesPriceTotal],[PurchaseCostTotal],[OrderType],[OrderTime],[DayTime],[State] 
			    FROM dbo.Goods_GoodsGrossProfitForEveryDay WITH(NOLOCK) 
                WHERE [Type]=2 AND DayTime='" + dayTime + @"'
            )#temp
            WHERE #temp.rowNum BETWEEN " + ((pageIndex - 1) * pageSize + 1) + " AND " + pageIndex * pageSize;

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.Query<GoodsGrossProfitRecordDetailInfo>(sql).AsList();
            }
        }

        /// <summary> 
        /// 删除已经转移的商品毛利数据
        /// </summary>
        /// <returns></returns>
        /// zal 2017-07-13
        public bool DeleteGoodsGrossProfit()
        {
            string sql = "TRUNCATE TABLE dbo.Goods_GoodsGrossProfitForEveryDay " +
                         "TRUNCATE TABLE dbo.Goods_StockAndOrderPurchaseCosts";

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                return conn.Execute(sql) > 0;
            }
        }
        #endregion

        /// <summary> 
        /// 获取执行失败的日期【公司毛利、商品毛利】
        /// </summary>
        /// <param name="executeTypeList">1:公司毛利(订单)；2:公司毛利(出入库单据)；3:商品毛利(订单)；4:商品毛利(出入库单据)</param>
        /// <returns></returns>
        /// zal 2017-07-13
        public IList<string> GetExecuteFailedDayTime(List<int> executeTypeList)
        {
            string sql = "SELECT DayTime,ExecuteType FROM dbo.GrossProfit_ExecuteFailedDayTime WITH(NOLOCK) WHERE ExecuteType IN(" + string.Join(",", executeTypeList) + ")";

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.Query(sql).Select(m => ((DateTime)m.DayTime).ToString() + "," + ((int)m.ExecuteType).ToString()).AsList();
            }
        }
        #endregion

        public List<SourceBindGoods> GetSourceBindGoodsesByOrderNo(string orderNo)
        {
            const string SQL =
                @"select GoodsId,GoodsCode,SUM(D.Quantity) AS Quantity,AVG(D.SellPrice) AS UnitPrice from lmShop_GoodsOrderDetail AS D 
INNER JOIN lmShop_GoodsOrder AS G ON D.OrderId=G.OrderId
WHERE G.OrderNo=@OrderNo
GROUP BY GoodsId,GoodsCode,G.OrderNo";
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                var result = conn.Query<SourceBindGoods>(SQL, new
                {
                    OrderNo = orderNo
                });
                return result != null ? result.AsList() : new List<SourceBindGoods>();
            }
        }
    }
}