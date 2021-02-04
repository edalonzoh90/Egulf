using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace EGullf.Services.Models.Management
{
    public class CompanyModel
    {
        public int? CompanyId { get; set; }
        [Required]
        public string CompanyName { get; set; }
        [Required]
        public string RFC { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string Email { get; set; }

        public List<InvitationCompanyModel> Invitations { get; set; }
    }
}
