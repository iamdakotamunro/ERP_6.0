using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StorageTask.Core.Model
{
    public class StorageRecordDetailBalanceInfo
    {
        public Guid DetailId { get; set; }
        public Guid RealGoodsId { get; set; }
        public Guid WarehouseId { get; set; }
        public int Quantity { get; set; }
        public int NonceBalance { get; set; }
        public DateTime TimeIndex { get; set; }
    }
}
