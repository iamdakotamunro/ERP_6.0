using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ERP.SAL.WMS
{
    [Serializable]
    public class StockStatisticsDTO
    {
        public Guid RealGoodsId { get; set; }

        public int CurrentStock { get; set; }

        public int UppingQuantity { get; set; }

        public int RequireQuantity { get; set; }

        public int SubtotalQuantity { get; set; }

        public Guid FilialeId { get; set; }

        public string FilialeName { get; set; }
    }
}
