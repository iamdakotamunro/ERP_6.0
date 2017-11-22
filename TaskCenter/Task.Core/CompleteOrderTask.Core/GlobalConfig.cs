using Config.Keede.Library;
using Framework.Core.Extension;
using Framework.Core.Utility;

namespace CompleteOrderTask.Core
{
    internal class GlobalConfig
    {
        static string GetAndVerifySetting(string settingName)
        {
            var val = ConfManager.GetAppsetting(settingName);
            if (string.IsNullOrEmpty(val))
            {
                throw new System.ArgumentNullException(settingName,settingName + " => AppSetting Need config!");
            }
            return val;
        }

        public static int ReadWaitConsignmentOrder
        {
            get
            {
                var val = GetAndVerifySetting("ReadWaitConsignmentOrder");
                return val.ToInt();
            }
        }

        public static string[] SecondConsignmentOrderHour
        {
            get
            {
                var val = GetAndVerifySetting("SecondConsignmentOrderHour");
                return val.Split(',');
            }
        }
    }
}
