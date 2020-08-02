using System;
using System.Globalization;
using System.Windows.Data;

namespace ThePrinterSpyControl.ValueConverters
{
    class SelectedPrinterVisibilityValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            return (int)value == 0 || (int)value == 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
