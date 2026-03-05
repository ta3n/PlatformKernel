using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Data;

public class AppDateConfiguration : BaseDataEntityTypeConfiguration<AppDate>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<AppDate> builder
    )
    {
        builder.ToTable("app_date", DbConfiguration.DefaultSchema);

        builder.Property(a => a.Id).ValueGeneratedNever();

        builder.Property(a => a.DateTime).IsRequired(); // 必須設定

        builder
            .HasMany(c => c.AppDateAppDateTypes)
            .WithOne(c => c.AppDate)
            .HasForeignKey(c => c.AppDateId);

        builder
            .HasMany(c => c.AppDateAppDateDatas)
            .WithOne(c => c.AppDate)
            .HasForeignKey(c => c.AppDateId);

        builder
            .HasMany(c => c.RoomGroupAppDates)
            .WithOne(c => c.AppDate)
            .HasForeignKey(c => c.AppDateId);

        builder
            .HasMany(c => c.RoomGroupSiteAppDatePriceDatas)
            .WithOne(c => c.AppDate)
            .HasForeignKey(c => c.AppDateId);

        builder
            .HasMany(c => c.RoomGroupSiteAppDates)
            .WithOne(c => c.AppDate)
            .HasForeignKey(c => c.AppDateId);

        builder
            .HasMany(c => c.OptionItemAppDates)
            .WithOne(c => c.AppDate)
            .HasForeignKey(c => c.AppDateId);
    }
}
