using System.Collections.Generic;

namespace Security.Models
{
    public class Resource
    {
        public int? ParentResourceId { get; set; }
        public int? ResourceId { get; set; }
        public string DisplayName { get; set; }
        public string Icon { get; set; }
        public string Url { get; set; }
        public int? Position { get; set; }
        public List<Resource> Children { get; set; }
        public bool? AddMenu { get; set; }
    }
}
