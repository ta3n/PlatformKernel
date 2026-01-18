using Liberty.ApplicationShared.Domains.Repositories;
using Liberty.ApplicationShared.Extensions;
using Liberty.Reservation.Employee.Application;
using Liberty.Reservation.Employee.Application.Contexts;
using Liberty.Reservation.Employee.Application.Domains.Repositories;
using Liberty.Reservation.Employee.Application.Domains.Repositories.Interfaces;
using Liberty.UnitOfWork;
using Liberty.UnitOfWork.Abstractions;
using Liberty.UnitOfWork.Implementations;
using Microsoft.EntityFrameworkCore;

namespace Liberty.Reservation.Employee.WebAPI.Configurations;

public static class DbContextStartup
{
    public static IServiceCollection AddDbContext(
        this IServiceCollection services,
        IConfiguration config
    )
    {
        services.Configure<ConnectionPoolOptions>(config.GetSection("ConnectionPool"));
        var connectionPoolOptions = config.GetOptionsExt<ConnectionPoolOptions>("ConnectionPool");
        var dbConnectionManager = new DbConnectionManager(connectionPoolOptions);
        services.AddSingleton<IDbConnectionManager>(_ => dbConnectionManager);

        var inMemoryDb = config.GetConnectionString("InMemoryDatabase");
        var migrationsAssembly = config.GetConnectionString("MigrationsAssembly");
        var connectionString = config.GetConnectionString("DataContextConnection");

        services.AddDbContext<EmployeeDataContext>(
            options =>
            {
                if (!string.IsNullOrEmpty(inMemoryDb))
                {
                    // options.UseInMemoryDatabase(inMemoryDb);
                }

                options.UseMySql(
                        connectionString,
                        new MySqlServerVersion(new Version(8, 0, 29)),
                        sqlOptions =>
                        {
                            sqlOptions.MigrationsAssembly(migrationsAssembly);
                            sqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(30), null);
                        }
                    )
                    .LogTo(
                        Console.WriteLine,
                        new[]
                        {
                            DbLoggerCategory.Database.Command.Name
                        },
                        LogLevel.Information
                    )
                    .EnableSensitiveDataLogging()
                    .EnableDetailedErrors()
                    .AddInterceptors(new CustomDbConnectionInterceptor(dbConnectionManager));
            }
        );

        services.AddScoped<DbContext>(
            provider => provider.GetRequiredService<EmployeeDataContext>()
        );

        services.AddTransient<IUnitOfWork, EmployeeUnitOfWork>();
        return services;
    }

    public static IApplicationBuilder UseApplicationDatabase(
        this IApplicationBuilder app,
        IHostEnvironment environment
    )
    {
        if (!environment.IsDevelopment())
        {
            return app;
        }

        using var scope = app.ApplicationServices.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<EmployeeDataContext>();
        context.Database.OpenConnection();
        context.Database.EnsureCreated();

        return app;
    }

    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration config
    )
    {
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped(typeof(IRepositoryBase<>), typeof(RepositoryBase<>));

        services.AddTransient<IEmployeeRepository, EmployeeRepository>();
        services.AddTransient<IEmployeeMetaRepository, EmployeeMetaRepository>();
        services.AddTransient<IAddressRepository, AddressRepository>();
        services.AddTransient<IFileRepository, FileRepository>();
        services.AddTransient<IFileEmployeeRepository, FileEmployeeRepository>();

        return services;
    }
}
