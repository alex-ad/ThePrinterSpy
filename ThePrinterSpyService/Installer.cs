using System.ComponentModel;
using System.ServiceProcess;

namespace ThePrinterSpyService
{
    [RunInstaller(true)]
    public partial class Installer : System.Configuration.Install.Installer
    {
        public Installer()
        {
            InitializeComponent();

            ServiceInstaller serviceInstaller = new ServiceInstaller();
            ServiceProcessInstaller processInstaller = new ServiceProcessInstaller();

            processInstaller.Account = ServiceAccount.LocalSystem;
            serviceInstaller.StartType = ServiceStartMode.Automatic;
            serviceInstaller.ServiceName = "ThePrinterSpyService";
            serviceInstaller.Description = "Monitoring on printed documents on local machine";
            Installers.Add(processInstaller);
            Installers.Add(serviceInstaller);
        }
    }
}
