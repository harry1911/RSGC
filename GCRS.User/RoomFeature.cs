using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GCRS.Base
{
    public class RoomFeature
    {
        public int RoomFeatureID { get; set; }
        [Display(Name = "Nombre")]
        [Required]
        public string Name { get; set; }
        [Display(Name = "Descripción")]
        public string Description { get; set; }
        public virtual ICollection<Room> Rooms { get; set; }
    }
}
