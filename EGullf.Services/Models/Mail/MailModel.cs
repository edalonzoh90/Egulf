using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGullf.Services.Models.Mail
{
    public class MailModel
    {

    }



    public class MailGunResponse
    {
        public string id { get; set; }

        public string message { get; set; }
    }

    public class MailParameters
    {
        public string from { get; set; }

        public string to { get; set; }

        public string cc { get; set; }

        public string bcc { get; set; }

        public string subject { get; set; }

        public string text { get; set; }

        public string html { get; set; }

        public string attachment { get; set; }
    }


    public class MailAttachments
    {
        public byte[] file { get; set; }

        public string fileName { get; set; }

        public string contentType { get; set; }
    }
   


}
