using System;
using System.Globalization;
using System.Windows.Data;

namespace ThePrinterSpyControl.ValueConverters
{
    class FullNameValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((value == null) || (value.ToString().Length < 1)) return "";
            return (string)value + " : ";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
