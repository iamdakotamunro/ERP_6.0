using System;
using Framework.Core.Extension;
using Framework.Core.Utility;
using Config.Keede.Library;

namespace AutoPurchasing.Core
{
    public class Config
    {
        public static string AutoPurchasingTime
        {
            get { return ConfManager.GetAppsetting("AutoPurchasingTime"); }
        }
    }
}
