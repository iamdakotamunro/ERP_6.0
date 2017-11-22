using System;
using System.ServiceModel;
using System.ServiceProcess;

namespace ERP.Service.Host
{
    partial class Service : ServiceBase
    {
        public Service()
        {
            InitializeComponent();
            ServiceName = ServiceConfig.ServiceName;
        }

        private ServiceHost _host;

        protected override void OnStart(string[] args)
        {
            if (_host == null)
                _host = new ServiceHost(typeof(Implement.Service));
            try
            {
                _host.Open();
                ERP.SAL.LogCenter.LogService.LogInfo("服务启动", "ERP服务", null);

                //加载商品责任人绑定本地缓存
                Cache.PurchseSet.Instance.Load();
            }
            catch (Exception exp)
            {
                ERP.SAL.LogCenter.LogService.LogError("服务启动", "ERP服务", exp);
            }
        }

        protected override void OnStop()
        {
            try
            {
                _host.Close();
                ERP.SAL.LogCenter.LogService.LogInfo("服务停止", "ERP服务", null);
            }
            catch (Exception exp)
            {
                ERP.SAL.LogCenter.LogService.LogError("服务停止", "ERP服务", exp);
            }
        }
    }
}
