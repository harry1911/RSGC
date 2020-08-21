using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Database;
using GCRS.Accounting;
using GCRS.Services;
using GCRS.Base.APIDatabase;

namespace GCRS.Web.Controllers
{
    [Authorize(Roles ="Admin")]
    public class ConceptsController : Controller
    {
        //private DB db = new DB();

        ConceptService conceptService;

        public ConceptsController(IDB idb):base()
        {
            conceptService = new ConceptService(idb);
        }

        // GET: Concepts
        public ActionResult Index()
        {
            return View(conceptService.ListAll());
        }

        // GET: Concepts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Concept concept = conceptService.FindById((int)id);
            if (concept == null)
            {
                return HttpNotFound();
            }
            return View(concept);
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
        public ActionResult Create([Bind(Include = "ConceptID,Name,Income")] Concept concept)
        {
            if (ModelState.IsValid)
            {
                conceptService.AddConcept(concept);
                return RedirectToAction("Index");
            }

            return View(concept);
        }

        // GET: Concepts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Concept concept = conceptService.FindById((int)id);
            if (concept == null)
            {
                return HttpNotFound();
            }
            return View(concept);
        }

        // POST: Concepts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ConceptID,Name,Income")] Concept concept)
        {
            if (ModelState.IsValid)
            {
                conceptService.EditConcept(concept);
                return RedirectToAction("Index");
            }
            return View(concept);
        }

        // GET: Concepts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Concept concept = conceptService.FindById((int)id);
            if (concept == null)
            {
                return HttpNotFound();
            }
            return View(concept);
        }

        // POST: Concepts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Concept concept = conceptService.FindById((int)id);
            conceptService.RemoveConcept(concept);
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
