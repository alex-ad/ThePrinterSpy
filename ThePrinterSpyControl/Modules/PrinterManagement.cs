using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Management;
using System.Windows.Documents;
using ThePrinterSpyControl.Models;
using ThePrinterSpyControl.ViewModels;

namespace ThePrinterSpyControl.Modules
{
    static class PrinterManagement
    {
        public static void Rename(PrinterChangeNames printerChangeNames)
        {
            if (printerChangeNames == null) throw new ArgumentNullException(nameof(printerChangeNames), "Given object cannot be null");

            ManagementScope scope = new ManagementScope(ManagementPath.DefaultPath);
            scope.Connect();

            SelectQuery selectQuery = new SelectQuery();
            selectQuery.QueryString = @"SELECT * FROM Win32_Printer WHERE Name = '" + printerChangeNames.PrinterOldName.Replace("\\", "\\\\") + "'";

            ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, selectQuery);
            ManagementObjectCollection items = searcher.Get();

            if (items.Count == 0) return;

            foreach (ManagementObject item in items)
            {
                item.InvokeMethod("RenamePrinter", new object[] { printerChangeNames.PrinterNewName });
                return;
            }
        }

        public static void SetEnabled(PrinterChangeEnabled printerChangeEnabled)
        {
            if (printerChangeEnabled == null) throw new ArgumentNullException(nameof(printerChangeEnabled), "Given object cannot be null");

            using (var db = new PrintSpyEntities())
            {
                var p = db.Printers.FirstOrDefault(x => x.Id == printerChangeEnabled.Id);
                if (p == null) return;
                p.Enabled = printerChangeEnabled.Enabled;
                db.Entry(p).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        public static void DeleteFromDb(int id)
        {
            if (id < 1) throw new ArgumentException("The Printer Id cannot be less then 1");

            using (var db = new PrintSpyEntities())
            {
                Printer p = db.Printers.FirstOrDefault(x => x.Id == id);
                if (p == null) return;
                var query = from d in db.PrintDatas
                    where d.PrinterId == id
                    select d;
                if (query.Any())
                {
                    List<PrintData> data = query.ToList();
                    db.PrintDatas.RemoveRange(data);
                }
                db.Printers.Remove(p);
                db.SaveChanges();
            }
        }
    }
}
