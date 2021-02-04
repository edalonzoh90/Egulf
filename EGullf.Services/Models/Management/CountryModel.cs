using System.ComponentModel.DataAnnotations;

namespace EGullf.Services.Models.Management
{
    public class CountryModel
    {
        [Required]
        public int? CountryId { get; set; }
        public string Name { get; set; }
        public string Acronym { get; set; }
    }
}
