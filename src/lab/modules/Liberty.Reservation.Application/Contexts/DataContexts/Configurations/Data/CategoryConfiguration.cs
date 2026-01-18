using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Data;

public class CategoryConfiguration : BaseDataEntityTypeConfiguration<Category>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<Category> builder
    )
    {
        builder.ToTable("Category", DbConfiguration.DefaultSchema);

        builder
            .HasMany(c => c.FileCategories)
            .WithOne(c => c.Category)
            .HasForeignKey(c => c.CategoryId);

        builder
            .HasMany(c => c.FacilityCategoryies)
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
