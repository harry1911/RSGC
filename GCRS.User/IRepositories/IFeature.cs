using GCRS.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCRS.Base.IRepositories
{
    public interface IFeature
    {
        IEnumerable<Feature> ListAllFeatures();
        bool GetFeatureById(int? id, out Feature feature);
        void AddFeature(Feature feature);
        void EditFeature(Feature feature);
        void DeleteFeature(int id);
        IOrderedQueryable<Feature> GetFeaturesSelectList();
        ICollection<Feature> GetFeatures(ICollection<Feature> features);
    }
}
