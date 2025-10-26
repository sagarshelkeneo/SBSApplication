using Microsoft.AspNetCore.Authorization;

namespace Client.API.Authorization.Requirements
{
    public class ScreenAccessRequirement : IAuthorizationRequirement
    {
        public string ScreenCode { get; }
        public string Permission { get; }

        public ScreenAccessRequirement(string screenCode, string permission)
        {
            ScreenCode = screenCode;
            Permission = permission;
        }
    }
}
