using System.ServiceProcess;
using CMS.WCF.ExceptionService;

namespace CMS.WCF.ExceptionHost
{
    public partial class Host : ServiceBase
    {
        public Host()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            TaskWork.Start();
        }

        protected override void OnStop()
        {
            TaskWork.Stop();
        }
    }
}
