using System;
using System.Globalization;
using System.Windows.Data;
using ThePrinterSpyControl.Models;

namespace ThePrinterSpyControl.Modules
{
    class PrinterFromIdValueConverter : IValueConverter
    {
        private readonly PrintersCollection _printers = new PrintersCollection();

        public struct Printer
        {
            public int Id;
            public string Name;
            public bool Enabled;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //if (value == null) throw new ArgumentNullException(nameof(value), "Printer Id cannot be Null");
            var p = _printers.GetPrinter((int)value);
            return p.Name;
            /*return new Printer
            {
                Id = 1,
                Name = "qqq",
                Enabled = true
            };*/
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
