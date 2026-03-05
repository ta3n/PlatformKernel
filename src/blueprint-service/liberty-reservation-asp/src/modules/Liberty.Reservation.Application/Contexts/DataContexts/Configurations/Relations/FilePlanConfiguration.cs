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
        builder.ToTable("file_plan", DbConfiguration.DefaultSchema);

        // 中間テーブル登録
        builder
            .HasKey(
                c => new
                {
                    c.FileId,
                    c.PlanId,
                    c.Index
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
