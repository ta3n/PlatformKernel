using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.Logging;

namespace PlatformKernel.Grpc.Interceptors;

public class ServerInterceptor(
    ILoggerFactory loggerFactory
) : Interceptor
{
    private readonly ILogger<ServerInterceptor> _logger = loggerFactory.CreateLogger<ServerInterceptor>();

    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation
    )
    {
        ValidateData(request, continuation);

        try
        {
            return await continuation(request, context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error thrown by {ContextMethod}.", context.Method);
            throw new RpcException(new Status(StatusCode.Internal, ex.Message));
        }
    }

    private static void ValidateData<TRequest, TResponse>(
        TRequest request,
        UnaryServerMethod<TRequest, TResponse> continuation
    )
        where TRequest : class
        where TResponse : class
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(continuation);
    }
}
