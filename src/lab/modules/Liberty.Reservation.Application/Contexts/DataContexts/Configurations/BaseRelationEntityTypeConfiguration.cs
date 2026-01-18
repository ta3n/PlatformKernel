using Liberty.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations;

public abstract class BaseRelationEntityTypeConfiguration<TBase> : IEntityTypeConfiguration<TBase>
    where TBase : EntityRelation
{
    public virtual void Configure(
        EntityTypeBuilder<TBase> builder
    )
    {
        EntityConfigure(builder);

        builder.HasQueryFilter(p => !p.IsDeleted);
    }

    protected abstract void EntityConfigure(
        EntityTypeBuilder<TBase> builder
    );
}
