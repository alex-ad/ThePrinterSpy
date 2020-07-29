using System;
using System.Data.Entity;
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
        public static ConfigDataBase Base { get; set; }

        public struct AdConfig
        {
            public byte Enabled;
            public string Server;
            public string User;
            public string Password;
        }

        public AppConfig()
        {
            _base = new PrintSpyEntities();
            Config data;
            try
            {
                data = _base.Configs.Find(1);
            }
            catch
            {
                data = new Config();
                //throw new Exception("Table `Config` in DataBase `PrinterSpy` is missing");
            }

            //if (data == null) throw new Exception("Table `Config` in DataBase `PrinterSpy` is missing");
            if (data == null) data = new Config();

            ActiveDirectory = new ConfigActiveDirectory(new AdConfig
            {
                Enabled = data.AdEnabled,
                Server = data.AdServer,
                Password = data.AdPassword,
                User = data.AdUser
            });
            PrinterNameMask = new ConfigPrinterNameMask();
            ReportDate = new ConfigReportDate();
            Base = new ConfigDataBase();
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
            var data = _base.Configs.Find(1);

            if (data == null) throw new Exception("Table `Config` in DataBase `PrinterSpy` is missing");

            data.AdEnabled = ActiveDirectory.IsEnabled ? (byte) 1 : (byte) 0;
            data.AdPassword = ActiveDirectory.Password;
            data.AdServer = ActiveDirectory.Server;
            data.AdUser = ActiveDirectory.User;
            _base.Entry(data).State = EntityState.Modified;
            _base.SaveChanges();
        }
    }
}
