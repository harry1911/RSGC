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
using System.IO;
using GCRS.Services;
using GCRS.Base.APIDatabase;

namespace GCRS.Web.Controllers
{
    public class RequestRentPublishingsController : Controller
    {
        //AppDB apiDb = new AppDB();
        UserService userService;
        RequestRentPublishingService reqserv;
        HouseService houseService;
        RentPublishingService rentPublishingService;
        IDB idb;

        public RequestRentPublishingsController(IDB idb):base()
        {
            userService = new UserService(idb);
            houseService = new HouseService(idb);
            rentPublishingService = new RentPublishingService(idb);
            reqserv = new RequestRentPublishingService(idb);

            this.idb = idb;
        }
        
        #region LISTINGS
        /// <summary>
        /// Lista los request que son atendidos por un realtor en especifico
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Realtor")]
        public ActionResult ListAttendedRequest()
        {
            var user = userService.GetUserByName(User.Identity.Name);
            if (user == null) return HttpNotFound();

            var requests = reqserv.GetRentRequestPublishingByRealtorId(user.UserID);
            return View(requests);
        }

        /// <summary>
        /// Lista los request sin atender
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Realtor")]
        public ActionResult ListNonAttendedRequest()
        {
            var requests = reqserv.GetRentRequestPublishingWithoutRealtor();
            return View(requests);
        }

        /// <summary>
        /// Lista todos los request del cliente sin importar el estado...
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Client")]
        public ActionResult ListRequestClient()
        {
            var user = userService.GetUserByName(User.Identity.Name);
            if (user == null) return HttpNotFound();
            var requests = reqserv.GetRentRequestPublishingByUserId(user.UserID);
            return View(requests);
        }
        #endregion

        #region INITIAL AND FINAL STATES OF THE FLOW
        // GET: RequestRentPublishings/Create
        //RequestHouseID
        public ActionResult RequestPost(int? id)
        {
            var user = userService.GetUserByName(User.Identity.Name);
            if(user == null) return HttpNotFound();

            House requestHouse; 
            if (houseService.GetHouseById(id, out requestHouse))
            {
                if(requestHouse == null) return HttpNotFound();

                var requestRooms = requestHouse.Rooms.
            Select(x => new
            {
                RoomID = x.RoomID,
                RoomName = x.Description
            }).ToList();

                ViewBag.RequestRooms = new MultiSelectList(requestRooms.OrderBy(x => x.RoomName), "RoomID", "RoomName");
                ViewBag.ID = requestHouse.HouseID;
                ViewBag.User = user;
                ViewBag.RequestHouse = requestHouse;

                TempData["requestHouse_id"] = id;

                return View();
            }
            else return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        // POST: RequestRentPublishings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Client")]
        public ActionResult RequestPost([Bind(Include = "RequestPublishingID,Description,Price,CommissionPercent,LastChange,RequestStatus,Rooms")] RequestRentPublishing requestRentPublishing,int[] RoomID, List<HttpPostedFileBase> images)
        {
            if (ModelState.IsValid && RoomID.Length > 0)
            {
                int id = (int)TempData["requestHouse_id"];
                House house;
                houseService.GetHouseById(id, out house);

                reqserv.TempAddRentRequestPublishing(requestRentPublishing);

                requestRentPublishing.User = userService.GetUserByName(User.Identity.Name);
                requestRentPublishing.House = house;
                requestRentPublishing.Rooms = house.Rooms.Where(x => RoomID.Contains(x.RoomID)).ToList();

                requestRentPublishing.InitToRequest();

                idb.SaveChanges();

                requestRentPublishing.Images = new List<Image>();
                if (images != null && images[0] != null)
                    foreach (var image in images)
                    {
                        var filename = image.FileName;
                        var filePathOriginal = Server.MapPath("/Content/Pictures");
                        string directory = Path.Combine(filePathOriginal, requestRentPublishing.RequestPublishingID.ToString());
                        Directory.CreateDirectory(directory);
                        string savedFileName = Path.Combine(directory, filename);
                        image.SaveAs(savedFileName);

                        var im = new Image();
                        im.Path = Path.Combine("~/Content/Pictures", requestRentPublishing.RequestPublishingID.ToString(), filename);
                        im.RequestPublishingID = requestRentPublishing.RequestPublishingID;
                        idb.SaveChanges();
                        requestRentPublishing.Images.Add(im);
                    }
                idb.SaveChanges();

                return RedirectToAction("ListRequestClient");
            }

            TempData["requestHouse_id"] = (int)TempData["requestHouse_id"];
            return View(requestRentPublishing);
        }

        [Authorize(Roles = "Realtor")]
        public ActionResult TakeRequest(int? id)
        {
            RequestRentPublishing requestRentPublishing;
            if (reqserv.GetRentRequestPublishingById(id, out requestRentPublishing))
            {
                if (requestRentPublishing == null)
                    return HttpNotFound();

                var realtor = userService.GetUserByName(User.Identity.Name);
                requestRentPublishing.RequestToPending(realtor);

                reqserv.EditRentRequestPublishing(requestRentPublishing);

                return RedirectToAction("ListAttendedRequest");
            }
            else return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            
        }

        [Authorize(Roles = "Client,Realtor")]
        public ActionResult AcceptRequest(int? id)
        {
            RequestRentPublishing request;
            if (reqserv.GetRentRequestPublishingById(id, out request))
            {
                if (request == null) return HttpNotFound();

                var user = userService.GetUserByName(User.Identity.Name);

                request.ToAccept(user);

                var rentPost = new RentPublishing();
                rentPost.CommissionPercent = request.CommissionPercent;
                rentPost.Description = request.Description;
                rentPost.Price = request.Price;

                rentPublishingService.TempAddRentPublishing(rentPost);

                rentPost.House = request.House;
                rentPost.Realtor = request.Realtor;
                rentPost.Rooms = request.Rooms;
                rentPost.User = request.User;
                rentPost.Images = request.Images;

                idb.SaveChanges();

                return Redirect();
            }
            else return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            //var room = new Room { Type = request.Type, Description = request.Description, Features = request.Features };
        }

        [Authorize(Roles = "Client,Realtor")]
        public ActionResult RejectRequest(int? id)
        {
            RequestRentPublishing request;
            if (reqserv.GetRentRequestPublishingById(id, out request))
            {
                if (request == null) return HttpNotFound();

                request.ToReject();

                reqserv.EditRentRequestPublishing(request);

                return Redirect();
            }
            else return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        #endregion

        #region MAIN FLOW
        [Authorize(Roles = "Client,Realtor")]
        public ActionResult FlowRequest(int? id)
        {
            RequestRentPublishing request;
            if (reqserv.GetRentRequestPublishingById(id, out request))
            {
                if (request == null) return HttpNotFound();

                TempData["Price"] = request.Price;
                TempData["Comission"] = request.CommissionPercent;

                return View(request);
            }
            else return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Client,Realtor")]
        public ActionResult FlowRequest([Bind(Include = "RequestPublishingID,Description,Price,CommissionPercent,LastChange,RequestStatus")] RequestRentPublishing requestRentPublishing)
        {
            if (ModelState.IsValid)
            {
                if (User.IsInRole("Realtor"))
                    requestRentPublishing.PendingToNegotiation((double)TempData["Price"], (double)TempData["Comission"]);
                else
                    requestRentPublishing.NegotiationToPending((double)TempData["Price"], (double)TempData["Comission"]);

                reqserv.EditRentRequestPublishing(requestRentPublishing);

                return Redirect();
            }
            return View(requestRentPublishing);
        }
        #endregion

        public ActionResult Redirect() => User.IsInRole("Realtor") ? RedirectToAction("ListAttendedRequest") : RedirectToAction("ListRequestClient");

        #region DEFAULT

        // GET: RequestRentPublishings
        public ActionResult Index()
        {
            return View(reqserv.GetAll());
        }

        // GET: RequestRentPublishings/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RequestRentPublishing requestRentPublishing;
            reqserv.GetRentRequestPublishingById(id,out requestRentPublishing);
            if (requestRentPublishing == null)
            {
                return HttpNotFound();
            }
            return View(requestRentPublishing);
        }


        // GET: RequestRentPublishings/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: RequestRentPublishings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "RequestPublishingID,Description,Price,CommissionPercent,LastChange,RequestStatus,RequestType")] RequestRentPublishing requestRentPublishing)
        {
            if (ModelState.IsValid)
            {
                reqserv.Add(requestRentPublishing);
                return RedirectToAction("Index");
            }

            return View(requestRentPublishing);
        }

        // GET: RequestRentPublishings/Edit/5
        //public ActionResult Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    RequestRentPublishing requestRentPublishing = reqserv.Find(id);
        //    if (requestRentPublishing == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(requestRentPublishing);
        //}

        // POST: RequestRentPublishings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "RequestPublishingID,Description,Price,CommissionPercent,LastChange,RequestStatus,RequestType")] RequestRentPublishing requestRentPublishing)
        {
            if (ModelState.IsValid)
            {
                reqserv.EditRentRequestPublishing(requestRentPublishing);
                return RedirectToAction("Index");
            }
            return View(requestRentPublishing);
        }

        // GET: RequestRentPublishings/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    RequestRentPublishing requestRentPublishing = db.RequestRentPublishings.Find(id);
        //    if (requestRentPublishing == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(requestRentPublishing);
        //}

        //// POST: RequestRentPublishings/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    RequestRentPublishing requestRentPublishing = db.RequestRentPublishings.Find(id);
        //    db.RequestRentPublishings.Remove(requestRentPublishing);
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
