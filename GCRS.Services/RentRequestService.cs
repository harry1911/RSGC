using GCRS.Base;
using GCRS.Base.APIDatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCRS.Services
{
    public class RentRequestService
    {
        Repo<RentRequest> rentRepo;
        IDB idb;
        public RentRequestService(IDB idb)
        {
            this.idb = idb;
            rentRepo = new Repo<RentRequest>(idb);
        }
        public bool GetRentRequestsNotInFinalStates(int? id, out IEnumerable<RentRequest> rentRequests)
        {
            rentRequests = new List<RentRequest>();
            if (id != null)
                rentRequests = rentRepo.Query(x => x.Publishing).Where(x => x.Publishing.PublishingID == id && x.RentState != RentRequest.Estados.Reject && x.RentState != RentRequest.Estados.Accept).ToList();
            return id != null;
        }

        public IEnumerable<RentRequest> GetRentRequestsByHouseId(int id) => rentRepo.Query(x => x.Publishing).Where(x => x.RentState != RentRequest.Estados.Reject && x.RentState != RentRequest.Estados.Accept && x.Publishing.House.HouseID == id);

        public IEnumerable<RentRequest> GetRentRequestsToSell(DateTime date, int id) => rentRepo.Query(x => x.Publishing).Where(x => x.RentState == RentRequest.Estados.Accept && x.Publishing.House.HouseID == id).ToList().Where(x => (date.Date > x.Start.Date && date.Date < x.Finish.Date));

        public IEnumerable<RentRequest> GetRentRequestToReject(int idPublishing, int idRequest) => rentRepo.Query(x => x.Publishing).Where(x => x.RentState != RentRequest.Estados.Reject && x.RentState != RentRequest.Estados.Accept && x.Publishing.PublishingID == idPublishing && x.RentRequestID != idRequest);

        public IEnumerable<RentRequest> GetRentRequestsByClientId(int id) => rentRepo.Query(x => x.Publishing, x => x.Services,x => x.Client).Where(x => x.Client.UserID == id).ToList();

        public bool GetRentRequestById(int? id, out RentRequest rentRequest)
        {
            rentRequest = null;
            if (id != null)
                rentRequest = rentRepo.Query(x => x.Client,x => x.Services,x => x.Publishing).SingleOrDefault(x => x.RentRequestID == id);
            return id != null;
        }

        public void TempAddRentRequest(RentRequest rentRequest) => rentRepo.Add(rentRequest);

        public void TempEditRentRequest(RentRequest rentRequest) => rentRepo.Edit(rentRequest);

        public void EditRentRequest(RentRequest rentRequest)
        {
            rentRepo.Edit(rentRequest);
            idb.SaveChanges();
        }
    }
}
