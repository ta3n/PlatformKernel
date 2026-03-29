using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SharedKernel.BulkInsertPipeline.Observability;
using SharedKernel.BulkInsertPipeline.Options;
using SharedKernel.BulkInsertPipeline.Providers;
using SharedKernel.BulkInsertPipeline.Providers.Dapper;
using SharedKernel.BulkInsertPipeline.Providers.EFCore;
using SharedKernel.BulkInsertPipeline.Providers.Npgsql;
using SharedKernel.BulkInsertPipeline.Providers.RepoDb;

namespace SharedKernel.BulkInsertPipeline.Extensions;

public static class ServiceCollectionExtensions
{
    public static BulkInsertBuilder AddBulkInsert(
        this IServiceCollection services,
        Action<BulkInsertOptions>? configure = null
    )
    {
        var options = new BulkInsertOptions();
        configure?.Invoke(options);

        services.TryAddSingleton(options);
        services.TryAddSingleton<BulkInsertTelemetry>();
        services.TryAddSingleton(typeof(IBulkInsertFailureSink<>), typeof(NoopBulkInsertFailureSink<>));

        services.TryAddSingleton(typeof(NpgsqlBulkInsertConnectionFactory<>));

        services.TryAddSingleton(typeof(NpgsqlBinaryCopyBulkInsertService<>));
        services.TryAddSingleton(typeof(DapperBulkInsertService<>));
        services.TryAddSingleton(typeof(EfCoreBulkInsertService<>));
        services.TryAddSingleton(typeof(RepoDbBulkInsertService<>));

        services.TryAddEnumerable(
            ServiceDescriptor.Singleton(typeof(IBulkInsertProvider<>), typeof(NpgsqlBinaryCopyBulkInsertService<>))
        );
        services.TryAddEnumerable(
            ServiceDescriptor.Singleton(typeof(IBulkInsertProvider<>), typeof(DapperBulkInsertService<>))
        );
        services.TryAddEnumerable(
            ServiceDescriptor.Singleton(typeof(IBulkInsertProvider<>), typeof(EfCoreBulkInsertService<>))
        );
        services.TryAddEnumerable(
            ServiceDescriptor.Singleton(typeof(IBulkInsertProvider<>), typeof(RepoDbBulkInsertService<>))
        );

        services.TryAddSingleton(typeof(IBulkInsertService<>), typeof(DispatchingBulkInsertService<>));

        return new BulkInsertBuilder(services);
    }
}
