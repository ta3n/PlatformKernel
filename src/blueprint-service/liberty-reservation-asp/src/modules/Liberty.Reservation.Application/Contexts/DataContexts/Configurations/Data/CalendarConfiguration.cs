using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Data;

public class CalendarConfiguration : BaseDataEntityTypeConfiguration<Calendar>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<Calendar> builder
    )
    {
        builder.ToTable("calendar", DbConfiguration.DefaultSchema);

        builder
            .HasMany(c => c.FacilityCalendars)
            .WithOne(c => c.Calendar)
            .HasForeignKey(c => c.CalendarId);

        // 中間テーブルとの関係性登録
        builder
            .HasMany(c => c.CalendarAppDateAppDateTypes)
            .WithOne(c => c.Calendar)
            .HasForeignKey(c => c.CalendarId);
    }
}
