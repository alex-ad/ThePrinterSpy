using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThePrinterSpyControl.ViewModels;

namespace ThePrinterSpyControl.Models
{
    public class UserNodeHead : UserNodeTail
    {
        public ObservableCollection<PrinterNodeTail> Printers { get; set; }

        public UserNodeHead()
        {
            Printers = new ObservableCollection<PrinterNodeTail>();
        }
    }
}
