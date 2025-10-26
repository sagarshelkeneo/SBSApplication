using System.Security.Claims;
using Client.API.Authorization.Requirements;
using Client.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace Client.API.Authorization.Handlers
{
    public class ScreenAccessHandler : AuthorizationHandler<ScreenAccessRequirement>
    {
        private readonly IRoleAccessRepository _roleAccessRepository;

        public ScreenAccessHandler(IRoleAccessRepository roleAccessRepository)
        {
            _roleAccessRepository = roleAccessRepository;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            ScreenAccessRequirement requirement)
        {
            // Get username from claims
            var username = context.User.FindFirst(ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(username))
            {
                context.Fail();
                return;
            }

            try
            {
                // Get user access from your existing repository
                var userAccess = await _roleAccessRepository.GetUserAccessAsync(null, username);

                // Check if user has required permission for the screen
                var hasAccess = userAccess.Any(access =>
                    access.A_screenCode == requirement.ScreenCode &&
                    HasRequiredPermission(access, requirement.Permission));

                if (hasAccess)
                {
                    context.Succeed(requirement);
                }
                else
                {
                    context.Fail();
                }
            }
            catch
            {
                context.Fail();
            }
        }

        private bool HasRequiredPermission(dynamic access, string permission)
        {
            return permission switch
            {
                "View" => access.A_viewAccess,
                "Create" => access.A_createAccess,
                "Edit" => access.A_editAccess,
                "Delete" => access.A_deleteAccess,
                _ => false
            };
        }
    }
}
