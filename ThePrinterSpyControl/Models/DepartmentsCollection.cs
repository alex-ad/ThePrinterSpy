using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThePrinterSpyControl.Modules;
using ThePrinterSpyControl.ViewModels;

namespace ThePrinterSpyControl.Models
{
    public class DepartmentsCollection
    {
        public static ObservableCollection<DepartmentNode> Departments { get; }
        private static readonly TotalCountStat TotalStat = new TotalCountStat();
        private readonly DBase _base;
        private readonly UsersCollection _users;

        static DepartmentsCollection()
        {
            Departments = new ObservableCollection<DepartmentNode>();
            Departments.CollectionChanged += Departments_CollectionChanged;
        }

        public DepartmentsCollection()
        {
            _base = new DBase();
            _users = new UsersCollection();
        }

        private static void Departments_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add || e.Action == NotifyCollectionChangedAction.Remove ||
                e.Action == NotifyCollectionChangedAction.Reset)
            {
                TotalStat.Departments = Departments.Count();
            }
        }
        
        public void GetAll()
        {
            var departments = _users.GetCollection().Select(x => x.Department).Distinct().ToList();
            int id = 0;

            foreach (string d in departments)
            {
                var u = _users.GetUserIdsByDepartment(d);
                if (u == null || !u.Any()) continue;

                var node = new DepartmentNode
                {
                    Id = id,
                    Name = d,
                    UserIds = u,
                    Comment = $"{u.Count}"
                };

                Departments.Add(node);
            }
        }
    }
}
