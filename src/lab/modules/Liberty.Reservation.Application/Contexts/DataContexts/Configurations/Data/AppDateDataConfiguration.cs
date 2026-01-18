using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Data;

public class AppDateDataConfiguration : BaseDataEntityTypeConfiguration<AppDateData>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<AppDateData> builder
    )
    {
        builder.ToTable("AppDateData", DbConfiguration.DefaultSchema);

        builder.Property(a => a.Id);

        // 中間テーブルとの関係性登録

        builder
            .HasMany(c => c.AppDateAppDateData)
            .WithOne(c => c.AppDateData)
            .HasForeignKey(c => c.AppDateDataId);
    }
}
