using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Application.Models;

namespace Liberty.Reservation.User.WebAPI.Test.InfrastructureOfTest;

public class MockBookingHoldManagementService : IBookingHoldManagementService
{
    public Task<bool> TryHoldRoomAsync(
        BookingHoldCheckModel model,
        CancellationToken cancellationToken = default
    )
    {
        return Task.FromResult(true);
    }

    public Task ReleaseHoldAsync(
        BookingHoldCheckModel model,
        CancellationToken cancellationToken = default
    )
    {
        return Task.CompletedTask;
    }

    public Task<Dictionary<long, int>> GetAvailabilityForUserAsync(
        BookingHoldCheckModel model,
        CancellationToken cancellationToken = default
    )
    {
        var availability = new Dictionary<long, int>
        {
            { 1, 5 }, // Example: Room ID 1 has 5 available rooms
            { 2, 3 } // Example: Room ID 2 has 3 available rooms
        };
        return Task.FromResult(availability);
    }

    public Task<Dictionary<long, int>> CheckAvailabilityForGuestConfirmAsync(
        BookingHoldCheckModel model,
        CancellationToken cancellationToken = default
    )
    {
        var availability = new Dictionary<long, int>
        {
            { 1, 5 }, // Example: Room ID 1 has 5 available rooms
            { 2, 3 } // Example: Room ID 2 has 3 available rooms
        };
        return Task.FromResult(availability);
    }
}
