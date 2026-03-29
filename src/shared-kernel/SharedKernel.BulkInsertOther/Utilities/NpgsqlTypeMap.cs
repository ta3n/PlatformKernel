using System.Text.Json;
using NpgsqlTypes;

namespace SharedKernel.BulkInsertOther.Utilities;

internal static class NpgsqlTypeMap
{
    public static NpgsqlDbType Resolve(
        Type clrType
    )
    {
        var target = Nullable.GetUnderlyingType(clrType) ?? clrType;

        if (target.IsEnum)
        {
            return NpgsqlDbType.Integer;
        }

        if (target == typeof(bool))
        {
            return NpgsqlDbType.Boolean;
        }

        if (target == typeof(short))
        {
            return NpgsqlDbType.Smallint;
        }

        if (target == typeof(int))
        {
            return NpgsqlDbType.Integer;
        }

        if (target == typeof(long))
        {
            return NpgsqlDbType.Bigint;
        }

        if (target == typeof(float))
        {
            return NpgsqlDbType.Real;
        }

        if (target == typeof(double))
        {
            return NpgsqlDbType.Double;
        }

        if (target == typeof(decimal))
        {
            return NpgsqlDbType.Numeric;
        }

        if (target == typeof(Guid))
        {
            return NpgsqlDbType.Uuid;
        }

        if (target == typeof(string))
        {
            return NpgsqlDbType.Text;
        }

        if (target == typeof(DateTime))
        {
            return NpgsqlDbType.TimestampTz;
        }

        if (target == typeof(DateTimeOffset))
        {
            return NpgsqlDbType.TimestampTz;
        }

        if (target == typeof(DateOnly))
        {
            return NpgsqlDbType.Date;
        }

        if (target == typeof(TimeOnly))
        {
            return NpgsqlDbType.Time;
        }

        if (target == typeof(TimeSpan))
        {
            return NpgsqlDbType.Interval;
        }

        if (target == typeof(byte[]))
        {
            return NpgsqlDbType.Bytea;
        }

        if (target == typeof(JsonDocument) || target == typeof(JsonElement))
        {
            return NpgsqlDbType.Jsonb;
        }

        throw new NotSupportedException(
            $"The CLR type '{clrType.FullName}' does not have a built-in PostgreSQL bulk insert mapping. Register the column explicitly."
        );
    }

    public static bool IsNullable(
        Type clrType
    )
    {
        return !clrType.IsValueType || Nullable.GetUnderlyingType(clrType) is not null;
    }
}
