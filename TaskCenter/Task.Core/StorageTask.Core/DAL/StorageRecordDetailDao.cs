using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Enum;
using StorageTask.Core.Model;

namespace StorageTask.Core.DAL
{
    internal class StorageRecordDetailDao
    {
        /// <summary>
        /// 获取没有库存流水的商品ID
        /// </summary>
        /// <param name="top"></param>
        /// <returns></returns>
        public static IList<NonceGoodsStockInfo> GetNoStorageRecordBalanceGoodsId(int top)
        {
            string sql = @"
SELECT TOP {0} SRD.GoodsId AS RealGoodsId,SR.WarehouseId FROM StorageRecordDetail SRD
INNER JOIN StorageRecord SR ON SR.StockId = SRD.StockId
LEFT JOIN StorageRecordBalanceDetail SRBD ON SRBD.DetailId = SRD.ID
WHERE SRBD.DetailId IS NULL AND [StockState]=" + (int)StorageRecordState.Finished + @"
--AND SRD.RealGoodsID='18F90EB2-2A37-48CE-9D04-E05DF26C5AEE'
GROUP BY SRD.GoodsId,SR.WarehouseId
";
            using (var db = DaoFactory.CreateERPDatabase())
            {
                return db.Select<NonceGoodsStockInfo>(true, string.Format(sql, top)).ToList();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="goodsId"></param>
        /// <param name="warehouseId"></param>
        /// <returns></returns>
        public static IList<StorageRecordDetailBalanceInfo> GetNeedCalculateList(Guid goodsId, Guid warehouseId)
        {
            string sql = @"
SELECT ID AS DetailId,SRD.GoodsId AS RealGoodsId,SR.WarehouseId,Quantity,SR.DateCreated AS TimeIndex 
FROM StorageRecordDetail SRD
INNER JOIN StorageRecord SR ON SR.StockId = SRD.StockId
LEFT JOIN StorageRecordBalanceDetail SRBD ON SRBD.DetailId = SRD.ID
WHERE SRBD.DetailId IS NULL AND [StockState]=" + (int)StorageRecordState.Finished + @"
AND SRD.GoodsId=@GoodsId AND SR.WarehouseId=@WarehouseId
ORDER BY SR.DateCreated
";
            using (var db = DaoFactory.CreateERPDatabase())
            {
                return db.Run(sql)
                    .AddParameter("GoodsId", goodsId)
                    .AddParameter("WarehouseId", warehouseId)
                    .Select<StorageRecordDetailBalanceInfo>().ToList();
            }
        }
    }
}
