using Liberty.Entity;
using Liberty.Entity.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations;

/// <summary>
/// Represents the base configuration class for mapping entities of type <typeparamref name="TBase"/>
/// to a relational database using Entity Framework Core. The generic type parameter <typeparamref name="TBase"/>
/// must inherit from <see cref="EntityData"/>.
/// </summary>
/// <typeparam name="TBase">The type of the entity that inherits from <see cref="EntityData"/>.</typeparam>
/// <remarks>
/// This abstract class provides common configuration operations such as:
/// - Setting a primary key on the "Id" property.
/// - Marking the "Code" property as required, with a unique constraint.
/// - Applying a query filter to exclude soft-deleted entities.
/// Subclasses must override the <see cref="EntityConfigure(EntityTypeBuilder{TBase})"/> method
/// to add entity-specific configuration logic.
/// </remarks>
public abstract class BaseDataEntityTypeConfiguration<TBase> : IEntityTypeConfiguration<TBase>
    where TBase : EntityData
{
    /// <summary>
    /// Represents the database column type for JSONB data in PostgreSQL.
    /// </summary>
    /// <remarks>
    /// This constant is typically used to define or interact with columns that store JSONB data.
    /// JSONB is a PostgreSQL column type that stores JSON data in a binary format, allowing
    /// for more advanced querying capabilities compared to plain JSON.
    /// </remarks>
    protected const string JsonbType = "jsonb";

    /// <summary>
    /// Represents the default value for a JSONB type in the database configuration.
    /// This value is set to an empty JSON object represented as a JSONB type.
    /// Commonly used in PostgreSQL database configurations to ensure JSONB columns have a default value of an empty object.
    /// </summary>
    protected const string JsonbDefaultValue = "'{}'::jsonb";

    /// <summary>
    /// Configures the entity type for the derived entity.
    /// </summary>
    /// <param name="builder">
    /// Provides a simple API to configure the properties and relationships of the entity type.
    /// </param>
    public void Configure(
        EntityTypeBuilder<TBase> builder
    )
    {
        EntityConfigure(builder);

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnOrder(1);

        builder.Property(u => u.Code).IsRequired();
        builder.HasIndex(u => u.Code).IsUnique();

        builder.HasQueryFilter(p => !p.IsDeleted);
    }

    /// <summary>
    /// Configures the entity of type <typeparamref name="TBase"/>.
    /// This method is abstract and must be implemented in derived classes to
    /// provide the configuration for a specific entity type.
    /// </summary>
    /// <typeparam name="TBase">The type of the entity being configured.</typeparam>
    /// <param name="builder">The builder used to configure the entity type.</param>
    protected abstract void EntityConfigure(
        EntityTypeBuilder<TBase> builder
    );

    /// Creates a value converter for the `MultilingualText` type, enabling conversion
    /// between a multilingual text object and its JSON string representation during
    /// database operations. This converter handles serialization of `MultilingualText`
    /// instances to JSON strings when saving to the database and deserialization back
    /// to `MultilingualText` when reading from the database.
    /// <returns>
    /// A `ValueConverter` instance configured for converting `MultilingualText?` to
    /// and from `string`.
    /// </returns>
    protected static ValueConverter<MultilingualText?, string> MultilingualTextConverter()
    {
        var multiLanguageConverter = new ValueConverter<MultilingualText?, string>(
            x => x == null ? "null" : JsonConvert.SerializeObject(x), // Convert Dictionary -> JSON string
            x => x == "null" || string.IsNullOrEmpty(x)
                ? new()
                : JsonConvert.DeserializeObject<MultilingualText>(x)!
        );

        return multiLanguageConverter;
    }
}
