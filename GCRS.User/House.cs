using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GCRS.Base
{
    public class House
    {
        public int HouseID { get; set; }
        [Display(Name ="Características")]
        public virtual ICollection<Feature> Features { get; set; }
        public virtual ICollection<Room> Rooms { get; set; }
        public virtual ICollection<SalePublishing> SalePublishings { get; set; }
        public virtual ICollection<RequestRentPublishing> RequestRentPublishings { get; set; }
        public virtual ICollection<RequestSalePublishing> RequestSalePublishings { get; set; }
        [Required]
        [Display(Name ="Dueño")]
        [ForeignKey("Owner")]
        public int OwnerID { get; set; }
        [Display(Name = "Dueño")]
        public virtual User Owner { get; set; }
        [Required]
        [Display(Name ="Dirección")]
        public string Address { get; set; }
        //public virtual Place Place { get; set; }
        [Display(Name = "Descripción")]
        public string Description { get; set; }
        public DbGeometry Location { get; set; }
        [Display(Name = "Mts. Cuadrados")]
        public float SquareMts { get; set; }
        [Display(Name = "Frecuencia de Agua")]
        public int WaterFreq { get; set; }
    }
}
