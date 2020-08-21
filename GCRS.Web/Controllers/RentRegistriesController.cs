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
using GCRS.Services;
using GCRS.Base.APIDatabase;

namespace GCRS.Web.Controllers
{
    public class RentRegistriesController : Controller
    {
        //AppDB apiDb = new AppDB();
        RentRegistryService rentRegistryService;
        IDB idb;

        public RentRegistriesController(IDB idb):base()
        {
            rentRegistryService = new RentRegistryService(idb);

            this.idb = idb;
        }

        // GET: RentRegistries
        public ActionResult Index()
        {
            return View(rentRegistryService.AllRentRegistries());
        }

        // GET: RentRegistries/Details/5
        public ActionResult Details(int? id1, int? id2)
        {
            RentRegistry rentRegistry;
            if (id1 != null && rentRegistryService.GetRentRegistryById(id2, out rentRegistry))
            {
                if (rentRegistry == null) return HttpNotFound();

                ViewBag.ID = id1;
                return View(rentRegistry);
            }
            else return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            
        }

        // GET: RentRegistries/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: RentRegistries/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "RegistryID,Start,Finish,RentPrice")] RentRegistry rentRegistry)
        {
            if (ModelState.IsValid)
            {
                rentRegistryService.AddRentRegistry(rentRegistry);
                return RedirectToAction("Index");
            }

            return View(rentRegistry);
        }

        // GET: RentRegistries/Edit/5
        public ActionResult Edit(int? id)
        {
            RentRegistry rentRegistry;
            if (rentRegistryService.GetRentRegistryById(id, out rentRegistry))
            {
                if (rentRegistry == null) return HttpNotFound();

                return View(rentRegistry);
            }
            else return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        // POST: RentRegistries/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "RegistryID,Start,Finish,RentPrice")] RentRegistry rentRegistry)
        {
            if (ModelState.IsValid)
            {
                rentRegistryService.EditRentRegistry(rentRegistry);
                return RedirectToAction("Index");
            }
            return View(rentRegistry);
        }

        // GET: RentRegistries/Delete/5
        public ActionResult Delete(int? id)
        {
            RentRegistry rentRegistry;
            if (rentRegistryService.GetRentRegistryById(id, out rentRegistry))
            {
                if (rentRegistry == null) return HttpNotFound();

                return View(rentRegistry);
            }
            else return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        // POST: RentRegistries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            RentRegistry rentRegistry;
            rentRegistryService.GetRentRegistryById(id, out rentRegistry);
            rentRegistryService.RemoveRentRegistry(rentRegistry);
            return RedirectToAction("Index");
        }

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing) apiDb.Dispose();
        //    base.Dispose(disposing);
        //}
    }
}
