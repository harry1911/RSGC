using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GCRS.Base.APIDatabase
{
    public interface IRepo<T>where T:class
    {
        void Add(T t);
        void Edit(T t);
        void Remove(int id);
        T Find(int id);
        IEnumerable<T> GetAll();
        IQueryable<T> Query(params Expression<Func<T, object>>[] includs);
    }
}
