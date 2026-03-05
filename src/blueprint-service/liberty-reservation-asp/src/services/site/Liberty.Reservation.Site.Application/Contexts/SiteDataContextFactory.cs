using Liberty.Reservation.Application.Behaviors;
using Microsoft.Extensions.DependencyInjection;

namespace Liberty.Reservation.Site.Application.Contexts;

public class SiteDataContextFactory(
    IServiceProvider serviceProvider
) : IDbContextFactory<SiteDataContext>
{
    public SiteDataContext CreateDbContext()
    {
        using var scope = serviceProvider.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<SiteDataContext>>();

        DbContextTypeHolder.UseMasterDb.Value = true;
        var useMasterNode = DbContextTypeHolder.UseMasterDb.Value;

        logger.LogInformation(
            "Factory creating {DbContextType} for database operations.",
            useMasterNode ? nameof(SiteWriteDataContext) : nameof(SiteReadDataContext)
        );

        if (useMasterNode)
        {
            var options = scope.ServiceProvider.GetRequiredService<DbContextOptions<SiteWriteDataContext>>();
            return new SiteWriteDataContext(options);
        }
        else
        {
            var options = scope.ServiceProvider.GetRequiredService<DbContextOptions<SiteReadDataContext>>();
            return new SiteReadDataContext(options);
        }
    }
}
