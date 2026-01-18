using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Liberty.Application.Swaggers
{
    
    public class ConfigureSwaggerUIOptions : IConfigureOptions<SwaggerUIOptions>
    {
        private readonly IApiVersionDescriptionProvider provider;

        public ConfigureSwaggerUIOptions(IApiVersionDescriptionProvider provider)
        {
            this.provider = provider;
        }

        public void Configure(SwaggerUIOptions options)
        {
            var descriptions = provider.ApiVersionDescriptions;
            foreach (var description in descriptions)
            {
                var url = $"/swagger/{description.GroupName}/swagger.json";
                var name = description.GroupName.ToUpperInvariant();
                options.SwaggerEndpoint(url, name);
            }
        }

        //private readonly IApiVersionDescriptionProvider provider;

        //public ConfigureSwaggerUIOptions(IApiVersionDescriptionProvider provider)
        //{
        //    this.provider = provider;
        //}

        //public void Configure(SwaggerGenOptions options)
        //{
        //    foreach (var description in provider.ApiVersionDescriptions)
        //    {
        //        options.SwaggerDoc(description.GroupName, new OpenApiInfo
        //        {
        //            Title = $"API {description.ApiVersion}",
        //            Version = description.ApiVersion.ToString()
        //        });
        //    }
        //}

        public void DescribeApiVersions(SwaggerUIOptions options) => throw new NotImplementedException();

    }
}
