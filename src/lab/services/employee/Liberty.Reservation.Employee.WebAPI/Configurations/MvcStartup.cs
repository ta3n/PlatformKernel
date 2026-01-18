using Asp.Versioning;
using Liberty.Pagination.Binders;
using Liberty.Reservation.Employee.WebAPI.Application;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Liberty.Reservation.Employee.WebAPI.Configurations;

public static class MvcStartup
{
    public static IServiceCollection AddWebModule(
        this IServiceCollection services
    )
    {
        services.AddHealthChecks();

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
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    options.SerializerSettings.Formatting = Formatting.Indented;
                    options.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
                }
            );

        var apiVersioningBuilder = services.AddApiVersioning(
            options =>
            {
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ReportApiVersions = true;
                options.ApiVersionReader = ApiVersionReader.Combine(
                    new QueryStringApiVersionReader("api-version"),
                    new HeaderApiVersionReader("X-API-Version"),
                    new MediaTypeApiVersionReader("x-version")
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

        // app.UseHttpsRedirection();
        // app.UseCors(config.Web.Cors.PolicyName);

        app.UseRouting();
        app.UseAuthorization();
        app
            .UseEndpoints(
                endpoints =>
                {
                    endpoints.MapControllers().RequirePerUserRateLimit();
                }
            );

        app.UseHealthChecks("/health");

        return app;
    }
}
