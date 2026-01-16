using PlatformKernel.Service.Base.Application.Behaviors;
using Microsoft.Extensions.DependencyInjection;

namespace PlatformKernel.Service.Application.Contexts;

public class AppDataContextFactory(
    IServiceProvider serviceProvider
) : IDbContextFactory<AppDataContext>
{
    public AppDataContext CreateDbContext()
    {
        using var scope = serviceProvider.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<AppDataContext>>();

        DbContextTypeHolder.UseMasterDb.Value = true;
        var useMasterNode = DbContextTypeHolder.UseMasterDb.Value;

        logger.LogInformation(
            "Factory creating {DbContextType} for database operations.",
            useMasterNode ? nameof(AppWriteDataContext) : nameof(AppReadDataContext)
        );

        if (useMasterNode)
        {
            var options = scope.ServiceProvider.GetRequiredService<DbContextOptions<AppWriteDataContext>>();
            return new AppWriteDataContext(options);
        }
        else
        {
            var options = scope.ServiceProvider.GetRequiredService<DbContextOptions<AppReadDataContext>>();
            return new AppReadDataContext(options);
        }
    }
}
