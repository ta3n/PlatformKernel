using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Data;

public class AlertMessageConfigration : BaseDataEntityTypeConfiguration<AlertMessage>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<AlertMessage> builder
    )
    {
        builder.ToTable("alert_message", DbConfiguration.DefaultSchema);

        builder.Property(e => e.Title)
            .HasColumnType("jsonb")
            .HasConversion(MultilingualTextConverter())
            .Metadata.SetValueComparer(typeof(MultilingualTextComparer));

        builder.Property(e => e.Title)
            .HasDefaultValueSql("'{}'::jsonb");

        builder
            .HasIndex(x => x.Title)
            .HasMethod("GIN");

        builder.Property(e => e.Content)
            .HasColumnType("jsonb")
            .HasConversion(MultilingualTextConverter())
            .Metadata.SetValueComparer(typeof(MultilingualTextComparer));

        builder.Property(e => e.Content)
            .HasDefaultValueSql("'{}'::jsonb");

        builder
            .HasIndex(x => x.Content)
            .HasMethod("GIN");
    }
}
