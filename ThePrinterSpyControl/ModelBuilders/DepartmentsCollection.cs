using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using ThePrinterSpyControl.Models;

namespace ThePrinterSpyControl.ModelBuilders
{
    public class DepartmentsCollection
    {
        public static ObservableCollection<DepartmentNode> Departments { get; }
        private static readonly TotalCountStat TotalStat;
        private readonly UsersCollection _users;

        static DepartmentsCollection()
        {
            Departments = new ObservableCollection<DepartmentNode>();
            TotalStat = new TotalCountStat();
            Departments.CollectionChanged += Departments_CollectionChanged;
        }

        public DepartmentsCollection()
        {
            _users = new UsersCollection();
        }

        private static void Departments_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add || e.Action == NotifyCollectionChangedAction.Remove ||
                e.Action == NotifyCollectionChangedAction.Reset)
            {
                TotalStat.Departments = Departments.Count();
            }
        }
        
        public void GetAll()
        {
            List<string> departments;
            try
            {
                departments = _users.GetCollection().Select(x => x.Department).Distinct().ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("DB: table 'Users' is missing or empty", ex.InnerException);
            }

            if (!departments.Any()) return;

            Departments.Clear();

            foreach (string d in departments)
            {
                var u = _users.GetUserIdsByDepartment(d);
                if (u == null || !u.Any()) continue;

                Departments.Add(new DepartmentNode
                {
                    Name = d,
                    UserIds = u,
                    Comment = $"[{u.Count}]"
                });
            }
        }
    }
}
