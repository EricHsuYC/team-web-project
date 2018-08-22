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

        public ActionResult Register()
        {
            return View();
        }


        [HttpPost]
        public ActionResult Register(member input_member)
        {
            if (ModelState.IsValid == false)
            {
                return View();
            }

            var member = db.member.Where(m => m.member_account == input_member.member_account).FirstOrDefault();
            if(member == null)
            {
                db.member.Add(input_member);
                db.SaveChanges();
                return RedirectToAction("Login");
            }
            else
            {
                ViewBag.Message = "此電子信箱已被註冊，請嘗試其他電子信箱";
                return View();
            }
                   
              
            
        }


        public ActionResult Logout()
        {
            Session.Clear();

            return RedirectToAction("Index");
        }

        
        //購物車
        public ActionResult Cart()
        {

            var user = appClass.Member;
            var CartList = db.shopping_cart.Where(m => m.member_account == user).ToList();
            return View(CartList);
        }


        //會員的訂單列表
        public ActionResult OrderList()
        {
            var user = appClass.Member;
            //找到該會員的訂單
            var orders = db.order_form.Where(m => m.member_account == user).OrderByDescending(m=>m.order_date).ToList();
            

            return View();
        }


        //產品頁面
        public ActionResult ProductDetail(string productName)
        {
            ViewBag.productName = productName;
            var productdetail = db.product.Where(m => m.product_name == productName).FirstOrDefault();
            return View(productdetail);

        }




    }
}