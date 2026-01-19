using Microsoft.AspNetCore.Authorization;
using BlueprintCqrs.Crosscutting.Constants;

namespace BlueprintCqrs.Security;

public static class PoliciesConstants
{
    public static readonly AuthorizationPolicy PolicyAdmin = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .RequireRole(RolesConstants.Admin)
        .Build();

    public static readonly AuthorizationPolicy PolicyUser = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .RequireRole(RolesConstants.User)
        .Build();
}
