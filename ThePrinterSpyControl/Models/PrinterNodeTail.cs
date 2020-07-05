using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ThePrinterSpyControl.ViewModels;

namespace ThePrinterSpyControl.Models
{
    public class PrinterNodeTail : INotifyPropertyChanged
    {
        private string _name;
        private bool _enabled;
        private readonly TotalCountStat _totalStat = new TotalCountStat();
        private readonly ListUserPrinter _userPrinter = new ListUserPrinter();

        public int Id { get; set; }

        public string Name
        {
            get => _name;
            set
            {
                if (value == _name) return;
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
