using System.Text.Json.Serialization;
using Asp.Versioning;
using Liberty.Pagination.Binders;
using Liberty.Reservation.Site.WebAPI.Application;
using Microsoft.AspNetCore.HttpOverrides;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Liberty.Reservation.Site.WebAPI.Configurations;

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

        app.UseDefaultFiles();
        app.UseStaticFiles();

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
