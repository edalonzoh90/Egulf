using EGullf.Services.Models.Management;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGullf.Services.DA.Management
{
    public class ReasonAvailabilityDA
    {
        public ReasonAvailabilityModel GetById(int id)
        {
            return
                Get(new ReasonAvailabilityModel()
                {
                    ReasonAvailabilityId = id
                }).FirstOrDefault();
        }

        public List<ReasonAvailabilityModel> Get(ReasonAvailabilityModel filter)
        {
            using (var db = new EGULFEntities())
            {
                return (from ct in db.ReasonAvailability
                        where ct.ReasonAvailabilityId == filter.ReasonAvailabilityId
                        || filter.ReasonAvailabilityId == null
                        select new ReasonAvailabilityModel
                        {
                            ReasonAvailabilityId = ct.ReasonAvailabilityId,
                            Reason = ct.Reason
                        }).OrderBy(x => x.Reason).ToList();
            }
        }
    }
}
