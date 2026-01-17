using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.Logging;

namespace SharedKernel.Grpc.Interceptors;

public class ClientLoggingInterceptor(
    ILoggerFactory loggerFactory
) : Interceptor
{
    private readonly ILogger _logger = loggerFactory.CreateLogger<ClientLoggingInterceptor>();

    public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(
        TRequest request,
        ClientInterceptorContext<TRequest, TResponse> context,
        AsyncUnaryCallContinuation<TRequest, TResponse> continuation
    )
    {
        _logger.LogInformation(
            "Starting call. Type/Method: {Type} / {Method}",
            context.Method.Type,
            context.Method.Name
        );
        return continuation(request, context);
    }
}
