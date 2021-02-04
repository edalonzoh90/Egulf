using EGULF.Helpers;
using EGullf.Services.Models.Utils;
using EGullf.Services.Services.Management;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static EGULF.Filters.SecurityFilterAction;

namespace EGULF.Controllers
{
    public class SupportController : Controller
    {


        public ActionResult Help()
        {
            return View();
        }


        [HttpPost]
        public JsonResult Help(string Email, string Message)
        {
            SupportServices SuppServ = new SupportServices();
            var resp = SuppServ.Help(Email, Message);
            if (resp.Status == Status.Success)
            {
                resp.Message = App_LocalResources.Support.HelpServiceConfirmed;
            }
            else
            {
                resp.Status = Status.Warning;
                resp.Message = App_LocalResources.Support.HelpServiceNotAvailable;
            }

            return Json(new
            {
                Data = resp
            });
        }


        [HttpPost]
        [Session]
        public JsonResult HelpSession(string Message)
        {
            SupportServices SuppServ = new SupportServices();
            var UserId = (int)SessionWeb.User.UserId;

            var resp = SuppServ.HelpSession(UserId, Message);
            if (resp.Status == Status.Success)
            {
                resp.Message = App_LocalResources.Support.HelpServiceConfirmed;
            }
            else
            {
                resp.Status = Status.Warning;
                resp.Message = App_LocalResources.Support.HelpServiceNotAvailable;
            }

            return Json(new
            {
                Data = resp
            });
        }




    }
}