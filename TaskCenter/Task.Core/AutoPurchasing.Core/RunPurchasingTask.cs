using System;

namespace AutoPurchasing.Core
{
    public class RunAutoPurchasingTask
    {
        //符合当前自动报备的日期
        private static string AutoPurchasingShortDate = DateTime.MinValue.ToShortDateString();

        public static void RunAutoPurchasing()
        {
            int week = GetWeek(DateTime.Now);
            //今天周一或周三
            if (week == 1 || week == 3)
            {
                //几点报备,并且符合当前报备日期
                if (DateTime.Now.Hour == Convert.ToInt32(Config.AutoPurchasingTime) && Convert.ToDateTime(AutoPurchasingShortDate) < Convert.ToDateTime(DateTime.Now.ToShortDateString()))
                {
                    try
                    {
                        AutoPurchasingShortDate = DateTime.Now.ToShortDateString();
                        AutoPurchasing.RunTask();
                        ERP.SAL.LogCenter.LogService.LogInfo("自动报备(采购)日志,自动备货成功运行", "自动报备(采购)", null);
                    }
                    catch (Exception ex)
                    {
                        AutoPurchasingShortDate = DateTime.MinValue.ToShortDateString();
                        ERP.SAL.LogCenter.LogService.LogError("执行自动报备(采购)出错", "自动报备(采购)", ex);
                    }
                }
            }
        }

        /// <summary> 今天星期几
        /// </summary>
        /// <returns></returns>
        public static int GetWeek(DateTime dateTime)
        {
            //string[] weekdays = { "星期日", "星期一", "星期二", "星期三", "星期四", "星期五", "星期六" };
            int[] weekdays = { 7, 1, 2, 3, 4, 5, 6 };
            int week = weekdays[Convert.ToInt32(dateTime.DayOfWeek)];

            return week;
        }
    }
}
