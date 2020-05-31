using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThePrinterSpyControl.Models
{
    public class PrintDataGrid
    {
        public string DocName { get; set; }
        public int Pages { get; set; }
        public DateTime TimeStamp { get; set; }
        public string UserName { get; set; }
        public string PrinterName { get; set; }
    }
}
