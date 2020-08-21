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
    public class RentRegistryService
    {
        private Repo<RentRegistry> db;
        private IDB idb;

        public RentRegistryService(IDB idb)
        {
            this.idb = idb;
            db = new Repo<RentRegistry>(idb);
        }

        public IEnumerable<RentRegistry> AllRentRegistries() => db.GetAll();

        public bool GetRentRegistryById(int? id, out RentRegistry rentRegistry)
        {
            rentRegistry = null;
            if (id != null)
                rentRegistry = idb.GetRepo<RentRegistry>().Query(x=>x.RentPublishing, x => x.Client, x => x.Services).SingleOrDefault(x => x.RegistryID == id);
            return id != null;
        }

        public void AddRentRegistry(RentRegistry rentRegistry)
        {
            db.Add(rentRegistry);
            idb.SaveChanges();
        }

        public void EditRentRegistry(RentRegistry rentRegistry)
        {
            db.Edit(rentRegistry);
            idb.SaveChanges();
        }

        public void RemoveRentRegistry(RentRegistry rentRegistry)
        {
            db.Remove(rentRegistry.RegistryID);
            idb.SaveChanges();
        }

        public void TempAddRentRegistry(RentRegistry rentRegistry) => db.Add(rentRegistry);
    }
}
