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
            //processInstaller.Account = ServiceAccount.User;
            //processInstaller.Username = @"LOT\l0t";
            //processInstaller.Password = "LordWarrior80";
            serviceInstaller.StartType = ServiceStartMode.Automatic;
            serviceInstaller.ServiceName = "ThePrinterSpyService";
            serviceInstaller.Description = "Отслеживание напечатанных на принтере или многофункциональном устройстве документов";
            Installers.Add(processInstaller);
            Installers.Add(serviceInstaller);
        }
    }
}
