using System;
using System.ServiceProcess;

namespace CMS.WCF.ExceptionHost
{
    static class Program
    {
        private static string LogExceptionPath;

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        static void Main()
        {

            try
            {
                LogExceptionPath = Framework.Common.Configuration.AppSettings["LogExceptionPath"];
                ServiceBase[] ServicesToRun = new ServiceBase[] 
                                              { 
                                                  new Host() 
                                              };
                ServiceBase.Run(ServicesToRun);
            }
            catch (Exception exp)
            {
                if (!string.IsNullOrEmpty(LogExceptionPath))
                {
                    Framework.Common.LogHelper.Log("CMS_Exception_Service", LogExceptionPath, "异常服务报错", exp);
                }
            }
        }
    }
}
