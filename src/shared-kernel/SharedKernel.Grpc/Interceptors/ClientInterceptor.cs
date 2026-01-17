using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SharedKernel.Grpc.Options;

namespace SharedKernel.Grpc.Interceptors;

public class ClientInterceptor(
    IConfiguration config,
    ILogger<ClientInterceptor> logger
) : Interceptor
{
    private readonly IConfiguration _config = config ?? throw new ArgumentNullException(nameof(config));
    private readonly ILogger<ClientInterceptor> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(
        TRequest request,
        ClientInterceptorContext<TRequest, TResponse> context,
        AsyncUnaryCallContinuation<TRequest, TResponse> continuation
    )
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(continuation);

        var grpcClientPolicyOptions = _config.GetSection(GrpcClientPolicyOptions.Name).Get<GrpcClientPolicyOptions>();
        ArgumentNullException.ThrowIfNull(grpcClientPolicyOptions);

        _logger.LogTrace(
            "{ServiceName}-{MethodName}-GrpcTimeout: {GrpcTimeout}s",
            context.Method.ServiceName,
            context.Method.Name,
            grpcClientPolicyOptions.Timeout
        );

        var headers = context.Options.Headers ?? [];
        var options = context.Options.WithHeaders(headers);

        // add timeout for gRPC request so that it will not starve the resource of the system
        options = options.WithDeadline(DateTime.UtcNow + TimeSpan.FromSeconds(grpcClientPolicyOptions.Timeout));

        context = new ClientInterceptorContext<TRequest, TResponse>(context.Method, context.Host, options);

        return continuation(request, context);
    }
}
