using MassTransit;

namespace SharedKernel.MassTransit;

/// <summary>
/// Encapsulates PostgreSQL-specific MassTransit Entity Framework configuration so new database providers
/// can be added later without changing the shared registration flow.
/// </summary>
internal static class PostgresEntityFrameworkConfigurator
{
    public static void ConfigureOutbox(
        IEntityFrameworkOutboxConfigurator configurator
    )
    {
        ArgumentNullException.ThrowIfNull(configurator);

        configurator.UsePostgres();
    }

    public static void ConfigureSagaRepository(
        IEntityFrameworkSagaRepositoryConfigurator configurator
    )
    {
        ArgumentNullException.ThrowIfNull(configurator);

        configurator.UsePostgres();
    }
}
