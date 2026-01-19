using System.Security.Claims;

namespace BlueprintCqrs.Test.Configuration;

public class MockClaimsPrincipalProvider(
    ClaimsPrincipal user
)
{
    public ClaimsPrincipal User { get; } = user;
}
