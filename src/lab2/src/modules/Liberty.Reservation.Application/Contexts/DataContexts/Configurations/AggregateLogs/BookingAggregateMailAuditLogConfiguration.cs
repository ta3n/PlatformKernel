using Liberty.Reservation.Application.Contexts.DataContexts.Entities.AggregateLogs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.AggregateLogs;

public class BookingAggregateMailAuditLogConfiguration : BaseDataEntityTypeConfiguration<BookingAggregateMailAuditLog>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<BookingAggregateMailAuditLog> builder
    )
    {
        builder.ToTable("booking_aggregate_mail_audit_log", DbConfiguration.DefaultSchema);
    }
}
