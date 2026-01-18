using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Data;

public class UserInfoConfiguration : BaseDataEntityTypeConfiguration<UserInfo>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<UserInfo> builder
    )
    {
        builder.ToTable("UserInfo", DbConfiguration.DefaultSchema);
    }
}
