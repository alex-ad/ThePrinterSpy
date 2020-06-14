using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ThePrinterSpyControl.Modules;

namespace ThePrinterSpyControl.ViewModels
{
    public class SelectedPrinter : INotifyPropertyChanged
    {
        private int _id;
        private string _newName;
        private string _oldName;
        private bool _enabled;
        private string _computerName;

        public int Id
        {
            get => _id;
            set
            {
                if (value == _id) return;
                _id = value;
                TryGetComputerName(_id, out _computerName);
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
                if (value == _enabled) return;
                _enabled = value;
                PrinterManagement.SetEnabled(this);
                OnPropertyChanged();
            }
        }

        public string ComputerName
        {
            get => _computerName;
            private set => _computerName = value;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void TryGetComputerName(int printerId, out string computerName)
        {
            computerName = string.Empty;
            var query = from p in PrinterSpyViewModel.Context.Printers
                where p.Id == printerId
                select p.ComputerId;

            if (!query.Any()) return;

            try
            {
                computerName = PrinterSpyViewModel.Computers.FirstOrDefault(x => x.Id == query.ToList()[0]).NetbiosName;
            }
            catch
            {
                computerName = string.Empty;
            }
        }
    }
}
