using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Liberty.Reservation.Booking.Worker.Test.InfrastructureOfTest;

public class TestStartup : Startup.Startup
{
    public override void ConfigureMiddleware(
        IApplicationBuilder app,
        IHostEnvironment environment,
        IConfiguration configuration
    )
    {
        environment.EnvironmentName = Environments.Development;
    }
}
