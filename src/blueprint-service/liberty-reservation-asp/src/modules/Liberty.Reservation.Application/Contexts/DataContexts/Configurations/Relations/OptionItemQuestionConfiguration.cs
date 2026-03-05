using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Relations;

public class OptionItemQuestionConfiguration : BaseRelationEntityTypeConfiguration<OptionItemQuestion>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<OptionItemQuestion> builder
    )
    {
        builder.ToTable("option_item_question", DbConfiguration.DefaultSchema);

        // 中間テーブル登録
        builder.HasQueryFilter(
            p => !p.OptionItem!.IsDeleted && !p.Question!.IsDeleted
        );

        builder
            .HasKey(
                c => new
                {
                    c.OptionItemId,
                    c.QuestionId
                }
            );

        builder
            .HasOne(sc => sc.OptionItem)
            .WithMany(s => s.OptionItemQuestions)
            .HasForeignKey(sc => sc.OptionItemId);

        builder
            .HasOne(sc => sc.Question)
            .WithMany(s => s.OptionItemQuestions)
            .HasForeignKey(sc => sc.QuestionId);
    }
}
