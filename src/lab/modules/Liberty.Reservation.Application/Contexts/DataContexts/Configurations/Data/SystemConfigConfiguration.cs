using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Data;

public class SystemConfigConfiguration : BaseDataEntityTypeConfiguration<SystemConfig>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<SystemConfig> builder
    )
    {
        builder.ToTable("SystemConfig", DbConfiguration.DefaultSchema);

        builder.Ignore(t => t.TemplateFormatData);
    }
}
