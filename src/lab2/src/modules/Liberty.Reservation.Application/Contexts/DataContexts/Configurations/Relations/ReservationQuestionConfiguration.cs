using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Relations;

public class ReservationQuestionConfiguration : BaseRelationEntityTypeConfiguration<ReservationQuestion>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<ReservationQuestion> builder
    )
    {
        builder.ToTable("reservation_question", DbConfiguration.DefaultSchema);

        // 中間テーブル登録
        builder
            .HasKey(
                c => new
                {
                    c.ReservationId,
                    c.QuestionId,
                    c.ReservationQuestionType
                }
            );

        builder
            .HasOne(sc => sc.Reservation)
            .WithMany(s => s.ReservationQuestions)
            .HasForeignKey(sc => sc.ReservationId);

        builder
            .HasOne(sc => sc.Question)
            .WithMany(s => s.ReservationQuestions)
            .HasForeignKey(sc => sc.QuestionId);
    }
}
