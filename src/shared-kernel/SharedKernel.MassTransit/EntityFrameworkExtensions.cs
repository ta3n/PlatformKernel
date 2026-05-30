using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace SharedKernel.MassTransit;

/// <summary>
/// Provides helper extensions for configuring MassTransit Entity Framework outbox and saga persistence.
/// </summary>
public static class EntityFrameworkExtensions
{
    /// <summary>
    /// Registers the Entity Framework transactional outbox for PostgreSQL using the specified <typeparamref name="TDbContext"/>.
    /// </summary>
    public static IBusRegistrationConfigurator AddEntityFrameworkOutboxCustom<TDbContext>(
        this IBusRegistrationConfigurator configurator,
        bool useBusOutbox = true,
        Action<IEntityFrameworkOutboxConfigurator>? configure = null,
        Action<IEntityFrameworkBusOutboxConfigurator>? busOutboxConfigure = null
    )
        where TDbContext : DbContext
    {
        ArgumentNullException.ThrowIfNull(configurator);

        configurator.AddEntityFrameworkOutbox<TDbContext>(
            outboxConfigurator =>
            {
                PostgresEntityFrameworkConfigurator.ConfigureOutbox(outboxConfigurator);

                if (useBusOutbox)
                {
                    outboxConfigurator.UseBusOutbox(busOutboxConfigure);
                }

                configure?.Invoke(outboxConfigurator);
            }
        );

        return configurator;
    }

    /// <summary>
    /// Applies Entity Framework outbox middleware to all receive endpoints configured via <c>ConfigureEndpoints</c>.
    /// </summary>
    public static IBusRegistrationConfigurator UseEntityFrameworkOutboxForConfiguredEndpoints<TDbContext>(
        this IBusRegistrationConfigurator configurator,
        Action<IOutboxOptionsConfigurator>? configure = null
    )
        where TDbContext : DbContext
    {
        ArgumentNullException.ThrowIfNull(configurator);

        configurator.AddConfigureEndpointsCallback(
            (
                context,
                _,
                endpointConfigurator
            ) =>
            {
                endpointConfigurator.UseEntityFrameworkOutbox<TDbContext>(context, configure);
            }
        );

        return configurator;
    }

    /// <summary>
    /// Applies Entity Framework outbox middleware to a manually configured receive endpoint.
    /// </summary>
    public static void UseEntityFrameworkOutboxCustom<TDbContext>(
        this IReceiveEndpointConfigurator configurator,
        IRegistrationContext context,
        Action<IOutboxOptionsConfigurator>? configure = null
    )
        where TDbContext : DbContext
    {
        ArgumentNullException.ThrowIfNull(configurator);
        ArgumentNullException.ThrowIfNull(context);

        configurator.UseEntityFrameworkOutbox<TDbContext>(context, configure);
    }

    /// <summary>
    /// Registers a saga state machine backed by a PostgreSQL Entity Framework saga repository using an existing <typeparamref name="TDbContext"/>.
    /// </summary>
    public static ISagaRegistrationConfigurator<TSaga> AddSagaStateMachineWithEntityFrameworkRepository<TStateMachine,
        TSaga, TDbContext>(
        this IBusRegistrationConfigurator configurator,
        ConcurrencyMode concurrencyMode = ConcurrencyMode.Pessimistic,
        Action<ISagaConfigurator<TSaga>>? configureSaga = null,
        Action<IEntityFrameworkSagaRepositoryConfigurator<TSaga>>? configureRepository = null
    )
        where TStateMachine : class, SagaStateMachine<TSaga>
        where TSaga : class, SagaStateMachineInstance
        where TDbContext : DbContext
    {
        ArgumentNullException.ThrowIfNull(configurator);

        var sagaConfigurator = configurator.AddSagaStateMachine<TStateMachine, TSaga>(configureSaga);

        sagaConfigurator.EntityFrameworkRepository(
            repositoryConfigurator =>
            {
                repositoryConfigurator.ConcurrencyMode = concurrencyMode;
                repositoryConfigurator.ExistingDbContext<TDbContext>();

                PostgresEntityFrameworkConfigurator.ConfigureSagaRepository(repositoryConfigurator);
                configureRepository?.Invoke(repositoryConfigurator);
            }
        );

        return sagaConfigurator;
    }

    /// <summary>
    /// Registers a saga state machine definition backed by a PostgreSQL Entity Framework saga repository using an existing <typeparamref name="TDbContext"/>.
    /// </summary>
    public static ISagaRegistrationConfigurator<TSaga> AddSagaStateMachineWithEntityFrameworkRepository<TStateMachine,
        TSaga, TDefinition,
        TDbContext>(
        this IBusRegistrationConfigurator configurator,
        ConcurrencyMode concurrencyMode = ConcurrencyMode.Pessimistic,
        Action<ISagaConfigurator<TSaga>>? configureSaga = null,
        Action<IEntityFrameworkSagaRepositoryConfigurator<TSaga>>? configureRepository = null
    )
        where TStateMachine : class, SagaStateMachine<TSaga>
        where TSaga : class, SagaStateMachineInstance
        where TDefinition : class, ISagaDefinition<TSaga>
        where TDbContext : DbContext
    {
        ArgumentNullException.ThrowIfNull(configurator);

        var sagaConfigurator = configurator.AddSagaStateMachine<TStateMachine, TSaga, TDefinition>(configureSaga);

        sagaConfigurator.EntityFrameworkRepository(
            repositoryConfigurator =>
            {
                repositoryConfigurator.ConcurrencyMode = concurrencyMode;
                repositoryConfigurator.ExistingDbContext<TDbContext>();

                PostgresEntityFrameworkConfigurator.ConfigureSagaRepository(repositoryConfigurator);
                configureRepository?.Invoke(repositoryConfigurator);
            }
        );

        return sagaConfigurator;
    }
}
