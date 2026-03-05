using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Data;

public class ConsumptionTaxConfiguration : BaseDataEntityTypeConfiguration<ConsumptionTax>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<ConsumptionTax> builder
    )
    {
        builder.ToTable("consumption_tax", DbConfiguration.DefaultSchema);
    }
}
