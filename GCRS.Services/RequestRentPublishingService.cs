using GCRS.Base;
using GCRS.Base.APIDatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCRS.Services
{
    public class RequestRentPublishingService
    {
        Repo<RequestRentPublishing> reqRentRepo;
        IDB idb;
        public RequestRentPublishingService(IDB idb)
        {
            this.idb = idb;
            reqRentRepo = new Repo<RequestRentPublishing>(idb);
        }
        public IEnumerable<RequestRentPublishing> GetRentRequestPublishingByRealtorId(int id) =>reqRentRepo.Query(x => x.House,x => x.Realtor,x => x.User,x => x.Rooms).Where(x => x.Realtor.UserID == id).ToList();

        public IEnumerable<RequestRentPublishing> GetRentRequestPublishingByUserId(int id) => reqRentRepo.Query(x => x.House,x => x.Realtor,x => x.User,x => x.Rooms).Where(x => x.User.UserID == id).ToList();

        public IEnumerable<RequestRentPublishing> GetRentRequestPublishingWithoutRealtor() => reqRentRepo.Query(x => x.House,x => x.Realtor,x => x.User,x => x.Rooms).Where(x => x.RequestStatus != RequestStatus.Reject && x.Realtor == null).ToList();

        public bool GetRentRequestPublishingById(int? id, out RequestRentPublishing rentRequest)
        {
            rentRequest = null;
            if (id != null)
                rentRequest = reqRentRepo.Query(x => x.House,x => x.Realtor,x => x.User,x => x.Rooms,x => x.Images).SingleOrDefault(x => x.RequestPublishingID == id);
            return id != null;
        }

        public void TempAddRentRequestPublishing(RequestRentPublishing requestRentPublishing) => reqRentRepo.Add(requestRentPublishing);

        public void EditRentRequestPublishing(RequestRentPublishing requestRentPublishing)
        {
            reqRentRepo.Edit(requestRentPublishing);
            idb.SaveChanges();
        }

        public IEnumerable<RequestRentPublishing> GetAll()
        {
            return reqRentRepo.GetAll();
        }

        public void Add(RequestRentPublishing requestRentPublishing)
        {
            reqRentRepo.Add(requestRentPublishing);
            idb.SaveChanges();
        }
    }
}
