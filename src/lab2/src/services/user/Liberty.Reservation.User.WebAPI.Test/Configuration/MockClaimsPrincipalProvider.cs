using System.Security.Claims;

namespace Liberty.Reservation.User.WebAPI.Test.Configuration;

public class MockClaimsPrincipalProvider(
    ClaimsPrincipal user
)
{
    public ClaimsPrincipal User { get; } = user;
}
