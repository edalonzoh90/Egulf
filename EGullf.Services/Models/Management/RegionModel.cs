using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EGullf.Services.Models.Management
{
    public class RegionModel
    {
        [Required]
        public int? RegionId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<LatLng> Coordenates { get; set; }
    }

    public class LatLng
    {
        public int index { get; set; }
        public decimal Lat { get; set; }
        public decimal Lng { get; set; }
        public decimal[] ToLatLng
        {
            get
            {
                return new decimal[] { Lat, Lng };
            }
        }
    }
}
