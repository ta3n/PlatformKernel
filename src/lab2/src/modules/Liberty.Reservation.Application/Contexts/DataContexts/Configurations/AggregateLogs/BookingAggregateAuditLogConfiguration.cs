using Liberty.Reservation.Application.Contexts.DataContexts.Entities.AggregateLogs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.AggregateLogs;

public class BookingAggregateAuditLogConfiguration : BaseDataEntityTypeConfiguration<BookingAggregateAuditLog>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<BookingAggregateAuditLog> builder
    )
    {
        builder.ToTable("booking_aggregate_audit_log", DbConfiguration.DefaultSchema);

        builder.Property(e => e.ChangedFields)
            .HasColumnType(JsonbType)
            .HasDefaultValueSql(JsonbDefaultValue);

        builder.Property(e => e.Request)
            .HasColumnType(JsonbType)
            .HasDefaultValueSql(JsonbDefaultValue);
    }
}
