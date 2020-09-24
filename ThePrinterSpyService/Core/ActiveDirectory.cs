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
            Config ad = SpyOnSpool.PrintSpyContext.Configs.Any() ? SpyOnSpool.PrintSpyContext.Configs.First() : new Config();

            _domain = ad?.AdServer ?? string.Empty;
            _accountName = ad?.AdUser ?? string.Empty;
            _password = ad?.AdPassword ?? string.Empty;
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
