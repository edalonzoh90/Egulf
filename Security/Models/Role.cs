using System.Collections.Generic;

namespace Security.Models
{
    public class Role
    {
        public Role()
        {
            Resources = new List<Resource>();
        }

        public string RoleName { get; set; }
        public int? RoleId { get; set; }
        public List<Resource> Resources { get; set; }
    }
}
