using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Data;

public class PermissionConfiguration : BaseDataEntityTypeConfiguration<Permission>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<Permission> builder
    )
    {
        builder.ToTable("permission", DbConfiguration.DefaultSchema);
    }
}
