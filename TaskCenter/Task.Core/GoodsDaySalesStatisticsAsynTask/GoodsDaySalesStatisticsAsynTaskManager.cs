using System;
using System.Collections.Generic;
using System.Linq;
using ERP.BLL.Implement;
using ERP.BLL.Implement.Order;
using ERP.DAL.Implement.Order;
using ERP.DAL.Interface.IOrder;
using ERP.Environment;
using ERP.Model;
using Keede.Ecsoft.Model;
using Framework.Core.Serialize;

namespace GoodsDaySalesStatisticsAsynTask
{
    public class GoodsDaySalesStatisticsAsynTaskManager
    {
        private static readonly IGoodsOrderDetail _goodsOrderDetail = new GoodsOrderDetail(GlobalConfig.DB.FromType.Write);
        private static readonly OrderManager _orderManager = OrderManager.WriteInstance;
        private static readonly RealTimeGrossSettlementManager _realTimeGrossSettlementManager =new RealTimeGrossSettlementManager(GlobalConfig.DB.FromType.Write);

        public static void RecordGoodsDaySalesStatistics()
        {
            try
            {
                var asynGoodsDaySalesStatisticsList = _goodsOrderDetail.GetAsynGoodsDaySalesStatisticsList();
                foreach (var item in asynGoodsDaySalesStatisticsList)
                {
                    GoodsOrderInfo orderInfo;
                    List<GoodsOrderDetailInfo> orderDetail;
                    List<GoodsOrderDetailInfo> oldOrderDetails;
                    JsonDeserialize(item, out orderInfo, out orderDetail, out oldOrderDetails);
                    var dicAvgSettlePrice = _realTimeGrossSettlementManager.GetLatestUnitPriceListByMultiGoods(orderInfo.SaleFilialeId, orderDetail.Select(ent => ent.GoodsID));
                    //销量处理状态（0新增销量，1更新销量，2删除销量）
                    switch (item.HandlingStatus)
                    {
                        case 0://新增销量
                            _orderManager.SaveGoodsDaySalesStatistics(orderInfo, orderDetail, dicAvgSettlePrice);
                            break;
                        case 1://更新销量
                            _orderManager.UpdateGoodsDaySalesStatistics(orderInfo, orderDetail, true, oldOrderDetails, dicAvgSettlePrice);
                            break;
                        case 2://删除销量
                            _orderManager.UpdateGoodsDaySalesStatistics(orderInfo, orderDetail, false, oldOrderDetails, dicAvgSettlePrice);
                            break;
                    }
                    _goodsOrderDetail.SetAsynGoodsDaySalesStatisticsExecuted(item.Id);
                }
            }
            catch (Exception ex)
            {
                ERP.SAL.LogCenter.LogService.LogError("异步销量任务执行异常", "GoodsDaySalesStatisticsAsynTaskManager.Error",  ex);
            }
        }

        static void JsonDeserialize(ASYN_GoodsDaySalesStatisticsInfo asynGoodsDaySalesStatisticsInfo, out GoodsOrderInfo orderInfo, out List<GoodsOrderDetailInfo> orderDetail, out List<GoodsOrderDetailInfo> oldOrderDetails)
        {
            var serrializer = new JsonSerializer();
            orderInfo = serrializer.Deserialize<GoodsOrderInfo>(asynGoodsDaySalesStatisticsInfo.OrderJsonStr);
            orderDetail = serrializer.Deserialize<List<GoodsOrderDetailInfo>>(asynGoodsDaySalesStatisticsInfo.OrderDetailJsonStr);
            oldOrderDetails = serrializer.Deserialize<List<GoodsOrderDetailInfo>>(asynGoodsDaySalesStatisticsInfo.OldOrderDetailJsonStr);
        }
    }
}
