using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreWeb.code;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CoreWeb.Controllers
{
    public class HomeController : Controller
    {
        IHttpContextAccessor _HttpContextAccessor;
        public HomeController(IHttpContextAccessor HttpContextAccessor)
        {
            _HttpContextAccessor = HttpContextAccessor;
        }
        public IActionResult Index(string LoginMsg = null)
        {
            ViewData["LoginMsg"] = LoginMsg;
            return View();
        }
        public ActionResult Login(string Account, string Password)
        {

            return wrongPassword("只能从 .net mvc 端登入");
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

            //this.Session["user"] = user;
            _HttpContextAccessor.HttpContext.Session.Set<HQuser>("user", user);

            return RedirectToAction("Main");
            // return View();
        }
        private ActionResult wrongPassword(string msg = null
            )
        {
            if (msg==null)
            {
                msg = "密码错误";
            }

            return RedirectToAction("Index", new { LoginMsg = msg });
        }



        public IActionResult Main()
        {
            var user = _HttpContextAccessor.HttpContext.Session.Get<HQuser>("user");
            if (user == null)
            {
                user = new HQuser();
                user.account = "没登入";
            }

            return View(user);
        }

    }
}
