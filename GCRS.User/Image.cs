using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GCRS.Base
{
    public class Image
    {
        public int ImageID { get; set; }
        [Display(Name = "Ruta")]
        public string Path { get; set; }
        [Display(Name = "Publicación")]
        public virtual Publishing Publishing { get; set; }
        [Display(Name = "Solicitud de Publicación")]
        public virtual RequestPublishing RequestPublishing { get; set; }
        [ForeignKey(nameof(Publishing))]
        public int? PublishingID { get; set; }
        [ForeignKey(nameof(RequestPublishing))]
        public int? RequestPublishingID { get; set; }
    }

}
