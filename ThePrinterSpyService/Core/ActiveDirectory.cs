using System;
using System.Diagnostics;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;

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

        public static bool Enabled { get; private set; }

        private readonly string _accountName;
        private readonly string _password;
        private readonly string _domain;
        private readonly string _dn;

        public ActiveDirectory()
        {
            _domain = "corp.vgtz.com";
            _dn = "DC=corp,DC=vgtz,DC=com";
            _accountName = "adreader";
            _password = "gfhjkm4adr!";
            Enabled = true;
        }

        public bool TryGetUser(string name)
        {
            TryGetPrincipalContext(out PrincipalContext oPrincipalContext);
            if (oPrincipalContext == null) return false;

            try
            {
                UserPrincipal user = UserPrincipal.FindByIdentity(oPrincipalContext, IdentityType.SamAccountName, name);
                if (user == null) return false;
                DirectoryEntry userExt = user.GetUnderlyingObject() as DirectoryEntry;
                Position = Convert.ToString(userExt.Properties["title"].Value);
                Department = Convert.ToString(userExt.Properties["department"].Value);
                Company = Convert.ToString(userExt.Properties["company"].Value);
                AccountName = user.SamAccountName;
                FullName = user.DisplayName;
                Sid = user.Sid.ToString();
                return true;
            }
            catch (Exception ex)
            {
                Log.AddException(ex);
                return false;
            }
        }

        private void TryGetPrincipalContext(out PrincipalContext principalContext)
        {
            try
            {
                principalContext = new PrincipalContext(ContextType.Domain, _domain, _dn, _accountName, _password);
            }
            catch (Exception ex)
            {
                Log.AddException(ex);
                principalContext = null;
            }
        }
    }
}
