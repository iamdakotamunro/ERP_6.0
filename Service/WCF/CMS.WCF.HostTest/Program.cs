using System;
using System.Collections.Generic;
using System.ServiceModel;
using ERP.DAL.Implement.Inventory;
using ERP.Enum;
using ERP.Environment;
using Config.Keede.Library;
using System.Collections.Generic;
using System.Linq;

namespace ERP.Service.HostTest
{
    class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        static void Main()
        {

            ConfManager.Init();
            // ERP 读写分离配置
            var readConnectionsOfErp = new List<string>();
            readConnectionsOfErp.Add(ConfManager.GetAppsetting("db_ERP_ReadConnections_1"));
            readConnectionsOfErp.Add(ConfManager.GetAppsetting("db_ERP_ReadConnections_2"));
            readConnectionsOfErp.Add(ConfManager.GetAppsetting("db_ERP_ReadConnections_3"));
            var writeConnectionOfErp = ConfManager.GetAppsetting("db_ERP_WriteConnection");
            Keede.DAL.RWSplitting.ConnectionContainer.AddDbConnections(GlobalConfig.ERP_DB_NAME, writeConnectionOfErp, readConnectionsOfErp.Where(m => !string.IsNullOrEmpty(m)).ToArray(), Keede.DAL.RWSplitting.EnumStrategyType.Loop);


            //var sh = new ServiceHost(typeof(Implement.Service));
            //sh.Open();

            var es = new Implement.Service();
            var mm = es.GetFilialeIdGoodsIdAvgSettlePrice(DateTime.Now);

            Console.WriteLine("ERP服务已启动！");
            Console.ReadLine();
        }
    }
}