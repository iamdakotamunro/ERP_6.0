using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using ERP.DAL.Interface.IShop;
using ERP.Environment;
using ERP.Model.ShopFront;
using ERP.Enum;
using Dapper;
using Keede.DAL.RWSplitting;
using System.Linq;
using System.Transactions;

namespace ERP.DAL.Implement.Shop
{
    public class ShopExchangedApplyDetailDal : IShopExchangedApplyDetail
    {
        public ShopExchangedApplyDetailDal(GlobalConfig.DB.FromType fromType) { }

        #region 相关sql

        /// <summary>
        /// 添加退货明细
        /// </summary>
        private const string SQL_INSERT_SHOPREFUNDAPPLYDETAIL = @"INSERT INTO ShopExchangedApplyDetail
           (ID,ApplyID,GoodsID,RealGoodsID,GoodsName,GoodsCode,Specification,Price,Quantity,Units,BatchNo,EffectiveDate)
     VALUES(@ID,@ApplyID,@GoodsID,@RealGoodsID,@GoodsName,@GoodsCode,@Specification,@Price,@Quantity,@Units,@BatchNo,@EffectiveDate)";

        /// <summary>
        /// 添加换货明细
        /// </summary>
        private const string SQL_INSERT_SHOPEXCHANGEDAPPLYDETAIL = @"INSERT INTO ShopExchangedApplyDetail
           (ID,ApplyID,GoodsID,RealGoodsID,GoodsName,GoodsCode,Specification,Price,Quantity,Units,BarterGoodsID,
BarterRealGoodsID,BarterGoodsName,BarterGoodsCode,BarterSpecification,BatchNo,EffectiveDate)
     VALUES(@ID,@ApplyID,@GoodsID,@RealGoodsID,@GoodsName,@GoodsCode,@Specification,@Price,@Quantity,@Units,
@BarterGoodsID,@BarterRealGoodsID,@BarterGoodsName,@BarterGoodsCode,@BarterSpecification,@BatchNo,@EffectiveDate)";

        /// <summary>
        /// 根据明细ID删除
        /// </summary>
        private const string SQL_DELETE_APPLYDETAILBYID = @"DELETE ShopExchangedApplyDetail WHERE ID=@ID";

        /// <summary>
        /// 根据退换货申请ID删除
        /// </summary>
        private const string SQL_DELETE_APPLYDETAILBYAPPLYID = @"DELETE ShopExchangedApplyDetail WHERE ApplyID=@ApplyID";

        /// <summary>
        /// 查询换货
        /// </summary>
        private const string SQL_SELECT_SHOPEXCHANGEDAPPLYDETAIL =
            @"SELECT ID,ApplyID,GoodsID,RealGoodsID,GoodsName,GoodsCode, 
Specification,Price,Quantity,Units,BarterGoodsID,BarterRealGoodsID,BarterGoodsName,BarterGoodsCode,BarterSpecification,BatchNo,EffectiveDate FROM ShopExchangedApplyDetail";

        /// <summary>
        /// 查询退货
        /// </summary>
        private const string SQL_SELECT_SHOPREFUNDAPPLYDETAIL =
            @"SELECT ID,ApplyID,GoodsID,RealGoodsID,GoodsName,GoodsCode, 
Specification,Price,Quantity,Units,BatchNo,EffectiveDate FROM ShopExchangedApplyDetail";

        /// <summary>
        /// 判断主商品是否存在退换货申请
        /// </summary>
        private const string SQL_SELECT_ISEXISTREFUNDAPPLY = @" SELECT COUNT(*) FROM ShopExchangedApplyDetail SD 
INNER JOIN ShopExchangedApply SA ON SD.ApplyID=SA.ApplyID WHERE SD.GoodsID=@GoodsID";

        private const string SQL_SELECT_ISALLOWPURCHASE = @"SELECT GoodsID,(CASE WHEN SUM(SEA.Quantity)>0 THEN 1
	ELSE 0 END) AS IsAllow FROM ShopExchangedApplyDetail SEA INNER JOIN ShopExchangedApply SE
ON SEA.ApplyID=SE.ApplyID WHERE {0} SE.ShopID=@ShopID AND IsBarter=@IsBarter AND SE.ExchangedState!=10 {1} GROUP BY SEA.GoodsID ";

        #endregion

        #region  参数

        private const string PARM_ID = "@ID";
        private const string PARM_APPLYID = "@ApplyID";
        private const string PARM_GOODS_ID = "@GoodsID";
        private const string PARM_REAL_GOODS_ID = "@RealGoodsID";
        private const string PARM_GOODS_NAME = "@GoodsName";
        private const string PARM_GOODS_CODE = "@GoodsCode";
        private const string PARM_SPECIFICATION = "@Specification";
        private const string PARM_PRICE = "@Price";
        private const string PARM_QUANTITY = "@Quantity";
        private const string PARM_UNITS = "@Units";
        private const string PARM_BARTER_GOODS_ID = "@BarterGoodsID";
        private const string PARM_BARTER_REAL_GOODS_ID = "@BarterRealGoodsID";
        private const string PARM_BARTER_GOODS_NAME = "@BarterGoodsName";
        private const string PARM_BARTER_GOODS_CODE = "@BarterGoodsCode";
        private const string PARM_BARTER_SPECIFICATION = "@BarterSpecification";
        private const string PARM_BATCHNO = "@BatchNo";

        #endregion

        /// <summary>
        /// 插入换货明细
        /// </summary>
        /// <param name="applyDetailInfo"></param>
        public void InsertShopExchangedApplyDetail(ShopExchangedApplyDetailInfo applyDetailInfo)
        {
            try
            {
                using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
                {
                    conn.Execute(SQL_INSERT_SHOPEXCHANGEDAPPLYDETAIL, new
                    {
                        ID = applyDetailInfo.ID,
                        ApplyID = applyDetailInfo.ApplyID,
                        GoodsID = applyDetailInfo.GoodsID,
                        RealGoodsID = applyDetailInfo.RealGoodsID,
                        GoodsName = applyDetailInfo.GoodsName,
                        GoodsCode = applyDetailInfo.GoodsCode,
                        Specification = string.IsNullOrEmpty(applyDetailInfo.Specification) ? string.Empty : applyDetailInfo.Specification,
                        Price = applyDetailInfo.Price,
                        Quantity = applyDetailInfo.Quantity,
                        Units = string.IsNullOrEmpty(applyDetailInfo.Units) ? string.Empty : applyDetailInfo.Units,
                        BarterGoodsID = applyDetailInfo.BarterGoodsID,
                        BarterRealGoodsID = applyDetailInfo.BarterRealGoodsID,
                        BarterGoodsName = applyDetailInfo.BarterGoodsName,
                        BarterGoodsCode = string.IsNullOrEmpty(applyDetailInfo.BarterGoodsCode) ? string.Empty : applyDetailInfo.BarterGoodsCode,
                        BarterSpecification = string.IsNullOrEmpty(applyDetailInfo.BarterSpecification) ? string.Empty : applyDetailInfo.BarterSpecification,
                        BatchNo = string.IsNullOrEmpty(applyDetailInfo.BatchNo) ? string.Empty : applyDetailInfo.BatchNo,
                        EffectiveDate = applyDetailInfo.EffectiveDate,
                    });
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("换货申请明细插入失败!", ex);
            }
        }

        /// <summary>
        /// 插入退货明细
        /// </summary>
        /// <param name="applyDetailInfo"></param>
        public void InsertShopdApplyDetail(ShopApplyDetailInfo applyDetailInfo)
        {
            try
            {
                using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
                {
                    conn.Execute(SQL_INSERT_SHOPREFUNDAPPLYDETAIL, new
                    {
                        ID = applyDetailInfo.ID,
                        ApplyID = applyDetailInfo.ApplyID,
                        GoodsID = applyDetailInfo.GoodsID,
                        RealGoodsID = applyDetailInfo.RealGoodsID,
                        GoodsName = applyDetailInfo.GoodsName,
                        GoodsCode = applyDetailInfo.GoodsCode,
                        Specification = string.IsNullOrEmpty(applyDetailInfo.Specification) ? string.Empty : applyDetailInfo.Specification,
                        Price = applyDetailInfo.Price,
                        Quantity = applyDetailInfo.Quantity,
                        Units = string.IsNullOrEmpty(applyDetailInfo.Units) ? string.Empty : applyDetailInfo.Units,
                        BatchNo = string.IsNullOrEmpty(applyDetailInfo.BatchNo) ? string.Empty : applyDetailInfo.BatchNo,
                        EffectiveDate = applyDetailInfo.EffectiveDate,
                    });
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("换货申请明细插入失败!", ex);
            }
        }

        /// <summary>
        /// 删除退换货明细
        /// </summary>
        /// <param name="id">单条明细ID</param>
        /// <returns></returns>
        public int DeleteShopExchangedApplyDetail(Guid id)
        {
            try
            {
                using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
                {
                    return conn.Execute(SQL_DELETE_APPLYDETAILBYID, new
                    {
                        ID = id,
                    });
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("退换货申请明细(单条)删除失败!", ex);
            }
        }

        /// <summary>
        /// 删除退换货商品明细(申请单的所有明细)
        /// </summary>
        /// <param name="applyId">退换货申请ID</param>
        /// <returns></returns>
        public int DeleteShopExchangedApplyDetails(Guid applyId)
        {
            try
            {
                using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
                {
                    return conn.Execute(SQL_DELETE_APPLYDETAILBYAPPLYID, new
                    {
                        ApplyID = applyId,
                    });
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("退换货申请明细删除失败!", ex);
            }
        }

        /// <summary>
        /// 获取退货明细
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ShopApplyDetailInfo GetShopApplyDetailInfo(Guid id)
        {
            var builder = new StringBuilder(SQL_SELECT_SHOPREFUNDAPPLYDETAIL);
            builder.Append(" WHERE ID=@ID");

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.QueryFirstOrDefault<ShopApplyDetailInfo>(builder.ToString(), new
                {
                    ID = id,
                });
            }
        }

        /// <summary>
        /// 获取换货明细
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ShopExchangedApplyDetailInfo GetShopExchangedApplyDetailInfo(Guid id)
        {
            var builder = new StringBuilder(SQL_SELECT_SHOPEXCHANGEDAPPLYDETAIL);
            builder.Append(" WHERE ID=@ID");

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.QueryFirstOrDefault<ShopExchangedApplyDetailInfo>(builder.ToString(), new
                {
                    ID = id,
                });
            }
        }

        /// <summary>
        /// 获取退货申请明细列表
        /// </summary>
        /// <param name="applyId"></param>
        /// <returns></returns>
        public IEnumerable<ShopApplyDetailInfo> GetShopApplyDetailList(Guid applyId)
        {
            var builder = new StringBuilder(SQL_SELECT_SHOPREFUNDAPPLYDETAIL);
            builder.Append(" WHERE ApplyID=@ApplyID");
            builder.Append(" ORDER BY GoodsName,Specification ");

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.Query<ShopApplyDetailInfo>(builder.ToString(), new
                {
                    ApplyID = applyId,
                }).AsList();
            }
        }

        /// <summary>
        /// 获取换货申请明细列表
        /// </summary>
        /// <param name="applyId"></param>
        /// <returns></returns>
        public IEnumerable<ShopExchangedApplyDetailInfo> GetShopExchangedApplyDetailList(Guid applyId)
        {
            var builder = new StringBuilder(SQL_SELECT_SHOPEXCHANGEDAPPLYDETAIL);
            builder.Append(" WHERE ApplyID=@ApplyID");
            builder.Append(" ORDER BY GoodsName,Specification ");

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.Query<ShopExchangedApplyDetailInfo>(builder.ToString(), new
                {
                    ApplyID = applyId,
                }).AsList();
            }
        }

        /// <summary>
        /// 判断商品是否存在退货申请
        /// </summary>
        /// <param name="shopId"></param>
        /// <param name="goodsId"></param>
        /// <param name="states"></param>
        /// <returns></returns>
        public bool IsExistExchangedData(Guid shopId, Guid goodsId, IList<int> states)
        {
            var builder = new StringBuilder(SQL_SELECT_ISEXISTREFUNDAPPLY);
            var stateStr = new StringBuilder();
            if (shopId != Guid.Empty)
            {
                builder.Append(" AND SA.ShopID='").Append(shopId).Append("'");
            }
            builder.Append(" AND SA.IsBarter=1 ");
            if (states != null && states.Count > 0)
            {
                foreach (var id in states)
                {
                    if (id != -1)
                    {
                        if (stateStr.Length == 0)
                        {
                            stateStr.Append(id);
                        }
                        else
                        {
                            stateStr.Append(",").Append(id);
                        }
                    }
                }
                if (stateStr.Length > 0)
                {
                    builder.AppendFormat(" AND SA.ExchangedState IN({0})", stateStr);
                }
            }

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.ExecuteScalar<int>(builder.ToString(), new
                {
                    GoodsID = goodsId,
                }) > 0;
            }
        }

        /// <summary>
        /// 判断一段时间内商品是否可采购
        /// </summary>
        /// <param name="shopId"></param>
        /// <param name="isBarter">false：换货,true 退货</param>
        /// <param name="goodsIds"> </param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns>key：主商品Id，value：是否可以采购</returns>
        public Dictionary<Guid, bool> IsAllowPurchase(Guid shopId, bool isBarter, IList<Guid> goodsIds, DateTime startTime, DateTime endTime)
        {
            var date = startTime <= DateTime.MinValue && endTime <= DateTime.MinValue;
            var goodsStr = new StringBuilder("");
            if (goodsIds != null && goodsIds.Count > 0)
            {
                goodsStr.Append(" AND SEA.GoodsID IN(");
                for (int i = 0; i < goodsIds.Count; i++)
                {
                    goodsStr.Append("'").Append(goodsIds[i]).Append("'");
                    if (i != goodsIds.Count - 1)
                        goodsStr.Append(",");
                }
                goodsStr.Append(")");
            }
            string sql = string.Format(SQL_SELECT_ISALLOWPURCHASE,
                                       date ? "" : " SE.ApplyTime BETWEEN @StartTime AND @EndTime AND ", goodsStr);
            
            try
            {
                using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
                {
                    return conn.Query(sql, new
                    {
                        ShopID = shopId,
                        IsBarter = isBarter ? 1 : 0,
                        StartTime = startTime,
                        EndTime = endTime,
                    }).ToDictionary(kv => (Guid)kv.GoodsID, kv => (bool)kv.IsAllow);
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("查询退货商品列表失败!", ex);
            }
        }


        /// <summary>
        /// 获取退换货商品已退还的商品信息
        /// </summary>
        /// <param name="shopId"></param>
        /// <param name="isbarter"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="states"></param>
        /// <param name="goodsIds"> </param>
        /// <returns></returns>
        public Dictionary<Guid, Dictionary<Guid, int>> GetExchangedApplyGoodsQuantity(Guid shopId, int isbarter,
            DateTime startTime, DateTime endTime, IList<int> states, IList<Guid> goodsIds)
        {
            var builder = new StringBuilder(@"SELECT SEA.GoodsID,SEA.RealGoodsID,SUM(SEA.Quantity) AS Quantity FROM ShopExchangedApplyDetail SEA 
INNER JOIN ShopExchangedApply SE ON SEA.ApplyID=SE.ApplyID
WHERE SE.ApplyNo NOT LIKE '%D%' AND SE.ApplyTime BETWEEN '{0}' AND '{1}' AND SE.ShopID='{2}' ");
            if (isbarter != -1)
            {
                builder.AppendFormat(" AND SE.IsBarter={0} ", isbarter == 1 ? 1 : 0);
            }
            if (states != null && states.Count > 0)
            {
                builder.Append(" AND SE.ExchangedState IN (");
                for (int i = 0; i < states.Count; i++)
                {
                    builder.Append(states[i]);
                    if (states.Count - 1 != i)
                        builder.Append(",");
                }
                builder.Append(")");
            }
            if (goodsIds != null && goodsIds.Count > 0)
            {
                builder.Append(" AND SEA.GoodsID IN (");
                for (int i = 0; i < goodsIds.Count; i++)
                {
                    builder.AppendFormat("'{0}'", goodsIds[i]);
                    if (goodsIds.Count - 1 != i)
                        builder.Append(",");
                }
                builder.Append(")");
            }
            builder.Append(" GROUP BY SEA.GoodsID,SEA.RealGoodsID ");

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.Query(string.Format(builder.ToString(), startTime, endTime, shopId))
                    .GroupBy(g => (Guid)g.GoodsID)
                    .ToDictionary(kv => kv.Key, kv => kv.GroupBy(g2 => (Guid)g2.RealGoodsID).ToDictionary(kv2 => kv2.Key, kv2 => kv2.Sum(s => (int)s.Quantity)));
            }
        }

        /// <summary>
        /// 获取商品该店铺最近一次调拨出库的商品单价
        /// </summary>
        /// <param name="shopId"></param>
        /// <param name="goodsId"></param>
        /// <returns></returns>
        public decimal GetLastUnitPrice(Guid shopId, Guid goodsId)
        {
            string sql = @"select TOP 1 SRD.UnitPrice from StorageRecordDetail SRD INNER JOIN StorageRecord SR
 ON SRD.StockId=SR.StockId WHERE SR.RelevanceFilialeId='{0}' AND SR.StockType=" + (int)StorageRecordType.TransferStockOut + @" AND SR.StockState=" + (int)StorageRecordState.Finished + @" 
 AND SRD.GoodsId='{1}' ORDER BY SR.DateCreated DESC";

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.ExecuteScalar<decimal>(string.Format(sql, shopId, goodsId));
            }
        }

        #region  通过退换货单据号获取退换货申请明细 add by liangcanren at 2015-05-08
        /// <summary>
        /// 通过退换货单据号获取退换货申请明细 
        /// </summary>
        /// <param name="aplyNo"></param>
        /// <returns></returns>
        public IEnumerable<ShopExchangedApplyDetailInfo> GetShopExchangedApplyDetailListByNo(string aplyNo)
        {
            const string SQL = @"SELECT SAD.ID,SAD.ApplyID,SAD.GoodsID,SAD.RealGoodsID,SAD.GoodsName,SAD.GoodsCode, 
SAD.Specification,SAD.Price,SAD.Quantity,SAD.Units,SAD.BarterGoodsID,SAD.BarterRealGoodsID,
SAD.BarterGoodsName,SAD.BarterGoodsCode,SAD.BarterSpecification FROM ShopExchangedApplyDetail SAD
INNER JOIN ShopExchangedApply SA ON SAD.ApplyID=SA.ApplyID
WHERE SA.ApplyNo=@ApplyNo ORDER BY GoodsName,Specification ";

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.Query<ShopExchangedApplyDetailInfo>(SQL, new
                {
                    ApplyNo = aplyNo
                }).AsList();
            }
        }
        #endregion
    }
}
