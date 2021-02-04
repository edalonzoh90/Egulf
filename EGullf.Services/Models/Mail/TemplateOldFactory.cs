using EGullf.Services.Services.Configuration;
using System.Collections.Generic;

namespace EGullf.Services.Models.Mail
{
    public class TemplateOldFactory : ITemplate
    {
        #region "Instances"
        EmailTemplateServices EmailTemplateServ = new EmailTemplateServices();
        #endregion

        public IMail GetTemplate(string email, string specificTemplate, Dictionary<string, string[]> parm, string CustomSender = null)
        {
            //1. Obtener template base
            string emailMainTemplate = EmailTemplateServ.getMasterEmailTemplate("EmailEgulfBasicTemplate");
            return new WelcomeMail(emailMainTemplate, email, specificTemplate);
        }
    }
}
