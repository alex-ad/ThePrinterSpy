using System.Data.Entity;
using ThePrinterSpyService.Core;

namespace ThePrinterSpyService.Models
{
    public class PrintSpyDbContext : DbContext
    {
        public PrintSpyDbContext()
            : base(new DataBaseConnection().Get(), true)
        {
        }

        public virtual DbSet<Printer> Printers { get; set; }
        public virtual DbSet<Computer> Computers { get; set; }
        public virtual DbSet<Server> Servers { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<PrintData> PrintDatas { get; set; }
        public virtual DbSet<Config> Configs { get; set; }
    }
}