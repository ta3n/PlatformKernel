using Liberty.Reservation.Application.Contexts.DataContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Liberty.Reservation.DbMigration.Contexts;

public class MigrationDataContext : DbContext
{
    protected override void OnModelCreating(
        ModelBuilder modelBuilder
    )
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ReservationDataContext).Assembly);
    }

    protected override void OnConfiguring(
        DbContextOptionsBuilder optionsBuilder
    )
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddEnvironmentVariables()
            .Build();

        var migrationsAssembly = configuration.GetConnectionString("MigrationsAssembly");
        var connectionString = configuration.GetConnectionString("DataContextConnection");

        optionsBuilder
            .UseNpgsql(
                connectionString,
                sqlOptions =>
                {
                    sqlOptions.MigrationsAssembly(migrationsAssembly);
                    sqlOptions.EnableRetryOnFailure(
                        5,
                        TimeSpan.FromSeconds(30),
                        null
                    );
                }
            )
            .UseSnakeCaseNamingConvention()
            .LogTo(Console.WriteLine, LogLevel.Information)
            .EnableDetailedErrors();

        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
        {
            optionsBuilder.EnableSensitiveDataLogging();
        }
    }
}
