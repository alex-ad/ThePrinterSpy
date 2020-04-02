using System.Linq;


namespace ThePrinterSpyControl.Models
{
    public class Server
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public static Server AddServer(string name)
        {
            if (IsExists(name))
                return GetServer(name);
            Server server = SpyOnSpool.PrintSpyContext.Servers.Add(new Server { Name = name });
            SpyOnSpool.PrintSpyContext.SaveChanges();
            return server;
        }

        public static Server GetServer(string name) => SpyOnSpool.PrintSpyContext.Servers.FirstOrDefault(s => s.Name == name);
        public static bool IsExists(string name) => SpyOnSpool.PrintSpyContext.Servers.FirstOrDefault(s => s.Name == name) != null;
    }
}
