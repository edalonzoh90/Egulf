using EGullf.Services.Models.Management;
using System.Collections.Generic;
using System.Linq;

namespace EGullf.Services.DA.Management
{
    public class PortDA
    {
        public PortModel GetById(int id)
        {
            return
                Get(new PortModel()
                {
                    PortId = id
                }).FirstOrDefault();
        }

        public List<PortModel> Get(PortModel filter)
        {
            using (var db = new EGULFEntities())
            {
                return (from ct in db.Port
                        where ct.PortId == filter.PortId
                        || filter.PortId == null
                        select new PortModel
                        {
                            PortId = ct.PortId,
                            Name = ct.Name,
                            Region = new RegionModel()
                            {
                                RegionId = ct.RegionId
                            }
                        }).ToList();
            }
        }
    }
}
