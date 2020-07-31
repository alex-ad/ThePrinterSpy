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

        public AdIdentity(string server, string user, string password)
        {
            if (string.IsNullOrEmpty(server) || string.IsNullOrEmpty(user) || string.IsNullOrEmpty(password)) throw new ArgumentNullException();

            User = user;
            Password = password;
            Server = server;
        }
    }
}
