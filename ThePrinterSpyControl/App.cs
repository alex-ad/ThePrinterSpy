using System.Windows;

namespace ThePrinterSpyControl
{
    public partial class App : Application
    {
        private App()
        {
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(ThePrinterSpyControl.Properties.Settings.Default.Language);
        }
    }
}
