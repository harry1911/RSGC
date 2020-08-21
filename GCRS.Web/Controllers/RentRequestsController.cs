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
using GCRS.Accounting;
using GCRS.Services;
using GCRS.Base.APIDatabase;

namespace GCRS.Web.Controllers
{
    public class RentRequestsController : Controller
    {
        //AppDB apiDb = new AppDB();
        UserService userService;
        RentPublishingService rentPublishingService;
        RentRegistryService rentRegistryService;
        DailySitService sitService;
        ConceptService conceptService;
        RentRequestService rentreq;
        ServicesService serv;
        IDB idb;

        public RentRequestsController(IDB idb):base()
        {
            userService = new UserService(idb);
            rentPublishingService = new RentPublishingService(idb);
            rentRegistryService = new RentRegistryService(idb);
            sitService = new DailySitService(idb);
            conceptService = new ConceptService(idb);
            rentreq = new RentRequestService(idb);
            serv = new ServicesService(idb);
            this.idb = idb;
        }

        #region LISTINGS
        /// <summary>
        /// Lista los rentRequest de una rentPublishing
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles ="Client,Realtor")]
        public ActionResult ListRentReqPub(int? id)
        {
            IEnumerable<RentRequest> rentRequests;
            if (rentreq.GetRentRequestsNotInFinalStates(id, out rentRequests))
            {
                ViewBag.ID = id;
                return View(rentRequests);
            }
            else return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        
        /// <summary>
        /// Lista los rentRequest del client
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles ="Client")]
        public ActionResult ListRentClient()
        {
            var user = userService.GetUserByName(User.Identity.Name);
            if (user == null) return HttpNotFound();

            var rents = rentreq.GetRentRequestsByClientId(user.UserID);
            return View(rents);
        }
        #endregion

        #region INITIAL AND FINAL STATES OF THE FLOW
        public ActionResult InitToPending(int? id)
        {
            RentPublishing rentPublishing;
            if (rentPublishingService.GetRentPublishingById(id, out rentPublishing))
            {
                if (rentPublishing == null) return HttpNotFound();

                TempData["id"] = id;

                var services = serv.AllServices().Select(x => new
                {
                    ServiceID = x.ServiceID,
                    ServiceString = x.Name + ": " + x.Price
                }).ToList();

                ViewBag.Services = new MultiSelectList(services, "ServiceID", "ServiceString");
                ViewBag.Price = rentPublishing.Price;
                ViewBag.Fechas = false;

                TempData["Price"] = rentPublishing.Price;
                TempData["Services"] = new MultiSelectList(services, "ServiceID", "ServiceString");

                return View();
            }
            else return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            
        }

        // POST: RentRequests/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult InitToPending([Bind(Include = "RequestID,RentPrice,Start,Finish")] RentRequest rentRequest, int[] ServiceID)
        {
            var user = userService.GetUserByName(User.Identity.Name);

            RentPublishing publishing;
            rentPublishingService.GetRentPublishingById((int)TempData["id"], out publishing);

            var services = ServiceID==null ? new List<Service>() : serv.GetServicesByIds(ServiceID);

            bool fechas = rentRequest.InitToPending(user, publishing, services);
            if (ModelState.IsValid && fechas)
            {
                //rentRequest.Client = db.Users.SingleOrDefault(x => x.Name == User.Identity.Name);
                //rentRequest.Publishing = (RentPublishing)TempData["rentPublishing"];
                //var services = db.Services.Where(x => ServiceID.Contains(x.ServiceID)).ToList();
                //rentRequest.Services = services;

                rentreq.TempAddRentRequest(rentRequest);

                rentRequest.Client = user;
                rentRequest.Publishing = publishing;
                rentRequest.Services = services;

                idb.SaveChanges();

                return RedirectToAction("ListRentClient");
            }
            
            // TODO: Gaby Preguntar a Hector que significa a = a
            ViewBag.Services = (MultiSelectList)TempData["Services"];
            TempData["Services"] = TempData["Services"];
            ViewBag.Price = TempData["Price"];
            TempData["Price"] = TempData["Price"];
            TempData["id"] = TempData["id"];
            ViewBag.Fechas = !fechas;

            return View(rentRequest);
        }

        [Authorize(Roles = "Client")]
        public ActionResult AcceptRequest(int? id1, int? id2)
        {
            RentRequest rentRequest;
            if (id1 != null && rentreq.GetRentRequestById(id2, out rentRequest))
            {
                if (rentRequest == null) return HttpNotFound();

                var user = userService.GetUserByName(User.Identity.Name);

                rentRequest.ToAccept(user);

                RentRegistry rentRegistry = new RentRegistry();

                rentRegistry.Start = rentRequest.Start;
                rentRegistry.Finish = rentRequest.Finish;
                rentRegistry.RentPrice = rentRequest.RentPrice;

                rentRegistryService.TempAddRentRegistry(rentRegistry);

                rentRegistry.Client = rentRequest.Client;
                rentRegistry.RentPublishing = rentRequest.Publishing;
                rentRegistry.Services = rentRequest.Services;

                //DailySit
                var dailySit = new DailySit();
                dailySit.Date = DateTime.Now;
                var nights = (rentRequest.Finish - rentRequest.Start).Days;
                dailySit.Value = ((rentRequest.Publishing.CommissionPercent * rentRequest.RentPrice) / 100) * nights;

                sitService.TempAddDailySit(dailySit);
                
                var concept = conceptService.GetConceptByName("Renta Casa");
                dailySit.TypeOfCompensation = concept;

                var other_rents = rentreq.GetRentRequestToReject(rentRequest.Publishing.PublishingID, rentRequest.RentRequestID);
                foreach (var item in other_rents)
                {
                    item.ToReject();
                    rentreq.TempEditRentRequest(item);
                }

                idb.SaveChanges();
                
                return rentRequest.Client.UserID == user.UserID ? RedirectToAction("ListRentClient") : RedirectToAction("ListRentReqPub", new { id = id1 });
            }
            else return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        [Authorize(Roles = "Client")]
        public ActionResult RejectRequest(int? id1, int? id2)
        {
            RentRequest rentRequest;
            if (rentreq.GetRentRequestById(id2, out rentRequest))
            {
                if (rentRequest == null) return HttpNotFound();

                rentRequest.ToReject();

                rentreq.EditRentRequest(rentRequest);

                var user = userService.GetUserByName(User.Identity.Name);

                return rentRequest.Client.UserID == user.UserID ? RedirectToAction("ListRentClient") : RedirectToAction("ListRentReqPub", new { id = id1 });
            }
            else return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        #endregion

        #region MAIN FLOW
        [Authorize(Roles ="Client,Realtor")]
        public ActionResult Pass(int? id1, int? id2)
        {
            RentRequest rentRequest;
            if(id1 != null && rentreq.GetRentRequestById(id2, out rentRequest))
            {
                var user = userService.GetUserByName(User.Identity.Name);

                if (rentRequest == null || user == null) return HttpNotFound();

                rentRequest.Pass(user);

                rentreq.EditRentRequest(rentRequest);
                return rentRequest.Client.UserID == user.UserID ? RedirectToAction("ListRentClient") : RedirectToAction("ListRentReqPub", new { id = id1 });
            }
            else return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        [Authorize(Roles ="Realtor")]
        public ActionResult PendingToPending2(int? id1, int? id2)
        {
            RentRequest rentRequest;
            if (id1 != null && rentreq.GetRentRequestById(id2, out rentRequest))
            {
                if (rentRequest == null) return HttpNotFound();

                rentRequest.PendingToPending2();

                rentreq.EditRentRequest(rentRequest);

                return RedirectToAction("ListRentReqPub/" + id1 + "/");
            }
            else return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        [Authorize(Roles = "Realtor")]
        public ActionResult Pending2ToPending(int? id1, int? id2)
        {
            RentRequest rentRequest;
            if (id1 != null && rentreq.GetRentRequestById(id2, out rentRequest))
            {
                if (rentRequest == null) return HttpNotFound();

                rentRequest.Pending2ToPending();

                rentreq.EditRentRequest(rentRequest);

                //TODO: Gaby
                return RedirectToAction("ListRentReqPub/" + id1 + "/");
            }
            else return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        [Authorize(Roles = "Realtor")]
        public ActionResult PendingToNegotiation(int? id1, int? id2)
        {
            RentRequest rentRequest;
            if (id1 != null && rentreq.GetRentRequestById(id2, out rentRequest))
            {
                if (rentRequest == null) return HttpNotFound();

                var services = serv.AllServices().Select(x => new
                {
                    ServiceID = x.ServiceID,
                    ServiceString = x.Name + ": " + x.Price
                }).ToList();

                ViewBag.Services = new MultiSelectList(services, "ServiceID", "ServiceString");

                TempData["multi_serv"] = new MultiSelectList(services, "ServiceID", "ServiceString");
                TempData["id1"] = id1;
                TempData["id_rentRequest"] = id2;
                TempData["RentPrice"] = rentRequest.RentPrice;
                TempData["Start"] = rentRequest.Start;
                TempData["Finish"] = rentRequest.Finish;
                TempData["Services"] = rentRequest.Services.ToList();
                TempData["Rents"] = rentRequest.Publishing.Rents.ToList();
                TempData["id_Publishing"] = rentRequest.Publishing.PublishingID;

                return View(rentRequest);
            }
            else return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        // POST: RentRequests/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PendingToNegotiation([Bind(Include = "RentRequestID,Start,Finish,RentPrice,RentState,LastChange")] RentRequest rentRequest, int[] ServiceID)
        {
            int id = (int)TempData["id_rentRequest"];

            rentRequest.Services = ServiceID.Length == 0 ? new List<Service>() : serv.GetServicesByIds(ServiceID);

            var _price = (double)TempData["RentPrice"];
            var _start = (DateTime)TempData["Start"];
            var _finish = (DateTime)TempData["Finish"];
            var _services = (List<Service>)TempData["Services"];
            var _rents = (List<RentRegistry>)TempData["Rents"];

            if (ModelState.IsValid && rentRequest.PendingToNegotiation(_price, _start, _finish, _services, _rents))
            {
                rentreq.EditRentRequest(rentRequest);

                id = (int)TempData["id1"];

                return RedirectToAction("ListRentReqPub/" + id + "/");
            }

            ViewBag.Services = (MultiSelectList)TempData["multi_serv"];
            
            // TODO: Hector explica lo de a = a
            TempData["multi_serv"] = (MultiSelectList)TempData["multi_serv"];
            TempData["id1"] = (int)TempData["id1"];
            TempData["id_rentRequest"] = (int)TempData["id_rentRequest"];
            TempData["RentPrice"] = (double)TempData["RentPrice"];
            TempData["Start"] = (DateTime)TempData["Start"];
            TempData["Finish"] = (DateTime)TempData["Finish"];
            TempData["Services"] = (List<Service>)TempData["Services"];
            TempData["Rents"] = (List<RentRegistry>)TempData["Rents"];

            return View(rentRequest);
        }

        [Authorize(Roles = "Realtor")]
        public ActionResult Pending2ToNegotiation2(int? id1, int? id2)
        {
            RentRequest rentRequest;
            if (id1 != null && rentreq.GetRentRequestById(id2, out rentRequest))
            {
                if (rentRequest == null) return HttpNotFound();

                var services = serv.AllServices().Select(x => new
                {
                    ServiceID = x.ServiceID,
                    ServiceString = x.Name + ": " + x.Price
                }).ToList();

                ViewBag.Services = new MultiSelectList(services, "ServiceID", "ServiceString");

                TempData["multi_serv"] = new MultiSelectList(services, "ServiceID", "ServiceString");
                TempData["id1"] = id1;
                TempData["id_rentRequest"] = id2;
                TempData["RentPrice"] = rentRequest.RentPrice;
                TempData["Start"] = rentRequest.Start;
                TempData["Finish"] = rentRequest.Finish;
                TempData["Services"] = rentRequest.Services.ToList();
                TempData["Rents"] = rentRequest.Publishing.Rents.ToList();
                TempData["id_Publishing"] = rentRequest.Publishing.PublishingID;
                return View(rentRequest);
            }
            else return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        // POST: RentRequests/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken] 
        public ActionResult Pending2ToNegotiation2([Bind(Include = "RentRequestID,Start,Finish,RentPrice,RentState,LastChange")] RentRequest rentRequest, int[] ServiceID)
        {            
            int id = (int)TempData["id_rentRequest"];
            rentRequest.Services = ServiceID.Length == 0 ? new List<Service>() : serv.GetServicesByIds(ServiceID);

            var _price = (double)TempData["RentPrice"];
            var _start = (DateTime)TempData["Start"];
            var _finish = (DateTime)TempData["Finish"];
            var _services = (List<Service>)TempData["Services"];
            var _rents = (List<RentRegistry>)TempData["Rents"];

            if (ModelState.IsValid && rentRequest.Pending2ToNegotiation2(_price, _start, _finish, _services, _rents))
            {
                //hay que poner antes del state las prop null
                rentreq.EditRentRequest(rentRequest);
                
                return RedirectToAction("ListRentReqPub", new { id = (int)TempData["id1"] });
            }

            ViewBag.Services = (MultiSelectList)TempData["multi_serv"];
            
            TempData["multi_serv"] = (MultiSelectList)TempData["multi_serv"];
            TempData["id1"] = (int)TempData["id1"];
            TempData["id_rentRequest"] = (int)TempData["id_rentRequest"];
            TempData["RentPrice"] = (double)TempData["RentPrice"];
            TempData["Start"] = (DateTime)TempData["Start"];
            TempData["Finish"] = (DateTime)TempData["Finish"];
            TempData["Services"] = (List<Service>)TempData["Services"];
            TempData["Rents"] = (List<RentRegistry>)TempData["Rents"];

            return View(rentRequest);
        }

        [Authorize(Roles = "Client")]
        public ActionResult NegotiationToPending(int? id)
        {
            RentRequest rentRequest;
            if (rentreq.GetRentRequestById(id, out rentRequest))
            {
                if (rentRequest == null) return HttpNotFound();

                var services = serv.AllServices().Select(x => new
                {
                    ServiceID = x.ServiceID,
                    ServiceString = x.Name + ": " + x.Price
                }).ToList();

                ViewBag.Services = new MultiSelectList(services, "ServiceID", "ServiceString");

                TempData["multi_serv"] = new MultiSelectList(services, "ServiceID", "ServiceString");
                TempData["id_rentRequest"] = id;
                TempData["RentPrice"] = rentRequest.RentPrice;
                TempData["Start"] = rentRequest.Start;
                TempData["Finish"] = rentRequest.Finish;
                TempData["Services"] = rentRequest.Services.ToList();
                TempData["Rents"] = rentRequest.Publishing.Rents.ToList();
                TempData["id_Publishing"] = rentRequest.Publishing.PublishingID;

                return View(rentRequest);
            }
            else return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        // POST: RentRequests/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NegotiationToPending([Bind(Include = "RentRequestID,Start,Finish,RentPrice,RentState,LastChange")] RentRequest rentRequest, int[] ServiceID)
        {
            int id = (int)TempData["id_rentRequest"];

            rentRequest.Services = ServiceID.Length == 0 ? new List<Service>() : serv.GetServicesByIds(ServiceID);

            var _price = (double)TempData["RentPrice"];
            var _start = (DateTime)TempData["Start"];
            var _finish = (DateTime)TempData["Finish"];
            var _services = (List<Service>)TempData["Services"];
            var _rents = (List<RentRegistry>)TempData["Rents"];

            if (ModelState.IsValid && rentRequest.NegotiationToPending(_price, _start, _finish, _services, _rents))
            {
                rentreq.EditRentRequest(rentRequest);

                return RedirectToAction("ListRentClient");
            }

            ViewBag.Services = (MultiSelectList)TempData["multi_serv"];
            
            // TODO: Hector Ya sabes
            TempData["multi_serv"] = (MultiSelectList)TempData["multi_serv"];
            TempData["id1"] = (int)TempData["id1"];
            TempData["id_rentRequest"] = (int)TempData["id_rentRequest"];
            TempData["RentPrice"] = (double)TempData["RentPrice"];
            TempData["Start"] = (DateTime)TempData["Start"];
            TempData["Finish"] = (DateTime)TempData["Finish"];
            TempData["Services"] = (List<Service>)TempData["Services"];
            TempData["Rents"] = (List<RentRegistry>)TempData["Rents"];

            return View(rentRequest);
        }

        [Authorize(Roles = "Client")]
        public ActionResult Negotiation2ToPending2(int? id1, int? id2)
        {
            RentRequest rentRequest;
            if (id1 != null && rentreq.GetRentRequestById(id2, out rentRequest))
            {
                if (rentRequest == null) return HttpNotFound();

                var services = serv.AllServices().Select(x => new
                {
                    ServiceID = x.ServiceID,
                    ServiceString = x.Name + ": " + x.Price
                }).ToList();

                ViewBag.Services = new MultiSelectList(services, "ServiceID", "ServiceString");

                TempData["multi_serv"] = new MultiSelectList(services, "ServiceID", "ServiceString");
                TempData["id1"] = id1;
                TempData["id_rentRequest"] = id2;
                TempData["RentPrice"] = rentRequest.RentPrice;
                TempData["Start"] = rentRequest.Start;
                TempData["Finish"] = rentRequest.Finish;
                TempData["Services"] = rentRequest.Services.ToList();
                TempData["Rents"] = rentRequest.Publishing.Rents.ToList();
                TempData["id_Publishing"] = rentRequest.Publishing.PublishingID;

                return View(rentRequest);
            }
            else return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        // POST: RentRequests/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Negotiation2ToPending2([Bind(Include = "RentRequestID,Start,Finish,RentPrice,RentState,LastChange")] RentRequest rentRequest, int[] ServiceID)
        {
            int id = (int)TempData["id_rentRequest"];

            rentRequest.Services = ServiceID.Length == 0 ? new List<Service>() : serv.GetServicesByIds(ServiceID);

            var _price = (double)TempData["RentPrice"];
            var _start = (DateTime)TempData["Start"];
            var _finish = (DateTime)TempData["Finish"];
            var _services = (List<Service>)TempData["Services"];
            var _rents = (List<RentRegistry>)TempData["Rents"];

            if (ModelState.IsValid && rentRequest.Negotiation2ToPending2(_price, _start, _finish, _services, _rents))
            {
                rentreq.EditRentRequest(rentRequest);

                return RedirectToAction("ListRentReqPub", new { id = (int)TempData["id1"] });
            }

            ViewBag.Services = (MultiSelectList)TempData["multi_serv"];

            // TODO: Hector Espero que ya sepas
            TempData["multi_serv"] = (MultiSelectList)TempData["multi_serv"];
            TempData["id1"] = (int)TempData["id1"];
            TempData["id_rentRequest"] = (int)TempData["id_rentRequest"];
            TempData["RentPrice"] = (double)TempData["RentPrice"];
            TempData["Start"] = (DateTime)TempData["Start"];
            TempData["Finish"] = (DateTime)TempData["Finish"];
            TempData["Services"] = (List<Service>)TempData["Services"];
            TempData["Rents"] = (List<RentRegistry>)TempData["Rents"];

            return View(rentRequest);
        }
        #endregion

        #region DEFAULT

        //// GET: RentRequests
        //public ActionResult Index()
        //{
        //    return View(db.RentRequests.ToList());
        //}

        // GET: RentRequests/Details/5
        public ActionResult Details(int? id1, int? id2)
        {
            if (id1 == null || id2 == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RentRequest rentRequest;
            rentreq.GetRentRequestById(id2,out rentRequest);
            var user = userService.GetUserByName(User.Identity.Name);
            if (rentRequest == null || user == null)
            {
                return HttpNotFound();
            }
            ViewBag.ID = id1;
            ViewBag.UserID = user.UserID;
            return View(rentRequest);
        }

        //// GET: RentRequests/Create
        //public ActionResult Create()
        //{
        //    return View();
        //}

        // POST: RentRequests/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "RentRequestID,Start,Finish,RentPrice,RentState,LastChange")] RentRequest rentRequest)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.RentRequests.Add(rentRequest);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    return View(rentRequest);
        //}

        //// GET: RentRequests/Edit/5
        //public ActionResult Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    RentRequest rentRequest = db.RentRequests.Find(id);
        //    if (rentRequest == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(rentRequest);
        //}

        //// POST: RentRequests/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "RentRequestID,Start,Finish,RentPrice,RentState,LastChange")] RentRequest rentRequest)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(rentRequest).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    return View(rentRequest);
        //}

        //// GET: RentRequests/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    RentRequest rentRequest = db.RentRequests.Find(id);
        //    if (rentRequest == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(rentRequest);
        //}

        //// POST: RentRequests/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    RentRequest rentRequest = db.RentRequests.Find(id);
        //    db.RentRequests.Remove(rentRequest);
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
