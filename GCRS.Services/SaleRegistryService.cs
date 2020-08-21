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
    public class SaleRegistryService
    {
        private Repo<SellRegistry> db;
        private IDB idb;

        public SaleRegistryService(IDB idb)
        {
            this.idb = idb;
            db = new Repo<SellRegistry>(idb);
        }

        public void TempAddSellRegistry(SellRegistry sellRegistry)
        {
            db.Add(sellRegistry);
        }
    }
}
