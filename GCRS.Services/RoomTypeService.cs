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
    public class RoomTypeService
    {
        public IDB idb;
        public RoomTypeService(IDB idb) {
            this.idb = idb;
        }
        public IEnumerable<RoomType> ListAllRoomTypes()
        {
            return idb.GetRepo<RoomType>().GetAll();
        }
        public bool GetRoomTypeById(int? id, out RoomType type)
        {
            type = null;
            if (id != null)
                type = idb.GetRepo<RoomType>().Find(id.Value);
            return id != null;
        }
        public void AddRoomType(RoomType type)
        {
            idb.GetRepo<RoomType>().Add(type);
            idb.SaveChanges();
        }
        public void EditRoomType(RoomType type)
        {
            idb.GetRepo<RoomType>().Edit(type);
            idb.SaveChanges();
        }
        public void DeleteRoomType(int id)
        {
            idb.GetRepo<RoomType>().Remove(id);
            idb.SaveChanges();
        }
        public IOrderedQueryable<RoomType> GetRoomTypesSelectList()
        {
            return from d in idb.GetRepo<RoomType>().Query() orderby d.Name select d;
        }
    }
}
