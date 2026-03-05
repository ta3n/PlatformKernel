using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Data;

public class LanguageConfiguration : BaseDataEntityTypeConfiguration<Language>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<Language> builder
    )
    {
        builder.ToTable("language", DbConfiguration.DefaultSchema);
    }
}
