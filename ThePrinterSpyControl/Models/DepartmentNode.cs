using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ThePrinterSpyControl.Models
{
    public class DepartmentNode : INotifyPropertyChanged
    {
        private string _comment;
        private List<int> _userIds;

        public int Id { get; set; }
        public string Name { get; set; }
        public string Comment
        {
            get => _comment;
            set
            {
                if (value.Equals(_comment, StringComparison.InvariantCulture)) return;
                _comment = value;
                OnPropertyChanged();
            }
        }
        public List<int> UserIds
        {
            get => _userIds;
            set
            {
                if (_userIds != null && _userIds.Equals(value)) return;
                _userIds = value;
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
