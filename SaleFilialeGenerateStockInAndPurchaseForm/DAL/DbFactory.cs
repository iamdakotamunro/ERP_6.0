using ERP.Environment;
using Keede.DAL.Helper;

namespace SaleFilialeGenerateStockInAndPurchaseForm.DAL
{
    public class DbFactory
    {
        public static Database Create()
        {
            return new Database(LogException, GlobalConfig.ERP_DB_NAME);
        }

        static void LogException(DbExceptionInfo info)
        {
            ERP.SAL.LogCenter.LogService.LogError(string.Format("脚本运行错误:CommandText={0},ParameterString={1}", info.CommandText, info.ParameterString), "任务中心销售公司每天凌晨生成采购单及入库单（来自B2C的订单）", info.Exception);
        }
    }
}
