using System;
using System.Collections.Generic;
using Keede.Ecsoft.Enum;
using Keede.Ecsoft.Model;
using Keede.SynService;
using LonmeShop.OrdersBLL;

public class SyncGoodsOrder
{
    private readonly GoodsOrder goodOrderDal = new GoodsOrder();
    private readonly int shortagesTime = Convert.ToInt32(Function.GetConfig("ShortagesTime"));
    private DateTime shortagesDate = DateTime.MinValue;

    /// <summary>
    /// 把总后台一至两天所有的需调拔的单子的状态弄到分后台去
    /// </summary>
    public void SyncGoodsOrderRedploy()
    {
        if (DateTime.Now.Hour == shortagesTime && shortagesDate.ToString("yyyyMMdd") != DateTime.Now.ToString("yyyyMMdd"))
        {
            shortagesDate = DateTime.Now;
            int i = 0;
            IList<GoodsOrderInfo> goodsOrderList = goodOrderDal.GetGoodsOrderList(OrderState.Redeploy, 2, 1); //需拔调,两至三天的单子
            foreach (GoodsOrderInfo goodsOrderInfo in goodsOrderList)
            {
                using (var syn = new Synchronous(goodsOrderInfo.FromSourceId))
                {
                    syn.UpdateOrderState(goodsOrderInfo.OrderId, OrderState.Redeploy);
                }
                i++;
            }
        }
    }

}

