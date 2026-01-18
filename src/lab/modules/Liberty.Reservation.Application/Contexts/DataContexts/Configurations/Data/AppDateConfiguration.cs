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
        builder.ToTable("AppDate", DbConfiguration.DefaultSchema);

        builder.Property(a => a.Id).ValueGeneratedNever(); // キーは日付のため手動で登録;

        builder.Property(a => a.DateTime).IsRequired(); // 必須設定

        // このプロパティはマッピングから外す
        // builder.Ignore(a => a.MCalendarDatas);

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
            .HasMany(c => c.CalendarAppDateAppDateTypes)
            .WithOne(c => c.AppDate)
            .HasForeignKey(c => c.AppDateId);

        builder
            .HasMany(c => c.PlanRoomGroupSiteAppDates)
            .WithOne(c => c.AppDate)
            .HasForeignKey(c => c.AppDateId);

        builder
            .HasMany(c => c.PlanRoomGroupSiteAppDatePriceDatas)
            .WithOne(c => c.AppDate)
            .HasForeignKey(c => c.AppDateId);

        builder
            .HasMany(c => c.OptionItemAppDates)
            .WithOne(c => c.AppDate)
            .HasForeignKey(c => c.AppDateId);

        builder
            .HasMany(c => c.ReservationRoomGroupAppDateOptionItems)
            .WithOne(c => c.AppDate)
            .HasForeignKey(c => c.AppDateId);

        builder
            .HasMany(c => c.ReservationRoomGroupAppDatePersonAgeTypes)
            .WithOne(c => c.AppDate)
            .HasForeignKey(c => c.AppDateId);

        builder
            .HasMany(c => c.ReservationPlanRoomGroupAppDates)
            .WithOne(c => c.AppDate)
            .HasForeignKey(c => c.AppDateId);
    }
}
