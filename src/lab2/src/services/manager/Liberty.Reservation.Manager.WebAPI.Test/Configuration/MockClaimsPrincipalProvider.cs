using System.Security.Claims;

namespace Liberty.Reservation.Manager.WebAPI.Test.Configuration;

public class MockClaimsPrincipalProvider(
    ClaimsPrincipal user
)
{
    public ClaimsPrincipal User { get; } = user;
}
