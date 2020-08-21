using GCRS.Base;
using GCRS.Base.APIDatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCRS.Services
{
    public class ServicesService
    {
        Repo<Service> repo;

        public ServicesService(IDB idb)
        {
            repo = new Repo<Service>(idb);
        }
        public IEnumerable<Service> AllServices() { 
           return repo.GetAll();
        }
        public List<Service> GetServicesByIds(int[] ids)
        {
            return repo.Query().Where(x => ids.Contains(x.ServiceID)).ToList();
        }
    }
}
