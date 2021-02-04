using EGullf.Services.DA;
using EGullf.Services.DA.Configuration;
using EGullf.Services.Models.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGullf.Services.Services.Configuration
{
    public class EmailTemplateServices
    {


        public string getMasterEmailTemplate(string TemplateName)
        {
            EmailTemplateDA EmailTemplateDA = new EmailTemplateDA();
            string resp = EmailTemplateDA.getMasterEmailTemplate(TemplateName);
            return resp;
        }

        public CustomTemplateModel getCustomTemplate(int? CustomTemplateId, string TemplateName)
        {
            EmailTemplateDA EmailTemplateDA = new EmailTemplateDA();
            CustomTemplateModel resultData = EmailTemplateDA.getCustomTemplate(CustomTemplateId,TemplateName);
            return resultData;
        }

        public List<CustomTemplateSectionModel> getCustomTemplateSections(int CustomTemplateId)
        {
            EmailTemplateDA EmailTemplateDA = new EmailTemplateDA();
            List<CustomTemplateSectionModel> resultCollection = EmailTemplateDA.getCustomTemplateSections(CustomTemplateId);
            return resultCollection;
        }


    }
}
