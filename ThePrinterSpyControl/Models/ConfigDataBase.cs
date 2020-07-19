using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Props = ThePrinterSpyControl.Properties.Settings;

namespace ThePrinterSpyControl.Models
{
    public class ConfigDataBase : INotifyPropertyChanged
    {
        private DbaseType _dbaseConnection;
        private byte _dbaseConnectionIndex;
        private string _server;
        private string _base;
        private string _user;
        private string _password;

        public enum DbaseType : byte
        {
            MsSql = 0,
            MySql
        }

        public DbaseType DbaseConnection
        {
            get => _dbaseConnection;
            set
            {
                if (value == _dbaseConnection) return;
                _dbaseConnection = value;
                OnPropertyChanged();
            }
        }

        public byte DbaseConnectionIndex
        {
            get => _dbaseConnectionIndex;
            set
            {
                if (value == _dbaseConnectionIndex) return;
                _dbaseConnectionIndex = value;
                _dbaseConnection = (DbaseType) _dbaseConnectionIndex;
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

        public string Base
        {
            get => _base;
            set
            {
                if (value == _base) return;
                _base = value;
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

        public Dictionary<DbaseType, string> DbaseConnectionList { get; }=new Dictionary<DbaseType, string>
        {
            { DbaseType.MsSql, "MS SQL" },
            { DbaseType.MySql, "My SQL" }
        };

        public ConfigDataBase()
        {
            try
            {
                _dbaseConnectionIndex = Props.Default.DbConnectionType;
                _dbaseConnection = (DbaseType) Props.Default.DbConnectionType;
                _server = Props.Default.DbServer;
                _base = Props.Default.DbName;
                _user = Props.Default.DbUser;
                _password = Props.Default.DbPassword;
            }
            catch
            {
                _dbaseConnectionIndex = 0;
                _dbaseConnection = DbaseType.MsSql;
                _server = "(localhost)";
                _base = "PrinterSpy";
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
