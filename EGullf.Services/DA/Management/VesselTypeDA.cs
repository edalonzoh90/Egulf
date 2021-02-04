using EGullf.Services.Models.Management;
using System.Collections.Generic;
using System.Linq;

namespace EGullf.Services.DA.Management
{
    public class VesselTypeDA
    {
        public VesselTypeModel GetById(int id)
        {
            return
                Get(new VesselTypeModel()
                {
                    VesselTypeId = id
                }).FirstOrDefault();
        }

        public List<VesselTypeModel> Get(VesselTypeModel filter)
        {
            using (var db = new EGULFEntities())
            {
                return (from ct in db.VesselType
                        where ct.VesselTypeId == filter.VesselTypeId
                        || filter.VesselTypeId == null
                        select new VesselTypeModel
                        {
                            VesselTypeId = ct.VesselTypeId,
                            Name = ct.Name,
                            Acronym = ct.Acronym
                        }).ToList();
            }
        }
    }
}
