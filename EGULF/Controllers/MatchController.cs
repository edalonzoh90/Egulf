using EGullf.Services.Services.Management;
using EGullf.Services.Services.Operation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EGULF.Helpers;
using EGullf.Services.Models.Utils;
using EGullf.Services.Models.Operation;
using System.Net;
using static EGULF.Filters.SecurityFilterAction;
using EGullf.Services.Models.Alert;
using Microsoft.AspNet.SignalR;
using EGULF.Hubs;
using EGullf.Services.Models.Configuration;
using EGullf.Services.Services.Alert;

namespace EGULF.Controllers
{
    public class MatchController : Controller
    {
        public int? UserCompanyId { get { return SessionWeb.User.CompanyId; } set { SessionWeb.User.CompanyId = value; } }
        public int? UserSessionId { get { return SessionWeb.User.UserId; } set { SessionWeb.User.UserId = value; } }
        public int? PersonSessionId { get { return SessionWeb.User.PersonId; } set { SessionWeb.User.PersonId = value; } }

        [Auth]
        public ActionResult Index()
        {
            ProjectServices ProjectServ = new ProjectServices();
            ProjectTypeServices projectTypeServices = new ProjectTypeServices();
            RegionServices regionServices = new RegionServices();
            

            ViewBag.LstProjects = ProjectServ
                .GetSelect(null, new EGullf.Services.Models.Operation.ProjectModel() { CompanyId = UserCompanyId ?? -1, Status = ProjectModel.STATUS_NEW })
                .Select(x => new SelectListItem { Value = x.Value, Text = x.Text });
            ViewBag.LstSuitability = projectTypeServices.GetSelect(null).Select(x => new SelectListItem { Value = x.Value, Text = x.Text });
            ViewBag.LstRegion = regionServices.GetSelect(null).Select(x => new SelectListItem { Value = x.Value, Text = x.Text });
            ViewBag.currentPersonId = SessionWeb.User.PersonId;
            return View();
        }

        public ActionResult Select(string id)
        {
            ProjectServices ProjectServ = new ProjectServices();
            Object result = null;
            switch (id.ToUpper())
            {
                case "PROJECT": {
                        result = ProjectServ.GetSelect(null,
                            new EGullf.Services.Models.Operation.ProjectModel()
                            {
                                CompanyId = UserCompanyId ?? -1,
                                Status = ProjectModel.STATUS_NEW
                            });
                }break;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Project(int id)
        {
            try
            {

                ProjectServices ProjectServ = new ProjectServices();
                RequestResult<ProjectModel> result = ProjectServ.GetProject(new ProjectModel() { CompanyId = (int)UserCompanyId, ProjectId = id });
                if (result.Status != Status.Success)
                    throw new Exception(result.Message);
                else
                    return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                Response.StatusDescription = ex.Message;
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Find(MatchModel parms)
        {
            MatchServices m = new MatchServices();

            parms.CompanyId = SessionWeb.User.CompanyId;

            var items = m.GetMatchProject(parms);

            return Json(items, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Offers(int id)
        {
            OfferServices serv = new OfferServices();
            PagerModel pagerParameters = new PagerModel();
            
            var offertF = new OfferModel()
            {
                UserId = PersonSessionId,
            };

            if (id != 0 && id != 1)
                offertF.OfferReceived = id == 2;
            var results = serv.GetAll(pagerParameters, offertF);

            return Json(results, JsonRequestBehavior.AllowGet);
        }

        public ActionResult VesselsToOffer(int company, int project, int offer)
        {
            VesselServices serv = new VesselServices();
            var vessels = serv.VesselAvailableProject(company, project, offer);

            return Json(vessels, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateVesselCost(OfferCostModel cost)
        {
            OfferServices ser = new OfferServices();
            try
            {
                var res = ser.UpdCost(cost);
                if (res.Status != Status.Success)
                    throw new Exception(string.Format("{0}: {1}", "Error al actualizar costos", res.Message));
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                Response.StatusDescription = ex.Message;

                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult OfferTransaction(string id, OfferModel offer)
        {
            RequestResult<List<AlertModel>> result = new RequestResult<List<AlertModel>>() { Status = Status.Success };
            List<AlertModel> lstAlertToProjectCompany = new List<AlertModel>();
            List<AlertModel> lstAlertToVesselCompany = new List<AlertModel>();
            OfferServices services = new OfferServices();
            AlertTemplateServices templateServices = new AlertTemplateServices();
            AlertServices alertServices = new AlertServices();

            try
            {
                OfferModel val = new OfferModel();

                if (offer.OfferId != null)
                {
                    val = services.GetById((int)offer.OfferId);
                    if (val == null)
                        throw new Exception("NOT_FOUND");
                }

                if (id.ToLower() == "offer")
                {
                    offer.ProjectAdmin.PersonId = SessionWeb.User.PersonId;
                    offer.UserModifiedId = SessionWeb.User.PersonId;
                    result = services.InsComplete(offer);

                    Dictionary<string, string> values = new Dictionary<string, string>();
                    values.Add("ID", offer.Vessel.VesselId.ToString());
                    AlertTemplateModel template = templateServices.GetById(8);
                    AlertModel alert = alertServices.GetWithValues(template, values);
                    lstAlertToProjectCompany.Add(alert);

                    //Trick the form how i get the offer
                    val = services.GetById((int)offer.OfferId); 
                }
                else if (id.ToLower() == "accept")
                {
                    offer.VesselAdmin.PersonId = SessionWeb.User.PersonId;
                    offer.UserModifiedId = SessionWeb.User.PersonId;
                    result = services.Accept(offer, (int)SessionWeb.User.PersonId);

                    Dictionary<string, string> values = new Dictionary<string, string>();
                    values.Add("ID", ""+val.Project.ProjectId);
                    AlertTemplateModel template = templateServices.GetById(10);
                    AlertModel alert = alertServices.GetWithValues(template, values);
                    lstAlertToProjectCompany.Add(alert);

                    Dictionary<string, string> values2 = new Dictionary<string, string>();
                    values2.Add("ID", PersonSessionId.ToString());
                    AlertTemplateModel template2 = templateServices.GetById(11);
                    AlertModel alert2 = alertServices.GetWithValues(template2, values2);
                    lstAlertToVesselCompany.Add(alert2);
                }
                else if (id.ToLower() == "reject")
                {
                    offer.VesselAdmin.PersonId = SessionWeb.User.PersonId;
                    offer.UserModifiedId = SessionWeb.User.PersonId;
                    result = services.Reject(offer);

                    Dictionary<string, string> values2 = new Dictionary<string, string>();
                    values2.Add("ID", PersonSessionId.ToString());
                    AlertTemplateModel template2 = templateServices.GetById(11);
                    AlertModel alert2 = alertServices.GetWithValues(template2, values2);
                    lstAlertToVesselCompany.Add(alert2);
                }
                else if (id.ToLower() == "fix")
                {
                    MessageServices chatServices = new MessageServices();
                    offer.VesselAdmin.PersonId = SessionWeb.User.PersonId;
                    offer.UserModifiedId = SessionWeb.User.PersonId;

                    if (val.Status == OfferModel.FIX && val.VesselAdmin.PersonId == SessionWeb.User.PersonId)
                    {
                        chatServices.MarkAsReaded(
                            new MessageModel() { ReferenceId = offer.OfferId, From = SessionWeb.User.PersonId });
                        return Json(result);
                    }

                    if (val.Status == OfferModel.NEW)
                    {
                        result = services.Fix(offer);

                        Dictionary<string, string> values = new Dictionary<string, string>();
                        values.Add("ID", offer.OfferId.ToString());
                        AlertTemplateModel template = templateServices.GetById(9);
                        AlertModel alert = alertServices.GetWithValues(template, values);
                        alert.To = val.ProjectAdmin.PersonId;
                        result.Data.Add(alert);

                        Dictionary<string, string> values2 = new Dictionary<string, string>();
                        values2.Add("ID", PersonSessionId.ToString());
                        AlertTemplateModel template2 = templateServices.GetById(11);
                        AlertModel alert2 = alertServices.GetWithValues(template2, values2);
                        lstAlertToVesselCompany.Add(alert2);
                    }
                }

                if (result.Status == Status.Success)
                {
                    var context = GlobalHost.ConnectionManager.GetHubContext<AlertHub>();

                    foreach (AlertModel alert in result.Data)
                    {
                        context.Clients.Group(string.Format("P{0}", alert.To))
                            .newAlert(alert);
                    }
                    foreach (AlertModel alert in lstAlertToProjectCompany)
                    {
                        context.Clients.Group(string.Format("C{0}", val.Project.CompanyId))
                        .newAlert(alert);
                    }
                    foreach (AlertModel alert in lstAlertToVesselCompany)
                    {
                        context.Clients.Group(string.Format("C{0}", val.Vessel.Company.CompanyId.ToString()))
                        .newAlert(alert);
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.Message == "STATUS_NOT_VALID")
                {
                    result.Message = "La oferta ya no se encuentra disponible.";
                    result.Status = Status.Error;
                }
                else if (ex.Message == "NOT_AVAILABILITY")
                {
                    result.Message = "El barco seleccionado, no está disponible en las fechas seleccionadas.";
                    result.Status = Status.Warning;
                }
                else
                {
                    throw new Exception(result.Message);
                }
            }

            result.Data = null;

            return Json(result);
        }
    }
}
