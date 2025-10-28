using Client_WebApp.Models;
using Client_WebApp.Services;
using Client_WebApp.Services.Config;
using Client_WebApp.Services.Master;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace Client_WebApp.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly LoginService _loginService;

        private readonly UserService _userService;
        private readonly RoleAccessService _roleAccessService;

        public HomeController(ILogger<HomeController> logger, LoginService loginService, UserService userService, RoleAccessService roleAccessService)
        {
            _logger = logger;
            _loginService = loginService;
            _roleAccessService = roleAccessService;
            _userService = userService;
        }
       
        public async Task<IActionResult> Index()
        {
            var role = _loginService.Role();
            if (string.IsNullOrEmpty(role))
            {
                _loginService.Logout();
                return RedirectToAction("Index", "Login");
            }
            else
            {
                await AddAuth();
            }

            ViewBag.UserRole = role;
            return View();
        }


        public bool CheckReportAccess(string? role)
        {
            return role == "Super Admin" || role == "Admin" || role == "Manager" || role == "report_user";
        }

        //--------------impliment auth

        public async Task AddAuth()
        {
            var getUserInfo = await _userService.GetAllUsersAsync(CurrentCompanyId, CurrentUserId, null);
            var currentUser = getUserInfo?.FirstOrDefault(u => u.Id == CurrentUserId);
            if (currentUser == null)
                throw new Exception("User not found.");

            string? roleName = currentUser.RoleName;
            int? roleId = currentUser.RoleMasterId;

            var getRoleAccess = await _roleAccessService.GetRoleAccessByRoleIdAsync(roleId ?? 0);

           
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, CurrentUserId.ToString()),
        new Claim(ClaimTypes.Role, roleName ?? string.Empty)
    };

            foreach (var access in getRoleAccess)
            {
                claims.Add(new Claim($"Module_{access.A_screenCode}_View", access.A_viewAccess.ToString()));
                claims.Add(new Claim($"Module_{access.A_screenCode}_Create", access.A_createAccess.ToString()));
                claims.Add(new Claim($"Module_{access.A_screenCode}_Edit", access.A_editAccess.ToString()));
                claims.Add(new Claim($"Module_{access.A_screenCode}_Delete", access.A_deleteAccess.ToString()));
            }

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            // ? specify the scheme explicitly
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }


        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
