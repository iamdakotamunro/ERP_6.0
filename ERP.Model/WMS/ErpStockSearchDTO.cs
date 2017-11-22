using System;
using System.Collections.Generic;

namespace ERP.Model.WMS
{
    [Serializable]
    public class ErpStockSearchDTO
    {
        public Guid FilialeId { get; set; }

        public String FilialeName { get; set; }

        public Dictionary<Guid, int> StockQuantity { get; set; }

        public ErpStockSearchDTO() { }

        public ErpStockSearchDTO(Guid filialeId, String filialeName, Dictionary<Guid, int> stockQuantity)
        {
            FilialeId = filialeId;
            FilialeName = filialeName;
            StockQuantity = stockQuantity;
        }
    }
}
