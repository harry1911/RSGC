using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCRS.Base.IRepositories
{
    public interface IPrivileges
    {
        IEnumerable<Privileges> ListAllPrivileges();
        bool GetPrivilegeById(int? id, out Privileges priv);
        void AddPrivilege(Privileges priv);
        void EditPrivilege(Privileges priv);
        void DeletePrivilege(int id);
        IOrderedQueryable<Privileges> GetPrivilegesSelectList();
    }
}
