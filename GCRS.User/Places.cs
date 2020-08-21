using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCRS.User
{
    public class Places
    {
        public int PlacesID { get; set; }
        public PlacesType Type { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
    }

    public class PlacesType
    {
        public int PlacesTypeID { get; set; }
        public string Name { get; set; }
    }
}
