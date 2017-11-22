using System;
using System.Text;
using System.Web;
using ERP.Environment;
using ERP.UI.Web.Common;
using PUSH.Core;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using Config.Keede.Library;

namespace ERP.UI.Web
{
    /// <summary>
    /// </summary>
    public class Global : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            ConfManager.Init();

            //设置推送数据的连接字符串名称
            Instance.Init.SetDataBaseConnectionString("System.Data.SqlClient", ConfManager.GetAppsetting("db_ERP_WriteConnection"));

            //设置操作日志的异步本地数据库连接
            OperationLog.Core.Init.InitSyncConnectionString(ConfManager.GetAppsetting("db_ERP_WriteConnection"));

            // ERP 读写分离配置
            var readConnectionsOfErp = new List<string>();
            readConnectionsOfErp.Add(ConfManager.GetAppsetting("db_ERP_ReadConnections_1"));
            readConnectionsOfErp.Add(ConfManager.GetAppsetting("db_ERP_ReadConnections_2"));
            readConnectionsOfErp.Add(ConfManager.GetAppsetting("db_ERP_ReadConnections_3"));
            var writeConnectionOfErp = ConfManager.GetAppsetting("db_ERP_WriteConnection");
            Keede.DAL.RWSplitting.ConnectionContainer.AddDbConnections(GlobalConfig.ERP_DB_NAME, writeConnectionOfErp, readConnectionsOfErp.Where(m => !string.IsNullOrEmpty(m)).ToArray(), Keede.DAL.RWSplitting.EnumStrategyType.Loop);

            // ERP 备份库读写分离配置
            var readConnectionsOfErpHistory = new List<string>();
            readConnectionsOfErpHistory.Add(ConfManager.GetAppsetting("db_ERP_HISTORY_ReadConnections_1"));
            readConnectionsOfErpHistory.Add(ConfManager.GetAppsetting("db_ERP_HISTORY_ReadConnections_2"));
            readConnectionsOfErpHistory.Add(ConfManager.GetAppsetting("db_ERP_HISTORY_ReadConnections_3"));
            var writeConnectionOfErpHistory = ConfManager.GetAppsetting("db_ERP_HISTORY_WriteConnection");            
            int maxYear = DateTime.Now.Year + 2;// 历史备份库年份，假定2年以后才会重启Web（事实可能吗？所以完全够了）
            for (var i = 2007; i <= maxYear; i++)
            {
                var readConnectionStringList = new List<string>();
                for (var j = 0; j < readConnectionsOfErpHistory.Count; j++)
                {
                    var readConnectionString = readConnectionsOfErpHistory[j];
                    if (string.IsNullOrEmpty(readConnectionString)) continue;

                    var connectionlist = readConnectionString.Split(';').ToList();
                    var oldReadDbName = connectionlist.FirstOrDefault(c => c.IndexOf("database=", StringComparison.CurrentCultureIgnoreCase) > -1);
                    var newReadDbName = oldReadDbName + i;
                    readConnectionString = readConnectionString.Replace(oldReadDbName, newReadDbName);
                    readConnectionStringList.Add(readConnectionString);
                }

                var writeConnectionString = writeConnectionOfErpHistory;
                var oldWriteDbName = writeConnectionString.Split(';').ToList().FirstOrDefault(c => c.IndexOf("database=", StringComparison.CurrentCultureIgnoreCase) > -1);
                var newWriteDbName = oldWriteDbName + i;
                writeConnectionString = writeConnectionString.Replace(oldWriteDbName, newWriteDbName);
                Keede.DAL.RWSplitting.ConnectionContainer.AddDbConnections(GlobalConfig.ERP_HISTORY_DB_NAME + i, writeConnectionString, readConnectionStringList.Where(m => !string.IsNullOrEmpty(m)).ToArray(), Keede.DAL.RWSplitting.EnumStrategyType.Loop);
            }

            // ERP Report 读写分离配置
            var readConnectionsOfErpReport = new List<string>();
            readConnectionsOfErpReport.Add(ConfManager.GetAppsetting("db_ERP_REPORT_ReadConnections_1"));
            readConnectionsOfErpReport.Add(ConfManager.GetAppsetting("db_ERP_REPORT_ReadConnections_2"));
            readConnectionsOfErpReport.Add(ConfManager.GetAppsetting("db_ERP_REPORT_ReadConnections_3"));
            var writeConnectionOfErpReport = ConfManager.GetAppsetting("db_ERP_REPORT_WriteConnection");
            Keede.DAL.RWSplitting.ConnectionContainer.AddDbConnections(GlobalConfig.ERP_REPORT_DB_NAME, writeConnectionOfErpReport, readConnectionsOfErpReport.Where(m => !string.IsNullOrEmpty(m)).ToArray(), Keede.DAL.RWSplitting.EnumStrategyType.Loop);

            Dapper.Extension.TypeMapper.Initialize(typeof(Keede.Ecsoft.Model.GoodsOrderInfo).Assembly);

            SAL.LogCenter.LogService.LogInfo("网站启动", "应用程序异常信息", null);
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {
            //在出现未处理的错误时运行的代码 
            Exception ex = Server.GetLastError().GetBaseException();
            string realName = string.Empty;
            try
            {
                if (HttpContext.Current.Session != null)
                {
                    var per = CurrentSession.Personnel.Get();
                    realName = per.RealName;
                }
            }
            catch (Exception)
            {
                realName = "未知";
            }


            StringBuilder str = new StringBuilder();
            str.Append("\r\n\t浏览器:" + Request.Browser.Browser);
            str.Append("\r\n\t浏览器版本:" + Request.Browser.MajorVersion);
            str.Append("\r\n\t操作系统:" + Request.Browser.Platform);
            str.Append("\r\n\t操作人:" + realName);
            str.Append("\r\n\t页面：" + Request.Url);
            str.Append("\r\n\t错误信息：" + ex.Message);
            str.Append("\r\n\t错误源：" + ex.Source);
            str.Append("\r\n\t异常方法：" + ex.TargetSite);
            str.Append("\r\n\t堆栈信息：" + ex.StackTrace);
            SAL.LogCenter.LogService.LogError(str.ToString(), "应用程序异常信息", ex);
        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}