using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EGullf.Services.Services.Configuration;
using EGullf.Services.Models.Configuration;

namespace EGullf.Services.Models.Mail
{
    public class MailEgulf : IMail
    {
        EmailTemplateServices EmailTemplateServ = new EmailTemplateServices();
        SystemVariableServices SystemVariableServ = new SystemVariableServices();
        string _template;
        string _email;
        string _from;
        string _subject;
        Dictionary<string,string[]> _parms;
        Dictionary<string,byte[]> _attch;
        List<CustomTemplateSectionModel> _customTemplateSections = new List<CustomTemplateSectionModel>();

        public MailEgulf(string template, string email, string spesificTemplate, Dictionary<string, string[]> parsm, Dictionary<string,byte[]> attachament = null, string CustomSender = null)
        {
            _from = (!string.IsNullOrEmpty(CustomSender)) ? CustomSender : SystemVariableServ.GetSystemVariableValue("EmailApiEmail");
            _template = template;
            _email = email;
            //2. Obtener Custom Template
            var customTemplate = EmailTemplateServ.getCustomTemplate(null, spesificTemplate);
            _subject = customTemplate.Subject;
            //3. Obtener Custom Template Sections
            _customTemplateSections = EmailTemplateServ.getCustomTemplateSections(customTemplate.CustomTemplateId);
            _parms = parsm;

        }

        public List<MailAttachments> Attachment()
        {
            List<MailAttachments> attachs = new List<MailAttachments>();
            foreach (KeyValuePair<string,byte[]> item in _attch)
            {
                var mailAtch = new MailAttachments() { fileName = item.Key, file = item.Value, contentType="por defiinir"};
                attachs.Add(mailAtch);
            }
            return attachs;
        }

        public void Fill()
        {
            _template = _template.Replace("{logo_url}", SystemVariableServ.GetSystemVariableValue("EgulfLogo"));
        }

        public MailParameters GetMail()
        {
            return new MailParameters()
            {
                from = _from,
                to = _email,
                subject = _subject,
                html = _template
            };
        }

        public void Map()
        {
            foreach (var item in _customTemplateSections)
            {
                var sectionKey = item.SectionKey;
                var sectionValue = item.SectionValue;
                if (_parms != null && _parms.ContainsKey(sectionKey))
                    sectionValue = string.Format(sectionValue, _parms[sectionKey]);
                _template = _template.Replace(sectionKey, sectionValue);
            }
        }
    }
}
