using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Templates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Data;

public class SystemConfigConfiguration : BaseDataEntityTypeConfiguration<SystemConfig>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<SystemConfig> builder
    )
    {
        builder.ToTable("system_config", DbConfiguration.DefaultSchema);

        builder.Property(e => e.TemplateFormatData)
            .HasColumnType("jsonb")
            .HasDefaultValue(new TemplateFormatData())
            .HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<TemplateFormatData>(v)
            );

        builder
            .HasIndex(x => x.TemplateFormatData)
            .HasMethod("GIN");
    }
}
