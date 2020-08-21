using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GCRS.Base
{
    public class Registry
    {
        [Key]
        public int RegistryID { get; set; }
        [Display(Name = "Cliente")]
        public User Client { get; set; }
    }

    public class RentRegistry : Registry
    {
        [Display(Name = "Inicio")]
        public DateTime Start { get; set; }
        [Display(Name = "Fin")]
        public DateTime Finish { get; set; }
        [Display(Name = "Precio por noche")]
        public double RentPrice { get; set; }
        //[ForeignKey(nameof(Publishing))]
        //public int PublishingId { get; set; }
        [Display(Name = "Publicación")]
        public virtual RentPublishing RentPublishing { get; set; } 
        public virtual ICollection<Service> Services { get; set; }
    }

    public class SellRegistry : Registry
    {
        //public DateTime SaleDate { get; set; }
        [Display(Name = "Publicación")]
        public virtual SalePublishing SellPublishing { get; set; }
        [Display(Name = "Precio")]
        public double Price { get; set; }
    }
}
