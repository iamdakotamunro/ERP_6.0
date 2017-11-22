using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework.Core.Extension;
using Framework.Core.Utility;
using Config.Keede.Library;

namespace SaleFilialeGenerateStockInAndPurchaseForm
{
    public class SaleFilialeGenerateStockInAndPurchaseFormTaskConfig
    {
        private static int _triggerStartHour = 0;
        private static int _triggerEndHour = 0;

        public static void Init()
        {
            //触发的小时段，如果不设置，则表示不限制
            _triggerStartHour = ConfManager.GetAppsetting("SaleFilialeGenerateStockInAndPurchase.TriggerStartHour").ToInt();
            if (_triggerStartHour < 0 || _triggerStartHour>=24)
            {
                _triggerStartHour = 0;
            }
            _triggerEndHour = ConfManager.GetAppsetting("SaleFilialeGenerateStockInAndPurchase.TriggerEndHour").ToInt();
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
