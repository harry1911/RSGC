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
    public class PrivilegesService
    {
         IDB idb;

        public PrivilegesService(IDB idb)
        {
            this.idb = idb;
        }

        public IEnumerable<Privileges> ListAllPrivileges()
        {
            return idb.GetRepo<Privileges>().GetAll();
        }
        public bool GetPrivilegeById(int? id, out Privileges priv)
        {
            priv = null;
            if (id != null)
                priv = idb.GetRepo<Privileges>().Find(id.Value);
            return id != null;
        }
        public void AddPrivilege(Privileges priv)
        {
            idb.GetRepo<Privileges>().Add(priv);
            idb.SaveChanges();
        }
        public void EditPrivilege(Privileges priv)
        {
            idb.GetRepo<Privileges>().Edit(priv);
            idb.SaveChanges();
        }
        public void DeletePrivilege(int id)
        {
            idb.GetRepo<Privileges>().Remove(id);
            idb.SaveChanges();
        }
        public IOrderedQueryable<Privileges> GetPrivilegesSelectList()
        {
            return from d in idb.GetRepo<Privileges>().Query() orderby d.Name select d;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
