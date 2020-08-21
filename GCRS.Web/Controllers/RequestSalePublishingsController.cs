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
    public class RequestSalePublishingsController : Controller
    {
        //AppDB apiDb = new AppDB();
        HouseService houseService;
        SalePublishingService salePublishingService;
        UserService userService;
        RequestSalePublishingService requestSalePublishingService;
        IDB idb;

        public RequestSalePublishingsController(IDB idb):base()
        {
            userService = new UserService(idb);
            houseService = new HouseService(idb);
            salePublishingService = new SalePublishingService(idb);
            requestSalePublishingService = new RequestSalePublishingService(idb);
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

            var requests = requestSalePublishingService.GetSellRequestByRealtorId(user.UserID);
            return View(requests);
        }

        /// <summary>
        /// Lista los request sin atender
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Realtor")]
        public ActionResult ListNonAttendedRequest()
        {
            var requests = requestSalePublishingService.GetNonAttendedSellRequests();
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

            var requests = requestSalePublishingService.GetSellRequestByUserId(user.UserID);
            return View(requests);
        }

        [Authorize(Roles = "Client")]
        public ActionResult ListPublishingsClient()
        {
            var user = userService.GetUserByName(User.Identity.Name);
            if (user == null) return HttpNotFound();

            //ya esta
            var requests = salePublishingService.GetSellPublishingByUserId(user.UserID);
            return View(requests);
        }

        [Authorize(Roles = "Realtor")]
        public ActionResult ListAcceptedRequest()
        {
            var user = userService.GetUserByName(User.Identity.Name);
            if (user == null) return HttpNotFound();

            //ya esta
            var requests = salePublishingService.GetSellPublishingByRealtorId(user.UserID);
            return View(requests);
        }

        #endregion
        
        #region INITIAL AND FINAL STATES OF THE FLOW
        // GET: RequestRentPublishings/Create
        //RequestHouseID
        public ActionResult RequestSalePost(int? id)
        {
            var user = userService.GetUserByName(User.Identity.Name);
            if(user == null) return HttpNotFound();

            House requestHouse;
            if(houseService.GetHouseById(id, out requestHouse))
            {
                if(requestHouse == null) return HttpNotFound();

                ViewBag.User = user;
                ViewBag.RequestHouse = requestHouse;

                TempData["RequestHouse"] = requestHouse.HouseID;
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
        public ActionResult RequestSalePost([Bind(Include = "RequestPublishingID,Description,Price,CommissionPercent,LastChange,RequestStatus,House")] RequestSalePublishing requestSalePublishing, List<HttpPostedFileBase> images)
        {
            if (ModelState.IsValid)
            {
                requestSalePublishingService.TempAddSellRequest(requestSalePublishing);

                var house = requestSalePublishing.House;
                requestSalePublishing.User = userService.GetUserByName(User.Identity.Name);
                houseService.GetHouseById((int)TempData["RequestHouse"], out house);
                requestSalePublishing.House = house;
                requestSalePublishing.House = house;

                requestSalePublishing.InitToRequest();

                idb.SaveChanges();

                requestSalePublishing.Images = new List<Image>();

                if (images != null && images[0] != null)
                    foreach (var image in images)
                    {
                        var filename = image.FileName;
                        var filePathOriginal = Server.MapPath("/Content/Pictures");
                        string directory = Path.Combine(filePathOriginal, requestSalePublishing.RequestPublishingID.ToString());
                        Directory.CreateDirectory(directory);
                        string savedFileName = Path.Combine(directory, filename);
                        image.SaveAs(savedFileName);

                        var im = new Image();
                        im.Path = Path.Combine("~/Content/Pictures", requestSalePublishing.RequestPublishingID.ToString(), filename);
                        im.RequestPublishingID = requestSalePublishing.RequestPublishingID;
                        idb.SaveChanges();
                        requestSalePublishing.Images.Add(im);
                    }
                idb.SaveChanges();

                return RedirectToAction("ListRequestClient");
            }
            return View(requestSalePublishing);
        }

        [Authorize(Roles = "Realtor")]
        public ActionResult TakeRequest(int? id)
        {
            RequestSalePublishing requestSalePublishing;
            if(requestSalePublishingService.GetSellRequestById(id, out requestSalePublishing))
            {
                if (requestSalePublishing == null) return HttpNotFound();

                var realtor = userService.GetUserByName(User.Identity.Name);
                requestSalePublishing.RequestToPending(realtor);
                requestSalePublishingService.EditSellRequest(requestSalePublishing);

                return RedirectToAction("ListAttendedRequest");
            }
            else return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        [Authorize(Roles = "Client,Realtor")]
        public ActionResult AcceptRequest(int? id)
        {
            RequestSalePublishing request;
            if (requestSalePublishingService.GetSellRequestById(id, out request))
            {
                if (request == null) return HttpNotFound();

                var user = userService.GetUserByName(User.Identity.Name);
                request.ToAccept(user);

                var salePost = new SalePublishing { CommissionPercent = request.CommissionPercent, Price = request.Price, Realtor = request.Realtor, House = request.House, User = request.User, Description = request.Description, Images= request.Images, Showable = true };
                salePublishingService.AddSellPublishing(salePost);

                return Redirect();
            }
            else return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        [Authorize(Roles = "Client,Realtor")]
        public ActionResult RejectRequest(int? id)
        {
            RequestSalePublishing request;
            if (requestSalePublishingService.GetSellRequestById(id, out request))
            {
                if (request == null) return HttpNotFound();

                request.ToReject();
                requestSalePublishingService.EditSellRequest(request);

                return Redirect();
            }
            else return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        #endregion

        #region MAIN FLOW
        [Authorize(Roles = "Client,Realtor")]
        public ActionResult FlowRequest(int? id)
        {
            RequestSalePublishing request;
            if (requestSalePublishingService.GetSellRequestById(id, out request))
            {
                if (request == null) return HttpNotFound();

                TempData["Price"] = request.Price;
                TempData["Comission"] = request.CommissionPercent;
                TempData["Description"] = request.Description;

                return View(request);
            }
            else return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Client,Realtor")]
        public ActionResult FlowRequest([Bind(Include = "RequestPublishingID,Description,Price,CommissionPercent,LastChange,RequestStatus")] RequestSalePublishing request)
        {
            if (ModelState.IsValid)
            {
                if (User.IsInRole("Realtor"))
                    request.PendingToNegotiation((double)TempData["Price"], (double)TempData["Comission"]);
                else request.NegotiationToPending((double)TempData["Price"], (double)TempData["Comission"]);

                request.Description = (string)TempData["Description"];
                requestSalePublishingService.EditSellRequest(request);

                return Redirect();
            }
            return View(request);
        }
        #endregion

        ActionResult Redirect() => User.IsInRole("Realtor") ? RedirectToAction("ListAttendedRequest") : RedirectToAction("ListRequestClient");

        // GET: RequestRentPublishings/Details/5
        public ActionResult Details(int? id)
        {
            RequestSalePublishing request;
            if (requestSalePublishingService.GetSellRequestById(id, out request))
            {
                if (request == null) return HttpNotFound();

                return View(request);
            }
            else return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

    }
}
