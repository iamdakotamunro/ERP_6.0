using Config.Keede.Library;
using Framework.Core.Extension;
using Framework.Core.Utility;

namespace ReckoningTask.Core
{
    internal static class ConfigInfo
    {
        public static string ERPConnectionName { get; internal set; }
        public static int ReckoningReadQuantity
        {
            get
            {
                var readCount = ConfManager.GetAppsetting("ReckoningReadQuantity").ToInt();
                if (readCount<=0)
                {
                    return 100;
                }
                return readCount;
            }
        }
    }
}
