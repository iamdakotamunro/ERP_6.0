using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepairRealTimeGrossSettlementApp.Model
{
    public class GoodsSplitCombineInfo
    {
        public Guid GoodsId { get; set; }

        public int Quantity { get; set; }

        public List<Tuple<Guid,int>> Details { get; set; }
    }
}
