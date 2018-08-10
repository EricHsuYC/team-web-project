
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using prjShoppingCar.Models;

namespace prjShoppingCar.Controllers
{
    public class HomeController : Controller
    {
        //建立可存取dbShoppingCar.mdf 資料庫的dbShoppingCarEntities 類別物件db
        dbShoppingCarEntities db = new dbShoppingCarEntities();
        
        // Get:Home/Index
        public ActionResult Index()
        {
            //取得所有產品放入products
            var products = db.tProduct.ToList();
            //若Session["Member"]為空，表示會員未登入
            if (Session["Member"] == null)
            {
                //指定Index.cshtml套用_Layout.cshtml，View使用products模型
                return View("Index", "_Layout", products);
            }
            //會員登入狀態
            //指定Index.cshtml套用_LayoutMember.cshtml，View使用products模型
            return View("Index", "_LayoutMember", products);
        }

        //Get: Home/Login
        public ActionResult Login()
        {
            return View();
        }
        //Post: Home/Login
        [HttpPost]
        public ActionResult Login(string fUserId, string fPwd)
        {
            // 依帳密取得會員並指定給member
            var member = db.tMember
                .Where(m => m.fUserId == fUserId && m.fPwd == fPwd)
                .FirstOrDefault();
            //若member為null，表示會員未註冊
            if (member == null)
            {
                ViewBag.Message = "帳密錯誤，登入失敗";
                return View();
            }
            //使用Session變數記錄歡迎詞
            Session["WelCome"] = member.fName + "歡迎光臨";
            //使用Session變數記錄登入的會員物件
            Session["Member"] = member;
            //執行Home控制器的Index動作方法
            return RedirectToAction("Index");
        }

        //Get:Home/Register
        public ActionResult Register()
        {
            return View();
        }
        //Post:Home/Register
        [HttpPost]
        public ActionResult Register(tMember pMember)
        {
            //若模型沒有通過驗證則顯示目前的View
            if (ModelState.IsValid == false)
            {
                return View();
            }
            // 依帳號取得會員並指定給member
            var member = db.tMember
                .Where(m => m.fUserId == pMember.fUserId)
                .FirstOrDefault();
            //若member為null，表示會員未註冊
            if (member == null)
            {
                //將會員記錄新增到tMember資料表
                db.tMember.Add(pMember);
                db.SaveChanges();
                //執行Home控制器的Login動作方法
                return RedirectToAction("Login");
            }
            ViewBag.Message = "此帳號己有人使用，註冊失敗";
            return View();
        }

        //Get:Index/Logout
        public ActionResult Logout()
        {
            Session.Clear();  //清除Session變數資料
            return RedirectToAction("Index");
        }

        //Get:Index/ShoppingCar
        public ActionResult ShoppingCar()
        {
            //取得登入會員的帳號並指定給fUserId
            string fUserId = (Session["Member"] as tMember).fUserId;
            //找出未成為訂單明細的資料，即購物車內容
            var orderDetails = db.tOrderDetail.Where
                (m => m.fUserId == fUserId && m.fIsApproved == "否")
                .ToList();
            //指定ShoopingCar.cshtml套用_LayoutMember.cshtml，View使用orderDetails模型
            return View("ShoppingCar", "_LayoutMember", orderDetails);
        }

        //Get:Index/AddCar
        public ActionResult AddCar(string fPId)
        {
            //取得會員帳號並指定給fUserId
            string fUserId = (Session["Member"] as tMember).fUserId;
            //找出會員放入訂單明細的產品，該產品的fIsApproved為"否"
            //表示該產品是購物車狀態
            var currentCar = db.tOrderDetail
                .Where(m => m.fPId == fPId && m.fIsApproved == "否" && m.fUserId == fUserId)
                .FirstOrDefault();
            //若currentCar等於null，表示會員選購的產品不是購物車狀態
            if (currentCar == null)
            {
                //找出目前選購的產品並指定給product
                var product = db.tProduct.Where(m => m.fPId == fPId).FirstOrDefault();
                //將產品放入訂單明細，因為產品的fIsApproved為"否"，表示為購物車狀態
                tOrderDetail orderDetail = new tOrderDetail();
                orderDetail.fUserId = fUserId;
                orderDetail.fPId = product.fPId;
                orderDetail.fName = product.fName;
                orderDetail.fPrice = product.fPrice;
                orderDetail.fQty = 1;
                orderDetail.fIsApproved = "否";
                db.tOrderDetail.Add(orderDetail);
            }
            else
            {
                //若產品為購物車狀態，即將該產品數量加1
                currentCar.fQty += 1;
            }
            db.SaveChanges();
            //執行Home控制器的ShoppingCar動作方法
            return RedirectToAction("ShoppingCar");
        }

        //Get:Index/DeleteCar
        public ActionResult DeleteCar(int fId)
        {
            // 依fId找出要刪除購物車狀態的產品
            var orderDetail = db.tOrderDetail.Where
                (m => m.fId == fId).FirstOrDefault();
            //刪除購物車狀態的產品
            db.tOrderDetail.Remove(orderDetail);
            db.SaveChanges();
            //執行Home控制器的ShoppingCar動作方法
            return RedirectToAction("ShoppingCar");
        }

        //Post:Index/ShoppingCar
        [HttpPost]
        public ActionResult ShoppingCar(string fReceiver, string fEmail, string fAddress)
        {
            //找出會員帳號並指定給fUserId
            string fUserId = (Session["Member"] as tMember).fUserId;
            //建立唯一的識別值並指定給guid變數，用來當做訂單編號
            //tOrder的fOrderGuid欄位會關聯到tOrderDetail的fOrderGuid欄位
            //形成一對多的關係，即一筆訂單資料會對應到多筆訂單明細
            string guid = Guid.NewGuid().ToString();
            //建立訂單主檔資料
            tOrder order = new tOrder();
            order.fOrderGuid = guid;
            order.fUserId = fUserId;
            order.fReceiver = fReceiver;
            order.fEmail = fEmail;
            order.fAddress = fAddress;
            order.fDate = DateTime.Now;
            db.tOrder.Add(order);
            //找出目前會員在訂單明細中是購物車狀態的產品
            var carList = db.tOrderDetail
                .Where(m => m.fIsApproved == "否" && m.fUserId == fUserId)
                .ToList();
            //將購物車狀態產品的fIsApproved設為"是"，表示確認訂購產品
            foreach (var item in carList)
            {
                item.fOrderGuid = guid;
                item.fIsApproved = "是";
            }
            //更新資料庫，異動tOrder和tOrderDetail
            //完成訂單主檔和訂單明細的更新
            db.SaveChanges();
            //執行Home控制器的OrderList動作方法
            return RedirectToAction("OrderList");
        }

        //Get:Home/OrderList
        public ActionResult OrderList()
        {
            //找出會員帳號並指定給fUserId
            string fUserId = (Session["Member"] as tMember).fUserId;
            //找出目前會員的所有訂單主檔記錄並依照fDate進行遞增排序
            //將查詢結果指定給orders
            var orders = db.tOrder.Where(m => m.fUserId == fUserId)
                .OrderByDescending(m => m.fDate).ToList();
            //目前會員的訂單主檔
            //指定OrderList.cshtml套用_LayoutMember.cshtml，View使用orders模型
            return View("OrderList", "_LayoutMember", orders);
        }

        //Get:Index/OrderDetail
        public ActionResult OrderDetail(string fOrderGuid)
        {
            //根據fOrderGuid找出和訂單主檔關聯的訂單明細，並指定給orderDetails
            var orderDetails = db.tOrderDetail
                .Where(m => m.fOrderGuid == fOrderGuid).ToList();
            //目前訂單明細
            //指定OrderDetail.cshtml套用_LayoutMember.cshtml，View使用orderDetails模型
            return View("OrderDetail", "_LayoutMember", orderDetails);
        }
    }
}