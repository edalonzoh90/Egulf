using EGullf.Services.Models.Mail;
using EGullf.Services.Models.Management;
using EGullf.Services.Models.Utils;
using EGullf.Services.Services.Configuration;
using EGullf.Services.Services.Mail;
using Security.Sevices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGullf.Services.Services.Management
{
    public class SupportServices
    {


        public RequestResult<object> Help(string Email, string Message, string UserEmail = null)
        {
            try
            {          
                SystemVariableServices SV = new SystemVariableServices();
                var Key = Crypto.ToBase64(SV.GetSystemVariableValue("CryptoKey")).ToString().Substring(0, 16);
                var Iv = Crypto.ToBase64(SV.GetSystemVariableValue("CryptoIV")).ToString().Substring(0, 16);
                var _Email = (!string.IsNullOrEmpty(UserEmail)) ? UserEmail : Crypto.DecryptString(Email, Key, Iv);
                var _Message = Crypto.DecryptString(Message, Key, Iv);
                var _To = SV.GetSystemVariableValue("SupportEmailInbox");
                var _From = SV.GetSystemVariableValue("EgulfSupportEmail");

                MailServices MailServ = new MailServices();
                ITemplate factory = new TemplateMessagesFactory();

                Dictionary<string, string[]> param = new Dictionary<string, string[]>();
                param.Add("{Enfasis}", new string[] { _Message });
                param.Add("{Text}", new string[] { _Email });
                MailServ.SendMail(factory.GetTemplate(_To, "NewIncident", param, _From));

                return new RequestResult<object> { Status = Status.Success };
            }
            catch (Exception ex)
            {
                return new RequestResult<object> { Status = Status.Error };
            }
        }

        public RequestResult<object> HelpSession(int UserId, string Message)
        {
            PersonServices PersonServ = new PersonServices();
            string Email = PersonServ.getFirstUserPerson(new UserPersonModel() { UserId = UserId }).Email;
            return Help(null,Message,Email);
        }

          
     


    }
}
