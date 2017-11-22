using System;
namespace ERP.DAL
{
    public class CompleteOrderTask
    {
        private static readonly Interface.IOrder.ICompleteOrderTask _instance = new Implement.Order.CompleteOrderTaskDAL();

        public static Interface.IOrder.ICompleteOrderTask Instance
        {
            get { return _instance; }
        }
    }
}
