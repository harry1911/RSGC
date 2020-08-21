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
    public class RoomTypesController : Controller
    {
        RoomTypeService apiDb;

        public RoomTypesController(IDB apiDb)
        {
            this.apiDb = new RoomTypeService(apiDb);
        }

        // GET: RoomTypes
        public ActionResult Index()
        {
            return View(apiDb.ListAllRoomTypes());
        }

        // GET: RoomTypes/Details/5
        public ActionResult Details(int? id)
        {
            RoomType type = new RoomType();
            if (apiDb.GetRoomTypeById(id, out type))
                if (type != null)
                    return View(type);
                else return HttpNotFound();
            else return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        // GET: RoomTypes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: RoomTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "RoomTypeID,Name")] RoomType roomType)
        {
            if (ModelState.IsValid) {
                apiDb.AddRoomType(roomType);
                return RedirectToAction("Index");
            }
            return View(roomType);
        }

        // GET: RoomTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            RoomType type = new RoomType();
            if (apiDb.GetRoomTypeById(id, out type))
                if (type != null)
                    return View(type);
                else return HttpNotFound();
            else return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        // POST: RoomTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "RoomTypeID,Name")] RoomType roomType)
        {
            if (ModelState.IsValid)
            {
                apiDb.EditRoomType(roomType);
                return RedirectToAction("Index");
            }

            return View(roomType);
        }

        // GET: RoomTypes/Delete/5
        public ActionResult Delete(int? id)
        {
            RoomType type = new RoomType();
            if (apiDb.GetRoomTypeById(id, out type))
                if (type != null)
                    return View(type);
                else return HttpNotFound();
            else return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        // POST: RoomTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            apiDb.DeleteRoomType(id);
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
