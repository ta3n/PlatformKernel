using MediatR;
using Microsoft.Extensions.Configuration;

namespace Blueprint.Service.Base.Application.Behaviors;

/// <summary>
/// Provides a mechanism to control whether the master database context should be used
/// in database operations. This class is primarily utilized in scenarios where
/// read or write routing for database contexts is required.
/// </summary>
/// <remarks>
/// The <see cref="DbContextTypeHolder"/> contains a single <c>AsyncLocal</c> property,
/// <see cref="UseMasterDb"/>, which determines whether a master database (write context)
/// should be used or not. It is initialized to <c>true</c> by default.
/// Lifecycle management of the <see cref="UseMasterDb"/> value is critical, and appropriate
/// cleanup should be done to reset its state after its usage. Typically, this management
/// is handled within behaviors like <see cref="DbContextRoutingBehavior{TRequest, TResponse}"/>.
/// </remarks>
public static class DbContextTypeHolder
{
    /// <summary>
    /// Indicates whether the application should use the master (write) database context or
    /// the read-only database context.
    /// </summary>
    /// <remarks>
    /// By default, this variable is set to true, meaning the master database context will be
    /// used. It can be dynamically reconfigured at runtime, for example, to switch to the
    /// read database context for query operations. This setting is typically modified within
    /// database request handling and reset after completion to ensure consistency.
    /// </remarks>
    public static readonly AsyncLocal<bool> UseMasterDb = new() { Value = true };
}

/// <summary>
/// Represents a behavior in the MediatR pipeline responsible for routing database
/// context usage based on the type of request being handled.
/// </summary>
/// <typeparam name="TRequest">The type of the request being processed. Must implement <see cref="IRequest{TResponse}"/>.</typeparam>
/// <typeparam name="TResponse">The type of the response returned by the request handler.</typeparam>
public class DbContextRoutingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    /// <summary>
    /// Defines the suffix used to determine if a request is categorized as a query operation.
    /// </summary>
    /// <remarks>
    /// The variable is primarily utilized within the database context routing behavior to identify
    /// query requests by checking if the request type's name ends with this suffix. If the request
    /// qualifies as a query and a dedicated read database connection string is available, a read-only
    /// database context is used. Otherwise, the master database context is employed. This ensures
    /// appropriate resource usage based on the request type.
    /// </remarks>
    private const string QueryRequestSuffix = "Query";

    /// <summary>
    /// Determines if the application should leverage a read-only database connection string
    /// for query operations within the context routing behavior.
    /// </summary>
    /// <remarks>
    /// This variable is initialized based on the presence of a valid read-only database connection
    /// string in the application configuration. If set to true, queries that match the routing
    /// criteria will utilize the read database context. This allows isolation of read and write
    /// operations for improved scalability and performance.
    /// </remarks>
    private readonly bool _useReadConnectionString;

    /// <summary>
    /// Initializes a new instance of the <see cref="DbContextRoutingBehavior{TRequest, TResponse}"/> class.
    /// </summary>
    /// <param name="configuration">The configuration used to determine if a read connection is available.</param>
    public DbContextRoutingBehavior(
        IConfiguration configuration
    )
    {
        var dataReadConnectionString = configuration.GetConnectionString("DataReadContextConnection");
        _useReadConnectionString = !string.IsNullOrEmpty(dataReadConnectionString);
    }

    /// <summary>
    /// Handles the pipeline behavior for routing DbContext usage based on request type.
    /// Determines whether the request is a query type and adjusts the DbContext accordingly.
    /// </summary>
    /// <param name="request">The incoming request object implementing the IRequest interface.</param>
    /// <param name="next">The delegate pointing to the next handler in the pipeline.</param>
    /// <param name="cancellationToken">A cancellation token for async operation cancellation.</param>
    /// <return>The response object of type TResponse after the request has been processed.</return>
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        var isRequestQuery = request.GetType().Name.EndsWith(QueryRequestSuffix, StringComparison.Ordinal);

        DbContextTypeHolder.UseMasterDb.Value = true;

        if (isRequestQuery && _useReadConnectionString)
        {
            DbContextTypeHolder.UseMasterDb.Value = false;
        }

        try
        {
            return await next();
        }
        finally
        {
            DbContextTypeHolder.UseMasterDb.Value = true; // cleanup
        }
    }
}
