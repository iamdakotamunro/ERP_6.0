using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StorageTask.Core.Model
{
    /// <summary>
    /// 当前商品库存信息
    /// </summary>
    public class NonceGoodsStockInfo
    {
        public Guid RealGoodsId { get; set; }
        public Guid WarehouseId { get; set; }
        public int NonceBalance { get; set; }
    }
}
