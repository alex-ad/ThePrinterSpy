using System;
using System.Management;
using System.Threading.Tasks;
using System.Windows;
using ThePrinterSpyControl.ModelBuilders;
using ThePrinterSpyControl.Models;

namespace ThePrinterSpyControl.Modules
{
    static class PrinterManagement
    {
        private static PrintersCollection Printers { get; set; }

        static PrinterManagement()
        {
            Printers = new PrintersCollection();
        }

        public static async Task Rename(SelectedPrinter printer, string computerName)
        {
            if (printer == null || printer.Id < 1) throw new ArgumentException("The Printer is undefined", nameof(printer));
            if (string.Compare(printer.NewName, printer.OldName, StringComparison.OrdinalIgnoreCase) == 0) return;
            if (string.IsNullOrEmpty(printer.NewName) || printer.NewName.Length < 3) return;

            await Task.Run((() =>
            {
                ManagementScope scope;
                try
                {
                    scope = new ManagementScope($@"\\{computerName}\root\cimv2");
                    scope.Connect();
                }
                catch
                {
                    return;
                }
                
                SelectQuery selectQuery = new SelectQuery();
                selectQuery.QueryString = @"SELECT * FROM Win32_Printer WHERE Name = '" + printer.OldName.Replace("\\", "\\\\") + "'";

                ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, selectQuery);
                ManagementObjectCollection items = searcher.Get();

                if (items.Count == 0) return;

                foreach (ManagementObject item in items)
                {
                    if (Convert.ToInt32(item.InvokeMethod("RenamePrinter", new object[] { printer.NewName })) == 0)
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            Printers.SetPrinterName(printer.Id, printer.NewName);
                        });
                }
            }));
        }

        public static void SetEnabled(SelectedPrinter printer)
        {
            if (printer == null || printer.Id < 1) throw new ArgumentException("The Printer is undefined", nameof(printer));
            Printers.SetPrinterEnabled(printer.Id, printer.Enabled);
        }

        public static async Task DeleteFromDb(SelectedPrinter printer)
        {
            if (printer == null || printer.Id < 1) throw new ArgumentException("The Printer is undefined", nameof(printer));
            await Printers.Remove(printer.Id);
        }
    }
}
