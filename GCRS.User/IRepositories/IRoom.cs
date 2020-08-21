using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCRS.Base.IRepositories
{
    public interface IRoom
    {
        IEnumerable<Room> ListAllRooms();
        bool GetRoomById(int? id, out Room room);
        void AddRoom(Room room);
        void EditRoom(Room room);
        void DeleteRoom(int id);
        ICollection<Room> CreateRooms(ICollection<Room> rooms);
    }
}
