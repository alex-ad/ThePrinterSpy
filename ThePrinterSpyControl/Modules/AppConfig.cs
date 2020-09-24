using System.Data.Entity;
using System.Linq;
using ThePrinterSpyControl.DataBase;
using ThePrinterSpyControl.Models;
using Props = ThePrinterSpyControl.Properties.Settings;

namespace ThePrinterSpyControl.Modules
{
    public class AppConfig
    {
        private static PrintSpyEntities _base;
        public static ConfigPrinterNameMask PrinterNameMask { get; set; }
        public static ConfigReportDate ReportDate { get; set; }
        public static ConfigActiveDirectory ActiveDirectory { get; set; }

        public struct AdConfig
        {
            public byte Enabled;
            public string Server;
            public string User;
            public string Password;
        }

        public AppConfig()
        {
            PrinterNameMask = new ConfigPrinterNameMask();
            ReportDate = new ConfigReportDate();

            _base = new PrintSpyEntities();
            if (_base == null) return;

            Config data = _base.Configs.Any() ? _base.Configs.First() : new Config();

            ActiveDirectory = new ConfigActiveDirectory(new AdConfig
            {
                Enabled = data.AdEnabled,
                Server = data.AdServer,
                Password = data.AdPassword,
                User = data.AdUser
            });
        }

        public void SaveToLocal()
        {
            Props.Default.PrinterNameMaskEnabled = PrinterNameMask.IsEnabled;
            Props.Default.PrinterNameMaskType = PrinterNameMask.TypeIndex;
            Props.Default.PrinterNameMaskValue = PrinterNameMask.Mask;
            Props.Default.ReportStart = ReportDate.Start;
            Props.Default.ReportEnd = ReportDate.End;
            Props.Default.ReportPeriod = ReportDate.PeriodIndex;
            Props.Default.ReportPeriodEnabled = ReportDate.IsEnabled;
            Props.Default.Save();
        }

        public void SaveToBase()
        {
            Config data = _base.Configs.Any() ? _base.Configs.First() : new Config();
            data.AdEnabled = ActiveDirectory.IsEnabled ? (byte)1 : (byte)0;
            data.AdPassword = ActiveDirectory.Password ?? string.Empty;
            data.AdServer = ActiveDirectory.Server ?? string.Empty;
            data.AdUser = ActiveDirectory.User ?? string.Empty;
           _base.Entry(data).State = _base.Configs.Any() ? EntityState.Modified : EntityState.Added;
            _base.SaveChanges();
        }
    }
}
