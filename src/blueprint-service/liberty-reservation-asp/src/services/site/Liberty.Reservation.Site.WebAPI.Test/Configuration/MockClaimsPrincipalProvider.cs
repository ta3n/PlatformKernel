using System.Security.Claims;

namespace Liberty.Reservation.Site.WebAPI.Test.Configuration;

public class MockClaimsPrincipalProvider(
    ClaimsPrincipal user
)
{
    public ClaimsPrincipal User { get; } = user;
}
