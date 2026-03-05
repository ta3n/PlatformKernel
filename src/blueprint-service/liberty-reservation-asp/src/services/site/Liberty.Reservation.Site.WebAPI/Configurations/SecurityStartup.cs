using Liberty.ApplicationShared.Extensions;
using Liberty.Reservation.Site.Application.Auth;
using Liberty.Reservation.Site.WebAPI.Application.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace Liberty.Reservation.Site.WebAPI.Configurations;

public static class SecurityStartup
{
    public static IServiceCollection AddSecurityModule(
        this IServiceCollection services,
        IConfiguration config
    )
    {
        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

        services.AddAuthentication(
            options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }
        );
        var identity = config.GetOptionsExt<IdentitySetting>("Identity");
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(
                options =>
                {
                    options.Authority = identity.Authority;
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateLifetime = true,
                        ValidateIssuer = true,
                        ValidateIssuerSigningKey = true,
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
        if (webCorsSetting.EnforceHttps)
        {
            app.UseHsts();
            app.UseHttpsRedirection();
        }

        return app;
    }
}
