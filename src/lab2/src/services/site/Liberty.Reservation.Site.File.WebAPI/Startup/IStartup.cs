namespace Liberty.Reservation.Site.File.WebAPI.Startup;

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
}
