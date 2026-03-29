using System.Reflection;
using SharedKernel.BulkInsertPipeline.Utilities;

namespace SharedKernel.BulkInsertPipeline.Mapping;

internal static class EntityColumnMapperFactory
{
    public static IEntityColumnMapper<T> Create<T>(
        BulkInsertEntityOptions<T> options
    )
    {
        if (options.Columns.Count > 0)
        {
            return CompiledEntityColumnMapper<T>.Create(options.Columns);
        }

        if (!options.AllowReflectionFallback)
        {
            throw new InvalidOperationException(
                $"No column mapping was registered for entity '{typeof(T).FullName}', and reflection fallback was disabled."
            );
        }

        var properties = typeof(T)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(static property => property.CanRead)
            .Where(static property => property.GetCustomAttribute<BulkInsertIgnoreAttribute>() is null)
            .Select(CreatePropertyMapping<T>)
            .ToArray();

        if (properties.Length == 0)
        {
            throw new InvalidOperationException(
                $"Entity '{typeof(T).FullName}' does not expose any readable properties for bulk insert."
            );
        }

        return CompiledEntityColumnMapper<T>.CreateFromProperties(properties);
    }

    private static CompiledEntityColumnMapper<T>.PropertyMapping CreatePropertyMapping<T>(
        PropertyInfo property
    )
    {
        var columnAttribute = property.GetCustomAttribute<BulkInsertColumnAttribute>();
        var columnName = columnAttribute?.ColumnName ?? BulkInsertNamingPolicy.ToSnakeCase(property.Name);
        var dbType =
            columnAttribute?.DbType
            ?? NpgsqlTypeMap.Resolve(property.PropertyType);
        var isNullable = columnAttribute?.IsNullable ?? NpgsqlTypeMap.IsNullable(property.PropertyType);

        return new CompiledEntityColumnMapper<T>.PropertyMapping(
            columnName,
            dbType,
            isNullable,
            property
        );
    }
}
