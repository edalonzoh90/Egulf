using System.ComponentModel.DataAnnotations;

namespace EGullf.Services.Models.Management
{
    public class VesselTypeModel
    {
        [Required]
        public int? VesselTypeId { get; set; }
        public string Name { get; set; }
        public string Acronym { get; set; }
    }
}
