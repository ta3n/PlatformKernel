using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Relations;

public class FacilityCalendarConfiguration : BaseRelationEntityTypeConfiguration<FacilityCalendar>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<FacilityCalendar> builder
    )
    {
        builder.ToTable("facility_calendar", DbConfiguration.DefaultSchema);

        // 中間テーブル登録
        builder
            .HasKey(
                c => new
                {
                    c.FacilityId,
                    c.CalendarId
                }
            );

        builder
            .HasOne(sc => sc.Facility)
            .WithMany(s => s.FacilityCalendars)
            .HasForeignKey(sc => sc.FacilityId);

        builder
            .HasOne(sc => sc.Calendar)
            .WithMany(s => s.FacilityCalendars)
            .HasForeignKey(sc => sc.CalendarId);
    }
}
