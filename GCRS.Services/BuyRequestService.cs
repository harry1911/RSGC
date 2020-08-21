using GCRS.Base;
using GCRS.Base.APIDatabase;
using GCRS.Base.IRepositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCRS.Services
{
    public class BuyRequestService
    {
        private Repo<BuyRequest> db;
        private IDB idb;

        public BuyRequestService(IDB idb)
        {
            this.idb = idb;
            db = new Repo<BuyRequest>(idb);
        }

        public void EditBuyRequest(BuyRequest buyRequest)
        {
            db.Edit(buyRequest);
            idb.SaveChanges();
        }

        public void TempEditBuyRequest(BuyRequest buyRequest)
        {
            db.Edit(buyRequest);
        }

        public bool GetRealtorBuyRequests(int? publishingId, int realtorId, out IEnumerable<BuyRequest> buyRequests)
        {
            buyRequests = null;
            if (publishingId != null)
                buyRequests = idb.GetRepo<BuyRequest>().Query().Where(x => x.SellData.PublishingID == publishingId && x.SellData.Realtor.UserID == realtorId).ToList();
            return publishingId != null;
        }

        public bool GetOwnerBuyRequests(int? publishingId, int userId, out IEnumerable<BuyRequest> buyRequests)
        {
            buyRequests = null;
            if (publishingId != null)
                buyRequests = idb.GetRepo<BuyRequest>().Query().Where(x => x.SellData.PublishingID == publishingId && x.SellData.User.UserID == userId).ToList();
            return publishingId != null;
        }

        public IEnumerable<BuyRequest> GetClientBuyRequests(int userId)
        {
            var buyRequests = idb.GetRepo<BuyRequest>().Query().Where(x => x.Client.UserID == userId);
            return buyRequests.ToList();
        }

        public bool GetBuyRequestById(int? id, out BuyRequest buyRequest)
        {
            buyRequest = null;
            if (id != null)
                buyRequest = idb.GetRepo<BuyRequest>().Query(x => x.SellData, x => x.Client).SingleOrDefault(x => x.BuyRequestID == id);
            return id != null;
        }

        public IEnumerable<BuyRequest> GetBuyRequestsByRequest(BuyRequest buyRequest)
        {
            return idb.GetRepo<BuyRequest>().Query().Where(x => x.SellData.PublishingID == buyRequest.SellData.PublishingID && x.BuyRequestID != buyRequest.BuyRequestID);
        }

        public void AddBuyRequest(BuyRequest buyRequest, User client, SalePublishing sell, bool negotiation)
        {
            db.Add(buyRequest);
            buyRequest.InitToPending(client, sell, negotiation);
            idb.SaveChanges();
        }

        public void TempAddBuyRequest(BuyRequest buyRequest)
        {
            db.Add(buyRequest);
        }

    }
}
