using Security.Models;
using System.Collections.Generic;
using System.Web;

namespace EGULF.Helpers
{
    public class SessionWeb
    {
        static string user = "user";
        static string resources = "resources";
        static string key = "key";
        static string iv = "iv";


        public static string Iv
        {
            get
            {
                if (HttpContext.Current == null)
                    return null;
                var u = HttpContext.Current.Session[iv];
                if (u != null)
                    return (string)u;
                return null;
            }
            set
            {
                HttpContext.Current.Session[iv] = value;
            }
        }

        public static string Key
        {
            get
            {
                if (HttpContext.Current == null)
                    return null;
                var u = HttpContext.Current.Session[key];
                if (u != null)
                    return (string)u;
                return null;
            }
            set
            {
                HttpContext.Current.Session[key] = value;
            }
        }

        public static User User
        {
            get
            {
                if (HttpContext.Current == null)
                    return null;
                var u = HttpContext.Current.Session[user];
                if (u != null)
                    return (User)u;
                return null;
            }
            set
            {
                HttpContext.Current.Session[user] = value;
            }
        }

        public static List<Resource> Resources
        {
            get
            {
                if (HttpContext.Current == null)
                    return new List<Resource>();
                var m = HttpContext.Current.Session[resources];
                if (m != null)
                    return (List<Resource>)m;
                return null;
            }
            set
            {
                HttpContext.Current.Session[resources] = value;
            }
        }
    }
}