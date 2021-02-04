using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EGULF.Helpers;
using EGullf.Services.Models.Management;
using EGullf.Services.Models.Utils;
using EGullf.Services.Services.Management;
using Security.Sevices;
using Security.Models;
using static EGULF.Filters.SecurityFilterAction;
using System.Net;
using localResource = EGULF.App_LocalResources.Company;
using EGullf.Services.Services.Configuration;
using EGullf.Services.Models.Configuration;
using EGullf.Services.Models.Mail;
using EGullf.Services.Services.Mail;
using EGullf.Services.Models.Alert;
using EGullf.Services.Services.Alert;
using Microsoft.AspNet.SignalR;
using EGULF.Hubs;

namespace EGULF.Controllers
{
    public class CompanyController : Controller
    {

        #region Attributes
        public int? UserCompanyId { get { return SessionWeb.User.CompanyId; } set { SessionWeb.User.CompanyId = value; } }
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
        public bool HasRejectedInvitation { get; set; }
        #endregion

        #region CompanyViews
        // GET: Company
        [Auth]
        [Session]
        public ActionResult Index()
        {
            SystemVariableServices SystemVariableServ = new SystemVariableServices();

            UserServices userServ = new UserServices();
            var role = userServ.GetRole(new User { UserId = (int)SessionWeb.User.UserId });
            CompanyServices CompServ = new CompanyServices();
            CompanyModel company = new CompanyModel();
            ViewData["isOwner"] = role != null ? role.RoleId.ToString().Equals(SystemVariableServ.GetSystemVariableValue("RolAdminCompany")) : false;
            ViewData["invitations"] = UserPerson.InvitationsCompanies;
            if (HasCompany())
                company = CompServ.Get(new CompanyModel() { CompanyId = UserCompanyId }).FirstOrDefault();

            return View(company);
        }

        // GET: Campany/Create
        [Auth]
        [Session]
        public ActionResult Create()
        {
            if (HasCompany())
                return RedirectToAction("Index");
            return View();
        }

        // POST: Campany/Create
        [HttpPost]
        [Session]
        public ActionResult Create(CompanyModel data)
        {
            try
            {
                if (HasCompany())
                    return RedirectToAction("Index");

                if (ModelState.IsValid)
                {

                    PersonServices PersonServ = new PersonServices();
                    var userId = (int)SessionWeb.User.UserId;
                    var person = PersonServ.getFirstUserPerson(new UserPersonModel() { UserId = userId }).ToPerson();
                    CompanyServices cs = new CompanyServices();
                    CompanyModel cm = new CompanyModel()
                    {
                        CompanyName = data.CompanyName,
                        RFC = data.RFC,
                        Address = data.Address,
                        PhoneNumber = data.PhoneNumber,
                        Email = data.Email
                    };
                    var result = cs.AddCompany(cm, person);
                    if (result.Status != Status.Success)
                        throw new Exception(string.Format("{0}: {1}", localResource.txt_company_error_saved, result.Message));
                    SessionWeb.User.CompanyId = ((CompanyModel)result.Data).CompanyId;
                    InvitationCompanyServices invServ = new InvitationCompanyServices();
                    foreach (var i in UserPerson.InvitationsCompanies)
                    {
                        i.Status = 2;
                        invServ.InsUpd(i);
                    }
                    return Json(result);
                }
                return Json(new RequestResult<object>() { Status = Status.Error, Message = localResource.txt_company_error_saved });

            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                Response.StatusDescription = ex.Message;

                return Json(ex.Message, JsonRequestBehavior.AllowGet);
                //return Json(new RequestResult<object>() { Status = Status.Error, Message = ex.Message });

            }
        }

        // GET: Campany/Edit/5
        [Session]
        public ActionResult Edit(int id)
        {
            if (!HasCompany())
                return RedirectToAction("Index");

            CompanyServices csrv = new CompanyServices();
            var company = csrv.Get(new CompanyModel() { CompanyId = id }).FirstOrDefault();
            if (company == null)
                return HttpNotFound();
            return View(company);
        }

        // POST: Campany/Edit/5
        [HttpPost]
        public ActionResult Edit(CompanyModel data)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    CompanyServices cs = new CompanyServices();
                    CompanyModel cm = new CompanyModel()
                    {
                        CompanyId = data.CompanyId,
                        CompanyName = data.CompanyName,
                        RFC = data.RFC,
                        Address = data.Address,
                        PhoneNumber = data.PhoneNumber,
                        Email = data.Email
                    };
                    var result = cs.InsUpd(cm);
                    if (result.Status != Status.Success)
                        throw new Exception(string.Format("{0}: {1}", localResource.txt_company_error_updated, result.Message));

                    return Json(result);
                }
                return Json(new RequestResult<object>() { Status = Status.Error, Message = localResource.txt_company_error_updated });
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                Response.StatusDescription = ex.Message;

                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        // POST: Campany/Delete/5
        [HttpPost]
        [Session]
        public ActionResult Delete(int id)
        {
            try
            {
                InvitationCompanyModel filters = new InvitationCompanyModel() { CompanyId = UserCompanyId};
                DatatableModel dt = new DatatableModel()
                {
                    iColumns = 5,
                    iDisplayLength = 10,
                    iDisplayStart = 0,
                    iSortCol_0 = 3,
                    iSortingCols = 1,
                    iTotalRecords = 0,
                    sSearch = null
                };
                var invitationsC = GetInvitationsCompany(dt, filters);
                if(invitationsC != null && invitationsC.Count() > 0)
                    return Json(new RequestResult<object>() { Status = Status.Warning, Message = localResource.txt_company_has_invitation});
                
                CompanyServices cs = new CompanyServices();
                CompanyModel cm = new CompanyModel()
                {
                    CompanyId = id,
                };
                if (cs.HasProjectsVesselsCompany(cm))
                    return Json(new RequestResult<object>() { Status = Status.Warning, Message = localResource.txt_company_has_vp });
               
                var result = cs.RemoveCompany(cm, UserPerson);
                UserCompanyId = null;
                if (result.Status != Status.Success)
                    throw new Exception(string.Format("{0}: {1}", localResource.txt_company_error_delete, result.Message));
                return Json(result);

            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                Response.StatusDescription = ex.Message;

                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: Campany/Valid
        [Auth]
        [Session]
        public ActionResult Valid(string companyName)
        {
            CompanyServices cs = new CompanyServices();
            var c = cs.Get(new CompanyModel() { CompanyName = companyName }).FirstOrDefault();
            if (c != null && c.CompanyId != UserCompanyId)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                Response.StatusDescription = "the company is not unique";
            }
            return Json(new RequestResult<object>() { Status = Status.Error, Message = "the company is not unique" }, JsonRequestBehavior.AllowGet);
        }

        [Auth]
        public ActionResult Valid_RFC(string rfc)
        {
            CompanyServices cs = new CompanyServices();
            var c = cs.Get(new CompanyModel() { RFC = rfc }).FirstOrDefault();
            if (c != null && c.CompanyId != UserCompanyId)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                Response.StatusDescription = "the rfc is not unique";
            }
            return Json(new RequestResult<object>() { Status = Status.Error, Message = "the company is not unique" }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region InvitationCompanyViews
        [Auth]
        public ActionResult Invitate() {
            SystemVariableServices SystemVariableServ = new SystemVariableServices();
            UserServices userServ = new UserServices();
            var role = userServ.GetRole(new User { UserId = (int)SessionWeb.User.UserId });

            if (role != null ? !role.RoleId.ToString().Equals(SystemVariableServ.GetSystemVariableValue("RolAdminCompany")) : true)
                return RedirectToAction("Index");
            return View();
        }
        
        [HttpGet]
        public ActionResult GetInvitation(DatatableModel dt, InvitationCompanyModel filters)
        {
            try
            {
                
                PagerModel pager = dt.ToPager();
                var collection = GetInvitationsCompany(dt, filters);

                return Json(new
                {
                    status = UI.Status.Success,
                    sEcho = dt.sEcho,
                    iTotalRecords = pager.TotalRecords,
                    iTotalDisplayRecords = pager.TotalRecords,
                    aaData = collection
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                Response.StatusDescription = ex.Message;

                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }
        
        [Session]
        [HttpPost]
        public ActionResult Invitate(string mails)
        {
            try
            {
                if(string.IsNullOrEmpty(mails.Trim()))
                    return Json(new RequestResult<object>() { Status = Status.Warning, Message = localResource.txt_invitation_empty });

                InvitationCompanyServices invServ = new InvitationCompanyServices();
                PersonServices persnServ = new PersonServices();
                string[] mailsS = mails.Split(new Char[] { ',' });
                RequestResult<object> result = new RequestResult<object>() { Status = Status.Success };
                string mailsNoSend = string.Empty;

                foreach (var mail in mailsS)
                {
                    var mailT = mail.Trim();
                    //Enviar invitación
                    if (!HasInvitation(mailT, UserCompanyId))
                    {
                        //si tiene CUENTA
                        UserPersonModel p = null;
                        if ((p = persnServ.getFirstUserPerson(new UserPersonModel() { Email = mailT })) != null)
                        {
                            if(p.CompanyId != null)
                            {
                                mailsNoSend += "," + mail;
                                continue;
                            }

                            SendNotificationsInvite(p.PersonId,2);
                        }
                        SendMailInvitation(mailT);
                        result = invServ.InsUpd(new InvitationCompanyModel()
                        {
                            CompanyId = UserCompanyId,
                            From = UserPerson.PersonId,
                            To = p==null ? null : p.PersonId,
                            CreatedAt = DateTime.Now,
                            Status = 0,
                            Email = mailT
                        });
                    }
                    if (HasRejectedInvitation)
                        mailsNoSend += "," + mail;
                }
                if (result == null || result.Status != Status.Success)
                    throw new Exception(string.Format("{0}: {1}", localResource.txt_error_invite, result.Message));
                if(!string.IsNullOrEmpty(mailsNoSend))
                    return Json(new RequestResult<object>() { Status = Status.Warning, Message = string.Format(localResource.txt_invitation_has_company,mailsNoSend.Substring(1)) });

                return Json(result);

            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                Response.StatusDescription = ex.Message;

                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult Confirm(int? companyId)
        {
            try
            {
                InvitationCompanyServices invServ = new InvitationCompanyServices();
                InvitationCompanyModel inv = UserPerson.InvitationsCompanies.Find(x => x.CompanyId == companyId);
                inv.Status = 1;
                UserPerson.CompanyId = inv.CompanyId;
                var result = invServ.ConfirmInvitation(inv, UserPerson);
                if (result == null || result.Status != Status.Success)
                    throw new Exception(string.Format("{0}: {1}", localResource.txt_company_error_delete, result.Message));
                SessionWeb.User.CompanyId = inv.CompanyId;
                foreach(var i in UserPerson.InvitationsCompanies)
                {
                    if (i.CompanyInvitationId == inv.CompanyInvitationId)
                        continue;
                    i.Status = 2;
                    invServ.InsUpd(i);
                }
                return Json(result);
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                Response.StatusDescription = ex.Message;

                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        public ActionResult Decline(int? companyId)
        {
            try
            {
                InvitationCompanyServices invServ = new InvitationCompanyServices();
                InvitationCompanyModel inv = UserPerson.InvitationsCompanies.Find(x => x.CompanyId == companyId);
                inv.Status = 2;
                UserPerson.CompanyId = inv.CompanyId;
                var result = invServ.InsUpd(inv);
                if (result == null || result.Status != Status.Success)
                    throw new Exception(string.Format("{0}: {1}", localResource.txt_company_error_delete, result.Message));
                return Json(result);
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                Response.StatusDescription = ex.Message;

                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        public ActionResult DeleteInvivtation(int id,string email)
        {
            try
            {
                PersonServices persnServ = new PersonServices();
                var p = persnServ.getFirstUserPerson(new UserPersonModel() { Email = email });
                InvitationCompanyServices cs = new InvitationCompanyServices();
                InvitationCompanyModel cm = new InvitationCompanyModel()
                {
                    CompanyInvitationId = id,
                    From = p.PersonId
                };
                PagerModel pager = new PagerModel(0, 100, "", "");
                var invitations = cs.Get(pager, cm);
                cm = invitations.Find(x => x.To == p.PersonId);
                var result = (p != null && p.CompanyId == UserCompanyId) ? cs.RemoveCompanyInvitation(cm, p.ToPerson()) : cs.Del(cm);
                if(p != null && cm.Status != 2)
                    SendNotificationsInvite(p.PersonId, 3);
                if (result.Status != Status.Success)
                    throw new Exception(string.Format("{0}: {1}", localResource.txt_error_remove_invitation, result.Message));
                return Json(result);

            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                Response.StatusDescription = ex.Message;

                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Send(string email) {
            try
            {
                SendMailInvitation(email);
                return Json(new RequestResult<object>() { Status = Status.Success, Message = "SendOk" });

            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                Response.StatusDescription = ex.Message;

                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region PrivateProperties
        private List<InvitationCompanyModel> GetInvitationsCompany(string email)
        {
            InvitationCompanyModel filters = new InvitationCompanyModel();
           
            InvitationCompanyServices service = new InvitationCompanyServices();
            
            var collection = service.Get(new PagerModel(0, 10, "", "", email), filters);
            return collection;
        }

        private bool HasCompany()
        {
            return UserCompanyId != null;
        }

        private bool HasInvitation(string email, int? companyId)
        {
            InvitationCompanyModel filters = new InvitationCompanyModel() { Email = email, CompanyId = companyId };
            DatatableModel dt = new DatatableModel()
            {
                iColumns = 5,
                iDisplayLength = 10,
                iDisplayStart = 0,
                iSortCol_0 = 3,
                iSortingCols = 1,
                iTotalRecords = 0,
                sSearch = email
            };
            InvitationCompanyServices service = new InvitationCompanyServices();
            PagerModel pager = dt.ToPager();
            var collection = service.Get(pager, filters).FirstOrDefault();
            HasRejectedInvitation = (collection != null && collection.Status == 2);
            return collection != null;
        }

        private List<InvitationCompanyModel> GetInvitationsCompany(DatatableModel dt, InvitationCompanyModel filters) {
            filters.CompanyId = UserCompanyId;
            InvitationCompanyServices service = new InvitationCompanyServices();
            PagerModel pager = dt.ToPager();
            return service.Get(pager, filters);
        }

        private void SendMailInvitation(string email) {
            MailServices MailServ = new MailServices();
            ITemplate factory = new TemplateMessagesFactory();
            CompanyServices csrv = new CompanyServices();
            var company = csrv.Get(new CompanyModel() { CompanyId = UserCompanyId }).FirstOrDefault();
            Dictionary<string, string[]> parsm = new Dictionary<string, string[]>();
            parsm.Add("{Enfasis}", new string[] { company.CompanyName });
           MailServ.SendMail(factory.GetTemplate(email, "InvitationCompany",parsm));
        }

        private void SendNotificationsInvite(int? personId, int templateId) {
            AlertServices alertServices = new AlertServices();
            CompanyServices csrv = new CompanyServices();
            var company = csrv.Get(new CompanyModel() { CompanyId = UserCompanyId }).FirstOrDefault();
            AlertModel alert = alertServices.GetWithValues(templateId, null);
            alert.To = personId;
            alert.Body = string.Format(alert.Body, company.CompanyName);
            alertServices.InsUpd(alert);
            var context = GlobalHost.ConnectionManager.GetHubContext<AlertHub>();
            context.Clients.Group(string.Format("P{0}", personId.ToString()))
                .newAlert(alert);
        }

        #endregion
    }
}
