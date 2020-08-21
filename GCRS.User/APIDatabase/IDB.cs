using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCRS.Base.APIDatabase
{
    public interface IDB
    {
        int SaveChanges();
        IRepo<T> GetRepo<T>()where T: class;
    }
}
