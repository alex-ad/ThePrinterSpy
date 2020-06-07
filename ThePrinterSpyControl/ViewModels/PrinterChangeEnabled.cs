using System;
using ThePrinterSpyControl.Models;

namespace ThePrinterSpyControl.ViewModels
{
    class PrinterChangeEnabled
    {
        public int Id { get; }
        public bool Enabled { get; }
       
        public PrinterChangeEnabled(int id, bool enabled)
        {
            Id = id;
            Enabled = enabled;
        }
    }
}
