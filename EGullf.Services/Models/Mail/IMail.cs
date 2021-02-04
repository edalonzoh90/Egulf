using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGullf.Services.Models.Mail
{
    public interface IMail
    {
        void Map();
        void Fill();
        MailParameters GetMail();
        List<MailAttachments> Attachment();

    }
}
