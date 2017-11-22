using System.ServiceProcess;

namespace TaskServiceHost
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        static void Main()
        {
            ServiceBase[] servicesToRun =
            { 
                new Service() 
            };
            ServiceBase.Run(servicesToRun);
        }
    }
}
