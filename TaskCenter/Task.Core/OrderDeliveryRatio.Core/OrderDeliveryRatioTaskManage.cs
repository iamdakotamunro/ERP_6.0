using System;
using ERP.DAL.Implement.Order;
using ERP.SAL;
using Framework.Common;

namespace OrderDeliveryRatio.Core
{
    /// <summary>订单发货率计算任务  2015-04-21  陈重文 
    /// </summary>
    public class OrderDeliveryRatioTaskManage
    {
        private static readonly OrderDeliveryRatioDAL _orderDeliveryRatio = new OrderDeliveryRatioDAL(ERP.Environment.GlobalConfig.DB.FromType.Write);
        /// <summary>执行订单发货率任务
        /// </summary>
        public static void RunOrderDeliveryRatioDeclare()
        {
            try
            {
                var date = DateTime.Now.AddDays(-1);
                var dayTime = HRSSao.DeliverWorkEndTime(date.Year, date.Month, date.Day);
                _orderDeliveryRatio.InsertOrderDeliveryRatio(dayTime.AddHours(-1));
            }
            catch (Exception exp)
            {
                ERP.SAL.LogCenter.LogService.Log(LogCenter.Logger.LogLevel.Error, "执行订单发货率异常" + ":" + exp.Message, "订单发货率计算任务", new LogCenter.Logger.Models.MethodInfo("OrderDeliveryRatio.Core", "OrderDeliveryRatioTaskManage", "RunOrderDeliveryRatioDeclare"), exp);
            }
        }
    }
}
