using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCRS.Base.IRepositories
{
    public interface IRoomFeature
    {
        IEnumerable<RoomFeature> ListAllRoomFeatures();
        bool GetRoomFeatureById(int? id, out RoomFeature feature);
        void AddRoomFeature(RoomFeature feature);
        void EditRoomFeature(RoomFeature feature);
        void DeleteRoomFeature(int id);
        IOrderedQueryable<RoomFeature> GetRoomFeaturesSelectList();
    }
}
