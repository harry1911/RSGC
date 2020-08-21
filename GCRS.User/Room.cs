using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GCRS.Base
{
    public class Room
    {
        public int RoomID { get; set; }
        [ForeignKey("RoomType")]
        [Required]
        [Display(Name ="Tipo")]
        public int RoomTypeID { get; set; }
        public virtual RoomType RoomType { get; set; }
        [Display(Name = "Descripción")]
        public string Description { get; set; }
        [Display(Name ="Características")]
        public virtual ICollection<RoomFeature> Features { get; set; }
        [Display(Name = "Mts. Cuadrados")]
        [Required]
        public float SquareMts { get; set; }
        [ForeignKey("House")]
        [Display(Name = "Casa")]
        public int HouseID { get; set; }
        public virtual House House { get; set; }
        //poner la casa con el atributo de la llave foranea
        public virtual ICollection<RentPublishing> RentPublishings { get; set; }
        public virtual ICollection<RequestRentPublishing> RequestRentPublishings { get; set; }
    }

    public class RoomType
    {
        public int RoomTypeID { get; set; }
        [Required]
        [Display(Name="Nombre")]
        public string Name { get; set; }
    }
}
