using System.Globalization;
using System.Windows;

namespace ThePrinterSpyControl
{
    public partial class App : Application
    {
        private App()
        {
            try
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture =
                    new CultureInfo(ThePrinterSpyControl.Properties.Settings.Default.Language);
            }
            catch
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = CultureInfo.DefaultThreadCurrentUICulture;
            }
        }
    }
}
