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

namespace GCRS.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PrivilegesController : Controller
    {
        PrivilegesService apiDb;

        public PrivilegesController(IDB idb):base()
        {
            apiDb = new PrivilegesService(idb);
        }

        // GET: Privileges
        public ActionResult Index()
        {
            return View(apiDb.ListAllPrivileges());
        }

        // GET: Privileges/Details/5
        public ActionResult Details(int? id)
        {
            Privileges priv = new Privileges();
            if (apiDb.GetPrivilegeById(id, out priv))
                if (priv != null)
                    return View(priv);
                else return HttpNotFound();
            else return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        // GET: Privileges/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Privileges/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PrivilegesID,Name")] Privileges priv)
        {
            if (ModelState.IsValid)
            {
                apiDb.AddPrivilege(priv);
                return RedirectToAction("Index");
            }
            return View(priv);
        }

        // GET: Privileges/Edit/5
        public ActionResult Edit(int? id)
        {
            Privileges priv = new Privileges();
            if (apiDb.GetPrivilegeById(id, out priv))
                if (priv != null)
                    return View(priv);
                else return HttpNotFound();
            else return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        // POST: Privileges/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PrivilegesID,Name")] Privileges priv)
        {
            if (ModelState.IsValid) {
                apiDb.EditPrivilege(priv);
                return RedirectToAction("Index");
            }

            return View(priv);
        }

        // GET: Privileges/Delete/5
        public ActionResult Delete(int? id)
        {
            Privileges priv = new Privileges();
            if (apiDb.GetPrivilegeById(id, out priv))
                if (priv != null)
                    return View(priv);
                else return HttpNotFound();
            else return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        // POST: Privileges/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            apiDb.DeletePrivilege(id);
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
