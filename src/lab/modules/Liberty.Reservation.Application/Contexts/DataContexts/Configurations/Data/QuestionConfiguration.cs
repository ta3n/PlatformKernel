using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Data;

public class QuestionConfiguration : BaseDataEntityTypeConfiguration<Question>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<Question> builder
    )
    {
        builder.ToTable("Question", DbConfiguration.DefaultSchema);

        builder
            .HasMany(c => c.FacilityQuestions)
            .WithOne(c => c.Question)
            .HasForeignKey(c => c.QuestionId);

        builder
            .HasMany(c => c.PlanQuestions)
            .WithOne(c => c.Question)
            .HasForeignKey(c => c.QuestionId);

        builder
            .HasMany(c => c.OptionItemQuestions)
            .WithOne(c => c.Question)
            .HasForeignKey(c => c.QuestionId);

        builder
            .HasMany(c => c.ReservationQuestions)
            .WithOne(c => c.Question)
            .HasForeignKey(c => c.QuestionId);
    }
}
