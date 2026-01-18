using Liberty.Reservation.Application.Behaviors;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Liberty.Reservation.Employee.Application.Contexts;

public class EmployeeDataContextFactory(
    IServiceProvider serviceProvider
) : IDbContextFactory<EmployeeDataContext>
{
    public EmployeeDataContext CreateDbContext()
    {
        using var scope = serviceProvider.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<EmployeeDataContext>>();

        DbContextTypeHolder.UseMasterDb.Value = true;
        var useMasterNode = DbContextTypeHolder.UseMasterDb.Value;

        logger.LogInformation(
            "Factory creating {DbContextType} for database operations.",
            useMasterNode ? nameof(EmployeeWriteDataContext) : nameof(EmployeeReadDataContext)
        );

        if (useMasterNode)
        {
            var options = scope.ServiceProvider.GetRequiredService<DbContextOptions<EmployeeWriteDataContext>>();
            return new EmployeeWriteDataContext(options);
        }
        else
        {
            var options = scope.ServiceProvider.GetRequiredService<DbContextOptions<EmployeeReadDataContext>>();
            return new EmployeeReadDataContext(options);
        }
    }
}
