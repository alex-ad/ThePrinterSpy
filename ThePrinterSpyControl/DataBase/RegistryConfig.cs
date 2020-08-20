using System;
using System.IO;
using System.Security;
using Microsoft.Win32;

namespace ThePrinterSpyControl.DataBase
{
    public class RegistryConfig
    {
        public string DbServer { get; }
        public string DbUser { get; }
        public string DbPassword { get; }
        public string DbName { get; }

        private const string RegKey = @"SOFTWARE\alex-ad\ThePrinterSpy";
        private const string RegServer = "DbServer";
        private const string RegUser = "DbUser";
        private const string RegPassword = "DbPassword";
        private const string RegBaseName = "DbName";

        public RegistryConfig()
        {
            RegistryKey rKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Default).OpenSubKey(RegKey, false);
            if (rKey == null) throw new Exception("There is error reading config from the system registry. Please, reinstall the application.");

            try
            {
                DbServer = rKey.GetValue(RegServer, string.Empty).ToString();
                DbUser = rKey.GetValue(RegUser, string.Empty).ToString();
                DbPassword = rKey.GetValue(RegPassword, string.Empty).ToString();
                DbName = rKey.GetValue(RegBaseName, string.Empty).ToString();
            }
            catch (SecurityException ex)
            {
                throw new Exception("There is error accessing the system registry. Make sure you are have needed privileges", ex.InnerException);
            }
            catch (ObjectDisposedException ex)
            {
                throw new Exception("There is error accessing the system registry. Make sure you are have needed privileges", ex.InnerException);
            }
            catch (IOException ex)
            {
                throw new Exception("There is error accessing the system registry. Make sure you are have needed privileges", ex.InnerException);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new Exception("There is error accessing the system registry. Make sure you are have needed privileges", ex.InnerException);
            }

            if (string.IsNullOrEmpty(DbServer) || string.IsNullOrEmpty(DbUser) || string.IsNullOrEmpty(DbPassword) || string.IsNullOrEmpty(DbName))
                throw new Exception("There is error reading config from the system registry. Please, reinstall the application.");
        }
    }
}
