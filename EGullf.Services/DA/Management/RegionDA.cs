using EGullf.Services.Models.Management;
using System.Collections.Generic;
using System.Linq;

namespace EGullf.Services.DA.Management
{
    public class RegionDA
    {
        public RegionModel GetById(int id)
        {
            return
                Get(new RegionModel()
                {
                    RegionId = id
                }).FirstOrDefault();
        }

        public List<RegionModel> Get(RegionModel filter)
        {
            using (var db = new EGULFEntities())
            {
                return (from ct in db.Region
                        where ct.RegionId == filter.RegionId
                        || filter.RegionId == null
                        select new RegionModel
                        {
                            RegionId = ct.RegionId,
                            Name = ct.Name,
                            Description = ct.Description,
                            Coordenates = ct.RegionCoordinates.Select(x => new LatLng { index = x.Index, Lat = x.Lat, Lng = x.Lng }).ToList()
                        }).ToList();
            }
        }
    }
}
