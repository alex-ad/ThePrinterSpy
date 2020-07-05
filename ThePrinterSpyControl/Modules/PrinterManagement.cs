using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Management;
using System.Threading.Tasks;
using ThePrinterSpyControl.Models;
using ThePrinterSpyControl.ViewModels;

namespace ThePrinterSpyControl.Modules
{
    static class PrinterManagement
    {
        //public static ListUserPrinter UsersPrinters { get; }
        private static PrintersCollection Printers { get; set; }

        static PrinterManagement()
        {
            Printers = new PrintersCollection();
           //UsersPrinters = new ListUserPrinter();
        }

        public static async void Rename(SelectedPrinter printer, string computerName)
        {
            if (printer == null) throw new ArgumentNullException(nameof(printer), "Given object cannot be null");

            ManagementScope scope = new ManagementScope($@"\\{computerName}\root\cimv2");
            scope.Connect();

            SelectQuery selectQuery = new SelectQuery();
            selectQuery.QueryString = @"SELECT * FROM Win32_Printer WHERE Name = '" + printer.OldName.Replace("\\", "\\\\") + "'";

            ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, selectQuery);
            ManagementObjectCollection items = searcher.Get();

            if (items.Count == 0) return;

            foreach (ManagementObject item in items)
            {
                await Task.Run(() =>
                {
                    item.InvokeMethod("RenamePrinter", new object[] { printer.NewName });
                    //UsersPrinters.SetPrinterName(printer.Id, printer.NewName);
                });
                
                return;
            }
        }

        public static async void SetEnabled(SelectedPrinter printer)
        {
            if (printer == null || printer.Id < 1) throw new ArgumentException(nameof(printer), "The Printer is undefined");
            await Printers.SetPrinterEnabled(printer.Id, printer.Enabled);
            //await UsersPrinters.SetPrinterEnabled(printer.Id, printer.Enabled);
        }

        public static async void DeleteFromDb(SelectedPrinter printer)
        {
            if (printer == null || printer.Id < 1) throw new ArgumentException(nameof(printer), "The Printer is undefined");
            //await UsersPrinters.RemovePrinter(printer.Id);
        }
    }
}
