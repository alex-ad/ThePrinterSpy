using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ThePrinterSpyControl.Modules;

namespace ThePrinterSpyControl.Models
{
    public class ActiveDirectoryConfig : INotifyPropertyChanged
    {
        private bool _isEnabled;
        private string _server;
        private string _user;
        private string _password;

        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                if (value == _isEnabled) return;
                _isEnabled = value;
                OnPropertyChanged();
            }
        }

        public string Server
        {
            get => _server;
            set
            {
                if (value == _server) return;
                _server = value;
                OnPropertyChanged();
            }
        }

        public string User
        {
            get => _user;
            set
            {
                if (value == _user) return;
                _user = value;
                OnPropertyChanged();
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                if (value == _password) return;
                _password = value;
                OnPropertyChanged();
            }
        }

        public ActiveDirectoryConfig(AppConfig.AdConfig cfg)
        {
            try
            {
                _isEnabled = cfg.Enabled != 0;
                _server = cfg.Server;
                _user = cfg.User;
                _password = cfg.Password;
            }
            catch
            {
                _isEnabled = false;
                _server = string.Empty;
                _user = string.Empty;
                _password = string.Empty;
            }
            finally
            {
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
