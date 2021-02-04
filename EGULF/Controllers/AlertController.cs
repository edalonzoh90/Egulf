using EGULF.Helpers;
using EGULF.Hubs;
using EGullf.Services.Models.Alert;
using EGullf.Services.Models.Configuration;
using EGullf.Services.Services.Alert;
using Microsoft.AspNet.SignalR;
using System.Collections.Generic;
using System.Web.Mvc;
using static EGULF.Filters.SecurityFilterAction;

namespace EGULF.Controllers
{
    public class AlertController : Controller
    {
        [Auth]
        public ActionResult Index()
        {
            return View();
        }

        [Session]
        [HttpGet]
        public JsonResult GetTable(DatatableModel dt, AlertModel filters)
        {
            filters.To = SessionWeb.User.PersonId;
            AlertServices services = new AlertServices();
            PagerModel pager = dt.ToPager();
            filters.Body = dt.sSearch;

            var collection = services.Get(pager, filters);

            return Json(new
            {
                status = UI.Status.Success,
                sEcho = dt.sEcho,
                iTotalRecords = pager.TotalRecords,
                iTotalDisplayRecords = pager.TotalRecords,
                aaData = collection
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult MarkAsReaded(AlertModel alert)
        {
            AlertServices alertServices = new AlertServices();
            alert.To = SessionWeb.User.PersonId; 
            AlertModel resp = alertServices.MarkAsReaded(alert);
            resp = resp ?? new AlertModel();
            
            return Json(resp);
        }

        [HttpPost]
        public JsonResult Send(int? PersonId)
        {
            PersonId = PersonId ?? SessionWeb.User.PersonId;
            AlertServices alertServices = new AlertServices();
            Dictionary<string, string> values = new Dictionary<string, string>();
            values.Add("Username", SessionWeb.User.UserName);

            AlertModel alert = alertServices.GetWithValues(1, values);
            alert.To = PersonId;
            alertServices.InsUpd(alert);

            var context = GlobalHost.ConnectionManager.GetHubContext<AlertHub>();
            context.Clients.Group(string.Format("P{0}", PersonId.ToString()))
                .newAlert(alert);

            return Json("{}");
        }

        [HttpGet]
        public JsonResult Get(PagerModel pager, AlertModel filters)
        {
            AlertServices service = new AlertServices();
            filters.To = SessionWeb.User.PersonId;
            var collection = service.Get(pager, filters);

            return Json(collection, JsonRequestBehavior.AllowGet);
        }

        [Auth]
        public ActionResult Total()
        {
            AlertServices alertServices = new AlertServices();
            PagerModel pager = new PagerModel(0,1,"","");
            AlertModel filter = new AlertModel();
            filter.Status = (int)AlertStatus.New;
            filter.To = SessionWeb.User.PersonId;
            alertServices.Get(pager, filter);
            string res = pager.TotalRecords == 0 ? string.Format("<span id='alertTotal' class='badge badge-pink noti-icon-badge'>{0}</span>", pager.TotalRecords)
                : string.Format("<span id='alertTotal' class='badge badge-pink noti-icon-badge'>{0}</span>", pager.TotalRecords);

            return Content(res);
        }

        [Auth]
        public ActionResult LastAlerts()
        {
            AlertServices alertServices = new AlertServices();
            PagerModel pager = new PagerModel(0, 8, "CreatedAt", "desc");
            AlertModel filter = new AlertModel();
            filter.Status = (int)AlertStatus.New;
            filter.To = SessionWeb.User.PersonId;

            List<AlertModel> lstAlert = alertServices.Get(pager, filter);
            string resp = "";
            foreach(AlertModel alert in lstAlert)
            {
                resp += string.Format("<a href='javascript:Main.MarkAsReaded({3});' id='alert_{3}' class='dropdown-item notify-item alert-item'>"
                + "<div class='notify-icon bg-info'><i class='mdi {1}'></i></div>"
                + "<p class='notify-details' style='white-space: normal'><small>{0}</small><small class='text-muted'>{2}</small></p>"
                + "</a>", alert.Body, alert.Icon, alert.TimeAgo, alert.AlertId);
            }
                    
            return Content(resp);
        }
    }
}