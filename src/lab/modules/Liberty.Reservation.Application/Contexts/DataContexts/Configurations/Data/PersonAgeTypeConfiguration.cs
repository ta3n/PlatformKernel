using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Data;

public class PersonAgeTypeConfiguration : BaseDataEntityTypeConfiguration<PersonAgeType>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<PersonAgeType> builder
    )
    {
        builder.ToTable("PersonAgeType", DbConfiguration.DefaultSchema);

        builder.Ignore(t => t.Meta);

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
