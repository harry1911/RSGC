using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Database;
using GCRS.Base;
using System.Data.Entity.Spatial;
using GCRS.Base.IRepositories;
using GCRS.Services;
using GCRS.Base.APIDatabase;

namespace GCRS.Web.Controllers.DataManager
{
    public class HousesController : Controller
    {
        HouseService apiDb;
        RoomService roomRepo;
        UserService userRepo ;
        FeatureService featureRepo ;
        RoomFeaturesService roomsFeaturesRepo ;
        RoomTypeService roomTypesRepo ;
        IDB idb;

        public HousesController(IDB idb):base()
        {
            apiDb = new HouseService(idb);
            roomRepo = new RoomService(idb);
            userRepo = new UserService(idb);
            featureRepo = new FeatureService(idb);
            roomsFeaturesRepo = new RoomFeaturesService(idb);
            roomTypesRepo = new RoomTypeService(idb);
            this.idb = idb;
        }

        private DbGeometry GetLocationFromString(string coord)
        {
            return DbGeometry.FromText("POINT (" + coord + ")", 4326);
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
        // GET: Houses
        public ActionResult Index()
        {
            var houses = apiDb.ListAllHouses();
            var dict = new Dictionary<int, string>();
            foreach (var h in houses)
                dict.Add(h.HouseID,BuildRoomsData(apiDb.GetRoomsData(h)));
            ViewBag.Info = dict;
            return View(houses);
        }
        //GET:MyHousesList
        [Authorize(Roles = "Client")]
        public ActionResult MyHousesList()
        {
            var houses = apiDb.ListAllHousesByUser(User.Identity.Name);
            var dict = new Dictionary<int, string>();
            foreach (var h in houses)
                dict.Add(h.HouseID,BuildRoomsData(apiDb.GetRoomsData(h)));
            ViewBag.Info = dict;
            return View(houses);
        }
        // GET: Houses/Details/5
        public ActionResult Details(int? id)
        {
            House house = new House();
            if (apiDb.GetHouseById(id, out house))
                if (house != null)
                {
                    ViewBag.Info = BuildRoomsData(apiDb.GetRoomsData(house));
                    return View(house);
                }
                else return HttpNotFound();
            else return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        // GET: Houses/Create
        public ActionResult Create()
        {
            var features = GetFeaturesSelectList();
            ViewBag.FeatureID = features;
            var roomsList = GetRoomTypesSelectList();
            ViewBag.RoomTypeID = roomsList;
            var roomFeatures = GetRoomFeaturesSelectList();
            ViewBag.RoomFeatureID = roomFeatures;
            return View();
        }
        private SelectList GetRoomTypesSelectList(object selectedRoomType = null)
        {
            var roomTypesQuery = roomTypesRepo.GetRoomTypesSelectList();
            return new SelectList(roomTypesQuery, "RoomTypeID", "Name", selectedRoomType);
        }
        private SelectList GetFeaturesSelectList(object selectedFeature = null)
        {
            var featuresQuery=featureRepo.GetFeaturesSelectList();
            return new SelectList(featuresQuery, "FeatureID", "Name", selectedFeature);
        }
        private SelectList GetRoomFeaturesSelectList(object selectedRoomFeature = null)
        {
            var roomFeaturesQuery = roomsFeaturesRepo.GetRoomFeaturesSelectList();
            return new SelectList(roomFeaturesQuery, "RoomFeatureID", "Name", selectedRoomFeature);

        }

        [HttpPost]
        public ActionResult Create(House House, string Coordinates)
        {
            JsonResult j = new JsonResult();
            var rooms = roomRepo.CreateRooms(House.Rooms);
            var features = featureRepo.GetFeatures(House.Features);
            House.Rooms = null;
            House.Features = null;
            idb.GetRepo<House>().Add(House);
            House.Location = GetLocationFromString(Coordinates);
            House.Rooms = rooms;
            House.Features = features;
            House.OwnerID = userRepo.GetUserByName(User.Identity.Name).UserID;
            idb.SaveChanges();
            //if (apiDb.SaveChanges(ModelState))
            j.Data = new { success = true, redirects = true };
            //else j.Data = new { success = true, redirects = false };

            return j;
        }

        // GET: Houses/Edit/5
        public ActionResult Edit(int? id)
        {
            House house = new House();
            if (apiDb.GetHouseById(id, out house))
                if (house != null)
                    return View(house);
                else return HttpNotFound();
            else return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        // POST: Houses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "HouseID,Description,SquareMts,WaterFreq")] House house)
        {
            if (ModelState.IsValid)
            {
                apiDb.EditHouse(house);
                return RedirectToAction("MyHousesList");
            }

            return View(house);
        }

        // GET: Houses/Delete/5
        public ActionResult Delete(int? id)
        {
            House house = new House();
            if (apiDb.GetHouseById(id, out house))
                if (house != null)
                    return View(house);
                else return HttpNotFound();
            else return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        // POST: Houses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            apiDb.DeleteHouse(id);
            return RedirectToAction("MyHousesList");
        }

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        apiDb.db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}
    }
}
