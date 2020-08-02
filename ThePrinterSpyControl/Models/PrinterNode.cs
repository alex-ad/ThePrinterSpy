using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ThePrinterSpyControl.Models
{
    public class PrinterNode : INotifyPropertyChanged
    {
        private string _name;
        private bool _enabled;

        public int Id { get; set; }

        public string Name
        {
            get => _name;
            set
            {
                if (string.Compare(_name, value, StringComparison.OrdinalIgnoreCase) == 0) return;
                _name = value;
                OnPropertyChanged();
            }
        }

        public int UserId { get; set; }
        
        public int ComputerId { get; set; }

        public int ServerId { get; set; }

        public bool Enabled
        {
            get => _enabled;
            set
            {
                if (value == _enabled) return;
                _enabled = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this,new PropertyChangedEventArgs(propertyName));
        }
    }
}
