using System.ComponentModel;
using System.Runtime.CompilerServices;
using ThePrinterSpyControl.Modules;

namespace ThePrinterSpyControl.Models
{
    public class SelectedPrinter : INotifyPropertyChanged
    {
        private static int _id;
        private static string _newName;
        private static string _oldName;
        private static bool _enabled;
        private static bool _isNewModel;

        public int Id
        {
            get => _id;
            set
            {
                if (value == _id) return;
                _id = value;
                OnPropertyChanged();
            }
        }

        public string NewName
        {
            get => _newName;
            set
            {
                if (value == _newName) return;
                _newName = value;
                OnPropertyChanged();
            }
        }

        public string OldName
        {
            get => _oldName;
            set
            {
                if (value == _oldName) return;
                _oldName = value;
                OnPropertyChanged();
            }
        }

        public bool Enabled
        {
            get => _enabled;
            set
            {
                if (!_isNewModel) PrinterManagement.SetEnabled(this);
                if (value == _enabled) return;
                _enabled = value;
                OnPropertyChanged();
            }
        }

        public bool IsNewModel
        {
            get => _isNewModel;
            set => _isNewModel = value;
        }

        public void Reset()
        {
            Id = 0;
            IsNewModel = true;
            Enabled = false;
            NewName = string.Empty;
            OldName = string.Empty;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
