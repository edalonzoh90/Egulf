using System.Collections.Generic;

namespace Security.Models
{
    public class User
    {
        public User()
        {
            Roles = new List<Role>();
        }

        public List<Role> Roles { get; set; }
        public int? UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string NewPassword { get; set; }
        public int? CompanyId { get; set; }
        public int? PersonId { get; set; }

    }
}
