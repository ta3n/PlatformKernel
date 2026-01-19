using Microsoft.Extensions.DependencyInjection;

namespace Modules.Common.Infrastructure.Database;

public static class DatabaseMigrationExtensions
{
	public static async Task MigrateModuleDatabasesAsync(
		this IServiceScope scope,
		CancellationToken cancellationToken = default
	)
	{
		foreach (var migrator in scope.ServiceProvider.GetRequiredService<IEnumerable<IModuleDatabaseMigrator>>())
		{
			await migrator.MigrateAsync(scope, cancellationToken);
		}
	}
}
