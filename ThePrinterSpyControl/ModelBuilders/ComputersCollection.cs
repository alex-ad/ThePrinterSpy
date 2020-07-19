using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using ThePrinterSpyControl.DataBase;
using ThePrinterSpyControl.Models;
using ThePrinterSpyControl.Modules;

namespace ThePrinterSpyControl.ModelBuilders
{
    public class ComputersCollection
    {
        public static ObservableCollection<ComputerNode> Computers { get; }
        private static readonly TotalCountStat TotalStat = new TotalCountStat();
        private readonly DBase _base;
        private readonly PrintersCollection _printers;

        static ComputersCollection()
        {
            Computers = new ObservableCollection<ComputerNode>();
            Computers.CollectionChanged += Computers_CollectionChanged;
        }

        public ComputersCollection()
        {
            _base = new DBase();
            _printers = new PrintersCollection();
        }

        private static void Computers_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add || e.Action == NotifyCollectionChangedAction.Remove ||
                e.Action == NotifyCollectionChangedAction.Reset)
            {
                TotalStat.Computers = Computers.Count();
            }
        }

        public void GetAll()
        {
            var computers = _base.GetComputersList();
            if (!computers.Any()) return;

            Computers.Clear();

            //await Task.Run(() =>
            //{
            foreach (var c in computers)
            {
                var pIds = _printers.GetIdsByComputer(c.Id);
                var pTotal = pIds.Count();
                var pEnabled = _printers.GetEnabledCountByComputer(c.Id);
                Computers.Add(new ComputerNode
                {
                    Id = c.Id,
                    NetBiosName = c.Name,
                    PrinterIds = pIds ?? null,
                    Comment = $"[{pEnabled}/{pTotal}]"
                });
            }
            //});
        }

        public string GetNameByPrinterId(int id)
        {
            string computerName = string.Empty;

            var p = _printers.GetPrinter(id);
            if (p == null) return computerName;

            var c = Computers.FirstOrDefault(x => x.Id == p.ComputerId);
            if (c != null) computerName = c.NetBiosName;
            return computerName;
        }

        public void PropertyPrinterIdsChanged(int id)
        {
            var c = Computers.FirstOrDefault(x=>x.PrinterIds.Contains(id));
            if (c == null) return;
            c.PrinterIds = new List<int>(c.PrinterIds);
            var pTotal = _printers.GetTotalCountByComputer(c.Id);
            var pEnabled = _printers.GetEnabledCountByComputer(c.Id);
            c.Comment = $"[{pEnabled}/{pTotal}]";
        }

        public void PropertyPrinterListChanged(int id)
        {
            var c = Computers.FirstOrDefault(x => x.PrinterIds.Contains(id));
            if (c == null) return;
            c.PrinterIds.Remove(id);
            var p = new List<int>(c.PrinterIds);
            c.PrinterIds = p;
            var pTotal = _printers.GetTotalCountByComputer(c.Id);
            var pEnabled = _printers.GetEnabledCountByComputer(c.Id);
            c.Comment = $"[{pEnabled}/{pTotal}]";
        }
    }
}
