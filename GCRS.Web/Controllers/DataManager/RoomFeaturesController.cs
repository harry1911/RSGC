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
    public class RoomFeaturesController : Controller
    {
        RoomFeaturesService roomFeatureRepo;

        public RoomFeaturesController(IDB idb):base()
        {
            roomFeatureRepo = new RoomFeaturesService(idb);
        }
        // GET: RoomFeatures
        public ActionResult Index()
        {
            return View(roomFeatureRepo.ListAllRoomFeatures());
        }

        // GET: RoomFeatures/Details/5
        public ActionResult Details(int? id)
        {
            RoomFeature feature = new RoomFeature();
            if (roomFeatureRepo.GetRoomFeatureById(id, out feature))
                if (feature != null)
                    return View(feature);
                else return HttpNotFound();
            else return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        // GET: RoomFeatures/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: RoomFeatures/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "RoomFeatureID,Name,Description")] RoomFeature feature)
        {
            if (ModelState.IsValid) {
                roomFeatureRepo.AddRoomFeature(feature);
                return RedirectToAction("Index");
            }

            return View(feature);
        }

        // GET: RoomFeatures/Edit/5
        public ActionResult Edit(int? id)
        {
            RoomFeature feature = new RoomFeature();
            if (roomFeatureRepo.GetRoomFeatureById(id, out feature))
                if (feature != null)
                    return View(feature);
                else return HttpNotFound();
            else return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        // POST: RoomFeatures/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "RoomFeatureID,Name,Description")] RoomFeature feature)
        {
            if (ModelState.IsValid) {
                roomFeatureRepo.EditRoomFeature(feature);
                return RedirectToAction("Index");
            }
            return View(feature);
        }

        // GET: RoomFeatures/Delete/5
        public ActionResult Delete(int? id)
        {
            RoomFeature feature = new RoomFeature();
            if (roomFeatureRepo.GetRoomFeatureById(id, out feature))
                if (feature != null)
                    return View(feature);
                else return HttpNotFound();
            else return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        // POST: RoomFeatures/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            roomFeatureRepo.DeleteRoomFeature(id);
            return RedirectToAction("Index");
        }

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        roomFeatureRepo.db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}
    }
}
