using Liberty.Reservation.Application.Behaviors;
using Microsoft.Extensions.DependencyInjection;

namespace Liberty.Reservation.Manager.Application.Contexts;

public class ManagerDataContextFactory(
    IServiceProvider serviceProvider
) : IDbContextFactory<ManagerDataContext>
{
    public ManagerDataContext CreateDbContext()
    {
        using var scope = serviceProvider.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<ManagerDataContext>>();

        DbContextTypeHolder.UseMasterDb.Value = true;
        var useMasterNode = DbContextTypeHolder.UseMasterDb.Value;

        logger.LogInformation(
            "Factory creating {DbContextType} for database operations.",
            useMasterNode ? nameof(ManagerWriteDataContext) : nameof(ManagerReadDataContext)
        );

        if (useMasterNode)
        {
            var options = scope.ServiceProvider.GetRequiredService<DbContextOptions<ManagerWriteDataContext>>();
            return new ManagerWriteDataContext(options);
        }
        else
        {
            var options = scope.ServiceProvider.GetRequiredService<DbContextOptions<ManagerReadDataContext>>();
            return new ManagerReadDataContext(options);
        }
    }
}
