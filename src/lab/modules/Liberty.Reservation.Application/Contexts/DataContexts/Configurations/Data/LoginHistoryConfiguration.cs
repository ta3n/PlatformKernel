using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Data;

public class LoginHistoryConfiguration : BaseDataEntityTypeConfiguration<LoginHistory>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<LoginHistory> builder
    )
    {
        builder.ToTable("LoginHistory", DbConfiguration.DefaultSchema);

        builder.Ignore(t => t.Meta);

        builder
            .HasMany(c => c.ApplicationUserLoginHistories)
            .WithOne(c => c.LoginHistory)
            .HasForeignKey(c => c.LoginHistoryId);
    }
}
