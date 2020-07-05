using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ThePrinterSpyControl.Modules;
using ThePrinterSpyControl.ViewModels;

namespace ThePrinterSpyControl.Models
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

        public async void GetAll()
        {
            var computers = await _base.GetComputersList();
            if (!computers.Any()) return;

            Computers.Clear();
            var printers = new PrintersCollection();

            //await Task.Run(() =>
            //{
            foreach (var c in computers)
            {
                var pIds = printers.GetIdsByComputer(c.Id);
                var pTotal = pIds.Count();
                var pEnabled = printers.GetEnabledCountByComputer(c.Id);
                Computers.Add(new ComputerNode
                {
                    Id = c.Id,
                    NetbiosName = c.Name,
                    PrinterIds = pIds ?? null,
                    Comment = $"[{pEnabled}/{pTotal}]"
                });
            }
            //});
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
    }
}
