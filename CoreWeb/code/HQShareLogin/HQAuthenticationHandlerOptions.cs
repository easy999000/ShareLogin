using Microsoft.AspNetCore.Authentication.Cookies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreWeb.code.HQShareLogin
{
    public class HQAuthenticationHandlerOptions
    {
        public HQAuthenticationHandlerOptions()
        {

        }
        public string Domain { get; set; }

        /// <summary>
        /// 这个设置所有同步共享的站点要保持一致
        /// </summary>
        public static string HQAuthenticationName { get; set; } = "HQAuthentication";
        /// <summary>
        /// 退出
        /// </summary>
        public Action<string> LoginEvent { get; set; }
        /// <summary>
        /// 退出
        /// </summary>
        public Action LogOutEvent { get; set; }
        /// <summary>
        /// 是否登入 如果登入成功返回登入账号,否者返回 ""
        /// </summary>
        public Func<string> HasLoginEvent { get; set; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="Context"></param>
        /// <returns></returns>
        public Task SignedInFun(CookieSignedInContext Context)
        {

            var Account = Context.Principal.FindFirst("Account");

            if (Account != null)
            {
                UserLogin(Account.Value);
                 

            }

            return Task.CompletedTask;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Context"></param>
        /// <returns></returns>
        public Task SigningOut(CookieSigningOutContext Context)
        {
            var Account = Context.HttpContext.User.FindFirst("Account");

            if (Account != null)
            {
                UserLogout(); 

            }
            return Task.CompletedTask;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Context"></param>
        /// <returns></returns>
        public Task CookieValidatePrincipal(CookieValidatePrincipalContext Context)
        {
            if (Context == null || Context.Principal == null)
            {
                UserLogout();
                return Task.CompletedTask;

            }
            var Account = Context.Principal.FindFirst("Account");

            if (Account == null)
            {
                UserLogout();
                return Task.CompletedTask;
            } 

            UserLogin(Account.Value);




            return Task.CompletedTask;
        }

        /// <summary>
        /// 退出用户
        /// </summary>
        private void UserLogout()
        {
            var LoginAccount = this.HasLoginEvent?.Invoke();
            if (!string.IsNullOrWhiteSpace(LoginAccount))
            {
                this.LogOutEvent?.Invoke();
            }
            else
            {

            }

        }

        /// <summary>
        /// 登入用户
        /// </summary>
        /// <param name="Account"></param>
        private void UserLogin(string Account)
        {
            var LoginAccount = this.HasLoginEvent?.Invoke();
            if (!string.IsNullOrWhiteSpace(LoginAccount))
            {
                if (Account.ToLower() != LoginAccount.ToLower())
                {
                    this.LoginEvent?.Invoke(Account);
                }
            }
            else
            {
                this.LoginEvent?.Invoke(Account);
            }

        }


    }
}
