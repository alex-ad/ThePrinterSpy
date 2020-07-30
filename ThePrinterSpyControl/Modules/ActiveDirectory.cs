using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThePrinterSpyControl.Models;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;

namespace ThePrinterSpyControl.Modules
{
    class ActiveDirectory
    {
        private readonly UserNode usersNode;
        private readonly AdIdentity _identity;

        public ActiveDirectory(AdIdentity ad) {
            _identity = ad;
            usersNode = new UserNode();
        }

        public void GetUsers()
        {
            PrincipalContext oPrincipalContext = GetPrincipalContext();
        }

        private PrincipalContext GetPrincipalContext()
        {
            return new PrincipalContext(ContextType.Domain, _identity.Server, _identity.Dn, _identity.User, _identity.Password);
        }
    }
}
