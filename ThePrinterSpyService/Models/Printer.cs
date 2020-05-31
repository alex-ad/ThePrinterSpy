using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Management;
using ThePrinterSpyService.Core;

namespace ThePrinterSpyService.Models
{
    public struct PrinterStruct
    {
        public string Name { get; set; }
        public int UserId { get; set; }
        public int ComputerId { get; set; }
        public int ServerId { get; set; }
        public bool Enabled { get; set; }
    }

    public class Printer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int UserId { get; set; }
        public int ComputerId { get; set; }
        public int ServerId { get; set; }
        public bool Enabled { get; set; }

        public static Printer Add(PrinterStruct printer)
        {
            Printer p = Get(printer);
            if (p != null) return p;
            p = new Printer
            {
                Name = printer.Name,
                UserId = printer.UserId,
                ComputerId = printer.ComputerId,
                ServerId = printer.ServerId,
                Enabled = printer.Enabled
            };
            SpyOnSpool.PrintSpyContext.Printers.Add(p);
            return p;
        }

        public static List<Printer> GetLocalPrinters(int computerId, int userId)
        {
            List<Printer> prnList = new List<Printer>();
            ManagementScope scope = new ManagementScope(ManagementPath.DefaultPath);
            scope.Connect();
            ManagementClass m = new ManagementClass("Win32_Printer");
            using (ManagementObjectCollection o = m.GetInstances())
                foreach (ManagementObject p in o)
                {
                    Server server = Server.Add(Environment.MachineName);
                    PrinterStruct prn = new PrinterStruct
                    {
                        Name = p["Name"].ToString(),
                        UserId = userId,
                        ComputerId = computerId,
                        ServerId = server.Id,
                        Enabled = true
                    };
                    prnList.Add(Add(prn));
                }
            if (prnList.Count > 0)
                SpyOnSpool.PrintSpyContext.SaveChanges();

            return prnList;
        }

        public static Printer Get(PrinterStruct printer) => SpyOnSpool.PrintSpyContext.Printers.FirstOrDefault(p => ((p.ComputerId == printer.ComputerId) && (p.ServerId == printer.ServerId) && (p.Name == printer.Name) ));

        public static void Rename(int id, string name)
        {
            var printer = SpyOnSpool.PrintSpyContext.Printers.FirstOrDefault(p => p.Id == id);
            if (printer == null) return;
            printer.Name = name;
            SpyOnSpool.PrintSpyContext.Entry(printer).State = EntityState.Modified;
            SpyOnSpool.PrintSpyContext.SaveChanges();
        }
    }
}
