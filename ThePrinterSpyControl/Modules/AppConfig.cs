using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using ThePrinterSpyControl.Models;
using ThePrinterSpyControl.ViewModels;
using Props = ThePrinterSpyControl.Properties.Settings;

namespace ThePrinterSpyControl.Modules
{
    public class AppConfig
    {
        private static PrintSpyEntities _base;
        public static PrinterNameMaskConfig PrinterNameMask { get; set; }
        public static ReportDateConfig ReportDate { get; set; }
        public static ActiveDirectoryConfig ActiveDirectory { get; set; }
        public static DbaseConfig Dbase { get; set; }

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
            var data = _base.Configs.Find(1);
            ActiveDirectory = new ActiveDirectoryConfig(new AdConfig
            {
                Enabled = data.AdEnabled,
                Server = data.AdServer,
                Password = data.AdPassword,
                User = data.AdUser
            });
            PrinterNameMask = new PrinterNameMaskConfig();
            ReportDate = new ReportDateConfig();
            Dbase = new DbaseConfig();
        }

        public string GetMaskedPrinterName(string mask)
        {
            try
            {
                if (PrinterNameMask.Type == PrinterNameMaskConfig.MaskType.BeginName)
                    return mask.Substring(0, Convert.ToInt32(PrinterNameMask.Mask));
                if (PrinterNameMask.Type == PrinterNameMaskConfig.MaskType.EndName)
                    return mask.Remove(0, mask.Length - Convert.ToInt32(PrinterNameMask.Mask));
                if (PrinterNameMask.Type == PrinterNameMaskConfig.MaskType.WholeName)
                    return mask;
                if (PrinterNameMask.Type == PrinterNameMaskConfig.MaskType.ContainsName)
                {
                    int start = mask.IndexOf(PrinterNameMask.Mask, StringComparison.InvariantCultureIgnoreCase);
                    if (start < 0) return mask;
                    return mask.Substring(start, PrinterNameMask.Mask.Length);
                }
                Regex regex = new Regex(PrinterNameMask.Mask);
                MatchCollection matches = regex.Matches(mask);
                return (matches.Count < 1) ? mask : matches[0].Value;
            }
            catch
            {
                return mask;
            }
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

        public void SaveToDbase()
        {
            var data = _base.Configs.Find(1);
            data.AdEnabled = ActiveDirectory.IsEnabled ? (byte) 1 : (byte) 0;
            data.AdPassword = ActiveDirectory.Password;
            data.AdServer = ActiveDirectory.Server;
            data.AdUser = ActiveDirectory.User;
            _base.Entry(data).State = EntityState.Modified;
            _base.SaveChanges();
        }
    }
}
