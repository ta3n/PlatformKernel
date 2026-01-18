using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Relations;

public class FacilityFileConfiguration : BaseRelationEntityTypeConfiguration<FacilityFile>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<FacilityFile> builder
    )
    {
        builder.ToTable("FacilityFile", DbConfiguration.DefaultSchema);

        builder.Property(x => x.FacilityId).HasColumnName("FacilityID");
        builder.Property(x => x.FileId).HasColumnName("FileID");

        // 中間テーブル登録
        builder
            .HasKey(
                c => new
                {
                    c.FacilityId,
                    c.FileId
                }
            );

        builder
            .HasOne(sc => sc.Facility)
            .WithMany(s => s.FacilityFiles)
            .HasForeignKey(sc => sc.FacilityId);

        builder
            .HasOne(sc => sc.File)
            .WithMany(s => s.FacilityFiles)
            .HasForeignKey(sc => sc.FileId);
    }
}
