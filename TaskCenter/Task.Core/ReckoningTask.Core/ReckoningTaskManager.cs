using System;

namespace ReckoningTask.Core
{
    public class ReckoningTaskManager
    {
        private static readonly ERP.BLL.Implement.ReckoningManager _reckoningManager
            = new ERP.BLL.Implement.ReckoningManager(ERP.Environment.GlobalConfig.DB.FromType.Write);
        public static void AddTask()
        {
            try
            {
                _reckoningManager.RunAsynAddTask(ConfigInfo.ReckoningReadQuantity);
            }
            catch (Exception exp)
            {
                ERP.SAL.LogCenter.LogService.LogError("执行往来帐任务出错", "任务中心往来帐", exp);
            }
        }
    }
}
