using System;
using System.Collections.Generic;
using ThePrinterSpyControl.Models;
using System.DirectoryServices.AccountManagement;
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
            var oPrincipalContext = GetPrincipalContext();
            return oPrincipalContext==null ? null : new PrincipalSearcher(new UserPrincipal(oPrincipalContext)).FindAll();
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
        }
    }
}
