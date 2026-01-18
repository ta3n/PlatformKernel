using Liberty.Reservation.Employee.Application.Contexts.DataContexts;
using Microsoft.EntityFrameworkCore;

namespace Liberty.Reservation.Employee.Application.Test.Mocks.DataContexts;

public class DataContextMock
{
    public static async Task<DataContext> Create(
        DbContextOptions<DataContext> options
    )
    {
        var context = new DataContext(options);

        await context.SaveChangesAsync();

        return context;
    }
}
