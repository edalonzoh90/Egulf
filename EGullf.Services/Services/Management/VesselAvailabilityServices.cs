using EGullf.Services.DA.Management;
using EGullf.Services.Models.Management;
using EGullf.Services.Models.Utils;
using System.Collections.Generic;
using System.Linq;

namespace EGullf.Services.Services.Management
{
    public class VesselAvailabilityServices
    {
        public RequestResult<VesselAvailabilityModel> InsUpd(VesselAvailabilityModel data)
        {
            VesselAvailabilityDA da = new VesselAvailabilityDA();
            return da.InsUpd(data);
        }

        public List<VesselAvailabilityModel> Get(VesselAvailabilityModel filters)
        {
            VesselAvailabilityDA da = new VesselAvailabilityDA();
            return da.Get(filters);
        }

        public VesselAvailabilityModel GetFirst(VesselAvailabilityModel filters)
        {
            return Get(filters).FirstOrDefault();
        }

        public RequestResult<VesselAvailabilityModel> Del(int Id)
        {
            VesselAvailabilityDA da = new VesselAvailabilityDA();
            return da.Del(Id);
        }
    }
}
