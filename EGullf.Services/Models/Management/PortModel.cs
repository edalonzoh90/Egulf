using System.ComponentModel.DataAnnotations;

namespace EGullf.Services.Models.Management
{
    public class PortModel
    {
        public PortModel()
        {
            Region = new RegionModel();
        }

        [Required]
        public int? PortId { get; set; }
        public string Name { get; set; }
        public RegionModel Region { get; set; }
    }
}
