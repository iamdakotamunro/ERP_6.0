using ERP.Environment;
using Keede.DAL.Helper;

namespace StorageTask.Core.DAL
{
    internal class DaoFactory
    {
        /// <summary>
        /// 创建ERP数据库访问对象
        /// </summary>
        /// <returns></returns>
        public static Database CreateERPDatabase()
        {
            return new Database(LogExcetion, GlobalConfig.ERP_DB_NAME);
        }

        static void LogExcetion(DbExceptionInfo info)
        {
            ERP.SAL.LogCenter.LogService.LogError(string.Format("脚本运行错误:CommandText={0},ParameterString={1}", info.CommandText, info.ParameterString), "任务中心出入库", info.Exception);
        }
    }
}
