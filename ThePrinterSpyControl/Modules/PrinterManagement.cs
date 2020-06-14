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
        public static void Rename(SelectedPrinter printer)
        {
            if (printer == null) throw new ArgumentNullException(nameof(printer), "Given object cannot be null");

            ManagementScope scope = new ManagementScope(ManagementPath.DefaultPath);
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

        public static void SetEnabled(SelectedPrinter printer)
        {
            if (printer == null) throw new ArgumentNullException(nameof(printer), "Given object cannot be null");

            using (var db = new PrintSpyEntities())
            {
                var p = db.Printers.FirstOrDefault(x => x.Id == printer.Id);
                if (p == null) return;

                p.Enabled = printer.Enabled;
                db.Entry(p).State = EntityState.Modified;
                db.SaveChanges();
                PrinterSpyViewModel.Users.FirstOrDefault().Printers.FirstOrDefault(x => x.Id == printer.Id).Enabled = p.Enabled;
                PrinterSpyViewModel.TotalStat.PrintersEnabled = PrinterSpyViewModel.Context.Printers.Sum(x => x.Enabled ? 1 : 0);

                var pu = PrinterSpyViewModel.Users.FirstOrDefault().Printers.FirstOrDefault(x => x.Id == printer.Id);
                var user = PrinterSpyViewModel.Users.Where(x => x.Printers.Contains(pu)).ToList()[0];
                
                var pTotal = from pt in PrinterSpyViewModel.Context.Printers
                    where pt.UserId == user.Id
                    select pt;
                var pEnabled = from pe in pTotal
                    where pe.Enabled
                    select pe;
                
                PrinterSpyViewModel.Users.FirstOrDefault(x => x.Id == user.Id).Comment = $"[{pEnabled.Count()}/{pTotal.Count()}]";
            }
        }

        public static void DeleteFromDb(SelectedPrinter printer)
        {
            if (printer.Id < 1) throw new ArgumentException("The Printer Id cannot be less then 1");

            using (var db = new PrintSpyEntities())
            {
                Printer p = db.Printers.FirstOrDefault(x => x.Id == printer.Id);
                if (p == null) return;
                var query = from d in db.PrintDatas
                    where d.PrinterId == printer.Id
                            select d;
                if (query.Any())
                {
                    List<PrintData> data = query.ToList();
                    db.PrintDatas.RemoveRange(data);
                }
                db.Printers.Remove(p);
                db.SaveChanges();

                var del = PrinterSpyViewModel.Users.FirstOrDefault().Printers.FirstOrDefault(x=>x.Id == printer.Id);
                if (del == null) return;
                PrinterSpyViewModel.Users.FirstOrDefault().Printers.Remove(del);
                PrinterSpyViewModel.TotalStat.PrintersAll = PrinterSpyViewModel.Context.Printers.Count();
            }
        }
    }
}
