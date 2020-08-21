using GCRS.Accounting;
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
    public class DailySitService
    {
        private Repo<DailySit> db;
        private IDB idb;

        public DailySitService(IDB idb)
        {
            this.idb = idb;
            db = new Repo<DailySit>(idb);
        }

        public IEnumerable<DailySit> ListAll()
        {
            return db.GetAll().ToList();
        }

        public DailySit FindById(int id)
        {
            return db.Find(id);
        }

        public void AddConcept(DailySit dailySit)
        {
            db.Add(dailySit);
            idb.SaveChanges();
        }

        public void EditConcept(DailySit dailySit)
        {
            db.Edit(dailySit);
            idb.SaveChanges();
        }

        public void RemoveConcept(DailySit dailySit)
        {
            db.Remove(dailySit.ID);
            idb.SaveChanges();
        }

        public void TempAddDailySit(DailySit dailySit) => db.Add(dailySit);
    }
}
