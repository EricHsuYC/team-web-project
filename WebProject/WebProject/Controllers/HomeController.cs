using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebProject.Models;
using WebProject.App_Class;

namespace WebProject.Controllers
{
    public class HomeController : Controller
    {

        team_web_projectEntities db = new team_web_projectEntities();
        

        // GET: Home
        public ActionResult Index()
        {

            var products = db.product;
            if (appClass.Member != "")
            {

            }



            return View(products);
        }

        
    }
}