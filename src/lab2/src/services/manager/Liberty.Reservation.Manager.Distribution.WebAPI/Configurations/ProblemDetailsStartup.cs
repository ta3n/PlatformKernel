using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Grpc.Core;
using Liberty.ApplicationShared.Extensions;
using Liberty.ApplicationShared.Settings;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Models.Responses;
using Liberty.SysException;
using Liberty.SysException.Exceptions;
using Microsoft.AspNetCore.Diagnostics;

namespace Liberty.Reservation.Manager.Distribution.WebAPI.Configurations;

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
                    var title = GetTitle(appException);
                    var description = GetDescription(appException);
                    var statusCode = GetStatusCode(appException);
                    var acceptList = context.HttpContext.Request.GetTypedHeaders().Accept;
                    var acceptHeader = string.Join(",", acceptList.Select(h => h.MediaType.Value)).ToUpperInvariant();

                    if (exception is RpcException rpc)
                    {
                        var trailers = rpc.Trailers;

                        errorCode = trailers.GetValue(ErrorConstant.Code) ?? "UPSTREAM_ERROR";
                        errorField = trailers.GetValue(ErrorConstant.Field);
                        title = trailers.GetValue(ErrorConstant.Title) ?? rpc.Status.Detail;
                        description = trailers.GetValue(ErrorConstant.Description) ?? rpc.Status.Detail;
                        statusCode = int.Parse(
                            trailers.GetValue(ErrorConstant.StatusCode) ?? $"{StatusCodes.Status400BadRequest}"
                        );
                    }

                    var baseResponse = new BaseDataResponse<object>
                    {
                        Success = "false",
                        ErrorMsg = $"{title}",
                        Data = null
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
                        dateUtc = $"{DateTimeOffset.UtcNow}"
                    };
                    context.ProblemDetails.Extensions["response"] = baseResponse;
                    context.HttpContext.Response.StatusCode = statusCode;
                    context.HttpContext.Response.ContentType = acceptHeader.Contains("xml") ? "application/xml" : "application/json";

                    if (!acceptHeader.Contains("xml"))
                    {
                        return;
                    }

                    var xmlError = new XmlError { Response = baseResponse };

                    var serializer = new XmlSerializer(xmlError.GetType());
                    using var writer = new Utf8StringWriter();
                    var namespaces = new XmlSerializerNamespaces([XmlQualifiedName.Empty]);
                    serializer.Serialize(writer, xmlError, namespaces);
                    var xml = writer.ToString();
                    context.HttpContext.Response.WriteAsync(xml, Encoding.UTF8);
                };
            }
        );

        return services;
    }

    private static string GetTitle(
        AppException ex
    )
    {
        return ex is AppUnknownErrorException unknown ? unknown.Exception.Message : ex.Title;
    }

    private static string? GetDescription(
        AppException ex
    )
    {
        return ex is AppUnknownErrorException ? null : ex.Message;
    }

    private static int GetStatusCode(
        AppException ex
    )
    {
        return ex switch
        {
            AppInvalidException or ModelException => StatusCodes.Status400BadRequest,
            AppAuthException => StatusCodes.Status401Unauthorized,
            AppForbiddenException => StatusCodes.Status403Forbidden,
            AppNotfoundException => StatusCodes.Status404NotFound,
            _ => StatusCodes.Status500InternalServerError
        };
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

    private sealed class Utf8StringWriter : StringWriter
    {
        public override Encoding Encoding => Encoding.UTF8;
    }
}
