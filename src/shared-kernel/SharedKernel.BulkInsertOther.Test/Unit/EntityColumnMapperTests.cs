using NpgsqlTypes;
using System.Globalization;
using SharedKernel.BulkInsertOther.Mapping;
using SharedKernel.BulkInsertOther.Test.TestEntities;

namespace SharedKernel.BulkInsertOther.Test.Unit;

public sealed class EntityColumnMapperTests
{
    [Fact]
    public void Explicit_mapping_builds_compiled_mapper_and_extracts_values()
    {
        var options = new BulkInsertEntityOptions<MetricPoint>();
        options.MapColumn(static point => point.DeviceId, "DeviceId", NpgsqlDbType.Text);
        options.MapColumn(static point => point.OccurredAt, "OccurredAt", NpgsqlDbType.TimestampTz);
        options.MapColumn(static point => point.Value, "Value", NpgsqlDbType.Double);
        options.MapColumn(static point => point.Quality, "Quality", NpgsqlDbType.Integer, isNullable: true);

        var descriptor = new BulkInsertEntityDescriptor<MetricPoint>(options);
        var values = new object?[descriptor.Mapper.Columns.Count];
        var entity = new MetricPoint(
            "sensor-1",
            DateTimeOffset.Parse("2026-03-29T10:15:30+00:00", CultureInfo.InvariantCulture),
            42.5d,
            7
        );

        descriptor.Mapper.Extract(in entity, values);

        Assert.Collection(
            descriptor.Mapper.Columns,
            column => Assert.Equal("DeviceId", column.ColumnName),
            column => Assert.Equal("OccurredAt", column.ColumnName),
            column => Assert.Equal("Value", column.ColumnName),
            column =>
            {
                Assert.Equal("Quality", column.ColumnName);
                Assert.True(column.IsNullable);
            }
        );

        Assert.Equal("sensor-1", values[0]);
        Assert.Equal(entity.OccurredAt, values[1]);
        Assert.Equal(42.5d, values[2]);
        Assert.Equal(7, values[3]);
    }

    [Fact]
    public void Reflection_fallback_uses_attributes_and_ignores_marked_members()
    {
        var descriptor = new BulkInsertEntityDescriptor<AttributedMetricPoint>(
            new BulkInsertEntityOptions<AttributedMetricPoint>()
        );

        Assert.Collection(
            descriptor.Mapper.Columns,
            column => Assert.Equal("device_id", column.ColumnName),
            column => Assert.Equal("occurred_at", column.ColumnName),
            column => Assert.Equal("value", column.ColumnName)
        );
    }
}
