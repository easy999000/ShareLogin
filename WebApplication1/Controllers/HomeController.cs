using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.code;
using WebApplication1.code.HQShareLogin;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index(string LoginMsg = null)
        {
            ViewData["LoginMsg"] = LoginMsg;
            return View();
        }

        public ActionResult Logout()
        {
            this.Session["user"] = null;
            HQAuthenticationManager.HQLogout();
            return RedirectToAction("Main");
        }



        // GET: Home
        public ActionResult Login(string Account, string Password)
        {
            switch (Account.ToLower())
            {
                case "admin":
                    if (Password.ToLower() != "admin")
                    {
                        return wrongPassword();

                    }
                    break;
                case "aaa":
                    if (Password.ToLower() != "aaa")
                    {
                        return wrongPassword();

                    }
                    break;
                case "bbb":
                    if (Password.ToLower() != "bbb")
                    {
                        return wrongPassword();

                    }
                    break;
                case "ccc":
                    if (Password.ToLower() != "ccc")
                    {
                        return wrongPassword();

                    }
                    break;
                default:
                    return wrongPassword();
                    break;
            }

            code.HQuser user = new code.HQuser() { account = Account, password = Password, id = 1 };

            this.Session["user"] = user;
            HQAuthenticationManager.HQLogin(Account);
            return RedirectToAction("Main");
            // return View();
        }
        private ActionResult wrongPassword()
        {

            return RedirectToAction("Index", new { LoginMsg = "密码错误" });
        }

        // GET: Home
        public ActionResult Main()
        {
            code.HQuser user = null;
            if (this.Session["user"] != null)
            {
                user = (HQuser)this.Session["user"];
            }
            else
            {
                user = new HQuser();
            }

            return View(user);
        }










    }
}