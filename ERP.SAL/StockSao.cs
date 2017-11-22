using System;
using System.Collections.Generic;

namespace ERP.SAL
{
    public class StockSao
    {
        public static bool IsExistGoodsStock(Guid filialeId, Guid compGoodsId, IList<Guid> realGoodsIds)
        {
            using (var client = ClientProxy.CreateShopStoreWcfClient(filialeId))
            {
                var result=client.Instance.IsExistGoodsNonceStockService(compGoodsId,realGoodsIds);
                return result != null && result.IsSuccess;
            }
        }
    }
}
