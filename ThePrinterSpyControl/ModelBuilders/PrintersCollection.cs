using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using ThePrinterSpyControl.DataBase;
using ThePrinterSpyControl.Models;
using ThePrinterSpyControl.ViewModels;

namespace ThePrinterSpyControl.ModelBuilders
{
    public class PrintersCollection
    {
        public static ObservableCollection<PrinterNode> Printers { get; }
        private readonly DBase _base;

        static PrintersCollection()
        {
            Printers = new ObservableCollection<PrinterNode>();
            Printers.CollectionChanged += Printers_CollectionChanged;
        }

        public PrintersCollection()
        {
            _base = new DBase();
        }

        private static void Printers_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add || e.Action == NotifyCollectionChangedAction.Remove ||
                e.Action == NotifyCollectionChangedAction.Reset)
            {
                PrinterSpyViewModel.TotalStat.PrintersAll = Printers.Count;
                PrinterSpyViewModel.TotalStat.PrintersEnabled = Printers.Count(x => x.Enabled);
            }
        }

        public void GetAll()
        {
            List<Printer> printers;
            try
            {
                printers = _base.GetPrintersList().Result;
            }
            catch (Exception ex)
            {
                throw new Exception("DB: table 'Printers' is missing or empty", ex.InnerException);
            }

            if (printers == null || !printers.Any()) return;

            Printers.Clear();

            foreach (var p in printers)
            {
                Printers.Add(new PrinterNode
                {
                    Id = p.Id,
                    Name = p.Name,
                    UserId = p.UserId,
                    ComputerId = p.ComputerId,
                    ServerId = p.ServerId,
                    Enabled = p.Enabled
                });
            }
        }

        public List<int> GetIdsByUser(int id) =>
            Printers.Where(x => x.UserId == id).Select(x => x.Id).ToList();

        public List<int> GetIdsByComputer(int id) => Printers.Where(x => x.ComputerId == id).Select(x => x.Id).ToList();

        public int GetEnabledCountByUser(int id) => Printers.Count(x=>x.UserId == id && x.Enabled);

        public int GetTotalCountByUser(int id) => Printers.Count(x => x.UserId == id);

        public int GetEnabledCountByComputer(int id) => Printers.Count(x => x.ComputerId == id && x.Enabled);

        public int GetTotalCountByComputer(int id) => Printers.Count(x => x.ComputerId == id);

        public PrinterNode GetPrinter(int id) => Printers.FirstOrDefault(x => x.Id == id);

        public ObservableCollection<PrinterNode> GetCollection() => Printers;

        public async Task SetPrinterEnabled(int id, bool enabled)
        {
            var p = Printers.FirstOrDefault(x => x.Id == id);
            if (p != null) p.Enabled = enabled;
            await SetDbPrinterEnabled(id, enabled);
        }

        public void SetPrinterName(int id, string name)
        {
            var p = Printers.FirstOrDefault(x => x.Id == id);
            if (p == null) return;
            p.Name = name;
            //SetDbPrinterName(id, name);
        }

        public async Task Remove(int id)
        {
            await RemoveFromDb(id);
            var p = Printers.FirstOrDefault(x => x.Id == id);
            if (p == null) return;
            Printers.Remove(p);
        }

        private async Task SetDbPrinterEnabled(int id, bool enabled) => await _base.SetPrinterEnabled(id, enabled);

        //private void SetDbPrinterName(int id, string name) => _base.SetPrinterName(id, name);

        private async Task RemoveFromDb(int id)
        {
            await _base.RemovePrinter(id);
        }
    }
}
