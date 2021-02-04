using EGULF.Helpers;
using EGullf.Services.Services.Configuration;
using System.Web.Mvc;
using static EGULF.Filters.SecurityFilterAction;

namespace EGULF.Controllers
{
    public class HomeController : Controller
    {
        [Auth]
        public ActionResult Index()
        {
            SystemVariableServices sv = new SystemVariableServices();
            SessionWeb.Key = Security.Sevices.Crypto.ToBase64(sv.GetSystemVariableValue("CryptoKey")).ToString().Substring(0, 16);
            SessionWeb.Iv = Security.Sevices.Crypto.ToBase64(sv.GetSystemVariableValue("CryptoIV")).ToString().Substring(0, 16);

            return View();
        }
    }
}