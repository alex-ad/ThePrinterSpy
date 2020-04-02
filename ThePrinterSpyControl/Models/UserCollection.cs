using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThePrinterSpyControl.Models
{
    class UserCollection
    {
        public int LastId => _id;
        private static int _id = 0;

        private ObservableCollection<User> Users { get; }

        public UserCollection()
        {
            Users = new ObservableCollection<User>();
        }

        public User Add(string name)
        {
            if (IsExists(name))
                return GetUser(name);
            _id++;
            User user = new User(_id, name);
            Users.Add(user);
            return user;
        }

        public User GetUser(string name) => Users.FirstOrDefault(u => u.Name == name);

        public bool IsExists(string name) => Users.FirstOrDefault(u => u.Name == name) != null;
    }
}
