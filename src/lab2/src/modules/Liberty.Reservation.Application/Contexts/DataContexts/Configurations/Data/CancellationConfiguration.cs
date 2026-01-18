using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Data;

public class CancellationConfiguration : BaseDataEntityTypeConfiguration<Cancellation>
{
    private const string JsonBType = "jsonb";
    private const string JsonBDefaultValue = "'{}'::jsonb";

    protected override void EntityConfigure(
        EntityTypeBuilder<Cancellation> builder
    )
    {
        builder.ToTable("cancellation", DbConfiguration.DefaultSchema);

        builder.Property(e => e.Name)
            .HasColumnType(JsonBType)
            .HasConversion(MultilingualTextConverter())
            .Metadata.SetValueComparer(typeof(MultilingualTextComparer));

        builder.Property(e => e.Name)
            .HasDefaultValueSql(JsonBDefaultValue);

        builder
            .HasIndex(x => x.Name)
            .HasMethod("GIN");

        builder.Property(e => e.Description)
            .HasColumnType(JsonBType)
            .HasConversion(MultilingualTextConverter())
            .Metadata.SetValueComparer(typeof(MultilingualTextComparer));

        builder.Property(e => e.Description)
            .HasDefaultValueSql(JsonBDefaultValue);

        builder
            .HasIndex(x => x.Description)
            .HasMethod("GIN");

        builder.Property(e => e.RuleDetail)
            .HasColumnType(JsonBType)
            .HasConversion(MultilingualTextConverter())
            .Metadata.SetValueComparer(typeof(MultilingualTextComparer));

        builder.Property(e => e.RuleDetail)
            .HasDefaultValueSql(JsonBDefaultValue);

        builder
            .HasIndex(x => x.RuleDetail)
            .HasMethod("GIN");

        builder.Property(e => e.TableSource)
            .HasColumnType(JsonBType)
            .HasConversion(MultilingualTextConverter())
            .Metadata.SetValueComparer(typeof(MultilingualTextComparer));

        builder.Property(e => e.TableSource)
            .HasDefaultValueSql(JsonBDefaultValue);

        builder
            .HasIndex(x => x.TableSource)
            .HasMethod("GIN");

        // 中間テーブルとの関係性登録
        builder
            .HasMany(c => c.CancellationCancellationDatas)
            .WithOne(c => c.Cancellation)
            .HasForeignKey(c => c.CancellationId);

        builder
            .HasMany(c => c.FacilityCancellations)
            .WithOne(c => c.Cancellation)
            .HasForeignKey(c => c.CancellationId);

        builder
            .HasMany(c => c.PlanRoomGroupCancellations)
            .WithOne(c => c.Cancellation)
            .HasForeignKey(c => c.CancellationId);
    }
}
