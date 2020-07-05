using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThePrinterSpyControl.Models;
using ThePrinterSpyControl.Modules;

namespace ThePrinterSpyControl.ViewModels
{
    public class ListUserPrinter
    {
        /*private static readonly DBase Base;
        public static ObservableCollection<UserNodeHead> Users { get; }

        static ListUserPrinter()
        {
            Base = new DBase();
            Users = new ObservableCollection<UserNodeHead>();
        }

        public void AddUser(UserNodeHead userNode) => Users.Add(userNode);

        public void AddPrinter(PrinterNodeTail printerNode, UserNodeHead userNode)
        {

        }

        public async Task RemovePrinter(int id)
        {
            var p = await Base.GetPrinterById(id);
            if (p == null) return;

            await Base.RemovePrintDataByPrinter(p);

            var del = Users.FirstOrDefault().Printers.FirstOrDefault(x => x.Id == id);
            if (del == null) return;

            Users.FirstOrDefault().Printers.Remove(del);
        }

        public List<PrinterNodeTail> GetPrintersByUser(int id) =>
            Users.FirstOrDefault(x => x.Id == id).Printers.ToList();

        public int GetPrintersEnabledCount() => Users.FirstOrDefault().Printers.Count(x => x.Enabled);

        public void SetPrinterName(int id, string name) => Users.FirstOrDefault().Printers.FirstOrDefault(x => x.Id == id).Name = name;

        public async Task SetPrinterEnabled(int id, bool enabled)
        {
            await SetDbPrinterEnabled(id, enabled);

            var pu = Users.FirstOrDefault().Printers.FirstOrDefault(x => x.Id == id);
            if (pu == null) return;
            var user = Users.Where(x => x.Printers.Contains(pu)).ToList()[0];
            if (user == null) return;

            pu.Enabled = enabled;
            UpdateUserPrintersStat(user);
        }

        private async Task SetDbPrinterEnabled(int id, bool enabled)
        {
            await Base.SetPrinterEnabled(id, enabled);
        }

        private void UpdateUserPrintersStat(UserNodeHead user)
        {
            var pTotal = GetPrintersByUser(user.Id);
            var pEnabled = from pe in pTotal
                where pe.Enabled
                select pe;
            user.Comment = $"[{pEnabled.Count()}/{pTotal.Count()}]";
        }*/
    }
}
