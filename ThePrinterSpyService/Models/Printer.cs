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

        private static Printer Add(PrinterStruct printer)
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

        private static Printer AddLocal(PrinterStruct printer)
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
            return p;
        }

        public static List<Printer> BuildLocalPrintersList(int computerId, int userId)
        {
            List<Printer> prnList = new List<Printer>();
            ManagementScope scope = new ManagementScope(ManagementPath.DefaultPath);
            scope.Connect();
            ManagementClass m = new ManagementClass("Win32_Printer");
            using (ManagementObjectCollection o = m.GetInstances())
                foreach (ManagementObject p in o)
                {
                    Server server = Server.Add(GetServerName(p["ServerName"]?.ToString(), p["PortName"]?.ToString()));
                    PrinterStruct prn = new PrinterStruct
                    {
                        Name = PrinterNameConvert(p["Name"].ToString()),
                        UserId = userId,
                        ComputerId = computerId,
                        ServerId = server.Id,
                        Enabled = !IsFilter(p["Name"].ToString())
                    };
                    prnList.Add(Add(prn));
                }
            if (prnList.Count > 0)
                SpyOnSpool.PrintSpyContext.SaveChanges();

            return prnList;
        }

        public static List<Printer> GetLocalList(int computerId, int userId)
        {
            List<Printer> prnList = new List<Printer>();
            ManagementScope scope = new ManagementScope(ManagementPath.DefaultPath);
            scope.Connect();
            ManagementClass m = new ManagementClass("Win32_Printer");
            using (ManagementObjectCollection o = m.GetInstances())
                foreach (ManagementObject p in o)
                {
                    Server server = Server.Add(GetServerName(p["ServerName"].ToString(), p["PortName"].ToString()));
                    PrinterStruct prn = new PrinterStruct
                    {
                        Name = PrinterNameConvert(p["Name"].ToString()),
                        UserId = userId,
                        ComputerId = computerId,
                        ServerId = server.Id,
                        Enabled = !IsFilter(p["Name"].ToString())
                    };
                    prnList.Add(AddLocal(prn));
                }

            return prnList;
        }

        public static Printer GetPrinterByName(int computerId, int userId, string printerName) => SpyOnSpool.PrintSpyContext.Printers.FirstOrDefault(p => ((p.ComputerId == computerId) && (p.UserId == userId) && (p.Name == printerName)));

        private static Printer Get(PrinterStruct printer) => SpyOnSpool.PrintSpyContext.Printers.FirstOrDefault(p => ((p.ComputerId == printer.ComputerId) && (p.ServerId == printer.ServerId) && (p.Name == printer.Name) ));

        public static void Rename(int computerId, int userId, string printerName, ref List<Printer> printersList)
        {
            var printer = GetRenamedPrinter(printersList, GetLocalList(computerId, userId), printerName);
            if (printer == null) return;

            printersList.Find(x => x.Id == printer.Id).Name = printerName;
            printer.Name = printerName;
            SpyOnSpool.PrintSpyContext.Entry(printer).State = EntityState.Modified;
            SpyOnSpool.PrintSpyContext.SaveChanges();
        }

        private static bool IsFilter(string printerName)
        {
            string[] filter = { "FAX", "XPS", "PDF", "ONENOTE", "2IMAGE", "VIRTUAL" };
            foreach (var s in filter)
                if (printerName.ToUpperInvariant().Contains(s)) return true;
            return false;
        }

        private static Printer GetRenamedPrinter(List<Printer> oldList, List<Printer> newList, string newName)
        {
            try
            {
                return oldList.Except(newList).Single(x => x.Name != newName);
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }

        private static string PrinterNameConvert(string name)
        {
            var slash = name.LastIndexOf('\\');
            if (slash > 0)
                name = name.Substring(slash + 1);
            return name;
        }

        private static string GetServerName(string server, string port)
        {
            var serverName = server?.Replace("\\", "") ?? Environment.MachineName;

            var slash = port.LastIndexOf('\\');
            if (slash < 0) return serverName;

            serverName = port.Substring(0, slash).Replace("\\", "");

            return serverName;
        }
    }
}
