using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoPurchasing.Core.Model
{
    [Serializable]
    public class GoodsSaleDaysInfo
    {
        public Guid RealGoodsId { get; set; }

        public Guid HostingFilialeId { get; set; }

        public int Days { get; set; }
    }
}
