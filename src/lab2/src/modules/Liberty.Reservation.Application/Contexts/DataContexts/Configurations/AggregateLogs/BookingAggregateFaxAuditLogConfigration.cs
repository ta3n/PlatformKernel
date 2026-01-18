using Liberty.Reservation.Application.Contexts.DataContexts.Entities.AggregateLogs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.AggregateLogs;

public class BookingAggregateFaxAuditLogConfigration : BaseDataEntityTypeConfiguration<BookingAggregateFaxAuditLog>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<BookingAggregateFaxAuditLog> builder
    )
    {
        builder.ToTable("booking_aggregate_fax_audit_log", DbConfiguration.DefaultSchema);
    }
}
