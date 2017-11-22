using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using ERP.DAL.Interface.IShop;
using Keede.Ecsoft.Model.ShopFront;
using ERP.Enum;
using ERP.Enum.ShopFront;
using Keede.DAL.RWSplitting;
using ERP.Environment;
using Dapper;
using System.Transactions;

namespace ERP.DAL.Implement.Shop
{
    public class ApplyStockDAL : IApplyStockDAL
    {
        public ApplyStockDAL(Environment.GlobalConfig.DB.FromType fromType) { }

        #region IApplyStockDAL 成员

        /// <summary>
        /// 新增一条门店采购申请记录
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public bool Insert(ApplyStockInfo info)
        {
            const string SQL = @"
IF EXISTS(SELECT 0 FROM [Shopfront_ApplyStock] WHERE ApplyId=@ApplyId)
BEGIN
	UPDATE [Shopfront_ApplyStock] SET SubtotalQuantity=@SubtotalQuantity WHERE ApplyId=@ApplyId
END
ELSE
BEGIN
	INSERT INTO [Shopfront_ApplyStock] 
	(ApplyId,ParentApplyId,TradeCode,FilialeId,FilialeName,WarehouseId,CompanyId,CompanyName,CompanyWarehouseId,DateCreated,Transactor,Description,SubtotalQuantity,StockState,PurchaseType,Direction) 
	VALUES
	(@ApplyId,@ParentApplyId,@TradeCode,@FilialeId,@FilialeName,@WarehouseId,@CompanyId,@CompanyName,@CompanyWarehouseId,@DateCreated,@Transactor,@Description,@SubtotalQuantity,@StockState,@PurchaseType,@Direction)
END
";
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                return conn.Execute(SQL, new
                {
                    ApplyId = info.ApplyId,
                    ParentApplyId = info.ParentApplyId,
                    TradeCode = info.TradeCode,
                    FilialeId = info.FilialeId,
                    FilialeName = info.FilialeName,
                    CompanyId = info.CompanyId,
                    CompanyName = info.CompanyName,
                    WarehouseId = info.WarehouseId,
                    CompanyWarehouseId = info.CompanyWarehouseId,
                    DateCreated = info.DateCreated,
                    Transactor = info.Transactor,
                    Description = info.Description,
                    SubtotalQuantity = info.SubtotalQuantity,
                    StockState = info.StockState,
                    PurchaseType = info.PurchaseType,
                    Direction = info.Direction,
                }) > 0;
            }
        }

        /// <summary>
        /// 插入明细申请单
        /// </summary>
        /// <param name="infoList"></param>
        /// <returns></returns>
        public int InsertDetail(IList<ApplyStockDetailInfo> infoList)
        {
            const string SQL = @"INSERT INTO [Shopfront_ApplyStockDetail] 
                                    (ApplyId,GoodsId,GoodsName,Specification,Quantity,Price,Description,IsPrint,ProcessNo,ShopFilialeId,CompGoodsId,Units,HasError,ErrorRemark)
                                    VALUES 
                                    (@ApplyId,@GoodsId,@GoodsName,@Specification,@Quantity,@Price,@Description,@IsPrint,@ProcessNo,@ShopFilialeId,@CompGoodsId,@Units,@HasError,@ErrorRemark)";

            return infoList.Select(info => new
            {
                ApplyId = info.ApplyId,
                GoodsId = info.GoodsId,
                GoodsName = info.GoodsName,
                Specification = string.IsNullOrEmpty(info.Specification) ? string.Empty : info.Specification,
                Quantity = info.Quantity,
                Description = string.IsNullOrEmpty(info.Description) ? string.Empty : info.Description,
                Price = info.Price,
                ProcessNo = info.ProcessNo ?? string.Empty,
                IsPrint = info.IsPrint,
                ShopFilialeId = info.ShopFilialeId,
                CompGoodsId = info.CompGoodsID == Guid.Empty ? info.GoodsId : info.CompGoodsID,
                Units = info.Units,
                HasError = info.IsComfirmed ? 1 : 0,
                ErrorRemark = info.ComfirmTips ?? string.Empty,
            }).Count(param =>
           {
               using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
               {
                   return conn.Execute(SQL, param) > 0;
               }
           });
        }

        /// <summary>
        /// 查询是否存在此采购单
        /// </summary>
        /// <param name="applyStockId"></param>
        /// <returns></returns>
        public ApplyStockInfo FindById(Guid applyStockId)
        {
            const string SQL =
@"SELECT TOP 1 
	ApplyId,
	ParentApplyId,
	SAS.TradeCode,
	ss.StockId AS SemiStockId,
	ss.TradeCode AS SemiStockCode,
	SAS.FilialeId,
	SAS.FilialeName,
	SAS.WarehouseId,
	'门店仓库' as WarehouseName,
	SAS.CompanyId,
	SAS.CompanyName,
	SAS.CompanyWarehouseId,
	SAS.DateCreated,
	SAS.Transactor,
	SAS.Description,
	SAS.SubtotalQuantity,
	SAS.StockState,
	SAS.PurchaseType,
    SAS.Direction 
FROM [Shopfront_ApplyStock] SAS
LEFT JOIN StorageRecord as ss ON ss.LinkTradeCode = SAS.TradeCode 
WHERE ApplyId=@ApplyId";

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.QueryFirstOrDefault<ApplyStockInfo>(SQL, new { ApplyId = applyStockId });
            }
        }

        /// <summary>
        /// 获取采购明细信息
        /// </summary>
        /// <param name="applyId"></param>
        /// <returns></returns>
        public IList<ApplyStockDetailInfo> FindDetailList(Guid applyId)
        {
            IList<ApplyStockDetailInfo> list = new List<ApplyStockDetailInfo>();
            const string SQL =
@"SELECT a.WarehouseId,
ad.ApplyId,
ad.GoodsId,
ad.GoodsName,
ad.Specification,
ad.Quantity,
ad.Price,
ad.Description,
ad.ProcessNo,
ad.ShopFilialeId,
a.CompanyId,
a.CompanyWarehouseId,
ad.CompGoodsId,
ad.Units,ad.HasError,ad.ErrorRemark 
FROM Shopfront_ApplyStockDetail AS ad 
RIGHT JOIN Shopfront_ApplyStock AS a 
ON ad.ApplyId=a.ApplyId
WHERE ad.ApplyId = @ApplyId  ";

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.Query<ApplyStockDetailInfo>(SQL, new { ApplyId = applyId }).AsList();
            }
        }

        /// <summary>
        /// 更新申请单状态
        /// </summary>
        /// <param name="applyId"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public bool UpdateApplyStockState(Guid applyId, int state)
        {
            const string SQL = @"UPDATE [Shopfront_ApplyStock] SET [StockState]=@State WHERE ApplyId=@ApplyId";

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                return conn.Execute(SQL, new
                {
                    State = state,
                    ApplyId = applyId
                }) > 0;
            }
        }

        #region  针对新需求，采购确认给出提示信息添加  add by liangcanren at 2015-04-15
        /// <summary>
        /// 更新采购明细需确认时的商品标识与提示
        /// </summary>
        /// <param name="applyId"></param>
        /// <param name="realGoodsId"></param>
        /// <param name="content"></param>
        /// <param name="isError"></param>
        /// <returns></returns>
        public bool UpdateDetailTips(Guid applyId, Guid realGoodsId, string content, bool isError)
        {
            const string SQL = @"UPDATE [Shopfront_ApplyStockDetail] SET [HasError]=@HasError,ErrorRemark=@ErrorRemark WHERE ApplyId=@ApplyId AND GoodsId=@RealGoodsId";

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                return conn.Execute(SQL, new
                {
                    HasError = isError,
                    ApplyId = applyId,
                    ErrorRemark = content,
                    RealGoodsId = realGoodsId,
                }) > 0;
            }
        }
        #endregion


        /// <summary>
        /// 获取申请采购列表
        /// 注意：这里的trackCode可以搜索申请单号和出据单号
        /// zhangfan added at 2012-July-19th
        /// </summary>
        /// <returns></returns>
        public IList<ApplyStockInfo> GetList(Guid filialeId, int applyStockState, int purchaseType, string tradeCode)
        {
            IList<ApplyStockInfo> list = new List<ApplyStockInfo>();
            var sql = new StringBuilder(
               @"SELECT ApplyId,
	ParentApplyId,
	SAS.TradeCode,
	SAS.FilialeId,
	SAS.FilialeName,
	SAS.WarehouseId,
    '门店仓库' as WarehouseName,
	SAS.CompanyId,
	SAS.CompanyName,
	SAS.CompanyWarehouseId,
	SAS.DateCreated,
	SAS.Transactor,
	SAS.Description,
	SAS.SubtotalQuantity,
	SAS.StockState,
	SAS.PurchaseType,
    SAS.Direction    
FROM [Shopfront_ApplyStock] SAS
WHERE SAS.FilialeId = '" + filialeId + "'");

            if (applyStockState != int.MinValue)
                sql.Append(" AND StockState = ").Append(applyStockState);
            if (purchaseType > 0)
                sql.Append(" AND PurchaseType = ").Append(purchaseType);

            sql.Append(" ORDER BY SAS.DateCreated DESC");

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                list = conn.Query<ApplyStockInfo>(sql.ToString()).AsList();
            }
            foreach (var ent in list)
                ent.SemiStockCode = GetSemiStockTradeCode(ent.TradeCode);
            return tradeCode == string.Empty ? list : list.Where(ent => ent.SemiStockCode.Contains(tradeCode) || ent.TradeCode == tradeCode).ToList();
        }

        /// <summary>
        /// 专门给需求查询中的门店采购申请
        /// </summary>
        /// <param name="goodsId"></param>
        /// <param name="warehouseId"></param>
        /// <returns></returns>
        public IList<ApplyStockInfo> GetList(Guid goodsId, Guid warehouseId)
        {
            string sql = @"
SELECT 
	sas.ApplyId,
	sas.ParentApplyId,
	sas.TradeCode,
	sas.FilialeId,
	sas.FilialeName,
	sas.WarehouseId,
	'门店仓库' as WarehouseName,
	sas.CompanyId,
	sas.CompanyName,
	sas.CompanyWarehouseId,
	sas.DateCreated,
	sas.Transactor,
	sas.Description,
	asd.Quantity-isnull((select SUM(abs(sgs.Quantity)) from StorageRecord AS ss LEFT JOIN StorageRecordDetail AS sgs ON sgs.StockId=ss.StockId where ss.StockState=" + (int)StorageRecordState.Finished + @" and ss.LinkTradeCode=sas.TradeCode and sgs.RealGoodsId=asd.GoodsId),0)as SubtotalQuantity,
	sas.StockState,
	sas.PurchaseType,sas.Direction  
FROM [Shopfront_ApplyStock] sas 
LEFT JOIN Shopfront_ApplyStockDetail AS asd ON sas.ApplyId=asd.ApplyId
WHERE sas.StockState<" + (int)ApplyStockState.Finished + @" AND sas.StockState>" + (int)ApplyStockState.Cancel + @" AND asd.GoodsId=@GoodsId
AND sas.CompanyWarehouseId=@WarehouseId
ORDER BY sas.DateCreated DESC";

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.Query<ApplyStockInfo>(sql,
                    new
                    {
                        GoodsId = goodsId,
                        WarehouseId = warehouseId,
                    }).AsList();
            }
        }

        /// <summary>
        /// 统计当前采购单下的明细商品数量
        /// </summary>
        /// <param name="applyStockId"></param>
        /// <returns></returns>
        public int CountDetailGoodsById(Guid applyStockId)
        {
            const string SQL = @"SELECT COUNT(0) FROM [Shopfront_ApplyStockDetail] WHERE ApplyId=@ApplyId";

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.ExecuteScalar<int>(SQL,
                    new
                    {
                        ApplyId = applyStockId,
                    });
            }
        }

        /// <summary>
        /// 获取出库单号
        /// </summary>
        /// <param name="stockNo"></param>
        /// <returns></returns>
        private string GetSemiStockTradeCode(string stockNo)
        {
            const string SQL = @"
DECLARE @S VARCHAR(8000)
SELECT @S=ISNULL(@S+',','')+a.TradeCode FROM (
	SELECT 
		CASE StockState 
		WHEN @StockState THEN 
			TradeCode+'(作废)' 
		ELSE 
			TradeCode 
		END as TradeCode
	FROM StorageRecord WITH(NOLOCK) WHERE LinkTradeCode=@LinkTradeCode
) a
SELECT @S as TradeCode
";
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.ExecuteScalar<string>(SQL,
                    new
                    {
                        LinkTradeCode = stockNo,
                        StockState = (int)StorageRecordState.Canceled,
                    }) ?? "";
            }
        }

        /// <summary>
        /// 更新采购申请单异常信息
        /// </summary>
        /// <param name="outstockno"></param>
        /// <param name="errorMessage"></param>
        public bool UpdateAddApplyStockExecption(string outstockno, Dictionary<Guid, String> errorMessage)
        {
            const string SQL1 = @"UPDATE StorageRecord SET IsError=@IsError WHERE TradeCode=@TradeCode;";
            const string SQL2 = @"UPDATE StorageRecordDetail SET IsError=@IsError,ExceptionMsg=@ExceptionMsg WHERE StockId=(SELECT TOP 1 StockId FROM StorageRecord WHERE TradeCode=@TradeCode) AND RealGoodsId=@RealGoodsId";

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        //--1、先更新申请单的状态
                        conn.Execute(SQL1,
                            new
                            {
                                IsError = true,
                                TradeCode = outstockno,
                            }, trans);

                        //--2、再更新采购商品明细单的错误信息
                        foreach (var keyValueInfo in errorMessage)
                        {
                            var sqlParams = new[]
                                                {
                                            new SqlParameter("@IsError", 1),
                                            new SqlParameter("@ExceptionMsg", keyValueInfo.Value + "\r\n"),
                                            new SqlParameter("@TradeCode", outstockno),
                                            new SqlParameter("@RealGoodsId", keyValueInfo.Key)
                                        };
                            conn.Execute(SQL2, new
                            {
                                IsError = true,
                                ExceptionMsg = keyValueInfo.Value + "\r\n",
                                TradeCode = outstockno,
                                RealGoodsId = keyValueInfo.Key
                            }, trans);
                        }
                        trans.Commit();
                    }
                    catch (Exception)
                    {
                        trans.Rollback();
                        return false;
                    }
                }
            }
            return true;
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tradeCode"></param>
        /// <returns></returns>
        public ApplyStockInfo GetApplyInfoByTradeCode(string tradeCode)
        {
            const string SQL = @"SELECT [ApplyId]
      ,[TradeCode]
      ,[FilialeId]
      ,[WarehouseId]
      ,[CompanyId]
      ,[CompanyWarehouseId]
      ,[DateCreated]
      ,[SubtotalQuantity]
      ,[StockState]
      ,[PurchaseType],[Direction] FROM [Shopfront_ApplyStock] WHERE [TradeCode] = @TradeCode";

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.QueryFirstOrDefault<ApplyStockInfo>(SQL, new { TradeCode = tradeCode });
            }
        }

        /// <summary>
        /// 通过门店采购申请编号获取明细 ADD BY LiangCanren at 2015-05-08
        /// </summary>
        /// <param name="tradeCode"></param>
        /// <returns></returns>
        public IList<ApplyStockDetailInfo> GetApplyStockDetailInfosByTradeCode(string tradeCode)
        {
            IList<ApplyStockDetailInfo> list = new List<ApplyStockDetailInfo>();
            const string SQL =
@"SELECT a.WarehouseId,
ad.ApplyId,
ad.GoodsId,
ad.GoodsName,
ad.Specification,
ad.Quantity,
ad.Price,
ad.Description,
ad.ProcessNo,
ad.ShopFilialeId,
a.CompanyId,
a.CompanyWarehouseId,
ad.CompGoodsId,
ad.Units,ad.HasError,ad.ErrorRemark 
FROM Shopfront_ApplyStockDetail AS ad 
RIGHT JOIN Shopfront_ApplyStock AS a 
ON ad.ApplyId=a.ApplyId
WHERE a.TradeCode = @TradeCode  ";

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.Query<ApplyStockDetailInfo>(SQL, new { TradeCode = tradeCode }).AsList();
            }
        }

        /// <summary>
        /// 根据单号更新申请单状态
        /// </summary>
        /// <param name="tradeCode"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public bool UpdateApplyStockStateByTradeCode(string tradeCode, int state)
        {
            const string SQL = @"UPDATE [Shopfront_ApplyStock] SET [StockState]=@State WHERE TradeCode=@TradeCode";

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                return conn.Execute(SQL, new
                {
                    State = state,
                    TradeCode = tradeCode
                }) > 0;
            }
        }

        /// <summary>
        /// 获取店铺某段时间内的采购数/可退换货数
        /// </summary>
        /// <param name="shopId">店铺id</param>
        /// <param name="goodsId">主商品Id</param>
        /// <param name="startTime">起始时间</param>
        /// <param name="endTime">截止时间</param>
        /// <param name="states"> </param>
        /// <returns></returns>
        public int GetApplyStockGoodsCount(Guid shopId, Guid goodsId, DateTime startTime, DateTime endTime, IList<int> states)
        {
            var builder = new StringBuilder(@"SELECT SUM(Quantity) AS Quantity FROM Shopfront_ApplyStockDetail AD INNER JOIN Shopfront_ApplyStock A
ON AD.ApplyId=A.ApplyId WHERE A.FilialeId=@ShopID AND A.DateCreated BETWEEN @StartTime AND @EndTime AND AD.CompGoodsId=@GoodsID");
            var sqlParams = new[]
                                {
                                    new SqlParameter("@ShopID",shopId),
                                    new SqlParameter("@StartTime",startTime),
                                    new SqlParameter("@EndTime",endTime),
                                    new SqlParameter("@GoodsID",goodsId)
                                };
            if (states != null && states.Count > 0)
            {
                var stateStr = new StringBuilder("");
                for (int i = 0; i < states.Count; i++)
                {
                    stateStr.Append(states[i]);
                    if (i != states.Count - 1)
                        stateStr.Append(",");
                }
                builder.AppendFormat(" AND A.StockState IN({0})", stateStr);
            }

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.ExecuteScalar<int>(builder.ToString(), new
                {
                    ShopID = shopId,
                    StartTime = startTime,
                    EndTime = endTime,
                    GoodsID = goodsId,
                });
            }
        }


        /// <summary>
        /// 获取门店采购申请列表
        /// </summary>
        /// <param name="shopId"></param>
        /// <param name="purchaseType"></param>
        /// <param name="states"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="searchKey"></param>
        /// <param name="goodsIds"> </param>
        /// <returns></returns>
        public IList<ApplyStockInfo> GetApplyStockList(Guid shopId, int purchaseType, IList<int> states,
            DateTime startTime, DateTime endTime, string searchKey, IList<Guid> goodsIds)
        {
            IList<ApplyStockInfo> list = new List<ApplyStockInfo>();
            var sqlBuilder = new StringBuilder(@"SELECT SS.ApplyId,
	SS.ParentApplyId,SS.TradeCode,SS.FilialeId,SS.FilialeName,SS.WarehouseId,'门店仓库' as WarehouseName,
	SS.CompanyId,SS.CompanyName,SS.CompanyWarehouseId,SS.DateCreated,SS.Transactor,SS.Description,SS.SubtotalQuantity,
	SS.StockState,SS.PurchaseType,SS.Direction FROM [Shopfront_ApplyStock] SS ");

            sqlBuilder.AppendFormat(" WHERE SS.DateCreated BETWEEN '{0}' AND '{1}'", startTime, endTime);
            if (shopId != Guid.Empty)
                sqlBuilder.AppendFormat(" AND SS.FilialeId='{0}'", shopId);
            if (purchaseType != 0)
            {
                sqlBuilder.AppendFormat(" AND SS.PurchaseType=").Append(purchaseType);
            }
            if (states != null && states.Count > 0)
            {
                var stateStr = new StringBuilder("");
                for (int i = 0; i < states.Count; i++)
                {
                    stateStr.Append(states[i]);
                    if (i != states.Count - 1)
                        stateStr.Append(",");
                }
                sqlBuilder.AppendFormat(" AND SS.StockState IN({0})", stateStr);
            }
            if (goodsIds != null && goodsIds.Count > 0)
            {
                sqlBuilder.Append(" AND SS.ApplyId IN(");
                sqlBuilder.Append(@"SELECT ASD.ApplyId FROM Shopfront_ApplyStockDetail ASD ");
                sqlBuilder.Append(" WHERE ASD.GoodsId IN(");
                for (int i = 0; i < goodsIds.Count; i++)
                {
                    sqlBuilder.Append("'").Append(goodsIds[i]).Append("'");
                    if (i != goodsIds.Count - 1)
                        sqlBuilder.Append(",");
                }
                sqlBuilder.Append(")");
                sqlBuilder.Append(" GROUP BY ASD.ApplyId) ");
            }
            sqlBuilder.Append(" ORDER BY SS.DateCreated DESC; ");

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                list = conn.Query<ApplyStockInfo>(sqlBuilder.ToString()).AsList();
            }
            foreach (var ent in list)
                ent.SemiStockCode = GetSemiStockTradeCode(ent.TradeCode);
            return string.IsNullOrEmpty(searchKey) ? list : list.Where(ent => ent.SemiStockCode.Contains(searchKey) || ent.TradeCode == searchKey).ToList();
        }

        /// <summary>
        /// 获取特定时间段内商品采购数列表
        /// </summary>
        /// <param name="shopId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="states"></param>
        /// <returns></returns>
        public Dictionary<Guid, Dictionary<Guid, int>> GetPurchaseGoodsQuantity(Guid shopId, DateTime startTime, DateTime endTime, IList<int> states)
        {
            var builder = new StringBuilder(@"SELECT ASD.CompGoodsId,ASD.GoodsId,SUM(ASD.Quantity) AS Quantity  
FROM Shopfront_ApplyStockDetail ASD INNER JOIN Shopfront_ApplyStock SAS 
ON ASD.ApplyId=SAS.ApplyId WHERE SAS.FilialeId='{0}' ");
            if (startTime != DateTime.MinValue || endTime != DateTime.MinValue)
            {
                if (startTime == DateTime.MinValue)
                    startTime = new DateTime(2014, 1, 1);
                if (endTime == DateTime.MinValue)
                    endTime = startTime.AddMonths(6);
                builder.AppendFormat(" AND SAS.DateCreated BETWEEN '{0}' AND '{1}'", startTime, endTime);
            }
            if (states != null && states.Count > 0)
            {
                var stateStr = new StringBuilder("");
                for (int i = 0; i < states.Count; i++)
                {
                    stateStr.Append(states[i]);
                    if (i != states.Count - 1)
                        stateStr.Append(",");
                }
                builder.AppendFormat(" AND SAS.StockState IN({0})", stateStr);
            }
            builder.Append(" GROUP BY ASD.CompGoodsId,ASD.GoodsId ");

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.Query(string.Format(builder.ToString(), shopId))
                    .GroupBy(g => (Guid)g.CompGoodsId)
                    .ToDictionary(kv => kv.Key, kv => kv.GroupBy(g2 => (Guid)g2.GoodsId).ToDictionary(kv2 => kv2.Key, kv2 => kv2.Sum(s => (int)s.Quantity)));
            }
        }
    }
}
