using ERP.Environment;
using Keede.DAL.Helper;

namespace AutoPurchasing.Core
{

    public class DatabaseFactory
    {

        //public static string ConnectionString
        //{
        //    get { return Framework.Common.Configuration.ConnectionStrings[ConnectionName].ConnectionString; }
        //}

        public static Database Create()
        {
            return new Database(LogExcetion, GlobalConfig.ERP_DB_NAME);
        }

        static void LogExcetion(DbExceptionInfo info)
        {
            ERP.SAL.LogCenter.LogService.LogError(string.Format("脚本运行错误:CommandText={0},ParameterString={1}", info.CommandText, info.ParameterString), "自动报备(采购)", info.Exception);
        }
    }
}
