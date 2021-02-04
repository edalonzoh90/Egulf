using System.Collections.Generic;

namespace EGullf.Services.Models.Mail
{
    public interface ITemplate
    {
        IMail GetTemplate(string email, string specificTemplate, Dictionary<string, string[]> parms, string CustomSender = null);
    }
}
