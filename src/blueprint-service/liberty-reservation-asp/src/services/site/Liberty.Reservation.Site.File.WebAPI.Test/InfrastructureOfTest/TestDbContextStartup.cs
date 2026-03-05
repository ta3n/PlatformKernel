using Liberty.Reservation.Site.File.WebAPI.Application.ExternalServices.Membership.Facility;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Liberty.Reservation.Site.File.WebAPI.Test.InfrastructureOfTest;

public static class TestDbContextStartup
{
    public static IApplicationBuilder UseMembershipDatabase(
        this IApplicationBuilder app,
        IHostEnvironment environment,
        bool isDbMigrationEnabled = false
    )
    {
        if (!isDbMigrationEnabled)
        {
            return app;
        }

        using var scope = app.ApplicationServices.CreateScope();
        var membershipContext = scope.ServiceProvider.GetRequiredService<MembershipFacilityExternalDbContext>();

        membershipContext.Database.OpenConnection();
        membershipContext.Database.EnsureCreated();

        return app;
    }
}
