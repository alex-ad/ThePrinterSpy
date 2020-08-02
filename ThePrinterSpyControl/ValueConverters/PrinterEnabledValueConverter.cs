using System;
using System.Globalization;
using System.Windows.Data;
using ThePrinterSpyControl.ModelBuilders;

namespace ThePrinterSpyControl.ValueConverters
{
    class PrinterEnabledValueConverter : IValueConverter
    {
        private readonly PrintersCollection _printers = new PrintersCollection();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return "Black";
            var p = _printers.GetPrinter((int)value);
            return (p.Enabled) ? "Black" : "Gray";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
