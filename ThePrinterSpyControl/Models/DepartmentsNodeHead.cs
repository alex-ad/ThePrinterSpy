using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThePrinterSpyControl.Models
{
    public class DepartmentsNodeHead
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Comment { get; set; }
        public ObservableCollection<UserNode> Users { get; set; }

        public DepartmentsNodeHead()
        {
            Users = new ObservableCollection<UserNode>();
        }
    }
}
