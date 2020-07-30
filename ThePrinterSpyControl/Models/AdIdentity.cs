using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThePrinterSpyControl.Models
{
    class AdIdentity
    {
        public string User { get; }
        public string Password { get; }
        public string Server { get; }
        public string Dn { get; }

        public AdIdentity(string server, string dn, string user, string password)
        {
            if (string.IsNullOrEmpty(server) || string.IsNullOrEmpty(user) || string.IsNullOrEmpty(password) ||
                string.IsNullOrEmpty(dn)) throw new ArgumentNullException();

            User = user;
            Password = password;
            Server = server;
            Dn = dn;
        }
    }
}
