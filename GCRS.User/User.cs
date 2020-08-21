using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace GCRS.Base
{
    public class User
    {
        public int UserID { get; set; }
        [Display(Name="Nombre")]
        [Required]
        public string Name { get; set; }
        [Display(Name = "Coreo Electrónico")]
        public string Email { get; set; }
        [Display(Name = "Teléfono")]
        public string Phone { get; set; }
        [Display(Name = "Contraseña")]
        [Required]
        public string Pasword { get; set; }
        [ForeignKey(nameof(Privileges))]
        [Display(Name="Rol")]
        [Required]
        public int PrivilegesID { get; set; }
        public virtual Privileges Privileges { get; set; }
        public virtual ICollection<House> Houses { get; set; }
    }
}
