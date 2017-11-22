using Config.Keede.Library;
using Framework.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GenerateSaleOrderGoodsSettlement
{
    public class GenerateSaleOrderGoodsSettlementTaskConfig
    {
        private static int _triggerStartHour = 0;
        private static int _triggerEndHour = 0;
        private static int _daysBefore = 2;
        private static int _maxRowCount = 100;

        public static void Init()
        {
            //触发的小时段，如果不设置，则表示不限制
            _triggerStartHour = ConfManager.GetAppsetting("GenerateSaleOrderGoodsSettlement.TriggerStartHour").ToInt();
            if (_triggerStartHour < 0 || _triggerStartHour>=24)
            {
                _triggerStartHour = 0;
            }
            _triggerEndHour = ConfManager.GetAppsetting("GenerateSaleOrderGoodsSettlement.TriggerEndHour").ToInt();
            if (_triggerEndHour <= 0 || _triggerEndHour >= 24)
            {
                _triggerEndHour = 23;
            }

            _daysBefore = ConfManager.GetAppsetting("GenerateSaleOrderGoodsSettlement.DaysBefore").ToInt();
            if (_daysBefore <= 0)
            {
                _daysBefore = 2;
            }

            _maxRowCount = ConfManager.GetAppsetting("GenerateSaleOrderGoodsSettlement.MaxRowCount").ToInt();
            if (_maxRowCount <= 0)
            {
                _maxRowCount = 100;
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

        /// <summary>
        /// 统计日期
        /// </summary>
        public static DateTime StatisticDate
        {
            get
            {
                return DateTime.Today.AddDays(-_daysBefore);
            }
        }

        /// <summary>
        /// 一次取的数据行数
        /// </summary>
        public static int MaxRowCount
        {
            get { return _maxRowCount; }
        }
    }
}
