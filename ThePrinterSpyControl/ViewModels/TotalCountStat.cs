using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ThePrinterSpyControl.ViewModels
{
    public class TotalCountStat : INotifyPropertyChanged
    {
        private int _users;
        private int _printersAll;
        private int _printersEnabled;
        private int _computers;
        private int _departments;
        private int _docsByNode;
        private int _pagesByNode;
        private string _reportPeriod;

        public int Users
        {
            get => _users;
            set
            {
                if (value == _users) return;
                _users = value;
                OnPropertyChanged();
            }

        }
        public int PrintersAll
        {
            get => _printersAll;
            set
            {
                if (value == _printersAll) return;
                _printersAll = value;
                OnPropertyChanged();
            }

        }
        public int PrintersEnabled
        {
            get => _printersEnabled;
            set
            {
                if (value == _printersEnabled) return;
                _printersEnabled = value;
                OnPropertyChanged();
            }

        }
        public int Computers
        {
            get => _computers;
            set
            {
                if (value == _computers) return;
                _computers = value;
                OnPropertyChanged();
            }

        }
        public int Departments
        {
            get => _departments;
            set
            {
                if (value == _departments) return;
                _departments = value;
                OnPropertyChanged();
            }

        }
        public int DocsByNode
        {
            get => _docsByNode;
            set
            {
                if (value == _docsByNode) return;
                _docsByNode = value;
                OnPropertyChanged();
            }

        }
        public int PagesByNode
        {
            get => _pagesByNode;
            set
            {
                if (value == _pagesByNode) return;
                _pagesByNode = value;
                OnPropertyChanged();
            }
        }
        public string ReportPeriod
        {
            get => _reportPeriod;
            set
            {
                if (value == _reportPeriod) return;
                _reportPeriod = value;
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
