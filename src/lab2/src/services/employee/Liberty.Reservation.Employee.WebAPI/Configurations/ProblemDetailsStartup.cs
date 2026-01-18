using System.ComponentModel.DataAnnotations;
using Liberty.ApplicationShared.Extensions;
using Liberty.ApplicationShared.Settings;
using Liberty.SysException.Exceptions;
using Microsoft.AspNetCore.Diagnostics;

namespace Liberty.Reservation.Employee.WebAPI.Configurations;

public static class ProblemDetailsStartup
{
    public static IServiceCollection AddProblemDetailsModule(
        this IServiceCollection services
    )
    {
        services.AddProblemDetails(
            config =>
            {
                config.CustomizeProblemDetails = context =>
                {
                    var serviceProvider = context.HttpContext.RequestServices;
                    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
                    var appInfo = configuration.GetOptionsExt<AppInfo>("App");

                    var exception = context.HttpContext.Features.Get<IExceptionHandlerFeature>()?.Error;
                    if (exception is null)
                    {
                        return;
                    }

                    var appException = AppException.GetAppException(exception);

                    var errorCode = appException.ErrorCode.ToString();
                    var errorField = appException.ErrorField;
                    var title = appException is AppUnknownErrorException errorException
                        ? errorException.Exception.Message
                        : appException.Title;
                    var description = appException is AppUnknownErrorException
                        ? null
                        : appException.Message;
                    var statusCode = appException switch
                    {
                        AppNotfoundException => StatusCodes.Status404NotFound,
                        AppInvalidException => StatusCodes.Status400BadRequest,
                        ModelException => StatusCodes.Status400BadRequest,
                        AppAuthException => StatusCodes.Status401Unauthorized,
                        AppUnknownErrorException when exception is ValidationException => StatusCodes.Status400BadRequest,
                        _ => StatusCodes.Status500InternalServerError
                    };

                    context.ProblemDetails.Title = title;
                    context.ProblemDetails.Status = statusCode;
                    context.ProblemDetails.Detail = description;
                    context.ProblemDetails.Extensions["code"] = errorCode;
                    context.ProblemDetails.Extensions["field"] = errorField;
                    context.ProblemDetails.Extensions["app"] = new
                    {
                        name = appInfo.AppName,
                        version = appInfo.AppVersion,
                        dateUtc = $"{DateTime.UtcNow}"
                    };

                    context.HttpContext.Response.StatusCode = statusCode;
                    context.HttpContext.Response.ContentType = "application/json";
                };
            }
        );

        return services;
    }

    public static IApplicationBuilder UseApplicationProblemDetails(
        this IApplicationBuilder app,
        IHostEnvironment environment
    )
    {
        app.UseExceptionHandler();

        app.UseStatusCodePages();

        if (environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        return app;
    }
}
