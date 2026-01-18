using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Data;

public class FacilityConfiguration : BaseDataEntityTypeConfiguration<Facility>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<Facility> builder
    )
    {
        builder.ToTable("Facility", DbConfiguration.DefaultSchema);

        builder.Ignore(t => t.Meta);

        // 中間テーブルとの関係性登録
        builder
            .HasMany(c => c.FacilityAppDateTypes)
            .WithOne(c => c.Facility)
            .HasForeignKey(c => c.FacilityId);

        // 中間テーブルとの関係性登録
        builder
            .HasMany(c => c.FacilityCalendars)
            .WithOne(c => c.Facility)
            .HasForeignKey(c => c.FacilityId);

        // 中間テーブルとの関係性登録
        builder
            .HasMany(c => c.FacilityApplicationUsers)
            .WithOne(c => c.Facility)
            .HasForeignKey(c => c.FacilityId);

        // 中間テーブルとの関係性登録
        builder
            .HasMany(c => c.FacilityCategories)
            .WithOne(c => c.Facility)
            .HasForeignKey(c => c.FacilityId);

        // 中間テーブルとの関係性登録
        builder
            .HasMany(c => c.FacilitySpaTaxGroups)
            .WithOne(c => c.Facility)
            .HasForeignKey(c => c.FacilityId);

        // 中間テーブルとの関係性登録
        builder
            .HasMany(c => c.FacilityPersonAgeTypes)
            .WithOne(c => c.Facility)
            .HasForeignKey(c => c.FacilityId);

        // 中間テーブルとの関係性登録
        builder
            .HasMany(c => c.FacilityFiles)
            .WithOne(c => c.Facility)
            .HasForeignKey(c => c.FacilityId);

        // 中間テーブルとの関係性登録
        builder
            .HasMany(c => c.FacilitySites)
            .WithOne(c => c.Facility)
            .HasForeignKey(c => c.FacilityId);

        // 中間テーブルとの関係性登録
        builder
            .HasMany(c => c.FacilityAllergens)
            .WithOne(c => c.Facility)
            .HasForeignKey(c => c.FacilityId);

        // 中間テーブルとの関係性登録
        builder
            .HasMany(c => c.FacilityOptionItems)
            .WithOne(c => c.Facility)
            .HasForeignKey(c => c.FacilityId);

        // 中間テーブルとの関係性登録
        builder
            .HasMany(c => c.FacilityQuestions)
            .WithOne(c => c.Facility)
            .HasForeignKey(c => c.FacilityId);

        // 中間テーブルとの関係性登録
        builder
            .HasMany(c => c.FacilityCancellations)
            .WithOne(c => c.Facility)
            .HasForeignKey(c => c.FacilityId);

        // 中間テーブルとの関係性登録
        builder
            .HasMany(c => c.FacilityFaxServices)
            .WithOne(c => c.Facility)
            .HasForeignKey(c => c.FacilityId);

        builder
            .HasMany(c => c.FacilityRoomGroups)
            .WithOne(c => c.Facility)
            .HasForeignKey(c => c.FacilityId);

        builder
            .HasMany(c => c.FacilityPlans)
            .WithOne(c => c.Facility)
            .HasForeignKey(c => c.FacilityId);
    }
}
