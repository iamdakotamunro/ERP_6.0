using System;
using System.Linq;
using ERP.BLL.Implement.Order;
using ERP.DAL.Interface.IOrder;
using Keede.Ecsoft.Model;

namespace CompleteOrderTask.Core
{
    /// <summary>
    /// 完成订单的第二步骤
    /// </summary>
    public class CompleteSecondTask
    {
        private static bool _isRunningCompleteOrder;
        private static readonly object _lockObj = new object();
        private static readonly IGoodsOrder _goodsOrder = new ERP.DAL.Implement.Order.GoodsOrder(ERP.Environment.GlobalConfig.DB.FromType.Write);
        private static readonly OrderManager _orderManager = new OrderManager();

        public static void RunWaitConsignmentOrderTask()
        {
            try
            {
                lock (_lockObj)
                {
                    if (_isRunningCompleteOrder)
                    {
                        return;
                    }
                }

                //验证时间
                var hour = DateTime.Now.Hour;
                if (!GlobalConfig.SecondConsignmentOrderHour.Contains(hour.ToString()))
                {
                    return;
                }

                //开始继续完成订单数据
                var waitConsignmentOrderList = _goodsOrder.GetWaitConsignmentOrder(GlobalConfig.ReadWaitConsignmentOrder);
                if (waitConsignmentOrderList.Count > 0)
                {
                    LogMessage("读取数据：" + waitConsignmentOrderList.Count);
                }
                lock (_lockObj)
                {
                    _isRunningCompleteOrder = true;
                }
                foreach (var waitInfo in waitConsignmentOrderList)
                {
                    GoodsOrderInfo orderInfo = null;
                    try
                    {
                        orderInfo = _goodsOrder.GetGoodsOrder(waitInfo.OrderId);
                        string errorMessage;
                        var success = _orderManager.FinishConsignmentOrder(orderInfo, waitInfo.Operator, out errorMessage);
                        if (!success)
                        {
                            LogMessage("订单号：" + orderInfo.OrderNo + "，完成失败！ 》》 " + errorMessage);
                        }
                    }
                    catch (Exception exp)
                    {
                        if (orderInfo != null)
                            LogMessage("订单号：" + orderInfo.OrderNo + "，完成失败！ 》》 " + exp.Message, exp);
                        else
                            LogMessage("获取订单失败：" + waitInfo.OrderId + " ! 》》 " + exp.Message, exp);
                    }
                }
                lock (_lockObj)
                {
                    _isRunningCompleteOrder = false;
                }
            }
            catch (Exception exp)
            {
                LogMessage("发生错误异常" + exp.Message);
            }
        }

        static void LogMessage(string message)
        {
            ERP.SAL.LogCenter.LogService.LogInfo(message, "订单完成异步处理", null);
        }

        static void LogMessage(string message, Exception exp)
        {
            ERP.SAL.LogCenter.LogService.LogInfo(message, "订单完成异步处理", exp);
        }
    }
}
