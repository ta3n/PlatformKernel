using Grpc.Core;
using Liberty.SysException;
using Liberty.SysException.Exceptions;
using Microsoft.Extensions.Logging;

namespace Liberty.Reservation.Site.WebAPI.Application.Boundaries.Grpc.Base;

/// <summary>
/// Provides a utility for handling errors in gRPC calls by logging exceptions and adding response trailers.
/// </summary>
public static class GrpcErrorHandler
{
    /// <summary>
    /// Executes a gRPC action with error handling, logging, and response trailer population.
    /// </summary>
    /// <typeparam name="T">The type of the result returned by the action.</typeparam>
    /// <param name="logger">The logger used to log errors.</param>
    /// <param name="context">The gRPC server call context.</param>
    /// <param name="action">The asynchronous action to execute.</param>
    /// <returns>A task representing the result of the action.</returns>
    /// <exception cref="RpcException">Thrown when an internal server error occurs.</exception>
    public static async Task<T> ExecuteAsync<T>(
        ILogger logger,
        ServerCallContext context,
        Func<Task<T>> action
    )
    {
        try
        {
            // Execute the provided action and return its result.
            return await action();
        }
        catch (Exception ex)
        {
            // Log the exception with the method name.
            logger.LogError(ex, "Error occurred while processing {MethodName}.", context.Method);

            // Map the exception to an application-specific exception.
            var appEx = AppException.GetAppException(ex);

            // Add error details to the gRPC response trailers.
            context.ResponseTrailers.Add(
                ErrorConstant.Code,
                appEx.ErrorCode.ToString()
            );
            context.ResponseTrailers.Add(
                ErrorConstant.Field,
                appEx.ErrorField ?? string.Empty
            );
            context.ResponseTrailers.Add(
                ErrorConstant.Title,
                appEx is AppUnknownErrorException u ? u.Exception.Message : appEx.Title
            );
            context.ResponseTrailers.Add(
                ErrorConstant.Description,
                appEx is AppUnknownErrorException ? string.Empty : appEx.Message
            );
            context.ResponseTrailers.Add(
                ErrorConstant.StatusCode,
                appEx switch
                {
                    AppInvalidException or ModelException => $"{StatusCodes.Status400BadRequest}",
                    AppUnknownErrorException when ex is ValidationException => $"{StatusCodes.Status400BadRequest}",
                    AppNotfoundException => $"{StatusCodes.Status404NotFound}",
                    _ => $"{StatusCodes.Status500InternalServerError}"
                }
            );

            // Throw an RpcException to indicate an internal server error.
            throw new RpcException(new Status(StatusCode.Internal, "Internal server error"));
        }
    }
}
