using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Client_Web_App.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(10, MinimumLength = 4,
            ErrorMessage = "Password must be between 4 and 10 characters.")]
        [RegularExpression("^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])(?=.*[@#$%^&\\-+=()])(?=\\S+$).{4,10}$",
            ErrorMessage = "Password must contain at least 1 uppercase, 1 lowercase, 1 number, and 1 special character.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Captcha is required")]
        public string RecaptchaToken { get; set; }
    }

    public class AuthResponse
    {
        public string Token { get; set; }
    }

    public class JwtClaims
    {
        public string User { get; set; }
        public int UserId { get; set; }
        public string Role { get; set; }
        public int CompanyID { get; set; }
        public string Email { get; set; }
    }
}