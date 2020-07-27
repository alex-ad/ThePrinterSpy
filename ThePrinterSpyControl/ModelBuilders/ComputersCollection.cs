using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Markup;
using ThePrinterSpyControl.DataBase;
using ThePrinterSpyControl.Models;

namespace ThePrinterSpyControl.ModelBuilders
{
    public class ComputersCollection
    {
        public static ObservableCollection<ComputerNode> Computers { get; }
        private static readonly TotalCountStat TotalStat;
        private static readonly ComputerNode Node;
        private readonly DBase _base;
        private readonly PrintersCollection _printers;

        static ComputersCollection()
        {
            Computers = new ObservableCollection<ComputerNode>();
            TotalStat = new TotalCountStat();
            Node = new ComputerNode();
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
            var computers = _base.GetComputersList().Result;
            if (!computers.Any()) return;
            //BuildAll(computers);

            Computers.Clear();

            foreach (var c in computers)
            {
                var pIds = _printers.GetIdsByComputer(c.Id);
                var pTotal = pIds.Count();
                var pEnabled = _printers.GetEnabledCountByComputer(c.Id);

                Node.Id = c.Id;
                Node.NetBiosName = c.Name;
                Node.PrinterIds = pIds ?? null;
                Node.Comment = $"[{pEnabled}/{pTotal}]";

                Computers.Add(Node);
            }
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

        /*private void BuildAll(List<Computer> computers)
        {
            Computers.Clear();
            //await Task.Run(() =>
            //{
                foreach (var c in computers)
                {
                    var pIds = _printers.GetIdsByComputer(c.Id);
                    var pTotal = pIds.Count();
                    var pEnabled = _printers.GetEnabledCountByComputer(c.Id);
                    //System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    //{
                        Computers.Add(new ComputerNode
                        {
                            Id = c.Id,
                            NetBiosName = c.Name,
                            PrinterIds = pIds ?? null,
                            Comment = $"[{pEnabled}/{pTotal}]"
                        });
                    //});
                }
            //});
        }*/
    }
}
