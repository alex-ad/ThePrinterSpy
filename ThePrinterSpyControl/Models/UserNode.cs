using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace ThePrinterSpyControl.Models
{
    public class UserNode : INotifyPropertyChanged
    {
        private string _fullName;
        private string _accountName;
        private string _comment;
        private string _department;
        private List<int> _printerIds;

        public int Id { get; set; }

        public string FullName
        {
            get => _fullName;
            set
            {
                if (value == null || value.Equals(_fullName, StringComparison.InvariantCulture)) return;
                _fullName = value;
                OnPropertyChanged();
            }
        }

        public string AccountName
        {
            get => _accountName;
            set
            {
                if (value == null || value.Equals(_accountName, StringComparison.InvariantCulture)) return;
                _accountName = value;
                OnPropertyChanged();
            }
        }

        public string Department
        {
            get => _department;
            set
            {
                if (value == null || value.Equals(_department, StringComparison.InvariantCulture)) return;
                _department = value;
                OnPropertyChanged();
            }
        }
        public string Company { get; set; }
        public string Position { get; set; }
        public string Sid { get; set; }
        public string Comment
        {
            get => _comment;
            set
            {
                if (_comment == null || value.Equals(_comment, StringComparison.InvariantCulture)) return;
                _comment = value;
                OnPropertyChanged();
            }
        }
        public List<int> PrinterIds
        {
            get => _printerIds;
            set
            {
                if (_printerIds != null && _printerIds.Equals(value)) return;
                _printerIds = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
