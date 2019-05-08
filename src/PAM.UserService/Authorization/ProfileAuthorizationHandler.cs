using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using PAM.UserService.Model;

namespace PAM.UserService.Authorization
{
    public class ProfileAuthorizationHandler : AuthorizationHandler<ProfileRequirement, User>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ProfileRequirement requirement, User resource)
        {
            if (context.User.Identity.Name == resource.Email)
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}
