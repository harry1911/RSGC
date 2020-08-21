using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GCRS.Base.APIDatabase
{
    public class Repo<T> : IRepo<T> where T:class
    {
        public DbContext db;

        public Repo(IDB db)
        {
            this.db =(DbContext)db;
        }

        public void Add(T t)
        {
            db.Set<T>().Add(t);
            //db.SaveChanges();
        }

        public void Edit(T t)
        {
            db.Entry(t).State = EntityState.Modified;
            //db.SaveChanges();
        }

        public void Remove(int id)
        {
            T t = Find(id);
            db.Set<T>().Remove(t);
            //db.SaveChanges();
        }

        public T Find(int id)
        {
            return db.Set<T>().Find(id);
        }

        public IEnumerable<T> GetAll()
        {
            return db.Set<T>();
        }
        public IQueryable<T> Query(params Expression<Func<T,object>>[] includs)
        {
            var query= db.Set<T>().AsQueryable();
            if (includs != null)
            {
                query = includs.Aggregate(query, (pres, current) => pres.Include(current));
            }
            return query;
        }

    }
}
