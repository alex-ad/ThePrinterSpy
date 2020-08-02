using System.Collections.Generic;

namespace ThePrinterSpyControl.Models
{
    public class DepartmentNode
    {
        public string Name { get; set; }
        public string Comment { get; set; }
        public List<int> UserIds { get; set; }
    }
}
