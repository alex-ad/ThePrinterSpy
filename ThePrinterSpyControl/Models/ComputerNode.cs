using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ThePrinterSpyControl.Models
{
    public class ComputerNode : INotifyPropertyChanged
    {
        private string _comment;
        private List<int> _printerIds;

        public int Id { get; set; }
        public string NetbiosName { get; set; }
        public string Comment
        {
            get => _comment;
            set
            {
                if (value == _comment) return;
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
