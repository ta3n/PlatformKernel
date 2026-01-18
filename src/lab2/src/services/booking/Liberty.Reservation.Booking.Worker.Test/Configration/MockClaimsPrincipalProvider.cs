using System.Security.Claims;

namespace Liberty.Reservation.Booking.Worker.Test.Configration;

public class MockClaimsPrincipalProvider(
    ClaimsPrincipal user
)
{
    public ClaimsPrincipal User { get; } = user;
}
