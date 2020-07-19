using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ThePrinterSpyControl.Models
{
    public class DepartmentNode
    {
        public string Name { get; set; }
        public string Comment { get; set; }
        public List<int> UserIds { get; set; }
    }
}
