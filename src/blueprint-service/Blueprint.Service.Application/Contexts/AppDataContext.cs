using Blueprint.Service.Base.Application.Contexts;
using SharedKernel.UnitOfWork;

namespace Blueprint.Service.Application.Contexts;

public class AppDataContext(
    DbContextOptions options
) : AppDbContextBase<AppDataContext>(options)
{

    protected override void OnModelCreating(
        ModelBuilder modelBuilder
    )
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppBaseDataContext).Assembly);
    }
}
