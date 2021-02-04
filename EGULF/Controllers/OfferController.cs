using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using EGullf.Services.Services.Reports;
using EGullf.Services.Models.Operation;
using EGULF.Helpers;
using static EGULF.Filters.SecurityFilterAction;
using EGullf.Services.Services.Operation;

namespace EGULF.Controllers
{
    public class OfferController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }


        [Session]
        public FileResult AgreementReport(OfferModel parameters)
        {
            ReportServices ReportServ = new ReportServices();
            byte[] fileBytes = ReportServ.GetAgreementReport((int)parameters.OfferId, SessionWeb.User.UserId ?? 0);

            string fileName = "AgreementReport.pdf";
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Pdf, fileName);
        }
    }
}