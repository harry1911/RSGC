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
    public class RentPublishingsController : Controller
    {
        //AppDB apiDb = new AppDB();
        UserService userService;
        RentPublishingService rentPublishingService;
        IDB idb;

        public RentPublishingsController(IDB idb):base()
        {
            userService = new UserService(idb);
            rentPublishingService = new RentPublishingService(idb);
            this.idb = idb;
        }

        #region LISTINGS

        /// <summary>
        /// Lista los rentPublishings de un cliente
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles ="Client")]
        public ActionResult ListRentPublishingsClient()
        {
            var user = userService.GetUserByName(User.Identity.Name);
            if (user == null)
                return HttpNotFound();

            var rents = rentPublishingService.GetRentPublishingByUserId(user.UserID);

            return View(rents);
        }
        
        /// <summary>
        /// Lista los rentRegistries de un rentPublishing
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles ="Client,Realtor")]
        public ActionResult ListRentRegPub(int? id)
        {
            RentPublishing rentPublishing;
            if (rentPublishingService.GetRentPublishingById(id, out rentPublishing))
            {
                if (rentPublishing == null) return HttpNotFound();

                var rentRegs = rentPublishing.Rents.ToList();
                ViewBag.ID = id;
                return View(rentRegs);
            }
            else return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
       
        /// <summary>
        /// Lista los rentPublishings acceptados por el realtor
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles ="Realtor")]
        public ActionResult ListAcceptedRequest()
        {
            var user = userService.GetUserByName(User.Identity.Name);
            if (user == null) return HttpNotFound();

            var rents = rentPublishingService.GetRentPublishingByRealtorId(user.UserID);
            return View(rents);
        }

        #endregion

        #region DEFAULT

        //// GET: RentPublishings
        //public ActionResult Index()
        //{
        //    return View(db.Publishes.ToList());
        //}

        // GET: RentPublishings/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RentPublishing rentPublishing;
            rentPublishingService.GetRentPublishingById(id, out rentPublishing);
            if (rentPublishing == null)
            {
                return HttpNotFound();
            }
            return View(rentPublishing);
        }

        //// GET: RentPublishings/Create
        //public ActionResult Create()
        //{
        //    return View();
        //}

        //// POST: RentPublishings/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "PublishingID,Description,Price,CommissionPercent")] RentPublishing rentPublishing)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Publishes.Add(rentPublishing);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    return View(rentPublishing);
        //}

        //// GET: RentPublishings/Edit/5
        //public ActionResult Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    RentPublishing rentPublishing = db.RentPublishes.Find(id);
        //    if (rentPublishing == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(rentPublishing);
        //}

        //// POST: RentPublishings/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "PublishingID,Description,Price,CommissionPercent")] RentPublishing rentPublishing)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(rentPublishing).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    return View(rentPublishing);
        //}

        //// GET: RentPublishings/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    RentPublishing rentPublishing = db.RentPublishes.Find(id);
        //    if (rentPublishing == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(rentPublishing);
        //}

        //// POST: RentPublishings/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    RentPublishing rentPublishing = db.RentPublishes.Find(id);
        //    db.Publishes.Remove(rentPublishing);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}
        #endregion
    }
}
