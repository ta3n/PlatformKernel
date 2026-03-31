using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace SharedKernel.BulkInsert.Internal;

internal sealed class EfCorePostgreSqlBulkInsertMetadataResolver
    : IPostgreSqlBulkInsertMetadataResolver
{
    public PostgreSqlBulkInsertMetadata<TEntity> Resolve<TEntity>(
        DbContext dbContext
    )
        where TEntity : class
    {
        var entityType = dbContext.Model.FindEntityType(typeof(TEntity))
            ?? throw new InvalidOperationException(
                $"Entity type '{typeof(TEntity).FullName}' is not mapped in the supplied DbContext."
            );

        var tableName = entityType.GetTableName()
            ?? throw new InvalidOperationException(
                $"Entity type '{entityType.DisplayName()}' is not mapped to a database table."
            );

        var schema = entityType.GetSchema();
        var storeObjectIdentifier = StoreObjectIdentifier.Table(tableName, schema);

        var columns = entityType
            .GetProperties()
            .Where(
                property =>
                    property.PropertyInfo is not null
                    && property.GetColumnName(storeObjectIdentifier) is not null
                    && ShouldInclude(property, storeObjectIdentifier)
            )
            .Select(
                property =>
                {
                    var propertyInfo = property.PropertyInfo
                        ?? throw new InvalidOperationException(
                            $"Property '{property.Name}' is not accessible for bulk insert."
                        );
                    var columnName = property.GetColumnName(storeObjectIdentifier)
                        ?? throw new InvalidOperationException(
                            $"Property '{property.Name}' is not mapped to a column."
                        );
                    var converter = property.GetTypeMapping().Converter;
                    var providerClrType = converter?.ProviderClrType
                        ?? Nullable.GetUnderlyingType(property.ClrType)
                        ?? property.ClrType;

                    return new PostgreSqlBulkInsertColumn<TEntity>(
                        property.Name,
                        columnName,
                        PostgreSqlBulkInsertSqlHelper.QuoteIdentifier(columnName),
                        providerClrType,
                        property.GetColumnType(storeObjectIdentifier),
                        entity =>
                        {
                            var value = propertyInfo.GetValue(entity);
                            return value is null
                                ? null
                                : converter?.ConvertToProvider(value) ?? value;
                        }
                    );
                }
            )
            .ToArray();

        if (columns.Length == 0)
        {
            throw new InvalidOperationException(
                $"Entity type '{entityType.DisplayName()}' does not expose any insertable scalar column."
            );
        }

        return new PostgreSqlBulkInsertMetadata<TEntity>(
            schema,
            tableName,
            PostgreSqlBulkInsertSqlHelper.GetQualifiedTableName(schema, tableName),
            PostgreSqlBulkInsertSqlHelper.GetQuotedQualifiedTableName(schema, tableName),
            columns
        );
    }

    private static bool ShouldInclude(
        IProperty property,
        StoreObjectIdentifier storeObjectIdentifier
    )
    {
        if (property.IsShadowProperty())
        {
            return false;
        }

        if (property.GetComputedColumnSql() is not null)
        {
            return false;
        }

        if (property.ValueGenerated is ValueGenerated.OnAddOrUpdate or ValueGenerated.OnUpdate)
        {
            return false;
        }

        var valueGenerationStrategy = property.GetValueGenerationStrategy(storeObjectIdentifier);
        if (
            valueGenerationStrategy is NpgsqlValueGenerationStrategy.IdentityAlwaysColumn
            or NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
            or NpgsqlValueGenerationStrategy.SerialColumn
        )
        {
            return false;
        }

        if (
            property.ValueGenerated == ValueGenerated.OnAdd
            && (property.GetDefaultValueSql() is not null || property.GetDefaultValue() is not null)
        )
        {
            return false;
        }

        return !(
            property.GetBeforeSaveBehavior() == PropertySaveBehavior.Ignore
            && property.ValueGenerated != ValueGenerated.Never
        );
    }
}
