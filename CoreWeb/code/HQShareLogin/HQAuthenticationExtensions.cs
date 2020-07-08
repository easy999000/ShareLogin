using CoreWeb.code.HQShareLogin;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class HQAuthenticationExtensions
    {
        public static AuthenticationBuilder AddHQAuthentication2(this AuthenticationBuilder builder
            , Action<HQAuthenticationHandlerOptions, CookieAuthenticationOptions> ConfigOption)
        {
            HQAuthenticationHandlerOptions option = new HQAuthenticationHandlerOptions();



            builder.AddHQCookie(HQAuthenticationHandlerOptions.HQAuthenticationName, o =>
            {

                o.Cookie.Name = "HQAuthentication." + HQAuthenticationHandlerOptions.HQAuthenticationName;

                ConfigOption?.Invoke(option, o);

                o.Events.OnSignedIn += option.SignedInFun;
                o.Events.OnSigningOut += option.SigningOut;
                o.Events.OnValidatePrincipal += option.CookieValidatePrincipal;

                o.TicketDataFormat = new HQSecureDataFormat();
                o.Cookie.Domain = option.Domain;

            });

            return builder;
        }


        public static AuthenticationBuilder AddHQCookie(this AuthenticationBuilder builder, string authenticationScheme, Action<CookieAuthenticationOptions> configureOptions)
            => builder.AddHQCookie(authenticationScheme, displayName: null, configureOptions: configureOptions);

        public static AuthenticationBuilder AddHQCookie(this AuthenticationBuilder builder, string authenticationScheme, string displayName, Action<CookieAuthenticationOptions> configureOptions)
        {
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<CookieAuthenticationOptions>, PostConfigureCookieAuthenticationOptions>());
            return builder.AddScheme<CookieAuthenticationOptions, HQCookieAuthenticationHandler>(authenticationScheme, displayName, configureOptions);
        }

    }
}
