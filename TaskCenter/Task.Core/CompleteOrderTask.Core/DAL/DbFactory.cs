using Keede.DAL.Helper;

namespace CompleteOrderTask.Core.DAL
{
    public class DbFactory
    {
        public static Database Create()
        {
            return new Database(LogException, ERP.Environment.GlobalConfig.ERP_DB_NAME);
        }

        static void LogException(DbExceptionInfo info)
        {
            ERP.SAL.LogCenter.LogService.LogError(string.Format("脚本运行错误:CommandText={0},ParameterString={1}", info.CommandText, info.ParameterString), "任务中心完成订单", info.Exception);
        }
    }
}
