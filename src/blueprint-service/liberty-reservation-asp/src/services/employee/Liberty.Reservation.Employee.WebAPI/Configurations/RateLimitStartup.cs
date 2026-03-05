using System.Security.Claims;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

namespace Liberty.Reservation.Employee.WebAPI.Configurations;

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
                        limiterOptions.PermitLimit = 1000;
                        limiterOptions.QueueLimit = 2000;
                        limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    }
                );

                options.AddPolicy(
                    PerUserRateLimitPolicy,
                    context =>
                    {
                        var logger = context.RequestServices.GetRequiredService<ILogger<Startup.Startup>>();

                        // We always have a user code
                        var id = context.User.FindFirstValue("code") ?? "unknown";
                        logger.LogInformation("Rate limit for user: {UserKey}", id);

                        return RateLimitPartition.GetTokenBucketLimiter(
                            id,
                            _ =>
                                new TokenBucketRateLimiterOptions
                                {
                                    ReplenishmentPeriod = TimeSpan.FromSeconds(5),
                                    AutoReplenishment = true,
                                    TokenLimit = 100,
                                    TokensPerPeriod = 50,
                                    QueueLimit = 10,
                                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst
                                }
                        );
                    }
                );
            }
        );
    }

    public static IEndpointConventionBuilder RequirePerUserRateLimit(
        this IEndpointConventionBuilder builder
    )
    {
        return builder
            .RequireRateLimiting(GlobalConcurrencyLimitPolicy)
            .RequireRateLimiting(PerUserRateLimitPolicy);
    }
}
