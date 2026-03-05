using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Data;

public class IntegrationEventOutboxConfiguration
    : BaseDataEntityTypeConfiguration<IntegrationEventOutbox>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<IntegrationEventOutbox> builder
    )
    {
        builder.ToTable("integration_event_outbox", DbConfiguration.DefaultSchema);
    }
}
