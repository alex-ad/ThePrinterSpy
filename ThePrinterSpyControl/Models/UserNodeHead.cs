using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThePrinterSpyControl.ViewModels;

namespace ThePrinterSpyControl.Models
{
    public class UserNodeHead : UserNodeTail
    {
        public static PrintersCollection Printers { get; set; }

        public UserNodeHead()
        {
            Printers = new PrintersCollection();
        }
    }
}
