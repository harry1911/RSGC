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
    public class FeatureService
    {
        private Repo<Feature> db;
        private IDB idb;
        public FeatureService(IDB idb)
        {
            this.idb = idb;
            db = new Repo<Feature>(idb);
        }
        public FeatureService(Repo<Feature> db, IDB idb)
        {
            this.db = db;
            this.idb = idb;
        }
        public IEnumerable<Feature> ListAllFeatures()
        {
            return db.GetAll();
        }
        public bool GetFeatureById(int? id, out Feature feature)
        {
            feature = null;
            if (id != null)
                feature = db.Find(id.Value);
            return id != null;
        }
        public void AddFeature(Feature feature)
        {
            db.Add(feature);
            idb.SaveChanges();
        }
        public void EditFeature(Feature feature)
        {
            db.Edit(feature);
            idb.SaveChanges();
        }
        public void DeleteFeature(int id)
        {
            db.Remove(id);
            idb.SaveChanges();
        }
        public IOrderedQueryable<Feature> GetFeaturesSelectList()
        {
            return from d in db.Query() orderby d.Name select d;
        }
        public ICollection<Feature> GetFeatures(ICollection<Feature> features)
        {
            List<Feature> feats = null;
            if (features != null)
            {
                feats = new List<Feature>();
                foreach (var feature in features)
                    feats.Add(db.Find(feature.FeatureID));
            }
            return feats;
        }

    }
}
