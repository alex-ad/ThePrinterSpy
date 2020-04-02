using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;

namespace ThePrinterSpyControl.Models
{
    class PrintersCollection : IEnumerable
    {
        public int LastId => _id;
        private static int _id = 0;

        private ObservableCollection<Printer> Printers { get; }
        private ServerCollection Servers { get; }
        
        public PrintersCollection()
        {
            Printers = new ObservableCollection<Printer>();
            Servers = new ServerCollection();
        }

        public Printer Add(PrinterStruct printer)
        {
            Printer p = GetPrinter(printer);
            if (p != null) return p;
            _id++;
            p = new Printer(_id, printer);
            Printers.Add(p);
            return p;
        }

        public void Remove(int id)
        {
            Printers.Remove(GetPrinter(id));
        }

        public void Remove(string name)
        {
            Printers.Remove(GetPrinter(name));
        }

        public void Remove(Printer printer)
        {
            Printers.Remove(printer);
        }

        public void Clear()
        {
            _id = 0;
            Printers.Clear();
        }

        public Printer GetPrinter(int id) => Printers.FirstOrDefault(p => p.Id == id);

        public Printer GetPrinter(string name) => Printers.FirstOrDefault(p => p.Name == name);
        
        public Printer GetPrinter(PrinterStruct printer) => Printers.FirstOrDefault(p => ((p.DeviceId == printer.DeviceId) && (p.ComputerId == printer.ComputerId)));

        public string GetPrinterName(int id) => Printers.FirstOrDefault(p => p.Id == id)?.Name;

        public string GetPrinterName(Printer printer) => Printers.FirstOrDefault(p => p.Id == printer.Id).Name;

        public int GetPrinterId(string name) => Printers.FirstOrDefault(p => p.Name == name).Id;

        public int GetPrinterId(Printer printer) => Printers.FirstOrDefault(p => p.Id == printer.Id).Id;

        public bool IsExists(Printer printer) => Printers.FirstOrDefault(p => ((p.DeviceId == printer.DeviceId) && (p.ComputerId == printer.ComputerId))) != null;
        
        public bool IsExists(PrinterStruct printer) => Printers.FirstOrDefault(p => ((p.DeviceId == printer.DeviceId) && (p.ComputerId == printer.ComputerId))) != null;

        public bool IsExists(int id) => Printers.FirstOrDefault(p => p.Id == id) != null;

        public bool IsExists(string name) => Printers.FirstOrDefault(p => p.Name == name) != null;

        public List<Printer> FindAllByComputer(Computer computer, bool enabledOnly = true)
        {
            if (Printers.Count < 1) return null;
            List<Printer> list = new List<Printer>();
            if (enabledOnly)
            {
                var printer = from p in Printers where (p.ComputerId == computer.Id && p.Enabled) select p;
                list.AddRange(printer);
            }
            else
            {
                var printer = from p in Printers where (p.ComputerId == computer.Id) select p;
                list.AddRange(printer);
            }
            return list;
        }

        public IEnumerator GetEnumerator()
        {
            if (Printers != null)
            {
                foreach (Printer c in Printers)
                {
                    yield return c;
                }
            }
        }

        public IEnumerable ConvertToString()
        {
            if (Printers != null)
            {
                foreach (Printer c in Printers)
                {
                    yield return $"Id: {c.Id}; Name: {c.Name}";
                }
            }
        }

        public List<Printer> GetLocalPrinters(int computerId)
        {
            List<Printer> prnList = new List<Printer>();
            ManagementScope scope = new ManagementScope(ManagementPath.DefaultPath);
            scope.Connect();
            ManagementClass m = new ManagementClass("Win32_Printer");
            using (ManagementObjectCollection o = m.GetInstances())
                foreach (ManagementObject p in o)
                {
                    //string serverName = (p["ServerName"] != null) ? p["ServerName"].ToString() : Environment.MachineName;
                    Server server = Servers.Add(Environment.MachineName);
                    PrinterStruct prn = new PrinterStruct
                    {
                        Name = p["Name"].ToString(),
                        ComputerId = computerId,
                        ServerId = server.Id,
                        DeviceId = p["DeviceID"].ToString(),
                        Enabled = true
                    };
                    prnList.Add(Add(prn));
                }

            return prnList;
        }
    }
}
