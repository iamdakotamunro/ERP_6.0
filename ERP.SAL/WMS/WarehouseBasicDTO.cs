using System;
using System.Collections.Generic;

namespace ERP.SAL.WMS
{
    public class WarehouseBasicDTO
    {
        public Guid WarehouseId { get; set; }

        public string WarehouseName { get; set; }

        public Guid LogisticFilialeId { get; set; }

        public Dictionary<byte, string> StorageTypes { get; set; }
    }
}
