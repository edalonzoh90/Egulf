using EGullf.Services.Models.Operation;

namespace EGULF.Models
{
    public class VesselViewModel
    {
        public VesselViewModel()
        {
            Vessel = new VesselModel();
            VesselSpecificInfo = new VesselSpecificInfoModel();
            SpecificInfo = new SpecificInformationModel();
            VesselCost = new VesselCostModel();
        }
        public VesselModel Vessel { get; set; }
        public VesselSpecificInfoModel VesselSpecificInfo { get; set; }
        public SpecificInformationModel SpecificInfo { get; set; }
        public VesselCostModel VesselCost { get; set; }
    }
}