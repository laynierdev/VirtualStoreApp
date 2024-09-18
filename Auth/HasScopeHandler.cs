using Microsoft.AspNetCore.Authorization;

namespace VirtualStoreApp.Auth;


public class HasScopeHandler : AuthorizationHandler<HasScopeRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HasScopeRequirement requirement)
    {
        var user = context.User;

            if (user==null || !user.Identity.IsAuthenticated)
            {
                return Task.CompletedTask;
            }
        


        var scopeClaim = user.FindFirst(c => c.Type == "scope" && c.Issuer == requirement.Issuer);
        if (scopeClaim == null)
        {
            return Task.CompletedTask;
        }

        var scopes = scopeClaim.Value.Split(' ');
        if (scopes.Any(s => s == requirement.Scope))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
