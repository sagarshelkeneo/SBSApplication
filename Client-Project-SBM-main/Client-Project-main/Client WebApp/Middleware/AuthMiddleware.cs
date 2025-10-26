using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Client_WebApp.Middleware
{
    public class AuthMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value?.ToLower();

            // Allow login and static resources without restriction
            if (path.Contains("/login") || path.Contains("/account/login") ||
                path.Contains("/css") || path.Contains("/js") || path.Contains("/images"))
            {
                await _next(context);
                return;
            }

            // Check if session token exists
            var token = context.Session.GetString("token");

            if (string.IsNullOrEmpty(token))
            {
                context.Response.Redirect("/Login");
                return;
            }

            await _next(context);
        }
    }
}
