using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Data;

public class CustomerInfoConfiguration : BaseDataEntityTypeConfiguration<CustomerInfo>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<CustomerInfo> builder
    )
    {
        builder.ToTable("customer_info", DbConfiguration.DefaultSchema);
    }
}
