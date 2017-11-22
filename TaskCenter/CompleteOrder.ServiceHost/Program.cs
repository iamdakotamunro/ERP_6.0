using System;
using System.ServiceProcess;

namespace CompleteOrderServiceHost
{
    static class Program
    {
        /// <summary>
        /// 订单一次完成程序。
        /// </summary>
        static void Main()
        {
            if (true)
            {
                try
                {
                    ServiceBase[] servicesToRun =
                    {
                        new Server()
                    };
                    ServiceBase.Run(servicesToRun);
                }
                catch (Exception exp)
                {
                    ERP.SAL.LogCenter.LogService.LogError("订单一次完成服务错误", "完成订单请求服务", exp);
                }
            }
        }

        static void Test()
        {
            //FinishTask.RunWaitConsignmentOrderTask();
            //return;
            //new CompleteOrderTask.Core.CompleteFristTask().RunFinish(DateTime.Now, "1BA7621F-1A13-4A53-ABED-5D2DA348DAE3".ToGuid(), "邓杨焱", "B5BCDF6E-95D5-4AEE-9B19-6EE218255C05".ToGuid(), "00000000-0000-0000-0000-000000000000".ToGuid());
            //Console.Write("完成！");
            //Console.ReadKey();

            //var _timer = new System.Timers.Timer(3000);
            //_timer.Elapsed += (sender, e) => FinishTask.RunWaitConsignmentOrderTask();
            //_timer.Start();
            Console.ReadKey();



            //var host = new ServiceHost(typeof(Finish));
            //host.Opened += (obj, e) => Console.WriteLine("服务开启...");
            //host.Open();
            //Console.ReadKey();




            ////初始化timer
            //PUSH.Core.Instance.Init.SetDataBaseConnectionName("db_CMS");
            //System.Timers.Timer timer = new System.Timers.Timer(1000);
            //timer.Elapsed += (sender, e) => ThreadPool.QueueUserWorkItem(obj =>
            //{
            //    while (!Core.FinishTask.IsIdle)
            //    {
            //        return;
            //    }
            //    Core.FinishTask.RunWaitConsignmentOrderTask();
            //});
            //timer.Start();
            //Console.Write("完成！");
            //Console.ReadKey();





            //var t = new[]
            //    {
            //        new[]{"2013.8.23","2013.10.1"}
            //    };
            //foreach (var s in t)
            //{
            //    FixStorageRecord.Fix(s[0], s[1]);
            //}
            //Console.Write("完成！");
            //Console.ReadKey();
        }
    }
}
