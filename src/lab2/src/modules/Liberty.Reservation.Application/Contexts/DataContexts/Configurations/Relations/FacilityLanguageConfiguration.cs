using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Relations;

public class FacilityLanguageConfiguration : BaseRelationEntityTypeConfiguration<FacilityLanguage>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<FacilityLanguage> builder
    )
    {
        builder.ToTable("facility_language", DbConfiguration.DefaultSchema);

        builder.HasKey(
            x => new
            {
                x.FacilityId,
                x.LanguageId
            }
        );
    }
}
