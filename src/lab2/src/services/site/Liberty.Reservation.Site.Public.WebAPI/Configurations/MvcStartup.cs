using System.Text.Json.Serialization;
using Liberty.Pagination.Binders;
using Microsoft.AspNetCore.HttpOverrides;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Liberty.Reservation.Site.Public.WebAPI.Configurations;

public static class MvcStartup
{
    public static IServiceCollection AddWebModule(
        this IServiceCollection services,
        IHostEnvironment env
    )
    {
        services.AddControllers(
                options =>
                {
                    options.ModelBinderProviders.Insert(0, new PageableBinderProvider());
                }
            )
            .AddApplicationPart(typeof(AssemblyDefinition).Assembly)
            .AddControllersAsServices()
            .AddNewtonsoftJson(
                options =>
                {
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    options.SerializerSettings.Formatting = Formatting.None;
                    options.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
                }
            )
            .AddJsonOptions(
                options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                }
            );

        var apiVersioningBuilder = services.AddApiVersioning(
            options =>
            {
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ReportApiVersions = true;
                options.ApiVersionReader = ApiVersionReader.Combine(
                    new QueryStringApiVersionReader("api-version")
                    // new HeaderApiVersionReader("X-API-Version"),
                    // new MediaTypeApiVersionReader("x-version")
                );
            }
        );

        apiVersioningBuilder.AddApiExplorer(
            options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            }
        );

        services.Configure<ForwardedHeadersOptions>(
            options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;

                if (env.IsDevelopment())
                {
                    options.KnownNetworks.Clear(); // Allow all networks (dev only)
                }

                options.KnownProxies.Clear();
            }
        );

        return services;
    }

    public static IApplicationBuilder UseApplicationWeb(
        this IApplicationBuilder app,
        IHostEnvironment env
    )
    {
        // Configure the HTTP request pipeline.
        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseForwardedHeaders();

        app.UseRouting();
        app.UseRateLimiter();
        app.UseAuthorization();
        app.UseEndpoints(
            endpoints =>
            {
                endpoints
                    .MapControllers()
                    .RequirePerIpRateLimit();
            }
        );

        return app;
    }
}

internal sealed class IgnoreIdContractResolver : CamelCasePropertyNamesContractResolver
{
    private readonly HashSet<string> _namesToIgnore;
    private readonly StringComparison _comparison;

    /// <summary>
    /// Creates a contract resolver that applies camelCase naming and ignores properties with specified names.
    /// </summary>
    /// <param name="namesToIgnore">Property names to ignore (case-insensitive by default). If null or empty, defaults to { "id" }.</param>
    /// <param name="comparison">String comparison to use when matching property names. Defaults to OrdinalIgnoreCase.</param>
    public IgnoreIdContractResolver(
        IEnumerable<string>? namesToIgnore = null,
        StringComparison comparison = StringComparison.OrdinalIgnoreCase
    )
    {
        _comparison = comparison;
        var toIgnore = namesToIgnore as string[] ?? [.. namesToIgnore!];
        _namesToIgnore = toIgnore.Length != 0
            ? [..toIgnore]
            : ["id"];
    }

    protected override JsonProperty CreateProperty(
        System.Reflection.MemberInfo member,
        MemberSerialization memberSerialization
    )
    {
        var prop = base.CreateProperty(member, memberSerialization);

        // PropertyName is already camel-cased by the base resolver.
        if (_namesToIgnore.Any(n => string.Equals(prop.PropertyName, n, _comparison)))
        {
            prop.Ignored = true;
        }

        return prop;
    }
}
