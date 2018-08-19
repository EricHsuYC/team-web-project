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

        public ActionResult Login()
        {
            return View();
        }


        [HttpPost]
        public ActionResult Login(string id, string pwd)
        {
            var Member = db.member.Where(m => m.member_account == id && m.member_password == pwd).FirstOrDefault();
            if (Member == null)
            {
                ViewBag.Message = "帳號或密碼輸入錯誤";
                return View();
            }
            else
            {
                //Session["Member"] = Member.member_name;
                appClass.Member = Member.member_name.ToString();


                return RedirectToAction("Index");

            }


        }

        public ActionResult register()
        {
            return View();
        }


        [HttpPost]
        public ActionResult register(member member)
        {
            if ()
            {
                return View();
            }
            else
                var member = db.member.Where(m=>)


                return View();
                


            
        }





    }
}