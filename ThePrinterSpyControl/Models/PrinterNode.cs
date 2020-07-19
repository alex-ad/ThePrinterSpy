using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ThePrinterSpyControl.Models
{
    public class PrinterNode : INotifyPropertyChanged
    {
        private string _name;
        private bool _enabled;
        //private readonly TotalCountStat _totalStat = new TotalCountStat();

        public int Id { get; set; }

        public string Name
        {
            get => _name;
            set
            {
                if (value.Equals(_name, StringComparison.InvariantCulture)) return;
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
                //_totalStat.PrintersEnabled = _userPrinter.GetPrintersEnabledCount();
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
