using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;

namespace ThePrinterSpyControl.Models
{
    public struct PrinterStruct
    {
        public string Name { get; set; }
        public int ComputerId { get; set; }
        public int ServerId { get; set; }
        public string DeviceId { get; set; }
        public bool Enabled { get; set; }
    }

    public class Printer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ComputerId { get; set; }
        public int ServerId { get; set; }
        public string DeviceId { get; set; }
        public bool Enabled { get; set; }

        public static Printer AddPrinter(PrinterStruct printer)
        {
            Printer p = GetPrinter(printer);
            if (p != null) return p;
            p = new Printer
            {
                Name = printer.Name,
                ComputerId = printer.ComputerId,
                ServerId = printer.ServerId,
                DeviceId = printer.DeviceId,
                Enabled = printer.Enabled
            };
            SpyOnSpool.PrintSpyContext.Printers.Add(p);
            return p;
        }

        public static List<Printer> GetLocalPrinters(int computerId)
        {
            List<Printer> prnList = new List<Printer>();
            ManagementScope scope = new ManagementScope(ManagementPath.DefaultPath);
            scope.Connect();
            ManagementClass m = new ManagementClass("Win32_Printer");
            using (ManagementObjectCollection o = m.GetInstances())
                foreach (ManagementObject p in o)
                {
                    Server server = Server.AddServer(Environment.MachineName);
                    PrinterStruct prn = new PrinterStruct
                    {
                        Name = p["Name"].ToString(),
                        ComputerId = computerId,
                        ServerId = server.Id,
                        DeviceId = p["DeviceID"].ToString(),
                        Enabled = true
                    };
                    prnList.Add(AddPrinter(prn));
                }
            if (prnList.Count > 0)
                SpyOnSpool.PrintSpyContext.SaveChanges();

            return prnList;
        }

        public static Printer GetPrinter(PrinterStruct printer) => SpyOnSpool.PrintSpyContext.Printers.FirstOrDefault(p => ((p.DeviceId == printer.DeviceId) && (p.ComputerId == printer.ComputerId) && (p.ServerId == printer.ServerId)));
    }
}
