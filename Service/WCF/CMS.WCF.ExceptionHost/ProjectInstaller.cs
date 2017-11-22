using System.ComponentModel;
using System.Configuration.Install;

namespace CMS.WCF.ExceptionHost
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
        }
    }
}
