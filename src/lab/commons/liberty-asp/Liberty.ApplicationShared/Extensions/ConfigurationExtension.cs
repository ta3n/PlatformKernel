using Microsoft.Extensions.Configuration;

namespace Liberty.ApplicationShared.Extensions;

public static class ConfigurationExtension
{
    public static TModel GetOptionsExt<TModel>(
        this IConfiguration configuration,
        string section
    ) where TModel : new()
    {
        var model = new TModel();
        configuration.GetSection(section).Bind(model);
        return model;
    }

    // public static IServiceCollection AddValidatorsExt(
    //     this IServiceCollection services,
    //     params Assembly[] validatorAssemblies
    // )
    // {
    //     return services.Scan(
    //         scan => scan
    //             .FromAssemblies(validatorAssemblies)
    //             .AddClasses(c => c.AssignableTo(typeof(IValidator<>)))
    //             .AsImplementedInterfaces()
    //             .WithTransientLifetime()
    //     );
    // }
}
