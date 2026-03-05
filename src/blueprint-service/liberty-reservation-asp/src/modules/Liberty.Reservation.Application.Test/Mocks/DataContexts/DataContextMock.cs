using Liberty.Reservation.Application.Contexts.DataContexts;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Liberty.Reservation.Application.Test.Mocks.DataContexts;

public static class DataContextMock
{
    public static async Task Create(
        DbContextOptions<ReservationDataContext> options,
        IHttpContextAccessor httpContextAccessor
    )
    {
        var context = new ReservationDataContext(options, httpContextAccessor);
        await context.SaveChangesAsync();
    }
}
