using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ThePrinterSpyControl.DataBase;
using ThePrinterSpyControl.Models;

namespace ThePrinterSpyControl.ModelBuilders
{
    public class ServersCollection
    {
        private static ObservableCollection<ServerNode> Servers { get; }
        private readonly DBase _base;

        static ServersCollection()
        {
            Servers = new ObservableCollection<ServerNode>();
        }

        public ServersCollection()
        {
            _base = new DBase();
        }

        public void GetAll()
        {
            List<Server> servers;
            try
            {
                servers = _base.GetServersList().Result;
            }
            catch (Exception ex)
            {
                throw new Exception("DB: table 'Servers' is missing or empty", ex.InnerException);
            }

            if (servers == null || !servers.Any()) return;

            Servers.Clear();

            foreach (var p in servers)
            {
                Servers.Add(new ServerNode
                {
                    Id = p.Id,
                    Name = p.Name
                });
            }
        }

        public string GetServer(int id) => Servers.FirstOrDefault(x => x.Id == id)?.Name;
    }
}
