using System;
using System.Threading;

namespace CompleteOrderServiceHost
{
    public class Finish : IFinish
    {
        public bool FinishOrder(DateTime finishDate, Guid warehouseId, Guid expressId, Guid personnelId, string operationer)
        {
            return true;
        }
    }
}
