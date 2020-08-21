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
    public class RentPublishingService
    {
        private Repo<RentPublishing> db;
        private IDB idb;

        public RentPublishingService(IDB idb)
        {
            this.idb = idb;
            db = new Repo<RentPublishing>(idb);
        }

        public IEnumerable<RentPublishing> GetRentPublishingByUserId(int id) => idb.GetRepo<RentPublishing>().Query(x => x.House, x => x.Realtor, x => x.User, x => x.Images, x => x.Rooms).Where(x => x.User.UserID == id).ToList();

        public IEnumerable<RentPublishing> GetRentPublishingsByHouseId(int id) => idb.GetRepo<RentPublishing>().Query(x => x.House, x => x.Realtor, x => x.User, x => x.Images, x => x.Rooms).Where(x => x.House.HouseID == id);

        public bool GetRentPublishingById(int? id, out RentPublishing rentPublishing)
        {
            rentPublishing = null;
            if (id != null)
                rentPublishing = idb.GetRepo<RentPublishing>().Query(x => x.House, x => x.Realtor, x => x.User, x => x.Images, x => x.Rooms, x => x.Rents).SingleOrDefault(x => x.PublishingID == id);
            return id != null;
        }

        public IEnumerable<RentPublishing> GetRentPublishingByRealtorId(int id) => idb.GetRepo<RentPublishing>().Query(x => x.House, x => x.Realtor, x => x.User, x => x.Images, x => x.Rooms).Where(x => x.Realtor.UserID == id).ToList();

        public void TempAddRentPublishing(RentPublishing rentPublishing) => db.Add(rentPublishing);

        public void TempEditRentPublishing(RentPublishing rentPublishing) => db.Edit(rentPublishing);
    }
}
