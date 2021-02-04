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
    public class WelcomeMail : IMail
    {

        EmailTemplateServices EmailTemplateServ = new EmailTemplateServices();
        SystemVariableServices SystemVariableServ = new SystemVariableServices();
        string _template;
        string _email;
        string _subject;
        List<CustomTemplateSectionModel> _customTemplateSections = new List<CustomTemplateSectionModel>();

        public WelcomeMail(string template, string email, string specificTemplate)
        {
            _template = template;
            _email = email;
            //2. Obtener Custom Template
            var customTemplate = EmailTemplateServ.getCustomTemplate(null, specificTemplate);
            _subject = customTemplate.Subject;
            //3. Obtener Custom Template Sections
            _customTemplateSections = EmailTemplateServ.getCustomTemplateSections(customTemplate.CustomTemplateId);

        }

        public void Map()
        {
            foreach (var item in _customTemplateSections)
            {
                var sectionKey = item.SectionKey;
                var sectionValue = item.SectionValue;
                _template = _template.Replace(sectionKey, sectionValue);
            }
        }

        public void Fill()
        {
            _template = _template.Replace("#logo-url#", SystemVariableServ.GetSystemVariableValue("EgulfLogo"));
            removeTable();

            _template = _template.Replace("#btn-url#", "");
            _template = _template.Replace("#btn-start#", "");
            _template = _template.Replace("#btn-end#", "");
        }

        private void removeTable()
        {
            string start = "#table-start#";
            string end = "#table-end#";

            int sIndex = _template.IndexOf(start);
            int eIndex = _template.IndexOf(end);
            if (sIndex != -1 && eIndex != -1)
            {
                int tableCodeLength = eIndex - sIndex;
                string tableCode = _template.Substring(sIndex, tableCodeLength + end.Length);
                _template = _template.Replace(tableCode, "");
            }
        }

        public MailParameters GetMail()
        {

            return new MailParameters()
            {
                from = SystemVariableServ.GetSystemVariableValue("EmailApiEmail"),
                to = _email,
                subject = _subject,
                html = _template
            };
        }

        public List<MailAttachments> Attachment()
        {
            throw new NotImplementedException();
        }
    }
}
