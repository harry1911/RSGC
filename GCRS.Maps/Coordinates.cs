using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCRS.Maps
{
    public struct Coordinates
    {
        public Coordinates(double Lat, double Long)
        {
            this.Lat = Lat;
            this.Long= Long;
        }

        public double  Lat { get; set; }
        public double Long { get; set; }
    }
}
