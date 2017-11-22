using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ERP.Environment;

namespace ERP.DAL
{
    #region -- 数据库访问器工厂
    /// <summary>
    /// 数据库访问器工厂
    /// </summary>
    internal class DatabaseFactory
    {
        public static Keede.DAL.Helper.Database Create()
        {
            return new Keede.DAL.Helper.Database(LogException,GlobalConfig.ERP_DB_NAME);
        }

        public static Keede.DAL.Helper.Database CreateRdb()
        {
            return new Keede.DAL.Helper.Database(LogException, GlobalConfig.ERP_REPORT_DB_NAME);
        }

        public static Keede.DAL.Helper.Database Create(int startyear)
        {
            return new Keede.DAL.Helper.Database(LogException,GlobalConfig.GetErpDbName(startyear));
        }

        static void LogException(Keede.DAL.Helper.DbExceptionInfo info)
        {
            SAL.LogCenter.LogService.LogError(string.Format("脚本运行错误,CommandText={0},ParameterString={1}", info.CommandText, info.ParameterString), "数据工厂", info.Exception);
        }
    }
    #endregion
}
