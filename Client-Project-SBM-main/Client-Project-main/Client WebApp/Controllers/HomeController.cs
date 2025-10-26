using Client_WebApp.Models;
using Client_WebApp.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Client_WebApp.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly LoginService _loginService;

        public HomeController(ILogger<HomeController> logger, LoginService loginService)
        {
            _logger = logger;
            _loginService = loginService;
        }

        public IActionResult Index()
        {
            var role = _loginService.Role();
            if (string.IsNullOrEmpty(role))
            {
                _loginService.Logout();
                return RedirectToAction("Index", "Login");
            }

            ViewBag.UserRole = role;
            return View();
        }

        public bool CheckReportAccess(string? role)
        {
            return role == "Super Admin" || role == "Admin" || role == "Manager" || role == "report_user";
        }
    }
}
