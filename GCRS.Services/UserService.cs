using GCRS.Base;
using GCRS.Base.APIDatabase;
using GCRS.Base.IRepositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCRS.Services
{
    public class UserService
    {
        private IDB idb;
        public UserService(IDB idb)
        {
            this.idb = idb;
        }

        public IEnumerable<User> ListAllUsers()
        {
            return idb.GetRepo<User>().Query(x=>x.Privileges).ToList();
        }
        public bool GetUserById(int? id, out User user)
        {
            user = null;
            if (id != null)
                user = idb.GetRepo<User>().Query(x => x.Privileges).Where(x => x.UserID == id).Single();
            return id != null;
        }
        public User GetUserByName(string name)
        {
            var user = idb.GetRepo<User>().Query(x => x.Privileges).Where(x => x.Name == name);
            if (user != null && user.Count() > 0)
                return user.Single();
            return null;
        }
        public void AddUser(User user)
        {
            idb.GetRepo<User>().Add(user);
            idb.SaveChanges();
        }
        public void EditUser(User user)
        {
            idb.GetRepo<User>().Edit(user);
            idb.SaveChanges();
        }
        public void DeleteUser(int id)
        {
            idb.GetRepo<User>().Remove(id);
            idb.SaveChanges();
        }

    }
}
