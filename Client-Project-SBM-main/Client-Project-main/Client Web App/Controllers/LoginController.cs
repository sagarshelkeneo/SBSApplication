using Client_Web_App.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Client_Web_App.Controllers
{
    public class LoginController : Controller
    {
        private readonly string _apiBaseUrl = "https://localhost:7165/api"; // change to your API URL

        [HttpGet]
        public ActionResult Index()
        {
            if (Session["token"] != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View(new LoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (var client = new HttpClient())
            {
                var requestData = new
                {
                    username = model.Username,
                    password = model.Password,
                    recaptchaToken = model.RecaptchaToken
                };

                var content = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");

                var response = await client.PostAsync($"{_apiBaseUrl}/User/login", content);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var authResponse = JsonConvert.DeserializeObject<AuthResponse>(jsonResponse);

                    Session["token"] = authResponse.Token;
                    TempData["SuccessMessage"] = "Logged in successfully!";

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError("", !string.IsNullOrEmpty(errorMessage) ? errorMessage : "Something went wrong.");
                    return View(model);
                }
            }
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index", "Login");
        }
    }
}