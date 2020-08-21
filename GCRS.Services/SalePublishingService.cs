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
    public class SalePublishingService
    {
        private Repo<SalePublishing> db;
        private IDB idb;

        public SalePublishingService(IDB idb)
        {
            this.idb = idb;
            db = new Repo<SalePublishing>(idb);
        }

        public bool GetSellPublishingById(int? id, out SalePublishing sellPublishing)
        {
            sellPublishing = null;
            if (id != null)
                sellPublishing = idb.GetRepo<SalePublishing>().Query(x => x.House, x => x.Realtor, x => x.User, x=>x.Images).SingleOrDefault(x => x.PublishingID == id);
            return id != null;
        }

        public IEnumerable<SalePublishing> GetSalePublishingsByHouseId(int id) => idb.GetRepo<SalePublishing>().Query(x => x.House, x => x.Realtor, x => x.User, x => x.Images).Where(x => x.House.HouseID == id);

        public IEnumerable<SalePublishing> GetSellPublishingByUserId(int id) => idb.GetRepo<SalePublishing>().Query(x => x.House, x => x.Realtor, x => x.User, x => x.Images).Where(x => x.User.UserID == id);

        public IEnumerable<SalePublishing> GetSellPublishingByRealtorId(int id) => idb.GetRepo<SalePublishing>().Query(x => x.House, x => x.Realtor, x => x.User, x => x.Images).Where(x => x.Realtor.UserID == id);

        public void AddSellPublishing(SalePublishing sellPublishing)
        {
            db.Add(sellPublishing);
            idb.SaveChanges();
        }

        public void TempEditSellPublishing(SalePublishing sellPublishing) => db.Edit(sellPublishing);
    }
}
