using EGULF.Helpers;
using EGULF.Models;
using EGullf.Services.Models.AzureStorage;
using EGullf.Services.Models.Configuration;
using EGullf.Services.Models.Management;
using EGullf.Services.Models.Operation;
using EGullf.Services.Models.Utils;
using EGullf.Services.Services.AzureStorage;
using EGullf.Services.Services.Management;
using EGullf.Services.Services.Operation;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using static EGULF.Filters.SecurityFilterAction;
using globalResources = EGULF.App_LocalResources.Main;
using localResource = EGULF.App_LocalResources.Vessels;
namespace EGULF.Controllers
{
    public class VesselController : Controller
    {
        [Auth]
        public ActionResult Index()
        {
            return View();
        }

        [Auth]
        public ActionResult Create()
        {
            CountryServices countryServices = new CountryServices();
            ProjectTypeServices projectTypeServices = new ProjectTypeServices();
            ClasificationSocietyServices clasificationSocietyServices = new ClasificationSocietyServices();
            PortServices portServices = new PortServices();
            RegionServices regionServices = new RegionServices();
            VesselTypeServices vesselTypeServices = new VesselTypeServices();

            ViewBag.LstCountry = countryServices.GetSelect(globalResources.Select).Select(x => new SelectListItem { Value = x.Value, Text = x.Text });
            ViewBag.LstProjectType = projectTypeServices.GetSelect(null).Select(x => new SelectListItem { Value = x.Value, Text = x.Text });
            ViewBag.LstVesselType = vesselTypeServices.GetSelect(globalResources.Select).Select(x => new SelectListItem { Value = x.Value, Text = x.Text });
            ViewBag.LstClasificationSociety = clasificationSocietyServices.GetSelect(globalResources.Select).Select(x => new SelectListItem { Value = x.Value, Text = x.Text });
            ViewBag.LstPort = portServices.GetSelect(globalResources.Select).Select(x => new SelectListItem { Value = x.Value, Text = x.Text });
            ViewBag.LstRegion = regionServices.GetSelect(globalResources.Select).Select(x => new SelectListItem { Value = x.Value, Text = x.Text });

            return View(new VesselViewModel());
        }

        [Auth]
        public ActionResult Edit(int Id)
        {
            CountryServices countryServices = new CountryServices();
            ProjectTypeServices projectTypeServices = new ProjectTypeServices();
            ClasificationSocietyServices clasificationSocietyServices = new ClasificationSocietyServices();
            PortServices portServices = new PortServices();
            RegionServices regionServices = new RegionServices();
            VesselTypeServices vesselTypeServices = new VesselTypeServices();
            VesselServices vesselServices = new VesselServices();

            VesselViewModel model = new VesselViewModel();

            //Desencriptamos y validamos permisos y existencia
            VesselModel vessel = vesselServices.GetFirst(new VesselModel() { VesselId = Id });
            if (vessel == null || vessel.Company.CompanyId != SessionWeb.User.CompanyId)
                return RedirectToAction("Unauthorized", "Redirect");

            model.Vessel = vessel;
            model.VesselSpecificInfo = vesselServices.GetSpecificInfo(Id);
            model.SpecificInfo = vesselServices.GetSpecificInfoExtra(Id);
            model.VesselCost = vesselServices.GetCost(Id);

            ViewBag.LstCountry = countryServices.GetSelect(globalResources.Select).Select(x => new SelectListItem { Value = x.Value, Text = x.Text });
            ViewBag.LstProjectType = projectTypeServices.GetSelect(null).Select(x => new SelectListItem { Value = x.Value, Text = x.Text });
            ViewBag.LstVesselType = vesselTypeServices.GetSelect(globalResources.Select).Select(x => new SelectListItem { Value = x.Value, Text = x.Text });
            ViewBag.LstClasificationSociety = clasificationSocietyServices.GetSelect(globalResources.Select).Select(x => new SelectListItem { Value = x.Value, Text = x.Text });
            ViewBag.LstPort = portServices.GetSelect(globalResources.Select).Select(x => new SelectListItem { Value = x.Value, Text = x.Text });
            ViewBag.LstRegion = regionServices.GetSelect(globalResources.Select).Select(x => new SelectListItem { Value = x.Value, Text = x.Text });

            return View("Create", model);
        }

        [Auth]
        public ActionResult Manage(int Id)
        {
            VesselServices services = new VesselServices();
            ReasonAvailabilityServices reasonServices = new ReasonAvailabilityServices();
            ViewBag.LstReason = reasonServices.GetSelect(globalResources.Select).Select(x => new SelectListItem { Value = x.Value, Text = x.Text });
            ViewBag.LstEstatus = services.GetEstatus();
            VesselModel vessel = services.GetFirst(new VesselModel() { VesselId = Id });
            return View(new VesselAvailabilityViewModel () { VesselId = Id, VesselEstatusId = (int)vessel.Status });
        }

        [Session]
        [HttpGet]
        public JsonResult List(DatatableModel dt, VesselModel filters)
        {
            VesselServices services = new VesselServices();
            PagerModel pager = dt.ToPager();
            filters.Company.CompanyId = SessionWeb.User.CompanyId;
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

        [HttpGet]
        [Session]
        public ActionResult Valid(string id, VesselModel data)
        {
            RequestResult<string> result;
            VesselServices vesselServices = new VesselServices();
            result = vesselServices.Eval(data);

            if (result.Status == Status.Warning)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                Response.StatusDescription = result.Message;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Session]
        public ActionResult Transaction(string id, VesselViewModel dataVW)
        {
            VesselModel data = dataVW.Vessel;
            RequestResult<VesselModel> result = new RequestResult<VesselModel>() { Status = Status.Success, Data = data };
            VesselServices vesselServices = new VesselServices();

            try
            {
                if (!ModelState.IsValid)
                    result = new RequestResult<VesselModel>() { Status = Status.Error, Message = localResource.ErrorOnSave };
                else
                {
                    if (id == "add")
                    {
                        if (Request.Files.Count > 0)
                        {
                            var File = Request.Files[0];
                            data.Image.FileName = File.FileName;
                            data.Image.ContentType = File.ContentType;
                            data.Image.FileContent = File.InputStream;
                        }

                        data.UserModifiedId = SessionWeb.User.UserId;

                        result = vesselServices.InsUpdComplete(
                            dataVW.Vessel, dataVW.VesselSpecificInfo, dataVW.SpecificInfo, dataVW.VesselCost);
                        result.Data.Image.FileContent = null;
                        if (result.Status != Status.Success)
                            throw new Exception(string.Format("{0}: {1}", globalResources.SomethingWrong, result.Message));
                        return Json(result);
                    }
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                Response.StatusDescription = ex.Message;
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }

            return Json(result);
        }

        [HttpPost]
        [Session]
        public ActionResult AvailabilityTransaction(string id, VesselAvailabilityViewModel data)
        {
            RequestResult<VesselAvailabilityModel> result = new RequestResult<VesselAvailabilityModel>() { Status = Status.Success, Data = data };
            VesselAvailabilityServices availabilityServices = new VesselAvailabilityServices();
            VesselServices services = new VesselServices();

            try
            {
                if (id.ToLower() == "add")
                {
                    if (!ModelState.IsValid)
                        result = new RequestResult<VesselAvailabilityModel>() { Status = Status.Error, Message = localResource.ErrorOnSave };
                    else
                    {
                        result = availabilityServices.InsUpd(data);
                        if (result.Status != Status.Success)
                            throw new Exception(string.Format("{0}: {1}", globalResources.SomethingWrong, result.Message));

                        return Json(result);
                    }
                }
                else if(id.ToLower() == "del")
                {
                    result = availabilityServices.Del((int)data.AvailabilityVesselId);
                    if (result.Status != Status.Success)
                        throw new Exception(string.Format("{0}: {1}", globalResources.SomethingWrong, result.Message));

                    return Json(result);
                }
                else if (id.ToLower() == "status")
                {
                    RequestResult<VesselModel> resultV = new RequestResult<VesselModel>() { Status = Status.Success, Data = data.ToVesselModel() };
                    resultV = services.InsUpd(data.ToVesselModel());
                    if (result.Status != Status.Success)
                        throw new Exception(string.Format("{0}: {1}", globalResources.SomethingWrong, result.Message));

                    return Json(result);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                Response.StatusDescription = ex.Message;
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }

            return Json(result);
        }

        [Session]
        [HttpGet]
        public JsonResult AvailabilityList(VesselAvailabilityModel filters)
        {
            VesselAvailabilityServices services = new VesselAvailabilityServices();

            return Json(services.Get(filters), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public FileResult GetImage(int? VesselId)
        {
            try
            {
                if (VesselId != null)
                {
                    FileServices fileServ = new FileServices();
                    string reference = string.Format("vessels/{0}/images/vesselimage-{0}.jpg", VesselId.ToString());
                    FileModel fileData = fileServ.GetFile(reference);
                    if (fileData != null)
                    {
                        var img = (MemoryStream)fileData.FileContent;
                        return File(img.ToArray(), fileData.ContentType);
                    }
                }
                
                var UrlImage = Server.MapPath("~/Content/Images/Barco-md.png");
                FileStream file = new FileStream(UrlImage, FileMode.Open);
                return File(file, "image/png");
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return null;
            }

        }
    }
}