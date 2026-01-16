using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using PlatformKernel.Entity;
using PlatformKernel.SysException.Exceptions;
using PlatformKernel.UnitOfWork.Abstractions;

namespace PlatformKernel.Service.Base.Application.Utils;

/// <summary>
/// Provides an extension method for retrieving metadata properties of an entity type
/// from the database context. This utility is designed to extract information
/// such as schema, table name, and column mappings for a specific entity type
/// associated with a given unit of work.
/// </summary>
public static class EntityPropertyExtension
{
    /// <summary>
    /// Retrieves the entity property metadata for a given entity type.
    /// </summary>
    /// <typeparam name="T">The type of the entity for which the property metadata is retrieved. Must implement <see cref="IBaseEntity"/>.</typeparam>
    /// <param name="context">The unit of work context providing access to the entity and database context.</param>
    /// <returns>
    /// An instance of <see cref="EntityProperty"/> containing metadata about the entity type, such as schema, table name, and column mappings.
    /// </returns>
    /// <exception cref="AppBaseException">
    /// Thrown if the entity type is null, the schema is null, the table name is null, or if any entity property does not have a mapped column name.
    /// </exception>
    public static EntityProperty GetEntityProperty<T>(
        this IUnitOfWork context
    )
        where T : IBaseEntity
    {
        var entityType = context
                .GetDbContext()
                .Model
                .FindEntityType(typeof(T))
            ?? throw new AppBaseException("Entity type is not null");

        var properties = entityType.GetProperties();
        var schema = entityType.GetSchema() ?? throw new AppBaseException("Schema of the entity is not null");
        var tableName = entityType.GetTableName() ?? throw new AppBaseException("Table name of the entity is not null");
        Dictionary<string, string> columnNames = [];
        List<string> dbColumnNames = [];
        foreach (var property in properties)
        {
            var fieldName = property.Name;
            var columnName = property.GetColumnName(StoreObjectIdentifier.Table(tableName, schema))
                ?? throw new AppBaseException($"{fieldName} of the {nameof(T)} not get mapping column name.");

            columnNames.Add(
                fieldName,
                columnName
            );
            dbColumnNames.Add(columnName);
        }

        var result = new EntityProperty
        {
            Schema = schema,
            TableName = tableName,
            ColumnNames = columnNames,
            DbColumnNames = dbColumnNames
        };

        return result;
    }
}

/// <summary>
/// Represents the properties and metadata associated with a database entity.
/// </summary>
public class EntityProperty
{
    /// Represents the database schema associated with an entity.
    /// This property holds the schema name where the entity table resides.
    /// The schema is a required field and defines the organizational structure
    /// or namespace under which the database objects, such as tables, are grouped.
    /// It is used to construct fully qualified table names and ensures proper mapping
    /// of the entity to the correct database schema.
    public required string Schema { get; set; }

    /// <summary>
    /// Gets or sets the name of the database table associated with this entity.
    /// </summary>
    /// <remarks>
    /// This property represents the table name from the underlying database schema. It is a required property and must not be null.
    /// </remarks>
    public required string TableName { get; set; }

    /// <summary>
    /// Represents an optional alias name for the table within the database query context.
    /// This property can be used to reference the table by a shorter or alternative name,
    /// enhancing query readability and usability.
    /// </summary>
    public string? Alias { get; set; }

    /// <summary>
    /// Gets the fully qualified name of the table in the format "Schema.TableName" unless an alias is provided.
    /// If an alias is defined, it returns the alias instead of the "Schema.TableName" format.
    /// </summary>
    /// <remarks>
    /// The property is useful for constructing SQL queries or performing database operations
    /// where the complete table name or its alias is required.
    /// </remarks>
    public string FullTableName => Alias ?? $"{Schema}.{TableName}";

    /// Provides a combined representation of the table name and its alias, if an alias is defined.
    /// This property constructs a string in the format `TableName AS Alias` when the alias is set.
    /// If no alias is defined, it returns the table name as is.
    public string TableNameAlias => Alias is not null
        ? $"{TableName} AS {Alias}"
        : TableName;

    /// Represents a mapping of entity properties to their corresponding database column names.
    /// Provides a dictionary where each key is the name of an entity's property,
    /// and the value is the corresponding column name in the database.
    public required Dictionary<string, string> ColumnNames { get; set; }

    /// Represents a collection of database column names associated with an entity.
    /// This property contains a list of the actual column names as they are mapped in the database.
    /// It is used to ensure that the correct column names are referenced during database operations.
    public required IEnumerable<string> DbColumnNames { get; set; }

    /// <summary>
    /// Retrieves the database column name associated with the specified field name in the entity.
    /// </summary>
    /// <param name="fieldName">The field name for which the corresponding database column name is required.</param>
    /// <returns>The database column name corresponding to the provided field name.</returns>
    /// <exception cref="AppBaseException">
    /// Thrown when the specified field name is not found in the column mappings.
    /// </exception>
    public string ColumnName(
        string fieldName
    )
    {
        try
        {
            return ColumnNames[fieldName];
        }
        catch (Exception ex)
        {
            throw new AppBaseException(ex.Message, ex);
        }
    }

    /// <summary>
    /// Constructs and returns the fully qualified column name, combining the full table name
    /// and the column name derived from the given field name.
    /// </summary>
    /// <param name="fieldName">The name of the entity field for which to generate the fully qualified column name.</param>
    /// <returns>The full column name in the format of 'FullTableName.ColumnName'.</returns>
    public string FullColumnName(
        string fieldName
    )
    {
        return $"{FullTableName}.{ColumnName(fieldName)}";
    }
}
