using Asp.Versioning.ApiExplorer;
using Liberty.Application.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Liberty.Application.Swaggers
{
    public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
    {
        private readonly IApiVersionDescriptionProvider provider;
        private readonly IConfiguration configuration;

        public ConfigureSwaggerOptions(
            IApiVersionDescriptionProvider provider,
            IConfiguration configuration)
        {
            this.provider = provider;
            this.configuration = configuration;
        }

        public void Configure(SwaggerGenOptions options)
        {
            var setting = configuration.Get<AppSetting>();

            foreach (var description in provider.ApiVersionDescriptions)
            {
                options.SwaggerDoc(description.GroupName, new OpenApiInfo
                {
                    Title = $"{setting.App.AppName}  - {description.ApiVersion}",
                    Version = description.ApiVersion.ToString(),
                    
                    Description = $"appversion: {setting.App.AppVersion}",
                    //TermsOfService = new Uri("https://example.com/terms"),
                    //Contact = new OpenApiContact
                    //{
                    //    Name = "Example Contact",
                    //    Url = new Uri("https://example.com/contact")
                    //},
                    //License = new OpenApiLicense
                    //{
                    //    Name = "Example License",
                    //    Url = new Uri("https://example.com/license")
                    //}
                });
            }
        }


    }

}
