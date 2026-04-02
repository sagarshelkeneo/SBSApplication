using Client_WebApp.Services;
using Microsoft.AspNetCore.Mvc;
using Client_WebApp.Models;
using Microsoft.Extensions.Options;

namespace Client_WebApp.Controllers
{
    public class LoginController : Controller
    {
        private readonly LoginService _loginService;
        private readonly GoogleReCaptchaConfig _captchaConfig;

        public LoginController(LoginService loginService, IOptions<GoogleReCaptchaConfig> captchaConfig)
        {
            _loginService = loginService;
            _captchaConfig = captchaConfig.Value;
        }

        [HttpGet]
        public IActionResult Index()
        {
            if (_loginService.IsLoggedIn())
                return RedirectToAction("Index", "Home");

            TempData.Remove("ErrorMessage");

            //ViewBag.SiteKey = "6Ld9bIIrAAAAAP88S3Mdc5TVVnzqKRep7cqRIxli";
            ViewBag.SiteKey = _captchaConfig.SiteKey;
            return View(new LoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(LoginViewModel model)
        {
            //ViewBag.SiteKey = "6Ld9bIIrAAAAAP88S3Mdc5TVVnzqKRep7cqRIxli";
            ViewBag.SiteKey = _captchaConfig.SiteKey;

            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var token = await _loginService.LoginAsync(model);

                // Store token in session
                HttpContext.Session.SetString("token", token);

                TempData["SuccessMessage"] = "Logged in successfully!";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                TempData["ErrorMessage"] = ex.Message;
                return View(model);
            }
        }

        public IActionResult Logout()
        {
            _loginService.Logout();
            if (TempData["SuccessMessage"] == null)
                TempData["SuccessMessage"] = "Logged out successfully!";
            return RedirectToAction("Index");
        }
    }
}
