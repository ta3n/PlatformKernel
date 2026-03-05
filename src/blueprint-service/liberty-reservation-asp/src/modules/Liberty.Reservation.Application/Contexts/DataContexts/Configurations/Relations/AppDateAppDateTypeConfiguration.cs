using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Relations;

public class AppDateAppDateTypeConfiguration : BaseRelationEntityTypeConfiguration<AppDateAppDateType>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<AppDateAppDateType> builder
    )
    {
        builder.ToTable("app_date_app_date_type", DbConfiguration.DefaultSchema);

        // 中間テーブル登録
        builder
            .HasKey(
                c => new
                {
                    c.AppDateId,
                    c.AppDateTypeId
                }
            );

        builder
            .HasOne(sc => sc.AppDate)
            .WithMany(s => s.AppDateAppDateTypes)
            .HasForeignKey(sc => sc.AppDateId);

        builder
            .HasOne(sc => sc.AppDateType)
            .WithMany(s => s.AppDateAppDateTypes)
            .HasForeignKey(sc => sc.AppDateTypeId);
    }
}
