using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ThePrinterSpyControl.Models
{
    public class ComputerNode : INotifyPropertyChanged
    {
        private string _comment;
        private List<int> _printerIds;

        public int Id { get; set; }
        public string NetBiosName { get; set; }
        public string Comment
        {
            get => _comment;
            set
            {
                if (string.Compare(_comment, value, StringComparison.OrdinalIgnoreCase) == 0) return;
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
