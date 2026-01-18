using Liberty.Reservation.Employee.WebAPI.Application.Settings;
using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;
using Serilog;

namespace Liberty.Reservation.Employee.WebAPI.Configurations;

public static class HttpClientStartup
{
    public static IHttpClientBuilder AddCustomHttpClient(
        this IServiceCollection services,
        string clientName,
        string clientUrl,
        HttpClientPolicySetting options
    )
    {
        var builder = services.AddHttpClient(
            clientName,
            (
                sp,
                client
            ) =>
            {
                client.BaseAddress = new Uri(clientUrl);
            }
        );

        builder.AddHttpClientResiliency(options);

        return builder;
    }

    public static IHttpClientBuilder AddHttpClientResiliency(
        this IHttpClientBuilder builder,
        HttpClientPolicySetting options
    )
    {
        // return !options.Enabled ? builder : builder.AddPolicyHandler(GetPolicies(options));

        return builder;
    }

    private static IAsyncPolicy<HttpResponseMessage> GetPolicies(
        HttpClientPolicySetting options
    )
    {
        var retryPolicy = HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
            .WaitAndRetryAsync(
                options.Retry,
                retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                    + TimeSpan.FromMilliseconds(new Random().Next(0, 1000)),
                (
                    ex,
                    retryCnt
                ) =>
                {
                    Log.Logger.Information("HttpClient_WaitAndRetryAsync: Retry count {RetryCount}", retryCnt);
                }
            );

        var timeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(
            options.Timeout,
            TimeoutStrategy.Pessimistic,
            (
                ctx,
                span,
                abandonedTask
            ) =>
            {
                Log.Logger.Information("HttpClient_TimeoutAsync: timeout in {Timeout}", span.TotalSeconds);
                return Task.FromResult(true);
            }
        );

        return Policy.WrapAsync(retryPolicy, timeoutPolicy);
    }
}
