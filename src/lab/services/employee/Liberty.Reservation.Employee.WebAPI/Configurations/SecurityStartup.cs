using Liberty.ApplicationShared.Extensions;
using Liberty.Reservation.Employee.Application.Contexts;
using Liberty.Reservation.Employee.WebAPI.Application.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace Liberty.Reservation.Employee.WebAPI.Configurations;

public static class SecurityStartup
{
    public static IServiceCollection AddSecurityModule(
        this IServiceCollection services,
        IConfiguration config
    )
    {
        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

        services
            .AddIdentity<Employee.Application.Contexts.Entities.Employee, IdentityRole>()
            .AddEntityFrameworkStores<EmployeeDataContext>()
            .AddUserManager<UserManager<Employee.Application.Contexts.Entities.Employee>>()
            .AddDefaultTokenProviders();

        services.AddAuthentication(
            options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }
        );

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(
                options =>
                {
                    // options.Authority = serviceOptions?.IdentityService?.Url;
                    options.RequireHttpsMetadata = false;
                    options.Audience = "personal-data";
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateLifetime = true,
                        ValidateIssuer = true,
                        ValidateIssuerSigningKey = true
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

        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
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

        app.UseCors(
            builder =>
            {
                if (!webCorsSetting.AllowedOrigins!.Equals("*"))
                {
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
                    .WithOrigins(webCorsSetting.AllowedOrigins)
                    .WithMethods(webCorsSetting.AllowedMethods)
                    .WithHeaders(webCorsSetting.AllowedHeaders!)
                    .WithExposedHeaders(webCorsSetting.ExposedHeaders!)
                    .SetPreflightMaxAge(TimeSpan.FromSeconds(webCorsSetting.MaxAge));
            }
        );

        app.UseAuthentication();
        if (webCorsSetting.EnforceHttps)
        {
            app.UseHsts();
            app.UseHttpsRedirection();
        }

        return app;
    }
}
