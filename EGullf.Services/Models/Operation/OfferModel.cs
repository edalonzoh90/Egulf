using EGullf.Services.Models.Configuration;
using EGullf.Services.Models.Management;
using System;

namespace EGullf.Services.Models.Operation
{
    public class OfferModel : AuditModel
    {
        public static int NEW = 1;
        public static int ACCEPTED = 2;
        public static int REJECTED = 3;
        public static int CANCELED = 4;
        public static int FIX = 5;

        public OfferModel()
        {
            Vessel = new VesselModel();
            Project = new ProjectModel();
            VesselAdmin = new PersonModel();
            ProjectAdmin = new PersonModel();
            VesselSpecificInfoModel = new VesselSpecificInfoModel();
            VesselSpecificInfoModelExtra = new SpecificInformationModel();
        }

        public int? OfferId { get; set; }
        public int? MessageNotReaded { get; set; }
        public VesselModel Vessel { get; set; }
        public VesselSpecificInfoModel VesselSpecificInfoModel { get; set; }
        public SpecificInformationModel VesselSpecificInfoModelExtra { get; set; }
        public ProjectModel Project { get; set; }
        public int? Status { get; set; }
        public PersonModel VesselAdmin { get; set; }
        public PersonModel ProjectAdmin { get; set; }
        public DateTime? OfferedDate { get; set; }
        public int? OfferReferenceId { get; set; }
        public OfferCostModel OfferCost { get; set; }
        public string Comment { get; set; }

        public int? UserId { get; set; }
        public bool? OfferReceived { get; set; }
        public int? Type { get; set; }
    }
}
