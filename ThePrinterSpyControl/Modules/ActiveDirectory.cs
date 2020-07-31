using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThePrinterSpyControl.Models;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Windows;
using System.Collections;

namespace ThePrinterSpyControl.Modules
{
    class ActiveDirectory : IEnumerable<Principal>
    {

        private static AdIdentity _identity;

        public ActiveDirectory(AdIdentity ad) {
            _identity = ad;
        }

        private static PrincipalSearchResult<Principal> GetUsers()
        {
            if (_identity == null) return null;

            PrincipalContext oPrincipalContext = GetPrincipalContext();
            if (oPrincipalContext == null)
            {
                LastError.Set("Ошибка подключения к Active Directory");
                return null;
            }

            return new PrincipalSearcher(new UserPrincipal(oPrincipalContext)).FindAll();
        }

        private static PrincipalContext GetPrincipalContext()
        {
            return new PrincipalContext(ContextType.Domain, _identity.Server, _identity.User, _identity.Password);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public IEnumerator<Principal> GetEnumerator()
        {
            return GetUsers().GetEnumerator();
            /*foreach (var p in GetUsers())
            {
                yield return p;
            }*/
        }
    }
}
