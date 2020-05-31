using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThePrinterSpyControl.Models
{
    public class ComputerNodeHead
    {
        public int Id { get; set; }
        public string NetbiosName { get; set; }
        public string Comment { get; set; }
        public ObservableCollection<PrinterNodeTail> Printers { get; set; }

        public ComputerNodeHead()
        {
            Printers = new ObservableCollection<PrinterNodeTail>();
        }
    }
}
