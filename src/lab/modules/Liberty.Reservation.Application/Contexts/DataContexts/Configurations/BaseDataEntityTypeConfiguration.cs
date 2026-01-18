using Liberty.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations;

public abstract class BaseDataEntityTypeConfiguration<TBase> : IEntityTypeConfiguration<TBase>
    where TBase : EntityData
{
    public void Configure(
        EntityTypeBuilder<TBase> builder
    )
    {
        EntityConfigure(builder);

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnOrder(1);

        builder.Property(u => u.Code).IsRequired();
        builder.HasIndex(u => u.Code).IsUnique();

        builder.HasQueryFilter(p => !p.IsDeleted);
    }

    protected abstract void EntityConfigure(
        EntityTypeBuilder<TBase> builder
    );
}
