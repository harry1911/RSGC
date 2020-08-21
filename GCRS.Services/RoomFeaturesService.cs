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
    public class RoomFeaturesService
    {
        public IDB idb;
        public RoomFeaturesService(IDB idb)
        {
            this.idb = idb;
        }
        public IEnumerable<RoomFeature> ListAllRoomFeatures()
        {
            return idb.GetRepo<RoomFeature>().GetAll();
        }
        public bool GetRoomFeatureById(int? id, out RoomFeature feature)
        {
            feature = null;
            if (id != null)
                feature = idb.GetRepo<RoomFeature>().Find(id.Value);
            return id != null;
        }
        public void AddRoomFeature(RoomFeature feature)
        {
            idb.GetRepo<RoomFeature>().Add(feature);
            idb.SaveChanges();
        }
        public void EditRoomFeature(RoomFeature feature)
        {
            idb.GetRepo<RoomFeature>().Edit(feature);
            idb.SaveChanges();
        }
        public void DeleteRoomFeature(int id)
        {
            idb.GetRepo<RoomFeature>().Remove(id);
            idb.SaveChanges();
        }

        public IOrderedQueryable<RoomFeature> GetRoomFeaturesSelectList()
        {
            return from d in idb.GetRepo<RoomFeature>().Query() orderby d.Name select d;
        }
    }
}
