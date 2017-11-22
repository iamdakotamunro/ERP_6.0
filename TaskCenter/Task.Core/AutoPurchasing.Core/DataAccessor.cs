using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using AutoPurchasing.Core.Model;
using ERP.Model;
using Keede.DAL.Helper;
using Keede.Ecsoft.Model;

namespace AutoPurchasing.Core
{
    public class DataAccessor
    {
        /// <summary>
        /// 获取常规报备的采购商品信息
        /// </summary>
        /// <returns></returns>
        public static IList<PurchasingGoods> GetPurchasingGoodsList(TaskType taskType)
        {
            string sql = QueryString.Where(QueryString.SELECT_PURCHASINGGOODS_ALL, @" ps.IsStockUp=1 AND ps.IsDelete=1 ");
            //if (taskType == TaskType.Routine)
            //{
            //    sql = QueryString.Where(QueryString.SELECT_PURCHASINGGOODS_ALL, @"ps.FilingDays = DATEPART(DD,GETDATE()) AND ps.IsStockUp=1 ");
            //}
            //else if (taskType == TaskType.Warning)
            //{
            //    sql = QueryString.Where(QueryString.SELECT_PURCHASINGGOODS_ALL, @"(DATEPART(DD,GETDATE())-ps.FilingDays NOT BETWEEN 0 AND 7) AND ( (CASE WHEN ps.FilingDays > DATEPART(DD,GETDATE()) THEN ps.FilingDays ELSE DATEDIFF(DAY,GETDATE(),DATEADD(MM,1,GETDATE()))-DATEPART(DD,GETDATE())+ps.FilingDays END ) > ps.ArrivalDays ) AND ps.IsStockUp=1 ");
            //}
            //else if (taskType == TaskType.All)
            //{
            //    sql = QueryString.Where(QueryString.SELECT_PURCHASINGGOODS_ALL, @" ps.IsStockUp=1 ");
            //}
            using (var db = DatabaseFactory.Create())
            {
                return db.Select<PurchasingGoods>(true, sql).ToList();
            }
        }

        /// <summary>
        /// 获取子商品的销售统计记录
        /// </summary>
        /// <param name="realGoodsIdList"></param>
        /// <param name="warehouseId"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public static IList<ChildGoodsSale> GetChildGoodsSaleList(List<Guid> realGoodsIdList, Guid warehouseId, DateTime startDate, DateTime endDate)
        {
            //获取子商品的销售信息
            var strbSql = new StringBuilder(@"
SELECT 
	RealGoodsId AS GoodsId,
	ABS(SUM(GoodsSales)) AS SaleQuantity,
	ISNULL(Specification,'-') AS Specification,HostingFilialeId,DayTime 
FROM [lmshop_GoodsDaySalesStatistics]
WHERE 
	DayTime >= @StartDate AND DayTime < @EndDate,
	AND RealGoodsId IN ($RealGoodsIds$) 
	AND DeliverWarehouseId=@WarehouseId 
GROUP BY RealGoodsId,DeliverWarehouseId,Specification,HostingFilialeId,DayTime");

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
                                new Parameter("@StartDate", startDate),
                                new Parameter("@EndDate", endDate)
                            };
            using (var db = DatabaseFactory.Create())
            {
                return db.Select<ChildGoodsSale>(true, strbSql.ToString(), parms).ToList();
            }
        }

        public static IList<GoodsDaySalesInfo> GetGoodsDaySalesInfos(List<Guid> realGoodsIdList, Guid warehouseId, DateTime startTime, DateTime endTime)
        {
            //获取子商品的销售信息
            var strbSql = new StringBuilder(@"
SELECT 
	RealGoodsId,DeliverWarehouseId AS WarehouseId,CONVERT(DATE,DayTime) as DayTime,SUM(GoodsSales) as GoodsSales,ISNULL(Specification,'-') AS Specification,HostingFilialeId 
FROM [lmshop_GoodsDaySalesStatistics]
WHERE 
	DayTime >= @StartTime AND DayTime < @EndTime
	AND RealGoodsId IN ($RealGoodsIds$) 
	AND DeliverWarehouseId=@WarehouseId 
GROUP BY DeliverWarehouseId,RealGoodsID,CONVERT(DATE,DayTime),Specification,HostingFilialeId 
ORDER BY SUM(GoodsSales) DESC");

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

        public static List<GoodsSaleDaysInfo> GetSaleDays(List<Guid> realGoodsIdList, Guid warehouseId, DateTime dateTime)
        {
            var sql = string.Format(@"SELECT 
	RealGoodsId,HostingFilialeId,DATEDIFF(DD,MIN(DayTime),@DateTime) AS [Days]
FROM [lmshop_GoodsDaySalesStatistics]
WHERE RealGoodsId IN ($RealGoodsIds$) 
	AND DeliverWarehouseId=@WarehouseId 
GROUP BY RealGoodsID,HostingFilialeId ");

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
                                new Parameter("@DateTime", dateTime),
                            }  ;
            using (var db = DatabaseFactory.Create())
            {
                var data = db.Select<GoodsSaleDaysInfo>(true, strbSql.ToString(), parms);
                return data != null && data.Any() ? data .ToList(): new List<GoodsSaleDaysInfo>();
            }
        }

        /// <summary>
        /// 插入采购记录
        /// </summary>
        /// <param name="purchasingId"></param>
        /// <param name="purchasingNo"></param>
        /// <param name="companyId"></param>
        /// <param name="companyName"></param>
        /// <param name="filialeId"></param>
        /// <param name="warehouseId"></param>
        /// <param name="purchasingFilialeId"></param>
        /// <param name="purchasingState"></param>
        /// <param name="purchasingType"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="description"></param>
        /// <param name="pmId"></param>
        /// <param name="purchasingToDate"></param>
        /// <param name="nextPurchasingDate"></param>
        /// <param name="isException"> </param>
        /// <param name="personResponsible"> </param>
        /// <returns></returns>
        public static bool InsertPurchasing(Guid purchasingId, string purchasingNo, Guid companyId, string companyName, Guid filialeId, Guid warehouseId,
            Guid purchasingFilialeId, int purchasingState, int purchasingType, DateTime startDate, DateTime endDate, string description, Guid pmId, 
            DateTime purchasingToDate, DateTime nextPurchasingDate, bool isException, Guid personResponsible)
        {
            const string SQL = QueryString.INSERT_PURCHASING;
            var parms = PrepareParameter.GetInsertPurchasingParms(purchasingId, purchasingNo, companyId, companyName, filialeId, warehouseId, purchasingFilialeId, purchasingState, purchasingType, startDate, endDate, description, pmId, purchasingToDate, nextPurchasingDate, isException, personResponsible);
            using (var db = DatabaseFactory.Create())
            {
                return db.Execute(false, SQL, parms);
            }
        }
        
        /// <summary>
        /// 插入采购商品的详细记录
        /// </summary>
        /// <param name="purchasingGoodsId"> </param>
        /// <param name="purchasingId"></param>
        /// <param name="goodsId"></param>
        /// <param name="goodsName"></param>
        /// <param name="unitName"></param>
        /// <param name="goodsCode"></param>
        /// <param name="specification"></param>
        /// <param name="companyId"></param>
        /// <param name="price"></param>
        /// <param name="planQuantity"></param>
        /// <param name="realityQuantity"></param>
        /// <param name="state"></param>
        /// <param name="description"></param>
        /// <param name="dayAvgStocking"></param>
        /// <param name="planStocking"></param>
        /// <param name="sixtyDays"></param>
        /// <param name="isException"> </param>
        /// <param name="sixtyDaySales"> </param>
        /// <param name="thirtyDaySales"> </param>
        /// <param name="elevenDaySales"> </param>
        /// <param name="purchasingGoodsType"> </param>
        /// <returns></returns>
        public static bool InsertPurchasingDetail(Guid purchasingGoodsId, Guid purchasingId, Guid goodsId, string goodsName, string unitName, string goodsCode, string specification, Guid companyId, decimal price, double planQuantity, double realityQuantity, int state, string description, double dayAvgStocking, double planStocking, int sixtyDays, bool isException, int sixtyDaySales, int thirtyDaySales, float elevenDaySales, int purchasingGoodsType)
        {
            const string SQL = QueryString.INSERT_PURCHASING_DETAILS;
            var parms = PrepareParameter.GetInsertPurchasingDetailParms(purchasingGoodsId, purchasingId, goodsId, goodsName, unitName,
                                                                        goodsCode, specification, companyId, price,
                                                                        planQuantity, realityQuantity, state,
                                                                        description, dayAvgStocking, planStocking,
                                                                        sixtyDays, isException, sixtyDaySales, thirtyDaySales, elevenDaySales, purchasingGoodsType);
            using (var db = DatabaseFactory.Create())
            {
                return db.Execute(false, SQL, parms);
            }
        }

        public static decimal GetPurchasingSumPrice(Guid purchasingId)
        {
            const string SQL = @"
SELECT SUM([Price]*[RealityQuantity]) FROM [lmShop_PurchasingDetail]
WHERE [PurchasingID]=@PurchasingID
";
            using (var db = DatabaseFactory.Create())
            {
                return db.GetValue<decimal>(true, SQL, new Parameter("PurchasingID", purchasingId));
            }
        }

        /// <summary>
        /// 统计采购的商品数量
        /// </summary>
        /// <param name="goodsId"></param>
        /// <param name="warehouseId"></param>
        /// <param name="hostingFilialeId"></param>
        /// <returns></returns>
        public static double GetSumPurchasingQuantity(Guid goodsId, Guid warehouseId,Guid hostingFilialeId)
        {
            const string SQL = QueryString.SUM_PURCHASING_QUANTITY;
            var param = PrepareParameter.Create();
            param.Append("@GoodsId", goodsId);
            param.Append("@WarehouseID", warehouseId);
            param.Append("@HostingFilialeId", hostingFilialeId);
            using (var db = DatabaseFactory.Create())
            {
                return db.GetValue<double>(true, SQL, param.Result());
            }
        }

        public static List<PlanPurchasingGoods> GetAllSumPurchasingQuantity(Guid warehouseId,Guid hostingFilialeId)
        {
            const string SQL = QueryString.SUM_PURCHASING_ALL_QUANTITY;
            var param = PrepareParameter.Create();
            param.Append("@WarehouseID", warehouseId);
            param.Append("@HostingFilialeId", hostingFilialeId);
            using (var db = DatabaseFactory.Create())
            {
                var result = db.Select<PlanPurchasingGoods>(true, SQL, param.Result());
                return result!=null?result.ToList():new List<PlanPurchasingGoods>();
            }
        }

        /// <summary>
        /// 获取在同一个责任人、供应商、仓库的未提交的自动报备采购信息ID
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="personResponsible"></param>
        /// <param name="warehouseId"></param>
        /// <param name="hostingFilialeId"></param>
        /// <returns></returns>
        public static Guid GetSamePurchasingId(Guid companyId, Guid personResponsible, Guid warehouseId,Guid hostingFilialeId)
        {
            const string SQL = QueryString.SELECT_SAME_PURCHASINGINFO;
            var param = PrepareParameter.Create();
            param.Append("@CompanyID", companyId);
            param.Append("@PersonResponsible", personResponsible);
            param.Append("@WarehouseID", warehouseId);
            param.Append("@HostingFilialeId", hostingFilialeId);
            using (var db = DatabaseFactory.Create())
            {
                return db.GetValue<Guid>(true, SQL, param.Result());
            }
        }

        /// <summary>
        /// 获取在同一个采购组、供应商、仓库的未提交的自动报备详细采购商品信息ID
        /// </summary>
        /// <param name="purchasingId"></param>
        /// <param name="goodsId"></param>
        /// <param name="specification"></param>
        /// <param name="hostingFilialeId"></param>
        /// <returns></returns>
        public static Guid GetSamePurchasingGoodsId(Guid purchasingId, Guid goodsId, string specification,Guid hostingFilialeId)
        {
            const string SQL = QueryString.SELECT_SAME_PURCHASINGDETAIL_SPECIFICATION;
            var param = PrepareParameter.Create();
            param.Append("@PurchasingID", purchasingId);
            param.Append("@GoodsId", goodsId);
            param.Append("@Specification", specification);
            param.Append("@FilialeId", hostingFilialeId);
            using (var db = DatabaseFactory.Create())
            {
                return db.GetValue<Guid>(true, SQL, param.Result());
            }
        }

        /// <summary>
        /// 更新同一规格下的采购数量
        /// </summary>
        /// <param name="purchasingGoodsId"></param>
        /// <param name="planQuantity"></param>
        /// <returns></returns>
        public static bool UpdatePurchasingGoodsPlanQuantity(Guid purchasingGoodsId, int planQuantity)
        {
            const string SQL = QueryString.UPDATE_PURCHASINGDETAIL_PLAN_QUANTITY;
            var param = PrepareParameter.Create();
            param.Append("@PurchasingGoodsID", purchasingGoodsId);
            param.Append("@PlanQuantity", planQuantity);
            using (var db = DatabaseFactory.Create())
            {
                return db.Execute(false, SQL, param.Result());
            }
        }

        /// <summary>
        /// 记录采购的最后日期
        /// </summary>
        /// <param name="goodsId"></param>
        /// <param name="lastDate"></param>
        /// <returns></returns>
        public static bool UpdateLastPurchasingDate(Guid warehouseId,Guid hostingFilialeId,Guid goodsId, DateTime lastDate)
        {
            const string SQL = QueryString.UPDATE_LASTPURCHASINGDATE;
            var param = PrepareParameter.Create();
            param.Append("@LastPurchasingDate", lastDate);
            param.Append("@GoodsId", goodsId);
            param.Append("@WarehouseId", warehouseId);
            param.Append("@HostingFilialeId", hostingFilialeId);
            using (var db = DatabaseFactory.Create())
            {
                return db.Execute(false, SQL, param.Result());
            }
        }

        /// <summary>
        /// 获取子商品的60、30、11天销量
        /// Add by liucaijun at 2012-10-12
        /// </summary>
        /// <param name="realGoodsId"></param>
        /// <param name="warehouseId"></param>
        /// <param name="endTime"></param>
        /// <param name="hostingFilialeId"></param>
        /// <returns></returns>
        public static ChildGoodsSalePurchasing GetChildGoodsSale(Guid realGoodsId, Guid warehouseId,Guid hostingFilialeId,DateTime endTime)
        {
            //获取子商品的销售信息
            const string SQL = @"
SELECT 
	RealGoodsId AS GoodsId,
	DeliverWarehouseId AS WarehouseId,
	ISNULL([60],0) AS SixtyDaySales,
    ISNULL([30],0) AS ThirtyDaySales,
    (ISNULL([11],0)/11) AS ElevenDaySales
FROM 
(
    SELECT RealGoodsId,DeliverWarehouseId,goodsSales,60 AS t FROM lmshop_GoodsDaySalesStatistics AS t1
    WHERE DayTime between dateadd(day,-60,convert(VARCHAR(10),@EndTime,120)) and dateadd(day,-30,convert(VARCHAR(10),@EndTime,120)) 
    and DeliverWarehouseId=@WareHouseId and RealGoodsId=@RealGoodsId and HostingFilialeId=@HostingFilialeId
    UNION all
    SELECT  RealGoodsId,DeliverWarehouseId,goodsSales, 30 as t from lmshop_GoodsDaySalesStatistics AS  t2
    WHERE  DayTime between dateadd(day,-30,convert(varchar(10),@EndTime,120)) and @EndTime 
    and  DeliverWarehouseId=@WareHouseId and RealGoodsId=@RealGoodsId and HostingFilialeId=@HostingFilialeId
    UNION  all
    SELECT  RealGoodsId,DeliverWarehouseId,goodsSales ,11 AS  t FROM  lmshop_GoodsDaySalesStatistics AS  t3
    WHERE  DayTime between dateadd(day,-11,convert(VARCHAR(10),@EndTime,120)) and @EndTime 
    and  DeliverWarehouseId=@WareHouseId and RealGoodsId=@RealGoodsId and HostingFilialeId=@HostingFilialeId
)kkk
PIVOT
(
sum (goodssales)
FOR t IN
( [11], [30], [60] )
) AS pvt
";
            var param = PrepareParameter.Create();
            param.Append("@RealGoodsId", realGoodsId);
            param.Append("@WareHouseId", warehouseId);
            param.Append("@EndTime", endTime);
            param.Append("@HostingFilialeId", hostingFilialeId);
            using (var db = DatabaseFactory.Create())
            {
                return db.Single<ChildGoodsSalePurchasing>(true, SQL, param.Result()) ?? new ChildGoodsSalePurchasing();
            }
        }

        /// <summary>
        /// 根据商品ID和是否现返获取采购促销明细
        /// </summary>
        /// <param name="goodsId"></param>
        /// <param name="promotionType"></param>
        /// <returns></returns>
        public static IList<PurchasePromotionInfo> GetPurchasePromotionList(Guid goodsId, int promotionType,Guid hostingFilialeId)
        {
            //IList<PurchasePromotionInfo> list = new List<PurchasePromotionInfo>();
            const string SQL_SELECT_PURCHASEPROMOTION = "SELECT [PromotionId],[GoodsId],[PromotionType],[PromotionKind],[BuyCount],[GivingCount],[PromotionInfo],[StartDate],[EndDate] FROM [lmshop_PurchasePromotion]";
            const string SQL = SQL_SELECT_PURCHASEPROMOTION + " WHERE [GoodsId]=@GoodsId AND [PromotionType]=@PromotionType AND HostingFilialeId=@HostingFilialeId";

            var parms = new[]
                            {
                                new Parameter("@GoodsId", goodsId),
                                new Parameter("@PromotionType", promotionType),
                                new Parameter("@HostingFilialeId", promotionType)
                            };

            using (var db = DatabaseFactory.Create())
            {
                return db.Select<PurchasePromotionInfo>(true, SQL, parms).ToList();
            }
        }

        #region[借记单]

        /// <summary>
        /// 添加借记单和明细
        /// </summary>
        /// <param name="info"></param>
        /// <param name="debitNoteDetailList">借记单明细</param>
        public static bool AddPurchaseSetAndDetail(DebitNoteInfo info, IList<DebitNoteDetailInfo> debitNoteDetailList)
        {
            const string SQL_INSERT_DEBITNOTE = @"
IF EXISTS(SELECT 0 FROM [lmshop_DebitNote] WHERE [PurchasingId]=@PurchasingId)
BEGIN
	UPDATE [lmshop_DebitNote] SET [PresentAmount]+=@PresentAmount WHERE [PurchasingId]=@PurchasingId
END
ELSE
BEGIN
    INSERT INTO [lmshop_DebitNote]([PurchasingId],[PurchasingNo],[CompanyId],[PresentAmount],[CreateDate],[FinishDate],[State],[WarehouseId],[Memo],[PersonResponsible]) VALUES(@PurchasingId,@PurchasingNo,@CompanyId,@PresentAmount,@CreateDate,@FinishDate,@State,@WarehouseId,@Memo,@PersonResponsible)
END
";
            const string SQL_INSERT_DEBITNOTEDETAIL = @"
IF EXISTS(SELECT 0 FROM [lmshop_DebitNoteDetail] WHERE [PurchasingId]=@PurchasingId AND [GoodsId]=@GoodsId)
BEGIN
	UPDATE [lmshop_DebitNoteDetail] SET [GivingCount]+=@GivingCount WHERE [PurchasingId]=@PurchasingId AND [GoodsId]=@GoodsId
END
ELSE
BEGIN
	INSERT INTO [lmshop_DebitNoteDetail]([PurchasingId],[GoodsId],[GoodsName],[Specification],[GivingCount],[ArrivalCount],[Price],[State],[Amount],[Memo],[ID]) VALUES(@PurchasingId,@GoodsId,@GoodsName,@Specification,@GivingCount,@ArrivalCount,@Price,@State,@Amount,@Memo,@ID)
END
";
            using (var db = DatabaseFactory.Create())
            {
                db.BeginTransaction();
                var parms = GetDebitNotetParameters(info);
                db.Execute(false, SQL_INSERT_DEBITNOTE, parms);
                foreach (var detailInfo in debitNoteDetailList)
                {
                    Parameter[] detailParms = GetDebitNoteDetailParameters(detailInfo);
                    db.Execute(false, SQL_INSERT_DEBITNOTEDETAIL, detailParms);
                }
                db.CompleteTransaction();
            }
            return true;
        }

        /// <summary>
        /// 根据采购单ID获取借记单
        /// </summary>
        /// <param name="purchasingId"></param>
        /// <returns></returns>
        public static DebitNoteInfo GetDebitNoteInfo(Guid purchasingId)
        {
            const string SQL_SELECT_DEBITNOTE = "SELECT [PurchasingId],[PurchasingNo],[CompanyId],[PresentAmount],[CreateDate],[FinishDate],[State],[WarehouseId],[Memo],[PersonResponsible],[NewPurchasingId] FROM [lmshop_DebitNote]";
            const string SQL = SQL_SELECT_DEBITNOTE + " WHERE [PurchasingId]=@PurchasingId";
            var parm = new Parameter(PARM_PURCHASINGID, purchasingId);

            //var info = new DebitNoteInfo();
            using (var db = DatabaseFactory.Create())
            {
                return db.Single<DebitNoteInfo>(true, SQL, parm);
            }
        }

        /// <summary>
        /// 获取借记单明细
        /// </summary>
        /// <param name="purchasingId">采购单ID</param>
        /// <returns></returns>
        public static IList<DebitNoteDetailInfo> GetDebitNoteDetailList(Guid purchasingId)
        {
            const string SQL_SELECT_DEBITNOTEDETAIL = "SELECT [PurchasingId],[GoodsId],[GoodsName],[Specification],[GivingCount],[ArrivalCount],[Price],[State],[Amount],[Memo],[ID] FROM [lmshop_DebitNoteDetail]";
            const string SQL = SQL_SELECT_DEBITNOTEDETAIL + " WHERE [PurchasingId]=@PurchasingId";
            var parm = new Parameter(PARM_PURCHASINGID, purchasingId);
            //IList<DebitNoteDetailInfo> list = new List<DebitNoteDetailInfo>();

            using (var db = DatabaseFactory.Create())
            {
                return db.Select<DebitNoteDetailInfo>(true, SQL, parm).ToList();
            }
        }

        /// <summary>
        /// 添加借记单明细
        /// </summary>
        /// <param name="detailInfo"></param>
        public static void AddDebitNoteDetail(DebitNoteDetailInfo detailInfo)
        {
            const string SQL_INSERT_DEBITNOTEDETAIL = @"
IF EXISTS(SELECT 0 FROM [lmshop_DebitNoteDetail] WHERE [PurchasingId]=@PurchasingId AND [GoodsId]=@GoodsId)
BEGIN
	UPDATE [lmshop_DebitNoteDetail] SET [GivingCount]+=@GivingCount WHERE [PurchasingId]=@PurchasingId AND [GoodsId]=@GoodsId
END
ELSE
BEGIN
	INSERT INTO [lmshop_DebitNoteDetail]([PurchasingId],[GoodsId],[GoodsName],[Specification],[GivingCount],[ArrivalCount],[Price],[State],[Amount],[Memo],[ID]) VALUES(@PurchasingId,@GoodsId,@GoodsName,@Specification,@GivingCount,@ArrivalCount,@Price,@State,@Amount,@Memo,@ID)
END
";
            var detailParms = GetDebitNoteDetailParameters(detailInfo);
            using (var db = DatabaseFactory.Create())
            {
                db.Execute(false, SQL_INSERT_DEBITNOTEDETAIL, detailParms);
            }
        }

        /// <summary>
        /// 根据采购单ID和商品ID更改借记单明细
        /// </summary>
        /// <param name="purchasingId"></param>
        /// <param name="goodsId"></param>
        /// <param name="givingCount"></param>
        public static void UpdateDebitNoteDetail(Guid purchasingId, Guid goodsId, int givingCount)
        {
            const string SQL = "UPDATE [lmshop_DebitNoteDetail] SET GivingCount+=@GivingCount,Amount=(Price*GivingCount+@GivingCount) WHERE [PurchasingId]=@PurchasingId AND [GoodsId]=@GoodsId;";
            var parms = new[]
                                     {
                                         new Parameter(PARM_PURCHASINGID,purchasingId),
                                         new Parameter(PARM_GOODSID,goodsId),
                                         new Parameter(PARM_GIVINGCOUNT,givingCount)
                                     };
            using (var db = DatabaseFactory.Create())
            {
                db.Execute(false, SQL, parms);
            }
        }

        #region [参数 DebitNote]

        private const string PARM_PURCHASINGID = "@PurchasingId";
        private const string PARM_PURCHASINGNO = "@PurchasingNo";
        private const string PARM_COMPANYID = "@CompanyId";
        private const string PARM_PRESENTAMOUNT = "@PresentAmount";
        private const string PARM_CREATEDATE = "@CreateDate";
        private const string PARM_FINISHDATE = "@FinishDate";
        private const string PARM_STATE = "@State";
        private const string PARM_WAREHOUSEID = "@WarehouseId";
        private const string PARM_MEMO = "@Memo";
        private const string PARM_PERSONRESPONSIBLE = "@PersonResponsible";

        #endregion [参数 DebitNote]

        private static Parameter[] GetDebitNotetParameters(DebitNoteInfo info)
        {
            return new[]
                            {
                                new Parameter(PARM_PURCHASINGID, info.PurchasingId),
                                new Parameter(PARM_PURCHASINGNO, info.PurchasingNo),
                                new Parameter(PARM_COMPANYID, info.CompanyId),
                                new Parameter(PARM_PRESENTAMOUNT, info.PresentAmount),
                                new Parameter(PARM_CREATEDATE, info.CreateDate),
                                new Parameter(PARM_FINISHDATE, info.FinishDate == DateTime.MinValue ? SqlDateTime.MinValue : info.FinishDate),
                                new Parameter(PARM_STATE,info.State),
                                new Parameter(PARM_WAREHOUSEID,  info.WarehouseId),
                                new Parameter(PARM_MEMO, info.Memo),
                                new Parameter(PARM_PERSONRESPONSIBLE, info.PersonResponsible)
                            };
        }

        #region [参数 DebitNoteDetail]

        private const string PARM_GOODSID = "@GoodsId";
        private const string PARM_GOODSNAME = "@GoodsName";
        private const string PARM_SPECIFICATION = "@Specification";
        private const string PARM_GIVINGCOUNT = "@GivingCount";
        private const string PARM_ARRIVALCOUNT = "@ArrivalCount";
        private const string PARM_PRICE = "@Price";
        private const string PARM_AMOUNT = "@Amount";
        private const string PARM_ID = "@ID";

        #endregion [参数 DebitNoteDetail]

        private static Parameter[] GetDebitNoteDetailParameters(DebitNoteDetailInfo info)
        {
            return new[]
                            {
                                new Parameter(PARM_PURCHASINGID, info.PurchasingId),
                                new Parameter(PARM_GOODSID, info.GoodsId),
                                new Parameter(PARM_GOODSNAME,info.GoodsName),
                                new Parameter(PARM_SPECIFICATION,info.Specification),
                                new Parameter(PARM_GIVINGCOUNT,info.GivingCount),
                                new Parameter(PARM_ARRIVALCOUNT,info.ArrivalCount),
                                new Parameter(PARM_PRICE,info.Price),
                                new Parameter(PARM_STATE,info.State),
                                new Parameter(PARM_AMOUNT,info.Amount),
                                new Parameter(PARM_MEMO, info.Memo),
                                new Parameter(PARM_ID, info.Id)
                            };
        }

        #endregion

        #region -- 单据号

        /// <summary>
        /// 获取指定类型的订单号
        /// </summary>
        /// <param name="codeType"></param>
        /// <returns></returns>
        public static string GetCode(BaseInfo.CodeType codeType)
        {
            DateTime dateTime = DateTime.Now;
            string tradeCode = codeType.ToString() + (dateTime.Year - (0)).ToString().Substring(2, 2) + dateTime.Month.ToString("D2") + dateTime.Day.ToString("D2") + dateTime.Hour.ToString("D2") + GetCodeValue(codeType).ToString("D3");
            return tradeCode;
        }

        /// <summary>
        /// 获取指定类型的当前编号
        /// </summary>
        /// <param name="codeType">单据类型</param>
        /// <returns></returns>
        public static int GetCodeValue(BaseInfo.CodeType codeType)
        {
            var parm = new Parameter(QueryString.PARM_CODETYPE, codeType);
            using (var db = DatabaseFactory.Create())
            {
                return db.GetValue<int>(true, QueryString.SELECT_GET_CODEVALUE, parm);
            }
        }

        #endregion

        #region -- 获取最低库存备货数量

        public static int GetGoodsStockMinQuantity(Guid goodsId, Guid warehouseId)
        {
            const string SQL = @"
SELECT TOP 1 [MinStockQuantity]
  FROM [StockSetting]
  WHERE [RealGoodsId]=@RealGoodsId AND [WarehouseId]=@WarehouseId
";
            using (var db = DatabaseFactory.Create())
            {
                return db.GetValue<int>(true, SQL, new[]
                    {
                        new Parameter("RealGoodsId",goodsId),
                        new Parameter("WarehouseId",warehouseId)
                    });
            }
        }

        #endregion
    }
}