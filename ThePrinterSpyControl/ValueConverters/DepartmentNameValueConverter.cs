using System;
using System.Globalization;
using System.Windows.Data;
using ThePrinterSpyControl.Properties;

namespace ThePrinterSpyControl.ValueConverters
{
    class DepartmentNameValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((value == null) || ((string)value).Length == 0) ? Resources.DepartmentNoName : (string)value;
            }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
