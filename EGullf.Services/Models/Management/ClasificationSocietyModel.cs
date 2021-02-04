using System.ComponentModel.DataAnnotations;

namespace EGullf.Services.Models.Management
{
    public class ClasificationSocietyModel
    {
        [Required]
        public int? ClasificationSocietyId { get; set; }
        public string Name { get; set; }
        public string Acronym { get; set; }
    }
}
