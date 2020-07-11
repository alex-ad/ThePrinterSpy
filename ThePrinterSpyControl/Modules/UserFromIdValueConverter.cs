using System;
using System.Globalization;
using System.Windows.Data;
using ThePrinterSpyControl.Models;

namespace ThePrinterSpyControl.Modules
{
    class UserFromIdValueConverter : IValueConverter
    {
        private readonly UsersCollection _users = new UsersCollection();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) throw new ArgumentNullException(nameof(value), "User Id cannot be Null");
            var u = _users.GetUser((int) value);
            var name = u.AccountName;
            if (!string.IsNullOrEmpty(u.FullName)) name = $"{u.FullName} ({u.AccountName})";
            return name;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
