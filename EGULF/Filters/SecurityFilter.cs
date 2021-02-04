using EGULF.Helpers;
using System.Web;
using System.Web.Mvc;

namespace EGULF.Filters
{
    public class SecurityFilterAction : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (SessionWeb.User != null && filterContext.ActionDescriptor.ActionName != "sessiontimeout")
            {
                System.Web.Security.FormsAuthentication.SetAuthCookie(SessionWeb.User.UserName, false);
                var sessionTimeout = HttpContext.Current.Session.Timeout * 60 * 1000;
                filterContext.Controller.ControllerContext.HttpContext.Response.AppendHeader("SessionTimeout", sessionTimeout.ToString());
            }
            else
                base.OnActionExecuting(filterContext);
        }

        public class AuthAttribute : AuthorizeAttribute
        {
            protected override bool AuthorizeCore(HttpContextBase httpContext)
            {
                if (SessionWeb.User == null)
                {

                    HttpContext.Current.Response.Redirect("/");
                    return true;
                }
                
                return SessionWeb.Resources.Exists(r => r.Url.ToLower() == GetUrlContext(httpContext).ToLower());
            }

            public string GetUrlContext(HttpContextBase httpContext)
            {
                var rd = httpContext.Request.RequestContext.RouteData;
                string currentAction = rd.GetRequiredString("action");
                string currentController = rd.GetRequiredString("controller");
                string currentArea = rd.Values["area"] as string;
                string url = (currentArea == null ? "" : "/" + currentArea) + "/" + currentController + "/" + currentAction;
                return url;
            }
        }

        public class SessionAttribute : AuthorizeAttribute
        {
            protected override bool AuthorizeCore(HttpContextBase httpContext)
            {
                if (SessionWeb.User == null)
                {
                    HttpContext.Current.Response.StatusCode = 440;
                    HttpContext.Current.Response.Redirect("/Redirect/SessionTimeout");

                    return true;
                }

                return true;
            }
        }
    }
}