using EGULF.Helpers;
using EGULF.Hubs;
using EGullf.Services.Models.Alert;
using EGullf.Services.Models.AzureStorage;
using EGullf.Services.Models.Configuration;
using EGullf.Services.Models.Example;
using EGullf.Services.Models.Operation;
using EGullf.Services.Models.Utils;
using EGullf.Services.Services.AzureStorage;
using EGullf.Services.Services.Example;
using EGullf.Services.Services.Operation;
using Microsoft.AspNet.SignalR;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using static EGULF.Filters.SecurityFilterAction;

namespace EGULF.Controllers
{
    public class ExampleController : Controller
    {
        // GET: Example
        public ActionResult Index()
        {
            return View();
        }

        [Session]
        [HttpPost]
        public JsonResult Transaction(string id, NotificationModel model)
        {
            RequestResult<object> resp = new RequestResult<object>();
            NotificationServices service = new NotificationServices();
            FileServices fileServices = new FileServices();

            try
            {
                //if (Request.Files.Count > 0)
                //{
                //    var file = Request.Files[0];
                //    if (file != null && file.ContentLength > 0)
                //    {
                //        var fileName = Path.GetFileName(file.FileName);
                //        fileServices.UploadFromStream("users2/"+fileName, file.InputStream);
                //    }
                //}
                if (Request.Files.Count > 0)
                {
                    FileModel file = new FileModel();
                    var req = Request.Files[0];

                    file.FileName = Path.GetFileName(req.FileName);
                    file.Path = "test/";
                    file.ContentType = req.ContentType;
                    file.FileContent = req.InputStream;
                    fileServices.SaveFile(file);
                }


            }
            catch (Exception ex)
            {
                //Log =/ 
            }

            return Json(resp);
        }

        [HttpGet]
        public JsonResult Get(DatatableModel dt, NotificationModel filters)
        {
            try
            {
                NotificationServices service = new NotificationServices();
                PagerModel pager = dt.ToPager();
                var collection = service.Get(pager, filters);

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
                return new JsonResult
                {
                    Data = new { status = UI.Status.Error, data = ex.Message },
                };
            }
        }

        [Session]
        [HttpPost]
        public ActionResult TransactionOffers(string id)
        {
            RequestResult<List<AlertModel>> result = new RequestResult<List<AlertModel>>() { Status = Status.Success };
            OfferServices services = new OfferServices();
            OfferModel model = new OfferModel();

            if (id == "offer")
            {
                model.Project.ProjectId = 13;
                model.Vessel.VesselId = 1;
                model.ProjectAdmin.PersonId = SessionWeb.User.PersonId;
                model.UserModifiedId = SessionWeb.User.PersonId;
                result = services.InsComplete(model);
            }
            else if (id == "accept")
            {
                model.OfferId = 2;
                result = services.Accept(model, (int)SessionWeb.User.PersonId);
               
            }

            if (result.Status == Status.Success)
            {
                foreach (AlertModel alert in result.Data)
                {
                    var context = GlobalHost.ConnectionManager.GetHubContext<AlertHub>();
                    context.Clients.Group(string.Format("P{0}", alert.To))
                        .newAlert(alert);
                }
            }

            result.Data = null;
            return Json(result);
        }

        [HttpGet]
        public ActionResult GetFile()
        {
            FileServices fileServices = new FileServices();

            FileModel file = fileServices.GetFile("Users/2/ProfileImage/13975325_1169888423072337_7289890490028059404_o.jpg");

                byte[] filedata = ((MemoryStream)file.FileContent).ToArray();

                return File(filedata, "text/plain");

            //var cd = new System.Net.Mime.ContentDisposition
            //{
            //    FileName = "helloworld.txt",
            //    Inline = true,
            //};

            //Response.AppendHeader("Content-Disposition", cd.ToString());
            //string contentType = MimeMapping.GetMimeMapping(filepath);

            //return File(filedata, "text/plain");
        }
    }
}