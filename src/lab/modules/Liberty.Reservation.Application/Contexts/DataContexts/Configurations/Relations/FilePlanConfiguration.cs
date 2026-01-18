using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Relations;

public class FilePlanConfiguration : BaseRelationEntityTypeConfiguration<FilePlan>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<FilePlan> builder
    )
    {
        builder.ToTable("FilePlan", DbConfiguration.DefaultSchema);

        builder.Property(x => x.FileId).HasColumnName("FileID");
        builder.Property(x => x.PlanId).HasColumnName("PlanID");

        // 中間テーブル登録
        builder
            .HasKey(
                c => new
                {
                    c.FileId,
                    c.PlanId
                }
            );

        builder
            .HasOne(sc => sc.File)
            .WithMany(s => s.FilePlans)
            .HasForeignKey(sc => sc.FileId);

        builder
            .HasOne(sc => sc.Plan)
            .WithMany(s => s.FilePlans)
            .HasForeignKey(sc => sc.PlanId);
    }
}
