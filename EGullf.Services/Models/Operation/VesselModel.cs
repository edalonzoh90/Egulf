using EGullf.Services.Models.AzureStorage;
using EGullf.Services.Models.Configuration;
using EGullf.Services.Models.Management;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EGullf.Services.Models.Operation
{
    public class VesselModel : AuditModel
    {
        public VesselModel()
        {
            Country = new CountryModel();
            ClasificationSociety = new ClasificationSocietyModel();
            VesselType = new VesselTypeModel();
            Suitability = new List<ProjectTypeModel>();
            HomePort = new PortModel();
            Image = new FileModel();
            Company = new CompanyModel();
            Location = new LatLng();
            VesselCost = new VesselCostModel();
        }

        public VesselCostModel VesselCost { get; set; }

        public string SuitabilityIds { get; set; }

        public string SuitabilityDescription { get; set; }

        public int? VesselId { get; set; }

        public int? Status { get; set; }

        public CompanyModel Company { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(50)]
        public string Imo { get; set; }

        [Required]
        [Range(1900, 9999)]
        public int? YearBuild { get; set; }

        public CountryModel Country { get; set; }

        public ClasificationSocietyModel ClasificationSociety { get; set; }

        [Required]
        [StringLength(50)]
        public string ClassNotation { get; set; }

        [Required]
        public DateTime? ClassValidity { get; set; }

        [Required]
        public VesselTypeModel VesselType { get; set; }

        public List<ProjectTypeModel> Suitability { get; set; }


        [Required]
        public PortModel HomePort { get; set; }

        public FileModel Image { get; set; }

        public LatLng Location { get; set; }
    }

    public class VesselSpecificInfoModel : AuditModel
    {

        public int? VesselId { set; get; }

        [Required]
        public decimal? GrossTonnage { set; get; }

        [Required]
        public decimal? NetTonnage { set; get; }

        [Required]
        public decimal? BeamOverall { set; get; }

        [Required]
        public decimal? LengthOverall { set; get; }

        public decimal? MaximumLoadedDraft { set; get; }

        public decimal? FreeDeckArea { set; get; }

        public decimal? DeckStrenght { set; get; }

        public decimal? FreshWaterCapacity { set; get; }

        public decimal? FuelOilCapacity { set; get; }

        public decimal? BallastWaterCapacity { set; get; }

        public decimal? MudCapacity { set; get; }

        public decimal? CementTanksCapacity { set; get; }

        public decimal? OilRecoveryCapacity { set; get; }


        public decimal? WaterMarkerPlant { set; get; }


        public decimal? HotWaterCalorifier { set; get; }


        public decimal? SewageTreatmentPlant { set; get; }

        public decimal? CruisingSpeed { set; get; }

        public decimal? MaximumSpeed { set; get; }

        public decimal? DistanceCruisingSpeed { set; get; }

        public decimal? DistanceMaxSpeed { set; get; }

        public decimal? FuelConsumptionCruisingSpeed { set; get; }

        public decimal? FuelConsumptionMaxSpeed { set; get; }


        public int? DynamicPositionSystem { set; get; }

        public string DynamicPositionSystemName { get; set; }

    }

    public class VesselCostModel
    {
        public int? VesselId { set; get; }

        public decimal? LodgingCost { set; get; }

        public decimal? MealCost { set; get; }

        public decimal? LaundryCost { set; get; }

        public decimal? DailyRateTowing { set; get; }

        public decimal? DailyRatePersonnelTransportation { set; get; }

        public decimal? DailyRateMaterialTransportation { set; get; }

        public decimal? DailyRateFlotel { set; get; }

        public int? UserModifiedId { set; get; }

        public decimal? DailyMaxRate { get; set; }
    }
}
