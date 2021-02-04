using EGULF.Helpers;
using EGullf.Services.Services.Configuration;
using EGullf.Services.Services.Management;
using EGullf.Services.Models.Configuration;
using EGullf.Services.Models.Management;
using Security.Models;
using Security.Sevices;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using static EGULF.Filters.SecurityFilterAction;
using EGullf.Services.Services.Alert;
using EGullf.Services.Models.Alert;
using Microsoft.AspNet.SignalR;
using EGULF.Hubs;
using EGullf.Services.Models.Utils;

namespace EGULF.Controllers
{
    public class LoginController : Controller
    {
        #region "Instances"
        UserServices UserServ = new UserServices();
        InvitationCompanyServices serviceIn = new InvitationCompanyServices();
        PersonServices personServ = new PersonServices();
        #endregion
        private PersonModel _userPerson;
        public PersonModel UserPerson
        {
            get
            {
                if (this._userPerson == null)
                {
                    var userId = (int)SessionWeb.User.UserId;
                    PersonServices PersonServ = new PersonServices();
                    _userPerson = PersonServ.getFirstUserPerson(new UserPersonModel() { UserId = userId }).ToPerson();
                    _userPerson.InvitationsCompanies = GetInvitationsCompany(_userPerson.Email).Where(x => x.Status == 0).ToList();
                    return _userPerson;
                }
                else
                {
                    return _userPerson;
                }

            }
            set
            {
                _userPerson = value;
            }
        }
        private List<InvitationCompanyModel> GetInvitationsCompany(string email)
        {
            InvitationCompanyModel filters = new InvitationCompanyModel();
            var collection = serviceIn.Get(new PagerModel(0,10,"","",email), filters);
            return collection;
        }
        [AllowAnonymous]
        public ActionResult Index()
        {
            if (SessionWeb.User != null)
            {
                Server.ClearError();
                Response.Clear();
                Response.Redirect("/match/index");
            }

            SystemVariableServices sv = new SystemVariableServices();
            SessionWeb.Key = Security.Sevices.Crypto.ToBase64(sv.GetSystemVariableValue("CryptoKey")).ToString().Substring(0,16);
            SessionWeb.Iv = Security.Sevices.Crypto.ToBase64(sv.GetSystemVariableValue("CryptoIV")).ToString().Substring(0, 16);

            return View();
        }

        [HttpPost]
        public JsonResult Login(User user)
        {
            try
            {
                //Idioma
                string lang = "es";
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(lang);
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(lang);
                HttpCookie cookie = new HttpCookie("Language");
                cookie.Value = lang;
                Response.Cookies.Add(cookie);
                //End Idioma      

                Validate validate = UserServ.ValidateLogin(user);
                if (validate.Data != null)
                {
                    SessionWeb.User = user;
                    SessionWeb.Resources = (List<Resource>)validate.Data;
                    FormsAuthentication.SetAuthCookie(SessionWeb.User.UserName, false);
                }
                foreach (var i in UserPerson.InvitationsCompanies)
                {
                    if (i.To != null)
                        continue;
                    i.To = UserPerson.PersonId;
                    serviceIn.InsUpd(i);
                    SendNotificationsInvite(UserPerson.PersonId, i.CompanyId);
                }   

                return new JsonResult
                {
                    Data = new { status = UI.Status.Success, data = validate.ErrorCode },
                };
            }
            catch (Exception ex)
            {
                return new JsonResult
                {
                    Data = new { status = UI.Status.Error, data = new {
                        subject = App_LocalResources.Login.IncorrectDataSubject,
                        message = App_LocalResources.Login.IncorrectDataMessage }
                    },
                };
            }
        }


        [HttpPost]
        public JsonResult Recover(string Email)
        {
            var resp = personServ.RecoverAccount(Email);
            if (resp.Status == Status.Warning)
                resp.Message = App_LocalResources.Login.AccountNoExist;
            else if (resp.Status == Status.Success)
                resp.Message = App_LocalResources.Login.ResetInstructions;

            return Json(new
            {
                Data = resp
            });
        }


        [HttpGet]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();

            Session.Clear();
            Session.Abandon();

            return RedirectToAction("/");
        }

        [Auth]
        public ActionResult Menu()
        {
            int UserId = (int)SessionWeb.User.UserId;
            string Menu = string.Empty;
            List<Resource> DataMenu = UserServ.GetUserMenu(UserId);
            string UrlBase = Request.Url.GetLeftPart(UriPartial.Authority);

            Menu += "";
            foreach (var item in DataMenu)
            {
                var hasChildren = false;
                if (item.Children != null && item.Children.Count() > 0) hasChildren = true;
                string itemMenu = hasChildren ? "<li class='has_sub'>" : "<li>";
                itemMenu += "<a href='" + UrlBase + item.Url + "' class='waves-effect waves-primary'><i class='" + item.Icon + "'></i>";
                itemMenu += "<span class='hide-menu'> " + item.DisplayName;

                if (hasChildren)
                {
                    itemMenu += "<span class='menu-arrow'></span></span></a>";
                    itemMenu += "<ul class='list-unstyled'>";
                    itemMenu += GetMenuChildren(item.Children);
                    itemMenu += "</ul>";
                }
                else
                {
                    itemMenu += "</span></a>";
                }
                itemMenu += "</li>";
                Menu += itemMenu;
            }
            Menu += "<li><a href='/login/logout' class='waves-effect waves-primary'><i class='ti-power-off'></i> <span class='hide-menu'>" + App_LocalResources.UserSession.TextLogOut + "</span></a></li>";

            return Content(Menu);
        }

        public string GetMenuChildren(List<Resource> MenuChild)
        {
            string MenuChildren = string.Empty;
            string UrlBase = Request.Url.GetLeftPart(UriPartial.Authority) + "/";

            foreach (var Child in MenuChild)
            {
                if (Child.Children != null && Child.Children.Count > 0)
                {
                    var hasChildren = false;
                    if (Child.Children != null && Child.Children.Count() > 0) hasChildren = true;
                    MenuChildren += hasChildren ? "<li class='has_sub'>" : "<li>";
                    MenuChildren += "<a href='" + UrlBase + Child.Url + "' class='waves-effect subdrop'>" + Child.DisplayName + "<span class='menu-arrow'></span></a><ul>";
                    MenuChildren += GetMenuChildren(Child.Children);
                    MenuChildren += "</ul></li>";
                }
                else
                {
                    MenuChildren += "<li><a href=" + UrlBase + Child.Url + ">" + Child.DisplayName + "</a></li>";
                }
            }

            return MenuChildren;
        }


        [HttpPost]
        public JsonResult SessionTimeout()
        {
            bool Expired = (SessionWeb.User == null);
            return Json(Expired);
        }

        private void SendNotificationsInvite(int? personId, int? companyId)
        {
            AlertServices alertServices = new AlertServices();
            CompanyServices csrv = new CompanyServices();
            var company = csrv.Get(new CompanyModel() { CompanyId = companyId }).FirstOrDefault();
            AlertModel alert = alertServices.GetWithValues(2, null);
            alert.To = personId;
            alert.Body = string.Format(alert.Body, company.CompanyName);
            alertServices.InsUpd(alert);
            var context = GlobalHost.ConnectionManager.GetHubContext<AlertHub>();
            context.Clients.Group(string.Format("P{0}", personId.ToString()))
                .newAlert(alert);
        }

    }
}