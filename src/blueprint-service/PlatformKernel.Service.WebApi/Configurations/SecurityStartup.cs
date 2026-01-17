using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Blueprint.Service.Application.Auth;
using Blueprint.Service.WebApi.Application.Settings;
using SharedKernel.ApplicationShared.Extensions;

namespace PlatformKernel.Service.WebApi.Configurations;

public static class SecurityStartup
{
    public static IServiceCollection AddSecurityModule(
        this IServiceCollection services,
        IConfiguration config
    )
    {
        var identitySetting = config.GetOptionsExt<IdentitySetting>("Identity");

        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

        services
            .AddAuthentication(
                options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                }
            )
            .AddJwtBearer(
                options =>
                {
                    options.Authority = identitySetting.Jwt?.Authority;
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateLifetime = true,
                        ValidateIssuer = false,
                        ValidateIssuerSigningKey = false,
                        ValidateAudience = false
                    };
                    options.IncludeErrorDetails = true;
                    options.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            var logFactory = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>();
                            var logger = logFactory.CreateLogger(nameof(Program));
                            logger.LogDebug("Authenticated failed: {Message}", context.Exception.Message);
                            return Task.CompletedTask;
                        }
                    };
                }
            );

        var webCorsSetting = config.GetOptionsExt<WebCorsSetting>("Cors");
        services.AddCors(
            options =>
            {
                options.AddPolicy(
                    webCorsSetting.PolicyName!,
                    builder =>
                    {
                        var allowAnyOrigin = webCorsSetting.AllowedOrigins!.Equals("*");

                        if (allowAnyOrigin)
                        {
                            // builder.AllowAnyOrigin();
                        }
                        else
                        {
                            builder.WithOrigins(webCorsSetting.GetAllowedOrigins());

                            if (webCorsSetting.AllowCredentials)
                            {
                                builder.AllowCredentials();
                            }
                            else
                            {
                                builder.DisallowCredentials();
                            }
                        }

                        builder
                            .WithOrigins(webCorsSetting.GetAllowedOrigins())
                            .WithMethods(webCorsSetting.GetAllowedMethods())
                            .WithHeaders(webCorsSetting.GetAllowedHeaders())
                            .WithExposedHeaders(webCorsSetting.GetExposedHeaders())
                            .SetPreflightMaxAge(TimeSpan.FromSeconds(webCorsSetting.MaxAge));
                    }
                );
            }
        );

        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddSingleton<ISecurityContextAccessor, SecurityContextAccessor>();
        services.AddHttpContextAccessor();
        services.AddAuthorization();

        return services;
    }

    public static IApplicationBuilder UseApplicationSecurity(
        this IApplicationBuilder app,
        IConfiguration config
    )
    {
        var webCorsSetting = config.GetOptionsExt<WebCorsSetting>("Cors");

        app.UseCors(webCorsSetting.PolicyName!);

        app.UseAuthentication();
        app.UseAuthorization();

        if (webCorsSetting.EnforceHttps)
        {
            app.UseHsts();
            app.UseHttpsRedirection();
        }

        return app;
    }
}
