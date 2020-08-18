using System;
using System.Globalization;
using System.Windows.Data;
using ThePrinterSpyControl.ModelBuilders;

namespace ThePrinterSpyControl.ValueConverters
{
    class UserFromIdValueConverter : IValueConverter
    {
        private readonly UsersCollection _users = new UsersCollection();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) throw new ArgumentNullException(nameof(value), "User Id cannot be Null");
            var u = _users.GetUser((int) value);
            return (!string.IsNullOrEmpty(u.FullName)) ? $"{u.FullName} ({u.AccountName})" : u.AccountName;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
