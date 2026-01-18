using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Relations;

public class FacilityQuestionConfiguration : BaseRelationEntityTypeConfiguration<FacilityQuestion>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<FacilityQuestion> builder
    )
    {
        builder.ToTable("FacilityQuestion", DbConfiguration.DefaultSchema);

        builder.Property(x => x.FacilityId).HasColumnName("FacilityID");
        builder.Property(x => x.QuestionId).HasColumnName("QuestionID");

        // 中間テーブル登録
        builder
            .HasKey(
                c => new
                {
                    c.FacilityId,
                    c.QuestionId
                }
            );

        builder
            .HasOne(sc => sc.Facility)
            .WithMany(s => s.FacilityQuestions)
            .HasForeignKey(sc => sc.FacilityId);

        builder
            .HasOne(sc => sc.Question)
            .WithMany(s => s.FacilityQuestions)
            .HasForeignKey(sc => sc.QuestionId);
    }
}
