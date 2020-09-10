using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using ThePrinterSpyControl.Modules;
using ThePrinterSpyControl.Properties;

namespace ThePrinterSpyControl.Models
{
    public partial class ConfigActiveDirectory : INotifyPropertyChanged
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

        [MinLength(5, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "ConfigADServerLengthError")]
        [RegularExpression("^\\w{2,}\\.\\w{2,}.*$", ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "ConfigADServerValueError")]
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
        [MinLength(2, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "ConfigADUserLengthError")]
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
        [MinLength(1, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "ConfigADPasswordLengthError")]
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

        public ConfigActiveDirectory(AppConfig.AdConfig cfg)
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
