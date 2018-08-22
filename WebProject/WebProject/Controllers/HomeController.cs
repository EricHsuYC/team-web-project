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
                appClass.Account = Member.member_account.ToString();



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
            if (member == null)
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


        //顯示購物車
        public ActionResult Cart()
        {

            var user = appClass.Member;
            var CartList = db.shopping_cart.Where(m => m.member_account == user).ToList();
            return View(CartList);
        }



        [HttpPost]
        public ActionResult AddCart(string productNo, int qantity, string productName)
        {

            var user = appClass.Member;
            if (appClass.Member == "")
            {
                //ViewBag.Alert = "請先登入!";
                return RedirectToAction("Login");
            }
            else
            {


                shopping_cart newItem = new shopping_cart();
                //var buyer = db.member.Where(m => m.member_name == user).FirstOrDefault();
                
                var item = db.shopping_cart.Where(m => m.product_no == productNo).FirstOrDefault();

                //若購物車內已有相同商品則只更改數量
                if (item == null)
                {
                    newItem.member_account = appClass.Account;
                    newItem.product_no = productNo;
                    newItem.product_quantity = qantity;
                    db.shopping_cart.Add(newItem);
                    db.SaveChanges();

                }
                else
                {
                    item.product_quantity = qantity;
                    db.SaveChanges();
                }
                TempData["Bought"] = "true";
                return RedirectToAction("ProductDetail", new { productName = productName, productNo = productNo});




            }


        }


        //會員的訂單列表
        public ActionResult OrderList()
        {
            var user = appClass.Member;
            //找到該會員的訂單
            var orders = db.order_form.Where(m => m.member_account == user).OrderByDescending(m => m.order_date).ToList();


            return View();
        }


        //產品頁面
        public ActionResult ProductDetail(string productName, string productNo)
        {
            ViewBag.productName = productName;
            var productdetail = db.product.Where(m => m.product_name == productName).FirstOrDefault();
            var cartitem = db.shopping_cart.Where(m => m.member_account == appClass.Account && m.product_no == productNo).FirstOrDefault();
            if(cartitem == null)
            {
                ViewBag.AllowBuy = "true";
            }
            else
            {
                ViewBag.AllowBuy = "false";

            }
            return View(productdetail);

        }




    }
}