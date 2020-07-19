using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Management;
using System.Threading.Tasks;
using ThePrinterSpyControl.ModelBuilders;
using ThePrinterSpyControl.Models;
using ThePrinterSpyControl.ViewModels;

namespace ThePrinterSpyControl.Modules
{
    static class PrinterManagement
    {
        private static PrintersCollection Printers { get; set; }

        static PrinterManagement()
        {
            Printers = new PrintersCollection();
        }

        public static void Rename(SelectedPrinter printer, string computerName)
        {
            if (printer == null || printer.Id < 1) throw new ArgumentException("The Printer is undefined", nameof(printer));
            if (printer.NewName.Equals(printer.OldName, StringComparison.InvariantCulture)) return;
            if (string.IsNullOrEmpty(printer.NewName) || printer.NewName.Length < 3) return;

            ManagementScope scope = new ManagementScope($@"\\{computerName}\root\cimv2");
            scope.Connect();

            SelectQuery selectQuery = new SelectQuery();
            selectQuery.QueryString = @"SELECT * FROM Win32_Printer WHERE Name = '" + printer.OldName.Replace("\\", "\\\\") + "'";

            ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, selectQuery);
            ManagementObjectCollection items = searcher.Get();

            if (items.Count == 0) return;

            foreach (ManagementObject item in items)
            {
                item.InvokeMethod("RenamePrinter", new object[] { printer.NewName });
                Printers.SetPrinterName(printer.Id, printer.NewName);
            }
        }

        public static void SetEnabled(SelectedPrinter printer)
        {
            if (printer == null || printer.Id < 1) throw new ArgumentException("The Printer is undefined", nameof(printer));
            Printers.SetPrinterEnabled(printer.Id, printer.Enabled);
        }

        public static void DeleteFromDb(SelectedPrinter printer)
        {
            if (printer == null || printer.Id < 1) throw new ArgumentException("The Printer is undefined", nameof(printer));
            Printers.Remove(printer.Id);
        }
    }
}
