using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using Database;
using GCRS.Accounting;
using GCRS.Services;
using GCRS.Base.APIDatabase;

namespace GCRS.Web.Controllers
{
    [Authorize(Roles ="Jefe")]
    public class DailySitsController : Controller
    {
        //private DB db = new DB();

        DailySitService sitService;

        public DailySitsController(IDB idb):base()
        {
            sitService = new DailySitService(idb);
        }

        // GET: Concepts
        public ActionResult Index()
        {
            return View(sitService.ListAll());
        }

        // GET: Concepts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DailySit dailySit = sitService.FindById((int)id);
            if (dailySit == null)
            {
                return HttpNotFound();
            }
            return View(dailySit);
        }

        // GET: Concepts/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Concepts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Date,Value")] DailySit dailySit)
        {
            if (ModelState.IsValid)
            {
                sitService.AddConcept(dailySit);
                return RedirectToAction("Index");
            }

            return View(dailySit);
        }

        // GET: Concepts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DailySit dailySit = sitService.FindById((int)id);
            if (dailySit == null)
            {
                return HttpNotFound();
            }
            return View(dailySit);
        }

        // POST: Concepts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Date,Value")] DailySit dailySit)
        {
            if (ModelState.IsValid)
            {
                sitService.EditConcept(dailySit);
                return RedirectToAction("Index");
            }
            return View(dailySit);
        }

        // GET: Concepts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DailySit dailySit = sitService.FindById((int)id);
            if (dailySit == null)
            {
                return HttpNotFound();
            }
            return View(dailySit);
        }

        // POST: Concepts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DailySit dailySit = sitService.FindById((int)id);
            sitService.RemoveConcept(dailySit);
            return RedirectToAction("Index");
        }

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}
    }
}
