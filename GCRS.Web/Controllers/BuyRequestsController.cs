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
    public class BuyRequestsController : Controller
    {
        //AppDB apiDb = new AppDB();
        BuyRequestService buyService;
        HouseService houseService;
        SalePublishingService salePublishingService;
        RentPublishingService rentPublishingService;
        DailySitService sitService;
        ConceptService conceptService;
        UserService userService;
        SaleRegistryService saleRegistryService;
        RentRequestService rentRequestService;
        IDB idb;

        public BuyRequestsController(IDB idb):base()
        {
            buyService = new BuyRequestService(idb);
            userService = new UserService(idb);
            houseService = new HouseService(idb);
            salePublishingService = new SalePublishingService(idb);
            rentPublishingService = new RentPublishingService(idb);
            conceptService = new ConceptService(idb);
            sitService = new DailySitService(idb);
            saleRegistryService = new SaleRegistryService(idb);
            rentRequestService = new RentRequestService(idb);
            this.idb = idb;
        }

        #region LISTINGS
        [Authorize(Roles = "Realtor")]
        public ActionResult ListSellRealtor(int? id)
        {
            var user = userService.GetUserByName(User.Identity.Name);
            if (user == null)
                return HttpNotFound();

            IEnumerable<BuyRequest> buyRequests;
            if (buyService.GetRealtorBuyRequests(id, user.UserID, out buyRequests) && buyRequests != null)
                return View(buyRequests);
            else return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        [Authorize(Roles = "Client")]
        public ActionResult ListSellOwner(int? id)
        {

            var user = userService.GetUserByName(User.Identity.Name);
            if (user == null)
                return HttpNotFound();

            IEnumerable<BuyRequest> buyRequests;
            if (buyService.GetOwnerBuyRequests(id, user.UserID, out buyRequests) && buyRequests != null)
                return View(buyRequests);
            else return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        [Authorize(Roles = "Client")]
        public ActionResult ListBuyClient()
        {
            var user = userService.GetUserByName(User.Identity.Name);
            if (user == null)
                return HttpNotFound();

            var buyRequests = buyService.GetClientBuyRequests(user.UserID);
            return View(buyRequests);
        }
        #endregion

        #region INIT
        // GET Negotiation
        public ActionResult InitNegotiate(int? id)
        {
            var user = userService.GetUserByName(User.Identity.Name);
            if(user == null) return HttpNotFound();

            SalePublishing buyHouse;
            if (salePublishingService.GetSellPublishingById(id, out buyHouse))
            {
                if(buyHouse == null) return HttpNotFound();

                TempData["SellPublishing"] = buyHouse.PublishingID;
                ViewBag.Action = "InitNegotiate";
                return View();
            }
            else return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        //POST Negotiation
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Client")]
        public ActionResult InitNegotiate([Bind(Include = "BuyRequestID,RequestPrice,LastModification,State,SellData,Client")] BuyRequest buyRequest)
        {
            var client = userService.GetUserByName(User.Identity.Name);
            if(client == null) return HttpNotFound();

            SalePublishing sell;
            if(salePublishingService.GetSellPublishingById((int)TempData["SellPublishing"], out sell))
            {
                if (sell == null) return HttpNotFound();

                buyService.AddBuyRequest(buyRequest, client, sell, true);

                return RedirectToAction("ListBuyClient");
            }
            else return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        // GET
        public ActionResult InitBuy(int? id)
        {
            var user = userService.GetUserByName(User.Identity.Name);
            if (user == null) return HttpNotFound();

            SalePublishing buyHouse;
            if (salePublishingService.GetSellPublishingById(id, out buyHouse))
            {
                if(buyHouse == null) return HttpNotFound();

                var buyRequest = new BuyRequest();
                buyService.AddBuyRequest(buyRequest, user, buyHouse, false);
                return RedirectToAction("ListBuyClient");
            }
            else return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        #endregion

        #region FINAL STATES
        [Authorize(Roles = "Client,Realtor")]
        public ActionResult Reject(int? id)
        {
            BuyRequest request;
            if(buyService.GetBuyRequestById(id, out request))
            {
                if (request == null) return HttpNotFound();

                request.ToReject();
                buyService.EditBuyRequest(request);
                return Redirect(request);
            }
            else return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        [Authorize(Roles = "Client")]
        public ActionResult Buy(int? id)
        {
            BuyRequest request;
            if (buyService.GetBuyRequestById(id, out request))
            {
                if (request == null) return HttpNotFound();

                var rentSell = rentRequestService.GetRentRequestsToSell(DateTime.Now, request.SellData.House.HouseID).Count();

                if (rentSell > 0)
                    return View("BuyError");

                request.ToSell();
                
                var buyRegistry = new SellRegistry { Price = request.RequestPrice, Client = request.Client, SellPublishing = request.SellData };
                saleRegistryService.TempAddSellRegistry(buyRegistry);

                // Agrega a la contablidad la nueva compra
                var dailySit = new DailySit();
                dailySit.Date = DateTime.Now;
                dailySit.Value = (request.SellData.CommissionPercent * request.RequestPrice) / 100;

                sitService.TempAddDailySit(dailySit);

                var concept = conceptService.GetConceptByName("Venta Casa");

                dailySit.TypeOfCompensation = concept;

                // Rechaza todas las peticiones de compra que no sean esta
                var queryBuyRequests = buyService.GetBuyRequestsByRequest(request);
                foreach (var buyReject in queryBuyRequests)
                {
                    buyReject.ToReject();
                    buyService.TempEditBuyRequest(buyReject);
                }

                // Rechaza todas las peticiones de renta que no se encuentren en un estado final
                var queryRentRequests = rentRequestService.GetRentRequestsByHouseId(request.SellData.House.HouseID);
                foreach (var rentReject in queryRentRequests)
                {
                    rentReject.ToReject();
                    rentRequestService.TempEditRentRequest(rentReject);
                }

                // Pone no visibles las publicaciones de renta de la casa
                var queryRentPublishes = rentPublishingService.GetRentPublishingsByHouseId(request.SellData.House.HouseID);
                foreach (var item in queryRentPublishes)
                {
                    item.Showable = false;
                    rentPublishingService.TempEditRentPublishing(item);
                }

                // Pone no visibles las publicaciones de venta de la casa
                var querySellPublishes = salePublishingService.GetSalePublishingsByHouseId(request.SellData.House.HouseID);
                foreach (var item in querySellPublishes)
                {
                    item.Showable = false;
                    salePublishingService.TempEditSellPublishing(item);
                }

                // Cambia el dueño de la casa al usuario que la compró
                House house;
                houseService.GetHouseById(request.SellData.House.HouseID, out house);
                house.Owner = request.Client;

                idb.SaveChanges();
                return Redirect(request);
            }
            else return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        #endregion

        #region PENDING TO REQUEST
        // GET: Realtor want to negotiate with the client
        [Authorize(Roles = "Realtor")]
        public ActionResult RealtorNegotiateClient(int? id)
        {
            BuyRequest buyRequest;
            if(buyService.GetBuyRequestById(id, out buyRequest))
            {
                if (buyRequest == null) return HttpNotFound();

                TempData["Price"] = buyRequest.RequestPrice;
                TempData["SellDataId"] = buyRequest.SellData.PublishingID;
                ViewBag.Action = "RealtorNegotiateClient";

                return View("InitNegotiate", buyRequest);
            }
            else return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        // POST: Realtor change the client's proposal
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Realtor")]
        public ActionResult RealtorNegotiateClient([Bind(Include = "BuyRequestID,RequestPrice,LastModification,State,SellData,Client")] BuyRequest buyRequest)
        {
            if (ModelState.IsValid)
            {
                buyRequest.PendingToRequest((double)TempData["Price"]);

                buyService.EditBuyRequest(buyRequest);

                return RedirectToAction("ListSellRealtor", new { id = (int)TempData["SellDataId"]});
            }
            return View(buyRequest);
        }

        public ActionResult RealtorPassClient(int? id)
        {
            BuyRequest buyRequest;
            if (buyService.GetBuyRequestById(id, out buyRequest))
            {
                if (buyRequest == null) return HttpNotFound();

                buyRequest.PendingToRequest();
                buyService.EditBuyRequest(buyRequest);

                return RedirectToAction("ListSellRealtor", new { id = buyRequest.SellData.PublishingID });
            }
            else return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        #endregion

        #region PENDING TO NEGOTIATION
        // GET: Realtor want to negotiate with the client
        [Authorize(Roles = "Realtor")]
        public ActionResult RealtorNegotiateOwner(int? id)
        {
            BuyRequest buyRequest;
            if (buyService.GetBuyRequestById(id, out buyRequest))
            {
                if (buyRequest == null) return HttpNotFound();

                TempData["Price"] = buyRequest.RequestPrice;
                ViewBag.Action = "RealtorNegotiateOwner";
                TempData["SellDataId"] = buyRequest.SellData.PublishingID;

                return View("InitNegotiate", buyRequest);
            }
            else return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        // POST: Realtor change the client's proposal
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Realtor")]
        public ActionResult RealtorNegotiateOwner([Bind(Include = "BuyRequestID,RequestPrice,LastModification,State,SellData,Client")] BuyRequest buyRequest)
        {
            if (ModelState.IsValid)
            {
                buyRequest.PendingToNegotiation((double)TempData["Price"]);

                buyService.EditBuyRequest(buyRequest);
                
                return RedirectToAction("ListSellRealtor", new { id = (int)TempData["SellDataId"] });
            }
            return View(buyRequest);
        }

        public ActionResult RealtorPassOwner(int? id)
        {
            BuyRequest buyRequest;
            if (buyService.GetBuyRequestById(id, out buyRequest))
            {
                if (buyRequest == null) return HttpNotFound();

                buyRequest.PendingToNegotiation();

                buyService.EditBuyRequest(buyRequest);

                return RedirectToAction("ListSellRealtor", new { id = buyRequest.SellData.PublishingID });
            }
            else return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        #endregion

        #region TO PENDING
        // GET: Realtor want to negotiate with the client
        [Authorize(Roles = "Client")]
        public ActionResult ClientToRealtor(int? id)
        {
            BuyRequest buyRequest;
            if (buyService.GetBuyRequestById(id, out buyRequest))
            {
                if (buyRequest == null) return HttpNotFound();

                TempData["Price"] = buyRequest.RequestPrice;
                ViewBag.Action = "ClientToRealtor";

                return View("InitNegotiate", buyRequest);
            }
            else return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        // POST: Realtor change the client's proposal
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Client")]
        public ActionResult ClientToRealtor([Bind(Include = "BuyRequestID,RequestPrice,LastModification,State,SellData,Client")] BuyRequest buyRequest)
        {
            if (ModelState.IsValid)
            {
                buyRequest.RequestToPending((double)TempData["Price"]);

                buyService.EditBuyRequest(buyRequest);
                               
                return RedirectToAction("ListBuyClient");
            }
            return View(buyRequest);
        }

        // GET: Realtor want to negotiate with the client
        [Authorize(Roles = "Client")]
        public ActionResult OwnerToRealtor(int? id)
        {
            BuyRequest buyRequest;
            if (buyService.GetBuyRequestById(id, out buyRequest))
            {
                if (buyRequest == null) return HttpNotFound();

                TempData["Price"] = buyRequest.RequestPrice;
                ViewBag.Action = "OwnerToRealtor";
                TempData["SellDataId"] = buyRequest.SellData.PublishingID;

                return View("InitNegotiate", buyRequest);
            }
            else return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        // POST: Realtor change the client's proposal
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Client")]
        public ActionResult OwnerToRealtor([Bind(Include = "BuyRequestID,RequestPrice,LastModification,State,SellData,Client")] BuyRequest buyRequest)
        {
            if (ModelState.IsValid)
            {
                buyRequest.NegotiationToPending((double)TempData["Price"]);

                buyService.EditBuyRequest(buyRequest);
                
                return RedirectToAction("ListSellOwner", new { id = (int)TempData["SellDataId"] });
            }
            return View(buyRequest);
        }
        #endregion

        public ActionResult Redirect(BuyRequest buyRequest)
        {
            if (User.IsInRole("Realtor"))
                return RedirectToAction("ListSellRealtor", new { id = buyRequest.SellData.PublishingID });
            else if (buyRequest.Client.Name == User.Identity.Name)
                return RedirectToAction("ListBuyClient");
            else return RedirectToAction("ListSellOwner", new { id = buyRequest.SellData.PublishingID });
        }

        public ActionResult Details(int? id)
        {
            BuyRequest buyRequest;
            if (buyService.GetBuyRequestById(id, out buyRequest))
            {
                if (buyRequest == null) return HttpNotFound();

                return View(buyRequest);
            }
            else return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
    }
}
