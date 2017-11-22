using System;
using System.Collections.Generic;
using System.Linq;

namespace StorageTask.Core.DAL
{
    internal class GoodsStockCurrentDao
    {
        public static Model.NonceGoodsStockInfo GetNonceStockInfo(Guid goodsId, Guid warehouseId)
        {
            const string SQL = @"
SELECT 	
	 RealGoodsId
    ,[WarehouseId]
    ,[NonceWarehouseGoodsStock] AS [NonceBalance]
  FROM [lmShop_GoodsStockCurrent]
  WHERE RealGoodsId=@GoodsId AND WarehouseId=@WarehouseId
";
            using (var db = DaoFactory.CreateERPDatabase())
            {
                return db.Run(SQL)
                    .AddParameter("GoodsId", goodsId)
                    .AddParameter("WarehouseId", warehouseId)
                    .Single<Model.NonceGoodsStockInfo>();
            }
        }
    }
}
