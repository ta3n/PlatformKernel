using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Relations;

public class CalendarAppDateAppDateTypeConfiguration : BaseRelationEntityTypeConfiguration<CalendarAppDateAppDateType>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<CalendarAppDateAppDateType> builder
    )
    {
        builder.ToTable("calendar_app_date_app_date_type", DbConfiguration.DefaultSchema);

        // 中間テーブル登録
        builder
            .HasKey(
                c => new
                {
                    c.CalendarId,
                    //c.AppDateId,
                    c.DateCalendar,
                    c.AppDateTypeId
                }
            );

        builder
            .HasOne(sc => sc.Calendar)
            .WithMany(s => s.CalendarAppDateAppDateTypes)
            .HasForeignKey(sc => sc.CalendarId);

        //builder
        //    .HasOne(sc => sc.AppDate)
        //    .WithMany(s => s.CalendarAppDateAppDateTypes)
        //    .HasForeignKey(sc => sc.AppDateId);

        builder
            .HasOne(sc => sc.AppDateType)
            .WithMany(s => s.CalendarAppDateAppDateTypes)
            .HasForeignKey(sc => sc.AppDateTypeId);
    }
}
