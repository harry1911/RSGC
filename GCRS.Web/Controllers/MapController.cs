using Database;
using GCRS.Base;
using GCRS.Base.APIDatabase;
using GCRS.Maps;
using GCRS.Maps.Core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;

namespace GCRS.Web.Controllers
{
    public class MapController : Controller
    {
        private readonly string mapFolder = ConfigurationManager.AppSettings["CubaMapFolder"];
        IDB apiDb;
        public MapController(IDB idb):base()
        {
            this.apiDb = idb;
        }
        // GET: Map
        public ActionResult Index()
        {
            var mapHandler = new CubaMapHandler(Server.MapPath(mapFolder));

            var url = Request.Url.AbsoluteUri;
            if (Request.Url.Query.Length > 0)
            {
                url = url.Replace(Request.Url.Query, string.Empty);
            }
            mapHandler.ProcessRequest(url);

            return new EmptyResult();
        }

        [HttpGet]
        public ActionResult GetRoute(double startX, double startY, double endX, double endY)
        {
            var pbfFile = Server.MapPath($"{mapFolder}/{ConfigurationManager.AppSettings["PbfFile"]}");
            var routeDbFile = Server.MapPath($"{mapFolder}/{ConfigurationManager.AppSettings["RouteDbFile"]}");
            var routingService = new RouteService(pbfFile, routeDbFile);

            return Json(routingService.GetRoute(startX, startY, endX, endY), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetRoutePoints()
        {
            string name = User.Identity.Name;
            var att = GetSaleRequestAttendedUnvisitedByUserName(name);
            
            List<Coordinates> res = new List<Coordinates>();
            foreach (var item in att)
                res.Add(new Coordinates(item.House.Location.YCoordinate.Value, item.House.Location.XCoordinate.Value));


            var pbfFile = Server.MapPath($"{mapFolder}/{ConfigurationManager.AppSettings["PbfFile"]}");
            var routeDbFile = Server.MapPath($"{mapFolder}/{ConfigurationManager.AppSettings["RouteDbFile"]}");
            var routingService = new RouteService(pbfFile, routeDbFile);

            return Json(routingService.GetRoute(res), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetRoutePointsRent()
        {
            string name = User.Identity.Name;
            var att = GetRentRequestAttendedUnvisitedByUserName(name);

            List<Coordinates> res = new List<Coordinates>();
            foreach (var item in att)
                res.Add(new Coordinates(item.House.Location.YCoordinate.Value, item.House.Location.XCoordinate.Value));


            var pbfFile = Server.MapPath($"{mapFolder}/{ConfigurationManager.AppSettings["PbfFile"]}");
            var routeDbFile = Server.MapPath($"{mapFolder}/{ConfigurationManager.AppSettings["RouteDbFile"]}");
            var routingService = new RouteService(pbfFile, routeDbFile);

            return Json(routingService.GetRoute(res), JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult GetPublishingsLocations()
        {
            var att = GetPublishings();
            return Json(att, JsonRequestBehavior.AllowGet);
        }

        IEnumerable<RequestSalePublishing> GetSaleRequestAttendedUnvisitedByUserName(string name)
        {
            return apiDb.GetRepo<RequestSalePublishing>().Query(x => x.Realtor,x => x.House).Where(x => x.Realtor.Name == name && x.RequestStatus != RequestStatus.Reject && x.RequestStatus != RequestStatus.Accept).ToList();//TODO: what states really
        }
        IEnumerable<RequestRentPublishing> GetRentRequestAttendedUnvisitedByUserName(string name)
        {
            return apiDb.GetRepo<RequestRentPublishing>().Query(x => x.Realtor, x => x.House).Where(x => x.Realtor.Name == name && x.RequestStatus != RequestStatus.Reject && x.RequestStatus != RequestStatus.Accept).ToList();//TODO: what states really
        }
        IEnumerable<Coordinates> GetPublishings()
        {
            List<Coordinates> res = new List<Coordinates>();
            var att=apiDb.GetRepo<SalePublishing>().Query(x => x.House).Where(x=>x.Showable).ToList();
            foreach (var item in att)
                res.Add(new Coordinates(item.House.Location.YCoordinate.Value, item.House.Location.XCoordinate.Value));

            var att2= apiDb.GetRepo<RentPublishing>().Query().Select(c => new { Publishing = c, Rooms = c.Rooms, Price = c.Price, Showable=c.Showable}).Where(x=>x.Showable);
            foreach (var item in att2)
                res.Add(new Coordinates(item.Rooms.First().House.Location.YCoordinate.Value, item.Rooms.First().House.Location.XCoordinate.Value));
            return res;
        }
    }
}