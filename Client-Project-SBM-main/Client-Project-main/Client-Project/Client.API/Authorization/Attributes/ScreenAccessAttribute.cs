using Microsoft.AspNetCore.Authorization;

namespace Client.API.Authorization.Attributes
{
    public class ScreenAccessAttribute : AuthorizeAttribute
    {
        public ScreenAccessAttribute(string screenCode, string permission = "View")
        {
            Policy = $"{screenCode}_{permission}";
        }
    }
}
