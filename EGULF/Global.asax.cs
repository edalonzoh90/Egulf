using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace EGULF
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalFilters.Filters.Add(new Filters.SecurityFilterAction());
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        protected void Application_EndRequest(object sender, EventArgs e)
        {
            if(this.Response.StatusCode == 401)
            {
                Server.ClearError();
                Response.Clear();
                Response.Redirect("/Redirect/Unauthorized");
            }
        }
    }
}
