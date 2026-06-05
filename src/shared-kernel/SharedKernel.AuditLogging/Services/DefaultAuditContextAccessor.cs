using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using SharedKernel.AuditLogging.Abstractions;
using SharedKernel.AuditLogging.Models;
using SharedKernel.AuditLogging.Options;

namespace SharedKernel.AuditLogging.Services;

public sealed class DefaultAuditContextAccessor(
    IHttpContextAccessor httpContextAccessor,
    IOptions<AuditLoggingOptions> options
) : IAuditContextAccessor
{
    public AuditContextSnapshot GetCurrent()
    {
        var httpContext = httpContextAccessor.HttpContext;
        var user = httpContext?.User;
        var userId = FirstClaimValue(
            user,
            ClaimTypes.NameIdentifier,
            "sub",
            "user_id",
            "userId",
            "code"
        );
        var userName = user?.Identity?.Name
            ?? FirstClaimValue(user, ClaimTypes.Name, "name", "preferred_username", "username");

        userId = NormalizeUserValue(userId);
        userName = NormalizeUserValue(userName);

        return new AuditContextSnapshot
        {
            UserId = userId,
            UserName = userName,
            TenantId = FirstClaimValue(user, "tenant_id", "tenantId"),
            IpAddress = httpContext?.Connection.RemoteIpAddress?.ToString(),
            TraceId = Activity.Current?.TraceId.ToString() ?? httpContext?.TraceIdentifier,
            Source = string.IsNullOrWhiteSpace(options.Value.Source)
                ? "Application"
                : options.Value.Source
        };
    }

    private static string NormalizeUserValue(
        string? value
    )
    {
        return string.IsNullOrWhiteSpace(value) ? "SYSTEM" : value;
    }

    private static string? FirstClaimValue(
        ClaimsPrincipal? user,
        params string[] claimTypes
    )
    {
        if (user is null)
        {
            return null;
        }

        foreach (var claimType in claimTypes)
        {
            var value = user.FindFirst(claimType)?.Value;
            if (!string.IsNullOrWhiteSpace(value))
            {
                return value;
            }
        }

        return null;
    }
}
