using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Metas;
using Liberty.Reservation.Application.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Data;

public class PersonAgeTypeConfiguration : BaseDataEntityTypeConfiguration<PersonAgeType>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<PersonAgeType> builder
    )
    {
        builder.ToTable("person_age_type", DbConfiguration.DefaultSchema);

        builder.Property(e => e.Meta)
            .HasColumnType("jsonb")
            .HasDefaultValue(new PersonAgeTypeMeta())
            .HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<PersonAgeTypeMeta>(v)
            );

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
            .HasMany(c => c.FacilityPersonAgeTypes)
            .WithOne(c => c.PersonAgeType)
            .HasForeignKey(c => c.PersonAgeTypeId);

        builder
            .HasMany(c => c.PlanRoomGroupSitePersonAgeTypes)
            .WithOne(c => c.PersonAgeType)
            .HasForeignKey(c => c.PersonAgeTypeId);

        builder
            .HasMany(c => c.PersonAgeTypeSpaTaxDatas)
            .WithOne(c => c.PersonAgeType)
            .HasForeignKey(c => c.PersonAgeTypeId);

        builder
            .HasMany(c => c.ReservationRoomGroupAppDatePersonAgeTypes)
            .WithOne(c => c.PersonAgeType)
            .HasForeignKey(c => c.PersonAgeTypeId);
    }
}
