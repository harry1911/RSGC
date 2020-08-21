using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCRS.Base.IRepositories
{
    public interface IUser
    {
        IEnumerable<User> ListAllUsers();
        bool GetUserById(int? id, out User user);
        User GetUserByName(string name);
        void AddUser(User user);
        void EditUser(User user);
        void DeleteUser(int id);
    }
}
