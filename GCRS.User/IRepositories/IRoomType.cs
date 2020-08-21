using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCRS.Base.IRepositories
{
    public interface IRoomType
    {
        IEnumerable<RoomType> ListAllRoomTypes();
        bool GetRoomTypeById(int? id, out RoomType type);
        void AddRoomType(RoomType type);
        void EditRoomType(RoomType type);
        void DeleteRoomType(int id);
        IOrderedQueryable<RoomType> GetRoomTypesSelectList();
    }
}
