using System;
using ERP.Model;

namespace ERP.DAL.Interface.IGoods
{
    public interface IGoodsOrderDeliver
    {
        bool InsertOrderDeliver(GoodsOrderDeliverInfo orderDeliverInfo);

        bool UpdateOrderDeliver(GoodsOrderDeliverInfo orderDeliverInfo);

        bool DeleteOrderDeliver(Guid orderId);

        GoodsOrderDeliverInfo GetOrderDeliverInfoByOrderId(Guid orderId);
    }
}
