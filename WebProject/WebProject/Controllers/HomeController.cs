using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebProject.Models;
using WebProject.App_Class;
using WebProject.ViewModel;

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

        //註冊
        public ActionResult Register()
        {
            return View();
        }

        //註冊
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

        //登出
        public ActionResult Logout()
        {
            Session.Clear();

            return RedirectToAction("Index");
        }


        //顯示購物車
        public ActionResult Cart()
        {
            var user = appClass.Account;

            CartViewModel cm = new CartViewModel()
            {
                CartItems = db.shopping_cart.Where(m => m.member_account == user).ToList(),
                Products = db.product.ToList()
            };
            return View(cm);

        } 






            //var CartPNo = db.shopping_cart.Where(m => m.member_account == user).Select(m => m.product_no).ToList();

            //var cart = db.shopping_cart.Where(m => m.member_account == user);
            //var pNo = db.product.Where(m => m.product_no == CartPNo.ToString()).ToList();

            //var CartItem = from s in cart
            //          join p in db.product on s.product_no equals p.product_no
            //          select new { s.product_no, s.member_account, s.product_quantity, p.product_description, p.product_image, p.product_name, p.product_price };
            //ViewBag.CartItem = CartItem.ToList();



            //var ProductList = db.product.Where(m => m.product_no == CartList.)





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
                
                var item = db.shopping_cart.Where(m => m.member_account == appClass.Account && m.product_no == productNo).FirstOrDefault();

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


                //加入購物車後回傳狀態給jquery執行提示視窗
                //TempData["Bought"] = "true";
                return RedirectToAction("ProductDetail", new { productName = productName, productNo = productNo, showPrompt = "true"});




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
        public ActionResult ProductDetail(string productName, string productNo, string showPrompt)
        {
            ViewBag.productName = productName;
            var productdetail = db.product.Where(m => m.product_name == productName).FirstOrDefault();
            var cartitem = db.shopping_cart.Where(m => m.member_account == appClass.Account && m.product_no == productNo).FirstOrDefault();

            //購物車是否已經有該產品
            if (cartitem == null)
            {
                ViewBag.AllowBuy = "true";
            }
            else
            {
                ViewBag.AllowBuy = "false";

            }
            ViewBag.Bought = showPrompt;
            return View(productdetail);

        }




    }
}