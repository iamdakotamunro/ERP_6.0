using Keede.DAL.Helper;
using System;

namespace StorageTask.Core.DAL
{
    internal class StorageRecordBalanceDetailDao
    {
        public static bool Insert(Model.StorageRecordDetailBalanceInfo balanceInfo)
        {
            const string SQL = @"
IF NOT EXISTS(SELECT 1 FROM StorageRecordBalanceDetail WHERE DetailId=@DetailId)
	BEGIN
		INSERT INTO StorageRecordBalanceDetail 
            ([DetailId]
            ,[RealGoodsId]
           ,[WarehouseId]
           ,[NonceBalance]
           ,[TimeIndex])
        VALUES
            (@DetailId,@RealGoodsId,@WarehouseId,@NonceBalance,@TimeIndex)
	END
";
            using (var db = DaoFactory.CreateERPDatabase())
            {
                return db.Run(SQL)
                    .AddParameter("DetailId", balanceInfo.DetailId)
                    .AddParameter("RealGoodsId",balanceInfo.RealGoodsId)
                    .AddParameter("NonceBalance", balanceInfo.NonceBalance)
                    .AddParameter("TimeIndex", balanceInfo.TimeIndex)
                    .AddParameter("WarehouseId", balanceInfo.WarehouseId)
                    .Execute();
            }
        }

        /// <summary>
        /// 获取最近一个商品的库存数据
        /// </summary>
        /// <param name="goodsId"></param>
        /// <param name="warehouseId"></param>
        /// <returns></returns>
        public static Model.NonceGoodsStockInfo GetRecentBalanceInfo(Guid goodsId, Guid warehouseId)
        {
            const string SQL = @"
SELECT TOP 1 SRBD.WarehouseId,SRD.GoodsId AS RealGoodsId,NonceBalance FROM StorageRecordBalanceDetail SRBD
INNER JOIN StorageRecordDetail SRD ON SRD.ID = SRBD.DetailId
WHERE GoodsId=@GoodsId AND SRBD.WarehouseId=@WarehouseId
ORDER BY SRBD.TimeIndex DESC
";
            using (var db = DaoFactory.CreateERPDatabase())
            {
                return db.Single<Model.NonceGoodsStockInfo>(true, SQL, new Parameter("GoodsId", goodsId), new Parameter("WarehouseId", warehouseId));
            }
        }
    }
}
