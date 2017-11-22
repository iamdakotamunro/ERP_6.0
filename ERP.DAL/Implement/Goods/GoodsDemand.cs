using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using ERP.DAL.Interface.IGoods;
using ERP.Environment;
using Keede.DAL.RWSplitting;
using Dapper;
using System.Linq;
using System.Transactions;

namespace ERP.DAL.Implement.Goods
{
    public class GoodsDemand : IGoodsDemand
    {
        public GoodsDemand(GlobalConfig.DB.FromType fromType) { }
        
        private const string PARM_FILIALEID = "@FilialeId";
        private const string PARM_STARTTIME = "@StartTime";
        private const string PARM_ENDTIME = "@EndTime";
        private const string PARM_GOODSID = "@GoodsId";
        private const string PARM_WAREHOUSEID = "@warehouseId";

        /// <summary>
        /// 获取商品的货位号（分仓后使用）
        /// </summary>
        /// <param name="goodsId"></param>
        /// <param name="filialeId"></param>
        /// <param name="warehouseId"></param>
        /// <returns></returns>
        public string GetGoodsShelfNo(Guid goodsId, Guid filialeId, Guid warehouseId)
        {
            const string SQL_SELECT_SHELFNO = "SELECT ShelfNo FROM lmShop_Shelf WHERE RealGoodsId=@GoodsId AND FilialeID=@FilialeId AND WarehouseID=@WarehouseId;";

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.ExecuteScalar<string>(SQL_SELECT_SHELFNO, new
                {
                    GoodsId = goodsId,
                    FilialeId = filialeId,
                    WarehouseId = warehouseId
                });
            }
        }

        /// <summary>
        /// 删除该商品到货通知
        /// </summary>
        /// <param name="goodsId"></param>
        public void DeleteGoodsStockStatement(Guid goodsId)
        {
            const string SQL_DELETE_BY_GOODSID = @"Delete From lmshop_GoodsStockStatement Where GoodsId=@GoodsId";
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                conn.Execute(SQL_DELETE_BY_GOODSID, new
                {
                    GoodsId = goodsId,
                });
            }
        }

        /// <summary>
        /// 获取商品销量列表
        /// Key是商品ID，Value是销售个数
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public IList<KeyValuePair<Guid, int>> GetGoodsSales(DateTime startTime, DateTime endTime)
        {
            const string SQL_SELECT_GOODS_SALES = "SELECT [RealGoodsId],SUM(isnull([GoodsSales],0)) as SaleNum FROM lmshop_GoodsDaySalesStatistics WHERE [DayTime] >= @StartTime AND [DayTime] <= @EndTime GROUP BY [RealGoodsId]";
            var paramStartTime = new SqlParameter(PARM_STARTTIME, SqlDbType.Date) { Value = startTime };
            var paramEndTime = new SqlParameter(PARM_ENDTIME, SqlDbType.Date) { Value = endTime };

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.Query(SQL_SELECT_GOODS_SALES, new
                {
                    StartTime = paramStartTime,
                    EndTime = paramEndTime,
                }).ToDictionary(kv => (Guid)kv.RealGoodsId, kv => (int)kv.SaleNum).ToList();
            }
        }
    }
}
