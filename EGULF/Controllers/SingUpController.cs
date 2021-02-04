using EGULF.Helpers;
using EGullf.Services.Models.Management;
using EGullf.Services.Models.Utils;
using EGullf.Services.Services.Configuration;
using EGullf.Services.Services.Management;
using Security.Models;
using System;
using System.Net;
using System.Reflection;
using System.Resources;
using System.Web.Mvc;

namespace EGULF.Controllers
{
    public class SingUpController : Controller
    {


        // GET: SingUp
        public ActionResult Index()
        {
            SystemVariableServices sv = new SystemVariableServices();
            SessionWeb.Key = Security.Sevices.Crypto.ToBase64(sv.GetSystemVariableValue("CryptoKey")).ToString().Substring(0, 16);
            SessionWeb.Iv = Security.Sevices.Crypto.ToBase64(sv.GetSystemVariableValue("CryptoIV")).ToString().Substring(0, 16);
            return View();
        }


        [HttpPost]
        public JsonResult SingUp(PersonModel personParameters, User userParameters)
        {
            try
            {
                RequestResult<object> response = new RequestResult<object>();

                PersonServices PersonServ = new PersonServices();
                var result = PersonServ.SingUp(personParameters, userParameters);

                if (result.Status == Status.Success)
                {
                    response.Status = result.Status;
                    response.Message = EGULF.App_LocalResources.MessageUI.MsgSuccess;
                }
                else if (result.Status == Status.Warning)
                {
                    response.Status = result.Status;
                    if (result.Message == "MsgUserExisting")
                        response.Message = EGULF.App_LocalResources.SingUp.MsgUserExisting;
                    else if (result.Message == "MsgEmailExisting")
                        response.Message = EGULF.App_LocalResources.SingUp.MsgEmailExisting;
                }
                else
                {
                    throw new Exception(result.Message);
                }   
 
                return Json(response);
            }
            catch (Exception ex)
            {
                return Json(new RequestResult<object>() { Status = Status.Error, Message = ex.Message });
            }
        }


        public ActionResult ValidateEmail(string email)
        {
            PersonServices PersonServ = new PersonServices();
            bool ValidateEmail = true;
            int? UserId = (SessionWeb.User != null) ? SessionWeb.User.UserId : null;
            if (UserId != null && UserId > 0)
            {
                UserPersonModel UserCurrentData = PersonServ.getFirstUserPerson(new UserPersonModel() { UserId = UserId });
                if (email.ToUpper().Trim() == UserCurrentData.Email.ToUpper().Trim())
                    ValidateEmail = false;
            }

            if (ValidateEmail)
            {
                var result = PersonServ.ValidateExistingEmail(email);
                if (result.Status == Status.Warning)
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    Response.StatusDescription = EGULF.App_LocalResources.SingUp.MsgEmailExisting;
                }
            }      
            return Json(new RequestResult<object>() { Status = Status.Success }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult ValidateUsername(string username)
        {
            PersonServices PersonServ = new PersonServices();
            var result = PersonServ.ValidateExistingUsername(username);
            if (result.Status == Status.Warning)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                Response.StatusDescription = EGULF.App_LocalResources.SingUp.MsgUserExisting;
            }
            return Json(new RequestResult<object>() { Status = Status.Success }, JsonRequestBehavior.AllowGet);
        }



    }
}