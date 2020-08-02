using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

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
                if (value == null || string.Compare(_fullName, value, StringComparison.OrdinalIgnoreCase) == 0) return;
                _fullName = value;
                OnPropertyChanged();
            }
        }

        public string AccountName
        {
            get => _accountName;
            set
            {
                if (value == null ||
                    string.Compare(_accountName, value, StringComparison.OrdinalIgnoreCase) == 0) return;
                _accountName = value;
                OnPropertyChanged();
            }
        }

        public string Department
        {
            get => _department;
            set
            {
                if (value == null ||
                    string.Compare(_department, value, StringComparison.OrdinalIgnoreCase) == 0) return;
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
                if (_comment == null ||
                    string.Compare(_comment, value, StringComparison.OrdinalIgnoreCase) == 0) return;
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
