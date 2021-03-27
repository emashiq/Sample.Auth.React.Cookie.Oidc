using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace React.BFF
{
    public static class AuthorizeSpaMiddleware
    {
        /// <summary>
        /// Triggers the authentication if the default spa endpoint is visited.
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseAuthorizeSpa(this IApplicationBuilder app)
            => app.Use(async (context, next) =>
            {
                var isDashboardPage = context.Request.Path == "/";
                if (!context.User.Identity.IsAuthenticated && isDashboardPage)
                {
                    await context.ChallengeAsync();
                    return;
                }
                await next();
            });
    }
}
