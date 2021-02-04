using EGULF.Helpers;
using EGullf.Services.Models.AzureStorage;
using EGullf.Services.Models.Management;
using EGullf.Services.Models.Utils;
using EGullf.Services.Services.Configuration;
using EGullf.Services.Services.Management;
using Security.Models;
using Security.Sevices;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static EGULF.Filters.SecurityFilterAction;

namespace EGULF.Controllers
{
    public class ProfileController : Controller
    {

        [Auth]
        public ActionResult Index(int? userId = 0)
        {
            if (userId == 0)
                userId = SessionWeb.User.UserId;

            ViewBag.UserId = userId;
            ViewBag.SessionUserId = SessionWeb.User.UserId;
            return View();
        }


        [HttpGet]
        [Session]
        public JsonResult GetUserProfile(int userId)
        {
            try
            {
                RequestResult<UserPersonModel> response = new RequestResult<UserPersonModel>();
                PersonServices PersonServ = new PersonServices();
                response.Data = PersonServ.getFirstUserPerson(new UserPersonModel() { UserId = userId });

                return Json(response, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new RequestResult<object>() { Status = Status.Error, Message = ex.Message });
            }
        }


        [HttpPost]
        [Session]
        public JsonResult SaveProfile(UserPersonModel parameters)
        {
            try
            {
                parameters.UserId = SessionWeb.User.UserId;
                if (Request.Files.Count > 0) {
                    var File = Request.Files[0];
                    parameters.FileName = File.FileName;
                    parameters.ContentType = File.ContentType;
                    parameters.File = File.InputStream;
                }

                PersonServices PersonServ = new PersonServices();
                var result = PersonServ.SaveProfile(parameters);
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new RequestResult<object>() { Status = Status.Error, Message = ex.Message });
            }
        }

        [HttpPost]
        [Session]
        public JsonResult ChangePassword(User parameters) {
            try
            {
                RequestResult<object> response = new RequestResult<object>();
                UserServices UserServ = new UserServices();

                parameters.UserId = SessionWeb.User.UserId;
                var result = UserServ.ChangePassword(parameters);
                if (result == null)
                {
                    response.Status = Status.Warning;
                    response.Message = EGULF.App_LocalResources.Profile.MsgNoValidPassword;
                }
                else
                {
                    response.Status = Status.Success;
                }

                return Json(response);
            }
            catch (Exception ex)
            {
                return Json(new RequestResult<object>() { Status = Status.Error, Message = ex.Message });
            }
        }


        [HttpGet]
        [Session]
        public FileResult GetUserImage(int? UserId)
        {
            try
            {
                UserId = UserId ?? SessionWeb.User.UserId;

                PersonServices PersonServ = new PersonServices();
                FileModel fileData = PersonServ.GetUserImage((int)UserId);
                if (fileData != null)
                {
                    var img = (MemoryStream)fileData.FileContent;
                    return File(img.ToArray(), fileData.ContentType);
                }
                else
                {
                    var UrlImage = Server.MapPath("~/Content/Images/avatar.png");
                    FileStream file = new FileStream(UrlImage, FileMode.Open);
                    return File(file, "image/png");
                }
            }
            catch (Exception ex)
            {
                return null;
            }

        }


        [HttpGet]
        [Session]
        public JsonResult DeleteUserImage()
        {
            try
            {
                RequestResult<object> response = new RequestResult<object>();
                int UserId = SessionWeb.User.UserId ?? 0;

                PersonServices PersonServ = new PersonServices();
                response = PersonServ.DeleteUserImage(UserId);

                return Json(response, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new RequestResult<object>() { Status = Status.Error, Message = ex.Message });
            }          
        }



    }
}