using Amazon.S3;
using Liberty.Media.Options;
using Liberty.Media.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Liberty.Media;

/// <summary>
/// Provides extension methods for integrating Amazon S3 services into an application.
/// </summary>
/// <remarks>
/// This static class includes an extension method for adding AWS S3 support to the
/// dependency injection container using the provided configuration.
/// </remarks>
public static class Extensions
{
    /// <summary>
    /// Adds and configures Amazon S3 related services to the provided service collection.
    /// </summary>
    /// <param name="services">The service collection to which the Amazon S3 services will be added.</param>
    /// <param name="configuration">The application configuration, which contains the settings for Amazon S3.</param>
    /// <returns>The updated service collection with the Amazon S3 services configured.</returns>
    public static IServiceCollection AddAmazonS3(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var section = configuration.GetSection("AmazonS3");
        var awsS3Options = section.Get<AwsS3Options>();
        services.Configure<AwsS3Options>(section);

        if (awsS3Options is null)
        {
            return services;
        }

        services.AddScoped<IAmazonS3, AmazonS3Client>();
        services.AddScoped<IAwsS3Service, AwsS3Service>();
        services.AddScoped<IUploadS3Service, UploadS3Service>();

        return services;
    }
}
