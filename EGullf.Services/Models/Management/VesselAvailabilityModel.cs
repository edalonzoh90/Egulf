using EGullf.Services.Models.Configuration;
using System;
using System.ComponentModel.DataAnnotations;

namespace EGullf.Services.Models.Management
{
    public class VesselAvailabilityModel
    {
        public static int DEFAULT = 1;

        public int? AvailabilityVesselId { get; set; }

        [Required]
        public int? VesselId { get; set; }

        [Required]
        public int? ReasonId { get; set; }
        public string Reason { get; set; }
        public string ReasonDescription { get; set; }

        [Required]
        public DateTime? StartDate { get; set; }

        [Required]
        public DateTime? EndDate { get; set; }
    }
}
