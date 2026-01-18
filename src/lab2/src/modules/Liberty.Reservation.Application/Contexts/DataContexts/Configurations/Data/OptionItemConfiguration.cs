using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Data;

public class OptionItemConfiguration : BaseDataEntityTypeConfiguration<OptionItem>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<OptionItem> builder
    )
    {
        builder.ToTable("option_item", DbConfiguration.DefaultSchema);

        builder.Property(e => e.Name)
            .HasColumnType("jsonb")
            .HasConversion(MultilingualTextConverter())
            .Metadata.SetValueComparer(typeof(MultilingualTextComparer));

        builder.Property(e => e.Name)
            .HasDefaultValueSql("'{}'::jsonb");

        builder
            .HasIndex(x => x.Name)
            .HasMethod("GIN");

        builder
            .HasMany(c => c.FacilityOptionItems)
            .WithOne(c => c.OptionItem)
            .HasForeignKey(c => c.OptionItemId);

        builder
            .HasMany(c => c.PlanOptionItems)
            .WithOne(c => c.OptionItem)
            .HasForeignKey(c => c.OptionItemId);

        builder
            .HasMany(c => c.FileOptionItems)
            .WithOne(c => c.OptionItem)
            .HasForeignKey(c => c.OptionItemId);

        builder
            .HasMany(c => c.OptionItemQuestions)
            .WithOne(c => c.OptionItem)
            .HasForeignKey(c => c.OptionItemId);

        builder
            .HasMany(c => c.OptionItemCategories)
            .WithOne(c => c.OptionItem)
            .HasForeignKey(c => c.OptionItemId);

        builder
            .HasMany(c => c.OptionItemAppDates)
            .WithOne(c => c.OptionItem)
            .HasForeignKey(c => c.OptionItemId);

        builder
            .Property(o => o.MaxSupplyNumber)
            .HasDefaultValue(10);
    }
}
