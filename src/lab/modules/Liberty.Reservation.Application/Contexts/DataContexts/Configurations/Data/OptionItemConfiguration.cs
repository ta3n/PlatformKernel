using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Data;

public class OptionItemConfiguration : BaseDataEntityTypeConfiguration<OptionItem>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<OptionItem> builder
    )
    {
        builder.ToTable("OptionItem", DbConfiguration.DefaultSchema);

        builder.Ignore(t => t.Meta);

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
            .HasMany(c => c.ReservationRoomGroupAppDateOptionItems)
            .WithOne(c => c.OptionItem)
            .HasForeignKey(c => c.OptionItemId);
    }
}
