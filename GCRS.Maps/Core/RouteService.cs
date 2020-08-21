using Itinero;
using Itinero.IO.Osm;
using Itinero.LocalGeo;
using Itinero.Osm.Vehicles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GCRS.Maps.Core
{
    /// <summary>
    ///     Routing service, use Itinero for routing between two points.
    /// </summary>
    public class RouteService
    {
        public class Path
        {
            public IList<Coordinate> PathNodes { get; set; }
        }

        private Router _router;
        private Itinero.Profiles.Vehicle _vehicle;
        private Itinero.Profiles.Profile _profile;
        private int _searchDistance = 5000;

        /// <summary>
        ///     If <paramref name="routerDbPath"/> exists, load routing data, otherwise create routing data from pbf file.
        /// </summary>
        /// <param name="pbfPath">PBF file, route info from Open Street Map project</param>
        /// <param name="routerDbPath">ROUTEDB file, serialized route info from Itinero</param>
        public RouteService(string pbfPath, string routerDbPath)
        {
            _vehicle = Vehicle.Car;
            _profile = _vehicle.Fastest();

            if (File.Exists(routerDbPath))
            {
                Load(routerDbPath);
            }
            else if(File.Exists(pbfPath))
            {
                Create(pbfPath, routerDbPath);
            }
            else
            {
                throw new ArgumentException("PBF and ROUTERDB files does not exists");
            }
        }

        /// <summary>
        ///     Return routing info from starting point to final point
        /// </summary>
        /// <param name="startX">Start point longitude</param>
        /// <param name="startY">Start point latitude</param>
        /// <param name="endX">End point longitude</param>
        /// <param name="endY">End point latitude</param>
        /// <returns>List of points that represents the path from start to end</returns>
        public Path GetRoute(double startX, double startY, double endX, double endY)
        {
            // get nearest points
            var resolved1 = _router.Resolve(_profile, new Coordinate((float)startY, (float)startX), _searchDistance);
            var resolved2 = _router.Resolve(_profile, new Coordinate((float)endY, (float)endX), _searchDistance);

            // calculate route.
            var route = _router.Calculate(_profile, resolved1, resolved2);

            var path = new Path
            {
                PathNodes = new List<Coordinate>()
            };
            foreach (var o in route.ToList())
            {
                var loc = o.Location();
                path.PathNodes.Add(loc);
            };

            return path;
        }

        /// <summary>
        ///     Return routing info from starting point to final point
        /// </summary>
        /// <param name="locations">List of points</param>
        /// <returns>List of points that represents the path from start to end</returns>
        public Path GetRoute(List<GCRS.Maps.Coordinates> locations)
        {
            //get nearest points
            var paths = locations.Select(l => _router.Resolve(_profile, new Coordinate((float)l.Lat, (float)l.Long), _searchDistance)).ToArray();

            //calculate route.
            var path = new Path
            {
                PathNodes = new List<Coordinate>()
            };
            if (paths.Count() >= 2)
            {
                try
                {
                    var route = _router.Calculate(_profile, paths);

                    foreach (var o in route.ToList())
                    {
                        var loc = o.Location();
                        path.PathNodes.Add(loc);
                    };
                    return path;

                }
                catch (Exception) { }
            }

            foreach (var item in locations)
               path.PathNodes.Add(new Coordinate((float)item.Lat, (float)item.Long));
        
            return path;

        }

    private void Create(string pbfPath, string routerDbPath)
        {
            // load some routing data and build a routing network.
            var routerDb = new RouterDb();
            using (var stream = new FileInfo(pbfPath).OpenRead())
            {
                // create the network for cars only.
                routerDb.LoadOsmData(stream, Vehicle.Car);
            }

            // write the routerdb to disk.
            using (var stream = new FileInfo(routerDbPath).Open(FileMode.Create))
            {
                routerDb.Serialize(stream);
            }
            _router = new Router(routerDb);
        }

        private void Load(string mapFile)
        {
            RouterDb routerDb;
            using (var stream = File.OpenRead(mapFile))
            {
                routerDb = RouterDb.Deserialize(stream);
            }
            _router = new Router(routerDb);
        }
    }
}
