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
using GCRS.Base.APIDatabase;
using GCRS.Base.IRepositories;
using GCRS.Services;

namespace GCRS.Web.Controllers.DataManager
{
    public class RoomsController : Controller
    {
        RoomService apiDb;
        RoomFeaturesService roomsFeaturesRepo ;
        RoomTypeService roomsTypesRepo ;

        public RoomsController(IDB idb):base()
        {
            apiDb = new RoomService(idb);
            roomsFeaturesRepo = new RoomFeaturesService(idb);
        }

        // GET: Rooms
        public ActionResult Index()
        {
            return View(apiDb.ListAllRooms());
        }

        // GET: Rooms/Details/5
        public ActionResult Details(int? id)
        {
            Room room = new Room();
            if (apiDb.GetRoomById(id, out room))
                if (room != null)
                    return View(room);
                else return HttpNotFound();
            else return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        // GET: Rooms/Create
        public ActionResult Create()
        {
            var roomsList = GetRoomTypesSelectList();
            ViewBag.RoomTypeID = roomsList;

            return View();
        }

        // POST: Rooms/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "RoomID,SquareMts,Description,RoomTypeID")] Room room)
        {
            if (ModelState.IsValid)
            {
                apiDb.AddRoom(room);
                return RedirectToAction("Index");
            }

            var roomsList = GetRoomTypesSelectList();
            ViewBag.RoomTypeID = roomsList;

            return View(room);
        }

        // GET: Rooms/Edit/5
        public ActionResult Edit(int? id)
        {
            Room room = new Room();
            if (apiDb.GetRoomById(id, out room))
                if (room != null)
                {
                    var roomsList = GetRoomTypesSelectList(room.RoomTypeID);
                    ViewBag.RoomTypeID = roomsList;
                    var featuresList = GetRoomFeaturesSelectList();
                    ViewBag.RoomFeatureID = featuresList;
                    return View(room);
                }
                else return HttpNotFound();
            else return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        private SelectList GetRoomFeaturesSelectList(object selectedRoomFeature = null)
        {
            var roomFeaturesQuery = roomsFeaturesRepo.GetRoomFeaturesSelectList();
            return new SelectList(roomFeaturesQuery, "RoomFeatureID", "Name", selectedRoomFeature);
        }
        private SelectList GetRoomTypesSelectList(object selectedRoomType = null)
        {
            var roomTypesQuery = roomsTypesRepo.GetRoomTypesSelectList();
            return new SelectList(roomTypesQuery, "RoomTypeID", "Name", selectedRoomType);
        }
        // POST: Rooms/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "RoomID,SquareMts, Description,RoomTypeID")] Room room)
        //{
        //    if (apiDb.EditRoom(room, ModelState))
        //        return RedirectToAction("Index");

        //    var roomsList = room.RoomType != null ? apiDb.GetRoomTypesSelectList(room.RoomType.RoomTypeID) : apiDb.GetRoomTypesSelectList();
        //    ViewBag.RoomTypeID = roomsList;

        //    return View(room);
        //}

        // POST: Rooms/Edit/5
        //[HttpPost]
        //public ActionResult Edit(Room room)
        //{
        //    //Room db_room = new Room();
        //    //apiDb.GetRoomById(room.RoomID, out db_room);

        //    //JsonResult j = new JsonResult();
        //    //j.Data = new
        //    //{
        //    //    success = true,
        //    //    redirects = false
        //    //};
        //    ////if (ModelState.IsValid)
        //    //{
        //    //    db_room.Description = room.Description;
        //    //    db_room.RoomTypeID = room.RoomTypeID;
        //    //    db_room.SquareMts = room.SquareMts;

        //    //    db_room.Features = null;
        //    //    apiDb.EditRoom(db_room);
        //    //    foreach (var feature in db_room.Features)
        //    //        apiDb.db.db.Entry(feature).State = EntityState.Modified;
        //    //    apiDb.db.db.SaveChanges();
        //    //    apiDb.GetRoomById(room.RoomID, out db_room);
        //    //    db_room.Features = new List<RoomFeature>();
        //    //    if (room.Features != null)
        //    //    {
        //    //        apiDb.db.db.Entry(db_room).State = EntityState.Modified;
        //    //        foreach (var feature in room.Features)
        //    //            db_room.Features.Add(apiDb.db.RoomFeatures.Find(feature.RoomFeatureID));
        //    //        apiDb.db.db.SaveChanges();
        //    //    }

        //    //    if (db_room.Features != null)
        //    //        foreach (var feature in db_room.Features)
        //    //            apiDb.db.Entry(feature).State = EntityState.Modified;
        //    //    apiDb.db.db.SaveChanges();

        //    //    j.Data = new { success = true, redirects = true };
        //    //    return j;
        //    //}
        //}

        //GET: Rooms/Delete/5
        public ActionResult Delete(int? id)
        {
            Room room = new Room();
            if (apiDb.GetRoomById(id, out room))
                if (room != null)
                    return View(room);
                else return HttpNotFound();
            else return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        // POST: Rooms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            apiDb.DeleteRoom(id);
            return RedirectToAction("Index");
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
