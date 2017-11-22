using System;
using Framework.Core.Extension;
using Framework.Core.Utility;

namespace ArchiveLastMonthGoodsSettlement
{
    public class ArchiveLastMonthGoodsSettlementTaskConfig
    {
        private static int _triggerStartHour = 0;
        private static int _triggerEndHour = 0;

        public static void Init()
        {
            //触发的小时段，如果不设置，则表示不限制
            _triggerStartHour = Configuration.AppSettings["ArchiveLastMonthGoodsSettlement.TriggerStartHour"].ToInt();
            if (_triggerStartHour < 0 || _triggerStartHour>=24)
            {
                _triggerStartHour = 0;
            }
            _triggerEndHour = Configuration.AppSettings["ArchiveLastMonthGoodsSettlement.TriggerEndHour"].ToInt();
            if (_triggerEndHour <= 0 || _triggerEndHour >= 24)
            {
                _triggerEndHour = 23;
            }
        }

        /// <summary>
        /// 是否触发任务
        /// </summary>
        /// <returns></returns>
        public static bool CanTrigger()
        {
            int currentHour = DateTime.Now.Hour;
            return currentHour >= _triggerStartHour && currentHour <= _triggerEndHour;
        }
    }
}
