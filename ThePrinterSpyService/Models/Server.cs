using System.Linq;
using ThePrinterSpyService.Core;


namespace ThePrinterSpyService.Models
{
    public class Server
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public static Server Add(string name)
        {
            if (IsExists(name))
                return Get(name);
            Server server = SpyOnSpool.PrintSpyContext.Servers.Add(new Server { Name = name });
            SpyOnSpool.PrintSpyContext.SaveChanges();
            return server;
        }

        private static Server Get(string name) => SpyOnSpool.PrintSpyContext.Servers.FirstOrDefault(s => s.Name == name);

        private static bool IsExists(string name) => SpyOnSpool.PrintSpyContext.Servers.FirstOrDefault(s => s.Name == name) != null;
    }
}
