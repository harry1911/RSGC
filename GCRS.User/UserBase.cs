using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCRS.User
{
    /*
    I'am not really sure if the separation between a normal user
    and a realtor are necessary. If it is, then it needs to be
    change in the Request class.
    */
    public abstract class UserBase
    {
        public int UserBaseID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public ICollection<Privileges> Privileges { get; set; }
    }

    public class Privileges
    {
        public int PrivilegesID { get; set; }
        public string Name { get; set; }
    }
}
