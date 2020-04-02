using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThePrinterSpyControl.Models
{
    class ServerCollection
    {
        private static int _id = 0;

        private ObservableCollection<Server> Servers { get; }

        public ServerCollection()
        {
            Servers = new ObservableCollection<Server>();
        }

        public Server Add(string name)
        {
            if (IsExists(name))
                return GetServer(name);
            _id++;
            Server server = new Server(_id, name);
            Servers.Add(server);
            return server;
        }

        private Server GetServer(string name) => Servers.FirstOrDefault(s => s.Name == name);

        private bool IsExists(string name) => Servers.FirstOrDefault(s => s.Name == name) != null;
    }
}
