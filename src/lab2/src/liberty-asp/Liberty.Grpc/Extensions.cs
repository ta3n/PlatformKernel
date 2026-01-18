using System.Net;
using Grpc.Core;
using Liberty.Grpc.Interceptors;
using Liberty.Grpc.Options;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Timeout;
using Serilog;

namespace Liberty.Grpc;

public static class Extensions
{
    public static IServiceCollection AddCustomGrpcClient<TGrpcClient>(
        this IServiceCollection services,
        string endpoint,
        GrpcClientPolicyOptions options
    ) where TGrpcClient : ClientBase
    {
        services.AddSingleton<ClientInterceptor>();

        var builder = services
            .AddGrpcClient<TGrpcClient>(
                o =>
                {
                    o.Address = new Uri(endpoint);
                    o.ChannelOptionsActions.Add(
                        channelOptions =>
                        {
                            channelOptions.DisposeHttpClient = true;
                            channelOptions.ThrowOperationCanceledOnCancellation = true;
                        }
                    );
                }
            )
            .AddInterceptor<ClientInterceptor>();

        if (options.Enabled)
        {
            builder.AddPolicyHandler(GetPolicies(options));
        }

        return services;
    }

    public static IServiceCollection AddCustomGrpcServer(
        this IServiceCollection services
    )
    {
        services.AddSingleton<ServerInterceptor>();

        services.AddGrpc(
            options =>
            {
                options.Interceptors.Add<ServerInterceptor>();

                options.EnableDetailedErrors = true;

                options.MaxSendMessageSize = 100 * 1024 * 1024; // 100 MB
                options.MaxReceiveMessageSize = 100 * 1024 * 1024;
            }
        );

        services.AddGrpcReflection();

        return services;
    }

    private static Polly.Wrap.AsyncPolicyWrap<HttpResponseMessage> GetPolicies(
        GrpcClientPolicyOptions options
    )
    {
        var retryPolicy = Policy
            .HandleResult<HttpResponseMessage>(IsExecuteHttpResponseMessage)
            .WaitAndRetryAsync(
                options.Retry,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                (
                    _,
                    retryCnt
                ) =>
                {
                    Log.Logger.Information("GrpcWaitAndRetryAsync: Retry count {RetryCount}", retryCnt);
                }
            );

        var breakerPolicy = Policy
            .HandleResult<HttpResponseMessage>(IsExecuteHttpResponseMessage)
            .CircuitBreakerAsync(
                options.ExceptionsAllowedBeforeBreaking,
                TimeSpan.FromSeconds(options.BreakDuration),
                OnBreak,
                OnReset,
                OnHalfOpen
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
                Log.Logger.Information("GrpcTimeoutAsync: timeout in {Timeout}", span.TotalSeconds);
                return Task.FromResult(true);
            }
        );

        return Policy.WrapAsync(
            retryPolicy,
            breakerPolicy,
            timeoutPolicy
        );
    }

    private static bool IsExecuteHttpResponseMessage(
        HttpResponseMessage msg
    )
    {
        var serverErrors = new[]
        {
            HttpStatusCode.BadGateway,
            HttpStatusCode.GatewayTimeout,
            HttpStatusCode.ServiceUnavailable,
            HttpStatusCode.InternalServerError,
            HttpStatusCode.TooManyRequests,
            HttpStatusCode.RequestTimeout
        };

        var gRpcErrors = new[]
        {
            StatusCode.DeadlineExceeded,
            StatusCode.Internal,
            StatusCode.NotFound,
            StatusCode.ResourceExhausted,
            StatusCode.Unavailable,
            StatusCode.Unknown
        };

        var grpcStatus = GetStatusCode(msg);
        var httpStatusCode = msg.StatusCode;

        return (grpcStatus == null && serverErrors.Contains(httpStatusCode)) // if the server sends an error before gRPC pipeline
            || (
                httpStatusCode == HttpStatusCode.OK
                && grpcStatus != null
                && gRpcErrors.Contains(
                    grpcStatus.Value
                )
            ); // if gRPC pipeline handled the request (gRPC always answers OK)
    }

    private static StatusCode? GetStatusCode(
        HttpResponseMessage response
    )
    {
        var headers = response.Headers;

        if (!headers.Contains("grpc-status") && response.StatusCode == HttpStatusCode.OK)
        {
            return StatusCode.OK;
        }

        if (headers.Contains("grpc-status"))
        {
            return (StatusCode)int.Parse(headers.GetValues("grpc-status").First());
        }

        return null;
    }

    private static void OnHalfOpen()
    {
        Log.Logger.Information("GrpcCircuitBreakerAsync: OnHalfOpen");
    }

    private static void OnReset(
        Context context
    )
    {
        Log.Logger.Information("GrpcCircuitBreakerAsync: OnReset");
    }

    private static void OnBreak(
        DelegateResult<HttpResponseMessage> delegateResult,
        TimeSpan timeSpan,
        Context context
    )
    {
        Log.Logger.Information("GrpcCircuitBreakerAsync: OnBreak");
    }
}
