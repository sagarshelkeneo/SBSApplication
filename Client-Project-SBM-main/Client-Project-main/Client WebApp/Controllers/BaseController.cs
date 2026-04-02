using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Mvc;

namespace Client_WebApp.Controllers
{
    public class BaseController : Controller
    {
        protected string CurrentUserName { get; private set; }
        protected string CurrentUserEmail { get; private set; }
        protected int CurrentUserId { get; private set; }
        protected int CurrentCompanyId { get; private set; }
        protected string CurrentUserRole { get; private set; }

        public BaseController()
        {
            CurrentUserId = 0;
            CurrentCompanyId = 0;
            CurrentUserName = "";
            CurrentUserEmail = "";
            CurrentUserRole = "";
        }

        public override void OnActionExecuting(Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            var tokenString = HttpContext.Session.GetString("token");
            if (!string.IsNullOrEmpty(tokenString))
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(tokenString);

                CurrentUserId = int.Parse(jwtToken.Claims.FirstOrDefault(c => c.Type == "userId")?.Value ?? "0");
                CurrentUserName = jwtToken.Claims.FirstOrDefault(c => c.Type == "user")?.Value ?? "";
                CurrentUserEmail = jwtToken.Claims.FirstOrDefault(c => c.Type == "Email")?.Value ?? "";
                CurrentCompanyId = int.Parse(jwtToken.Claims.FirstOrDefault(c => c.Type == "CompanyID")?.Value ?? "0");
                CurrentUserRole = jwtToken.Claims.FirstOrDefault(c => c.Type == "role" || c.Type == "Role" || c.Type == "UserRole")?.Value ?? "Guest";


                ViewBag.UserId = CurrentUserId;
                ViewBag.CompanyId = CurrentCompanyId;
                ViewBag.Username = CurrentUserName;
                ViewBag.Email = CurrentUserEmail;
                ViewBag.UserRole = CurrentUserRole;
            }
        }
    }

}
