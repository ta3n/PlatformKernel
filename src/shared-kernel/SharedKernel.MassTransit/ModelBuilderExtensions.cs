using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace SharedKernel.MassTransit;

/// <summary>
/// Provides Entity Framework model builder helpers for MassTransit persistence.
/// </summary>
public static class ModelBuilderExtensions
{
    /// <summary>
    /// Adds the MassTransit transactional outbox entities to the current <see cref="ModelBuilder"/>.
    /// </summary>
    public static ModelBuilder AddMassTransitTransactionalOutboxEntities(
        this ModelBuilder modelBuilder
    )
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.AddInboxStateEntity();
        modelBuilder.AddOutboxMessageEntity();
        modelBuilder.AddOutboxStateEntity();

        return modelBuilder;
    }
}
