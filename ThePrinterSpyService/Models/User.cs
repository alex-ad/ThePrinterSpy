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

        public static User Add(string name, string sid)
        {
            if (IsExists(sid))
                return Get(sid);

            ActiveDirectory ad = new ActiveDirectory();
            if (SpyOnSpool.PrintSpyContext.Configs.Find(1)?.AdEnabled == 1)
                ad.TryGetUser(sid);
            User user = SpyOnSpool.PrintSpyContext.Users.Add(new User
            {
                AccountName = name,
                FullName = ad.FullName ?? string.Empty,
                Department = ad.Department ?? string.Empty,
                Position = ad.Position ?? string.Empty,
                Company = ad.Company ?? string.Empty,
                Sid = sid
            });
            SpyOnSpool.PrintSpyContext.SaveChanges();
            return user;
        }

        public static User Get(string sid) => SpyOnSpool.PrintSpyContext.Users.FirstOrDefault(u => u.Sid == sid);
        public static bool IsExists(string sid) => SpyOnSpool.PrintSpyContext.Users.FirstOrDefault(u => u.Sid == sid) != null;
    }
}
