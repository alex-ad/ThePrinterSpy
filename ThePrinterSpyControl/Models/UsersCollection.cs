using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using ThePrinterSpyControl.Modules;
using ThePrinterSpyControl.ViewModels;

namespace ThePrinterSpyControl.Models
{
    public class UsersCollection
    {
        public static ObservableCollection<UserNodeTail> Users { get; }
        private static readonly TotalCountStat TotalStat = new TotalCountStat();
        private readonly DBase _base;

        static UsersCollection()
        {
            Users = new ObservableCollection<UserNodeTail>();
            Users.CollectionChanged += Users_CollectionChanged;
        }

        public UsersCollection()
        {
            _base = new DBase();

        }

        private static void Users_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add || e.Action == NotifyCollectionChangedAction.Remove ||
                e.Action == NotifyCollectionChangedAction.Reset)
            {
                TotalStat.Users = Users.Count();
            }
        }

        public async void GetAll()
        {
            var users = await _base.GetUsersList();
            if (!users.Any()) return;

            Users.Clear();
            var printers = new PrintersCollection();

            //await Task.Run(() =>
            //{
                foreach (var u in users)
                {
                    var pIds = printers.GetIdsByUser(u.Id);
                    var pTotal = pIds.Count();
                    var pEnabled = printers.GetEnabledCountByUser(u.Id);
                    Users.Add(new UserNodeTail
                    {
                        Id = u.Id,
                        FullName = u.FullName,
                        AccountName = u.AccountName,
                        PrinterIds = pIds ?? null,
                        Comment = $"[{pEnabled}/{pTotal}]"
                    });
                }
            //});
        }
    }
}
