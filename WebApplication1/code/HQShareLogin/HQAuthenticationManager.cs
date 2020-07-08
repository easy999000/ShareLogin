
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using WebApplication1.code.HQShareLogin;

[assembly: OwinStartup(typeof(HQAuthenticationManager))]
namespace WebApplication1.code.HQShareLogin
{
    public class HQAuthenticationManager
    {
        /// <summary>
        /// 策略名称,和core保持一致
        /// </summary>
        public static string HQAuthenticationName = "HQAuthentication";
        /// <summary>
        /// cookie域名,同步站点保持一致
        /// </summary>
        public static string CookieDomain = "hqbuy.hqenet.com";
        public void Configuration(IAppBuilder app)
        { 
             

            // 使应用程序可以使用 Cookie 来存储已登录用户的信息
            // 并使用 Cookie 来临时存储有关使用第三方登录提供程序登录的用户的信息
            // 配置登录 Cookie
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                //AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                AuthenticationType = HQAuthenticationName
                ,
                // LoginPath = new PathString("/Account/Login"),
                CookieDomain = CookieDomain
                ,
                CookieName = "HQAuthentication." + HQAuthenticationName
                ,
                TicketDataFormat = new HQSecureDataFormat()
                ,
                ExpireTimeSpan = new TimeSpan(0, 20, 0)
                 
                //,
                //,
                //Provider = new CookieAuthenticationProvider
                //{

                //    // 当用户登录时使应用程序可以验证安全戳。
                //    // 这是一项安全功能，当你更改密码或者向帐户添加外部登录名时，将使用此功能。
                //    //OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>(
                //    //    validateInterval: TimeSpan.FromMinutes(30),
                //    //    regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
                //}
            }); ;

        }

        //private IAuthenticationManager AuthenticationManager
        //{
        //    get
        //    {

        //        return HttpContext.Current.GetOwinContext().Authentication;

        //    }
        //}

        public static void HQLogin(string Account)
        {
            var AuthenticationManager = HttpContext.Current.GetOwinContext().Authentication;

            Claim claim = new Claim("Account", Account);

            var claimsIdentity = new ClaimsIdentity(HQAuthenticationName);
            //var claimsIdentity = new ClaimsIdentity(DefaultAuthenticationTypes.ApplicationCookie);

            claimsIdentity.AddClaim(claim);
            // 3. 将上面拿到的identity对象登录
            AuthenticationManager.SignIn(claimsIdentity);


        }
        public static void HQLogout()
        {
            var AuthenticationManager = HttpContext.Current.GetOwinContext().Authentication;
             
            // 3. 将上面拿到的identity对象登录
            AuthenticationManager.SignOut();
        }
    }
}