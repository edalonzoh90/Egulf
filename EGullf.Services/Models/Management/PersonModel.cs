using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGullf.Services.Models.Management
{
    public class PersonModel
    {
        public int? PersonId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; } 

        public int? UserId { get; set; }

        public string PhoneNumber { get; set; }   

        public string Email { get; set; }

        public string Skype { get; set; }

        public int? FileReferenceId { get; set; }

        public int? CompanyId { get; set; }

        public List<InvitationCompanyModel> InvitationsCompanies { get; set; }
    }


    public class UserPersonModel
    {
        public int? PersonId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int? UserId { get; set; }

        public string Username { get; set; }

        public string PhoneNumber { get; set; }

        public string Email { get; set; }

        public string Skype { get; set; }

        public int? CompanyId { get; set; }

        public string CompanyName { get; set; }

        public int? FileReferenceId { get; set; }

        public string FileName { get; set; }

        public string Path { get; set; }

        public string ContentType { get; set; }

        public Stream File { get; set; }

        public PersonModel ToPerson() {
            return new PersonModel()
            {
                PersonId = this.PersonId,
                FirstName = this.FirstName,
                LastName = this.LastName,
                UserId = this.UserId,
                PhoneNumber = this.PhoneNumber,
                Email = this.Email,
                Skype = this.Skype,
                FileReferenceId = this.FileReferenceId,
                CompanyId = this.CompanyId
            };
        }
    }


}
