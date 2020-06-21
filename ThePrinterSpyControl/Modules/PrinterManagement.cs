using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Management;
using ThePrinterSpyControl.Models;
using ThePrinterSpyControl.ViewModels;

namespace ThePrinterSpyControl.Modules
{
    static class PrinterManagement
    {
        private static readonly DBase Base;

        static PrinterManagement()
        {
            Base = new DBase();
        }

        public static void Rename(SelectedPrinter printer, string computerName)
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
                item.InvokeMethod("RenamePrinter", new object[] { printer.NewName });
                PrinterSpyViewModel.Users.FirstOrDefault().Printers.FirstOrDefault(x => x.Id == printer.Id).Name = printer.NewName;
                return;
            }
        }

        public static async void SetEnabled(SelectedPrinter printer)
        {
            if (printer == null) throw new ArgumentNullException(nameof(printer), "Given object cannot be null");

            var p = await Base.GetPrinterById(printer.Id);
            if (p == null) return;

            p.Enabled = printer.Enabled;
            await Base.SetPrinterEnabled(p);
            PrinterSpyViewModel.Users.FirstOrDefault().Printers.FirstOrDefault(x => x.Id == printer.Id).Enabled = p.Enabled;
            PrinterSpyViewModel.TotalStat.PrintersEnabled = await Base.GetEnabledPrintersCount();

            var pu = PrinterSpyViewModel.Users.FirstOrDefault().Printers.FirstOrDefault(x => x.Id == printer.Id);
            var user = PrinterSpyViewModel.Users.Where(x => x.Printers.Contains(pu)).ToList()[0];

            var pTotal = await Base.GetPrintersByUserId(user.Id);
            var pEnabled = from pe in pTotal
                where pe.Enabled
                select pe;
            
            PrinterSpyViewModel.Users.FirstOrDefault(x => x.Id == user.Id).Comment = $"[{pEnabled.Count()}/{pTotal.Count()}]";
        }

        public static async void DeleteFromDb(SelectedPrinter printer)
        {
            if (printer.Id < 1) throw new ArgumentException("The Printer Id cannot be less then 1");

            var p = await Base.GetPrinterById(printer.Id);
            if (p == null) return;

            await Base.RemovePrintDataByPrinter(p);

            var del = PrinterSpyViewModel.Users.FirstOrDefault().Printers.FirstOrDefault(x=>x.Id == printer.Id);
            if (del == null) return;
            PrinterSpyViewModel.Users.FirstOrDefault().Printers.Remove(del);
            PrinterSpyViewModel.TotalStat.PrintersAll = await Base.GetPrintersCount();
        }
    }
}
