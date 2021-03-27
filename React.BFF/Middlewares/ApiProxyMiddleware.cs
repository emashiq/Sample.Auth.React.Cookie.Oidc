using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Configuration;
using ProxyKit;
using React.BFF.OidcConfiguration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace React.BFF
{
    public static class ApiProxyMiddleware
    {
        private const string UpstreamHost = "https://localhost:44397";

        /// <summary>
        /// Proxies the request to the Upstream API and attaches the Token to it..
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseApiProxy(this IApplicationBuilder app,IConfiguration configuration)
            => app.MapWhen(context => context.Request.GetDisplayUrl().Contains("/api/"), appInner =>
            {
                var oidcConfiguration = configuration.GetSection(nameof(OidcConfig)).Get<OidcConfig>();
                appInner.RunProxy(async context =>
                {

                    var host = (oidcConfiguration.IsDocker?"http://bff.api/":UpstreamHost);
                    var forwardContext = context.ForwardTo(host);

                    var token = await context.GetUserAccessTokenAsync();
                    forwardContext.UpstreamRequest.SetBearerToken(token);

                    return await forwardContext.Send();
                });
            });
    }
}
