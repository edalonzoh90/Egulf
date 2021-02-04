using EGullf.Services.Models.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGullf.Services.DA.Configuration
{
    public class EmailTemplateDA
    {


        public string getMasterEmailTemplate(string TemplateName)
        {
            using (var db = new EGULFEntities())
            {
                EmailTemplateModel TemplateData = new EmailTemplateModel();

                TemplateData = (from t in db.EmailTemplate.ToList()
                                where t.Name == TemplateName
                                select new EmailTemplateModel
                                {
                                    EmailTemplateId = t.EmailTemplateId,
                                    Name = t.Name,
                                    Html = t.Html,
                                    Module = t.Module,
                                    Description = t.Description
                                }).FirstOrDefault();

                if (TemplateData.Name != null)
                    return TemplateData.Html;
                else
                    return null;
            }
        }

        public CustomTemplateModel getCustomTemplate(int? CustomTemplateId, string TemplateName)
        {
            CustomTemplateModel Data = new CustomTemplateModel();
            using (var db = new EGULFEntities())
            {
                Data = (from ct in db.CustomTemplate.ToList()
                        where ct.CustomTemplateId == CustomTemplateId
                        || ct.TemplateName == TemplateName
                        select new CustomTemplateModel
                        {
                            CustomTemplateId = ct.CustomTemplateId,
                            TemplateName = ct.TemplateName,
                            Subject = ct.Subject
                        }).FirstOrDefault();
            }

            return Data;
        }

        public List<CustomTemplateSectionModel> getCustomTemplateSections(int CustomTemplateId)
        {
            List<CustomTemplateSectionModel> Collection = new List<CustomTemplateSectionModel>();

            using (var db = new EGULFEntities())
            {
                Collection = db.sp_getCustomTemplateSections(CustomTemplateId,
                                                              null
                                                              ).Select(x => new CustomTemplateSectionModel()
                                                                {
                                                                  CustomTemplateSectionId = x.CustomTemplateSectionId,
                                                                  CustomTemplateId = (int)x.CustomTemplateId,
                                                                  SectionKey = x.SectionKey,
                                                                  SectionValue = x.SectionValue
                                                                }).ToList();
            }

            return Collection;
        }



    }
}
