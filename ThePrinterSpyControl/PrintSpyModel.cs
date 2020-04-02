using ThePrinterSpyControl.Models;

namespace ThePrinterSpyControl
{
    using System;
    using System.Data.Entity;
    using System.Linq;

    public class PrintSpyModel : DbContext
    {
        public PrintSpyModel()
            : base("name=PrintSpy")
        {
        }

        public virtual DbSet<Printer> Printers { get; set; }
        public virtual DbSet<Computer> Computers { get; set; }
        public virtual DbSet<Server> Servers { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<PrintData> PrintDatas { get; set; }
    }
}