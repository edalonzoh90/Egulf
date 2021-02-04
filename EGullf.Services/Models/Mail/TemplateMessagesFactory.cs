using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EGullf.Services.Models.Configuration;
using EGullf.Services.Models.Mail;
using EGullf.Services.Services.Configuration;

namespace EGullf.Services.Models.Mail
{
    public class TemplateMessagesFactory : ITemplate
    {
        #region "Instances"
        EmailTemplateServices EmailTemplateServ = new EmailTemplateServices();
        #endregion

        public IMail GetTemplate(string email, string specificTemplate, Dictionary<string, string[]> parameters,string CustomSender = null)
        {
            //1. Obtener template base
            string emailMainTemplate = EmailTemplateServ.getMasterEmailTemplate("EmailCustomTemplate");
            return new MailEgulf(emailMainTemplate, email, specificTemplate, parameters,null,CustomSender);
            //return new WelcomeMail(emailMainTemplate, email, specificTemplate);
        }

     

    }
}
