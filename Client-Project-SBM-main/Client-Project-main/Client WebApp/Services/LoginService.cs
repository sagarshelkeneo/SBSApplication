using System.IdentityModel.Tokens.Jwt;
using Client.Application.Features.User.Dtos; // LoginDto
using Client_WebApp.Models;
using Client.Application.Interfaces; // AuthResponse (make sure you have this DTO)

namespace Client_WebApp.Services
{
    public class LoginService
    {
        private readonly IUserRepository _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LoginService(IUserRepository userRepository, IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<string> LoginAsync(LoginViewModel model)
        {
            var loginDto = new LoginDto
            {
                Username = model.Username,
                Password = model.Password,
                RecaptchaToken = model.RecaptchaToken
            };

            var result = await _userRepository.ValidateUserAsync(loginDto);

            if (!string.IsNullOrEmpty(result.Token))
            {
                _httpContextAccessor.HttpContext.Session.SetString("token", result.Token);
                return result.Token;
            }

            throw new Exception(result.Message ?? "Invalid username or password.");
        }

        public void Logout()
        {
            _httpContextAccessor.HttpContext.Session.Clear();
        }

        public bool IsLoggedIn()
        {
            return !string.IsNullOrEmpty(_httpContextAccessor.HttpContext.Session.GetString("token"));
        }

        public string Role()
        {
            var token = _httpContextAccessor.HttpContext.Session.GetString("token");
            if (token != null)
            {
                var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
                return jwt.Claims.FirstOrDefault(c => c.Type == "role")?.Value;
            }
            return null;
        }
    }
}
