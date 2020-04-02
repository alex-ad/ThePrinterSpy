using System.Linq;

namespace ThePrinterSpyControl.Models
{
    public class Computer
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public static Computer AddComputer(string name)
        {
            if (IsExists(name))
                return GetComputer(name);
            Computer computer =  SpyOnSpool.PrintSpyContext.Computers.Add(new Computer{Name = name});
            SpyOnSpool.PrintSpyContext.SaveChanges();
            return computer;
        }

        public static Computer GetComputer(string name) => SpyOnSpool.PrintSpyContext.Computers.FirstOrDefault(c => c.Name == name);
        public static bool IsExists(string name) => SpyOnSpool.PrintSpyContext.Computers.FirstOrDefault(c => c.Name == name) != null;
    }
}
