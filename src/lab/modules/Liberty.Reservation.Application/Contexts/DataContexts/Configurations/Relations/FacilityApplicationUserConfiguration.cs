using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Relations;

public class FacilityApplicationUserConfiguration : BaseRelationEntityTypeConfiguration<FacilityApplicationUser>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<FacilityApplicationUser> builder
    )
    {
        builder.ToTable("FacilityApplicationUser", DbConfiguration.DefaultSchema);

        builder.Property(x => x.FacilityId).HasColumnName("FacilityID");
        builder.Property(x => x.UserId).HasColumnName("UserID");

        // 中間テーブル登録
        builder
            .HasKey(
                c => new
                {
                    c.FacilityId,
                    c.UserId
                }
            );

        builder
            .HasOne(sc => sc.Facility)
            .WithMany(s => s.FacilityApplicationUsers)
            .HasForeignKey(sc => sc.FacilityId);

        builder
            .HasOne(sc => sc.User)
            .WithMany(s => s.FacilityApplicationUsers)
            .HasForeignKey(sc => sc.UserId);
    }
}
