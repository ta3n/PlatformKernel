using Microsoft.Extensions.Configuration;

namespace SharedKernel.ApplicationShared.Extensions;

/// <summary>
/// Provides extension methods for working with configuration related functionality.
/// </summary>
public static class ConfigurationExtension
{
    /// Retrieves and binds a configuration section to the specified model type.
    /// <typeparam name="TModel">The type of the model to bind the configuration section to. Must have a parameterless constructor.</typeparam>
    /// <param name="configuration">The configuration instance to retrieve the section from.</param>
    /// <param name="section">The name of the configuration section to bind to the model.</param>
    /// <returns>An instance of the specified model type with properties bound to the configuration values.</returns>
    public static TModel GetOptionsExt<TModel>(
        this IConfiguration configuration,
        string section
    ) where TModel : new()
    {
        var model = new TModel();
        configuration.GetSection(section).Bind(model);
        return model;
    }
}
