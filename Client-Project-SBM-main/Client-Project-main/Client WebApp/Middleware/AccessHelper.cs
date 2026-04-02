using System.Security.Claims;

namespace Client_WebApp.Middleware
{
    public static class AccessHelper
    {
        public static bool HasAccess(ClaimsPrincipal user, string screenCode, string actionType)
        {
            var claimType = $"Module_{screenCode}_{actionType}";
            var claim = user.Claims.FirstOrDefault(c => c.Type == claimType);
            return claim != null && bool.TryParse(claim.Value, out var hasAccess) && hasAccess;
        }
    }

}
