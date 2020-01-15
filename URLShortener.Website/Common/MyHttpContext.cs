using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace URLShortener.Website.Common
{
    public class MyHttpContext
    {
        private static IHttpContextAccessor _httpContextAccessor;
        public static HttpContext Current => _httpContextAccessor.HttpContext;
        public static string AppBaseUrl => $"{Current.Request.Scheme}://{Current.Request.Host}{Current.Request.PathBase}";
        internal static void Configure(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
    }
    public static class HttpContextExtensions
    {
        public static IApplicationBuilder UseMyHttpContext(this IApplicationBuilder app)
        {
            MyHttpContext.Configure(app.ApplicationServices.GetRequiredService<IHttpContextAccessor>());
            return app;
        }
    }
}
