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
    public class FeaturesController : Controller
    {
        FeatureService apiDb;

        public FeaturesController(IDB idb):base()
        {
            apiDb = new FeatureService(idb);
        }
        // GET: Features
        public ActionResult Index()
        {
            return View(apiDb.ListAllFeatures());
        }

        // GET: Features/Details/5
        public ActionResult Details(int? id)
        {
            Feature feature = new Feature();
            if (apiDb.GetFeatureById(id, out feature))
                if (feature != null)
                    return View(feature);
                else return HttpNotFound();
            else return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        // GET: Features/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Features/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "FeatureID,Name,Description")] Feature feature)
        {
            if (ModelState.IsValid) {
                apiDb.AddFeature(feature);
                return RedirectToAction("Index");
            }
            return View(feature);
        }

        // GET: Features/Edit/5
        public ActionResult Edit(int? id)
        {
            Feature feature = new Feature();
            if (apiDb.GetFeatureById(id, out feature))
                if (feature != null)
                    return View(feature);
                else return HttpNotFound();
            else return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        // POST: Features/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "FeatureID,Name,Description")] Feature feature)
        {
            if (ModelState.IsValid)
            {
                apiDb.EditFeature(feature);
                return RedirectToAction("Index");
            }
            return View(feature);
        }

        // GET: Features/Delete/5
        public ActionResult Delete(int? id)
        {
            Feature feature = new Feature();
            if (apiDb.GetFeatureById(id, out feature))
                if (feature != null)
                    return View(feature);
                else return HttpNotFound();
            else return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        // POST: Features/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            apiDb.DeleteFeature(id);
            return RedirectToAction("Index");
        }

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        featureRepo.db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}
    }
}
