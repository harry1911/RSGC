using GCRS.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using GCRS.Base.IRepositories;
using System.Data;
using GCRS.Base.APIDatabase;

namespace GCRS.Services
{
    public class RoomService
    {
        private IDB idb;

        public RoomService(IDB idb)
        {
            this.idb = idb;
        }

        public IEnumerable<Room> ListAllRooms()
        {
            return idb.GetRepo<Room>().Query(x => x.RoomType).ToList();
        }
        public bool GetRoomById(int? id, out Room room)
        {
            room = null;
            if (id != null)
                room = idb.GetRepo<Room>().Query(x => x.RoomType).Where(x => x.RoomID == id).Single();
            return id != null;
        }
        public void AddRoom(Room room)
        {
            idb.GetRepo<Room>().Add(room);
            idb.SaveChanges();
        }
        public void EditRoom(Room room)
        {
            idb.GetRepo<Room>().Edit(room);
            idb.SaveChanges();
        }
        public void DeleteRoom(int id)
        {
            idb.GetRepo<Room>().Remove(id);
            idb.SaveChanges();
        }

        //public SelectList GetRoomsSelectList(object selectedRoom = null)
        //{
        //    var roomsQuery = from d in db.Rooms.Include(x => x.RoomType) orderby d.RoomType.Name select new { RoomID = d.RoomID, Name = d.RoomType.Name };
        //    return new SelectList(roomsQuery, "RoomID", "Name", selectedRoom);
        //}

        public ICollection<Room> CreateRooms(ICollection<Room> rooms)
        {
            List<Room> nRooms = null;
            if (rooms != null)
            {
                nRooms = new List<Room>();
                foreach (var room in rooms)
                {
                    var roomFeatures = new List<RoomFeature>();
                    foreach (var feature in room.Features)
                        roomFeatures.Add(idb.GetRepo<RoomFeature>().Find(feature.RoomFeatureID));
                    room.Features = null;
                    idb.GetRepo<Room>().Add(room);
                    room.Features = roomFeatures;
                    nRooms.Add(room);
                    //idb.SaveChanges();
                }
            }
            return nRooms;
        }
    }
}
