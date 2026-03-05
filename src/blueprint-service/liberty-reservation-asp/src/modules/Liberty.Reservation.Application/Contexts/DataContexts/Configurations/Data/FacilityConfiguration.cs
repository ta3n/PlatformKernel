using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Metas;
using Liberty.Reservation.Application.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Data;

public class FacilityConfiguration : BaseDataEntityTypeConfiguration<Facility>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<Facility> builder
    )
    {
        builder.ToTable("facility", DbConfiguration.DefaultSchema);

        builder.Property(a => a.Id)
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(e => e.Meta)
            .HasColumnType(JsonbType)
            .HasDefaultValue(new FacilityMeta())
            .HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<FacilityMeta>(v)
            );

        builder
            .HasIndex(x => x.Meta)
            .HasMethod("GIN");

        builder.Property(e => e.Name)
            .HasColumnType(JsonbType)
            .HasConversion(MultilingualTextConverter())
            .Metadata.SetValueComparer(typeof(MultilingualTextComparer));

        builder.Property(e => e.Name)
            .HasDefaultValueSql(JsonbDefaultValue);

        builder
            .HasIndex(x => x.Name)
            .HasMethod("GIN");

        builder.Property(e => e.Address1)
            .HasColumnType(JsonbType)
            .HasConversion(MultilingualTextConverter())
            .Metadata.SetValueComparer(typeof(MultilingualTextComparer));

        builder.Property(e => e.Address1)
            .HasDefaultValueSql(JsonbDefaultValue);

        builder
            .HasIndex(x => x.Address1)
            .HasMethod("GIN");

        builder.Property(e => e.Address2)
            .HasColumnType(JsonbType)
            .HasConversion(MultilingualTextConverter())
            .Metadata.SetValueComparer(typeof(MultilingualTextComparer));

        builder.Property(e => e.Address2)
            .HasDefaultValueSql(JsonbDefaultValue);

        builder
            .HasIndex(x => x.Address2)
            .HasMethod("GIN");

        builder.Property(e => e.Address3)
            .HasColumnType(JsonbType)
            .HasConversion(MultilingualTextConverter())
            .Metadata.SetValueComparer(typeof(MultilingualTextComparer));

        builder.Property(e => e.Address3)
            .HasDefaultValueSql(JsonbDefaultValue);

        builder
            .HasIndex(x => x.Address3)
            .HasMethod("GIN");

        builder.Property(e => e.Address4)
            .HasColumnType(JsonbType)
            .HasConversion(MultilingualTextConverter())
            .Metadata.SetValueComparer(typeof(MultilingualTextComparer));

        builder.Property(e => e.Address4)
            .HasDefaultValueSql(JsonbDefaultValue);

        builder
            .HasIndex(x => x.Address4)
            .HasMethod("GIN");

        builder.Property(e => e.Heading1)
            .HasColumnType(JsonbType)
            .HasConversion(MultilingualTextConverter())
            .Metadata.SetValueComparer(typeof(MultilingualTextComparer));

        builder.Property(e => e.Heading1)
            .HasDefaultValueSql(JsonbDefaultValue);

        builder
            .HasIndex(x => x.Heading1)
            .HasMethod("GIN");

        builder.Property(e => e.SpaTaxComment)
            .HasColumnType(JsonbType)
            .HasConversion(MultilingualTextConverter())
            .Metadata.SetValueComparer(typeof(MultilingualTextComparer));

        builder.Property(e => e.SpaTaxComment)
            .HasDefaultValueSql(JsonbDefaultValue);

        builder
            .HasIndex(x => x.SpaTaxComment)
            .HasMethod("GIN");

        builder.Property(e => e.SpaTaxTable)
            .HasColumnType(JsonbType)
            .HasConversion(MultilingualTextConverter())
            .Metadata.SetValueComparer(typeof(MultilingualTextComparer));

        builder.Property(e => e.SpaTaxTable)
            .HasDefaultValueSql(JsonbDefaultValue);

        builder
            .HasIndex(x => x.SpaTaxTable)
            .HasMethod("GIN");

        builder.Property(e => e.BarrierFreeInfoComment)
            .HasColumnType(JsonbType)
            .HasConversion(MultilingualTextConverter())
            .Metadata.SetValueComparer(typeof(MultilingualTextComparer));

        builder.Property(e => e.BarrierFreeInfoComment)
            .HasDefaultValueSql(JsonbDefaultValue);

        builder
            .HasIndex(x => x.BarrierFreeInfoComment)
            .HasMethod("GIN");

        builder.Property(e => e.AccessInfoComment)
            .HasColumnType(JsonbType)
            .HasConversion(MultilingualTextConverter())
            .Metadata.SetValueComparer(typeof(MultilingualTextComparer));

        builder.Property(e => e.AccessInfoComment)
            .HasDefaultValueSql(JsonbDefaultValue);

        builder
            .HasIndex(x => x.AccessInfoComment)
            .HasMethod("GIN");

        builder.Property(e => e.ParkingInfoComment)
            .HasColumnType(JsonbType)
            .HasConversion(MultilingualTextConverter())
            .Metadata.SetValueComparer(typeof(MultilingualTextComparer));

        builder.Property(e => e.ParkingInfoComment)
            .HasDefaultValueSql(JsonbDefaultValue);

        builder
            .HasIndex(x => x.ParkingInfoComment)
            .HasMethod("GIN");

        builder.Property(e => e.TransferComment)
            .HasColumnType(JsonbType)
            .HasConversion(MultilingualTextConverter())
            .Metadata.SetValueComparer(typeof(MultilingualTextComparer));

        builder.Property(e => e.TransferComment)
            .HasDefaultValueSql(JsonbDefaultValue);

        builder
            .HasIndex(x => x.TransferComment)
            .HasMethod("GIN");

        builder.Property(e => e.NearStationInfoComment)
            .HasColumnType(JsonbType)
            .HasConversion(MultilingualTextConverter())
            .Metadata.SetValueComparer(typeof(MultilingualTextComparer));

        builder.Property(e => e.NearStationInfoComment)
            .HasDefaultValueSql(JsonbDefaultValue);

        builder
            .HasIndex(x => x.NearStationInfoComment)
            .HasMethod("GIN");

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
