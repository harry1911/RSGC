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
using GCRS.Accounting;

namespace GCRS.Services
{
    public class ConceptService
    {
        private Repo<Concept> db;
        private IDB idb;

        public ConceptService(IDB idb)
        {
            this.idb = idb;
            db = new Repo<Concept>(idb);
        }

        public Concept GetConceptByName(string name)
        {
            //db.Concepts.AddOrUpdate(p => p.Name, new Concept { Name = name, Income = true });
            var concept = idb.GetRepo<Concept>().Query().SingleOrDefault(x => x.Name == name);
            if (concept == null)
            {
                concept = new Concept { Name = name, Income = true };
            }
            return concept;
        }

        public IEnumerable<Concept> ListAll()
        {
            return db.GetAll().ToList();
        }

        public Concept FindById(int id)
        {
            return db.Find(id);
        }

        public void AddConcept(Concept concept)
        {
            db.Add(concept);
            idb.SaveChanges();
        }

        public void EditConcept(Concept concept)
        {
            db.Edit(concept);
            idb.SaveChanges();
        }

        public void RemoveConcept(Concept concept)
        {
            db.Remove(concept.ConceptID);
            idb.SaveChanges();
        }
    }
}
