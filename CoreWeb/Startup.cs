using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreWeb.code;
using CoreWeb.code.HQShareLogin;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualBasic.FileIO;

namespace CoreWeb
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.AddHttpContextAccessor();

            services.AddSession();

            ///添加路由
            services.AddRouting();

            ////////////
            ///设置登入共享参数
            ///////////////
            services.AddAuthentication(HQAuthenticationHandlerOptions.HQAuthenticationName)///这个很重要,要和mvc的保持一致
                .AddHQAuthentication2((options, CookieOption) =>
            {

                options.HasLoginEvent +=// class1.loginFun;
                () =>
               {
                   var httpContextAccessor = services.BuildServiceProvider().GetService<IHttpContextAccessor>();

                   var v1 = httpContextAccessor.HttpContext.Session.Get<HQuser>("user");

                   if (v1 == null)
                   {
                       return "";
                   }
                   Console.WriteLine("AddHQAuthentication2 HasLogin ");
                   return v1.account;
               };
                options.LoginEvent += o =>
                {

                    HQuser User = new HQuser() { account = o, password = o, id = 1 };


                    var httpContextAccessor = services.BuildServiceProvider().GetService<IHttpContextAccessor>();

                    httpContextAccessor.HttpContext.Session.Set<HQuser>("user", User);


                    Console.WriteLine("AddHQAuthentication2 Login ");

                };
                options.LogOutEvent += () =>
                {
                    var httpContextAccessor = services.BuildServiceProvider().GetService<IHttpContextAccessor>();

                    httpContextAccessor.HttpContext.Session.Set<HQuser>("user", null);

                    Console.WriteLine("AddHQAuthentication2 LogOut ");
                 };

                options.Domain = "hqbuy.com";

                CookieOption.AccessDeniedPath = "/home/index?LoginMsg=AccessDeniedPath";
                CookieOption.LoginPath = "/home/index?LoginMsg=LoginPath";
                CookieOption.LogoutPath = "/home/index?LoginMsg=LogoutPath";
                CookieOption.ExpireTimeSpan = new TimeSpan(0, 5, 0);

            });


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSession();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
