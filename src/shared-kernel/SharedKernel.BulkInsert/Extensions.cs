using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SharedKernel.BulkInsert.Abstractions;
using SharedKernel.BulkInsert.Internal;
using SharedKernel.BulkInsert.Services;

namespace SharedKernel.BulkInsert;

public static class Extensions
{
    public static IServiceCollection AddPostgreSqlBulkInsert(
        this IServiceCollection services
    )
    {
        services.TryAddSingleton<IPostgreSqlBulkInsertMetadataResolver, EfCorePostgreSqlBulkInsertMetadataResolver>();
        services.TryAddSingleton<IPostgreSqlBulkInsertService, PostgreSqlBulkInsertService>();

        services.TryAddEnumerable(
            ServiceDescriptor.Singleton<IPostgreSqlBulkInsertStrategy, EfCorePostgreSqlBulkInsertStrategy>()
        );
        services.TryAddEnumerable(
            ServiceDescriptor.Singleton<IPostgreSqlBulkInsertStrategy, DapperPostgreSqlBulkInsertStrategy>()
        );
        services.TryAddEnumerable(
            ServiceDescriptor.Singleton<IPostgreSqlBulkInsertStrategy, RepoDbPostgreSqlBulkInsertStrategy>()
        );

        return services;
    }
}
