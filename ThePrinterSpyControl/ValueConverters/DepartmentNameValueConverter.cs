using System;
using System.Globalization;
using System.Windows.Data;

namespace ThePrinterSpyControl.ValueConverters
{
    class DepartmentNameValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((value == null) || ((string)value).Length == 0) ? "<Без имени>" : (string)value;
            }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
