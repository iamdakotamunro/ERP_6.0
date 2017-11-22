using System.Linq;
using Framework.Common;
using Config.Keede.Library;

namespace StorageTask.Core
{
    public class ConfigManager
    {
        public static void Init()
        {
            //读取读取待添加出入库的条数
            ConfigInfo.NeedAddStorageReadQuantity =
                ConfManager.GetAppsetting("NeedAddStorageReadQuantity").ToInt();
        }

        public static bool IsStopTask(TaskKind task)
        {
            //读取停止任务列表
            var stopTasks = ConfManager.GetAppsetting("StopTask").Split('|').ToList();
            return stopTasks.Contains(task.ToString());
        }

        public static int ExcuteMinutes()
        {
            return ConfManager.GetAppsetting("Minutes").ToInt();
        }
    }
}
