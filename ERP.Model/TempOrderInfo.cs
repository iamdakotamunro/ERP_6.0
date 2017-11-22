using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ERP.Model
{
    public class TempOrderInfo
    {
        public Guid OrderId { get; set; }

        public Guid DeliverFilialeId { get; set; }

        public Guid DeliverWarehouseId { get; set; }

        public DateTime OrderTime { get; set; }

        public Guid SaleFilialeId { get; set; }

        public Guid SalePlatformId { get; set; }
    }
}
