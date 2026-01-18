namespace Liberty.Reservation.Site.Public.WebAPI.Startup;

public interface IStartup
{
    void Configure(
        IConfiguration configuration,
        IServiceCollection services
    );

    void ConfigureServices(
        IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment
    );

    void ConfigureMiddleware(
        IApplicationBuilder app,
        IHostEnvironment environment,
        IConfiguration configuration
    );

    void ConfigureEndpoints(
        IApplicationBuilder app,
        IHostEnvironment environment,
        IConfiguration configuration
    );

    void ConfigureGrpcServices(
        WebApplication app,
        IHostEnvironment environment,
        IConfiguration configuration
    );
}
