using Liberty.ApplicationShared.Extensions;
using Liberty.ApplicationShared.Settings;
using Liberty.Reservation.Application.Exceptions;
using Liberty.Reservation.Employee.WebAPI.Application.Localizes;
using Microsoft.AspNetCore.Diagnostics;
using Newtonsoft.Json;
using System.Net;

namespace Liberty.Reservation.Employee.WebAPI.Handlers;

public static class ErrorHandler
{
    public static async Task HandleRequest(
        HttpContext context
    )
    {
        var serviceProvider = context.RequestServices;
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        var appInfo = configuration.GetOptionsExt<AppInfo>("App");

        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>()
                                          ?? throw new NullReferenceException();

        var appException = AppException.GetAppException(exceptionHandlerPathFeature.Error);

        // FIXME: とりあえずエラーログ
        Console.WriteLine(appException.Message);

        // 変換エラーがある可能性があるので、握り潰し処理もしておく
        var title = appException.Title;
        var description = appException.Message;
        try
        {
            var i18NData = I18nE.LocalizedWithData(appException.ErrorCode.ToString());
            if (i18NData is not null)
            {
                title = i18NData.Title;
                description = i18NData.Description;
            }
        }
        catch (Exception)
        {
            // ignored
        }

        // 返却するJSONデータを成形
        var ex = new
        {
            // Exception
            code = appException.ErrorCode.ToString(),
            title,
            description,
            app = new
            {
                name = appInfo.AppName,
                version = appInfo.AppVersion,
                date = $"{DateTime.UtcNow}"
            }
        };
        // json化
        var result = JsonConvert.SerializeObject(ex);

        // ステータスコード  通常は500:InternalServerError

        var statusCode = appException switch
        {
            AppNotfoundException => HttpStatusCode.NotFound,
            AppInvalidException => HttpStatusCode.BadRequest,
            AppAuthException => HttpStatusCode.Unauthorized,
            _ => HttpStatusCode.InternalServerError
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;
        await context.Response.WriteAsync(result);
    }
}
