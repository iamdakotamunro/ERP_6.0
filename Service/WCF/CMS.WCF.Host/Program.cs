using Config.Keede.Library;
using ERP.Environment;
using PUSH.Core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
#if !DEBUG
using System.ServiceProcess;
#endif
using System.Windows.Forms;

namespace ERP.Service.Host
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        static void Main()
        {
            ConfManager.Init();

            //设置推送数据的连接字符串名称
            Instance.Init.SetDataBaseConnectionString("System.Data.SqlClient", ConfManager.GetAppsetting("db_ERP_WriteConnection"));

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

#if DEBUG
            ServiceHost sh = new ServiceHost(typeof(Implement.Service));
            sh.Open();
            ServiceHost hostForTest = new ServiceHost(typeof(Implement.TestForJMeter));
            hostForTest.Open();
            var form = new Form();
            form.Text = "ERP服务";
            Application.Run(form);
#else
            var _servicesToRun = new ServiceBase[] { new Service() };
            ServiceBase.Run(_servicesToRun);
#endif
        }
    }
}