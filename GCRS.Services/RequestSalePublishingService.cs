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
    public class RequestSalePublishingService
    {
        private Repo<RequestSalePublishing> db;
        private IDB idb;

        public RequestSalePublishingService(IDB idb)
        {
            this.idb = idb;
            db = new Repo<RequestSalePublishing>(idb);
        }

        public IEnumerable<RequestSalePublishing> GetSellRequestByRealtorId(int id) => idb.GetRepo<RequestSalePublishing>().Query(x => x.Realtor, x => x.User, x => x.House).Where(x => x.Realtor.UserID == id);

        public IEnumerable<RequestSalePublishing> GetSellRequestByUserId(int id) => idb.GetRepo<RequestSalePublishing>().Query(x => x.Realtor, x => x.User, x => x.House).Where(x => x.User.UserID == id);

        public IEnumerable<RequestSalePublishing> GetNonAttendedSellRequests() => idb.GetRepo<RequestSalePublishing>().Query(x => x.Images, x => x.Realtor, x => x.User, x => x.House).Where(x => x.Realtor == null);

        public bool GetSellRequestById(int? id, out RequestSalePublishing sellRequest)
        {
            sellRequest = null;
            if (id != null)
                sellRequest = idb.GetRepo<RequestSalePublishing>().Query(x => x.Images, x => x.Realtor, x => x.User, x => x.House).SingleOrDefault(x => x.RequestPublishingID == id);
            return id != null;
        }

        public void TempAddSellRequest(RequestSalePublishing sellRequest) => db.Add(sellRequest);

        public void EditSellRequest(RequestSalePublishing sellRequest)
        {
            db.Edit(sellRequest);
            idb.SaveChanges();
        }
    }
}
