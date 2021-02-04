using System.Web.Mvc;

namespace EGULF.Controllers
{
    public class RedirectController : Controller
    {
        public ActionResult Error()
        {
            Response.StatusCode = 405;
            ViewBag.StatusCode = 500;
            ViewBag.StatusName = "Internal Server Error";
            ViewBag.Title = "Error";

            return View("Redirect");
        }

        public ViewResult NotFound()
        {
            Response.StatusCode = 404;
            ViewBag.StatusCode = 404;
            ViewBag.StatusName = "Not Found";
            ViewBag.Title = "Not Found";

            return View("Redirect");
        }

        public ViewResult Unauthorized()
        {
            Response.StatusCode = 403;
            ViewBag.StatusCode = 403;
            ViewBag.StatusName = "Forbidden";
            ViewBag.Title = "Forbidden";

            return View("Redirect");
        }

        public JsonResult SessionTimeout()
        {
            Response.StatusCode = 440;
            return Json(new { StatusCode = 440 }, JsonRequestBehavior.AllowGet);
        }
    }
}