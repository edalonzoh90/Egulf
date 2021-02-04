using EGullf.Services.DA.Management;
using EGullf.Services.Models.Configuration;
using EGullf.Services.Models.Management;
using System.Collections.Generic;
using System.Linq;

namespace EGullf.Services.Services.Management
{
    public class ReasonAvailabilityServices
    {
        public ReasonAvailabilityModel GetById(int id)
        {
            ReasonAvailabilityDA da = new ReasonAvailabilityDA();
            return da.GetById(id);
        }

        public List<ReasonAvailabilityModel> Get(ReasonAvailabilityModel filter)
        {
            ReasonAvailabilityDA da = new ReasonAvailabilityDA();
            return da.Get(filter);
        }

        public List<SelectModel> GetSelect(string resource)
        {
            List<ReasonAvailabilityModel> lst = Get(new ReasonAvailabilityModel());
            List<SelectModel> resp = lst
                .Select(x => new SelectModel { Value = x.ReasonAvailabilityId.ToString(), Text = x.Reason })
                .OrderBy(x => x.Text)
                .ToList();

            resp.Insert(0, new SelectModel()
            {
                Value = null,
                Text = resource
            });

            return resp;
        }
    }
}
