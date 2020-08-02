using System;
using System.Globalization;
using System.Windows.Data;
using ThePrinterSpyControl.ModelBuilders;

namespace ThePrinterSpyControl.ValueConverters
{
    class PrinterFromIdValueConverter : IValueConverter
    {
        private readonly PrintersCollection _printers = new PrintersCollection();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) throw new ArgumentNullException(nameof(value), "Printer Id cannot be Null");
            var p = _printers.GetPrinter((int)value);
            return p.Name;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
