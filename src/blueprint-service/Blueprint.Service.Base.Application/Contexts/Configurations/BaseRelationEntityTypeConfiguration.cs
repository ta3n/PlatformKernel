using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedKernel.Entity;

namespace Blueprint.Service.Base.Application.Contexts.Configurations;

/// <summary>
/// Abstract base class providing a generic entity type configuration for entities
/// that inherit from the <see cref="EntityRelation"/> class. This class utilizes
/// the Entity Framework Core configuration mechanism for setting up shared properties
/// and behaviors for derived relation entities.
/// </summary>
/// <typeparam name="TBase">
/// The type of the entity being configured. Must be a subclass of <see cref="EntityRelation"/>.
/// </typeparam>
public abstract class BaseRelationEntityTypeConfiguration<TBase> : IEntityTypeConfiguration<TBase>
    where TBase : EntityRelation
{
    /// <summary>
    /// Configures the entity of type TBase by applying necessary configurations.
    /// It ensures that a query filter is applied to exclude entities marked as deleted.
    /// </summary>
    /// <param name="builder">The builder to be used for entity type configuration.</param>
    public virtual void Configure(
        EntityTypeBuilder<TBase> builder
    )
    {
        EntityConfigure(builder);

        builder.HasQueryFilter(p => !p.IsDeleted);
    }

    /// <summary>
    /// Defines the specific configuration for the given entity type.
    /// This method must be implemented in derived classes to provide
    /// entity-specific configuration logic.
    /// </summary>
    /// <param name="builder">
    /// The reference to the <see cref="EntityTypeBuilder{TBase}"/> used to configure the entity type.
    /// </param>
    protected abstract void EntityConfigure(
        EntityTypeBuilder<TBase> builder
    );
}
