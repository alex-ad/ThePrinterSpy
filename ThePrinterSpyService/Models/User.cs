using System.Linq;
using ThePrinterSpyService.Core;

namespace ThePrinterSpyService.Models
{
    public class User
    {
        public int Id { get; set; }
        public string AccountName { get; set; }
        public string FullName { get; set; }
        public string Department { get; set; }
        public string Position { get; set; }
        public string Company { get; set; }
        public string Sid { get; set; }

        public static User Add(string name)
        {
            if (IsExists(name))
                return Get(name);

            ActiveDirectory ad = new ActiveDirectory();
            if (SpyOnSpool.PrintSpyContext.Configs.Find(1)?.AdEnabled == 1)
                ad.TryGetUser(name);
            User user = SpyOnSpool.PrintSpyContext.Users.Add(new User
            {
                AccountName = name,
                FullName = ad.FullName,
                Department = ad.Department,
                Position = ad.Position,
                Company = ad.Company,
                Sid = ad.Sid
            });
            SpyOnSpool.PrintSpyContext.SaveChanges();
            return user;
        }

        public static User Get(string name) => SpyOnSpool.PrintSpyContext.Users.FirstOrDefault(u => u.AccountName == name);
        public static bool IsExists(string name) => SpyOnSpool.PrintSpyContext.Users.FirstOrDefault(u => u.AccountName == name) != null;
    }
}
