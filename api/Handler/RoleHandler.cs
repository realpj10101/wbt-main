using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace api.Handler;

public class CoachRoleRequirement : IAuthorizationRequirement { }

public class RoleHandler : AuthorizationHandler<CoachRoleRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CoachRoleRequirement requirement)
    {
        if (!context.User.IsInRole("coach"))
        {
            context.Fail();
            return Task.CompletedTask;
        }
        
        context.Succeed(requirement);
        return Task.CompletedTask;
    }
}