using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.Logging;

namespace PlatformKernel.Grpc.Interceptors;

public class ServerLoggerInterceptor(
    ILogger<ServerLoggerInterceptor> logger
) : Interceptor
{
    private readonly ILogger _logger = logger;

    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation
    )
    {
        _logger.LogInformation(
            "Starting receiving call. Type/Method: {Type} / {Method}",
            MethodType.Unary,
            context.Method
        );
        try
        {
            return await continuation(request, context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error thrown by {ContextMethod}.", context.Method);

            throw new RpcException(
                new Status(
                    StatusCode.Internal,
                    $"An error occurred in {nameof(ServerLoggerInterceptor)}.{nameof(UnaryServerHandler)}: {ex.Message}"
                ),
                ex.Message
            );
        }
    }
}
