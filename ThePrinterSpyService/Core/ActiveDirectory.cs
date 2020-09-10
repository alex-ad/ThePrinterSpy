using System;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using ThePrinterSpyService.Models;

namespace ThePrinterSpyService.Core
{
    class ActiveDirectory
    {
        public string AccountName { get; private set; }
        public string FullName { get; private set; }
        public string Department { get; private set; }
        public string Position { get; private set; }
        public string Company { get; private set; }
        public string Sid { get; private set; }

        private readonly string _accountName;
        private readonly string _password;
        private readonly string _domain;

        public ActiveDirectory()
        {
            Config ad;
            try
            {
                ad = SpyOnSpool.PrintSpyContext.Configs.First();
            }
            catch (InvalidOperationException ex)
            {
                throw new Exception("DataBase is missing or not initialized. Please, reinstall program.", ex);
            }

            _domain = ad?.AdServer;
            _accountName = ad?.AdUser;
            _password = ad?.AdPassword;
        }

        public void TryGetUser(string sid)
        {
            TryGetPrincipalContext(out PrincipalContext oPrincipalContext);
            if (oPrincipalContext == null) return;

            try
            {
                UserPrincipal user = UserPrincipal.FindByIdentity(oPrincipalContext, IdentityType.Sid, sid);
                if (user == null) return;
                DirectoryEntry userExt = user.GetUnderlyingObject() as DirectoryEntry;
                Position = Convert.ToString(userExt?.Properties["title"].Value);
                Department = Convert.ToString(userExt?.Properties["department"].Value);
                Company = Convert.ToString(userExt?.Properties["company"].Value);
                AccountName = user.SamAccountName;
                FullName = user.DisplayName;
                Sid = user.Sid.ToString();
            }
            catch (Exception ex)
            {
                Log.AddException(ex);
            }
        }

        private void TryGetPrincipalContext(out PrincipalContext principalContext)
        {
            try
            {
                principalContext = new PrincipalContext(ContextType.Domain, _domain, _accountName, _password);
            }
            catch (Exception ex)
            {
                Log.AddException(ex);
                principalContext = null;
            }
        }
    }
}
