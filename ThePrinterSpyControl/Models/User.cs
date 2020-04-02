using System.Linq;

namespace ThePrinterSpyControl.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public static User AddUser(string name)
        {
            if (IsExists(name))
                return GetUser(name);
            User user = SpyOnSpool.PrintSpyContext.Users.Add(new User { Name = name });
            SpyOnSpool.PrintSpyContext.SaveChanges();
            return user;
        }

        public static User GetUser(string name) => SpyOnSpool.PrintSpyContext.Users.FirstOrDefault(u => u.Name == name);
        public static bool IsExists(string name) => SpyOnSpool.PrintSpyContext.Users.FirstOrDefault(u => u.Name == name) != null;
    }
}
