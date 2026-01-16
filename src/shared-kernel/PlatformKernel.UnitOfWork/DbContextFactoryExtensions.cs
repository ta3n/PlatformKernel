using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace PlatformKernel.UnitOfWork;

/// <summary>
/// Provides extension methods for adding and configuring DbContext factory forwarding in an application's dependency injection container.
/// </summary>
public static class DbContextFactoryExtensions
{
    /// Registers a forwarding DbContext factory that forwards calls to an underlying DbContext factory implementation.
    /// This allows you to utilize the implementation's factory and still work with its interface.
    /// <typeparam name="TContext">The base DbContext type to register the factory for.</typeparam>
    /// <typeparam name="TImplementation">The actual DbContext implementation type to forward calls to.</typeparam>
    /// <param name="services">The IServiceCollection to add the forwarding factory to.</param>
    /// <returns>The IServiceCollection with the forwarding DbContext factory registered.</returns>
    public static IServiceCollection AddForwardingDbContextFactory<TContext, TImplementation>(
        this IServiceCollection services
    )
        where TContext : DbContext
        where TImplementation : DbContext, TContext
    {
        services.AddSingleton<IDbContextFactory<TContext>>(
            sp =>
                new ForwardingDbContextFactory<TContext, TImplementation>(
                    sp.GetRequiredService<IDbContextFactory<TImplementation>>()
                )
        );

        return services;
    }
}

/// <summary>
/// A forwarding implementation of <see cref="IDbContextFactory{TContext}"/> that delegates the creation of
/// DbContext instances to another <see cref="IDbContextFactory{TImplementation}"/>.
/// </summary>
/// <typeparam name="TContext">The base type of the DbContext.</typeparam>
/// <typeparam name="TImplementation">The concrete implementation of <typeparamref name="TContext"/> that is being forwarded to.</typeparam>
public class ForwardingDbContextFactory<TContext, TImplementation>(
    IDbContextFactory<TImplementation> factory
) : IDbContextFactory<TContext>
    where TContext : DbContext
    where TImplementation : DbContext, TContext
{
    /// Creates a database context instance of the specified type.
    /// <returns>A new instance of the specified database context type.</returns>
    public TContext CreateDbContext()
    {
        return factory.CreateDbContext();
    }

    /// <summary>
    /// Creates a new instance of the DbContext asynchronously using the underlying factory.
    /// </summary>
    /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous creation of the new DbContext instance. The task result contains the created DbContext instance.</returns>
    public async Task<TContext> CreateDbContextAsync(
        CancellationToken cancellationToken = default
    )
    {
        return await factory.CreateDbContextAsync(cancellationToken);
    }
}
