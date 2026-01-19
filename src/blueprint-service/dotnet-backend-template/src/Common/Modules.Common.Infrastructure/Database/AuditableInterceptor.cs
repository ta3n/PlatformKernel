using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Modules.Common.Domain;

namespace Modules.Common.Infrastructure.Database;

public class AuditableInterceptor : SaveChangesInterceptor
{
	public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
		DbContextEventData eventData,
		InterceptionResult<int> result,
		CancellationToken cancellationToken = default
	)
	{
		var context = eventData.Context!;
		foreach (var entry in context.ChangeTracker.Entries<IAuditableEntity>())
		{
			switch (entry.State)
			{
				case EntityState.Added:
					entry.Entity.CreatedAtUtc = DateTime.UtcNow;
					break;
				case EntityState.Modified:
					entry.Entity.UpdatedAtUtc = DateTime.UtcNow;
					break;
			}
		}

		return await base.SavingChangesAsync(eventData, result, cancellationToken);
	}
}
