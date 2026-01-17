using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;
using Serilog;

namespace SharedKernel.Grpc.HttpClient;

public static class HttpClientExtension
{
    public static IHttpClientBuilder AddCustomHttpClient(
        this IServiceCollection services,
        string clientName,
        Uri clientUrl,
        HttpClientPolicyOptions options
    )
    {
        var builder = services.AddHttpClient(
            clientName,
            (
                _,
                client
            ) =>
            {
                client.BaseAddress = new Uri(clientUrl.AbsoluteUri);
            }
        );

        builder.AddHttpClientResiliency(options);

        return builder;
    }

    private static IHttpClientBuilder AddHttpClientResiliency(
        this IHttpClientBuilder builder,
        HttpClientPolicyOptions options
    )
    {
        return !options.Enabled ? builder : builder.AddPolicyHandler(GetPolicies(options));
    }

    private static Polly.Wrap.AsyncPolicyWrap<HttpResponseMessage> GetPolicies(
        HttpClientPolicyOptions options
    )
    {
        var retryPolicy = HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
            .WaitAndRetryAsync(
                options.Retry,
                retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)) + TimeSpan.FromMilliseconds(Random.Shared.Next(0, 1000)),
                (
                    _,
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
                _,
                span,
                _
            ) =>
            {
                Log.Logger.Information("HttpClient_TimeoutAsync: timeout in {Timeout}", span.TotalSeconds);
                return Task.FromResult(true);
            }
        );

        return Policy.WrapAsync(retryPolicy, timeoutPolicy);
    }
}
