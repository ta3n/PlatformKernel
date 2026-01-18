using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Relations;

public class OptionItemAppDateConfiguration : BaseRelationEntityTypeConfiguration<OptionItemAppDate>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<OptionItemAppDate> builder
    )
    {
        builder.ToTable("OptionItemAppDate", DbConfiguration.DefaultSchema);

        builder.Property(x => x.OptionItemId).HasColumnName("OptionItemID");
        builder.Property(x => x.AppDateId).HasColumnName("AppDateID");

        // 中間テーブル登録
        builder
            .HasKey(
                c => new
                {
                    c.OptionItemId,
                    c.AppDateId
                }
            );

        builder
            .HasOne(sc => sc.OptionItem)
            .WithMany(s => s.OptionItemAppDates)
            .HasForeignKey(sc => sc.OptionItemId);

        builder
            .HasOne(sc => sc.AppDate)
            .WithMany(s => s.OptionItemAppDates)
            .HasForeignKey(sc => sc.AppDateId);

        builder
            .HasMany(c => c.ReservationRoomGroupAppDateOptionItems)
            .WithOne(c => c.OptionItemAppDate)
            .HasForeignKey(
                sc => new
                {
                    sc.OptionItemId,
                    sc.AppDateId
                }
            );
    }
}
