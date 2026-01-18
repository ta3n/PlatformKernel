using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Data;

public class CategoryConfiguration : BaseDataEntityTypeConfiguration<Category>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<Category> builder
    )
    {
        builder.ToTable("category", DbConfiguration.DefaultSchema);

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

        builder
            .HasMany(c => c.FileCategories)
            .WithOne(c => c.Category)
            .HasForeignKey(c => c.CategoryId);

        builder
            .HasMany(c => c.FacilityCategories)
            .WithOne(c => c.Category)
            .HasForeignKey(c => c.CategoryId);

        builder
            .HasMany(c => c.RoomGroupCategories)
            .WithOne(c => c.Category)
            .HasForeignKey(c => c.CategoryId);

        builder
            .HasMany(c => c.PlanCategories)
            .WithOne(c => c.Category)
            .HasForeignKey(c => c.CategoryId);

        builder
            .HasMany(c => c.OptionItemCategories)
            .WithOne(c => c.Category)
            .HasForeignKey(c => c.CategoryId);
    }
}
