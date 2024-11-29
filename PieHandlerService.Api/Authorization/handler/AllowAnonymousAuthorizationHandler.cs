using Microsoft.AspNetCore.Authorization;

namespace PieHandlerService.Api.Authorization.Handler;

/// <summary>
/// Allow anonymous authorization handler - for development when no authorization is wanted
/// </summary>
internal sealed class AllowAnonymousAuthorizationHandler : IAuthorizationHandler
{
    public Task HandleAsync(AuthorizationHandlerContext context)
    {
        foreach (IAuthorizationRequirement requirement in context.PendingRequirements)
        {
            context.Succeed(requirement);
        }
        return Task.CompletedTask;
    }
}
