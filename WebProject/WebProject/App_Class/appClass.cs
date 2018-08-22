using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebProject.App_Class
{
    public class appClass
    {
        /// <summary>
        /// login member 
        /// </summary>
        public static string Member
        {
            get { return (HttpContext.Current.Session["Member"] == null) ? "" : HttpContext.Current.Session["Member"].ToString(); }
            set { HttpContext.Current.Session["Member"] = value; }

        }

        public static string Account
        {
            get { return (HttpContext.Current.Session["Account"] == null) ? "" : HttpContext.Current.Session["Account"].ToString(); }
            set { HttpContext.Current.Session["Account"] = value; }

        }



    }
}