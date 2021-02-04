using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGullf.Services.Models.Configuration
{
    public class EmailTemplateModel
    {
        public int EmailTemplateId { get; set; }

        public string Name { get; set; }

        public string Html { get; set; }

        public string Module { get; set; }

        public string Description { get; set; }
        
    }

    public class CustomTemplateModel
    {
        public int CustomTemplateId { get; set; }

        public string TemplateName { get; set; }

        public string Subject { get; set; }
    }

    public class CustomTemplateSectionModel
    {
        public int CustomTemplateSectionId { get; set; }

        public int CustomTemplateId { get; set; }

        public string SectionKey { get; set; }

        public string SectionValue { get; set; }
    }



}
