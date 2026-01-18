using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Data;

public class CancellationConfiguration : BaseDataEntityTypeConfiguration<Cancellation>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<Cancellation> builder
    )
    {
        builder.ToTable("Cancellation", DbConfiguration.DefaultSchema);

        builder.Ignore(t => t.Meta);

        // 中間テーブルとの関係性登録
        builder
            .HasMany(c => c.CancellationCancellationDatas)
            .WithOne(c => c.Cancellation)
            .HasForeignKey(c => c.CancellationId);

        builder
            .HasMany(c => c.FacilityCancellations)
            .WithOne(c => c.Cancellation)
            .HasForeignKey(c => c.CancellationId);

        builder
            .HasMany(c => c.PlanRoomGroupCancellations)
            .WithOne(c => c.Cancellation)
            .HasForeignKey(c => c.CancellationId);
    }
}
