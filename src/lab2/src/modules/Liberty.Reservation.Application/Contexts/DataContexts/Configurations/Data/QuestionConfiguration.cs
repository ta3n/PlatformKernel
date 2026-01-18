using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Data;

public class QuestionConfiguration : BaseDataEntityTypeConfiguration<Question>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<Question> builder
    )
    {
        builder.ToTable("question", DbConfiguration.DefaultSchema);

        builder.Property(e => e.Name)
            .HasColumnType("jsonb")
            .HasConversion(MultilingualTextConverter())
            .Metadata.SetValueComparer(typeof(MultilingualTextComparer));

        builder.Property(e => e.Name)
            .HasDefaultValueSql("'{}'::jsonb");

        builder
            .HasIndex(x => x.Name)
            .HasMethod("GIN");

        builder.Property(e => e.Description)
            .HasColumnType("jsonb")
            .HasConversion(MultilingualTextConverter())
            .Metadata.SetValueComparer(typeof(MultilingualTextComparer));

        builder.Property(e => e.Description)
            .HasDefaultValueSql("'{}'::jsonb");

        builder
            .HasIndex(x => x.Description)
            .HasMethod("GIN");

        builder.Property(e => e.FormData)
            .HasColumnType("jsonb")
            .HasConversion(MultilingualTextConverter())
            .Metadata.SetValueComparer(typeof(MultilingualTextComparer));

        builder.Property(e => e.FormData)
            .HasDefaultValueSql("'{}'::jsonb");

        builder
            .HasIndex(x => x.FormData)
            .HasMethod("GIN");

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
