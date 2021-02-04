using EGullf.Services.Models.Configuration;
using EGullf.Services.Models.Mail;
using EGullf.Services.Services.Configuration;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Web;
using System.Web.Script.Serialization;

namespace EGullf.Services.Services.Mail
{
    public class MailServices
    {
        #region "Instances"
        SystemVariableServices SystemVariableServ = new SystemVariableServices();
        JavaScriptSerializer Deserializer = new JavaScriptSerializer();
        EmailTemplateServices EmailTemplateServ = new EmailTemplateServices();
        #endregion



        public bool wellcomeEmail(string Email)
        {
            //variables
            string HtmlBody = string.Empty;
            string Subject = string.Empty;
            string From = SystemVariableServ.GetSystemVariableValue("EmailApiEmail");
            string EmailMainTemplate = EmailTemplateServ.getMasterEmailTemplate("EmailEgulfBasicTemplate");

            //instances 
            CustomTemplateModel CustomTemplate = new CustomTemplateModel();
            List<CustomTemplateSectionModel> CustomTemplateSections = new List<CustomTemplateSectionModel>();

            //we get email templates
            CustomTemplate = EmailTemplateServ.getCustomTemplate(null, "WellcomeTemplate");
            CustomTemplateSections = EmailTemplateServ.getCustomTemplateSections(CustomTemplate.CustomTemplateId);          

            //we load basic template things and remove innecesary stuffs
            EmailMainTemplate = getReadyMainTemplate(EmailMainTemplate);
            EmailMainTemplate = removeTable(EmailMainTemplate);

            //we get template custom sections
            EmailMainTemplate = buildTemplateSections(EmailMainTemplate,CustomTemplateSections);
            EmailMainTemplate = EmailMainTemplate.Replace("#btn-url#","");
            EmailMainTemplate = EmailMainTemplate.Replace("#btn-start#", "");
            EmailMainTemplate = EmailMainTemplate.Replace("#btn-end#", "");

            HtmlBody = EmailMainTemplate;
            Subject = CustomTemplate.Subject;

            MailParameters parameters = new MailParameters();
            parameters.from = From;
            parameters.to = Email;
            parameters.subject = Subject;
            parameters.html = HtmlBody;

            MailGunResponse result = sendEmail(parameters,null);
            if (!string.IsNullOrEmpty(result.id))
                return true;
            else
                return false;
        }

     

        private MailGunResponse sendEmail(MailParameters parameters, List<MailAttachments> attachments)
        {
            //parameters for the api request
            var APIMainUri = SystemVariableServ.GetSystemVariableValue("EmailApiUrl");
            var MailgunDomain = SystemVariableServ.GetSystemVariableValue("EmailApiDomain");
            var APIMessageEndpoint = SystemVariableServ.GetSystemVariableValue("EmailApiMessageEndpoint");
            var APIUser = SystemVariableServ.GetSystemVariableValue("EmailApiUsername");
            var APIKey = SystemVariableServ.GetSystemVariableValue("EmailApiKey");

            RestClient client = new RestClient();
            client.BaseUrl = new Uri(APIMainUri);
            client.Authenticator = new HttpBasicAuthenticator(APIUser, APIKey);

            RestRequest request = new RestRequest();
            request.Method = Method.POST;
            request.AddParameter("domain", MailgunDomain, ParameterType.UrlSegment);
            request.Resource = "{domain}" + APIMessageEndpoint;

            if (parameters != null)
            {
                foreach (PropertyInfo cProperty in parameters.GetType().GetProperties())
                {
                    var cPropertyName = cProperty.Name;
                    var cPropertyValue = cProperty.GetValue(parameters);
                    if (cPropertyValue != null)
                    {
                        request.AddParameter(cPropertyName, cPropertyValue);
                    };
                }
            }

            if (attachments != null && attachments.Count > 0)
            {
                foreach (var cAttachment in attachments.ToList())
                {
                    request.AddFileBytes("attachment", cAttachment.file, cAttachment.fileName, (cAttachment.contentType));
                }
            }

            //IRestResponse response = client.Execute(request);
            //MailGunResponse result = Deserializer.Deserialize<MailGunResponse>(response.Content);

            MailGunResponse result = new MailGunResponse();
            result.id = "set by async funtionality";
            var Asyncresult = client.ExecuteAsync<MailGunResponse>(request, response => { });
          
            return result;  
        }


        private string buildTemplateSections(string EmailMainTemplate, List<CustomTemplateSectionModel> Sections)
        {
            foreach (var item in Sections.ToList())
            {
                var sectionKey = item.SectionKey;
                var sectionValue = item.SectionValue;

                EmailMainTemplate = EmailMainTemplate.Replace(sectionKey, sectionValue); 
            }

            return EmailMainTemplate;
        }

    
        private string removeTable(string EmailMainTemplate)
        {
            string start = "#table-start#";
            string end = "#table-end#";

            int sIndex = EmailMainTemplate.IndexOf(start);
            int eIndex = EmailMainTemplate.IndexOf (end);
            if (sIndex != -1 && eIndex != -1)
            {
                int tableCodeLength = eIndex - sIndex;
                string tableCode = EmailMainTemplate.Substring(sIndex, tableCodeLength + end.Length);
                EmailMainTemplate = EmailMainTemplate.Replace(tableCode, "");
            }
            return EmailMainTemplate;
        }

        private string removeButton(string EmailMainTemplate)
        {
            string start = "#btn-start#";
            string end = "#btn-end#";

            int sIndex = EmailMainTemplate.IndexOf(start);
            int eIndex = EmailMainTemplate.IndexOf(end);
            if (sIndex != -1 && eIndex != -1)
            {
                int btnCodeLength = eIndex - sIndex;
                string btnCode = EmailMainTemplate.Substring(sIndex, btnCodeLength + end.Length);
                EmailMainTemplate = EmailMainTemplate.Replace(btnCode, "");
            }
            return EmailMainTemplate;
        }

        private string getReadyMainTemplate(string EmailMainTemplate)
        {
            string LogoUrl = SystemVariableServ.GetSystemVariableValue("EgulfLogo");
            //string EgulfPhone = SystemVariableServ.GetSystemVariableValue("EgulfPhone");
            //string EgulfEmail = SystemVariableServ.GetSystemVariableValue("EgulfEmail");

            EmailMainTemplate = EmailMainTemplate.Replace("#logo-url#", LogoUrl);
            //EmailMainTemplate = EmailMainTemplate.Replace("#contact-phone#", EgulfPhone);
            //EmailMainTemplate = EmailMainTemplate.Replace("#contact-email#", EgulfEmail);

            return EmailMainTemplate;
        }

        public bool SendMail(IMail mail)
        {
            mail.Map();
            mail.Fill();
            MailGunResponse result = sendEmail(mail.GetMail(), null);
            if (!string.IsNullOrEmpty(result.id))
                return true;
            return false;
        }

        public bool SendMailWithAttachment(IMail mail, List<MailAttachments> attachments)
        {
            mail.Map();
            mail.Fill();
            MailGunResponse result = sendEmail(mail.GetMail(), attachments);
            if (!string.IsNullOrEmpty(result.id))
                return true;
            return false;
        }

        public bool SendMail(IMail mail, List<MailAttachments> lst)
        {
            mail.Map();
            mail.Fill();
            MailGunResponse result = sendEmail(mail.GetMail(), lst);
            if (!string.IsNullOrEmpty(result.id))
                return true;
            return false;
        }

    }
}
