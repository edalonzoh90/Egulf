using EGULF.Helpers;
using EGullf.Services.Models.Configuration;
using EGullf.Services.Models.Management;
using EGullf.Services.Models.Operation;
using EGullf.Services.Models.Utils;
using EGullf.Services.Services.Configuration;
using EGullf.Services.Services.Management;
using EGullf.Services.Services.Operation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using static EGULF.Filters.SecurityFilterAction;
using globalResources = EGULF.App_LocalResources.Main;

namespace EGULF.Controllers
{
    public class ProjectController : Controller
    {

        [Auth]
        public ActionResult Index()
        {
            ViewBag.CompanyId = SessionWeb.User.CompanyId ?? -1;
            return View();
        }

        [Auth]
        public ActionResult Create(int? id = 0)
        {
            ProjectTypeServices ProjectTypeServ = new ProjectTypeServices();
            RegionServices RegionServ = new RegionServices();
            SystemVariableServices SystemVarServ = new SystemVariableServices();

            ViewBag.ProjectId = id;
            ViewBag.LstProjectType = ProjectTypeServ.GetSelect(null).Select(x => new SelectListItem() { Value = x.Value, Text = x.Text });
            ViewBag.LstRegion = RegionServ.GetSelect(null).Select(x => new SelectListItem() { Value = x.Value, Text = x.Text });
            //ViewBag.ProjectCategoryType = JsonConvert.SerializeObject(SystemVarServ.GetSystemVariableValue("ProjectCategoryClasification"));
            ViewBag.CabinSpecificationType = JsonConvert.SerializeObject(SystemVarServ.GetSystemVariableValue("CabinSpecificationTypes"));
            ViewBag.LstProjectCategoryType =  JsonConvert.SerializeObject(ProjectTypeServ.Get(new ProjectTypeModel()).ToList());
            return View();
        }

        [Auth]
        public ActionResult Manage(int? id = 0)
        {
            return View();
        }

        [Auth]
        public ActionResult Delete(int? id = 0)
        {
            ViewBag.ProjectId = id;
            return View();
        }

        [HttpGet]
        [Session]
        public JsonResult Get(DatatableModel dt, ProjectModel parameters)
        {
            try
            {
                SystemVariableServices SystemVariableServ = new SystemVariableServices();
                var ActiveProject = Convert.ToInt32(SystemVariableServ.GetSystemVariableValue("EstatusActiveProject"));

                ProjectServices ProjectServ = new ProjectServices();
                PagerModel pager = dt.ToPager();

                parameters.CompanyId = SessionWeb.User.CompanyId ?? -1;
                var collection = ProjectServ.GetProjectCollection(parameters,pager);

                return Json(new
                {
                    status = UI.Status.Success,
                    sEcho = dt.sEcho,
                    iTotalRecords = pager.TotalRecords,
                    iTotalDisplayRecords = pager.TotalRecords,
                    aaData = collection.Data
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return new JsonResult
                {
                    Data = new { status = UI.Status.Error,
                                data = ex.Message },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
        }


        [HttpPost]
        [Session]
        public ActionResult SaveProjectInfo(ProjectModel parameters)
        {
            try
            {
                SystemVariableServices SystemVariableServ = new SystemVariableServices();
                var ActiveProject = Convert.ToInt32(SystemVariableServ.GetSystemVariableValue("EstatusActiveProject"));

                ProjectServices ProjectServ = new ProjectServices();
                parameters.CompanyId = (int)SessionWeb.User.CompanyId;
                parameters.Status = ActiveProject;

                RequestResult<object> result = ProjectServ.SaveProject(parameters); //InsUpdProjectInfo(parameters);
                if (result.Status != Status.Success)
                    throw new Exception(result.Message);
                else
                    return Json(result);
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                Response.StatusDescription = ex.Message;
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpGet]
        [Session]
        public ActionResult GetProjectInfo(ProjectModel parameters)
        {
            try {
                parameters.CompanyId = SessionWeb.User.CompanyId ?? -1;
                ProjectServices ProjectServ = new ProjectServices();
                RequestResult<ProjectModel> result = ProjectServ.GetProject(parameters);
                if (result.Status != Status.Success)
                    throw new Exception(result.Message);
                else
                    return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex) {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                Response.StatusDescription = ex.Message;
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult ValidateStartDate(int? ProjectId, DateTime StartDate)
        {
            ProjectServices ProjectServ = new ProjectServices();
            var result = ProjectServ.ValidateStartDate(ProjectId, StartDate, SessionWeb.User.CompanyId ?? -1);
            if (result.Status == Status.Warning)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                Response.StatusDescription = EGULF.App_LocalResources.Project.MsgWrongStartDate;
            }
            return Json(new RequestResult<object>() { Status = Status.Success }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        [Session]
        public ActionResult DeleteProject(ProjectModel parameters)
        {
            try
            {
                ProjectServices ProjectServ = new ProjectServices();
                var result = ProjectServ.DelProject(parameters);
                if (result.Status != Status.Success)
                    throw new Exception(result.Message);
                else
                    return Json(result);
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                Response.StatusDescription = ex.Message;
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }



    }
}