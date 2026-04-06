using Microsoft.Extensions.DependencyInjection;

namespace SharedKernel.BulkInsert.Extensions;

public static class FluentBulkInsertServiceCollectionExtensions
{
    public static FluentBulkInsertBuilder AddFluentBulkInsert(
        this IServiceCollection services
    )
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddPostgreSqlBulkInsert();

        return new FluentBulkInsertBuilder(services);
    }
}
