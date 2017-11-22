using System;
using System.Collections.Generic;

namespace ERP.Model.WMS
{
    [Serializable]
    public class HostingFilialeSaleDTO
    {
        public Guid WarehouseId { get; set; }

        public string WarehouseName { get; set; }

        public Guid HostingFilialeId { get; set; }

        public string HostingFilialeName { get; set; }

        public Guid KeyId { get; set; }

        public List<HostingFilialeSaleDetailDTO> SaleDetails { get; set; }
    }
}
