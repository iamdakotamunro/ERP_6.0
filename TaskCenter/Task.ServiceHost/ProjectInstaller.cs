using System.ComponentModel;

namespace TaskServiceHost
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
            serviceInstaller1.ServiceName = ServiceConfig.ServiceName;
            serviceInstaller1.DisplayName = ServiceConfig.DisplayName;
            serviceInstaller1.Description = ServiceConfig.Description;
        }
    }
}
