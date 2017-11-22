using ERP.Environment;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RebuildPurchaseFormOfSaleFromHostingApp
{
    class Program
    {
        static void Main(string[] args)
        {
            // ERP 读写分离配置
            var readConnectionsOfErp = new List<string>();
            readConnectionsOfErp.Add(ConfigurationManager.AppSettings["db_ERP_ReadConnections_1"]);
            readConnectionsOfErp.Add(ConfigurationManager.AppSettings["db_ERP_ReadConnections_2"]);
            readConnectionsOfErp.Add(ConfigurationManager.AppSettings["db_ERP_ReadConnections_3"]);
            var writeConnectionOfErp = ConfigurationManager.AppSettings["db_ERP_WriteConnection"];
            Keede.DAL.RWSplitting.ConnectionContainer.AddDbConnections(GlobalConfig.ERP_DB_NAME, writeConnectionOfErp, readConnectionsOfErp.Where(m => !string.IsNullOrEmpty(m)).ToArray(), Keede.DAL.RWSplitting.EnumStrategyType.Loop);

            GenerateSaleFilialeStockInAndPurchaseFormTask.Generate();
            Console.WriteLine("按回车键结束");
            Console.ReadLine();
        }
    }
}
