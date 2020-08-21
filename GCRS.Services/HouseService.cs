using GCRS.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using GCRS.Base.IRepositories;
using GCRS.Base.APIDatabase;

namespace GCRS.Services
{
    public class HouseService
    {
        private Repo<House> db;
        private Repo<Room> Roomdb;
        private IDB idb;

        public HouseService(IDB idb)
        {
            this.idb = idb;
            this.Roomdb = new Repo<Room>(idb);
            this.db = new Repo<House>(idb);
        }

        public IEnumerable<House> ListAllHouses()
        {
            return db.Query(x => x.Rooms, x => x.Features, x => x.Owner).ToList();
        }
        public IEnumerable<House> ListAllHousesByUser(string name)
        {
            return db.Query(x => x.Rooms,x => x.Features,x => x.Owner).Where(x => x.Owner.Name == name).ToList();
        }
        public bool GetHouseById(int? id, out House house)
        {
            house = null;
            if (id != null)
                house = db.Query(x => x.Rooms,x => x.Features,x => x.Owner).Where(x => x.HouseID == id).Single();
            return id != null;
        }
        public void AddHouse(House house)
        {
            db.Add(house);
            idb.SaveChanges();
        }
        public void EditHouse(House house)
        {
            db.Edit(house);
            idb.SaveChanges();
        }
        public void DeleteHouse(int id)
        {
            db.Remove(id);
            idb.SaveChanges();
        }
        public Dictionary<string, int> GetRoomsData(House house)
        {
            var rooms = Roomdb.Query(c => c.RoomType);
            Dictionary<string, int> data = new Dictionary<string, int>();
            foreach (var room in house.Rooms)
            {
                string name = rooms.Where(c => c.RoomID == room.RoomID).Single().RoomType.Name;
                if (data.ContainsKey(name))
                    data[name]++;
                else
                    data.Add(name, 1);
            }

            return data;
        }
     
    }
}
