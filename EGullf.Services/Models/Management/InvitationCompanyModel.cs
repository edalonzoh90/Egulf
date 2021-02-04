using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGullf.Services.Models.Management
{
    public class InvitationCompanyModel
    {
        public int? CompanyInvitationId { get; set; }
        public int? CompanyId { get; set; }
        public int? From { get; set; }
        public int? To { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? Status { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CompanyName { get; set; }
        public string FullName
        {
            get
            {
                return string.Format("{0} {1}", this.FirstName, this.LastName).ToUpper();
            }
        }
    }
}
