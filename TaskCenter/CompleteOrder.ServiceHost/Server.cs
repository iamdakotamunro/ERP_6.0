using System;
using System.ServiceProcess;

namespace CompleteOrderServiceHost
{
    partial class Server : ServiceBase
    {
        public Server()
        {
            InitializeComponent();
            ServiceName = ServiceConfig.ServiceName;
        }

        private System.ServiceModel.ServiceHost _host;

        protected override void OnStart(string[] args)
        {
            if (_host == null)
                _host = new System.ServiceModel.ServiceHost(typeof(Finish));

            try
            {
                _host.Open();
            }
            catch (Exception exp)
            {
                ERP.SAL.LogCenter.LogService.LogError(exp.Message, "完成订单请求服务", exp);
            }
        }

        protected override void OnStop()
        {
            _host.Close();
        }
    }
}
