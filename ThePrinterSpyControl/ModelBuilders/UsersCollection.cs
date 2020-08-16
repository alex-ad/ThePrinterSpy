using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using ThePrinterSpyControl.DataBase;
using ThePrinterSpyControl.Models;
using ThePrinterSpyControl.ViewModels;

namespace ThePrinterSpyControl.ModelBuilders
{
    public class UsersCollection
    {
        public static ObservableCollection<UserNode> Users { get; }
        private readonly DBase _base;
        private readonly PrintersCollection _printers;

        static UsersCollection()
        {
            Users = new ObservableCollection<UserNode>();
            Users.CollectionChanged += Users_CollectionChanged;
        }

        public UsersCollection()
        {
            _base = new DBase();
            _printers = new PrintersCollection();
        }

        private static void Users_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add || e.Action == NotifyCollectionChangedAction.Remove ||
                e.Action == NotifyCollectionChangedAction.Reset)
            {
                PrinterSpyViewModel.TotalStat.Users = Users.Count();
            }
        }

        public void GetAll()
        {
            List<User> users;
            try
            {
                users = _base.GetUsersList().Result;
            }
            catch (Exception ex)
            {
                throw new Exception("DB: table 'Users' is missing or empty", ex.InnerException);
            }
           
            if (users == null || !users.Any()) return;

            Users.Clear();

            foreach (var u in users)
            {
                var pIds = _printers.GetIdsByUser(u.Id);
                var pTotal = pIds.Count();
                var pEnabled = _printers.GetEnabledCountByUser(u.Id);

                Users.Add(new UserNode
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    AccountName = u.AccountName,
                    PrinterIds = pIds,
                    Company = u.Company,
                    Department = u.Department,
                    Position = u.Position,
                    Sid = u.Sid,
                    Comment = $"[{pEnabled}/{pTotal}]"
            });
            }
        }

        public List<UserNode> GetUsersByDepartment(string name) => Users.Where(x => x.Department == name).ToList();

        public List<int> GetUserIdsByDepartment(string name) =>
            Users.Where(x => x.Department == name).Select(x => x.Id).ToList();

        public ObservableCollection<UserNode> GetCollection() => Users;

        public void PropertyPrinterIdsChanged(int id)
        {
            var u = Users.FirstOrDefault(x => x.PrinterIds.Contains(id));
            if (u == null) return;
            u.PrinterIds = new List<int>(u.PrinterIds);
            var pTotal = _printers.GetTotalCountByUser(u.Id);
            var pEnabled = _printers.GetEnabledCountByUser(u.Id);
            u.Comment = $"[{pEnabled}/{pTotal}]";
        }

        public UserNode GetUser(int id) => Users.FirstOrDefault(x => x.Id == id);

        public UserNode GetUser(string sid) => Users.FirstOrDefault(x => x.Sid == sid);

        public void UpdateUser(Principal user)
        {
            var u = GetUser(user.Sid.ToString());
            if (u == null) return;
            u.FullName = user.DisplayName;
            u.AccountName = user.SamAccountName;
            u.Department = user.Description;
            _base.UpdateUser(u);
        }

        public void PropertyPrinterListChanged(int id)
        {
            var u = Users.FirstOrDefault(x => x.PrinterIds.Contains(id));
            if (u == null) return;
            u.PrinterIds.Remove(id);
            var p = new List<int>(u.PrinterIds);
            u.PrinterIds = p;
            var pTotal = _printers.GetTotalCountByUser(u.Id);
            var pEnabled = _printers.GetEnabledCountByUser(u.Id);
            u.Comment = $"[{pEnabled}/{pTotal}]";
        }
    }
}
