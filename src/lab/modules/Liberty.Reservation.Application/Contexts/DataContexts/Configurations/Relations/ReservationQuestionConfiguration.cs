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
        builder.ToTable("ReservationQuestion", DbConfiguration.DefaultSchema);

        // 中間テーブル登録
        builder
            .HasKey(
                c => new
                {
                    ReservationID = c.ReservationId,
                    QuestionID = c.QuestionId,
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
