using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using ERP.DAL.Interface.IInventory;
using ERP.Environment;
using Keede.DAL.Helper;
using Keede.Ecsoft.Model;

namespace ERP.DAL.Implement.Inventory
{
    /// <summary>
    /// 采购促销明细
    /// </summary>
    public class PurchasePromotion : IPurchasePromotion
    {
        public PurchasePromotion(Environment.GlobalConfig.DB.FromType fromType)
        {

        }

        #region [SQL语句]
        private const string SQL_SELECT_PURCHASEPROMOTION = "SELECT [PromotionId],[GoodsId],[PromotionType],[PromotionKind],[BuyCount],[GivingCount],[PromotionInfo],[StartDate],[EndDate],[IsSingle],[WarehouseId],[HostingFilialeId] FROM [lmshop_PurchasePromotion]";
        private const string SQL_INSERT_PURCHASEPROMOTION = "INSERT INTO [lmshop_PurchasePromotion]([PromotionId],[GoodsId],[PromotionType],[PromotionKind],[BuyCount],[GivingCount],[PromotionInfo],[StartDate],[EndDate],[IsSingle],[WarehouseId],[HostingFilialeId]) VALUES(@PromotionId,@GoodsId,@PromotionType,@PromotionKind,@BuyCount,@GivingCount,@PromotionInfo,@StartDate,@EndDate,@IsSingle,@WarehouseId,@HostingFilialeId)";
        #endregion

        #region [参数]
        private const string PARM_PROMOTIONID = "@PromotionId";
        private const string PARM_GOODSID = "@GoodsId";
        private const string PARM_PROMOTIONTYPE = "@PromotionType";
        private const string PARM_PROMOTIONKIND = "@PromotionKind";
        private const string PARM_BUYCOUNT = "@BuyCount";
        private const string PARM_GIVINGCOUNT = "@GivingCount";
        private const string PARM_PROMOTIONINFO = "@PromotionInfo";
        private const string PARM_STARTDATE = "@StartDate";
        private const string PARM_ENDDATE = "@EndDate";
        private const string PARM_ISSINGLE = "@IsSingle";
        #endregion


        private static SqlParameter[] GetPurchasePromotionParameters()
        {
            var parms = new[] {
                new SqlParameter(PARM_PROMOTIONID, SqlDbType.UniqueIdentifier),
                new SqlParameter(PARM_GOODSID, SqlDbType.UniqueIdentifier),
                new SqlParameter(PARM_PROMOTIONTYPE, SqlDbType.Int),
                new SqlParameter(PARM_PROMOTIONKIND, SqlDbType.Int),
                new SqlParameter(PARM_BUYCOUNT, SqlDbType.Int),
                new SqlParameter(PARM_GIVINGCOUNT, SqlDbType.Int),
                new SqlParameter(PARM_PROMOTIONINFO, SqlDbType.VarChar,5000),
                new SqlParameter(PARM_STARTDATE, SqlDbType.DateTime),
                new SqlParameter(PARM_ENDDATE, SqlDbType.DateTime),
               new SqlParameter(PARM_ISSINGLE,SqlDbType.Bit) ,
               new SqlParameter("@WarehouseId",SqlDbType.UniqueIdentifier), 
               new SqlParameter("@HostingFilialeId",SqlDbType.UniqueIdentifier)
            };
            return parms;
        }

        private PurchasePromotionInfo ReaderPurchasePromotion(IDataReader dr)
        {
            var info = new PurchasePromotionInfo
            {
                PromotionId = dr["PromotionId"] == DBNull.Value ? Guid.Empty : new Guid(dr["PromotionId"].ToString()),
                GoodsId = dr["GoodsId"] == DBNull.Value ? Guid.Empty : new Guid(dr["GoodsId"].ToString()),
                PromotionType = dr["PromotionType"] == DBNull.Value ? 0 : int.Parse(dr["PromotionType"].ToString()),
                PromotionKind = dr["PromotionKind"] == DBNull.Value ? 0 : int.Parse(dr["PromotionKind"].ToString()),
                BuyCount = dr["BuyCount"] == DBNull.Value ? 0 : int.Parse(dr["BuyCount"].ToString()),
                GivingCount = dr["GivingCount"] == DBNull.Value ? 0 : int.Parse(dr["GivingCount"].ToString()),
                PromotionInfo = dr["PromotionInfo"] == DBNull.Value ? string.Empty : dr["PromotionInfo"].ToString(),
                StartDate = dr["StartDate"] == DBNull.Value ? DateTime.MinValue : DateTime.Parse(dr["StartDate"].ToString()),
                EndDate = dr["EndDate"] == DBNull.Value ? DateTime.MinValue : DateTime.Parse(dr["EndDate"].ToString()),
                IsSingle = dr["IsSingle"] != DBNull.Value && Convert.ToBoolean(dr["IsSingle"]),
                WarehouseId = dr["WarehouseId"]==DBNull.Value?Guid.Empty:new Guid(dr["WarehouseId"].ToString()),
                HostingFilialeId = dr["HostingFilialeId"] == DBNull.Value ? Guid.Empty : new Guid(dr["HostingFilialeId"].ToString())
            };
            return info;
        }

        /// <summary>
        /// 根据PromotionId获取采购促销明细
        /// </summary>
        /// <param name="promotionId"></param>
        /// <returns></returns>
        public IList<PurchasePromotionInfo> GetPurchasePromotionList(Guid promotionId)
        {
            IList<PurchasePromotionInfo> list = new List<PurchasePromotionInfo>();
            const string SQL = SQL_SELECT_PURCHASEPROMOTION + " WHERE [PromotionId]=@PromotionId";
            var parameter = new SqlParameter(PARM_PROMOTIONID, SqlDbType.UniqueIdentifier) { Value = promotionId };

            using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL, parameter))
            {
                while (dr.Read())
                {
                    list.Add(ReaderPurchasePromotion(dr));
                }
            }
            return list;
        }


        /// <summary>
        /// 根据商品Id获取采购促销信息
        /// </summary>
        /// <param name="goodsId"></param>
        /// <param name="hostingFilialeId"></param>
        /// <param name="warehouseId"></param>
        /// <returns></returns>
        public IList<PurchasePromotionInfo> GetPurchasePromotionListByGoodsId(Guid goodsId, Guid hostingFilialeId,Guid warehouseId)
        {
            IList<PurchasePromotionInfo> list = new List<PurchasePromotionInfo>();
            const string SQL = SQL_SELECT_PURCHASEPROMOTION + " WHERE [GoodsId]=@GoodsId AND [WarehouseId]=@WarehouseId AND HostingFilialeId=@HostingFilialeId";
            var parameter =  new []
            {
                new SqlParameter("@GoodsId", SqlDbType.UniqueIdentifier) { Value = goodsId },
                new SqlParameter("@WarehouseId",SqlDbType.UniqueIdentifier){Value = warehouseId},
                new SqlParameter("@HostingFilialeId",SqlDbType.UniqueIdentifier){Value = hostingFilialeId}
            };

            using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL, parameter))
            {
                while (dr.Read())
                {
                    list.Add(ReaderPurchasePromotion(dr));
                }
            }
            return list;
        }

        /// <summary>
        /// 根据商品ID和是否现返获取采购促销明细
        /// </summary>
        /// <param name="promotionId"></param>
        /// <param name="goodsId"></param>
        /// <param name="warehouseId"></param>
        /// <param name="hostingFilialeId"></param>
        /// <param name="promotionType"></param>
        /// <returns></returns>
        public IList<PurchasePromotionInfo> GetPurchasePromotionList(Guid promotionId, Guid goodsId, Guid warehouseId,Guid hostingFilialeId, int promotionType)
        {
            IList<PurchasePromotionInfo> list = new List<PurchasePromotionInfo>();
            const string SQL = SQL_SELECT_PURCHASEPROMOTION + " WHERE [GoodsId]=@GoodsId  AND [WarehouseId]=@WarehouseId AND [PromotionType]=@PromotionType AND HostingFilialeId=@HostingFilialeId";

            var parms = new[]
                            {
                                new SqlParameter(PARM_GOODSID, SqlDbType.UniqueIdentifier),
                                new SqlParameter(PARM_PROMOTIONTYPE, SqlDbType.Int),
                                new SqlParameter("@WarehouseId",SqlDbType.UniqueIdentifier),
                                new SqlParameter("@HostingFilialeId",SqlDbType.UniqueIdentifier)
                            };
            parms[0].Value = goodsId;
            parms[1].Value = promotionType;
            parms[2].Value = warehouseId;
            parms[3].Value = hostingFilialeId;

            using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL, parms))
            {
                while (dr.Read())
                {
                    list.Add(ReaderPurchasePromotion(dr));
                }
            }
            return list;
        }


        /// <summary>
        /// 添加采购促销明细
        /// </summary>
        /// <param name="info"></param>
        public void AddPurchasePromotion(PurchasePromotionInfo info)
        {
            SqlParameter[] parms = GetPurchasePromotionParameters();
            parms[0].Value = info.PromotionId;
            parms[1].Value = info.GoodsId;
            parms[2].Value = info.PromotionType;
            parms[3].Value = info.PromotionKind;
            parms[4].Value = info.BuyCount;
            parms[5].Value = info.GivingCount;
            parms[6].Value = info.PromotionInfo;
            parms[7].Value = info.StartDate;
            parms[8].Value = info.EndDate;
            parms[9].Value = info.IsSingle ? 1 : 0;
            parms[10].Value = info.WarehouseId;
            parms[11].Value = info.HostingFilialeId;

            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL_INSERT_PURCHASEPROMOTION, parms);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// 删除采购促销明细
        /// </summary>
        /// <param name="promotionId"></param>
        public void DeletePurchasePromotion(Guid promotionId)
        {
            const string SQL = "DELETE FROM [lmshop_PurchasePromotion] WHERE [PromotionId]=@PromotionId";
            var parameter = new SqlParameter(PARM_PROMOTIONID, SqlDbType.UniqueIdentifier) { Value = promotionId };

            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL, parameter);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
    }
}
