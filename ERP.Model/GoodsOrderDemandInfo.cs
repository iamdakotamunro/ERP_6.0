using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ERP.Model
{
    [Serializable]
    public class GoodsOrderDemandInfo
    {
        public Guid OrderId { get; set; }

        public string OrderNo { get; set; }

        public int Quantity { get; set; }

        public int BusinessType { get; set; }

        public Guid SaleFilialeId { get; set; }

        public int LackQuantity { get; set; }
    }
}
