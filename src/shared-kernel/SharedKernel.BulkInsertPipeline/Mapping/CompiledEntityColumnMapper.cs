using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using Npgsql;

namespace SharedKernel.BulkInsertPipeline.Mapping;

internal sealed class CompiledEntityColumnMapper<T>(
    IReadOnlyList<IEntityColumnAccessor<T>> accessors
) : IEntityColumnMapper<T>
{
    public IReadOnlyList<BulkInsertColumn> Columns { get; } =
        [.. accessors.Select(static accessor => accessor.Column)];

    public void Write(
        in T entity,
        NpgsqlBinaryImporter writer
    )
    {
        writer.StartRow();

        foreach (var accessor in accessors)
        {
            accessor.Write(entity, writer);
        }
    }

    public async ValueTask WriteAsync(
        T entity,
        NpgsqlBinaryImporter writer,
        CancellationToken ct = default
    )
    {
        await writer.StartRowAsync(ct).ConfigureAwait(false);

        foreach (var accessor in accessors)
        {
            await accessor.WriteAsync(entity, writer, ct).ConfigureAwait(false);
        }
    }

    public void Extract(
        in T entity,
        Span<object?> destination
    )
    {
        if (destination.Length < accessors.Count)
        {
            throw new ArgumentException(
                "Destination span is shorter than the mapped column count.",
                nameof(destination)
            );
        }

        for (var index = 0; index < accessors.Count; index++)
        {
            destination[index] = accessors[index].GetValue(entity);
        }
    }

    internal static CompiledEntityColumnMapper<T> Create(
        IReadOnlyList<BulkInsertEntityOptions<T>.ColumnRegistration> columns
    )
    {
        return new CompiledEntityColumnMapper<T>(
            [.. columns.Select(EntityColumnAccessorFactory.Create)]
        );
    }

    internal static CompiledEntityColumnMapper<T> CreateFromProperties(
        IReadOnlyList<PropertyMapping> properties
    )
    {
        return new CompiledEntityColumnMapper<T>(
            [
                .. properties.Select(
                    EntityColumnAccessorFactory.CreateReflectionAccessor
                )
            ]
        );
    }

    internal sealed record PropertyMapping(
        string ColumnName,
        NpgsqlTypes.NpgsqlDbType DbType,
        bool IsNullable,
        PropertyInfo Property
    );
}

internal interface IEntityColumnAccessor<in T>
{
    BulkInsertColumn Column { get; }

    void Write(
        T entity,
        NpgsqlBinaryImporter writer
    );

    ValueTask WriteAsync(
        T entity,
        NpgsqlBinaryImporter writer,
        CancellationToken ct
    );

    object? GetValue(
        T entity
    );
}

[SuppressMessage(
    "Major Code Smell",
    "S3011",
    Justification =
        "The mapper factory resolves private generic helper methods once at startup to avoid reflection on the write hot path."
)]
internal static class EntityColumnAccessorFactory
{
    [SuppressMessage(
        "Major Code Smell",
        "S3011",
        Justification =
            "The mapper factory resolves private generic helper methods once at startup to avoid reflection on the write hot path."
    )]
    public static IEntityColumnAccessor<T> Create<T>(
        BulkInsertEntityOptions<T>.ColumnRegistration registration
    )
    {
        var method = typeof(EntityColumnAccessorFactory)
            .GetMethod(nameof(CreateTypedAccessor), BindingFlags.NonPublic | BindingFlags.Static)!;

        var genericMethod = method.MakeGenericMethod(typeof(T), registration.PropertyType);
        return (IEntityColumnAccessor<T>)genericMethod.Invoke(
            null,
            [
                registration.Selector,
                registration.ColumnName,
                registration.DbType,
                registration.IsNullable
            ]
        )!;
    }

    [SuppressMessage(
        "Major Code Smell",
        "S3011",
        Justification =
            "The mapper factory resolves private generic helper methods once at startup to avoid reflection on the write hot path."
    )]
    public static IEntityColumnAccessor<T> CreateReflectionAccessor<T>(
        CompiledEntityColumnMapper<T>.PropertyMapping property
    )
    {
        var propertyType = property.Property.PropertyType;
        var method = typeof(EntityColumnAccessorFactory)
            .GetMethod(nameof(CreateTypedReflectionAccessor), BindingFlags.NonPublic | BindingFlags.Static)!;

        var genericMethod = method.MakeGenericMethod(typeof(T), propertyType);
        return (IEntityColumnAccessor<T>)genericMethod.Invoke(
            null,
            [
                property.Property,
                property.ColumnName,
                property.DbType,
                property.IsNullable
            ]
        )!;
    }

    private static IEntityColumnAccessor<T> CreateTypedAccessor<T, TProperty>(
        Expression<Func<T, TProperty>> selector,
        string columnName,
        NpgsqlTypes.NpgsqlDbType dbType,
        bool? isNullable
    )
    {
        var getter = selector.Compile();
        return CreateAccessor(getter, columnName, dbType, isNullable);
    }

    private static IEntityColumnAccessor<T> CreateTypedReflectionAccessor<T, TProperty>(
        PropertyInfo property,
        string columnName,
        NpgsqlTypes.NpgsqlDbType dbType,
        bool isNullable
    )
    {
        var getterMethod = property.GetMethod
            ?? throw new InvalidOperationException(
                $"Property '{property.Name}' on '{typeof(T).FullName}' does not expose a readable getter."
            );

        var getter = (Func<T, TProperty>)getterMethod.CreateDelegate(typeof(Func<T, TProperty>));
        return CreateAccessor(getter, columnName, dbType, isNullable);
    }

    private static IEntityColumnAccessor<T> CreateAccessor<T, TProperty>(
        Func<T, TProperty> getter,
        string columnName,
        NpgsqlTypes.NpgsqlDbType dbType,
        bool? isNullable
    )
    {
        var column = new BulkInsertColumn(
            columnName,
            dbType,
            isNullable ?? IsNullable(typeof(TProperty))
        );

        var nullableUnderlyingType = Nullable.GetUnderlyingType(typeof(TProperty));
        if (nullableUnderlyingType is not null)
        {
            var method = typeof(EntityColumnAccessorFactory)
                .GetMethod(nameof(CreateNullableAccessor), BindingFlags.NonPublic | BindingFlags.Static)!;

            var genericMethod = method.MakeGenericMethod(typeof(T), nullableUnderlyingType);
            return (IEntityColumnAccessor<T>)genericMethod.Invoke(null, [getter, column])!;
        }

        if (!typeof(TProperty).IsValueType)
        {
            var method = typeof(EntityColumnAccessorFactory)
                .GetMethod(nameof(CreateReferenceAccessor), BindingFlags.NonPublic | BindingFlags.Static)!;

            var genericMethod = method.MakeGenericMethod(typeof(T), typeof(TProperty));
            return (IEntityColumnAccessor<T>)genericMethod.Invoke(null, [getter, column])!;
        }

        var structMethod = typeof(EntityColumnAccessorFactory)
            .GetMethod(nameof(CreateStructAccessor), BindingFlags.NonPublic | BindingFlags.Static)!;

        var closedStructMethod = structMethod.MakeGenericMethod(typeof(T), typeof(TProperty));
        return (IEntityColumnAccessor<T>)closedStructMethod.Invoke(null, [getter, column])!;
    }

    private static IEntityColumnAccessor<T> CreateReferenceAccessor<T, TValue>(
        Func<T, TValue> getter,
        BulkInsertColumn column
    )
        where TValue : class
    {
        return new ReferenceColumnAccessor<T, TValue>(getter, column);
    }

    private static IEntityColumnAccessor<T> CreateStructAccessor<T, TValue>(
        Func<T, TValue> getter,
        BulkInsertColumn column
    )
        where TValue : struct
    {
        return new StructColumnAccessor<T, TValue>(getter, column);
    }

    private static IEntityColumnAccessor<T> CreateNullableAccessor<T, TValue>(
        Func<T, TValue?> getter,
        BulkInsertColumn column
    )
        where TValue : struct
    {
        return new NullableStructColumnAccessor<T, TValue>(getter, column);
    }

    private static bool IsNullable(
        Type type
    )
    {
        return !type.IsValueType || Nullable.GetUnderlyingType(type) is not null;
    }
}

internal sealed class StructColumnAccessor<T, TValue>(
    Func<T, TValue> getter,
    BulkInsertColumn column
) : IEntityColumnAccessor<T>
    where TValue : struct
{
    public BulkInsertColumn Column => column;

    public void Write(
        T entity,
        NpgsqlBinaryImporter writer
    )
    {
        writer.Write(getter(entity), column.DbType);
    }

    public async ValueTask WriteAsync(
        T entity,
        NpgsqlBinaryImporter writer,
        CancellationToken ct
    )
    {
        await writer.WriteAsync(getter(entity), column.DbType, ct).ConfigureAwait(false);
    }

    public object GetValue(
        T entity
    )
    {
        return getter(entity);
    }
}

internal sealed class NullableStructColumnAccessor<T, TValue>(
    Func<T, TValue?> getter,
    BulkInsertColumn column
) : IEntityColumnAccessor<T>
    where TValue : struct
{
    public BulkInsertColumn Column => column;

    public void Write(
        T entity,
        NpgsqlBinaryImporter writer
    )
    {
        var value = getter(entity);
        if (value.HasValue)
        {
            writer.Write(value.Value, column.DbType);
            return;
        }

        writer.WriteNull();
    }

    public async ValueTask WriteAsync(
        T entity,
        NpgsqlBinaryImporter writer,
        CancellationToken ct
    )
    {
        var value = getter(entity);
        if (value.HasValue)
        {
            await writer.WriteAsync(value.Value, column.DbType, ct).ConfigureAwait(false);
            return;
        }

        await writer.WriteNullAsync(ct).ConfigureAwait(false);
    }

    public object? GetValue(
        T entity
    )
    {
        return getter(entity);
    }
}

internal sealed class ReferenceColumnAccessor<T, TValue>(
    Func<T, TValue?> getter,
    BulkInsertColumn column
) : IEntityColumnAccessor<T>
    where TValue : class
{
    public BulkInsertColumn Column => column;

    public void Write(
        T entity,
        NpgsqlBinaryImporter writer
    )
    {
        var value = getter(entity);
        if (value is null)
        {
            writer.WriteNull();
            return;
        }

        writer.Write(value, column.DbType);
    }

    public async ValueTask WriteAsync(
        T entity,
        NpgsqlBinaryImporter writer,
        CancellationToken ct
    )
    {
        var value = getter(entity);
        if (value is null)
        {
            await writer.WriteNullAsync(ct).ConfigureAwait(false);
            return;
        }

        await writer.WriteAsync(value, column.DbType, ct).ConfigureAwait(false);
    }

    public object? GetValue(
        T entity
    )
    {
        return getter(entity);
    }
}
