using System;
using System.Collections.Generic;
using System.ServiceProcess;
using System.Threading.Tasks;
using System.Timers;
using GoodsDaySalesStatisticsAsynTask;
using ReckoningTask.Core;
using SaleFilialeGenerateStockInAndPurchaseForm;
using StorageTask.Core;
using ConfigManager = StorageTask.Core.ConfigManager;
using GenerateSaleOrderGoodsSettlement;
using ArchiveLastMonthGoodsSettlement;
using System.Configuration;
using ERP.Environment;
using System.Linq;
using PUSH.Core;
using Config.Keede.Library;

namespace TaskServiceHost
{
    public partial class Service : ServiceBase
    {
        public Service()
        {
            InitializeComponent();
            ServiceName = ServiceConfig.ServiceName;
        }

        //任务列表（字典集合 key:任务枚举,value:间隔时间）
        private readonly IDictionary<TaskKind, Timer> _timerList = new Dictionary<TaskKind, Timer>();

        /// <summary>初始化任务
        /// </summary>
        private void Init()
        {
            ConfManager.Init();

            //设置推送数据的连接字符串名称
            Instance.Init.SetDataBaseConnectionString("System.Data.SqlClient", ConfManager.GetAppsetting("db_ERP_WriteConnection"));

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

            //初始化AOP
            ConfigManager.Init();
            //全局异常记录
            TaskScheduler.UnobservedTaskException += (sender, exceptionArgs) =>
                                                        {
                                                            Exception ex = exceptionArgs.Exception.InnerException ?? exceptionArgs.Exception;
                                                            if (ex.InnerException != null)
                                                                ex = ex.InnerException;
                                                            LogAggregateException(ex, sender);
                                                            exceptionArgs.SetObserved();
                                                        };

            //记录添加出库任务器
            AddTask(TaskKind.AddSellOut, AddStorageTaskManager.AddSellOutTask, 60);

            //往来帐生成任务
            AddTask(TaskKind.BuildReckoning, ReckoningTaskManager.AddTask, 60);

            //完成订单第二步骤任务
            AddTask(TaskKind.CompleteOrderWithSecond, CompleteOrderTask.Core.CompleteSecondTask.RunWaitConsignmentOrderTask, 60);

            //自动报备(采购)
            AddTask(TaskKind.AutoPurchasing, AutoPurchasing.Core.RunAutoPurchasingTask.RunAutoPurchasing, 60);

            //进货申报
            AddTask(TaskKind.AutoStockDeclare, AutoStockDeclare.Core.StockDeclareTask.RunStockDeclare, 60);

            //商品结算价、库存及供应商销售额存档
            AddTask(TaskKind.GoodsStockRecordAndSales, GoodsStockRecordTask.Core.GoodsStockRecordTaskManager.RunGoodsStockRecordTask, 60 * ConfigManager.ExcuteMinutes());

            //每月商品毛利数据存档
            AddTask(TaskKind.GoodsGrossProfit, GoodsStockRecordTask.Core.GoodsGrossProfitManager.RunGoodsGrossProfitTask, 1800);

            //每月公司毛利数据存档
            AddTask(TaskKind.CompanyGrossProfit, GoodsStockRecordTask.Core.CompanyGrossProfitManager.RunCompanyGrossProfitTask, 1800);

            //供应商资质检索
            AddTask(TaskKind.QualificationManager, GoodsStockRecordTask.Core.QualificationTaskManager.RunQualificationTask, 43200);

            //销量异步任务
            AddTask(TaskKind.GoodsDaySalesStatisticsAsynTask, GoodsDaySalesStatisticsAsynTaskManager.RecordGoodsDaySalesStatistics, 60);

            //销售公司每天凌晨生成采购单及入库单任务（来自B2C的订单）
            SaleFilialeGenerateStockInAndPurchaseFormTaskConfig.Init();
            AddTask(TaskKind.GenerateSaleFilialeStockInAndPurchaseFormTask, GenerateSaleFilialeStockInAndPurchaseFormTask.Generate, 3600);

            //销售订单关联的即时结算价
            GenerateSaleOrderGoodsSettlementTaskConfig.Init();
            AddTask(TaskKind.GenerateSaleOrderGoodsSettlement, GenerateSaleOrderGoodsSettlementTask.Generate, 3600);

            //商品即时结算价按月归档
            ArchiveLastMonthGoodsSettlementTaskConfig.Init();
            AddTask(TaskKind.ArchiveLastMonthGoodsSettlement, ArchiveLastMonthGoodsSettlementTask.Generate, 3600);
        }

        /// <summary>
        /// 添加任务
        /// </summary>
        /// <param name="taskKind">任务类型</param>
        /// <param name="task">任务</param>
        /// <param name="seconds">间隔时间（秒）</param>
        private void AddTask(TaskKind taskKind, Action task, int seconds)
        {
            var timer = new Timer(seconds * 1000);
            timer.Elapsed += (sender, e) =>
            {
                var t = (Timer)sender;
                t.Stop();
                Task.Factory.StartNew(task).ContinueWith(ent => t.Start());
            };
            _timerList.Add(taskKind, timer);
        }

        /// <summary>开始任务
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {
            try
            {
                Init();
                foreach (var timer in _timerList)
                {

                    if (!ConfigManager.IsStopTask(timer.Key))
                    {
                        timer.Value.Start();
                    }
                }
                ERP.SAL.LogCenter.LogService.LogInfo("服务启动", "任务中心任务主服务", null);
            }
            catch (Exception exp)
            {
                ERP.SAL.LogCenter.LogService.LogError(string.Format(exp.Message + " args:{0}", string.Join(",", args)), "任务中心任务主服务", exp);
            }
        }

        /// <summary>停止定时器
        /// </summary>
        protected override void OnStop()
        {
            foreach (var timer in _timerList)
            {
                timer.Value.Stop();
                timer.Value.Dispose();
            }
            ERP.SAL.LogCenter.LogService.LogInfo("服务停止", "任务中心任务主服务", null);
        }

        /// <summary>处理并行任务中的未知异常
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="sender"></param>
        private static void LogAggregateException(Exception ex, object sender)
        {
            string msg = ex == null
                             ? "Task出现未观察到的异常，且不包含异常信息"
                             : string.Format("Task出现未观察到的异常，Message:{0},StackTrace:{1}", ex.Message, ex.StackTrace);
            ERP.SAL.LogCenter.LogService.LogError(msg, "任务中心任务主服务", ex);
        }
    }
}
