﻿using System.ComponentModel;
using System.Configuration.Install;

namespace ERP.Service.Host
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();

            serviceInstaller1.DisplayName = ServiceConfig.DisplayName;
            serviceInstaller1.Description = ServiceConfig.Description;
            serviceInstaller1.ServiceName = ServiceConfig.ServiceName;
        }
    }
}
