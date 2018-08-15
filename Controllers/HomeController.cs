using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CSharpFinal.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

namespace CSharpFinal.Controllers
{
    public class HomeController : Controller
    {
        private CSharpFinalContext _context;
        public HomeController(CSharpFinalContext context)
        {
            _context = context;
        }

        [Route("")]
        public IActionResult LoginRegister()
        {
            return View("RegisterMain");
        }

        [HttpPost("/process/register")]
        public IActionResult ProcessRegister(RegisterValidator IncomingUser)
        {
            if(ModelState.IsValid)
            {
                PasswordHasher<RegisterValidator> Hasher = new PasswordHasher<RegisterValidator>();
                IncomingUser.Password = Hasher.HashPassword(IncomingUser, IncomingUser.Password);
                User NewUser = new User
                {
                    FirstName = IncomingUser.FirstName,
                    LastName = IncomingUser.LastName,
                    UserName = IncomingUser.UserName,
                    Wallet = 1000.00,
                    Password = IncomingUser.Password,
                };
                _context.Add(NewUser);
                _context.SaveChanges();
                HttpContext.Session.SetInt32("ActiveUserId", NewUser.UserId);
                HttpContext.Session.SetString("ActiveUserFName", NewUser.FirstName);
                return RedirectToAction("CurrentAuctions");
            }
            else
            {
                return View("RegisterMain");
            }
        }

        [HttpPost("process/login")]
        public IActionResult ProcessLogin(LoginValidator ReturningUser)
        {
            if(ModelState.IsValid)
            {
                var user = _context.Users.SingleOrDefault(u => u.UserName == ReturningUser.LoginUserName);
                if(user != null)
                {
                    var Hasher = new PasswordHasher<User>();
                    if(0 != Hasher.VerifyHashedPassword(user, user.Password, ReturningUser.LoginPassword))
                    {
                        HttpContext.Session.SetInt32("ActiveUserId", user.UserId);
                        HttpContext.Session.SetString("ActiveUserFName", user.FirstName);

                        return RedirectToAction("CurrentAuctions");
                    }
                    else
                    {
                        ViewBag.Error = "Unfortunately; your password did not match the registered user";
                        return View("RegisterMain");
                    }
                }
                else
                {
                    ViewBag.Error = "Unfortunately; your username did not match any registered usernames";
                    return View("RegisterMain");
                }
            }
            else
            {
                return View("RegisterMain");
            }
        }

        [Route("CurrentAuctions")]
        public IActionResult CurrentAuctions()
        {
            int ActiveUserId = HttpContext.Session.GetInt32("ActiveUserId").GetValueOrDefault(-1);
            if(ActiveUserId == -1){
                return View("RegisterMain");
            };
            User ActiveUser = _context.Users.SingleOrDefault(u => u.UserId == ActiveUserId);
            List<Product> Products = _context.Products.Include(s => s.Bidders_On).ThenInclude(bo => bo.User).Include(s => s.Seller).OrderBy(s => s.EndDate).ToList();
            DateTime currTime = DateTime.Now;
            foreach(var product in Products){
                if(product.EndDate <= currTime){
                    double MaxBid = product.CurrentBid;
                    User MaxBidder = ActiveUser;
                    foreach(var bid in product.Bidders_On){
                        if(bid.user_bid > MaxBid){
                            MaxBid = bid.user_bid;
                            MaxBidder = bid.User;
                        }
                    }
                    MaxBidder.Wallet = MaxBidder.Wallet - MaxBid;
                    product.Seller.Wallet = product.Seller.Wallet + MaxBid;
                    _context.Products.Remove(product);
                    _context.SaveChanges();
                }
            }


            ViewBag.Products = Products;
            ViewBag.ActiveUserId = ActiveUserId;
            ViewBag.ActiveUserFName = ActiveUser.FirstName;
            ViewBag.ActiveUserWallet = ActiveUser.Wallet;
            return View();
        }

        [HttpGet("auction/new")]
        public IActionResult NewAuction()
        {

            return View();
        }

        [HttpPost("process/new/auction")]
        public IActionResult ProcessNewAuction(AuctionValidator IncomingAuction)
        {
            if(ModelState.IsValid){

                Product NewProduct = new Product
                {
                    ProductName = IncomingAuction.ProductName,
                    ProductDesc = IncomingAuction.ProductDesc,
                    CurrentBid = IncomingAuction.CurrentBid,
                    EndDate = IncomingAuction.EndDate,
                    SellerId = HttpContext.Session.GetInt32("ActiveUserId").GetValueOrDefault(),
                };
                _context.Add(NewProduct);
                _context.SaveChanges();
                return RedirectToAction("CurrentAuctions");
            }
            else{
                return View("NewAuction");
            }
            
        }

        [HttpPost("view/product")]
        public IActionResult ViewProduct(int productid)
        {
            Product ThisProduct = _context.Products.Include(p => p.Seller).Include(p => p.Bidders_On).ThenInclude(bo => bo.User).SingleOrDefault(p => p.ProductId == productid);
            ViewBag.Product = ThisProduct;
            return View("ViewAuction");
        }

        [HttpPost("process/new/bid")]
        public IActionResult ProcessNewBid(int productid, double userbid, double maxbid)
        {
            Product ThisProduct = _context.Products.Include(p => p.Seller).Include(p => p.Bidders_On).ThenInclude(bo => bo.User).SingleOrDefault(p => p.ProductId == productid);
            int ActiveUserId = HttpContext.Session.GetInt32("ActiveUserId").GetValueOrDefault();
            User ActiveUser = _context.Users.SingleOrDefault(u => u.UserId == ActiveUserId);
            if(userbid > ActiveUser.Wallet){              
                ViewBag.Product = ThisProduct;
                ViewBag.Error = "Unfortunately; you don't actually have that much money";
                return View("ViewAuction");
            } else if(userbid < maxbid){
                ViewBag.Product = ThisProduct;
                ViewBag.Error = "You'll need to bid higher than that to win this one...";
                return View("ViewAuction");
            }
            Bidder NewBidder = new Bidder
            {
                UserId = ActiveUserId,
                ProductId = ThisProduct.ProductId,
                user_bid = userbid,
            };
            _context.Add(NewBidder);
            ThisProduct.Bidders_On.Add(NewBidder);
            ActiveUser.Bidder_On.Add(NewBidder);
            _context.SaveChanges();
            return RedirectToAction("CurrentAuctions");
        }

        [HttpPost("process/delete")]
        public IActionResult ProcessDeleteAuction(int productid){
            Product DoomedProduct = _context.Products.SingleOrDefault(p => p.ProductId == productid);
            int ActiveUserId = HttpContext.Session.GetInt32("ActiveUserId").GetValueOrDefault();
            if(ActiveUserId == DoomedProduct.SellerId){
                _context.Products.Remove(DoomedProduct);
                _context.SaveChanges();
            }
            return RedirectToAction("CurrentAuctions");
        }


        [Route("process/logout")]
        public IActionResult ProcessLogout(){
            HttpContext.Session.Clear();
            return RedirectToAction("LoginRegister");
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
