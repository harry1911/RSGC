using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GCRS.Base
{
    //public class GroupUser
    //{
    //    public int GroupUserID { get; set; }
    //    public string GroupName { get; set; }
    //    //public virtual ICollection<User> Users { get; set; }
    //    //public virtual ICollection<Privileges> Privileges { get; set; }
    //}

    public class Privileges
    {
        public int PrivilegesID { get; set; }
        [Display(Name = "Nombre")]
        [Required]
        public string Name { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}
