using System.Threading.RateLimiting;
using Liberty.Reservation.Site.Application.Auth;
using Liberty.Reservation.Site.WebAPI.Application.Web.Extensions;
using Microsoft.AspNetCore.RateLimiting;

namespace Liberty.Reservation.Site.WebAPI.Configurations;

public static class RateLimitStartup
{
    private const string GlobalConcurrencyLimitPolicy = "GlobalConcurrencyLimit";
    private const string PerUserRateLimitPolicy = "PerUserRateLimit";

    public static IServiceCollection AddRateLimiting(
        this IServiceCollection services
    )
    {
        return services.AddRateLimiter(
            options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                options.AddConcurrencyLimiter(
                    GlobalConcurrencyLimitPolicy,
                    limiterOptions =>
                    {
                        limiterOptions.PermitLimit = 1500;
                        limiterOptions.QueueLimit = 2000;
                        limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    }
                );

                options.AddPolicy(
                    PerUserRateLimitPolicy,
                    context =>
                    {
                        var logger = context.RequestServices.GetRequiredService<ILogger<Startup.Startup>>();

                        var securityContextAccessor = context.RequestServices.GetRequiredService<ISecurityContextAccessor>();
                        var userKey = securityContextAccessor.GetApplicationUserKey();
                        var ip = context.GetClientIp();
                        var key = userKey ?? ip;
                        var isJwt = !string.IsNullOrEmpty(userKey);

                        logger.LogInformation("Rate limit for user: {UserKey}", userKey);
                        logger.LogInformation("Rate limit for IP: {Ip}", ip);

                        return RateLimitPartition.GetTokenBucketLimiter(
                            key,
                            _ => new TokenBucketRateLimiterOptions
                            {
                                ReplenishmentPeriod = TimeSpan.FromSeconds(3),
                                AutoReplenishment = true,
                                TokenLimit = isJwt ? 100 : 250,
                                TokensPerPeriod = isJwt ? 50 : 100,
                                QueueLimit = 10,
                                QueueProcessingOrder = QueueProcessingOrder.OldestFirst
                            }
                        );
                    }
                );
            }
        );
    }

    public static IEndpointConventionBuilder RequirePerIpRateLimit(
        this IEndpointConventionBuilder builder
    )
    {
        return builder
            .RequireRateLimiting(GlobalConcurrencyLimitPolicy)
            .RequireRateLimiting(PerUserRateLimitPolicy);
    }
}
