using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCRS.Base.IRepositories
{
    public interface IHouse
    {
        IEnumerable<House> ListAllHouses();
        IEnumerable<House> ListAllHousesByUser(string name, IUser iuser);
        bool GetHouseById(int? id, out House house);
        void AddHouse(House house);
        void EditHouse(House house);
        void DeleteHouse(int id);
        Dictionary<string, int> GetRoomsData(House house);
    }
}
