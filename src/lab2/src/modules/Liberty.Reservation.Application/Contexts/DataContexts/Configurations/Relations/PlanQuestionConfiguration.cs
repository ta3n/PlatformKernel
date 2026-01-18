using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Relations;

public class PlanQuestionConfiguration : BaseRelationEntityTypeConfiguration<PlanQuestion>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<PlanQuestion> builder
    )
    {
        builder.ToTable("plan_question", DbConfiguration.DefaultSchema);

        builder.HasQueryFilter(
            p => !p.Plan!.IsDeleted && !p.Question!.IsDeleted
        );

        // 中間テーブル登録
        builder
            .HasKey(
                c => new
                {
                    c.PlanId,
                    c.QuestionId
                }
            );

        builder
            .HasOne(sc => sc.Question)
            .WithMany(s => s.PlanQuestions)
            .HasForeignKey(sc => sc.QuestionId);

        builder
            .HasOne(sc => sc.Plan)
            .WithMany(s => s.PlanQuestions)
            .HasForeignKey(sc => sc.PlanId);
    }
}
