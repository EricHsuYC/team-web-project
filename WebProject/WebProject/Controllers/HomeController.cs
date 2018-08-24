using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebProject.Models;
using WebProject.App_Class;
using WebProject.ViewModel;
using System.Data.Entity;
using PagedList;



namespace WebProject.Controllers
{
    public class HomeController : Controller
    {

        team_web_projectEntities db = new team_web_projectEntities();

        int pageSize = 12;

        // GET: Home
        public ActionResult Index(int page = 1)
        {
            int currentPage = page < 1 ? 1 : page;
            var products = db.product.OrderBy(m=>m.product_no).ToList();
            ViewBag.Welcome = "歡迎光臨! " + appClass.Member;
            var result = products.ToPagedList(currentPage, pageSize);

            return View(result);
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

            var cartItem = db.shopping_cart.Where(m=>m.member_account == user).Include(m => m.product);
            //var cartItem = db.product.Include(m => m.shopping_cart);




            //CartViewModel cm = new CartViewModel()
            //{
            //    CartItems = db.shopping_cart.Where(m => m.member_account == user).ToList(),
            //    Products = db.product.ToList()
            //};
            return View(cartItem.ToList());

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


        
        public ActionResult DeleteCart(int rowid)
        {
            var item = db.shopping_cart.Where(m => m.rowid == rowid).FirstOrDefault();
            db.shopping_cart.Remove(item);
            db.SaveChanges();
            var user = appClass.Member;

            return RedirectToAction("Cart");
        }


        [HttpPost]
        public ActionResult AddOrder(string rname, string rphone, string raddress, string remark)
        {
            var time = DateTime.Now;
            //string sqlFormattedDate = time.ToString("yyyy-MM-dd HH:mm:ss.fff");
            order_form order = new order_form();





            order.order_date = time;
            order.member_account = appClass.Account;
            order.remittance_account = "0000-0000-0000";
            order.payment_method = "匯款";
            order.shipping_status = "已接收訂單";
            order.recipient_name = rname;
            order.recipient_phone = rphone;
            order.recipient_address = raddress;
            order.remark = remark;
            db.order_form.Add(order);
            try { db.SaveChanges(); }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {

                throw ex;
            }

            order_detail orderDetail = new order_detail();
            var cartlist = db.shopping_cart.Where(m => m.member_account == appClass.Account).ToList();
            foreach (var item in cartlist)
            {
                orderDetail.order_id = order.order_id;
                orderDetail.product_quantity = item.product_quantity;
                orderDetail.product_no = item.product_no;
                db.order_detail.Add(orderDetail);
                try { db.SaveChanges(); }
                catch (System.Data.Entity.Validation.DbEntityValidationException ex)
                {

                    throw ex;
                }
            }
            var clearCart = db.shopping_cart.Where(m => m.member_account == appClass.Account).ToList();
            foreach (var item in clearCart)
            {
                var deleteItem = db.shopping_cart.Where(m => m.member_account == appClass.Account).FirstOrDefault();
                db.shopping_cart.Remove(deleteItem);
                db.SaveChanges();
            }







            return RedirectToAction("OrderList");

            //return View();
        }






        //會員的訂單列表
        public ActionResult OrderList()
        {
            var orders = db.order_form.Where(m => m.member_account == appClass.Account).OrderByDescending(m => m.order_date).ToList();


            return View(orders);
        }

        public ActionResult OrderDetail(int id)
        {

            var od = db.order_detail.Where(m => m.order_id == id).Include(m => m.order_form).Include(m=>m.product);


            return View(od);
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