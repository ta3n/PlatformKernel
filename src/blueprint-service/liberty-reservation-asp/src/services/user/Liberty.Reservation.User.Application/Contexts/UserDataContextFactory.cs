using Liberty.Reservation.Application.Behaviors;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Liberty.Reservation.User.Application.Contexts;

public class UserDataContextFactory(
    IServiceProvider serviceProvider
) : IDbContextFactory<UserDataContext>
{
    public UserDataContext CreateDbContext()
    {
        using var scope = serviceProvider.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<UserDataContext>>();

        DbContextTypeHolder.UseMasterDb.Value = true;
        var useMasterNode = DbContextTypeHolder.UseMasterDb.Value;

        logger.LogInformation(
            "Factory creating {DbContextType} for database operations.",
            useMasterNode ? nameof(UserWriteDataContext) : nameof(UserReadDataContext)
        );

        if (useMasterNode)
        {
            var options = scope.ServiceProvider.GetRequiredService<DbContextOptions<UserWriteDataContext>>();
            return new UserWriteDataContext(options);
        }
        else
        {
            var options = scope.ServiceProvider.GetRequiredService<DbContextOptions<UserReadDataContext>>();
            return new UserReadDataContext(options);
        }
    }
}
