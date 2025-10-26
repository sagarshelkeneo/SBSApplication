//using System;
//using System.IdentityModel.Tokens.Jwt;
//using System.Linq;
//using Client_Web_App.Models;

//namespace Client_Web_App.Services
//{
//    public class LoginService
//    {
//        private readonly UserController _userService;

//        public LoginService()
//        {
//            _userService = new UserController(); // Direct call to API logic
//        }

//        public AuthResponse Login(LoginViewModel model)
//        {
//            // Call your API method directly
//            var loginDto = new Cl.Models.LoginDto
//            {
//                Username = model.Username,
//                Password = model.Password,
//                RecaptchaToken = model.RecaptchaToken
//            };

//            var authResponse = _userService.Login(loginDto);

//            if (authResponse == null || string.IsNullOrEmpty(authResponse.Token))
//                throw new Exception("Invalid username or password");

//            // Save JWT in session
//            System.Web.HttpContext.Current.Session["token"] = authResponse.Token;

//            return authResponse;
//        }

//        public void Logout()
//        {
//            System.Web.HttpContext.Current.Session.Clear();
//        }

//        public bool IsLoggedIn()
//        {
//            return System.Web.HttpContext.Current.Session["token"] != null;
//        }

//        public string Role()
//        {
//            var token = System.Web.HttpContext.Current.Session["token"] as string;
//            if (!string.IsNullOrEmpty(token))
//            {
//                var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
//                return jwt.Claims.FirstOrDefault(c => c.Type == "role")?.Value;
//            }
//            return null;
//        }

//        public string User()
//        {
//            var token = System.Web.HttpContext.Current.Session["token"] as string;
//            if (!string.IsNullOrEmpty(token))
//            {
//                var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
//                return jwt.Claims.FirstOrDefault(c => c.Type == "user")?.Value;
//            }
//            return null;
//        }
//    }
//}
