using EGullf.Services.Models.Management;
using EGullf.Services.Models.Operation;

namespace EGULF.Models
{
    public class VesselAvailabilityViewModel : VesselAvailabilityModel
    {
        public int VesselEstatusId { get; set; }

        public VesselAvailabilityModel ToVesselAvailabilityModel()
        {
            return new VesselAvailabilityModel()
            {
                AvailabilityVesselId = AvailabilityVesselId,
                EndDate = EndDate,
                Reason = Reason,
                ReasonDescription = ReasonDescription,
                ReasonId = ReasonId,
                StartDate = StartDate,
                VesselId = VesselId
            };
        }

        public VesselModel ToVesselModel()
        {
            return new VesselModel()
            {
                VesselId = VesselId,
                Status = VesselEstatusId
            };
        }
    }
}