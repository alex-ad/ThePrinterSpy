using System;
using System.Globalization;
using System.Windows.Data;
using ThePrinterSpyControl.ModelBuilders;

namespace ThePrinterSpyControl.ValueConverters
{
    class PrinterFromIdValueConverter : IValueConverter
    {
        private readonly PrintersCollection _printers;
        private readonly ServersCollection _servers;
        private readonly ComputersCollection _computers;

        public PrinterFromIdValueConverter()
        {
            _printers = new PrintersCollection();
            _servers = new ServersCollection();
            _computers = new ComputersCollection();
    }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) throw new ArgumentNullException(nameof(value), "Printer Id cannot be Null");

            var p = _printers.GetPrinter((int)value);
            var c = _computers.GetComputer(p.ComputerId);
            var s = _servers.GetServer(p.ServerId);
            string shared = (string.Compare(c, s, StringComparison.OrdinalIgnoreCase) == 0) ? "" : $" [{s}]";
            return p.Name + shared;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
