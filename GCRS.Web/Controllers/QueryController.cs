using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Database;
using GCRS.Base;
using Newtonsoft.Json;
using GCRS.Services;
using GCRS.Base.APIDatabase;

namespace GCRS.Web.Controllers
{
    public class QueryController : Controller
    {
        //private DB db = new DB();
        //AppDB apiDb = new AppDB();
        HouseService houseService;
        IDB idb;

        public QueryController(IDB idb)
        {
            this.idb = idb;
            houseService = new HouseService(idb);
        }
        // GET: Query
        public ActionResult Index(string sqrtMtMax, string sqrtMtMin, string address, string bedrmsMax, string bedrmsMin, string features, string bathMax, string bathMin, string priceMax, string priceMin,string poligons)
        {
            ViewBag.TableData = QueryPublishing(sqrtMtMax, sqrtMtMin, address, bedrmsMax, bedrmsMin, features, bathMax, bathMin, priceMax, priceMin,poligons);
            ViewBag.FilterOpt = new string[] { sqrtMtMax, sqrtMtMin, address, bedrmsMax, bedrmsMin, features, bathMax, bathMin, priceMax, priceMin };
            FillFeaturesDropDownList();
            return View("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IndexPost(string sqrtMtMax, string sqrtMtMin, string address, string bedrmsMax, string bedrmsMin, string features, string bathMax, string bathMin, string priceMax, string priceMin,string poligons)
        {
            return Index(sqrtMtMax, sqrtMtMin, address, bedrmsMax, bedrmsMin, features, bathMax, bathMin, priceMax, priceMin,poligons);
        }

        class FeatureListElem
        {
            public int FeatureID { get; set; }
            public string Name{ get; set; }
        }

        private void FillFeaturesDropDownList(object selectedLabel = null)
        {
            var featuresQuery = from d in idb.GetRepo<Feature>().Query() orderby d.Name select new FeatureListElem{ FeatureID = d.FeatureID, Name = d.Name };
            var roomType = from d in idb.GetRepo<RoomType>().Query() orderby d.Name select new FeatureListElem{ FeatureID = d.RoomTypeID, Name = d.Name };
            var roomFeatures= from d in idb.GetRepo<RoomFeature>().Query() orderby d.Name select new FeatureListElem { FeatureID = d.RoomFeatureID, Name = d.Name };

            var list = featuresQuery.Union(roomType).Union(roomFeatures);

            ViewBag.Features = new SelectList(list.Select(c => new { FeatureID=c.FeatureID, Name=c.Name.ToString() }), "FeatureID", "Name", selectedLabel);
        }

        public ActionResult GetDetails(int id)
        {
            var publish = idb.GetRepo<Publishing>().Query(c => c.Images, c => c.User).Where(c => c.PublishingID == id).Single();
            ViewBag.Id = publish.PublishingID;
            if (publish is Base.SalePublishing)
            {
                var salePublish = (GCRS.Base.SalePublishing)publish;
                ViewBag.Info = BuildRoomsData(houseService.GetRoomsData(salePublish.House));
                return View("~/Views/Publishing/SaleDetails.cshtml", publish);
            }
            else if (publish is Base.RentPublishing)
            {
                return View("~/Views/Publishing/RentDetails.cshtml", publish);
            }
            else
            {
                throw new Exception("No se x q llegaria aki");
            }
        }

        private Dictionary<string, string>[] QueryPublishing(string sqrtMtMax, string sqrtMtMin, string address, string bedrmsMax, string bedrmsMin, string features, string bathMax, string bathMin, string priceMax, string priceMin,string poligons)
        {
            //TODO: Find the magic way of appendit to the form data to aoid this parse...
            double[][][] pol;
            if (poligons != null) pol = JsonConvert.DeserializeObject<double[][][]>(poligons);
            else pol = new double[0][][];
            List<DbGeometry> pols = new List<DbGeometry>();
            for (int i = 0; i < pol.Length; i++)
            {
                string points = "";
                for (int j = 0; j < pol[i].Length; j++)
                {
                    points += pol[i][j][0] + " " + pol[i][j][1];
                    if (j != pol[i].Length - 1) points += ", ";
                }
                pols.Add(DbGeometry.PolygonFromText("POLYGON((" + points + "))", 4326));
            }

            int minMt = 0;
            int maxMt = int.MaxValue;
            int minBed = 0;
            int maxBed = int.MaxValue;
            int minBath = 0;
            int maxBath = int.MaxValue;
            float minPrice = 0;
            float maxPrice = float.MaxValue;
            address = "";

            SortedSet<string> featuresSet = new SortedSet<string>(new List<string>());


            if (sqrtMtMax != null)
            {
                minMt = sqrtMtMin == "" ? 0 : int.Parse(sqrtMtMin);
                maxMt = sqrtMtMax == "" ? int.MaxValue : int.Parse(sqrtMtMax);
                minBed = bedrmsMin == "" ? 0 : int.Parse(bedrmsMin);
                maxBed = bedrmsMax == "" ? int.MaxValue : int.Parse(bedrmsMax);
                minBath = bathMin == "" ? 0 : int.Parse(bathMin);
                maxBath = bathMax == "" ? int.MaxValue : int.Parse(bathMax);
                minPrice = priceMin == "" ? 0 : float.Parse(priceMin);
                maxPrice = priceMax == "" ? float.MaxValue : float.Parse(priceMax);

                featuresSet = new SortedSet<string>((features != "") ? features.Split(';').Select(x => x.ToLower()) : new List<string>());
            }
            
            return QueryPublishing(maxMt, minMt, address, maxBed, minBed, featuresSet, maxBath, minBath, maxPrice, minPrice,pols);
        }

        private Dictionary<string, string>[] QueryPublishing(int sqrtMtMax, int sqrtMtMin, string address, int bedrmsMax, int bedrmsMin, IEnumerable<string> features, int bathMax, int bathMin, float priceMax, float priceMin,List<DbGeometry> poligons)
        {
            var houses = idb.GetRepo<SalePublishing>().Query().Select(c => new { Publishing = c, House = c.House, Price = c.Price})
               .Where(x => (address == "" || x.House.Address.ToLower().Contains(address))
                && (poligons.Count == 0 || poligons.Any(z => z.Contains(x.House.Location))) &&
                    x.Publishing.Showable &&
                    x.House.SquareMts >= sqrtMtMin &&
                    x.House.SquareMts <= sqrtMtMax &&
                    x.House.Rooms.Count >= bedrmsMin &&
                    x.House.Rooms.Count <= bedrmsMax &&
                    x.Price >= priceMin &&
                    x.Price <= priceMax &&
                    x.House.Rooms.Count >= bathMin &&//hay que poner para que sean banos
                    x.House.Rooms.Count <= bathMax
                    &&
                    (features.Count() == 0 || x.House.Features.Select(a => a.Name.ToLower()).ToList().Intersect(features)
                        .Union(features.Intersect(x.House.Rooms.Select(a => a.RoomType.Name.ToLower())))
                        .Union(features.Intersect(x.House.Rooms.SelectMany(c => c.Features).Select(c => c.Name.ToLower()))).Count()
                        == features.Count())
                        ).ToList();

            var rooms = idb.GetRepo<RentPublishing>().Query().Select(c => new { Publishing=c, Rooms = c.Rooms, Price = c.Price }).
                Where(x =>(x.Rooms.Any(c => c.House.Address.ToLower().Contains(address)) )&&
                x.Publishing.Showable &&
                x.Price >= priceMin &&
                x.Price <= priceMax &&
                x.Rooms.Any(c => c.SquareMts >= sqrtMtMin) &&
                x.Rooms.Any(c => c.SquareMts <= sqrtMtMax) &&
                (features.Count() == 0 || x.Rooms.SelectMany(c=>c.Features).Select(q=>q.Name.ToLower()).Intersect(features)
                    .Union(features.Intersect(x.Rooms.Select(a => a.RoomType.Name.ToLower()))).Count()
                    == features.Count())).ToList();

            Dictionary<string, string>[] list = new Dictionary<string, string>[houses.Count() + rooms.Count()];
            for (int i = 0; i < houses.Count(); i++)
            {
                list[i] = new Dictionary<string, string>();

                list[i].Add("id", houses[i].Publishing.PublishingID.ToString());

                list[i].Add("Venta/Alquiler", "Venta");

                list[i].Add("Descripción", houses[i].Publishing.Description);

                list[i].Add("Precio de Venta", houses[i].Price.ToString());

                //list[i].Add("Images", houses[i].Publishing.Images.Select(c => c.Path).First());
            }

            for (int i = 0; i < rooms.Count(); i++)
            {
                list[houses.Count() + i] = new Dictionary<string, string>();

                list[houses.Count() + i].Add("id", rooms[i].Publishing.PublishingID.ToString());

                list[houses.Count() + i].Add("Venta/Alquiler", "Alquiler");

                list[houses.Count() + i].Add("Descripción", rooms[i].Publishing.Description);

                list[houses.Count() + i].Add("Precio de Venta", rooms[i].Publishing.Price.ToString());

                //list[houses.Count() + i].Add("Images", rooms[i].Publishing.Images.Select(c => c.Path).First());
            }

            return list;
        }

        private string BuildRoomsData(Dictionary<string, int> roomsData)
        {
            string data = "";
            int i = 0;
            foreach (var key in roomsData.Keys)
            {
                i++;

                if (i < roomsData.Keys.Count)
                    data += roomsData[key] + " " + key + (roomsData[key] > 1 ? "s, " : ", ");
                else
                    data += roomsData[key] + " " + key + (roomsData[key] > 1 ? "s" : "");

            }

            return data;
        }
    }
}