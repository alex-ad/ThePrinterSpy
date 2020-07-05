using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Documents;
using ThePrinterSpyControl.Modules;
using ThePrinterSpyControl.ViewModels;

namespace ThePrinterSpyControl.Models
{
    public class PrintersCollection
    {
        public static ObservableCollection<PrinterNodeTail> Printers { get; }
        private static readonly TotalCountStat TotalStat = new TotalCountStat();
        private readonly DBase _base;

        static PrintersCollection()
        {
            Printers = new ObservableCollection<PrinterNodeTail>();
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
                TotalStat.PrintersAll = Printers.Count;
                TotalStat.PrintersEnabled = Printers.Count(x => x.Enabled);
            }
        }

        public async void GetAll()
        {
            var printers = await _base.GetPrintersList();
            if (!printers.Any()) return;

            Printers.Clear();

            //await Task.Run(() =>
            //{
                foreach (var p in printers)
                {
                    Printers.Add(new PrinterNodeTail
                    {
                        Id = p.Id,
                        Name = p.Name,
                        UserId = p.UserId,
                        ComputerId = p.ComputerId,
                        ServerId = p.ServerId,
                        Enabled = p.Enabled,
                    });
                }
            //});
        }

        public List<int> GetIdsByUser(int id) =>
            Printers.Where(x => x.UserId == id).Select(x => x.Id).ToList();

        public List<int> GetIdsByComputer(int id) => Printers.Where(x => x.ComputerId == id).Select(x => x.Id).ToList();

        public int GetEnabledCountByUser(int id) => Printers.Count(x=>x.UserId == id && x.Enabled);

        public int GetEnabledCountByComputer(int id) => Printers.Count(x => x.ComputerId == id && x.Enabled);

        public int GetTotalCountByComputer(int id) => Printers.Count(x => x.ComputerId == id);

        public PrinterNodeTail GetPrinter(int id) => Printers.FirstOrDefault(x => x.Id == id);

        public async Task SetPrinterEnabled(int id, bool enabled)
        {
            await SetDbPrinterEnabled(id, enabled);
            Printers.FirstOrDefault(x => x.Id == id).Enabled = enabled;
            //UpdateUserPrintersStat(user);
        }

        private async Task SetDbPrinterEnabled(int id, bool enabled)
        {
            await _base.SetPrinterEnabled(id, enabled);
        }

        /*private void UpdateUserPrintersStat(UserNodeHead user)
        {
            var pTotal = GetPrintersByUser(user.Id);
            var pEnabled = from pe in pTotal
                where pe.Enabled
                select pe;
            user.Comment = $"[{pEnabled.Count()}/{pTotal.Count()}]";
        }*/
    }
}
